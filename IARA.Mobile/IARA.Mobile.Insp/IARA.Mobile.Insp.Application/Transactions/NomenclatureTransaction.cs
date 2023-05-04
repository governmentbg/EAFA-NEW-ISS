using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Database;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Application.Transactions.Base;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.Transactions
{
    public class NomenclatureTransaction : BaseTransaction, INomenclatureTransaction
    {
        public NomenclatureTransaction(BaseTransactionProvider provider)
            : base(provider)
        {
        }

        public List<SelectNomenclatureDto> GetCountries()
        {
            return GetNomenclatureDtos<NCountry>();
        }

        public List<SelectNomenclatureDto> GetDistricts()
        {
            return GetNomenclatureDtos<NDistrict>();
        }

        public List<SelectNomenclatureDto> GetMuncipalities(int districtId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from municipality in context.NMunicipalities
                    where municipality.IsActive
                        && municipality.DistrictId == districtId
                    orderby municipality.Id
                    select new SelectNomenclatureDto
                    {
                        Id = municipality.Id,
                        Code = municipality.Code,
                        Name = municipality.Name,
                    }
                ).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetPopulatedAreas(int municipalityId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from populatedArea in context.NPopulatedAreas
                    where populatedArea.IsActive
                        && populatedArea.MunicipalityId == municipalityId
                    orderby populatedArea.Id
                    select new SelectNomenclatureDto
                    {
                        Id = populatedArea.Id,
                        Code = populatedArea.Code,
                        Name = populatedArea.Name,
                    }
                ).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetDocumentTypes()
        {
            return GetNomenclatureDtos<NDocumentType>();
        }

        public List<SelectNomenclatureDto> GetFileTypes(string pageCode)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from requiredFileType in context.NRequiredFileTypes
                    join fileType in context.NFileTypes on requiredFileType.FileTypeId equals fileType.Id
                    where requiredFileType.Name == pageCode
                        && fileType.IsActive
                        && requiredFileType.IsActive
                    orderby fileType.Id
                    select new SelectNomenclatureDto
                    {
                        Id = fileType.Id,
                        Code = fileType.Code,
                        Name = fileType.Name,
                    }
                ).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetFileTypes()
        {
            return GetNomenclatureDtos<NFileType>();
        }

        public SelectNomenclatureDto GetFileType(string code)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return context.NFileTypes
                    .Where(f => f.Code == code)
                    .Select(f => new SelectNomenclatureDto
                    {
                        Id = f.Id,
                        Code = f.Code,
                        Name = f.Name
                    })
                    .FirstOrDefault();
            }
        }

        public List<SelectNomenclatureDto> GetInstitutions()
        {
            return GetNomenclatureDtos<NInstitution>();
        }

        public List<ObservationToolNomenclatureDto> GetObservationTools()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from observationTool in context.NObservationTools
                    where observationTool.IsActive
                    orderby observationTool.Id
                    select new ObservationToolNomenclatureDto
                    {
                        Id = observationTool.Id,
                        Code = observationTool.Code,
                        Name = observationTool.Name,
                        HasDescription = observationTool.Code == CommonConstants.NomenclatureOther,
                        OnBoard = observationTool.OnBoard,
                    }
                ).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetPatrolVehicleTypes(bool? isWaterVehicle)
        {
            IEnumerable<PatrolVehicleType> types = null;

            if (isWaterVehicle == null)
            {
                types = new List<PatrolVehicleType> { PatrolVehicleType.Marine, PatrolVehicleType.Ground };
            }
            else if (isWaterVehicle.Value)
            {
                types = new List<PatrolVehicleType> { PatrolVehicleType.Marine };
            }
            else
            {
                types = new List<PatrolVehicleType> { PatrolVehicleType.Ground };
            }

            types = types.Concat(new List<PatrolVehicleType> { PatrolVehicleType.Air, PatrolVehicleType.Other }).ToList();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from pvt in context.NPatrolVehicleTypes
                    where pvt.IsActive
                        && types.Contains(pvt.VehicleType)
                    orderby pvt.Id
                    select new SelectNomenclatureDto
                    {
                        Id = pvt.Id,
                        Code = pvt.Code,
                        Name = pvt.Name,
                    }
                ).ToList();
            }
        }

        public List<VesselActivityDto> GetVesselActivities()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from vesselActivity in context.NVesselActivitys
                    where vesselActivity.IsActive
                    orderby vesselActivity.Id
                    select new VesselActivityDto
                    {
                        Id = vesselActivity.Id,
                        Code = vesselActivity.Code,
                        Name = vesselActivity.Name,
                        HasDescription = vesselActivity.HasAdditionalDescr,
                        IsFishingActivity = vesselActivity.IsFishingActivity,
                    }
                ).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetVesselTypes()
        {
            return GetNomenclatureDtos<NVesselType>();
        }

        public List<SelectNomenclatureDto> GetInspectionVesselTypes()
        {
            return GetNomenclatureDtos<NInspectionVesselType>();
        }

        public List<InspectorNomenclatureDto> GetInspectors(int page, int count, string search = null)
        {
            search = search?.ToLower();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from insp in context.Inspectors
                    where search == null
                        || insp.NormalizedName.Contains(search)
                        || insp.NormalizedCardNum.Contains(search)
                    orderby insp.Id
                    select new InspectorNomenclatureDto
                    {
                        Id = insp.Id,
                        Code = insp.CardNum,
                        Name = $"{insp.FirstName} {(insp.MiddleName == null ? string.Empty : insp.MiddleName + " ")}{insp.LastName}",
                        UserId = insp.UserId,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public List<ShipSelectNomenclatureDto> GetShips(int page, int count, string search = null)
        {
            search = search?.ToLower();
            int[] fleets = Settings.Fleets;

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from ship in context.Ships
                    join ass in context.NShipAssociations on ship.AssociationId equals ass.Id into associationGroup
                    from association in associationGroup.DefaultIfEmpty()
                    where (fleets.Length == 0
                        || ship.FleetTypeId == null
                        || fleets.Contains(ship.FleetTypeId.Value)
                    ) && (search == null
                            || (association != null && association.IsActive && association.NormalizedName.Contains(search))
                            || ship.NormalizedCFR.Contains(search)
                            || ship.NormalizedExtMarkings.Contains(search)
                            || ship.NormalizedShipName.Contains(search))
                    orderby ship.Id
                    select new ShipSelectNomenclatureDto
                    {
                        Id = ship.Id,
                        Uid = ship.Uid,
                        Code = ship.CFR,
                        Name = ship.Name,
                        AssociationName = association?.Name,
                        ExtMarkings = ship.ExtMarkings,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public ShipSelectNomenclatureDto GetShipNomenclature(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from ship in context.Ships
                    join ass in context.NShipAssociations on ship.AssociationId equals ass.Id into associationGroup
                    from association in associationGroup.DefaultIfEmpty()
                    where ship.Id == id
                    select new ShipSelectNomenclatureDto
                    {
                        Id = ship.Id,
                        Uid = ship.Uid,
                        Code = ship.CFR,
                        Name = ship.Name,
                        AssociationName = association?.Name,
                        ExtMarkings = ship.ExtMarkings,
                    }
                ).FirstOrDefault();
            }
        }

        public ShipDto GetShip(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from ship in context.Ships
                    where ship.Id == id
                    orderby ship.Id
                    select new ShipDto
                    {
                        Id = ship.Id,
                        Uid = ship.Uid,
                        UVI = ship.UVI,
                        ShipTypeId = ship.ShipTypeId,
                        MMSI = ship.MMSI,
                        FlagId = ship.FlagId,
                        CallSign = ship.CallSign,
                        AssociationId = ship.AssociationId,
                        CFR = ship.CFR,
                        Name = ship.Name,
                        ExtMarkings = ship.ExtMarkings,
                    }
                ).FirstOrDefault();
            }
        }

        public List<InspectionCheckTypeDto> GetInspectionCheckTypes(InspectionType inspectionType)
        {
            string inspectionTypeString = inspectionType.ToString();
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                int inspectionTypeId = (
                    from inspType in context.NInspectionTypes
                    where inspType.Code == inspectionTypeString
                    select inspType.Id
                ).First();

                return (
                    from ict in context.NInspectionCheckTypes
                    where ict.InsectionTypeId == inspectionTypeId
                        && ict.IsActive
                    orderby ict.Id
                    select new InspectionCheckTypeDto
                    {
                        Id = ict.Id,
                        Code = ict.Code,
                        HasDescription = ict.HasDescription,
                        IsMandatory = ict.IsMandatory,
                        Name = ict.Name,
                        Type = ict.Type,
                        DescriptionLabel = ict.DescriptionLabel,
                    }
                ).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetCatchInspectionTypes()
        {
            return GetNomenclatureDtos<NCatchInspectionType>();
        }

        public List<SelectNomenclatureDto> GetFishes()
        {
            return GetNomenclatureDtos<NFish>();
        }

        public List<SelectNomenclatureDto> GetFishingGears()
        {
            return GetNomenclatureDtos<NFishingGear>();
        }

        public List<SelectNomenclatureDto> GetPatrolVehicles(bool? isWaterVehicle, int page, int count, string search = null)
        {
            IEnumerable<PatrolVehicleType> types = null;

            if (isWaterVehicle == null)
            {
                types = new List<PatrolVehicleType> { PatrolVehicleType.Marine, PatrolVehicleType.Ground };
            }
            else if (isWaterVehicle.Value)
            {
                types = new List<PatrolVehicleType> { PatrolVehicleType.Marine };
            }
            else
            {
                types = new List<PatrolVehicleType> { PatrolVehicleType.Ground };
            }

            types = types.Concat(new List<PatrolVehicleType> { PatrolVehicleType.Air, PatrolVehicleType.Other }).ToList();

            search = search?.ToLower();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from patrolVehicle in context.PatrolVehicles
                    where (search == null
                        || patrolVehicle.NormalizedExternalMark.Contains(search)
                        || patrolVehicle.NormalizedName.Contains(search)
                        || patrolVehicle.NormalizedExternalMark.Contains(search)
                        || patrolVehicle.RegistrationNumber.Contains(search)
                        ) && types.Contains(patrolVehicle.VehicleType)
                    orderby patrolVehicle.Id
                    select new SelectNomenclatureDto
                    {
                        Id = patrolVehicle.Id,
                        Code = patrolVehicle.ExternalMark ?? patrolVehicle.RegistrationNumber ?? patrolVehicle.CallSign,
                        Name = patrolVehicle.Name,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public VesselDuringInspectionDto GetPatrolVehicle(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from patrolVehicle in context.PatrolVehicles
                    where patrolVehicle.Id == id
                    select new VesselDuringInspectionDto
                    {
                        UnregisteredVesselId = patrolVehicle.Id,
                        Name = patrolVehicle.Name,
                        CFR = patrolVehicle.RegistrationNumber,
                        ExternalMark = patrolVehicle.ExternalMark,
                        FlagCountryId = patrolVehicle.FlagId,
                        PatrolVehicleTypeId = patrolVehicle.PatrolVehicleTypeId,
                        RegularCallsign = patrolVehicle.CallSign,
                        InstitutionId = patrolVehicle.InstitutionId,
                    }
                ).FirstOrDefault();
            }
        }

        public List<ShipPersonnelDto> GetShipPersonnel(int shipUid)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                List<ShipPersonnelDto> personnel = (
                    from shipOwner in context.ShipOwners
                    join person in context.Persons on shipOwner.PersonId equals person.Id
                    where shipOwner.ShipUid == shipUid
                    orderby shipOwner.Id
                    select new ShipPersonnelDto
                    {
                        Id = person.Id,
                        EntryId = shipOwner.Id,
                        Code = person.EgnLnc,
                        Type = InspectedPersonType.OwnerPers,
                        Name = $"{person.FirstName} {(person.MiddleName == null ? string.Empty : person.MiddleName + " ")}{person.LastName}",
                    }
                ).ToList();

                personnel.AddRange((
                    from shipOwner in context.ShipOwners
                    join legal in context.Legals on shipOwner.LegalId equals legal.Id
                    where shipOwner.ShipUid == shipUid
                    orderby shipOwner.Id
                    select new ShipPersonnelDto
                    {
                        Id = legal.Id,
                        EntryId = shipOwner.Id,
                        Code = legal.Eik,
                        Type = InspectedPersonType.OwnerLegal,
                        Name = legal.Name,
                    }
                ).ToList());

                personnel.AddRange((
                    from permit in context.PermitLicenses
                    join legal in context.Legals on permit.LegalId equals legal.Id
                    where permit.ShipUid == shipUid
                    orderby permit.Id
                    group permit.Id by new
                    {
                        legal.Id,
                        legal.Eik,
                        legal.Name,
                    } into grp
                    select new ShipPersonnelDto
                    {
                        Id = grp.Key.Id,
                        EntryId = grp.Min(f => f),
                        Code = grp.Key.Eik,
                        Type = InspectedPersonType.LicUsrLgl,
                        Name = grp.Key.Name,
                    }
                ).ToList());

                personnel.AddRange((
                    from permit in context.PermitLicenses
                    join person in context.Persons on permit.PersonId equals person.Id
                    where permit.ShipUid == shipUid
                    orderby permit.Id
                    group permit.Id by new
                    {
                        person.Id,
                        person.EgnLnc,
                        person.FirstName,
                        person.MiddleName,
                        person.LastName,
                    } into grp
                    select new ShipPersonnelDto
                    {
                        Id = grp.Key.Id,
                        EntryId = grp.Min(f => f),
                        Code = grp.Key.EgnLnc,
                        Type = InspectedPersonType.LicUsrPers,
                        Name = $"{grp.Key.FirstName} {(grp.Key.MiddleName == null ? string.Empty : grp.Key.MiddleName + " ")}{grp.Key.LastName}",
                    }
                ).ToList());

                personnel.AddRange((
                    from permit in context.PermitLicenses
                    join person in context.Persons on permit.PersonCaptainId equals person.Id
                    where permit.ShipUid == shipUid
                    orderby permit.Id
                    group permit.Id by new
                    {
                        person.Id,
                        person.EgnLnc,
                        person.FirstName,
                        person.MiddleName,
                        person.LastName,
                    } into grp
                    select new ShipPersonnelDto
                    {
                        Id = grp.Key.Id,
                        EntryId = grp.Min(f => f),
                        Code = grp.Key.EgnLnc,
                        Type = InspectedPersonType.CaptFshmn,
                        Name = $"{grp.Key.FirstName} {(grp.Key.MiddleName == null ? string.Empty : grp.Key.MiddleName + " ")}{grp.Key.LastName}",
                    }
                ).ToList());

                return personnel;
            }
        }

        public List<ShipPersonnelDto> GetPoundNetOwners(int poundNetId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                List<ShipPersonnelDto> personnel = (
                    from permit in context.PoundNetPermitLicenses
                    join legal in context.Legals on permit.LegalId equals legal.Id
                    where permit.PoundNetId == poundNetId
                    orderby permit.Id
                    group permit.Id by new
                    {
                        legal.Id,
                        legal.Eik,
                        legal.Name,
                    } into grp
                    select new ShipPersonnelDto
                    {
                        Id = grp.Key.Id,
                        EntryId = grp.Min(f => f),
                        Code = grp.Key.Eik,
                        Type = InspectedPersonType.OwnerLegal,
                        Name = grp.Key.Name,
                    }
                ).ToList();

                personnel.AddRange((
                    from permit in context.PoundNetPermitLicenses
                    join person in context.Persons on permit.PersonId equals person.Id
                    where permit.PoundNetId == poundNetId
                    orderby permit.Id
                    group permit.Id by new
                    {
                        person.Id,
                        person.EgnLnc,
                        person.FirstName,
                        person.MiddleName,
                        person.LastName,
                    } into grp
                    select new ShipPersonnelDto
                    {
                        Id = grp.Key.Id,
                        EntryId = grp.Min(f => f),
                        Code = grp.Key.EgnLnc,
                        Type = InspectedPersonType.OwnerPers,
                        Name = $"{grp.Key.FirstName} {(grp.Key.MiddleName == null ? string.Empty : grp.Key.MiddleName + " ")}{grp.Key.LastName}",
                    }
                ).ToList());

                return personnel;
            }
        }

        public ShipPersonnelDetailedDto GetDetailedShipPerson(int entryId, InspectedPersonType type)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                switch (type)
                {
                    case InspectedPersonType.OwnerPers:
                        return (
                            from shipOwner in context.ShipOwners
                            join person in context.Persons on shipOwner.PersonId equals person.Id
                            join populatedArea in context.NPopulatedAreas on person.PopulatedAreaId equals populatedArea.Id into pagrp
                            from populatedArea in pagrp.DefaultIfEmpty()
                            where shipOwner.Id == entryId
                            select new ShipPersonnelDetailedDto
                            {
                                Id = person.Id,
                                EntryId = shipOwner.Id,
                                EgnLnc = new EgnLncDto
                                {
                                    EgnLnc = person.EgnLnc,
                                    IdentifierType = person.IdentifierType
                                },
                                IsLegal = false,
                                Type = InspectedPersonType.OwnerPers,
                                FirstName = person.FirstName,
                                MiddleName = person.MiddleName,
                                LastName = person.LastName,
                                Address = person.HasAddress ? new InspectionSubjectAddressDto
                                {
                                    PopulatedArea = populatedArea?.Name,
                                    ApartmentNum = person.ApartmentNum,
                                    CountryId = person.CountryId,
                                    BlockNum = person.BlockNum,
                                    EntranceNum = person.EntranceNum,
                                    FloorNum = person.FloorNum,
                                    StreetNum = person.StreetNum,
                                    Region = person.Region,
                                    Street = person.Street,
                                    DistrictId = person.DistrictId,
                                    MunicipalityId = person.MunicipalityId,
                                    PopulatedAreaId = person.PopulatedAreaId,
                                    PostCode = person.PostalCode,
                                } : null,
                            }
                        ).FirstOrDefault();
                    case InspectedPersonType.OwnerLegal:
                        return (
                            from shipOwner in context.ShipOwners
                            join legal in context.Legals on shipOwner.LegalId equals legal.Id
                            join populatedArea in context.NPopulatedAreas on legal.PopulatedAreaId equals populatedArea.Id into pagrp
                            from populatedArea in pagrp.DefaultIfEmpty()
                            where shipOwner.Id == entryId
                            select new ShipPersonnelDetailedDto
                            {
                                Id = legal.Id,
                                EntryId = shipOwner.Id,
                                Eik = legal.Eik,
                                IsLegal = true,
                                Type = InspectedPersonType.OwnerLegal,
                                FirstName = legal.Name,
                                Address = legal.HasAddress ? new InspectionSubjectAddressDto
                                {
                                    PopulatedArea = populatedArea?.Name,
                                    ApartmentNum = legal.ApartmentNum,
                                    CountryId = legal.CountryId,
                                    BlockNum = legal.BlockNum,
                                    EntranceNum = legal.EntranceNum,
                                    FloorNum = legal.FloorNum,
                                    StreetNum = legal.StreetNum,
                                    Region = legal.Region,
                                    Street = legal.Street,
                                    DistrictId = legal.DistrictId,
                                    MunicipalityId = legal.MunicipalityId,
                                    PopulatedAreaId = legal.PopulatedAreaId,
                                    PostCode = legal.PostalCode,
                                } : null,
                            }
                        ).FirstOrDefault();
                    case InspectedPersonType.LicUsrPers:
                        return (
                            from permit in context.PermitLicenses
                            join person in context.Persons on permit.PersonId equals person.Id
                            join populatedArea in context.NPopulatedAreas on person.PopulatedAreaId equals populatedArea.Id into pagrp
                            from populatedArea in pagrp.DefaultIfEmpty()
                            where permit.Id == entryId
                            select new ShipPersonnelDetailedDto
                            {
                                Id = person.Id,
                                EntryId = permit.Id,
                                EgnLnc = new EgnLncDto
                                {
                                    EgnLnc = person.EgnLnc,
                                    IdentifierType = person.IdentifierType
                                },
                                IsLegal = false,
                                Type = InspectedPersonType.LicUsrPers,
                                FirstName = person.FirstName,
                                MiddleName = person.MiddleName,
                                LastName = person.LastName,
                                Address = person.HasAddress ? new InspectionSubjectAddressDto
                                {
                                    PopulatedArea = populatedArea?.Name,
                                    ApartmentNum = person.ApartmentNum,
                                    CountryId = person.CountryId,
                                    BlockNum = person.BlockNum,
                                    EntranceNum = person.EntranceNum,
                                    FloorNum = person.FloorNum,
                                    StreetNum = person.StreetNum,
                                    Region = person.Region,
                                    Street = person.Street,
                                    DistrictId = person.DistrictId,
                                    MunicipalityId = person.MunicipalityId,
                                    PopulatedAreaId = person.PopulatedAreaId,
                                    PostCode = person.PostalCode,
                                } : null,
                            }
                        ).FirstOrDefault();
                    case InspectedPersonType.LicUsrLgl:
                        return (
                            from permit in context.PermitLicenses
                            join legal in context.Legals on permit.LegalId equals legal.Id
                            join populatedArea in context.NPopulatedAreas on legal.PopulatedAreaId equals populatedArea.Id into pagrp
                            from populatedArea in pagrp.DefaultIfEmpty()
                            where permit.Id == entryId
                            select new ShipPersonnelDetailedDto
                            {
                                Id = legal.Id,
                                EntryId = permit.Id,
                                Eik = legal.Eik,
                                IsLegal = true,
                                Type = InspectedPersonType.LicUsrLgl,
                                FirstName = legal.Name,
                                Address = legal.HasAddress ? new InspectionSubjectAddressDto
                                {
                                    PopulatedArea = populatedArea?.Name,
                                    ApartmentNum = legal.ApartmentNum,
                                    CountryId = legal.CountryId,
                                    BlockNum = legal.BlockNum,
                                    EntranceNum = legal.EntranceNum,
                                    FloorNum = legal.FloorNum,
                                    StreetNum = legal.StreetNum,
                                    Region = legal.Region,
                                    Street = legal.Street,
                                    DistrictId = legal.DistrictId,
                                    MunicipalityId = legal.MunicipalityId,
                                    PopulatedAreaId = legal.PopulatedAreaId,
                                    PostCode = legal.PostalCode,
                                } : null,
                            }
                        ).FirstOrDefault();
                    case InspectedPersonType.CaptFshmn:
                        return (
                            from permit in context.PermitLicenses
                            join person in context.Persons on permit.PersonCaptainId equals person.Id
                            join populatedArea in context.NPopulatedAreas on person.PopulatedAreaId equals populatedArea.Id into pagrp
                            from populatedArea in pagrp.DefaultIfEmpty()
                            where permit.Id == entryId
                            select new ShipPersonnelDetailedDto
                            {
                                Id = person.Id,
                                EntryId = permit.CaptainId,
                                EgnLnc = new EgnLncDto
                                {
                                    EgnLnc = person.EgnLnc,
                                    IdentifierType = person.IdentifierType
                                },
                                IsLegal = false,
                                Type = InspectedPersonType.CaptFshmn,
                                FirstName = person.FirstName,
                                MiddleName = person.MiddleName,
                                LastName = person.LastName,
                                Address = person.HasAddress ? new InspectionSubjectAddressDto
                                {
                                    PopulatedArea = populatedArea?.Name,
                                    ApartmentNum = person.ApartmentNum,
                                    CountryId = person.CountryId,
                                    BlockNum = person.BlockNum,
                                    EntranceNum = person.EntranceNum,
                                    FloorNum = person.FloorNum,
                                    StreetNum = person.StreetNum,
                                    Region = person.Region,
                                    Street = person.Street,
                                    DistrictId = person.DistrictId,
                                    MunicipalityId = person.MunicipalityId,
                                    PopulatedAreaId = person.PopulatedAreaId,
                                    PostCode = person.PostalCode,
                                } : null,
                            }
                        ).FirstOrDefault();
                    default:
                        throw new NotImplementedException($"The provided {nameof(InspectedPersonType)} inside {nameof(GetDetailedShipPerson)} is not implemented.");
                }
            }
        }

        public ShipPersonnelDetailedDto GetDetailedPoundNetPerson(int entryId, InspectedPersonType type)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                switch (type)
                {
                    case InspectedPersonType.OwnerPers:
                        return (
                            from permit in context.PoundNetPermitLicenses
                            join person in context.Persons on permit.PersonId equals person.Id
                            join populatedArea in context.NPopulatedAreas on person.PopulatedAreaId equals populatedArea.Id into pagrp
                            from populatedArea in pagrp.DefaultIfEmpty()
                            where permit.Id == entryId
                            select new ShipPersonnelDetailedDto
                            {
                                Id = person.Id,
                                EntryId = permit.Id,
                                EgnLnc = new EgnLncDto
                                {
                                    EgnLnc = person.EgnLnc,
                                    IdentifierType = person.IdentifierType
                                },
                                IsLegal = false,
                                Type = InspectedPersonType.LicUsrPers,
                                FirstName = person.FirstName,
                                MiddleName = person.MiddleName,
                                LastName = person.LastName,
                                Address = person.HasAddress ? new InspectionSubjectAddressDto
                                {
                                    PopulatedArea = populatedArea?.Name,
                                    ApartmentNum = person.ApartmentNum,
                                    CountryId = person.CountryId,
                                    BlockNum = person.BlockNum,
                                    EntranceNum = person.EntranceNum,
                                    FloorNum = person.FloorNum,
                                    StreetNum = person.StreetNum,
                                    Region = person.Region,
                                    Street = person.Street,
                                    DistrictId = person.DistrictId,
                                    MunicipalityId = person.MunicipalityId,
                                    PopulatedAreaId = person.PopulatedAreaId,
                                    PostCode = person.PostalCode,
                                } : null,
                            }
                        ).FirstOrDefault();
                    case InspectedPersonType.OwnerLegal:
                        return (
                            from permit in context.PoundNetPermitLicenses
                            join legal in context.Legals on permit.LegalId equals legal.Id
                            join populatedArea in context.NPopulatedAreas on legal.PopulatedAreaId equals populatedArea.Id into pagrp
                            from populatedArea in pagrp.DefaultIfEmpty()
                            where permit.Id == entryId
                            select new ShipPersonnelDetailedDto
                            {
                                Id = legal.Id,
                                EntryId = permit.Id,
                                Eik = legal.Eik,
                                IsLegal = true,
                                Type = InspectedPersonType.LicUsrLgl,
                                FirstName = legal.Name,
                                Address = legal.HasAddress ? new InspectionSubjectAddressDto
                                {
                                    PopulatedArea = populatedArea?.Name,
                                    ApartmentNum = legal.ApartmentNum,
                                    CountryId = legal.CountryId,
                                    BlockNum = legal.BlockNum,
                                    EntranceNum = legal.EntranceNum,
                                    FloorNum = legal.FloorNum,
                                    StreetNum = legal.StreetNum,
                                    Region = legal.Region,
                                    Street = legal.Street,
                                    DistrictId = legal.DistrictId,
                                    MunicipalityId = legal.MunicipalityId,
                                    PopulatedAreaId = legal.PopulatedAreaId,
                                    PostCode = legal.PostalCode,
                                } : null,
                            }
                        ).FirstOrDefault();
                    default:
                        throw new NotImplementedException($"The provided {nameof(InspectedPersonType)} inside {nameof(GetDetailedShipPerson)} is not implemented.");
                }
            }
        }

        public List<CatchZoneNomenclatureDto> GetCatchZones()
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from zone in context.NCatchZones
                    where zone.IsActive
                    orderby zone.Id
                    select new CatchZoneNomenclatureDto
                    {
                        Id = zone.Id,
                        Code = zone.Code,
                        Name = zone.Name,
                        Block = new RectangleF(zone.X, zone.Y, zone.Width, zone.Height)
                    }
                ).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetWaterBodyTypes()
        {
            return GetNomenclatureDtos<NWaterBodyType>();
        }

        public List<SelectNomenclatureDto> GetPorts(int page, int count, string search = null)
        {
            search = search?.ToLower();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from port in context.NPorts
                    where port.IsActive
                        && (search == null
                            || port.NormalizedName.Contains(search))
                    orderby port.Id
                    select new SelectNomenclatureDto
                    {
                        Id = port.Id,
                        Code = port.Code,
                        Name = port.Name,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetFishingGearMarkStatuses()
        {
            return GetNomenclatureDtos<NFishingGearMarkStatus>();
        }

        public List<SelectNomenclatureDto> GetTransportVehicleTypes()
        {
            return GetNomenclatureDtos<NTransportVehicleType>();
        }

        public List<SelectNomenclatureDto> GetPoundNets(int page, int count, string search = null)
        {
            search = search?.ToLower();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from poundNet in context.PoundNets
                    where search == null
                        || poundNet.NormalizedName.Contains(search)
                    orderby poundNet.Id
                    select new SelectNomenclatureDto
                    {
                        Id = poundNet.Id,
                        Name = poundNet.Name,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public SelectNomenclatureDto GetPoundNet(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from poundNet in context.PoundNets
                    where poundNet.Id == id
                    select new SelectNomenclatureDto
                    {
                        Id = poundNet.Id,
                        Name = poundNet.Name,
                    }
                ).FirstOrDefault();
            }
        }

        public List<SelectNomenclatureDto> GetGenders()
        {
            return GetNomenclatureDtos<NGender>();
        }

        public List<PermitLicenseDto> GetPermitLicenses(int shipUid)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from permit in context.PermitLicenses
                    join type in context.NPermitLicenseTypes on permit.TypeId equals type.Id
                    where !permit.IsSuspended && permit.ShipUid == shipUid
                    orderby permit.Id
                    select new PermitLicenseDto
                    {
                        Id = permit.Id,
                        LicenseNumber = permit.LicenseNumber,
                        PermitNumber = permit.PermitNumber,
                        From = permit.ValidFrom,
                        To = permit.ValidTo,
                        TypeId = permit.TypeId,
                        TypeName = type.Name,
                    }
                ).ToList();
            }
        }

        public List<PermitDto> GetPermits(int shipUid)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from permit in context.Permits
                    join type in context.NPermitTypes on permit.TypeId equals type.Id
                    where !permit.IsSuspended && permit.ShipUid == shipUid
                    orderby permit.Id
                    select new PermitDto
                    {
                        Id = permit.Id,
                        PermitNumber = permit.PermitNumber,
                        From = permit.ValidFrom,
                        To = permit.ValidTo,
                        TypeId = permit.TypeId,
                        TypeName = type.Name,
                    }
                ).ToList();
            }
        }

        public List<LogBookDto> GetLogBooks(int shipUid)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from logBook in context.LogBooks
                    where logBook.ShipUid == shipUid
                    orderby logBook.Id
                    select new LogBookDto
                    {
                        Id = logBook.Id,
                        Number = logBook.Number,
                        From = logBook.IssuedOn,
                        StartPage = logBook.StartPage,
                        EndPage = logBook.EndPage,
                    }
                ).ToList();
            }
        }

        public SelectNomenclatureDto GetAssociation(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from association in context.NShipAssociations
                    where association.Id == id
                    select new SelectNomenclatureDto
                    {
                        Id = association.Id,
                        Code = association.Code,
                        Name = association.Name,
                    }
                ).FirstOrDefault();
            }
        }

        public List<SelectNomenclatureDto> GetAssociations()
        {
            return GetNomenclatureDtos<NShipAssociation>();
        }

        public List<SelectNomenclatureDto> GetBuyers(int page, int count, string search = null)
        {
            search = search?.ToLower();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from buyer in context.Buyers
                    join person in context.Persons on buyer.PersonId equals person.Id into pgrp
                    from person in pgrp.DefaultIfEmpty()
                    join legal in context.Legals on buyer.LegalId equals legal.Id into lgrp
                    from legal in lgrp.DefaultIfEmpty()
                    where search == null
                        || (person != null && (person.FirstName.Contains(search)
                            || person.MiddleName.Contains(search)
                            || person.LastName.Contains(search)
                            || person.EgnLnc == search)
                        )
                        || (legal != null && (legal.Name.Contains(search)
                            || legal.Eik == search)
                        )
                    orderby buyer.Id
                    select new SelectNomenclatureDto
                    {
                        Id = buyer.Id,
                        Code = person != null ? person.EgnLnc : legal?.Eik,
                        Name = person != null
                            ? (person.FirstName
                                + (person.MiddleName != null ? (" " + person.MiddleName) : "")
                                + " "
                                + person.LastName)
                            : legal?.Name,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public InspectionSubjectPersonnelDto GetBuyer(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from buyer in context.Buyers
                    join person in context.Persons on buyer.PersonId equals person.Id into pgrp
                    from person in pgrp.DefaultIfEmpty()
                    join legal in context.Legals on buyer.LegalId equals legal.Id into lgrp
                    from legal in lgrp.DefaultIfEmpty()
                    join personPopulatedArea in context.NPopulatedAreas on person?.PopulatedAreaId equals personPopulatedArea.Id into ppagrp
                    from personPopulatedArea in ppagrp.DefaultIfEmpty()
                    join legalPopulatedArea in context.NPopulatedAreas on legal?.PopulatedAreaId equals legalPopulatedArea.Id into lpagrp
                    from legalPopulatedArea in lpagrp.DefaultIfEmpty()
                    where buyer.Id == id
                    select new InspectionSubjectPersonnelDto
                    {
                        Id = person?.Id ?? legal?.Id,
                        Type = InspectedPersonType.RegBuyer,
                        EntryId = buyer.Id,
                        CitizenshipId = person != null ? person.CountryId : legal.CountryId,
                        EgnLnc = person != null ? new EgnLncDto
                        {
                            EgnLnc = person != null ? person.EgnLnc : legal.Eik,
                            IdentifierType = person.IdentifierType
                        } : null,
                        Eik = legal?.Eik,
                        IsLegal = legal != null,
                        FirstName = person != null ? person.FirstName : legal.Name,
                        LastName = person?.LastName,
                        MiddleName = person?.MiddleName,
                        RegisteredAddress = person != null && person.HasAddress
                            ? new InspectionSubjectAddressDto
                            {
                                PopulatedArea = personPopulatedArea?.Name,
                                ApartmentNum = person.ApartmentNum,
                                CountryId = person.CountryId,
                                BlockNum = person.BlockNum,
                                EntranceNum = person.EntranceNum,
                                FloorNum = person.FloorNum,
                                StreetNum = person.StreetNum,
                                Region = person.Region,
                                Street = person.Street,
                                DistrictId = person.DistrictId,
                                MunicipalityId = person.MunicipalityId,
                                PopulatedAreaId = person.PopulatedAreaId,
                                PostCode = person.PostalCode,
                            }
                            : legal != null && legal.HasAddress ? new InspectionSubjectAddressDto
                            {
                                PopulatedArea = legalPopulatedArea?.Name,
                                ApartmentNum = legal.ApartmentNum,
                                CountryId = legal.CountryId,
                                BlockNum = legal.BlockNum,
                                EntranceNum = legal.EntranceNum,
                                FloorNum = legal.FloorNum,
                                StreetNum = legal.StreetNum,
                                Region = legal.Region,
                                Street = legal.Street,
                                DistrictId = legal.DistrictId,
                                MunicipalityId = legal.MunicipalityId,
                                PopulatedAreaId = legal.PopulatedAreaId,
                                PostCode = legal.PostalCode,
                            } : null
                    }
                ).FirstOrDefault();
            }
        }

        public List<SelectNomenclatureDto> GetFleetTypes()
        {
            return GetNomenclatureDtos<NFleetType>();
        }

        public List<SelectNomenclatureDto> GetFishPresentations()
        {
            return GetNomenclatureDtos<NFishPresentation>();
        }

        public BuyerUtilityDto GetBuyerUtility(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from buyer in context.Buyers
                    join populatedArea in context.NPopulatedAreas on buyer?.PopulatedAreaId equals populatedArea.Id into ppagrp
                    from populatedArea in ppagrp.DefaultIfEmpty()
                    where buyer.Id == id
                        && buyer.HasUtility
                    select new BuyerUtilityDto
                    {
                        Name = buyer.UtilityName,
                        Address = buyer.HasAddress ? new InspectionSubjectAddressDto
                        {
                            PopulatedArea = populatedArea?.Name,
                            ApartmentNum = buyer.ApartmentNum,
                            CountryId = buyer.CountryId,
                            BlockNum = buyer.BlockNum,
                            EntranceNum = buyer.EntranceNum,
                            FloorNum = buyer.FloorNum,
                            StreetNum = buyer.StreetNum,
                            Region = buyer.Region,
                            Street = buyer.Street,
                            DistrictId = buyer.DistrictId,
                            MunicipalityId = buyer.MunicipalityId,
                            PopulatedAreaId = buyer.PopulatedAreaId,
                            PostCode = buyer.PostalCode,
                        } : null
                    }
                ).FirstOrDefault();
            }
        }

        public List<SelectNomenclatureDto> GetAquacultures(int page, int count, string search = null)
        {
            search = search?.ToLower();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from aqua in context.Aquacultures
                    where search == null
                        || aqua.NormalizedName.Contains(search)
                        || aqua.UrorNum.Contains(search)
                    orderby aqua.Id
                    select new SelectNomenclatureDto
                    {
                        Id = aqua.Id,
                        Code = aqua.UrorNum,
                        Name = aqua.Name,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetFishSex()
        {
            return GetNomenclatureDtos<NFishSex>();
        }

        public SelectNomenclatureDto GetAquaculture(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from aqua in context.Aquacultures
                    where aqua.Id == id
                    select new SelectNomenclatureDto
                    {
                        Id = aqua.Id,
                        Code = aqua.UrorNum,
                        Name = aqua.Name,
                    }
                ).FirstOrDefault();
            }
        }

        public List<PermitNomenclatureDto> GetPoundNetPermits(int poundNetId, int page, int count, string search = null)
        {
            search = search?.ToLower();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from permit in context.PoundNetPermitLicenses
                    join type in context.NPermitLicenseTypes on permit.TypeId equals type.Id
                    where (search == null
                        || permit.LicenseNumber.Contains(search)
                        || type.Name.Contains(search))
                        && permit.PoundNetId == poundNetId
                    orderby permit.Id
                    select new PermitNomenclatureDto
                    {
                        Id = permit.Id,
                        Code = type.Name,
                        Name = permit.LicenseNumber,
                        From = permit.ValidFrom,
                        To = permit.ValidTo,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public List<PermitNomenclatureDto> GetPermits(int shipUid, int page, int count, string search = null)
        {
            search = search?.ToLower();

            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from permit in context.PermitLicenses
                    join type in context.NPermitLicenseTypes on permit.TypeId equals type.Id
                    where (search == null
                        || permit.LicenseNumber.Contains(search)
                        || type.Name.Contains(search))
                        && permit.ShipUid == shipUid
                    orderby permit.Id
                    select new PermitNomenclatureDto
                    {
                        Id = permit.Id,
                        Code = type.Name,
                        Name = permit.LicenseNumber,
                        From = permit.ValidFrom,
                        To = permit.ValidTo,
                    }
                ).Skip(page * count).Take(count).ToList();
            }
        }

        public List<SelectNomenclatureDto> GetFishingGearPingerStatuses()
        {
            return GetNomenclatureDtos<NFishingGearPingerStatus>();
        }

        public PermitNomenclatureDto GetPoundNetPermit(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from permit in context.PoundNetPermitLicenses
                    join type in context.NPermitLicenseTypes on permit.TypeId equals type.Id
                    where permit.Id == id
                    select new PermitNomenclatureDto
                    {
                        Id = permit.Id,
                        Code = type.Name,
                        Name = permit.LicenseNumber,
                        From = permit.ValidFrom,
                        To = permit.ValidTo,
                    }
                ).SingleOrDefault();
            }
        }

        public PermitNomenclatureDto GetPermit(int id)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from permit in context.PermitLicenses
                    join type in context.NPermitLicenseTypes on permit.TypeId equals type.Id
                    where permit.Id == id
                    select new PermitNomenclatureDto
                    {
                        Id = permit.Id,
                        Code = type.Name,
                        Name = permit.LicenseNumber,
                        From = permit.ValidFrom,
                        To = permit.ValidTo,
                    }
                ).SingleOrDefault();
            }
        }

        public List<SelectNomenclatureDto> GetFishingGearRecheckReasons()
        {
            return GetNomenclatureDtos<NFishingGearRecheckReason>();
        }

        public List<SelectNomenclatureDto> GetFishingGearCheckReasons()
        {
            return GetNomenclatureDtos<NFishingGearCheckReason>();
        }

        public List<SelectNomenclatureDto> GetTurbotSizeGroups()
        {
            return GetNomenclatureDtos<NTurbotSizeGroup>();
        }

        public ShipPersonnelDetailedDto GetAquacultureOwner(int aquacultureId)
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                return (
                    from aqua in context.Aquacultures
                    join legal in context.Legals on aqua.LegalId equals legal.Id
                    join populatedArea in context.NPopulatedAreas on legal.PopulatedAreaId equals populatedArea.Id into pagrp
                    from populatedArea in pagrp.DefaultIfEmpty()
                    where aqua.Id == aquacultureId
                    select new ShipPersonnelDetailedDto
                    {
                        Id = legal.Id,
                        EntryId = aqua.Id,
                        Eik = legal.Eik,
                        IsLegal = true,
                        Type = InspectedPersonType.OwnerLegal,
                        FirstName = legal.Name,
                        Address = legal.HasAddress ? new InspectionSubjectAddressDto
                        {
                            PopulatedArea = populatedArea?.Name,
                            ApartmentNum = legal.ApartmentNum,
                            CountryId = legal.CountryId,
                            BlockNum = legal.BlockNum,
                            EntranceNum = legal.EntranceNum,
                            FloorNum = legal.FloorNum,
                            StreetNum = legal.StreetNum,
                            Region = legal.Region,
                            Street = legal.Street,
                            DistrictId = legal.DistrictId,
                            MunicipalityId = legal.MunicipalityId,
                            PopulatedAreaId = legal.PopulatedAreaId,
                            PostCode = legal.PostalCode,
                        } : null,
                    }
                ).FirstOrDefault();
            }
        }

        private List<SelectNomenclatureDto> GetNomenclatureDtos<TEntity>()
            where TEntity : ICodeNomenclature
        {
            using (IAppDbContext context = ContextBuilder.CreateContext())
            {
                // This can't be done with the C# Query language because it doesn't get translated in sqlite net pcl
                return context.TLTable<TEntity>()
                    .Where(f => f.IsActive)
                    .Select(f => new SelectNomenclatureDto
                    {
                        Id = f.Id,
                        Code = f.Code,
                        Name = f.Name,
                    })
                    .OrderBy(f => f.Id)
                    .ToList();
            }
        }
    }
}
