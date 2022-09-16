using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Extended.Models.ActualState;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TL.RegiXClient.Extended.Models.IAMA;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TL.RegiXClient.Extended.Models.PersonDataSearch;

namespace IARA.RegixIntegration.Utils
{
    public static class RegixDataMappers
    {
        public static RegixPersonContext MapForeignPersonIdentityResponse(ForeignIdentityInfoResponseType response)
        {
            if (response != null && response.PersonNames != null)
            {
                return new RegixPersonContext
                {
                    Person = new RegixPersonDataDTO
                    {
                        EgnLnc = new EgnLncDTO
                        {
                            EgnLnc = response.EGN,
                            IdentifierType = IdentifierTypeEnum.LNC
                        },
                        FirstName = response.PersonNames.FirstName,
                        MiddleName = response.PersonNames.Surname,
                        LastName = response.PersonNames.FamilyName,
                        GenderName = response.GenderName,
                        Document = new PersonDocumentDTO
                        {
                            DocumentIssuedBy = response.IdentityDocument.IssuerName,
                            DocumentIssuedOn = response.IdentityDocument.IssueDate,
                            DocumentNumber = response.IdentityDocument.IdentityDocumentNumber,
                            DocumentTypeName = response.IdentityDocument.DocumentType,
                        },
                        HasBulgarianAddressRegistration = response.PermanentAddress != null
                    },
                    Addresses = new List<AddressRegistrationDTO>
                    {
                        new AddressRegistrationDTO
                        {
                            AddressType = AddressTypesEnum.PERMANENT,
                            ApartmentNum = response.PermanentAddress.Apartment,
                            BlockNum = response.PermanentAddress.BuildingNumber,
                            DistrictName = response.PermanentAddress.DistrictName,
                            EntranceNum = response.PermanentAddress.Entrance,
                            FloorNum = response.PermanentAddress.Floor,
                            MunicipalityName = response.PermanentAddress.MunicipalityName,
                            PopulatedAreaName = response.PermanentAddress.SettlementName,
                            Region = response.PermanentAddress.LocationName
                        }
                    }
                };
            }

            return null;
        }

        public static AddressRegistrationDTO MapPermanentAddressResponse(PermanentAddressResponseType response)
        {
            if (response != null)
            {
                return new AddressRegistrationDTO
                {
                    AddressType = AddressTypesEnum.PERMANENT,
                    ApartmentNum = response.Apartment?.TrimStart('0'),
                    BlockNum = response.BuildingNumber?.TrimStart('0'),
                    DistrictName = response.DistrictName,
                    EntranceNum = response.Entrance?.TrimStart('0'),
                    FloorNum = response.Floor?.TrimStart('0'),
                    MunicipalityName = response.MunicipalityName,
                    PopulatedAreaName = response.SettlementName,
                    Region = response.CityArea,
                    Street = response.LocationName
                };
            }
            else
            {
                return null;
            }
        }

        public static RegixPersonContext MapPersonDataSearchResponse(PersonDataResponseType response)
        {
            if (response != null)
            {
                return new RegixPersonContext
                {
                    Person = new RegixPersonDataDTO
                    {
                        BirthDate = response.BirthDate,
                        EgnLnc = new EgnLncDTO
                        {
                            EgnLnc = response.EGN,
                            IdentifierType = IdentifierTypeEnum.EGN
                        },
                        FirstName = response.PersonNames.FirstName,
                        LastName = response.PersonNames.FamilyName,
                        MiddleName = response.PersonNames.SurName,
                        GenderName = response.Gender?.GenderName.ToString(),
                        CitizenshipCountryName = response.Nationality?.NationalityName,
                        HasBulgarianAddressRegistration = true
                    }
                };
            }
            else
            {
                return null;
            }
        }

        public static RegixPersonContext MapPersonIdentityInfoResponse(PersonalIdentityInfoResponseType response)
        {
            if (response != null && response.PersonNames != null)
            {
                return new RegixPersonContext
                {
                    Person = new RegixPersonDataDTO
                    {
                        FirstName = response.PersonNames.FirstName,
                        MiddleName = response.PersonNames.Surname,
                        LastName = response.PersonNames.FamilyName,
                        EgnLnc = new EgnLncDTO
                        {
                            EgnLnc = response.EGN,
                            IdentifierType = IdentifierTypeEnum.EGN
                        },
                        GenderName = response.GenderName,
                        Document = new PersonDocumentDTO
                        {
                            DocumentIssuedBy = response.IssuerName,
                            DocumentNumber = response.IdentityDocumentNumber,
                            DocumentIssuedOn = response.IssueDate,
                            DocumentTypeName = response.DocumentType
                        }
                    },
                    Addresses = new List<AddressRegistrationDTO>
                    {
                        new AddressRegistrationDTO
                        {
                            AddressType = AddressTypesEnum.PERMANENT,
                            ApartmentNum = response.PermanentAddress?.Apartment,
                            BlockNum = response.PermanentAddress?.BuildingNumber,
                            DistrictName = response.PermanentAddress?.DistrictName,
                            EntranceNum = response.PermanentAddress?.Entrance,
                            FloorNum = response.PermanentAddress?.Floor,
                            MunicipalityName = response.PermanentAddress?.MunicipalityName,
                            PopulatedAreaName = response.PermanentAddress?.SettlementName,
                            Region = response.PermanentAddress?.LocationName
                        }
                    }
                };
            }
            else
            {
                return null;
            }
        }

