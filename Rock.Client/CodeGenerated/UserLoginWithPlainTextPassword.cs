//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
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


namespace Rock.Client
{
    /// <summary>
    /// Use Rock.Client.UserLoginWithPlainTextPassword and set PlainTextPassword to set a new password as part of a api/UserLogins POST or PUT
    /// </summary>
    public partial class UserLoginWithPlainTextPasswordEntity
    {
        /// <summary />
        public int Id { get; set; }

        /// <summary />
        public string ApiKey { get; set; }

        /// <summary />
        public int? EntityTypeId { get; set; }

        /// <summary />
        public int? FailedPasswordAttemptCount { get; set; }

        /// <summary />
        public DateTime? FailedPasswordAttemptWindowStartDateTime { get; set; }

        /// <summary />
        public bool? IsConfirmed { get; set; }

        /// <summary />
        public bool? IsLockedOut { get; set; }

        /// <summary />
        public bool? IsOnLine { get; set; }

        /// <summary />
        public DateTime? LastActivityDateTime { get; set; }

        /// <summary />
        public DateTime? LastLockedOutDateTime { get; set; }

        /// <summary />
        public DateTime? LastLoginDateTime { get; set; }

        /// <summary />
        public DateTime? LastPasswordChangedDateTime { get; set; }

        /// <summary />
        public DateTime? LastPasswordExpirationWarningDateTime { get; set; }

        /// <summary />
        public string Password { get; set; }

        /// <summary />
        public int? PersonId { get; set; }

        /// <summary />
        public string PlainTextPassword { get; set; }

        /// <summary />
        public string UserName { get; set; }

        /// <summary />
        public Guid Guid { get; set; }

        /// <summary />
        public string ForeignId { get; set; }

    }

    /// <summary>
    /// Use Rock.Client.UserLoginWithPlainTextPassword and set PlainTextPassword to set a new password as part of a api/UserLogins POST or PUT
    /// </summary>
    public partial class UserLoginWithPlainTextPassword : UserLoginWithPlainTextPasswordEntity
    {
        /// <summary />
        public EntityType EntityType { get; set; }

        /// <summary />
        public DateTime? CreatedDateTime { get; set; }

        /// <summary />
        public DateTime? ModifiedDateTime { get; set; }

        /// <summary />
        public int? CreatedByPersonAliasId { get; set; }

        /// <summary />
        public int? ModifiedByPersonAliasId { get; set; }

        /// <summary>
        /// NOTE: Attributes are only populated when ?loadAttributes is specified. Options for loadAttributes are true, false, 'simple', 'expanded' 
        /// </summary>
        public Dictionary<string, Rock.Client.Attribute> Attributes { get; set; }

        /// <summary>
        /// NOTE: AttributeValues are only populated when ?loadAttributes is specified. Options for loadAttributes are true, false, 'simple', 'expanded' 
        /// </summary>
        public Dictionary<string, Rock.Client.AttributeValue> AttributeValues { get; set; }
    }
}
