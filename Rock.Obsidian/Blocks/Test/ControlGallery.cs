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
using System.ComponentModel;
using Rock.Attribute;
using Rock.Model;

namespace Rock.Obsidian.Blocks.Test
{
    /// <summary>
    /// Allows the user to authenticate.
    /// </summary>
    /// <seealso cref="Rock.Blocks.ObsidianBlockType" />

    [DisplayName( "Control Gallery" )]
    [Category( "Obsidian > Test" )]
    [Description( "Allows the user to try out various controls." )]
    [IconCssClass( "fa fa-flask" )]

    public class ControlGallery : ObsidianBlockType
    {
        #region IObsidianBlockType Implementation

        /// <summary>
        /// Gets the required Obsidian interface version.
        /// </summary>
        /// <value>
        /// The required obsidian interface version.
        /// </value>
        public override int RequiredObsidianVersion => 1;

        /// <summary>
        /// Gets the block markup file identifier.
        /// </summary>
        /// <value>
        /// The block markup file identifier.
        /// </value>
        public override string BlockMarkupFileIdentifier => "Test/ControlGallery";

        /// <summary>
        /// Gets the property values that will be sent to the browser.
        /// </summary>
        /// <returns>
        /// A collection of string/object pairs.
        /// </returns>
        public override object GetConfigurationValues()
        {
            return new { };
        }

        #endregion
    }
}
