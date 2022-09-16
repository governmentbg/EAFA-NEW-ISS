using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class PersonsData
    {
        public static List<Person> Persons
        {
            get
            {
                return new List<Person>
                {
                    new Person
                    {
                        Id = 1,
                        FirstName = "Константин",
                        MiddleName = "Иванов",
                        LastName = "Славчев",
                        EgnLnc = "7607179487",
                        CitizenshipCountryId = AddressData.Countries[0].Id,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1996, 8, 18)
                    },
                    new Person
                    {
                        Id = 2,
                        FirstName = "Димитър",
                        MiddleName = null,
                        LastName = "Андонов",
                        EgnLnc = "4111123456",
                        CitizenshipCountryId = AddressData.Countries[1].Id,
                        HasBulgarianAddressRegistration = false,
                        BirthDate = new DateTime(1970, 6, 14)
                    },
                    new Person
                    {
                        Id = 3,
                        FirstName = "Галина",
                        MiddleName = "Иванова",
                        LastName = "Пенева",
                        EgnLnc = "7306287811",
                        CitizenshipCountryId = AddressData.Countries[2].Id,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1971, 10, 31)
                    },
                    new Person
                    {
                        Id = 4,
                        FirstName = "Добрин",
                        MiddleName = "Атанасов",
                        LastName = "Петров",
                        EgnLnc = "7504259733",
                        CitizenshipCountryId = null,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(2000, 1, 1)
                    },
                    new Person
                    {
                        Id = 5,
                        FirstName = "Димитър",
                        MiddleName = null,
                        LastName = "Андонов",
                        EgnLnc = "7504259734",
                        CitizenshipCountryId = null,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1941, 11, 12)
                    },
                    new Person
                    {
                        Id = 6,
                        FirstName = "Петър",
                        MiddleName = "Петров",
                        LastName = "Петров",
                        EgnLnc = "8506030111",
                        CitizenshipCountryId = null,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1985, 06, 03)
                    },
                    new Person
                    {
                        Id = 7,
                        FirstName = "Кристина",
                        MiddleName = "Янева",
                        LastName = "Янева",
                        EgnLnc = "8506030112",
                        CitizenshipCountryId = null,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1985, 06, 03)
                    },
                    new Person
                    {
                        Id = 8,
                        FirstName = "Пламена",
                        MiddleName = "Янева",
                        LastName = "Петрова",
                        EgnLnc = "8506030113",
                        CitizenshipCountryId = null,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1985, 06, 03)
                    },
                    new Person
                    {
                        Id = 9,
                        FirstName = "Звезда",
                        MiddleName = "Петрова",
                        LastName = "Стамболова",
                        EgnLnc = "8506030114",
                        CitizenshipCountryId = null,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1985, 06, 03)
                    },
                    new Person
                    {
                        Id = 10,
                        FirstName = "Иван",
                        MiddleName = "Георгиев",
                        LastName = "Георгиев",
                        EgnLnc = "8506030115",
                        CitizenshipCountryId = null,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1985, 06, 03)
                    },
                    new Person
                    {
                        Id = 11,
                        FirstName = "Асен",
                        MiddleName = "Георгиев",
                        LastName = "Кирилов",
                        EgnLnc = "8506030116",
                        CitizenshipCountryId = null,
                        HasBulgarianAddressRegistration = true,
                        BirthDate = new DateTime(1985, 06, 03)
                    },
                    new Person
                    {
                        Id = 12,
                        FirstName = "Иван",
                        MiddleName = "Иванов",
                        LastName = "Иванов",
                        EgnLnc = "9509291111",
                        ValidFrom = new DateTime(2020, 1, 1, 12, 0, 0),
                        ValidTo = DefaultConstants.MAX_VALID_DATE
                    }
                };
            }
        }

        public static List<PersonAddress> PersonAddresses
        {
            get
            {
                return new List<PersonAddress>
                {
                    new PersonAddress
                    {
                        Id = 1,
                        PersonId = Persons[0].Id,
                        AddressId = AddressData.Addresses[4].Id,
                        AddressTypeId = AddressData.AddressTypes[0].Id
                    },
                    new PersonAddress
                    {
                        Id = 2,
                        PersonId = Persons[0].Id,
                        AddressId = AddressData.Addresses[3].Id,
                        AddressTypeId = AddressData.AddressTypes[1].Id
                    },
                    new PersonAddress
                    {
                        Id = 3,
                        PersonId = Persons[1].Id,
                        AddressId = AddressData.Addresses[2].Id,
                        AddressTypeId = AddressData.AddressTypes[0].Id
                    },
                    new PersonAddress
                    {
                        Id = 4,
                        PersonId = Persons[2].Id,
                        AddressId = AddressData.Addresses[1].Id,
                        AddressTypeId = AddressData.AddressTypes[0].Id
                    },
                    new PersonAddress
                    {
                        Id = 5,
                        PersonId = Persons[3].Id,
                        AddressId = AddressData.Addresses[0].Id,
                        AddressTypeId = AddressData.AddressTypes[1].Id
                    }
                };
            }
        }

        public static List<NdocumentType> NdocumentTypes
        {
            get
            {
                return new List<NdocumentType>
                {
                    new NdocumentType { Id = 1, Code = "LK", Name = "Лична карта" },
                    new NdocumentType { Id = 2, Code = "SHK", Name = "Шофьорска книжка" }
                };
            }
        }

        public static List<PersonDocument> PersonDocuments
        {
            get
            {
                return new List<PersonDocument>
                {
                    new PersonDocument
                    {
                        Id = 1,
                        PersonId = Persons[0].Id,
                        DocumentTypeId = NdocumentTypes[0].Id,
                        DocumentNumber = "12345",
                        DocumentIssueDate = new DateTime(2016, 4, 3),
                        DocumentPublisher = "МВР Бургас"
                    },
                    new PersonDocument
                    {
                        Id = 2,
                        PersonId = Persons[1].Id,
                        DocumentTypeId = NdocumentTypes[1].Id,
                        DocumentNumber = "5555",
                        DocumentIssueDate = new DateTime(2014, 1, 25),
                        DocumentPublisher = "МВР Бургас"
                    },
                    new PersonDocument
                    {
                        Id = 3,
                        PersonId = Persons[2].Id,
                        DocumentTypeId = NdocumentTypes[0].Id,
                        DocumentNumber = "123456874",
                        DocumentIssueDate = new DateTime(2012, 12, 24),
                        DocumentPublisher = "МВР Русе"
                    },
                    new PersonDocument
                    {
                        Id = 4,
                        PersonId = Persons[3].Id,
                        DocumentTypeId = NdocumentTypes[1].Id,
                        DocumentNumber = "9786",
                        DocumentIssueDate = new DateTime(2021, 2, 25),
                        DocumentPublisher = "МВР Тутракан"
                    },
                    new PersonDocument
                    {
                        Id = 5,
                        PersonId = Persons[4].Id,
                        DocumentTypeId = NdocumentTypes[0].Id,
                        DocumentNumber = "1285412",
                        DocumentIssueDate = new DateTime(2017, 7, 2),
                        DocumentPublisher = "МВР Каспичан"
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
                    new EmailAddress { Id = 1, Email = "email1@email.com" },
                    new EmailAddress { Id = 2, Email = "email2@email.com" },
                    new EmailAddress { Id = 3, Email = "email3@email.com" },
                    new EmailAddress { Id = 4, Email = "email4@email.com" },
                    new EmailAddress { Id = 5, Email = "email5@email.com" },
                    new EmailAddress { Id = 6, Email = "iivanov@technologica.com", IsActive = true }
                };
            }
        }

        public static List<PersonEmailAddress> PersonEmailAddresses
        {
            get
            {
                return new List<PersonEmailAddress>
                {
                    new PersonEmailAddress { Id = 1, PersonId = Persons[0].Id, EmailAddressId = EmailAddresses[0].Id },
                    new PersonEmailAddress { Id = 2, PersonId = Persons[1].Id, EmailAddressId = EmailAddresses[1].Id },
                    new PersonEmailAddress { Id = 3, PersonId = Persons[2].Id, EmailAddressId = EmailAddresses[2].Id },
                    new PersonEmailAddress { Id = 4, PersonId = Persons[3].Id, EmailAddressId = EmailAddresses[3].Id },
                    new PersonEmailAddress { Id = 5, PersonId = Persons[4].Id, EmailAddressId = EmailAddresses[4].Id },
                    new PersonEmailAddress { Id = 6, PersonId = Persons[11].Id, EmailAddressId = EmailAddresses[5].Id, IsActive = true }
                };
            }
        }

        public static List<PhoneNumber> PhoneNumbers
        {
            get
            {
                return new List<PhoneNumber>
                {
                    new PhoneNumber { Id = 1, Phone = "0887397551" },
                    new PhoneNumber { Id = 2, Phone = "0887397552" },
                    new PhoneNumber { Id = 3, Phone = "0887397553" },
                    new PhoneNumber { Id = 4, Phone = "0887397554" },
                    new PhoneNumber { Id = 5, Phone = "0887397555" }
                };
            }
        }

        public static List<PersonPhoneNumber> PersonPhoneNumbers
        {
            get
            {
                return new List<PersonPhoneNumber>
                {
                    new PersonPhoneNumber { Id = 1, PersonId = Persons[0].Id, PhoneId = PhoneNumbers[0].Id },
                    new PersonPhoneNumber { Id = 2, PersonId = Persons[1].Id, PhoneId = PhoneNumbers[1].Id },
                    new PersonPhoneNumber { Id = 3, PersonId = Persons[2].Id, PhoneId = PhoneNumbers[2].Id },
                    new PersonPhoneNumber { Id = 4, PersonId = Persons[3].Id, PhoneId = PhoneNumbers[3].Id },
                    new PersonPhoneNumber { Id = 5, PersonId = Persons[4].Id, PhoneId = PhoneNumbers[4].Id }
                };
            }
        }
    }
}
