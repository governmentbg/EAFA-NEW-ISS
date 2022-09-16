using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class LegalsData
    {
        public static List<Legal> Legals
        {
            get
            {
                return new List<Legal>
                {
                    new Legal
                    {
                        Id = 1,
                        Eik = "1111",
                        Name = "Юридическо лице 1",
                        ValidFrom = new DateTime(2021, 5, 1),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        RecordType = "Application"
                    },
                    new Legal
                    {
                        Id = 2,
                        Eik = "2222",
                        Name = "Юридическо лице 2",
                        ValidFrom = new DateTime(2020, 3, 1),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        RecordType = "Application"
                    },
                    new Legal
                    {
                        Id = 3,
                        Eik = "3333",
                        Name = "Юридическо лице 3",
                        ValidFrom = new DateTime(2021, 6, 1),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        RecordType = "Application"
                    },
                    new Legal
                    {
                        Id = 4,
                        Eik = "4444",
                        Name = "Юридическо лице 4",
                        ValidFrom = new DateTime(2021, 4, 1),
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                        RecordType = "Application"
                    }
                };
            }
        }

        public static List<LegalsAddress> LegalAddresses
        {
            get
            {
                return new List<LegalsAddress>
                {
                    new LegalsAddress
                    {
                        Id = 1,
                        LegalId = Legals[0].Id,
                        AddressId = AddressData.Addresses[4].Id,
                        AddressTypeId = AddressData.AddressTypes[0].Id
                    },
                    new LegalsAddress
                    {
                        Id = 2,
                        LegalId = Legals[0].Id,
                        AddressId = AddressData.Addresses[3].Id,
                        AddressTypeId = AddressData.AddressTypes[1].Id
                    },
                    new LegalsAddress
                    {
                        Id = 3,
                        LegalId = Legals[1].Id,
                        AddressId = AddressData.Addresses[2].Id,
                        AddressTypeId = AddressData.AddressTypes[0].Id
                    },
                    new LegalsAddress
                    {
                        Id = 4,
                        LegalId = Legals[2].Id,
                        AddressId = AddressData.Addresses[1].Id,
                        AddressTypeId = AddressData.AddressTypes[0].Id
                    }
                };
            }
        }

        public static List<EmailAddress> EmailAddresses
        {
            get
            {
                return new List<EmailAddress>
                {
                    new EmailAddress { Id = 7, Email = "email1@email.com" },
                    new EmailAddress { Id = 8, Email = "email2@email.com" },
                    new EmailAddress { Id = 9, Email = "email3@email.com" },
                    new EmailAddress { Id = 10, Email = "email4@email.com" }
                };
            }
        }

        public static List<LegalEmailAddress> LegalEmailAddresses
        {
            get
            {
                return new List<LegalEmailAddress>
                {
                    new LegalEmailAddress { Id = 1, LegalId = Legals[0].Id, EmailAddressId = EmailAddresses[0].Id },
                    new LegalEmailAddress { Id = 2, LegalId = Legals[1].Id, EmailAddressId = EmailAddresses[1].Id },
                    new LegalEmailAddress { Id = 3, LegalId = Legals[2].Id, EmailAddressId = EmailAddresses[2].Id },
                    new LegalEmailAddress { Id = 4, LegalId = Legals[3].Id, EmailAddressId = EmailAddresses[3].Id }
                };
            }
        }

        public static List<PhoneNumber> PhoneNumbers
        {
            get
            {
                return new List<PhoneNumber>
                {
                    new PhoneNumber { Id = 6, Phone = "0887397551" },
                    new PhoneNumber { Id = 7, Phone = "0887397552" },
                    new PhoneNumber { Id = 8, Phone = "0887397553" },
                    new PhoneNumber { Id = 9, Phone = "0887397554" }
                };
            }
        }

        public static List<LegalPhoneNumber> LegalPhoneNumbers
        {
            get
            {
                return new List<LegalPhoneNumber>
                {
                    new LegalPhoneNumber { Id = 1, LegalId = Legals[0].Id, PhoneId = PhoneNumbers[0].Id },
                    new LegalPhoneNumber { Id = 2, LegalId = Legals[1].Id, PhoneId = PhoneNumbers[1].Id },
                    new LegalPhoneNumber { Id = 3, LegalId = Legals[2].Id, PhoneId = PhoneNumbers[2].Id },
                    new LegalPhoneNumber { Id = 4, LegalId = Legals[3].Id, PhoneId = PhoneNumbers[3].Id }
                };
            }
        }
    }
}
