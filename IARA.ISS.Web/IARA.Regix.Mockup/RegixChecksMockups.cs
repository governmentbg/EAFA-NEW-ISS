using System;
using TL.RegiXClient.Extended.Models.ActualState;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TL.RegiXClient.Extended.Models.NelkEisme;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TL.RegiXClient.Extended.Models.PersonDataSearch;
using TL.RegiXClient.Extended.Models.RelationsSearch;

namespace IARA.Regix.Mockups
{
    public static class RegixChecksMockups
    {
        internal const int MILISECONDS_DELAY = 5000;

        public static RelationsResponseType RELATIONSHIPS = new RelationsResponseType
        {
            PersonRelations = new TL.RegiXClient.Extended.Models.RelationsSearch.PersonRelationType[]
            {
                new TL.RegiXClient.Extended.Models.RelationsSearch.PersonRelationType
                {
                    EGN = "1111111110",
                    FamilyName = "Петров",
                    FirstName = "Петър",
                    Gender = new TL.RegiXClient.Extended.Models.RelationsSearch.Gender
                    {
                        GenderName = TL.RegiXClient.Extended.Models.RelationsSearch.GenderNameType.Мъж
                    },
                    RelationCode = TL.RegiXClient.Extended.Models.RelationsSearch.RelationType.Баща,
                    SurName = "Петров",
                    Nationality = new TL.RegiXClient.Extended.Models.RelationsSearch.Nationality
                    {
                        NationalityName = "БЪЛГАРИЯ"
                    },
                    RelationCodeSpecified = true
                }
            }
        };


        public static PersonalIdentityInfoResponseType PERSON_IDENTITY = new PersonalIdentityInfoResponseType
        {
            EGN = "1111111110",
            PersonNames = new TL.RegiXClient.Extended.Models.PersonalIdentity.PersonNames
            {
                FamilyName = "Петров",
                FirstName = "Петър",
                Surname = "Иванов",
                FirstNameLatin = "Petar",
                LastNameLatin = "Petrov",
                SurnameLatin = "Ivanov"
            },
            DocumentType = "лична карта",
            GenderName = "мъж",
            Height = 1.77,
            HeightSpecified = true,
            IdentityDocumentNumber = "1111111110",
            PermanentAddress = new PermanentAddress
            {
                Apartment = "1",
                DistrictName = "Бургас",
                BuildingNumber = "1",
                SettlementName = "Бургас",
                Entrance = "А",
                Floor = "5",
                LocationName = "Бургас",
                MunicipalityName = "Бургас",
                DistrictNameLatin = "Burgas",
                LocationNameLatin = "Burgas",
                MunicipalityNameLatin = "Burgas",
                SettlementNameLatin = "Burgas"
            },
            NationalityList = new TL.RegiXClient.Extended.Models.PersonalIdentity.Nationality[]
            {
                new TL.RegiXClient.Extended.Models.PersonalIdentity.Nationality
                {
                    NationalityName = "БЪЛГАРИЯ"
                }
            },
            IssueDate = new DateTime(2020, 10, 10),
            IssuerPlace = "Бургас",
            IssuerName = "МВР Бургас",
            IssueDateSpecified = true,
            ValidDate = new DateTime(2030, 10, 10)
        };

        public static PersonDataResponseType PERSON_DATA = new PersonDataResponseType
        {
            EGN = "1111111110",
            PersonNames = new TL.RegiXClient.Extended.Models.PersonDataSearch.PersonNames
            {
                FamilyName = "Петров",
                FirstName = "Петър",
                SurName = "Иванов"
            },
            Gender = new TL.RegiXClient.Extended.Models.PersonDataSearch.Gender
            {
                GenderName = TL.RegiXClient.Extended.Models.PersonDataSearch.GenderNameType.Мъж
            },
            Nationality = new TL.RegiXClient.Extended.Models.PersonDataSearch.Nationality
            {
                NationalityName = "БЪЛГАРИЯ",
            },
            PlaceBirth = "Бургас",
            LatinNames = new TL.RegiXClient.Extended.Models.PersonDataSearch.PersonNames
            {
                FirstName = "Petar",
                FamilyName = "Petrov",
                SurName = "Ivanov"
            },
            Alias = "Гошо от почивка"
        };

        public static PermanentAddressResponseType PERMANENT_ADDRESS = new PermanentAddressResponseType
        {
            Apartment = "1",
            DistrictName = "Бургас",
            BuildingNumber = "1",
            SettlementName = "Бургас",
            Entrance = "А",
            Floor = "5",
            LocationName = "Бургас",
            MunicipalityName = "Бургас",
            CityArea = "Бургас"
        };

        public static ExpertDecisionsResponse LAST_EXPERT_DECISION = new ExpertDecisionsResponse
        {
            ExpertDecisionDocument = new ExpertDecisionDocument[]
            {
                    new ExpertDecisionDocument
                    {
                        DurationDisabilityDateSpecified = true,
                        DurationDisabilityDate = new DateTime(2030,10,10),
                        RegistrationNumber = 1111111111
                    }
            }
        };


        public static ForeignIdentityInfoResponseType FOREIGN_PERSON = new ForeignIdentityInfoResponseType
        {
            EGN = "1111111110",
            LNCh = "1111111110",
            PersonNames = new TL.RegiXClient.Extended.Models.ForeignPersonIdentity.PersonNames
            {
                FamilyName = "Петров",
                FirstName = "Петър",
                Surname = "Иванов",
                FirstNameLatin = "Petar",
                LastNameLatin = "Petrov",
                SurnameLatin = "Ivanov"
            },
            PermanentAddress = new AddressBG
            {
                Apartment = "1",
                DistrictName = "Бургас",
                BuildingNumber = "1",
                SettlementName = "Бургас",
                Entrance = "А",
                Floor = "5",
                LocationName = "Бургас",
                MunicipalityName = "Бургас",
                DistrictNameLatin = "Burgas",
                LocationNameLatin = "Burgas",
                MunicipalityNameLatin = "Burgas",
                SettlementNameLatin = "Burgas"
            },
            GenderName = "мъж",
            Height = 1.77,
            HeightSpecified = true,
            IdentityDocument = new TL.RegiXClient.Extended.Models.ForeignPersonIdentity.IdentityDocument
            {
                IdentityDocumentNumber = "1111111111",
                DocumentType = "лична карта",
                IssueDate = new DateTime(2020, 10, 10),
                IssuePlace = "Бургас",
                IssuerName = "МВР Бургас",
                IssueDateSpecified = true,
                ValidDate = new DateTime(2030, 10, 10)
            }
        };

        public static ActualStateResponseType ACTUAL_STATE_CHECK = new ActualStateResponseType
        {
            UIC = "0200113329",
            Seat = new SeatType
            {
                Address = new AddressType
                {
                    Apartment = "1",
                    Area = "Бургас",
                    Block = "1",
                    Country = "България",
                    District = "Бургас",
                    Entrance = "А",
                    Floor = "5",
                    Settlement = "Бургас",
                    Street = "Пробуда",
                    StreetNumber = "52",
                    PostCode = "8000"
                },
                Contacts = new ContactsType
                {
                    Phone = "+359 2 91 91",
                    EMail = "office@technologica.com"
                }
            },
            SeatForCorrespondence = new AddressType
            {
                Apartment = "1",
                Area = "Бургас",
                Block = "1",
                Country = "България",
                District = "Бургас",
                Entrance = "А",
                Floor = "5",
                Settlement = "Бургас",
                Street = "Пробуда",
                StreetNumber = "52",
                PostCode = "8000"
            },
            Company = "Технологика"
        };
    }
}
