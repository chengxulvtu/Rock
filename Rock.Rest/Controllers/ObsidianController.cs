﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;

using Newtonsoft.Json.Linq;

using Rock.Model;
using Rock.Rest.Filters;
using Rock.Web.Cache;

namespace Rock.Rest.Controllers
{
    /// <summary>
    /// Obsidian Controller
    /// </summary>
    public class ObsidianController: ApiControllerBase
    {
        /// <summary>
        /// Executes an action for a Rock Control.
        /// </summary>
        /// <param name="controlType">Type of the control.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [Authenticate]
        [Route( "api/obsidian/control/{controlType}/{actionName}" )]
        public IHttpActionResult ControlAction( string controlType, string actionName, [FromBody] JToken parameters )
        {
            try
            {
                return ProcessControlAction( Request.Method.ToString(), controlType, actionName, parameters );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        /// <summary>
        /// Processes the action.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <param name="controlType">Type of the control.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private IHttpActionResult ProcessControlAction( string verb, string controlType, string actionName, JToken parameters )
        {
            // Get the authenticated person
            var person = GetPerson();
            HttpContext.Current.AddOrReplaceItem( "CurrentPerson", person );

            // Get the class that handles the logic for the control.
            var blockCompiledType = blockCache.BlockType.GetCompiledType();
            var block = Activator.CreateInstance( blockCompiledType );

            if ( !( block is Blocks.IRockBlockType rockBlock ) )
            {
                return NotFound();
            }

            //
            // Set the basic block parameters.
            //
            rockBlock.BlockCache = blockCache;
            rockBlock.PageCache = pageCache;
            rockBlock.RequestContext = new Net.RockRequestContext( Request );

            var actionParameters = new Dictionary<string, JToken>();

            //
            // Parse any posted parameter data.
            //
            if ( parameters != null )
            {
                try
                {
                    foreach ( var kvp in parameters.ToObject<Dictionary<string, JToken>>() )
                    {
                        actionParameters.AddOrReplace( kvp.Key, kvp.Value );
                    }
                }
                catch
                {
                    return BadRequest( "Invalid parameter data." );
                }
            }

            //
            // Parse any query string parameter data.
            //
            foreach ( var q in Request.GetQueryNameValuePairs() )
            {
                actionParameters.AddOrReplace( q.Key, JToken.FromObject( q.Value.ToString() ) );
            }

            return InvokeAction( rockBlock, verb, actionName, actionParameters );
        }

        /// <summary>
        /// Processes the specified block action.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="verb">The HTTP Method Verb that was used for the request.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="actionParameters">The action parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// actionName
        /// or
        /// actionData
        /// </exception>
        private IHttpActionResult InvokeAction( Blocks.IRockBlockType block, string verb, string actionName, Dictionary<string, JToken> actionParameters )
        {
            MethodInfo action;

            //
            // Find the action they requested.
            //
            action = block.GetType().GetMethods( BindingFlags.Instance | BindingFlags.Public )
                .SingleOrDefault( m => m.GetCustomAttribute<Blocks.BlockActionAttribute>()?.ActionName == actionName );

            if ( action == null )
            {
                return NotFound();
            }

            var methodParameters = action.GetParameters();
            var parameters = new List<object>();

            //
            // Go through each parameter and convert it to the proper type.
            //
            for ( int i = 0; i < methodParameters.Length; i++ )
            {
                var key = actionParameters.Keys.SingleOrDefault( k => k.ToLowerInvariant() == methodParameters[i].Name.ToLower() );

                if ( key != null )
                {
                    try
                    {
                        //
                        // If the target type is nullable and the action parameter is an empty
                        // string then consider it null. A GET query cannot have null values.
                        //
                        if ( Nullable.GetUnderlyingType( methodParameters[i].ParameterType ) != null )
                        {
                            if ( actionParameters[key].Type == JTokenType.String && actionParameters[key].ToString() == string.Empty )
                            {
                                parameters.Add( null );

                                continue;
                            }
                        }

                        parameters.Add( actionParameters[key].ToObject( methodParameters[i].ParameterType ) );
                    }
                    catch
                    {
                        return BadRequest( $"Parameter type mismatch for '{methodParameters[i].Name}'." );
                    }
                }
                else if ( methodParameters[i].IsOptional )
                {
                    parameters.Add( Type.Missing );
                }
                else
                {
                    return BadRequest( $"Parameter '{methodParameters[i].Name}' is required." );
                }
            }

            object result;
            try
            {
                result = action.Invoke( block, parameters.ToArray() );
            }
            catch ( TargetInvocationException ex )
            {
                ExceptionLogService.LogApiException( ex.InnerException, Request, GetPersonAlias() );
                result = new Rock.Blocks.BlockActionResult( HttpStatusCode.InternalServerError );
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogApiException( ex, Request, GetPersonAlias() );
                result = new Rock.Blocks.BlockActionResult( HttpStatusCode.InternalServerError );
            }

            //
            // Handle the result type.
            //
            if ( result is IHttpActionResult )
            {
                return ( IHttpActionResult ) result;
            }
            else if ( result is Rock.Blocks.BlockActionResult actionResult )
            {
                if ( actionResult.Error != null )
                {
                    return Content( actionResult.StatusCode, new HttpError( actionResult.Error ) );
                }
                else if ( actionResult.Content is HttpContent httpContent )
                {
                    var response = Request.CreateResponse( actionResult.StatusCode );
                    response.Content = httpContent;
                    return new System.Web.Http.Results.ResponseMessageResult( response );
                }
                else if ( actionResult.ContentClrType != null )
                {
                    var genericType = typeof( System.Web.Http.Results.NegotiatedContentResult<> ).MakeGenericType( actionResult.ContentClrType );
                    return ( IHttpActionResult ) Activator.CreateInstance( genericType, actionResult.StatusCode, actionResult.Content, this );
                }
                else
                {
                    return StatusCode( actionResult.StatusCode );
                }
            }
            else if ( action.ReturnType == typeof(void))
            {
                return Ok();
            }
            else
            {
                return Ok( result );
            }
        }
    }
}