        public static RegixLegalContext MapActualStateResponse(ActualStateResponseType response)
        {
            if (response != null)
            {
                var legal = new RegixLegalContext
                {
                    Legal = new RegixLegalDataDTO
                    {
                        EIK = response.UIC,
                        Name = response.Company,
                        Phone = response.Seat?.Contacts?.Phone,
                        Email = response.Seat?.Contacts?.EMail
                    }
                };

                if (response.SeatForCorrespondence != null)
                {
                    legal.Addresses = new List<AddressRegistrationDTO>
                    {
                       MapFromAddressType(response.SeatForCorrespondence, AddressTypesEnum.CORRESPONDENCE)
                    };
                }

                if (response.Seat != null && response.Seat.Address != null)
                {
                    legal.Addresses.Add(MapFromAddressType(response.Seat.Address, AddressTypesEnum.COURT_REGISTRATION));
                }

                return legal;
            }
            else
            {
                return null;
            }
        }

        public static VesselContext MapRegixVessel(GovReportShip ship)
        {
            var vesselContext = new VesselContext
            {
                VesselData = new ShipRegisterBaseRegixDataDTO
                {
                    BoardHeight = ship.Characteristics.ShipboardHeight,
                    GrossTonnage = ship.Characteristics.BT,
                    NetTonnage = ship.Characteristics.NT,
                    TotalLength = ship.Characteristics.MaxLength,
                    TotalWidth = ship.Characteristics.MaxWidth,
                    RegLicencePublishPage = ship.RegistrationInfo.Page,
                    RegLicencePublishVolume = ship.RegistrationInfo.Tom,
                    RegLicenceNum = ship.RegistrationInfo.RegistrationNumber,
                    Name = ship.RegistrationInfo.ShipName,
                    ShipDraught = ship.Characteristics.Displacement
                },
                Owners = new List<ShipOwnerRegixDataDTO>()
            };

            foreach (var owner in ship.OwnersInfo)
            {
                var vesselOwner = new ShipOwnerRegixDataDTO
                {
                    AddressRegistrations = new List<AddressRegistrationDTO>
                    {
                        new AddressRegistrationDTO
                        {
                            Street = owner.Address
                        }
                    },
                    EgnLncEik = owner.BulstatEGN,
                    IsOwnerPerson = owner.IsUserSpecified && owner.IsUser.HasValue && owner.IsUser.Value,
                };

                if (vesselOwner.IsOwnerPerson.HasValue && vesselOwner.IsOwnerPerson.Value)
                {
                    vesselOwner.RegixPersonData = new RegixPersonDataDTO
                    {
                        FirstName = owner.Name,
                        MiddleName = owner.SurName,
                        LastName = owner.FamilyName,
                        EgnLnc = new EgnLncDTO
                        {
                            IdentifierType = IdentifierTypeEnum.EGN,
                            EgnLnc = owner.BulstatEGN
                        }
                    };
                }
                else
                {
                    vesselOwner.RegixLegalData = new RegixLegalDataDTO
                    {
                        Name = owner.Name,
                        EIK = owner.BulstatEGN
                    };
                }

                vesselContext.Owners.Add(vesselOwner);
            }

            return vesselContext;
        }

        private static AddressRegistrationDTO MapFromAddressType(AddressType address, AddressTypesEnum addressType)
        {
            if (address != null)
            {
                return new AddressRegistrationDTO
                {
                    AddressType = addressType,
                    CountryName = address.Country,
                    ApartmentNum = address.Apartment,
                    BlockNum = address.Block,
                    DistrictName = address.District,
                    EntranceNum = address.Entrance,
                    FloorNum = address.Floor,
                    MunicipalityName = address.Municipality,
                    PopulatedAreaName = address.Settlement,
                    PostalCode = address.PostCode,
                    Region = address.District,
                    Street = address.Street,
                    StreetNum = address.StreetNumber
                };
            }
            else
            {
                return null;
            }
        }
    }
}
