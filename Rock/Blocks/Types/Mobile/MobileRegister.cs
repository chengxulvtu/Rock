﻿using System;
using System.ComponentModel;

using Rock.Attribute;
using Rock.Data;
using Rock.Mobile.Common.Blocks.RegisterAccount;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Blocks.Types.Mobile
{
    [DisplayName( "Mobile Register" )]
    [Category( "Mobile" )]
    [Description( "Allows user to register a new account on mobile application." )]
    [IconCssClass( "fa fa-user-plus" )]

    #region Block Attributes

    [DefinedValueField(
        "Connection Status",
        Key = AttributeKeys.ConnectionStatus,
        Description = "The connection status to use for new individuals (default = 'Web Prospect'.)",
        DefinedTypeGuid = "2E6540EA-63F0-40FE-BE50-F2A84735E600",
        IsRequired = true,
        AllowMultiple = false,
        DefaultValue = "368DD475-242C-49C4-A42C-7278BE690CC2",
        Order = 11 )]

    [DefinedValueField(
        "Record Status",
        Key = AttributeKeys.RecordStatus,
        Description = "The record status to use for new individuals (default = 'Pending'.)",
        DefinedTypeGuid = "8522BADD-2871-45A5-81DD-C76DA07E2E7E",
        IsRequired = true,
        AllowMultiple = false,
        DefaultValue = "283999EC-7346-42E3-B807-BCE9B2BABB49",
        Order = 12 )]

    [BooleanField(
        "Birthdate Show",
        Key = AttributeKeys.BirthDateShow,
        Description = "Determines whether the birthdate field will be available for input.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Category = "custommobile",
        Order = 0 )]

    [BooleanField(
        "BirthDate Required",
        Key = AttributeKeys.BirthDateRequired,
        Description = "Requires that a birthdate value be entered before allowing the user to register.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Category = "custommobile",
        Order = 1 )]

    [BooleanField(
        "Campus Show",
        Key = AttributeKeys.CampusShow,
        Description = "Determines whether the campus field will be available for input.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Category = "custommobile",
        Order = 2 )]

    [BooleanField(
        "Campus Required",
        Key = AttributeKeys.CampusRequired,
        Description = "Requires that a campus value be entered before allowing the user to register.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Category = "custommobile",
        Order = 3 )]

    [BooleanField(
        "Email Show",
        Key = AttributeKeys.EmailShow,
        Description = "Determines whether the email field will be available for input.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Category = "custommobile",
        Order = 4 )]

    [BooleanField(
        "Email Required",
        Key = AttributeKeys.EmailRequired,
        Description = "Requires that a email value be entered before allowing the user to register.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Category = "custommobile",
        Order = 5 )]

    [BooleanField(
        "Mobile Phone Show",
        Key = AttributeKeys.MobilePhoneShow,
        Description = "Determines whether the mobile phone field will be available for input.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Category = "custommobile",
        Order = 6 )]

    [BooleanField(
        "Mobile Phone Required",
        Key = AttributeKeys.MobilePhoneRequired,
        Description = "Requires that a mobile phone value be entered before allowing the user to register.",
        IsRequired = true,
        DefaultBooleanValue = true,
        Category = "custommobile",
        Order = 7 )]

    #endregion

    public class MobileRegister : RockBlockType, IRockMobileBlockType
    {
        public static class AttributeKeys
        {
            public const string ConnectionStatus = "ConnectionStatus";
            public const string RecordStatus = "RecordStatus";

            public const string BirthDateShow = "BirthDateShow";
            public const string BirthDateRequired = "BirthDateRequired";
            public const string CampusShow = "CampusShow";
            public const string CampusRequired = "CampusRequired";
            public const string EmailShow = "EmailShow";
            public const string EmailRequired = "EmailRequired";
            public const string MobilePhoneShow = "MobilePhoneShow";
            public const string MobilePhoneRequired = "MobilePhoneRequired";
        }

        #region IRockMobileBlockType Implementation

        /// <summary>
        /// Gets the required mobile application binary interface version required to render this block.
        /// </summary>
        /// <value>
        /// The required mobile application binary interface version required to render this block.
        /// </value>
        int IRockMobileBlockType.RequiredMobileAbiVersion => 1;

        /// <summary>
        /// Gets the class name of the mobile block to use during rendering on the device.
        /// </summary>
        /// <value>
        /// The class name of the mobile block to use during rendering on the device
        /// </value>
        string IRockMobileBlockType.MobileBlockType => "Rock.Mobile.Blocks.RegisterAccount";

        /// <summary>
        /// Gets the property values that will be sent to the device in the application bundle.
        /// </summary>
        /// <returns>
        /// A collection of string/object pairs.
        /// </returns>
        object IRockMobileBlockType.GetMobileConfigurationValues()
        {
            return new
            {
            };
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="account">The account data.</param>
        /// <returns></returns>
        [BlockAction]
        public object RegisterUser( AccountData account )
        {
            if ( account.Username.IsNullOrWhiteSpace() || account.FirstName.IsNullOrWhiteSpace() || account.LastName.IsNullOrWhiteSpace() )
            {
                return ActionBadRequest( "Missing required information." );
            }

            if ( !UserLoginService.IsPasswordValid( account.Password ) )
            {
                return ActionBadRequest( UserLoginService.FriendlyPasswordRules() );
            }

            var userLoginService = new UserLoginService( new RockContext() );
            var userLogin = userLoginService.GetByUserName( account.Username );
            if ( userLogin != null )
            {
                return ActionBadRequest( "Username already exists." );
            }

            // TODO: Do we need to do duplicate matching? -dsh
            var person = CreateUser( CreatePerson( account ), account, true );

            return ActionOk();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the person.
        /// </summary>
        /// <param name="account">The account details.</param>
        /// <returns></returns>
        private Person CreatePerson( AccountData account )
        {
            var rockContext = new RockContext();

            DefinedValueCache dvcConnectionStatus = DefinedValueCache.Get( GetAttributeValue( AttributeKeys.ConnectionStatus ).AsGuid() );
            DefinedValueCache dvcRecordStatus = DefinedValueCache.Get( GetAttributeValue( AttributeKeys.RecordStatus ).AsGuid() );

            Person person = new Person
            {
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                Gender = ( Gender ) account.Gender,
                IsEmailActive = true,
                EmailPreference = EmailPreference.EmailAllowed,
                RecordTypeValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id
            };

            if ( dvcConnectionStatus != null )
            {
                person.ConnectionStatusValueId = dvcConnectionStatus.Id;
            }

            if ( dvcRecordStatus != null )
            {
                person.RecordStatusValueId = dvcRecordStatus.Id;
            }

            if ( account.BirthDate.HasValue )
            {
                person.BirthMonth = account.BirthDate.Value.Month;
                person.BirthDay = account.BirthDate.Value.Day;
                if ( account.BirthDate.Value.Year != DateTime.MinValue.Year )
                {
                    person.BirthYear = account.BirthDate.Value.Year;
                }
            }

            if ( !string.IsNullOrWhiteSpace( PhoneNumber.CleanNumber( account.MobilePhone ) ) )
            {
                int phoneNumberTypeId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid() ).Id;

                var phoneNumber = new PhoneNumber
                {
                    NumberTypeValueId = phoneNumberTypeId,
                    Number = PhoneNumber.CleanNumber( account.MobilePhone )
                };
                person.PhoneNumbers.Add( phoneNumber );

                // TODO: Do we need to deal with this? -dsh
                //phoneNumber.CountryCode = PhoneNumber.CleanNumber( pnbPhone.CountryCode );

                // TODO: How to deal with SMS enabled option? -dsh
                phoneNumber.IsMessagingEnabled = false;
            }

            int? campusId = null;

            if ( account.Campus.HasValue )
            {
                campusId = CampusCache.Get( account.Campus.Value ).Id;
            }

            PersonService.SaveNewPerson( person, rockContext, campusId, false );

            return person;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <param name="account">The account details.</param>
        /// <param name="confirmed">if set to <c>true</c> [confirmed].</param>
        /// <returns></returns>
        private UserLogin CreateUser( Person person, AccountData account, bool confirmed )
        {
            var rockContext = new RockContext();
            var userLoginService = new UserLoginService( rockContext );

            return UserLoginService.Create(
                rockContext,
                person,
                AuthenticationServiceType.Internal,
                EntityTypeCache.Get( Rock.SystemGuid.EntityType.AUTHENTICATION_DATABASE.AsGuid() ).Id,
                account.Username,
                account.Password,
                confirmed );
        }

        #endregion
    }
}
