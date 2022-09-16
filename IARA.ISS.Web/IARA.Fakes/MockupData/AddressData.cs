using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class AddressData
    {
        public static List<Ncountry> Countries
        {
            get
            {
                return new List<Ncountry>
                {
                    new Ncountry { Id = 1, Code = "BG", Name = "България" },
                    new Ncountry { Id = 2, Code = "RO", Name = "Румъния" },
                    new Ncountry { Id = 3, Code = "GR", Name = "Гърция" },
                };
            }
        }

        public static List<Ndistrict> Districts
        {
            get
            {
                return new List<Ndistrict>
                {
                    new Ndistrict
                    {
                        Id = 24,
                        Code = "SOF",
                        Name = "София (столица)",
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new Ndistrict
                    {
                        Id = 3,
                        Code = "BGS",
                        Name = "Бургас",
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new Ndistrict
                    {
                        Id = 20,
                        Code = "SLS",
                        Name = "Силистра",
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new Ndistrict
                    {
                        Id = 5,
                        Code = "VTR",
                        Name = "Велико Търново",
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                };
            }
        }

        public static List<Nmunicipality> Municipalities
        {
            get
            {
                return new List<Nmunicipality>
                {
                    new Nmunicipality
                    {
                        Id = 748,
                        Code = "SOF46",
                        Name = "Столична",
                        DistrictId = 24,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new Nmunicipality
                    {
                        Id = 569,
                        Code = "BGS04",
                        Name = "Бургас",
                        DistrictId = 3,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new Nmunicipality
                    {
                        Id = 737,
                        Code = "SLS31",
                        Name = "Силистра",
                        DistrictId = 20,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new Nmunicipality
                    {
                        Id = 575,
                        Code = "VTR04",
                        Name = "Велико Търново",
                        DistrictId = 5,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                };
            }
        }

        public static List<NpopulatedArea> PopulatedAreas
        {
            get
            {
                return new List<NpopulatedArea>
                {
                    new NpopulatedArea
                    {
                        Id = 10968,
                        Code = "68134",
                        Name = "гр. София",
                        MunicipalityId = 748,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new NpopulatedArea
                    {
                        Id = 7142,
                        Code = "07079",
                        Name = "гр. Бургас",
                        MunicipalityId = 569,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new NpopulatedArea
                    {
                        Id = 10802,
                        Code = "66425",
                        Name = "гр. Силистра",
                        MunicipalityId = 737,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                    new NpopulatedArea
                    {
                        Id = 7253,
                        Code = "10447",
                        Name = "гр. Велико Търново",
                        MunicipalityId = 575,
                        ValidFrom = DateTime.Now.Date,
                        ValidTo = DefaultConstants.MAX_VALID_DATE,
                    },
                };
            }
        }

        public static List<NaddressType> AddressTypes
        {
            get
            {
                return new List<NaddressType>
                {
                    new NaddressType
                    {
                        Id = 1,
                        Code = "PERMANENT",
                        Name = "Постоянен адрес"
                    },
                    new NaddressType
                    {
                        Id = 2,
                        Code = "CORRESPONDENCE",
                        Name = "Адрес за кореспонденция"
                    },
                };
            }
        }

        public static List<Address> Addresses
        {
            get
            {
                return new List<Address>
                {
                    new Address
                    {
                        Id = 1,
                        CountryId = 1,
                        PopulatedAreaId = 10968, //София
                        PostCode = "1000",
                        DistrictId = 24,
                        MunicipalityId = 748,
                        Region = "Младост",
                        Street = "Бъднина",
                        StreetNum = "3",
                        BlockNum = null,
                        EntranceNum = null,
                        FloorNum = null,
                        ApartmentNum = null,
                        IsActive = true,
                    },
                    new Address
                    {
                        Id = 2,
                        CountryId = 1,
                        PopulatedAreaId = 7142, //Бургас
                        PostCode = "8000",
                        DistrictId = 3,
                        MunicipalityId = 569,
                        Region = "Лазур",
                        Street = "Батак",
                        StreetNum = "12",
                        BlockNum = null,
                        EntranceNum = null,
                        FloorNum = null,
                        ApartmentNum = null,
                        IsActive = true,
                    },
                    new Address
                    {
                        Id = 3,
                        CountryId = 1,
                        PopulatedAreaId = 10968, //София
                        PostCode = "1000",
                        DistrictId = 24,
                        MunicipalityId = 748,
                        Region = "Младост",
                        Street = null,
                        StreetNum = null,
                        BlockNum = "407",
                        EntranceNum = "2",
                        FloorNum = "3",
                        ApartmentNum = "8",
                        IsActive = true,
                    },
                    new Address
                    {
                        Id = 4,
                        CountryId = 1,
                        PopulatedAreaId = 7142, //Бургас
                        PostCode = "8000",
                        DistrictId = 3,
                        MunicipalityId = 569,
                        Region = "Лазур",
                        Street = null,
                        StreetNum = null,
                        BlockNum = "54",
                        EntranceNum = "2",
                        FloorNum = "8",
                        ApartmentNum = "среден",
                        IsActive = true,
                    },
                    new Address
                    {
                        Id = 5,
                        CountryId = 1,
                        PopulatedAreaId = 10802, //Силистра
                        PostCode = "7500",
                        DistrictId = 20,
                        MunicipalityId = 737,
                        Region = "Малчика",
                        Street = "Лида",
                        StreetNum = "33",
                        BlockNum = null,
                        EntranceNum = null,
                        FloorNum = "2",
                        ApartmentNum = null,
                        IsActive = true,
                    },
                };
            }
        }
    }
}
