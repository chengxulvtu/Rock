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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Rock.Blocks;
using Rock.Mobile;

namespace Rock.Obsidian.Blocks
{
    /// <summary>
    /// Obsidian Block Type
    /// </summary>
    /// <seealso cref="Rock.Blocks.RockBlockType" />
    /// <seealso cref="IObsidianBlockType" />
    public abstract class ObsidianBlockType : RockBlockType, IObsidianBlockType
    {
        #region Properties

        /// <summary>
        /// Gets the required Obsidian interface version.
        /// </summary>
        /// <value>
        /// The required obsidian interface version.
        /// </value>
        public abstract int RequiredObsidianVersion { get; }

        /// <summary>
        /// Gets the block markup file identifier.
        /// </summary>
        /// <value>
        /// The block markup file identifier.
        /// </value>
        public abstract string BlockMarkupFileIdentifier { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the property values that will be sent to the browser.
        /// </summary>
        /// <returns>
        /// A collection of string/object pairs.
        /// </returns>
        public abstract object GetConfigurationValues();

        /// <summary>
        /// Gets the additional settings defined for this block instance.
        /// </summary>
        /// <returns>An AdditionalBlockSettings object.</returns>
        public AdditionalBlockSettings GetAdditionalSettings()
        {
            return BlockCache?.AdditionalSettings.FromJsonOrNull<AdditionalBlockSettings>() ?? new AdditionalBlockSettings();
        }

        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <returns></returns>
        public override string GetControlMarkup()
        {
            var instanceId = $"obsidian-{this.BlockCache.Guid}";

            return
$@"<div id=""{instanceId}""></div>
<script type=""text/javascript"">
    Obsidian.Blocks['{BlockMarkupFileIdentifier}'] = {{
        instanceId: '{instanceId}',
        pageId: {BlockCache.PageId},
        blockId: {BlockCache.Id},
        additionalSettingsJson: {ConvertObjectToJavaScript( GetAdditionalSettings() )},
        configurationValuesJson: {ConvertObjectToJavaScript( GetConfigurationValues() )},
        blockAction: Obsidian.Util.getBlockActionFunction({{
            pageId: {BlockCache.PageId},
            blockId: {BlockCache.Id}
        }})
    }};

    new Vue({{
        el: '#{instanceId}',
        components: {{
            block: Obsidian.Util.loadVueFile('/Obsidian/Blocks/{BlockMarkupFileIdentifier}.vue')
        }},
        provide: {{
            context: Obsidian.Blocks['{BlockMarkupFileIdentifier}']
        }},
        template: '<block id=""{instanceId}"" />'
    }});
</script>";
        }

        #endregion Methods

        #region JavaScript Helpers

        /// <summary>
        /// Gets the javascript object literal.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        private static string ConvertObjectToJavaScript( object source )
        {
            if ( source == null )
            {
                return "null";
            }

            var bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

            var dictionary = source.GetType().GetProperties( bindingFlags ).ToDictionary
            (
                pi => pi.Name,
                pi => pi.GetValue( source, null )
            );

            return ConvertDictionaryToJavaScript( dictionary );
        }

        /// <summary>
        /// Gets the javascript object literal.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        private static string ConvertDictionaryToJavaScript( IDictionary<string, object> dictionary )
        {
            if ( dictionary == null )
            {
                return "null";
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append( "{" );

            var keyCount = dictionary.Keys.Count;
            var index = 0;

            foreach ( var key in dictionary.Keys )
            {
                var value = dictionary[key];
                var isLast = index == ( keyCount - 1 );

                stringBuilder.Append( $"'{key}': " );
                stringBuilder.Append( value == null ? "null" : $"'{value.ToString()}'" );

                if ( !isLast )
                {
                    stringBuilder.Append( "," );
                }

                index++;
            }

            stringBuilder.Append( "}" );
            return stringBuilder.ToString();
        }

        #endregion JavaScript Helpers
    }
}
