using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.ControlActivity.Inspections;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public abstract class BaseInspectionService<T> : Service, IInspectionService<T>
        where T : InspectionEditDTO, new()
    {
        protected readonly IFishingGearsService FishingGearService;

        protected BaseInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService)
            : base(dbContext)
        {
            FishingGearService = fishingGearService;
        }

        public T GetEntry(int id)
        {
            var inspDbEntry = (
                from inspection in Db.InspectionsRegister
                join inspectionState in Db.NinspectionStates on inspection.StateId equals inspectionState.Id
                join inspectionType in Db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                where inspection.Id == id
                select new
                {
                    Inspection = inspection,
                    InspectionState = inspectionState.Code,
                    InspectionType = inspectionType.Code
                }).First();

            if (inspDbEntry.InspectionState == nameof(InspectionStatesEnum.Draft))
            {
                T deserialized = CommonUtils.Deserialize<T>(inspDbEntry.Inspection.InspectionDraft);

                deserialized.Id = inspDbEntry.Inspection.Id;
                deserialized.Files = Db.GetFiles(Db.InspectionRegisterFiles, id);

                return deserialized;
            }
            else
            {
                T inspectionBaseDTO = new()
                {
                    Id = inspDbEntry.Inspection.Id,
                    InspectionState = Enum.Parse<InspectionStatesEnum>(inspDbEntry.InspectionState),
                    ReportNum = inspDbEntry.Inspection.ReportNum,
                    StartDate = inspDbEntry.Inspection.InspectionStart,
                    EndDate = inspDbEntry.Inspection.InspectionEnd,
                    InspectionType = Enum.Parse<InspectionTypesEnum>(inspDbEntry.InspectionType),
                    ByEmergencySignal = inspDbEntry.Inspection.IsByEmergencySignal,
                    InspectorComment = inspDbEntry.Inspection.InspectorCommentText,
                    AdministrativeViolation = inspDbEntry.Inspection.HasAdministrativeViolation,
                    ActionsTaken = inspDbEntry.Inspection.ActionsTakenText,
                    IsActive = inspDbEntry.Inspection.IsActive,
                    Files = Db.GetFiles(Db.InspectionRegisterFiles, id),
                    Inspectors = GetInspectors(id),
                    Personnel = GetPersonnel(id),
                    Checks = GetChecks(id),
                    PatrolVehicles = GetPatrolVehicles(id),
                    ObservationTexts = GetObservationTexts(id),
                    ViolatedRegulations = GetViolatedRegulations(id)
                };

                return Get(inspectionBaseDTO);
            }
        }

        public int SubmitEntry(T itemDTO, InspectionTypesEnum inspectionType, int userId)
        {
            using TransactionScope scope = new();

            InspectionRegister inspDbEntry;

            if (!itemDTO.Id.HasValue || itemDTO.Id.Value <= 0)
            {
                inspDbEntry = new InspectionRegister();
                Db.InspectionsRegister.Add(inspDbEntry);
            }
            else
            {
                inspDbEntry = Db.InspectionsRegister
                    .Include(x => x.InspectionRegisterFiles)
                    .First(x => x.Id == itemDTO.Id.Value);
            }

            DateTime now = DateTime.Now;

            InspectorDuringInspectionDTO inspectorInCharge = itemDTO.Inspectors.First(x => x.IsInCharge);

            int inspectionTypeId = Db.NinspectionTypes.Single(x => x.Code == inspectionType.ToString()).Id;
            int submittedStateId = Db.NinspectionStates.Single(f => f.Code == nameof(InspectionStatesEnum.Submitted)).Id;

            if (inspectorInCharge?.InspectorId == null)
            {
                throw new ArgumentException("NotInspector");
            }

            Inspector inspector = Db.Inspectors
                .Include(f => f.User)
                    .ThenInclude(f => f.UserInfo)
                        .ThenInclude(f => f.TerritoryUnit)
                .First(x => x.Id == inspectorInCharge.InspectorId.Value);

            inspDbEntry.ReportNum = GenerateReportNumber(inspector);
            inspDbEntry.InspectionTypeId = inspectionTypeId;
            inspDbEntry.StateId = submittedStateId;
            inspDbEntry.InspectionStart = itemDTO.StartDate ?? DateTime.Now.Date;
            inspDbEntry.InspectionEnd = itemDTO.EndDate;
            inspDbEntry.IsByEmergencySignal = itemDTO.ByEmergencySignal;
            inspDbEntry.HasAdministrativeViolation = itemDTO.AdministrativeViolation;
            inspDbEntry.InspectorCommentText = itemDTO.InspectorComment;
            inspDbEntry.ActionsTakenText = itemDTO.ActionsTaken;
            inspDbEntry.CreatedByUserId = userId;
            inspDbEntry.InspectionDraft = null;
            inspDbEntry.TerritoryUnit = inspector.User?.UserInfo?.TerritoryUnit;
            inspDbEntry.IsActive = true;

            Db.SaveChanges();

            if (itemDTO.Files != null)
            {
                foreach (FileInfoDTO file in itemDTO.Files)
                {
                    Db.AddOrEditFile(inspDbEntry, inspDbEntry.InspectionRegisterFiles, file);
                }
            }

            AddInspectedPersons(inspDbEntry, itemDTO.Personnel, SubjectRoleEnum.Inspected);
            AddInspectionInspectors(inspDbEntry, itemDTO.Inspectors);
            AddInspectionChecks(inspDbEntry, itemDTO.Checks);
            AddInspectionObservationTexts(inspDbEntry, itemDTO.ObservationTexts, inspectionType);
            AddInspectionPatrolVehicles(inspDbEntry, itemDTO.PatrolVehicles);
            AddViolatedRegulations(inspDbEntry, itemDTO.ViolatedRegulations);

            Db.SaveChanges();

            Submit(inspDbEntry, itemDTO);

            Db.SaveChanges();

            scope.Complete();

            return inspDbEntry.Id;
        }

        protected abstract T Get(T inspection);

        protected abstract void Submit(InspectionRegister inspDbEntry, T item);

        protected List<InspectionSubjectPersonnelDTO> GetPersonnel(int inspectionId, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
        {
            List<InspectionSubjectPersonnelDTO> people = (
                from inspectedPerson in Db.InspectedPersons
                join inspectedPersonType in Db.NinspectedPersonTypes on inspectedPerson.InspectedPersonTypeId equals inspectedPersonType.Id
                join person in Db.Persons on inspectedPerson.PersonId equals person.Id
                join address in Db.Addresses on inspectedPerson.AddressId equals address.Id into addressMatchTable
                from address in addressMatchTable.DefaultIfEmpty()
                join populatedArea in Db.NpopulatedAreas on address.PopulatedAreaId equals populatedArea.Id into populatedAreaMatchTable
                from populatedArea in populatedAreaMatchTable.DefaultIfEmpty()
                where inspectedPerson.IsActive
                    && inspectedPerson.InspectionId == inspectionId
                    && inspectedPerson.InspectedShipType == role.ToString()
                select new InspectionSubjectPersonnelDTO
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    LastName = person.LastName,
                    Type = Enum.Parse<InspectedPersonTypeEnum>(inspectedPersonType.Code),
                    EgnLnc = new EgnLncDTO
                    {
                        EgnLnc = person.EgnLnc,
                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType),
                    },
                    IsLegal = false,
                    CitizenshipId = person.CitizenshipCountryId,
                    IsRegistered = true,
                    RegisteredAddress = address != null ? new InspectionSubjectAddressDTO
                    {
                        Id = address.Id,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        PostCode = address.PostCode,
                        Region = address.Region,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PopulatedArea = populatedArea.Name,
                    } : null,
                    IsActive = inspectedPerson.IsActive,
                }).ToList();

            List<InspectionSubjectPersonnelDTO> legals = (
                from inspectedPerson in Db.InspectedPersons
                join inspectedPersonType in Db.NinspectedPersonTypes on inspectedPerson.InspectedPersonTypeId equals inspectedPersonType.Id
                join legal in Db.Legals on inspectedPerson.LegalId equals legal.Id
                join address in Db.Addresses on inspectedPerson.AddressId equals address.Id into addressMatchTable
                from address in addressMatchTable.DefaultIfEmpty()
                join populatedArea in Db.NpopulatedAreas on address.PopulatedAreaId equals populatedArea.Id into populatedAreaMatchTable
                from populatedArea in populatedAreaMatchTable.DefaultIfEmpty()
                where inspectedPerson.IsActive
                    && inspectedPerson.InspectionId == inspectionId
                    && inspectedPerson.InspectedShipType == role.ToString()
                select new InspectionSubjectPersonnelDTO
                {
                    Id = legal.Id,
                    FirstName = legal.Name,
                    Type = Enum.Parse<InspectedPersonTypeEnum>(inspectedPersonType.Code),
                    Eik = legal.Eik,
                    IsLegal = true,
                    IsRegistered = true,
                    RegisteredAddress = address != null ? new InspectionSubjectAddressDTO
                    {
                        Id = address.Id,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        PostCode = address.PostCode,
                        Region = address.Region,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PopulatedArea = populatedArea.Name,
                    } : null,
                    IsActive = inspectedPerson.IsActive,
                }).ToList();

            List<InspectionSubjectPersonnelDTO> unregisteredSubjects = (
                from inspectedPerson in Db.InspectedPersons
                join inspectedPersonType in Db.NinspectedPersonTypes on inspectedPerson.InspectedPersonTypeId equals inspectedPersonType.Id
                join address in Db.Addresses on inspectedPerson.AddressId equals address.Id into addressMatchTable
                from address in addressMatchTable.DefaultIfEmpty()
                join populatedArea in Db.NpopulatedAreas on address.PopulatedAreaId equals populatedArea.Id into populatedAreaMatchTable
                from populatedArea in populatedAreaMatchTable.DefaultIfEmpty()
                join unregisteredPerson in Db.UnregisteredPersons on inspectedPerson.UnregisteredPersonId equals unregisteredPerson.Id
                where inspectedPerson.IsActive
                    && inspectedPerson.InspectionId == inspectionId
                    && inspectedPerson.InspectedShipType == role.ToString()
                select new InspectionSubjectPersonnelDTO
                {
                    Id = unregisteredPerson.Id,
                    FirstName = unregisteredPerson.FirstName,
                    MiddleName = unregisteredPerson.MiddleName,
                    LastName = unregisteredPerson.LastName,
                    Type = Enum.Parse<InspectedPersonTypeEnum>(inspectedPersonType.Code),
                    Address = unregisteredPerson.Address,
                    HasBulgarianAddressRegistration = unregisteredPerson.HasBulgarianAddressRegistration,
                    EgnLnc = unregisteredPerson.IdentifierType != nameof(IdentifierTypeEnum.LEGAL) ? new EgnLncDTO
                    {
                        EgnLnc = unregisteredPerson.EgnLnc,
                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(unregisteredPerson.IdentifierType),
                    } : null,
                    Eik = unregisteredPerson.IdentifierType == nameof(IdentifierTypeEnum.LEGAL) ? unregisteredPerson.EgnLnc : null,
                    IsLegal = unregisteredPerson.IdentifierType == nameof(IdentifierTypeEnum.LEGAL),
                    CitizenshipId = unregisteredPerson.CitizenshipCountryId,
                    Comment = unregisteredPerson.Comments,
                    IsRegistered = false,
                    RegisteredAddress = address != null ? new InspectionSubjectAddressDTO
                    {
                        Id = address.Id,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        PostCode = address.PostCode,
                        Region = address.Region,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PopulatedArea = populatedArea.Name,
                    } : null,
                    IsActive = inspectedPerson.IsActive,
                }).ToList();

            return people
                .Concat(legals)
                .Concat(unregisteredSubjects)
                .ToList();
        }

        protected List<InspectionCheckDTO> GetChecks(int inspectionId, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
        {
            List<InspectionCheckDTO> result = (
                from inspCheck in Db.InspectionChecks
                where inspCheck.InspectionId == inspectionId
                    && inspCheck.InspectedShipType == role.ToString()
                    && inspCheck.IsActive
                select new InspectionCheckDTO
                {
                    Id = inspCheck.Id,
                    CheckTypeId = inspCheck.CheckTypeId,
                    CheckValue = Enum.Parse<InspectionToggleTypesEnum>(inspCheck.CheckValue),
                    Description = inspCheck.Description,
                    Number = inspCheck.UnregisteredObjectIdentifier,
                }
            ).ToList();

            return result;
        }

        protected VesselDuringInspectionDTO GetInspectedShip(VesselDuringInspectionDTO vesselDTO)
        {
            VesselDuringInspectionDTO result = null;

            if (vesselDTO.ShipId != null)
            {
                DateTime now = DateTime.Now;

                result = (
                    from ship in Db.ShipsRegister
                    where ship.Id == vesselDTO.ShipId.Value
                    select new VesselDuringInspectionDTO
                    {
                        ShipId = ship.Id,
                        UnregisteredVesselId = null,
                        IsRegistered = true,
                        Name = ship.Name,
                        ExternalMark = ship.ExternalMark,
                        CFR = ship.Cfr,
                        UVI = ship.Uvi,
                        RegularCallsign = ship.IrcscallSign,
                        MMSI = ship.Mmsi,
                        FlagCountryId = ship.FlagCountryId,
                        PatrolVehicleTypeId = null,
                        VesselTypeId = ship.VesselTypeId,
                        ShipAssociationId = ship.ShipAssociationId,
                        IsActive = ship.ValidFrom <= now && ship.ValidTo >= now,
                    }
                ).SingleOrDefault();
            }
            else if (vesselDTO.UnregisteredVesselId != null)
            {
                result = (
                    from ship in Db.UnregisteredVessels
                    where ship.Id == vesselDTO.UnregisteredVesselId.Value
                    select new VesselDuringInspectionDTO
                    {
                        ShipId = null,
                        UnregisteredVesselId = ship.Id,
                        IsRegistered = false,
                        Name = ship.Name,
                        ExternalMark = ship.ExternalMark,
                        CFR = ship.Cfr,
                        UVI = ship.Uvi,
                        RegularCallsign = ship.IrcscallSign,
                        MMSI = ship.Mmsi,
                        FlagCountryId = ship.FlagCountryId,
                        PatrolVehicleTypeId = null,
                        VesselTypeId = ship.VesselTypeId,
                        InstitutionId = ship.InstitutionId,
                        IsActive = ship.IsActive,
                    }
                ).SingleOrDefault();
            }

            if (result != null)
            {
                result.Location = vesselDTO.Location;
                result.LocationDescription = vesselDTO.LocationDescription;
                result.CatchZoneId = vesselDTO.CatchZoneId;
            }

            return result;
        }

        protected List<InspectionPermitDTO> GetPermits(int inspectionId, SubjectRoleEnum role)
        {
            List<InspectionPermitDTO> result = (
                from inspPermit in Db.InspectionPermits
                join permit in Db.CommercialFishingPermitRegisters on inspPermit.PermitId equals permit.Id into pgrp
                from permit in pgrp.DefaultIfEmpty()
                join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id into pltgrp
                from permitType in pltgrp.DefaultIfEmpty()
                where inspPermit.InspectionId == inspectionId
                    && inspPermit.InspectedShipType == role.ToString()
                select new InspectionPermitDTO
                {
                    Id = inspPermit.Id,
                    CheckValue = Enum.Parse<InspectionToggleTypesEnum>(inspPermit.CheckPermitLicenseMatches),
                    Description = inspPermit.Description,
                    From = permit != null ? permit.PermitValidFrom : null,
                    To = permit != null ? permit.PermitValidTo : null,
                    PermitLicenseId = inspPermit.PermitId,
                    PermitNumber = permit != null ? permit.RegistrationNum : inspPermit.UnregisteredPermitNum,
                    TypeId = permitType != null ? permitType.Id : null,
                    TypeName = permitType != null ? permitType.ShortName : null,
                }
            ).ToList();

            return result;
        }

        protected List<InspectionPermitDTO> GetPermitLicenses(int inspectionId, SubjectRoleEnum role)
        {
            List<InspectionPermitDTO> result = (
                from inspPermit in Db.InspectionPermitLicenses
                join permitLicense in Db.CommercialFishingPermitLicensesRegisters on inspPermit.PermitLicenseId equals permitLicense.Id into plgrp
                from permitLicense in plgrp.DefaultIfEmpty()
                join permit in Db.CommercialFishingPermitRegisters on permitLicense.PermitId equals permit.Id into pgrp
                from permit in pgrp.DefaultIfEmpty()
                join permitLicenseType in Db.NcommercialFishingPermitLicenseTypes on permitLicense.PermitLicenseTypeId equals permitLicenseType.Id into pltgrp
                from permitLicenseType in pltgrp.DefaultIfEmpty()
                where inspPermit.InspectionId == inspectionId
                    && inspPermit.InspectedShipType == role.ToString()
                select new InspectionPermitDTO
                {
                    Id = inspPermit.Id,
                    CheckValue = Enum.Parse<InspectionToggleTypesEnum>(inspPermit.CheckPermitLicenseMatches),
                    Description = inspPermit.Description,
                    From = permitLicense != null ? permitLicense.PermitLicenseValidFrom : null,
                    To = permitLicense != null ? permitLicense.PermitLicenseValidTo : null,
                    PermitLicenseId = inspPermit.PermitLicenseId,
                    LicenseNumber = permitLicense != null ? permitLicense.RegistrationNum : inspPermit.UnregisteredLicenseNum,
                    PermitNumber = permit != null ? permit.RegistrationNum : null,
                    TypeId = permitLicenseType != null ? permitLicenseType.Id : null,
                    TypeName = permitLicenseType != null ? permitLicenseType.ShortName : null,
                }
            ).ToList();

            return result;
        }

        protected List<InspectionLogBookDTO> GetLogBooks(int inspectionId, SubjectRoleEnum role)
        {
            List<InspectionLogBookDTO> result = (
                from inspLogBook in Db.InspectionLogBookPages
                join logBook in Db.LogBooks on inspLogBook.LogBookId equals logBook.Id into lbgrp
                from logBook in lbgrp.DefaultIfEmpty()
                join logBookPage in Db.ShipLogBookPages on inspLogBook.ShipLogBookPageId equals logBookPage.Id into lbpgrp
                from logBookPage in lbpgrp.DefaultIfEmpty()
                where inspLogBook.InspectionId == inspectionId
                    && inspLogBook.InspectedShipType == role.ToString()
                select new InspectionLogBookDTO
                {
                    Id = inspLogBook.Id,
                    CheckValue = Enum.Parse<InspectionToggleTypesEnum>(inspLogBook.CheckLogBookMatches),
                    Description = inspLogBook.Description,
                    EndPage = logBook != null ? logBook.EndPageNum : null,
                    StartPage = logBook != null ? logBook.StartPageNum : null,
                    From = logBook != null ? logBook.IssueDate : null,
                    Number = logBook != null ? logBook.LogNum : inspLogBook.UnregisteredLogBookNum,
                    LogBookId = inspLogBook.LogBookId,
                    PageId = inspLogBook.ShipLogBookPageId,
                    PageNum = logBookPage != null ? logBookPage.PageNum : inspLogBook.UnregisteredPageNum,
                }
            ).ToList();

            return result;
        }

        protected List<InspectionCatchMeasureDTO> GetCatchMeasures(int inspectionId, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
        {
            DateTime now = DateTime.Now;

            List<InspectionCatchMeasureDTO> result = (
                from catchMeasure in Db.InspectionCatchMeasures
                join page in Db.InspectionLogBookPages on catchMeasure.InspectedLogBookPageId equals page.Id into pgrp
                from page in pgrp.DefaultIfEmpty()
                join ship in Db.ShipsRegister on page.ShipId equals ship.Id into sgrp
                from ship in sgrp.DefaultIfEmpty()
                join unregShip in Db.UnregisteredVessels on page.UnregisteredShipId equals unregShip.Id into usgrp
                from unregShip in usgrp.DefaultIfEmpty()
                where catchMeasure.IsActive
                      && catchMeasure.InspectionId == inspectionId
                      && catchMeasure.InspectedShipType == role.ToString()
                select new InspectionCatchMeasureDTO
                {
                    Id = catchMeasure.Id,
                    CatchInspectionTypeId = catchMeasure.CatchInspectionTypeId,
                    FishId = catchMeasure.FishId,
                    AllowedDeviation = catchMeasure.AllowedDeviation,
                    CatchZoneId = catchMeasure.CatchZoneId,
                    IsTaken = catchMeasure.IsTaken,
                    Action = catchMeasure.IsStored == null
                        ? null
                        : catchMeasure.IsStored == true
                        ? CatchActionEnum.Stored
                        : catchMeasure.IsDestroyed == true
                        ? CatchActionEnum.Destroyed
                        : catchMeasure.IsDonated == true
                        ? CatchActionEnum.Donated
                        : CatchActionEnum.Returned,
                    CatchQuantity = catchMeasure.CatchQuantity,
                    StorageLocation = catchMeasure.StorageLocation,
                    UnloadedQuantity = catchMeasure.UnloadedQuantity,
                    AverageSize = catchMeasure.AverageSize,
                    FishSexId = catchMeasure.FishSexId,
                    CatchCount = catchMeasure.CatchCount,
                    TurbotSizeGroupId = catchMeasure.TurbotSizeGroupId,
                    OriginShip = ship != null ? new VesselDuringInspectionDTO
                    {
                        ShipId = ship.Id,
                        UnregisteredVesselId = null,
                        IsRegistered = true,
                        Name = ship.Name,
                        ExternalMark = ship.ExternalMark,
                        CFR = ship.Cfr,
                        UVI = ship.Uvi,
                        RegularCallsign = ship.IrcscallSign,
                        MMSI = ship.Mmsi,
                        FlagCountryId = ship.FlagCountryId,
                        PatrolVehicleTypeId = null,
                        VesselTypeId = ship.VesselTypeId,
                        ShipAssociationId = ship.ShipAssociationId,
                        IsActive = ship.ValidFrom <= now && ship.ValidTo >= now,
                    }
                    : unregShip != null ? new VesselDuringInspectionDTO
                    {
                        ShipId = null,
                        UnregisteredVesselId = unregShip.Id,
                        IsRegistered = false,
                        Name = unregShip.Name,
                        ExternalMark = unregShip.ExternalMark,
                        CFR = unregShip.Cfr,
                        UVI = unregShip.Uvi,
                        RegularCallsign = unregShip.IrcscallSign,
                        MMSI = unregShip.Mmsi,
                        FlagCountryId = unregShip.FlagCountryId,
                        PatrolVehicleTypeId = null,
                        VesselTypeId = unregShip.VesselTypeId,
                        InstitutionId = unregShip.InstitutionId,
                        IsActive = unregShip.IsActive,
                    } : null,
                }
            ).ToList();

            return result;
        }

        protected List<InspectedDeclarationCatchDTO> GetDeclarationCatchMeasures(int inspectionId)
        {
            List<InspectedDeclarationCatchDTO> result = (
                from fish in Db.InspectionCatchMeasures
                join page in Db.InspectionLogBookPages on fish.InspectedLogBookPageId equals page.Id
                join ship in Db.ShipsRegister on page.ShipId equals ship.Id into sgrp
                from ship in sgrp.DefaultIfEmpty()
                join unregShip in Db.UnregisteredVessels on page.UnregisteredShipId equals unregShip.Id into usgrp
                from unregShip in usgrp.DefaultIfEmpty()
                where fish.InspectionId == inspectionId
                select new InspectedDeclarationCatchDTO
                {
                    Id = fish.Id,
                    CatchCount = fish.CatchCount,
                    CatchQuantity = fish.CatchQuantity,
                    PresentationId = fish.PresentationId,
                    UnloadedQuantity = fish.UnloadedQuantity,
                    CatchTypeId = fish.CatchInspectionTypeId,
                    CatchZoneId = fish.CatchZoneId,
                    FishTypeId = fish.FishId,
                    LogBookType = page.LogBookType == null ? null : Enum.Parse<DeclarationLogBookTypeEnum>(page.LogBookType),
                    LogBookPageId = page.LogBookType == nameof(DeclarationLogBookTypeEnum.ShipLogBook)
                        ? page.ShipLogBookPageId
                        : page.LogBookType == nameof(DeclarationLogBookTypeEnum.TransportationLogBook)
                        ? page.TransportationLogBookPageId
                        : page.LogBookType == nameof(DeclarationLogBookTypeEnum.FirstSaleLogBook)
                        ? page.FirstSaleLogBookPageId
                        : page.LogBookType == nameof(DeclarationLogBookTypeEnum.AdmissionLogBook)
                        ? page.AdmissionLogBookPageId
                        : page.LogBookType == nameof(DeclarationLogBookTypeEnum.AquacultureLogBook)
                        ? page.AquacultureLogBookPageId
                        : null,
                    UnregisteredPageDate = page.UnregisteredPageDate,
                    UnregisteredPageNum = page.UnregisteredPageNum,
                    AquacultureId = page.AquacultureId,
                    UnregisteredEntityData = page.UnregisteredEntityData,
                    OriginShip = ship != null ? new VesselDuringInspectionDTO
                    {
                        ShipId = ship.Id,
                        IsRegistered = true,
                        Name = ship.Name,
                        ExternalMark = ship.ExternalMark,
                        CFR = ship.Cfr,
                        UVI = ship.Uvi,
                        RegularCallsign = ship.IrcscallSign,
                        MMSI = ship.Mmsi,
                        FlagCountryId = ship.FlagCountryId,
                        VesselTypeId = ship.VesselTypeId,
                    } : unregShip != null ? new VesselDuringInspectionDTO
                    {
                        UnregisteredVesselId = unregShip.Id,
                        IsRegistered = false,
                        Name = unregShip.Name,
                        ExternalMark = unregShip.ExternalMark,
                        CFR = unregShip.Cfr,
                        UVI = unregShip.Uvi,
                        RegularCallsign = unregShip.IrcscallSign,
                        MMSI = unregShip.Mmsi,
                        FlagCountryId = unregShip.FlagCountryId,
                        VesselTypeId = unregShip.VesselTypeId,
                        InstitutionId = unregShip.InstitutionId,
                    } : null
                }
            ).ToList();

            return result;
        }

        protected List<InspectedFishingGearDTO> GetFishingGears(int inspectionId)
        {
            var fishingGears = (
                from ifg in Db.InspectedFishingGears
                join pfg in Db.FishingGearRegisters on ifg.RegisteredFishingGearId equals pfg.Id into pfgGroup
                from pfg in pfgGroup.DefaultIfEmpty()
                join pfgt in Db.NfishingGears on pfg.FishingGearTypeId equals pfgt.Id into pfgtGroup
                from pfgt in pfgtGroup.DefaultIfEmpty()
                join ifgn in Db.FishingGearRegisters on ifg.InspectedFishingGearId equals ifgn.Id into ifgnGroup
                from ifgn in ifgnGroup.DefaultIfEmpty()
                join ifgnt in Db.NfishingGears on ifgn.FishingGearTypeId equals ifgnt.Id into ifgntGroup
                from ifgnt in ifgntGroup.DefaultIfEmpty()
                where ifg.InspectionId == inspectionId
                    && ifg.IsActive
                select new
                {
                    ifg.HasAttachedAppliances,
                    ifg.CheckInspectedMatchingRegisteredGear,
                    PermittedFishingGear = pfg == null ? null : new FishingGearDTO
                    {
                        Id = pfg.Id,
                        TypeId = pfg.FishingGearTypeId,
                        Type = $"{pfgt.Code} - {pfgt.Name}",
                        PermitId = pfg.PermitLicenseId,
                        Count = pfg.GearCount,
                        Length = pfg.Length,
                        Height = pfg.Height,
                        NetEyeSize = pfg.NetEyeSize,
                        HookCount = pfg.HookCount,
                        Description = pfg.Description,
                        TowelLength = pfg.TowelLength,
                        HouseLength = pfg.HouseLength,
                        HouseWidth = pfg.HouseWidth,
                        CordThickness = pfg.CordThickness,
                        HasPingers = pfg.HasPinger,
                        LineCount = pfg.LineCount,
                        NetNominalLength = pfg.NetNominalLength,
                        NetsInFleetCount = pfg.NumberOfNetsInFleet,
                        TrawlModel = pfg.TrawlModel,
                        IsActive = pfg.IsActive
                    },
                    InspectedFishingGear = ifgn == null ? null : new FishingGearDTO
                    {
                        Id = ifgn.Id,
                        TypeId = ifgn.FishingGearTypeId,
                        Type = $"{ifgnt.Code} - {ifgnt.Name}",
                        PermitId = ifgn.PermitLicenseId,
                        Count = ifgn.GearCount,
                        Length = ifgn.Length,
                        Height = ifgn.Height,
                        NetEyeSize = ifgn.NetEyeSize,
                        HookCount = ifgn.HookCount,
                        Description = ifgn.Description,
                        TowelLength = ifgn.TowelLength,
                        HouseLength = ifgn.HouseLength,
                        HouseWidth = ifgn.HouseWidth,
                        CordThickness = ifgn.CordThickness,
                        HasPingers = ifgn.HasPinger,
                        LineCount = ifgn.LineCount,
                        NetNominalLength = ifgn.NetNominalLength,
                        NetsInFleetCount = ifgn.NumberOfNetsInFleet,
                        TrawlModel = ifgn.TrawlModel,
                        IsActive = ifgn.IsActive
                    },
                    ifg.IsActive,
                }
            ).ToList();
            List<InspectedFishingGearDTO> result = new(fishingGears.Count);

            FishingGearService.MapFishingGearMarksAndPingers(fishingGears.Select(f => f.InspectedFishingGear).Where(f => f != null).ToList());
            FishingGearService.MapFishingGearMarksAndPingers(fishingGears.Select(f => f.PermittedFishingGear).Where(f => f != null).ToList());

            foreach (var fishingGear in fishingGears)
            {
                if (fishingGear.PermittedFishingGear != null)
                {
                    fishingGear.PermittedFishingGear.MarksNumbers = string.Join(", ", fishingGear.PermittedFishingGear.Marks.Where(x => x.IsActive).Select(x => x.Number));
                }
                if (fishingGear.InspectedFishingGear != null)
                {
                    fishingGear.InspectedFishingGear.MarksNumbers = string.Join(", ", fishingGear.InspectedFishingGear.Marks.Where(x => x.IsActive).Select(x => x.Number));
                }

                result.Add(new InspectedFishingGearDTO
                {
                    HasAttachedAppliances = fishingGear.HasAttachedAppliances,
                    CheckInspectedMatchingRegisteredGear = Enum.TryParse(fishingGear.CheckInspectedMatchingRegisteredGear?.ToString(), out InspectedFishingGearEnum toggles)
                        ? new InspectedFishingGearEnum?(toggles)
                        : null,
                    InspectedFishingGear = fishingGear.InspectedFishingGear,
                    PermittedFishingGear = fishingGear.PermittedFishingGear,
                    IsActive = fishingGear.IsActive
                });
            }

            return result;
        }

        protected PortVisitDTO GetLastPort(int inspectionId, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
        {
            PortVisitDTO result = (
                from portVisit in Db.InspectionLastPortVisits
                join port in Db.Nports on portVisit.PortId equals port.Id into pgrp
                from port in pgrp.DefaultIfEmpty()
                where portVisit.IsActive
                    && portVisit.InspectionId == inspectionId
                    && portVisit.InspectedShipType == role.ToString()
                select new PortVisitDTO
                {
                    PortId = portVisit.PortId,
                    PortName = port != null ? port.Name : portVisit.UnregisteredPortName,
                    PortCountryId = portVisit.UnregisteredPortCountryId,
                    VisitDate = portVisit.VisitDate,
                    IsActive = portVisit.IsActive
                }).SingleOrDefault();

            return result;
        }

        protected void AddPermitLicenses(InspectionRegister inspection, List<InspectionPermitDTO> permits, SubjectRoleEnum role)
        {
            if (permits != null)
            {
                foreach (InspectionPermitDTO permit in permits)
                {
                    InspectionPermitLicense permLicen = new()
                    {
                        UnregisteredLicenseNum = permit.LicenseNumber,
                        CheckPermitLicenseMatches = (permit.CheckValue ?? InspectionToggleTypesEnum.Y).ToString(),
                        Description = permit.Description,
                        InspectedShipType = role.ToString(),
                        Inspection = inspection,
                        PermitLicenseId = permit.PermitLicenseId,
                        IsActive = true,
                    };

                    Db.InspectionPermitLicenses.Add(permLicen);
                }
            }
        }

        protected void AddPermits(InspectionRegister inspection, List<InspectionPermitDTO> permits, SubjectRoleEnum role)
        {
            if (permits != null)
            {
                foreach (InspectionPermitDTO permit in permits)
                {
                    InspectionPermit permLicen = new()
                    {
                        UnregisteredPermitNum = permit.PermitNumber,
                        CheckPermitLicenseMatches = (permit.CheckValue ?? InspectionToggleTypesEnum.Y).ToString(),
                        Description = permit.Description,
                        InspectedShipType = role.ToString(),
                        Inspection = inspection,
                        PermitId = permit.PermitLicenseId,
                        IsActive = true,
                    };

                    Db.InspectionPermits.Add(permLicen);
                }
            }
        }

        protected void AddLogBooks(InspectionRegister inspection, List<InspectionLogBookDTO> logBooks, SubjectRoleEnum role)
        {
            if (logBooks != null)
            {
                foreach (InspectionLogBookDTO logBook in logBooks)
                {
                    InspectionLogBookPage logBookPage = new()
                    {
                        CheckLogBookMatches = (logBook.CheckValue ?? InspectionToggleTypesEnum.Y).ToString(),
                        Description = logBook.Description,
                        InspectedShipType = role.ToString(),
                        Inspection = inspection,
                        LogBookId = logBook.LogBookId,
                        ShipLogBookPageId = logBook.PageId,
                        UnregisteredLogBookNum = logBook.Number,
                        UnregisteredPageNum = logBook.PageNum,
                        IsActive = true,
                    };

                    Db.InspectionLogBookPages.Add(logBookPage);
                }
            }
        }

        protected void AddPortVisit(InspectionRegister inspection, PortVisitDTO portVisit, SubjectRoleEnum role)
        {
            if (portVisit != null)
            {
                InspectionLastPortVisit portVisitDb = new()
                {
                    InspectionId = inspection.Id,
                    PortId = portVisit.PortId,
                    InspectedShipType = role.ToString(),
                    UnregisteredPortName = portVisit.PortName,
                    UnregisteredPortCountryId = portVisit.PortCountryId,
                    VisitDate = portVisit.VisitDate,
                    IsActive = true,
                };

                Db.InspectionLastPortVisits.Add(portVisitDb);
            }
        }

        protected void AddInspectionVesselActivities(InspectionRegister inspection, List<InspectionVesselActivityNomenclatureDTO> vesselActivities)
        {
            if (vesselActivities != null)
            {
                foreach (InspectionVesselActivityNomenclatureDTO activity in vesselActivities)
                {
                    InspectionVesselActivity activityDb = new()
                    {
                        Inspection = inspection,
                        VesselActivityId = activity.Value,
                        ActivityDescr = activity.Description,
                        IsActive = true,
                    };
                    Db.InspectionVesselActivities.Add(activityDb);
                }
            }
        }

        protected void AddObservationTools(InspectionRegister inspection, List<InspectionObservationToolDTO> observationTools)
        {
            if (observationTools != null)
            {
                foreach (InspectionObservationToolDTO tool in observationTools)
                {
                    InspectionObservationTool obsToolDb = new()
                    {
                        IsOnBoard = tool.IsOnBoard,
                        Inspection = inspection,
                        ObservationToolId = tool.ObservationToolId,
                        ObservationToolDesc = tool.Description,
                        IsActive = true,
                    };
                    Db.InspectionObservationTools.Add(obsToolDb);
                }
            }
        }

        protected void AddCatchMeasures(InspectionRegister inspection, List<InspectionCatchMeasureDTO> catchMeasures, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
        {
            if (catchMeasures != null)
            {
                foreach (InspectionCatchMeasureDTO catchMeasure in catchMeasures)
                {
                    InspectionLogBookPage page = null;

                    if (catchMeasure.OriginShip != null)
                    {
                        UnregisteredVessel unregisteredShip = null;

                        if (catchMeasure.OriginShip.IsRegistered == false)
                        {
                            unregisteredShip = AddUnregisteredShip(catchMeasure.OriginShip);
                        }

                        page = new InspectionLogBookPage
                        {
                            InspectionId = inspection.Id,
                            ShipId = catchMeasure.OriginShip.ShipId,
                            UnregisteredShip = unregisteredShip,
                            InspectedShipType = role.ToString(),
                            CheckLogBookMatches = nameof(InspectionToggleTypesEnum.Y),
                        };
                    }

                    InspectionCatchMeasure measureDb = new()
                    {
                        InspectedLogBookPage = page,
                        Inspection = inspection,
                        CatchInspectionTypeId = catchMeasure.CatchInspectionTypeId,
                        FishId = catchMeasure.FishId,
                        CatchQuantity = catchMeasure.CatchQuantity,
                        CatchZoneId = catchMeasure.CatchZoneId,
                        AllowedDeviation = catchMeasure.AllowedDeviation,
                        StorageLocation = catchMeasure.StorageLocation,
                        UnloadedQuantity = catchMeasure.UnloadedQuantity,
                        IsTaken = catchMeasure.IsTaken,
                        IsStored = catchMeasure.Action == null ? null : catchMeasure.Action == CatchActionEnum.Stored,
                        IsDestroyed = catchMeasure.Action == null ? null : catchMeasure.Action == CatchActionEnum.Destroyed,
                        IsDonated = catchMeasure.Action == null ? null : catchMeasure.Action == CatchActionEnum.Donated,
                        InspectedShipType = role.ToString(),
                        AverageSize = catchMeasure.AverageSize,
                        FishSexId = catchMeasure.FishSexId,
                        CatchCount = catchMeasure.CatchCount,
                        TurbotSizeGroupId = catchMeasure.TurbotSizeGroupId,
                        IsActive = true
                    };
                    Db.InspectionCatchMeasures.Add(measureDb);
                }
            }
        }

        protected void AddDeclarationCatchMeasures(InspectionRegister inspection, List<InspectedDeclarationCatchDTO> catchMeasures)
        {
            if (catchMeasures != null)
            {
                foreach (InspectedDeclarationCatchDTO fish in catchMeasures)
                {
                    UnregisteredVessel unregisteredShip = null;

                    if (fish.OriginShip?.IsRegistered == false)
                    {
                        unregisteredShip = AddUnregisteredShip(fish.OriginShip);
                    }

                    InspectionLogBookPage page = new()
                    {
                        InspectionId = inspection.Id,
                        ShipId = fish.OriginShip?.ShipId,
                        UnregisteredShip = unregisteredShip,
                        LogBookType = fish.LogBookType?.ToString(),
                        UnregisteredPageDate = fish.UnregisteredPageDate,
                        UnregisteredPageNum = fish.UnregisteredPageNum,
                        InspectedShipType = nameof(SubjectRoleEnum.Inspected),
                        CheckLogBookMatches = nameof(InspectionToggleTypesEnum.Y),
                        AquacultureId = fish.AquacultureId,
                        UnregisteredEntityData = fish.UnregisteredEntityData,
                    };

                    switch (fish.LogBookType.Value)
                    {
                        case DeclarationLogBookTypeEnum.FirstSaleLogBook:
                            page.FirstSaleLogBookPageId = fish.LogBookPageId;
                            break;
                        case DeclarationLogBookTypeEnum.TransportationLogBook:
                            page.TransportationLogBookPageId = fish.LogBookPageId;
                            break;
                        case DeclarationLogBookTypeEnum.AdmissionLogBook:
                            page.AdmissionLogBookPageId = fish.LogBookPageId;
                            break;
                        case DeclarationLogBookTypeEnum.ShipLogBook:
                            page.ShipLogBookPageId = fish.LogBookPageId;
                            break;
                        case DeclarationLogBookTypeEnum.AquacultureLogBook:
                            page.AquacultureLogBookPageId = fish.LogBookPageId;
                            break;
                    }

                    Db.InspectionCatchMeasures.Add(new InspectionCatchMeasure
                    {
                        InspectionId = inspection.Id,
                        InspectedLogBookPage = page,
                        FishId = fish.FishTypeId,
                        CatchInspectionTypeId = fish.CatchTypeId,
                        CatchCount = fish.CatchCount,
                        CatchQuantity = fish.CatchQuantity,
                        CatchZoneId = fish.CatchZoneId,
                        PresentationId = fish.PresentationId,
                        UnloadedQuantity = fish.UnloadedQuantity,
                        InspectedShipType = nameof(SubjectRoleEnum.Inspected)
                    });
                }
            }
        }

        protected void AddFishingGears(InspectionRegister inspection, List<InspectedFishingGearDTO> inspectedFishingGears)
        {
            if (inspectedFishingGears != null)
            {
                foreach (InspectedFishingGearDTO inspectedGear in inspectedFishingGears)
                {
                    FishingGearRegister gear = null;

                    if (inspectedGear.InspectedFishingGear != null)
                    {
                        FishingGearDTO inspFishingGear = inspectedGear.InspectedFishingGear;

                        gear = new FishingGearRegister
                        {
                            PermitLicenseId = null,
                            FishingGearTypeId = inspFishingGear.TypeId,
                            GearCount = inspFishingGear.Count,
                            NetEyeSize = inspFishingGear.NetEyeSize,
                            HookCount = inspFishingGear.HookCount,
                            Length = inspFishingGear.Length,
                            Height = inspFishingGear.Height,
                            Description = inspFishingGear.Description,
                            HouseLength = inspFishingGear.HouseLength,
                            HouseWidth = inspFishingGear.HouseWidth,
                            TowelLength = inspFishingGear.TowelLength,
                            CordThickness = inspFishingGear.CordThickness,
                            InspectionId = inspection.Id,
                            IsActive = true,
                            HasPinger = inspFishingGear.HasPingers,
                        };
                        Db.FishingGearRegisters.Add(gear);

                        if (inspFishingGear.Marks != null)
                        {
                            foreach (FishingGearMarkDTO mark in inspFishingGear.Marks)
                            {
                                FishingGearMark entry = new()
                                {
                                    FishingGear = gear,
                                    MarkNum = mark.Number,
                                    MarkStatusId = mark.StatusId,
                                    Inspection = inspection,
                                    IsActive = true,
                                };
                                Db.FishingGearMarks.Add(entry);
                            }
                        }

                        if (inspFishingGear.Pingers != null)
                        {
                            foreach (FishingGearPingerDTO pinger in inspFishingGear.Pingers)
                            {
                                FishingGearPinger entry = new()
                                {
                                    FishingGear = gear,
                                    PingerNum = pinger.Number,
                                    PingerStatusId = pinger.StatusId,
                                    IsActive = true,
                                    Brand = pinger.Brand,
                                    Model = pinger.Model,
                                };
                                Db.FishingGearPingers.Add(entry);
                            }
                        }
                    }

                    InspectedFishingGear inspectedGearDb = new()
                    {
                        Inspection = inspection,
                        HasAttachedAppliances = inspectedGear.HasAttachedAppliances,
                        InspectedFishingGearNavigation = gear,
                        RegisteredFishingGearId = inspectedGear.PermittedFishingGear?.Id,
                        CheckInspectedMatchingRegisteredGear = inspectedGear.CheckInspectedMatchingRegisteredGear?.ToString()[0],
                        IsActive = true,
                    };
                    Db.InspectedFishingGears.Add(inspectedGearDb);
                }
            }
        }

        protected void AddInspectedPersons(InspectionRegister inspection, List<InspectionSubjectPersonnelDTO> inspectedPersonnel, SubjectRoleEnum roleEnum)
        {
            if (inspectedPersonnel != null)
            {
                List<NomenclatureDTO> inspectedPersonTypes = Db.NinspectedPersonTypes
                    .Select(f => new NomenclatureDTO
                    {
                        Value = f.Id,
                        Code = f.Code
                    })
                    .ToList();

                foreach (InspectionSubjectPersonnelDTO inspectedPerson in inspectedPersonnel)
                {
                    InspectedPerson inspectedPersonDb = new()
                    {
                        Inspection = inspection,
                        IsActive = true,
                        InspectedPersonTypeId = inspectedPersonTypes
                            .Find(f => f.Code == inspectedPerson.Type.ToString()).Value,
                        InspectedShipType = roleEnum.ToString()
                    };

                    if (inspectedPerson.IsRegistered && inspectedPerson.Id.HasValue)
                    {
                        switch (inspectedPerson.Type)
                        {
                            case InspectedPersonTypeEnum.OwnerPers:
                            case InspectedPersonTypeEnum.LicUsrPers:
                            case InspectedPersonTypeEnum.ReprsPers:
                            case InspectedPersonTypeEnum.ActualOwn:
                            case InspectedPersonTypeEnum.Driver:
                            case InspectedPersonTypeEnum.RegBuyer:
                            case InspectedPersonTypeEnum.CaptFshmn:
                            case InspectedPersonTypeEnum.Importer:
                                inspectedPersonDb.Person = Db.Persons
                                    .FirstOrDefault(f => f.Id == inspectedPerson.Id);
                                break;
                            case InspectedPersonTypeEnum.OwnerLegal:
                            case InspectedPersonTypeEnum.LicUsrLgl:
                                inspectedPersonDb.Legal = Db.Legals
                                    .FirstOrDefault(f => f.Id == inspectedPerson.Id);
                                break;
                        }

                        if (inspectedPerson.EntryId.HasValue)
                        {
                            switch (inspectedPerson.Type)
                            {
                                case InspectedPersonTypeEnum.RegBuyer:
                                case InspectedPersonTypeEnum.OwnerBuyer:
                                    inspectedPersonDb.Buyer = Db.BuyerRegisters
                                        .FirstOrDefault(f => f.Id == inspectedPerson.EntryId.Value);
                                    break;
                                case InspectedPersonTypeEnum.CaptFshmn:
                                    inspectedPersonDb.CaptainFishermen = Db.FishermenRegisters
                                        .FirstOrDefault(f => f.Id == inspectedPerson.EntryId.Value);
                                    break;
                            }
                        }

                        if (inspectedPerson.RegisteredAddress != null)
                        {
                            InspectionSubjectAddressDTO address = inspectedPerson.RegisteredAddress;

                            inspectedPersonDb.Address = new Address
                            {
                                ApartmentNum = address.ApartmentNum,
                                BlockNum = address.BlockNum,
                                CountryId = address.CountryId,
                                DistrictId = address.DistrictId,
                                EntranceNum = address.EntranceNum,
                                FloorNum = address.FloorNum,
                                MunicipalityId = address.MunicipalityId,
                                PopulatedAreaId = address.PopulatedAreaId,
                                StreetNum = address.StreetNum,
                                Region = address.Region,
                                Street = address.Street,
                                PostCode = address.PostCode,
                                IsActive = true,
                            };
                        }
                    }
                    else
                    {
                        inspectedPersonDb.UnregisteredPerson = AddUnregisteredPerson(inspectedPerson);
                    }

                    Db.InspectedPersons.Add(inspectedPersonDb);
                }
            }
        }

        protected void AddInspectionChecks(InspectionRegister inspection, List<InspectionCheckDTO> checks, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
        {
            if (checks != null)
            {
                foreach (InspectionCheckDTO check in checks)
                {
                    InspectionCheck checkDb = new()
                    {
                        InspectionId = inspection.Id,
                        CheckTypeId = check.CheckTypeId.Value,
                        CheckValue = check.CheckValue.ToString(),
                        Description = check.Description,
                        UnregisteredObjectIdentifier = check.Number,
                        InspectedShipType = role.ToString(),
                        IsActive = true,
                    };
                    Db.InspectionChecks.Add(checkDb);
                }
            }
        }

        protected UnregisteredPerson AddUnregisteredPerson(UnregisteredPersonDTO itemDTO)
        {
            if (itemDTO == null)
            {
                return null;
            }

            UnregisteredPerson unregPerson;

            if (itemDTO.IsLegal)
            {
                unregPerson = new UnregisteredPerson
                {
                    FirstName = itemDTO.FirstName,
                    Address = itemDTO.Address,
                    EgnLnc = itemDTO.Eik,
                    IdentifierType = nameof(IdentifierTypeEnum.LEGAL),
                    CitizenshipCountryId = itemDTO.CitizenshipId,
                    HasBulgarianAddressRegistration = itemDTO.HasBulgarianAddressRegistration,
                    Comments = itemDTO.Comment,
                    IsActive = true
                };
            }
            else
            {
                unregPerson = new UnregisteredPerson
                {
                    FirstName = itemDTO.FirstName,
                    MiddleName = itemDTO.MiddleName,
                    LastName = itemDTO.LastName,
                    Address = itemDTO.Address,
                    EgnLnc = itemDTO.EgnLnc?.EgnLnc,
                    IdentifierType = itemDTO.EgnLnc?.IdentifierType.ToString(),
                    CitizenshipCountryId = itemDTO.CitizenshipId,
                    HasBulgarianAddressRegistration = itemDTO.HasBulgarianAddressRegistration,
                    Comments = itemDTO.Comment,
                    IsActive = true
                };
            }

            Db.UnregisteredPersons.Add(unregPerson);

            return unregPerson;
        }

        protected UnregisteredVessel AddUnregisteredShip(VesselDTO itemDTO)
        {
            UnregisteredVessel itemDB = null;
            if (itemDTO.UnregisteredVesselId != null)
            {
                itemDB = Db.UnregisteredVessels.FirstOrDefault(x => x.Id == itemDTO.UnregisteredVesselId);
            }

            if (itemDB == null)
            {
                itemDB = new UnregisteredVessel
                {
                    Name = itemDTO.Name,
                    FlagCountryId = itemDTO.FlagCountryId,
                    ExternalMark = itemDTO.ExternalMark,
                    PatrolVehicleTypeId = itemDTO.PatrolVehicleTypeId,
                    Cfr = itemDTO.CFR,
                    Uvi = itemDTO.UVI,
                    IrcscallSign = itemDTO.RegularCallsign,
                    Mmsi = itemDTO.MMSI,
                    VesselTypeId = itemDTO.VesselTypeId,
                    InstitutionId = itemDTO.InstitutionId,
                    IsActive = true
                };
                Db.UnregisteredVessels.Add(itemDB);
            }

            return itemDB;
        }

        private List<InspectionObservationTextDTO> GetObservationTexts(int inspectionId)
        {
            List<InspectionObservationTextDTO> result = (
                from obsText in Db.InspectionObservationTexts
                join nomObsText in Db.NinspectionObservationTextCategories on obsText.InspectionTextCategoryId equals nomObsText.Id
                where obsText.InspectionId == inspectionId
                    && obsText.IsActive
                select new InspectionObservationTextDTO
                {
                    Id = obsText.Id,
                    Text = obsText.ObservationsOrViolationsText,
                    Category = Enum.Parse<InspectionObservationCategoryEnum>(nomObsText.Code)
                }
            ).ToList();

            return result;
        }

        private List<VesselDuringInspectionDTO> GetPatrolVehicles(int inspectionId, bool? active = null)
        {
            List<VesselDuringInspectionDTO> result = (
                from inspectionPatrolVehicle in Db.InspectionPatrolVehicles
                join unregisteredVessel in Db.UnregisteredVessels on inspectionPatrolVehicle.PatrolUnregisteredVesselId equals unregisteredVessel.Id
                where inspectionPatrolVehicle.InspectionId == inspectionId
                    && (active == null || inspectionPatrolVehicle.IsActive == active)
                select new VesselDuringInspectionDTO
                {
                    Id = inspectionPatrolVehicle.Id,
                    ShipId = null,
                    UnregisteredVesselId = unregisteredVessel.Id,
                    IsRegistered = true,
                    Name = unregisteredVessel.Name,
                    ExternalMark = unregisteredVessel.ExternalMark,
                    CFR = unregisteredVessel.Cfr,
                    UVI = unregisteredVessel.Uvi,
                    RegularCallsign = unregisteredVessel.IrcscallSign,
                    MMSI = unregisteredVessel.Mmsi,
                    FlagCountryId = unregisteredVessel.FlagCountryId,
                    PatrolVehicleTypeId = unregisteredVessel.PatrolVehicleTypeId,
                    VesselTypeId = unregisteredVessel.VesselTypeId,
                    IsActive = unregisteredVessel.IsActive,
                    Location = inspectionPatrolVehicle.PatrolVesselCoordinates != null
                        ? new LocationDTO { Longitude = inspectionPatrolVehicle.PatrolVesselCoordinates.X, Latitude = inspectionPatrolVehicle.PatrolVesselCoordinates.Y }
                        : null,
                    LocationText = inspectionPatrolVehicle.PatrolVesselCoordinates != null
                        ? new CoordinatesDMS(inspectionPatrolVehicle.PatrolVesselCoordinates.X, inspectionPatrolVehicle.PatrolVesselCoordinates.Y).ToDisplayString()
                        : null,
                    InstitutionId = unregisteredVessel.InstitutionId,
                }
            ).ToList();

            return result;
        }

        private List<InspectorDuringInspectionDTO> GetInspectors(int inspectionId)
        {
            List<InspectorDuringInspectionDTO> result = (
                from inspInspector in Db.InspectionInspectors
                join inspector in Db.Inspectors on inspInspector.InspectorId equals inspector.Id
                join institution in Db.Ninstitutions on inspector.InstitutionId equals institution.Id
                join user in Db.Users on inspector.UserId equals user.Id into userMatchTable
                from userMatch in userMatchTable.DefaultIfEmpty()
                join person in Db.Persons on userMatch.PersonId equals person.Id into personMatchTable
                from personMatch in personMatchTable.DefaultIfEmpty()
                join unregisteredPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregisteredPerson.Id into unregisteredPersonMatchTable
                from unregisteredPersonMatch in unregisteredPersonMatchTable.DefaultIfEmpty()
                where inspInspector.InspectionId == inspectionId
                    && inspInspector.IsActive
                orderby inspInspector.OrderNum
                select new InspectorDuringInspectionDTO
                {
                    Id = inspInspector.Id,
                    InspectorId = inspector.Id,
                    CardNum = inspector.InspectorCardNum,
                    HasIdentifiedHimself = inspInspector.HasIdentifiedHimself ?? false,
                    IsInCharge = inspInspector.IsInCharge,
                    UserId = userMatch != null ? new int?(userMatch.Id) : null,
                    UnregisteredPersonId = unregisteredPersonMatch != null ? new int?(unregisteredPersonMatch.Id) : null,
                    IsNotRegistered = userMatch == null,
                    InstitutionId = inspector.InstitutionId,
                    Institution = institution.Name,
                    FirstName = personMatch != null ? personMatch.FirstName : unregisteredPersonMatch.FirstName,
                    MiddleName = personMatch != null ? personMatch.MiddleName : unregisteredPersonMatch.MiddleName,
                    LastName = personMatch != null ? personMatch.LastName : unregisteredPersonMatch.LastName,
                    EgnLnc = new EgnLncDTO
                    {
                        EgnLnc = personMatch != null ? personMatch.EgnLnc : unregisteredPersonMatch.EgnLnc,
                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(personMatch != null ? personMatch.IdentifierType : unregisteredPersonMatch.IdentifierType),
                    },
                    CitizenshipId = personMatch != null ? personMatch.CitizenshipCountryId : unregisteredPersonMatch.CitizenshipCountryId,
                    IsActive = inspector.IsActive,
                }).ToList();

            return result;
        }

        private List<AuanViolatedRegulationDTO> GetViolatedRegulations(int inspectionId)
        {
            return (
                from reg in Db.ViolatedRegulations
                join section in Db.NlawSections on reg.LawSectionId equals section.Id into lawSection
                from section in lawSection.DefaultIfEmpty()
                where reg.InspectionId == inspectionId
                select new AuanViolatedRegulationDTO
                {
                    Id = reg.Id,
                    Article = reg.Article,
                    Paragraph = reg.Paragraph,
                    Section = reg.Section,
                    Letter = reg.Letter,
                    LawSectionId = reg.LawSectionId,
                    LawText = section != null ? section.LawText : null,
                    IsActive = reg.IsActive
                }).ToList();
        }

        private void AddInspectionPatrolVehicles(InspectionRegister inspection, List<VesselDuringInspectionDTO> patrolVehicles)
        {
            if (patrolVehicles != null)
            {
                foreach (VesselDuringInspectionDTO patrolVehicle in patrolVehicles)
                {
                    UnregisteredVessel vesselDb = null;
                    if (patrolVehicle.UnregisteredVesselId.HasValue)
                    {
                        vesselDb = Db.UnregisteredVessels.First(x => x.Id == patrolVehicle.UnregisteredVesselId);
                    }
                    else
                    {
                        vesselDb = new UnregisteredVessel
                        {
                            Name = patrolVehicle.Name,
                            FlagCountryId = patrolVehicle.FlagCountryId,
                            ExternalMark = patrolVehicle.ExternalMark,
                            PatrolVehicleTypeId = patrolVehicle.PatrolVehicleTypeId,
                            Cfr = patrolVehicle.CFR,
                            Uvi = patrolVehicle.UVI,
                            IrcscallSign = patrolVehicle.RegularCallsign,
                            Mmsi = patrolVehicle.MMSI,
                            VesselTypeId = patrolVehicle.VesselTypeId,
                            IsActive = true,
                            InstitutionId = patrolVehicle.InstitutionId,
                        };
                        Db.UnregisteredVessels.Add(vesselDb);
                    }

                    InspectionPatrolVehicle inspectionPatrolVehicleDb = new()
                    {
                        InspectionId = inspection.Id,
                        PatrolUnregisteredVessel = vesselDb,
                        PatrolVesselCoordinates = patrolVehicle.Location != null
                            ? new Point(patrolVehicle.Location.Longitude.Value, patrolVehicle.Location.Latitude.Value)
                            : null,
                        IsActive = true,
                    };
                    Db.InspectionPatrolVehicles.Add(inspectionPatrolVehicleDb);
                }
            }
        }

        private void AddInspectionInspectors(InspectionRegister inspection, List<InspectorDuringInspectionDTO> inspectors)
        {
            if (inspectors != null)
            {
                for (int i = 0; i < inspectors.Count; i++)
                {
                    InspectorDuringInspectionDTO inspector = inspectors[i];
                    Inspector inspectorDb = null;

                    if (inspector.InspectorId.HasValue)
                    {
                        inspectorDb = Db.Inspectors.First(x => x.Id == inspector.InspectorId);
                    }
                    else
                    {
                        UnregisteredPerson unregisteredPersonDb = null;

                        if (inspector.IsNotRegistered && inspector.UnregisteredPersonId == null)
                        {
                            unregisteredPersonDb = AddUnregisteredPerson(inspector);
                        }

                        inspectorDb = new Inspector
                        {
                            UnregisteredPerson = unregisteredPersonDb,
                            InstitutionId = inspector.InstitutionId,
                            InspectionSequenceNum = 1,
                            InspectorCardNum = inspector.CardNum,
                            IsActive = true
                        };
                        Db.Inspectors.Add(inspectorDb);
                    }

                    InspectionInspector inspectorInspectionDb = new()
                    {
                        Inspection = inspection,
                        Inspector = inspectorDb,
                        IsInCharge = inspector.IsInCharge,
                        OrderNum = (short)(inspector.IsInCharge ? 0 : i + 1),
                        IsActive = true,
                        HasIdentifiedHimself = inspector.HasIdentifiedHimself,
                    };
                    Db.InspectionInspectors.Add(inspectorInspectionDb);
                }
            }
        }

        private void AddInspectionObservationTexts(InspectionRegister inspection, List<InspectionObservationTextDTO> texts, InspectionTypesEnum inspectionType)
        {
            if (texts?.Count > 0)
            {
                var categories = (
                    from category in Db.NinspectionObservationTextCategories
                    join inspType in Db.NinspectionTypes on category.InspectionTypeId equals inspType.Id
                    where inspType.Code == inspectionType.ToString()
                    select new
                    {
                        category.Id,
                        Code = Enum.Parse<InspectionObservationCategoryEnum>(category.Code)
                    }
                ).ToList();

                foreach (InspectionObservationTextDTO text in texts)
                {
                    if (!string.IsNullOrWhiteSpace(text.Text))
                    {
                        InspectionObservationText textDb = new()
                        {
                            InspectionId = inspection.Id,
                            ObservationsOrViolationsText = text.Text,
                            InspectionTextCategoryId = categories.Find(f => f.Code == text.Category).Id,
                            IsActive = true,
                        };
                        Db.InspectionObservationTexts.Add(textDb);
                    }
                }
            }
        }

        private void AddViolatedRegulations(InspectionRegister inspection, List<AuanViolatedRegulationDTO> violatedRegulations)
        {
            if (violatedRegulations?.Count > 0)
            {
                foreach (AuanViolatedRegulationDTO regulation in violatedRegulations.Where(f => f.IsActive == true))
                {
                    ViolatedRegulation vr = new()
                    {
                        Inspection = inspection,
                        Article = regulation.Article,
                        Paragraph = regulation.Paragraph,
                        Section = regulation.Section,
                        Letter = regulation.Letter,
                        LawSectionId = regulation.LawSectionId,
                        IsActive = regulation.IsActive.Value,
                    };
                    Db.ViolatedRegulations.Add(vr);
                }
            }
        }

        private string GenerateReportNumber(Inspector inspector)
        {
            string idText = inspector.Id.ToString();

            string territoryNodeCode = HandleNumber(inspector.User?.UserInfo?.TerritoryUnit?.Code, idText);

            string cardNum = HandleNumber(inspector.InspectorCardNum, idText);

            inspector.InspectionSequenceNum++;

            Db.Inspectors.Update(inspector);

            string reportNum = territoryNodeCode + "-" + cardNum + "-" + inspector.InspectionSequenceNum.ToString("D3");

            int duplicateReportNums = Db.InspectionsRegister.Count(f => f.ReportNum.StartsWith(reportNum));

            if (duplicateReportNums > 0)
            {
                return reportNum + "#" + duplicateReportNums;
            }

            return reportNum;
        }

        private string HandleNumber(string number, string alternative)
        {
            if (number != null)
            {
                return number.Length > 3 ? number.Substring(0, 3) : number.PadRight(3, '0');
            }
            else if (alternative != null)
            {
                return HandleNumber(alternative, null);
            }
            else
            {
                return "000";
            }
        }

        protected static TDto AssignFromBase<TDto>(TDto dto, InspectionEditDTO inspectionBaseDTO)
            where TDto : InspectionEditDTO
        {
            dto.Id = inspectionBaseDTO.Id;
            dto.Files = inspectionBaseDTO.Files;
            dto.Inspectors = inspectionBaseDTO.Inspectors;
            dto.InspectionState = inspectionBaseDTO.InspectionState;
            dto.ReportNum = inspectionBaseDTO.ReportNum;
            dto.StartDate = inspectionBaseDTO.StartDate;
            dto.EndDate = inspectionBaseDTO.EndDate;
            dto.InspectionType = inspectionBaseDTO.InspectionType;
            dto.ByEmergencySignal = inspectionBaseDTO.ByEmergencySignal;
            dto.InspectorComment = inspectionBaseDTO.InspectorComment;
            dto.AdministrativeViolation = inspectionBaseDTO.AdministrativeViolation;
            dto.ActionsTaken = inspectionBaseDTO.ActionsTaken;
            dto.IsActive = inspectionBaseDTO.IsActive;
            dto.Personnel = inspectionBaseDTO.Personnel;
            dto.Checks = inspectionBaseDTO.Checks;
            dto.PatrolVehicles = inspectionBaseDTO.PatrolVehicles;
            dto.ObservationTexts = inspectionBaseDTO.ObservationTexts;
            dto.ViolatedRegulations = inspectionBaseDTO.ViolatedRegulations;
            return dto;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
