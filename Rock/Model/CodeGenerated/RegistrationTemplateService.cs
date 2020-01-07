//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright>
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
using System.Linq;

using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// RegistrationTemplate Service class
    /// </summary>
    public partial class RegistrationTemplateService : Service<RegistrationTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationTemplateService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public RegistrationTemplateService(RockContext context) : base(context)
        {
        }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if this instance can delete the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDelete( RegistrationTemplate item, out string errorMessage )
        {
            errorMessage = string.Empty;
 
            if ( new Service<RegistrationTemplatePlacement>( Context ).Queryable().Any( a => a.RegistrationTemplateId == item.Id ) )
            {
                errorMessage = string.Format( "This {0} is assigned to a {1}.", RegistrationTemplate.FriendlyTypeName, RegistrationTemplatePlacement.FriendlyTypeName );
                return false;
            }  
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class RegistrationTemplateExtensionMethods
    {
        /// <summary>
        /// Clones this RegistrationTemplate object to a new RegistrationTemplate object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static RegistrationTemplate Clone( this RegistrationTemplate source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as RegistrationTemplate;
            }
            else
            {
                var target = new RegistrationTemplate();
                target.CopyPropertiesFrom( source );
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another RegistrationTemplate object to this RegistrationTemplate object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom( this RegistrationTemplate target, RegistrationTemplate source )
        {
            target.Id = source.Id;
            target.AddPersonNote = source.AddPersonNote;
            target.AllowExternalRegistrationUpdates = source.AllowExternalRegistrationUpdates;
            target.AllowGroupPlacement = source.AllowGroupPlacement;
            target.AllowMultipleRegistrants = source.AllowMultipleRegistrants;
            target.BatchNamePrefix = source.BatchNamePrefix;
            target.CategoryId = source.CategoryId;
            target.ConfirmationEmailTemplate = source.ConfirmationEmailTemplate;
            target.ConfirmationFromEmail = source.ConfirmationFromEmail;
            target.ConfirmationFromName = source.ConfirmationFromName;
            target.ConfirmationSubject = source.ConfirmationSubject;
            target.Cost = source.Cost;
            target.DefaultPayment = source.DefaultPayment;
            target.Description = source.Description;
            target.DiscountCodeTerm = source.DiscountCodeTerm;
            target.FeeTerm = source.FeeTerm;
            target.FinancialGatewayId = source.FinancialGatewayId;
            target.ForeignGuid = source.ForeignGuid;
            target.ForeignKey = source.ForeignKey;
            target.GroupMemberRoleId = source.GroupMemberRoleId;
            target.GroupMemberStatus = source.GroupMemberStatus;
            target.GroupTypeId = source.GroupTypeId;
            target.IsActive = source.IsActive;
            target.LoginRequired = source.LoginRequired;
            target.MaxRegistrants = source.MaxRegistrants;
            target.MinimumInitialPayment = source.MinimumInitialPayment;
            target.Name = source.Name;
            target.Notify = source.Notify;
            target.PaymentReminderEmailTemplate = source.PaymentReminderEmailTemplate;
            target.PaymentReminderFromEmail = source.PaymentReminderFromEmail;
            target.PaymentReminderFromName = source.PaymentReminderFromName;
            target.PaymentReminderSubject = source.PaymentReminderSubject;
            target.PaymentReminderTimeSpan = source.PaymentReminderTimeSpan;
            target.RegistrantsSameFamily = source.RegistrantsSameFamily;
            target.RegistrantTerm = source.RegistrantTerm;
            target.RegistrarOption = source.RegistrarOption;
            target.RegistrationAttributeTitleEnd = source.RegistrationAttributeTitleEnd;
            target.RegistrationAttributeTitleStart = source.RegistrationAttributeTitleStart;
            target.RegistrationInstructions = source.RegistrationInstructions;
            target.RegistrationTerm = source.RegistrationTerm;
            target.RegistrationWorkflowTypeId = source.RegistrationWorkflowTypeId;
            target.ReminderEmailTemplate = source.ReminderEmailTemplate;
            target.ReminderFromEmail = source.ReminderFromEmail;
            target.ReminderFromName = source.ReminderFromName;
            target.ReminderSubject = source.ReminderSubject;
            target.RequestEntryName = source.RequestEntryName;
            target.RequiredSignatureDocumentTemplateId = source.RequiredSignatureDocumentTemplateId;
            target.SetCostOnInstance = source.SetCostOnInstance;
            target.ShowCurrentFamilyMembers = source.ShowCurrentFamilyMembers;
            target.SignatureDocumentAction = source.SignatureDocumentAction;
            target.SuccessText = source.SuccessText;
            target.SuccessTitle = source.SuccessTitle;
            target.WaitListEnabled = source.WaitListEnabled;
            target.WaitListTransitionEmailTemplate = source.WaitListTransitionEmailTemplate;
            target.WaitListTransitionFromEmail = source.WaitListTransitionFromEmail;
            target.WaitListTransitionFromName = source.WaitListTransitionFromName;
            target.WaitListTransitionSubject = source.WaitListTransitionSubject;
            target.CreatedDateTime = source.CreatedDateTime;
            target.ModifiedDateTime = source.ModifiedDateTime;
            target.CreatedByPersonAliasId = source.CreatedByPersonAliasId;
            target.ModifiedByPersonAliasId = source.ModifiedByPersonAliasId;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;

        }
    }
}
