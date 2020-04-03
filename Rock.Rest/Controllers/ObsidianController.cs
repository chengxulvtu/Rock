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
using Rock.Obsidian.Controls;
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
        /// <param name="controlTypeName">Type of the control.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private IHttpActionResult ProcessControlAction( string verb, string controlTypeName, string actionName, JToken parameters )
        {
            // Get the authenticated person
            var person = GetPerson();
            HttpContext.Current.AddOrReplaceItem( "CurrentPerson", person );

            // Get the class that handles the logic for the control
            var controlCompiledType = ObsidianControlsContainer.Get( controlTypeName );
            var controlInstance = Activator.CreateInstance( controlCompiledType ) as IObsidianControl;

            if ( controlInstance == null )
            {
                return NotFound();
            }

            // Parse any posted parameter data
            var actionParameters = new Dictionary<string, JToken>();

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

            // Parse any query string parameter data.
            foreach ( var q in Request.GetQueryNameValuePairs() )
            {
                actionParameters.AddOrReplace( q.Key, JToken.FromObject( q.Value.ToString() ) );
            }

            return InvokeAction( controlInstance, verb, actionName, actionParameters );
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="actionParameters">The action parameters.</param>
        /// <returns></returns>
        private IHttpActionResult InvokeAction( IObsidianControl control, string verb, string actionName, Dictionary<string, JToken> actionParameters )
        {
            var action = control.GetType().GetMethods( BindingFlags.Instance | BindingFlags.Public )
                .SingleOrDefault( m => m.GetCustomAttribute<ControlActionAttribute>()?.ActionName == actionName );

            if ( action == null )
            {
                return NotFound();
            }

            var methodParameters = action.GetParameters();
            var parameters = new List<object>();

            // Go through each parameter and convert it to the proper type
            for ( int i = 0; i < methodParameters.Length; i++ )
            {
                var key = actionParameters.Keys.SingleOrDefault( k => k.ToLowerInvariant() == methodParameters[i].Name.ToLower() );

                if ( key != null )
                {
                    try
                    {
                        // If the target type is nullable and the action parameter is an empty
                        // string then consider it null. A GET query cannot have null values
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
                result = action.Invoke( control, parameters.ToArray() );
            }
            catch ( TargetInvocationException ex )
            {
                ExceptionLogService.LogApiException( ex.InnerException, Request, GetPersonAlias() );
                result = new ControlActionResult( HttpStatusCode.InternalServerError );
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogApiException( ex, Request, GetPersonAlias() );
                result = new ControlActionResult( HttpStatusCode.InternalServerError );
            }

            // Handle the result type
            if ( result is IHttpActionResult )
            {
                return ( IHttpActionResult ) result;
            }
            else if ( result is ControlActionResult actionResult )
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
