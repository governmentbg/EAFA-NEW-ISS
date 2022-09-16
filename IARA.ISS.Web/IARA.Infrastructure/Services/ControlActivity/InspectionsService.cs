using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.Mobile.Ships;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Infrastructure.Services.Mobile;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.ControlActivity;
using IARA.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services.ControlActivity
{
    public class InspectionsService : Service, IInspectionsService
    {
        private const string PersonSeparator = ", ";

        private readonly IFishingGearsService fishingGearService;
        private readonly IJasperReportExecutionService jasperReport;

        public InspectionsService(IARADbContext db, IFishingGearsService fishingGearService, IJasperReportExecutionService jasperReport)
            : base(db)
        {
            this.fishingGearService = fishingGearService;
            this.jasperReport = jasperReport;
        }

        #region Public

        public IQueryable<InspectionDTO> GetAll(InspectionsFilters filter, int? userId)
        {
            IQueryable<InspectionDTO> result;

            if (filter == null || !filter.HasAnyFilters(false))
            {
                result = GetAllNoFilter(filter?.ShowInactiveRecords != true, userId);
            }
            else if (filter.HasAnyFilters(true))
            {
                result = GetAllFilter(filter, userId);
            }
            else if (filter.HasFreeTextSearch())
            {
                result = GetAllFreeTextFilter(filter.FreeTextSearch, !filter.ShowInactiveRecords, userId);
            }
            else
            {
                result = Enumerable.Empty<InspectionDTO>().AsQueryable();
            }

            return result;
        }

        public InspectionEditDTO GetRegisterEntry(int id)
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
                InspectionEditDTO deserialized = DeserializeInspectionDraft(inspDbEntry.Inspection.InspectionDraft, Enum.Parse<InspectionTypesEnum>(inspDbEntry.InspectionType));

                deserialized.Id = inspDbEntry.Inspection.Id;
                deserialized.Files = Db.GetFiles(Db.InspectionRegisterFiles, id);

                return deserialized;
            }
            else
            {
                InspectionEditDTO inspectionBaseDTO = new InspectionEditDTO
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
                };

                inspectionBaseDTO.Files = Db.GetFiles(Db.InspectionRegisterFiles, id);
                inspectionBaseDTO.Inspectors = GetInspectors(id);
                inspectionBaseDTO.Personnel = GetPersonnel(id);
                inspectionBaseDTO.Checks = GetChecks(id);
                inspectionBaseDTO.PatrolVehicles = GetPatrolVehicles(id);
                inspectionBaseDTO.ObservationTexts = GetObservationTexts(id);

                return inspectionBaseDTO.InspectionType switch
                {
                    InspectionTypesEnum.OTH => GetRegisterOTH(inspectionBaseDTO),
                    InspectionTypesEnum.OFS => GetRegisterOFS(inspectionBaseDTO),
                    InspectionTypesEnum.IBS => GetRegisterIBS(inspectionBaseDTO),
                    InspectionTypesEnum.IBP => GetRegisterIBP(inspectionBaseDTO),
                    InspectionTypesEnum.ITB => GetRegisterITB(inspectionBaseDTO),
                    InspectionTypesEnum.IVH => GetRegisterIVH(inspectionBaseDTO),
                    InspectionTypesEnum.IFS => GetRegisterIFS(inspectionBaseDTO),
                    InspectionTypesEnum.IAQ => GetRegisterIAQ(inspectionBaseDTO),
                    InspectionTypesEnum.IFP => GetRegisterIFP(inspectionBaseDTO),
                    InspectionTypesEnum.CWO => GetRegisterCWO(inspectionBaseDTO),
                    InspectionTypesEnum.IGM => GetRegisterIGM(inspectionBaseDTO),
                    _ => null,
                };
            }
        }

        public int AddRegisterEntry(InspectionDraftDTO itemDTO, InspectionTypesEnum inspectionType, int userId)
        {
            using TransactionScope scope = new();

            List<FileInfoDTO> files = itemDTO.Files;

            itemDTO.Files = null;

            int inspectionTypeId = Db.NinspectionTypes.Single(f => f.Code == inspectionType.ToString()).Id;
            int draftStateId = Db.NinspectionStates.Single(f => f.Code == nameof(InspectionStatesEnum.Draft)).Id;

            InspectionRegister newInspDbEntry = new InspectionRegister
            {
                StateId = draftStateId,
                InspectionTypeId = inspectionTypeId,
                InspectionStart = itemDTO.StartDate ?? DateTime.Today,
                InspectionEnd = itemDTO.EndDate,
                IsByEmergencySignal = itemDTO.ByEmergencySignal,
                HasAdministrativeViolation = itemDTO.AdministrativeViolation,
                InspectorCommentText = itemDTO.InspectorComment,
                ActionsTakenText = itemDTO.ActionsTaken,
                CreatedByUserId = userId,
                IsActive = true
            };

            Db.InspectionsRegister.Add(newInspDbEntry);
            Db.SaveChanges();

            itemDTO.Id = newInspDbEntry.Id;
            newInspDbEntry.InspectionDraft = itemDTO.Json;

            if (files != null)
            {
                foreach (FileInfoDTO file in files)
                {
                    Db.AddOrEditFile(newInspDbEntry, newInspDbEntry.InspectionRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            scope.Complete();

            return newInspDbEntry.Id;
        }

        public void EditRegisterEntry(InspectionDraftDTO itemDTO, InspectionTypesEnum inspectionType)
        {
            using TransactionScope scope = new TransactionScope();

            InspectionRegister inspDbEntry = Db.InspectionsRegister
                .Include(x => x.InspectionRegisterFiles)
                .First(x => x.Id == itemDTO.Id.Value);

            int draftStateId = Db.NinspectionStates.Single(x => x.Code == nameof(InspectionStatesEnum.Draft)).Id;

            if (inspDbEntry.StateId != draftStateId)
            {
                throw new ArgumentException("Submitted");
            }

            List<FileInfoDTO> files = itemDTO.Files;

            itemDTO.Files = null;

            inspDbEntry.InspectionStart = itemDTO.StartDate ?? DateTime.Now.Date;
            inspDbEntry.InspectionEnd = itemDTO.EndDate;
            inspDbEntry.IsByEmergencySignal = itemDTO.ByEmergencySignal;
            inspDbEntry.HasAdministrativeViolation = itemDTO.AdministrativeViolation;
            inspDbEntry.InspectorCommentText = itemDTO.InspectorComment;
            inspDbEntry.ActionsTakenText = itemDTO.ActionsTaken;
            inspDbEntry.InspectionDraft = itemDTO.Json;

            if (files != null)
            {
                foreach (FileInfoDTO file in files)
                {
                    Db.AddOrEditFile(inspDbEntry, inspDbEntry.InspectionRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            scope.Complete();
        }

        public int SubmitReport(InspectionEditDTO itemDTO, InspectionTypesEnum inspectionType, int userId)
        {
            using TransactionScope scope = new TransactionScope();

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

            Db.SaveChanges();

            switch (inspectionType)
            {
                case InspectionTypesEnum.OFS: //OFS - Observation Fishing ship at Sea - Наблюдение на риболовен кораб в открито море
                    {
                        AddRegisterOFS(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.IBS: //IBS - Inspection on Board fishing Ship - Инспекция на борда на риболовен кораб ва вода
                    {
                        AddRegisterIBS(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.IBP: //IBP - Inspection on Board fishing ship in Port - Инспекция на риболовен кораб в пристанище
                    {
                        AddRegisterITB(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.ITB: //ITB - Inspection TransBoarding - Инспекция при трансбордиране
                    {
                        AddRegisterITB(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.IVH: //IVH - Inspection of VeHicle - Инспекция на средство за транспорт
                    {
                        AddRegisterIVH(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.IFS: //IFS - Inspection at first sale - Инспекция при първа продажба
                    {
                        AddRegisterIFS(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.IAQ: //IAQ - Inspection of AQuaculture property - Инспекция на аквакултурно стопанство
                    {
                        AddRegisterIAQ(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.IFP: //IFP - Inspection of Fishing Person - Инспекция на лица, извършващи любителски риболов
                    {
                        AddRegisterIFP(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.CWO: //CWO - Check of Water Object - Проверка на воден обект без извършване на инспекция
                    {
                        AddRegisterCWO(itemDTO, inspDbEntry);
                        break;
                    }
                case InspectionTypesEnum.IGM: //CMU - Check and Mark of fishing Utility - Проверка и маркировка на риболовен уред
                    {
                        AddRegisterIGM(itemDTO, inspDbEntry);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            Db.SaveChanges();

            scope.Complete();

            return inspDbEntry.Id;
        }

        public void Delete(int inspectionId)
        {
            InspectionRegister inspection = Db.InspectionsRegister.First(x => x.Id == inspectionId);
            inspection.IsActive = false;
            Db.SaveChanges();
        }

        public void SafeDelete(int inspectionId)
        {
            DateTime now = DateTime.Now;

            InspectionRegister inspection = Db.InspectionsRegister.First(x => x.Id == inspectionId);
            int draftStateId = (
                from state in Db.NinspectionStates
                where state.Code == nameof(InspectionStatesEnum.Draft)
                    && state.ValidFrom < now
                    && state.ValidTo > now
                select state.Id
            ).Single();

            if (inspection.StateId != draftStateId)
            {
                return;
            }

            inspection.IsActive = false;
            Db.SaveChanges();
        }

        public void Undelete(int inspectionId)
        {
            Db.InspectionsRegister.First(x => x.Id == inspectionId).IsActive = true;
            Db.SaveChanges();
        }

        public void SignInspection(int inspectionId, List<FileInfoDTO> files)
        {
            if (files == null || files.Count < 1)
            {
                return;
            }

            using TransactionScope scope = new TransactionScope();

            InspectionRegister inspDbEntry = Db.InspectionsRegister
                .Include(x => x.InspectionRegisterFiles)
                .First(x => x.Id == inspectionId);

            inspDbEntry.StateId = Db.NinspectionStates.Single(f => f.Code == nameof(InspectionStatesEnum.Signed)).Id;

            foreach (FileInfoDTO file in files)
            {
                Db.AddOrEditFile(inspDbEntry, inspDbEntry.InspectionRegisterFiles, file);
            }

            Db.SaveChanges();

            scope.Complete();
        }

        public Task<byte[]> DownloadInspection(int inspectionId)
        {
            var inspDbEntry = (
                from insp in Db.InspectionsRegister
                join inspType in Db.NinspectionTypes on insp.InspectionTypeId equals inspType.Id
                join inspState in Db.NinspectionStates on insp.StateId equals inspState.Id
                where insp.Id == inspectionId
                select new
                {
                    State = Enum.Parse<InspectionStatesEnum>(inspState.Code),
                    Type = Enum.Parse<InspectionTypesEnum>(inspType.Code),
                }
            ).First();

            if (inspDbEntry.State == InspectionStatesEnum.Draft)
            {
                return Task.FromResult<byte[]>(null);
            }

            return jasperReport.GetInspectionReport(inspectionId, inspDbEntry.Type);
        }

        public List<AuanRegisterDTO> GetInspectionAUANs(List<int> inspectionIds)
        {
            List<AuanRegisterDTO> result = (
                from auan in this.Db.AuanRegister
                join inspPerson in this.Db.Persons on auan.InspectedPersonId equals inspPerson.Id into inspPer
                from inspPerson in inspPer.DefaultIfEmpty()
                join inspLegal in this.Db.Legals on auan.InspectedLegalId equals inspLegal.Id into inspLeg
                from inspLegal in inspLeg.DefaultIfEmpty()
                join inspInsp in this.Db.InspectionInspectors on auan.InspectionId equals inspInsp.InspectionId into inspection
                from inspInsp in inspection.DefaultIfEmpty()
                join inspector in this.Db.Inspectors on inspInsp.InspectorId equals inspector.Id into insp
                from inspector in insp.DefaultIfEmpty()
                join unregPerson in this.Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPer
                from unregPerson in unregPer.DefaultIfEmpty()
                join user in this.Db.Users on inspector.UserId equals user.Id into us
                from user in us.DefaultIfEmpty()
                join person in this.Db.Persons on user.PersonId equals person.Id into per
                from person in per.DefaultIfEmpty()
                where auan.IsActive
                    && inspectionIds.Contains(auan.InspectionId)
                select new AuanRegisterDTO
                {
                    Id = auan.Id,
                    InspectionId = auan.InspectionId,
                    AuanNum = auan.AuanNum,
                    InspectedEntity = inspPerson != null
                        ? inspPerson.FirstName + " " + inspPerson.LastName
                        : inspLegal.Name,
                    Drafter = unregPerson != null
                        ? unregPerson.FirstName + " " + unregPerson.LastName
                        : person.FirstName + " " + person.LastName,
                    DraftDate = auan.DraftDate,
                    IsActive = auan.IsActive
                }
            ).ToList();

            return result;
        }

        public IQueryable<NomenclatureDTO> GetPatrolVehicles(bool isWaterVehicle)
        {
            PatrolVehicleTypeEnum type = isWaterVehicle ? PatrolVehicleTypeEnum.Marine : PatrolVehicleTypeEnum.Ground;

            IQueryable<NomenclatureDTO> result = (
                from vehicle in Db.UnregisteredVessels
                join patrolType in Db.NpatrolVehicleTypes on vehicle.PatrolVehicleTypeId equals patrolType.Id
                where vehicle.IsActive
                    && (
                        patrolType.VehicleType == type.ToString()
                        || patrolType.VehicleType == nameof(PatrolVehicleTypeEnum.Air)
                        || patrolType.VehicleType == nameof(PatrolVehicleTypeEnum.Other)
                    )
                select new NomenclatureDTO
                {
                    Value = vehicle.Id,
                    DisplayName = vehicle.Name + " (" + vehicle.Cfr + ")",
                    IsActive = vehicle.IsActive
                });

            return result;
        }

        public VesselDTO GetPatrolVehicle(int id)
        {
            VesselDTO result = (
                from vehicle in Db.UnregisteredVessels
                join patrolType in Db.NpatrolVehicleTypes on vehicle.PatrolVehicleTypeId equals patrolType.Id
                where vehicle.Id == id
                select new VesselDTO
                {
                    ShipId = null,
                    UnregisteredVesselId = vehicle.Id,
                    IsRegistered = false,
                    Name = vehicle.Name,
                    ExternalMark = vehicle.ExternalMark,
                    CFR = vehicle.Cfr,
                    UVI = vehicle.Uvi,
                    RegularCallsign = vehicle.IrcscallSign,
                    MMSI = vehicle.Mmsi,
                    FlagCountryId = vehicle.FlagCountryId,
                    PatrolVehicleTypeId = vehicle.PatrolVehicleTypeId,
                    VesselTypeId = vehicle.VesselTypeId,
                    InstitutionId = vehicle.InstitutionId,
                    IsActive = vehicle.IsActive
                }).FirstOrDefault();

            return result;
        }

        public IQueryable<NomenclatureDTO> GetInspectors()
        {
            DateTime now = DateTime.Now;

            IQueryable<NomenclatureDTO> result =
                from inspector in Db.Inspectors
                join user in Db.Users on inspector.UserId equals user.Id into userMatchTable
                from user in userMatchTable.DefaultIfEmpty()
                join person in Db.Persons on user.PersonId equals person.Id into personMatchTable
                from person in personMatchTable.DefaultIfEmpty()
                join unregisteredPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregisteredPerson.Id into unregisteredPersonMatchTable
                from unregisteredPerson in unregisteredPersonMatchTable.DefaultIfEmpty()
                where inspector.IsActive
                    && (inspector.UserId != null || inspector.UnregisteredPersonId != null)
                select new NomenclatureDTO
                {
                    Value = inspector.Id,
                    DisplayName = (
                        person != null
                            ? (person.FirstName + " " + (
                                person.MiddleName != null
                                    ? person.MiddleName + " "
                                    : string.Empty
                                ) + person.LastName
                            )
                            : unregisteredPerson.FirstName
                        ) + " (" + inspector.InspectorCardNum + ")",
                    IsActive = inspector.IsActive,
                };

            return result;
        }

        public InspectorDTO GetInspector(int id)
        {
            InspectorDTO result = (
                from inspector in Db.Inspectors
                join user in Db.Users on inspector.UserId equals user.Id into userMatchTable
                from user in userMatchTable.DefaultIfEmpty()
                join person in Db.Persons on user.PersonId equals person.Id into personMatchTable
                from person in personMatchTable.DefaultIfEmpty()
                join unregisteredPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregisteredPerson.Id into unregisteredPersonMatchTable
                from unregisteredPerson in unregisteredPersonMatchTable.DefaultIfEmpty()
                where inspector.Id == id
                select new InspectorDTO
                {
                    Id = inspector.Id,
                    CardNum = inspector.InspectorCardNum,
                    CitizenshipId = person != null ? person.CitizenshipCountryId : unregisteredPerson.CitizenshipCountryId,
                    FirstName = person != null ? person.FirstName : unregisteredPerson.FirstName,
                    MiddleName = person != null ? person.MiddleName : unregisteredPerson.MiddleName,
                    LastName = person != null ? person.LastName : unregisteredPerson.LastName,
                    InspectorId = inspector.Id,
                    InstitutionId = inspector.InstitutionId,
                    UnregisteredPersonId = unregisteredPerson.Id,
                    UserId = user.Id,
                    IsActive = inspector.IsActive,
                }).FirstOrDefault();

            return result;
        }

        public InspectorDTO GetInspectorByUserId(int userId)
        {
            InspectorDTO result = (
                from inspector in Db.Inspectors
                join user in Db.Users on inspector.UserId equals user.Id
                join person in Db.Persons on user.PersonId equals person.Id
                where user.Id == userId
                    && inspector.IsActive
                select new InspectorDTO
                {
                    Id = inspector.Id,
                    CardNum = inspector.InspectorCardNum,
                    CitizenshipId = person.CitizenshipCountryId,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    LastName = person.LastName,
                    InspectorId = inspector.Id,
                    InstitutionId = inspector.InstitutionId,
                    UserId = user.Id,
                    IsActive = inspector.IsActive,
                    IsNotRegistered = false,
                }).SingleOrDefault();

            return result;
        }

        public VesselDTO GetShip(int id)
        {
            DateTime now = DateTime.Now;

            VesselDTO result = (
                from ship in Db.ShipsRegister
                where ship.Id == id
                select new VesselDTO
                {
                    ShipId = ship.Id,
                    ExternalMark = ship.ExternalMark,
                    CFR = ship.Cfr,
                    FlagCountryId = ship.FlagCountryId,
                    MMSI = ship.Mmsi,
                    Name = ship.Name,
                    RegularCallsign = ship.IrcscallSign,
                    UVI = ship.Uvi,
                    VesselTypeId = ship.VesselTypeId,
                    IsActive = ship.ValidFrom < now && ship.ValidTo > now
                }
            ).FirstOrDefault();

            return result;
        }

        public List<InspectionShipSubjectNomenclatureDTO> GetShipPersonnel(int shipId)
        {
            // Приемам предложения за опростяване на логиката тука

            DateTime now = DateTime.Now;

            int shipUid = Db.ShipsRegister.First(f => f.Id == shipId).ShipUid;

            var owners = (
                from owner in Db.ShipOwners
                where owner.ShipRegisterId == shipId
                    && owner.IsActive
                select new
                {
                    Id = owner.Id,
                    PersonId = owner.OwnerPersonId,
                    LegalId = owner.OwnerLegalId,
                }
            ).ToList();

            var permits = (
                from ship in Db.ShipsRegister
                join plr in Db.CommercialFishingPermitLicensesRegisters on ship.Id equals plr.ShipId
                where plr.RecordType == nameof(RecordTypesEnum.Register)
                    && plr.PermitLicenseValidFrom.Value < now && plr.PermitLicenseValidTo.Value > now
                    && plr.IsActive
                    && ship.ShipUid == shipUid
                select new
                {
                    Id = plr.Id,
                    PersonId = plr.SubmittedForPersonId,
                    LegalId = plr.SubmittedForLegalId,
                    CaptainId = plr.QualifiedFisherId,
                }
            ).ToList();

            HashSet<int> personIds = owners.Where(f => f.PersonId.HasValue).Select(f => f.PersonId.Value)
                .Concat(permits.Where(f => f.PersonId.HasValue).Select(f => f.PersonId.Value))
                .Concat(permits.Select(f => f.CaptainId))
                .ToHashSet();

            HashSet<int> legalIds = owners.Where(f => f.LegalId.HasValue).Select(f => f.LegalId.Value)
                .Concat(permits.Where(f => f.LegalId.HasValue).Select(f => f.LegalId.Value))
                .ToHashSet();

            var persons = (
                from person in Db.Persons
                join address in (
                    from personAddress in Db.PersonAddresses
                    join address in Db.Addresses on personAddress.AddressId equals address.Id
                    join addressType in Db.NaddressTypes on personAddress.AddressTypeId equals addressType.Id
                    where addressType.Code == nameof(AddressTypesEnum.PERMANENT)
                        && address.IsActive
                        && personAddress.IsActive
                    select new InspectionSubjectAddressDTO
                    {
                        Id = personAddress.PersonId,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PostCode = address.PostCode,
                        Region = address.Region,
                    }) on person.Id equals address.Id into agrp
                from address in agrp.DefaultIfEmpty()
                where personIds.Contains(person.Id)
                select new
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    LastName = person.LastName,
                    CountryId = person.CitizenshipCountryId,
                    EgnLnc = new EgnLncDTO
                    {
                        EgnLnc = person.EgnLnc,
                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                    },
                    Address = address.Id.HasValue ? address : null,
                }).ToList();

            var legals = (
                from legal in Db.Legals
                join address in (
                    from legalAddress in Db.LegalsAddresses
                    join address in Db.Addresses on legalAddress.AddressId equals address.Id
                    join addressType in Db.NaddressTypes on legalAddress.AddressTypeId equals addressType.Id
                    where addressType.Code == nameof(AddressTypesEnum.COMPANY_HEADQUARTERS)
                        && address.IsActive
                        && legalAddress.IsActive
                    select new InspectionSubjectAddressDTO
                    {
                        Id = legalAddress.LegalId,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PostCode = address.PostCode,
                        Region = address.Region,
                    }) on legal.Id equals address.Id into agrp
                from address in agrp.DefaultIfEmpty()
                where legalIds.Contains(legal.Id)
                select new
                {
                    Id = legal.Id,
                    Name = legal.Name,
                    Eik = legal.Eik,
                    Address = address.Id.HasValue ? address : null,
                }).ToList();

            List<InspectionShipSubjectNomenclatureDTO> personnel = (
                from owner in owners
                join person in persons on owner.PersonId equals person.Id
                select new InspectionShipSubjectNomenclatureDTO
                {
                    Value = person.Id,
                    EntryId = owner.Id,
                    Code = person.EgnLnc.EgnLnc,
                    EgnLnc = person.EgnLnc,
                    IsLegal = false,
                    Address = person.Address,
                    DisplayName = person.FirstName
                        + (person.MiddleName == null ? " " : $" {person.MiddleName} ")
                        + person.LastName,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    LastName = person.LastName,
                    CountryId = person.CountryId ?? person.Address?.CountryId,
                    Type = InspectedPersonTypeEnum.OwnerPers,
                    IsActive = true,
                }
            ).ToList();

            personnel.AddRange((
                from owner in owners
                join legal in legals on owner.LegalId equals legal.Id
                select new InspectionShipSubjectNomenclatureDTO
                {
                    Value = legal.Id,
                    EntryId = owner.Id,
                    Code = legal.Eik,
                    Eik = legal.Eik,
                    IsLegal = true,
                    Address = legal.Address,
                    DisplayName = legal.Name,
                    FirstName = legal.Name,
                    CountryId = legal.Address?.CountryId,
                    Type = InspectedPersonTypeEnum.OwnerLegal,
                    IsActive = true,
                }
            ).ToList());

            personnel.AddRange((
                from permit in permits
                join person in persons on permit.PersonId equals person.Id
                select new InspectionShipSubjectNomenclatureDTO
                {
                    Value = person.Id,
                    EntryId = permit.Id,
                    Code = person.EgnLnc.EgnLnc,
                    EgnLnc = person.EgnLnc,
                    IsLegal = false,
                    Address = person.Address,
                    DisplayName = person.FirstName
                        + (person.MiddleName == null ? " " : $" {person.MiddleName} ")
                        + person.LastName,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    LastName = person.LastName,
                    CountryId = person.CountryId ?? person.Address?.CountryId,
                    Type = InspectedPersonTypeEnum.LicUsrPers,
                    IsActive = true,
                }
            ).ToList());

            personnel.AddRange((
                from permit in permits
                join legal in legals on permit.LegalId equals legal.Id
                select new InspectionShipSubjectNomenclatureDTO
                {
                    Value = legal.Id,
                    EntryId = permit.Id,
                    Code = legal.Eik,
                    Eik = legal.Eik,
                    IsLegal = false,
                    Address = legal.Address,
                    DisplayName = legal.Name,
                    FirstName = legal.Name,
                    CountryId = legal.Address?.CountryId,
                    Type = InspectedPersonTypeEnum.LicUsrLgl,
                    IsActive = true,
                }
            ).ToList());

            personnel.AddRange((
                from permit in permits
                join person in persons on permit.CaptainId equals person.Id
                select new InspectionShipSubjectNomenclatureDTO
                {
                    Value = person.Id,
                    EntryId = permit.Id,
                    Code = person.EgnLnc.EgnLnc,
                    EgnLnc = person.EgnLnc,
                    IsLegal = false,
                    Address = person.Address,
                    DisplayName = person.FirstName
                        + (person.MiddleName == null ? " " : $" {person.MiddleName} ")
                        + person.LastName,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    LastName = person.LastName,
                    CountryId = person.CountryId ?? person.Address?.CountryId,
                    Type = InspectedPersonTypeEnum.CaptFshmn,
                    IsActive = true,
                }
            ).ToList());

            return personnel
                .GroupBy(f => new { Id = f.Value, f.Type })
                .Select(f => f.First())
                .ToList();
        }

        public List<FishingGearDTO> GetPermittedFishingGears(int subjectId, InspectionSubjectEnum subjectType)
        {
            List<int> shipIds = null;
            if (subjectType == InspectionSubjectEnum.Ship)
            {
                int shipUid = Db.ShipsRegister.First(x => x.Id == subjectId).ShipUid;
                shipIds = Db.ShipsRegister.Where(x => x.ShipUid == shipUid).Select(x => x.Id).ToList();
            }
            List<FishingGearDTO> fishingGears = (
                from fishingGear in Db.FishingGearRegisters
                join type in Db.NfishingGears on fishingGear.FishingGearTypeId equals type.Id
                join permit in Db.CommercialFishingPermitLicensesRegisters on fishingGear.PermitLicenseId equals permit.Id
                where fishingGear.InspectionId == null
                    && ((subjectType == InspectionSubjectEnum.Poundnet && permit.PoundNetId == subjectId)
                        || (subjectType == InspectionSubjectEnum.Ship && shipIds.Contains(permit.ShipId)))
                    && fishingGear.IsActive
                    && permit.IsActive
                select new FishingGearDTO
                {
                    Id = fishingGear.Id,
                    TypeId = fishingGear.FishingGearTypeId,
                    Count = fishingGear.GearCount,
                    Length = fishingGear.Length,
                    Height = fishingGear.Height,
                    NetEyeSize = fishingGear.NetEyeSize,
                    CordThickness = fishingGear.CordThickness,
                    HookCount = fishingGear.HookCount,
                    Description = fishingGear.Description,
                    TowelLength = fishingGear.TowelLength,
                    HouseLength = fishingGear.HouseLength,
                    HouseWidth = fishingGear.HouseWidth,
                    HasPingers = fishingGear.HasPinger,
                    Type = $"{type.Code} - {type.Name}",
                    PermitId = fishingGear.PermitLicenseId,
                    IsActive = fishingGear.IsActive,
                }).ToList();

            fishingGearService.MapFishingGearMarksAndPingers(fishingGears);

            return fishingGears;
        }

        public List<InspectionCheckTypeNomenclatureDTO> GetCheckTypesForInspectionType(InspectionTypesEnum inspectionTypeParam)
        {
            string inspectionTypeString = inspectionTypeParam.ToString();
            DateTime now = DateTime.Now;

            List<InspectionCheckTypeNomenclatureDTO> result = (
                from checkType in Db.NinspectionCheckTypes
                join inspectionType in Db.NinspectionTypes on checkType.InspectionTypeId equals inspectionType.Id
                where inspectionType.Code == inspectionTypeString
                    && checkType.ValidFrom < now && checkType.ValidTo > now
                select new InspectionCheckTypeNomenclatureDTO
                {
                    Value = checkType.Id,
                    DisplayName = checkType.Name,
                    Code = checkType.Code,
                    IsMandatory = checkType.IsMandatory,
                    HasAdditionalDescr = checkType.HasDescription,
                    CheckType = Enum.Parse<InspectionCheckTypesEnum>(checkType.CheckType.ToUpper()),
                    InspectionTypeId = inspectionType.Id,
                    DescriptionLabel = checkType.DescriptionLabel,
                    IsActive = checkType.ValidFrom <= now && checkType.ValidTo >= now
                }).ToList();

            return result;
        }

        public List<InspectionPermitLicenseDTO> GetShipPermitLicenses(int shipId)
        {
            DateTime now = DateTime.Now;

            int shipUid = Db.ShipsRegister.First(f => f.Id == shipId).ShipUid;

            List<int> shipIds = Db.ShipsRegister.Where(x => x.ShipUid == shipUid)
                .Select(x => x.Id)
                .ToList();

            List<InspectionPermitLicenseDTO> result = (
                from plr in Db.CommercialFishingPermitLicensesRegisters
                join plrType in Db.NcommercialFishingPermitLicenseTypes on plr.PermitLicenseTypeId equals plrType.Id
                join pr in Db.CommercialFishingPermitRegisters on plr.PermitId equals pr.Id
                where plr.RecordType == nameof(RecordTypesEnum.Register)
                    && plr.PermitLicenseValidFrom.Value < now && plr.PermitLicenseValidTo.Value > now
                    && plr.IsActive
                    && pr.IsActive
                    && shipIds.Contains(plr.ShipId)
                select new InspectionPermitLicenseDTO
                {
                    Id = plr.Id,
                    PermitNumber = pr.RegistrationNum,
                    LicenseNumber = plr.RegistrationNum,
                    TypeName = plrType.ShortName,
                    TypeId = plrType.Id,
                    ValidFrom = plr.PermitLicenseValidFrom.Value,
                    ValidTo = plr.PermitLicenseValidTo.Value,
                }
            ).ToList();

            return result;
        }

        public List<InspectionPermitLicenseDTO> GetPoundNetPermitLicenses(int poundNetId)
        {
            DateTime now = DateTime.Now;

            List<InspectionPermitLicenseDTO> result = (
                from plr in Db.CommercialFishingPermitLicensesRegisters
                join plrType in Db.NcommercialFishingPermitLicenseTypes on plr.PermitLicenseTypeId equals plrType.Id
                join pr in Db.CommercialFishingPermitRegisters on plr.PermitId equals pr.Id
                where plr.RecordType == nameof(RecordTypesEnum.Register)
                    && plr.PermitLicenseValidFrom.Value < now && plr.PermitLicenseValidTo.Value > now
                    && plr.IsActive
                    && pr.IsActive
                    && plr.PoundNetId == poundNetId
                select new InspectionPermitLicenseDTO
                {
                    Id = plr.Id,
                    PermitNumber = pr.RegistrationNum,
                    LicenseNumber = plr.RegistrationNum,
                    TypeName = plrType.ShortName,
                    TypeId = plrType.Id,
                    ValidFrom = plr.PermitLicenseValidFrom.Value,
                    ValidTo = plr.PermitLicenseValidTo.Value,
                }
            ).ToList();

            return result;
        }

        public List<InspectionShipLogBookDTO> GetShipLogBooks(int shipId)
        {
            DateTime now = DateTime.Now;

            int shipUid = Db.ShipsRegister.First(f => f.Id == shipId).ShipUid;

            List<int> shipIds = Db.ShipsRegister.Where(x => x.ShipUid == shipUid)
                .Select(x => x.Id)
                .ToList();

            List<InspectionShipLogBookDTO> result = (
                from logBook in Db.LogBooks
                join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                where logBook.IsActive
                    && logBook.FinishDate == null
                    && logBookStatus.Code != nameof(LogBookStatusesEnum.Finished)
                    && logBook.ShipId.HasValue
                    && shipIds.Contains(logBook.ShipId.Value)
                select new InspectionShipLogBookDTO
                {
                    Id = logBook.Id,
                    Number = logBook.LogNum,
                    IssuedOn = logBook.IssueDate,
                    StartPage = logBook.StartPageNum,
                    EndPage = logBook.EndPageNum,
                }
            ).ToList();

            List<int> logBookIds = result.ConvertAll(f => f.Id);

            List<LogBookPageMobileDTO> pages = (
                from rootShip in Db.ShipsRegister
                join ship in Db.ShipsRegister on rootShip.ShipUid equals ship.ShipUid
                join plr in Db.CommercialFishingPermitLicensesRegisters on ship.Id equals plr.ShipId
                join lbpl in Db.LogBookPermitLicenses on plr.Id equals lbpl.PermitLicenseRegisterId
                join logBook in Db.LogBooks on lbpl.LogBookId equals logBook.Id
                join logBookPage in Db.ShipLogBookPages on logBook.Id equals logBookPage.LogBookId
                where logBookIds.Contains(logBook.Id)
                    && logBookPage.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    && rootShip.ValidFrom < now && rootShip.ValidTo > now
                select new LogBookPageMobileDTO
                {
                    Id = logBookPage.Id,
                    LogBookId = logBook.Id,
                    PageNum = logBookPage.PageNum,
                }
            ).ToList();

            foreach (InspectionShipLogBookDTO logBook in result)
            {
                logBook.Pages = pages
                    .Where(f => f.LogBookId == logBook.Id)
                    .Select(f => new NomenclatureDTO
                    {
                        Value = f.Id,
                        DisplayName = f.PageNum
                    })
                    .ToList();
            }

            return result;
        }

        public List<InspectedBuyerNomenclatureDTO> GetBuyers()
        {
            List<InspectedBuyerNomenclatureDTO> result = (
                from buyer in Db.BuyerRegisters
                join person in Db.Persons on buyer.SubmittedForPersonId equals person.Id into pgrp
                from person in pgrp.DefaultIfEmpty()
                join legal in Db.Legals on buyer.SubmittedForLegalId equals legal.Id into lgrp
                from legal in lgrp.DefaultIfEmpty()
                join legalAddress in (
                    from legalAddress in Db.LegalsAddresses
                    join address in Db.Addresses on legalAddress.AddressId equals address.Id
                    join addressType in Db.NaddressTypes on legalAddress.AddressTypeId equals addressType.Id
                    where addressType.Code == nameof(AddressTypesEnum.COMPANY_HEADQUARTERS)
                        && address.IsActive
                        && legalAddress.IsActive
                    select new InspectionSubjectAddressDTO
                    {
                        Id = legalAddress.LegalId,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PostCode = address.PostCode,
                        Region = address.Region,
                    }) on buyer.SubmittedForLegalId equals legalAddress.Id into agrp
                from legalAddress in agrp.DefaultIfEmpty()
                join personAddress in (
                    from personAddress in Db.PersonAddresses
                    join address in Db.Addresses on personAddress.AddressId equals address.Id
                    join addressType in Db.NaddressTypes on personAddress.AddressTypeId equals addressType.Id
                    where addressType.Code == nameof(AddressTypesEnum.COMPANY_HEADQUARTERS)
                        && address.IsActive
                        && personAddress.IsActive
                    select new InspectionSubjectAddressDTO
                    {
                        Id = personAddress.PersonId,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PostCode = address.PostCode,
                        Region = address.Region,
                    }) on buyer.SubmittedForPersonId equals personAddress.Id into pagrp
                from personAddress in pagrp.DefaultIfEmpty()
                where buyer.IsActive
                    && buyer.RecordType == nameof(RecordTypesEnum.Register)
                    && !(buyer.SubmittedForPersonId == null && buyer.SubmittedForLegalId == null)
                select new InspectedBuyerNomenclatureDTO
                {
                    Value = person != null ? person.Id : legal.Id,
                    DisplayName = person != null
                        ? (person.FirstName + " " + (person.MiddleName != null ? person.MiddleName + " " : string.Empty) + person.LastName + " (" + person.EgnLnc + ")")
                        : legal.Name + " (" + legal.Eik + ")",
                    EntryId = buyer.Id,
                    HasUtility = buyer.HasUtility == true,
                    HasVehicle = buyer.HasVehicle == true,
                    UtilityName = buyer.UtilityName,
                    UtilityAddress = legalAddress.Id.HasValue ? legalAddress : null,
                    VehicleNumber = buyer.VehicleNumber,
                    Address = person != null ? personAddress : legalAddress,
                    EgnLnc = person != null ? new EgnLncDTO
                    {
                        EgnLnc = person.EgnLnc,
                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType),
                    } : null,
                    Eik = person == null ? legal.Eik : null,
                    IsLegal = person == null,
                    FirstName = person != null ? person.FirstName : legal.Name,
                    MiddleName = person != null ? person.MiddleName : null,
                    LastName = person != null ? person.LastName : null,
                    CountryId = person != null
                        ? person.CitizenshipCountryId
                        : legalAddress != null
                        ? legalAddress.CountryId : null,
                    Type = InspectedPersonTypeEnum.RegBuyer,
                    IsActive = buyer.IsActive
                }
            ).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetAquacultures()
        {
            List<NomenclatureDTO> result = (
                from aqua in Db.AquacultureFacilitiesRegister
                select new NomenclatureDTO
                {
                    Value = aqua.Id,
                    Code = aqua.UrorNum,
                    DisplayName = aqua.UrorNum + " - " + aqua.Name,
                }).ToList();

            return result;
        }

        public List<DeclarationLogBookPageDTO> GetDeclarationLogBookPages(DeclarationLogBookTypeEnum type, int shipId)
        {
            int shipUid = this.Db.ShipsRegister
                .Where(f => f.Id == shipId)
                .Select(f => f.ShipUid)
                .First();

            HashSet<int> shipLogBookIds = (
                from ship in this.Db.ShipsRegister
                join logBook in this.Db.LogBooks on ship.Id equals logBook.ShipId
                where ship.ShipUid == shipUid
                select logBook.Id
            ).ToHashSet();

            List<DeclarationLogBookPageDTO> result = type switch
            {
                DeclarationLogBookTypeEnum.FirstSaleLogBook => (
                    from fslbp in this.Db.FirstSaleLogBookPages
                    where shipLogBookIds.Contains(fslbp.LogBookId)
                        && fslbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    select new DeclarationLogBookPageDTO
                    {
                        Id = fslbp.Id,
                        Num = fslbp.PageNum.ToString(),
                        Date = fslbp.SaleDate.Value,
                    }
                ).ToList(),
                DeclarationLogBookTypeEnum.TransportationLogBook => (
                    from tlbp in this.Db.TransportationLogBookPages
                    where shipLogBookIds.Contains(tlbp.LogBookId)
                        && tlbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    select new DeclarationLogBookPageDTO
                    {
                        Id = tlbp.Id,
                        Num = tlbp.PageNum.ToString(),
                        Date = tlbp.LoadingDate.Value,
                    }
                ).ToList(),
                DeclarationLogBookTypeEnum.AdmissionLogBook => (
                    from albp in this.Db.AdmissionLogBookPages
                    where shipLogBookIds.Contains(albp.LogBookId)
                        && albp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    select new DeclarationLogBookPageDTO
                    {
                        Id = albp.Id,
                        Num = albp.PageNum.ToString(),
                        Date = albp.HandoverDate.Value,
                    }
                ).ToList(),
                DeclarationLogBookTypeEnum.ShipLogBook => (
                    from slbp in this.Db.ShipLogBookPages
                    where shipLogBookIds.Contains(slbp.LogBookId)
                        && slbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    select new DeclarationLogBookPageDTO
                    {
                        Id = slbp.Id,
                        Num = slbp.PageNum,
                        Date = slbp.PageFillDate.Value,
                    }
                ).ToList(),
                _ => throw new Exception($"{nameof(DeclarationLogBookTypeEnum)} provided to {nameof(GetDeclarationLogBookPages)} inside {nameof(MobileInspectionsService)} was of wrong type."),
            };

            return result;
        }

        public InspectionSubjectPersonnelDTO GetAquacultureOwner(int aquacultureId)
        {
            InspectionSubjectPersonnelDTO result = (
                from aqua in Db.AquacultureFacilitiesRegister
                join owner in Db.Legals on aqua.SubmittedForLegalId equals owner.Id
                join address in (
                    from legalAddress in Db.LegalsAddresses
                    join address in Db.Addresses on legalAddress.AddressId equals address.Id
                    join addressType in Db.NaddressTypes on legalAddress.AddressTypeId equals addressType.Id
                    where addressType.Code == nameof(AddressTypesEnum.COMPANY_HEADQUARTERS)
                        && address.IsActive
                        && legalAddress.IsActive
                    select new InspectionSubjectAddressDTO
                    {
                        Id = legalAddress.LegalId,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PostCode = address.PostCode,
                        Region = address.Region,
                    }) on owner.Id equals address.Id into agrp
                from address in agrp.DefaultIfEmpty()
                where aqua.Id == aquacultureId
                select new InspectionSubjectPersonnelDTO
                {
                    Id = owner.Id,
                    EntryId = aqua.Id,
                    Eik = owner.Eik,
                    RegisteredAddress = address,
                    IsLegal = true,
                    FirstName = owner.Name,
                    CitizenshipId = address == null ? null : address.CountryId,
                    Type = InspectedPersonTypeEnum.OwnerLegal,
                    IsActive = true,
                }
            ).First();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return base.GetSimpleEntityAuditValues(Db.InspectionsRegister, id);
        }

        #endregion

        #region Add

        private void AddRegisterOFS(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionObservationAtSeaDTO ofsDTO = itemDTO as InspectionObservationAtSeaDTO;

            ObservationAtSea newObsDbEntry = new ObservationAtSea
            {
                Inspection = newInspDbEntry,
                ObservedShipCoordinates = ofsDTO.ObservedVessel?.Location != null
                    ? new Point(ofsDTO.ObservedVessel.Location.Longitude, ofsDTO.ObservedVessel.Location.Latitude)
                    : null,
                Direction = ofsDTO.Course,
                Speed = ofsDTO.Speed,
                HasShipContact = ofsDTO.HasShipContact,
                HasShipCommunication = ofsDTO.HasShipCommunication,
                ShipCommunicationDescr = ofsDTO.ShipCommunicationDescription,
                ObservedShipCatchZoneId = ofsDTO.ObservedVessel?.CatchZoneId,
                ObservedShipLocation = ofsDTO.ObservedVessel?.LocationDescription,
                IsActive = true
            };

            if (ofsDTO.ObservedVessel != null)
            {
                if (ofsDTO.ObservedVessel.ShipId.HasValue && ofsDTO.ObservedVessel.IsRegistered == true)
                {
                    newObsDbEntry.ObservedShip = Db.ShipsRegister.First(x => x.Id == ofsDTO.ObservedVessel.ShipId.Value);
                }
                else
                {
                    newObsDbEntry.ObservedUnregisteredShip = AddUnregisteredShip(ofsDTO.ObservedVessel);
                }
            }

            AddObservationTools(newInspDbEntry, ofsDTO.ObservationTools);
            AddInspectionVesselActivities(newInspDbEntry, ofsDTO.ObservedVesselActivities);

            Db.ObservationsAtSea.Add(newObsDbEntry);
        }

        private void AddRegisterIBS(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionAtSeaDTO ibsDTO = itemDTO as InspectionAtSeaDTO;

            ShipInspection newIbsDbEntry = new ShipInspection
            {
                Inspection = newInspDbEntry,
                CaptainComment = ibsDTO.CaptainComment,
                InspectedShipCoordinates = ibsDTO.InspectedShip?.Location != null
                    ? new Point(ibsDTO.InspectedShip.Location.Longitude, ibsDTO.InspectedShip.Location.Latitude)
                    : null,
                InspectedShipType = nameof(SubjectRoleEnum.Inspected),
                InspectedShipCatchZoneId = ibsDTO.InspectedShip?.CatchZoneId,
                InspectedShipLocation = ibsDTO.InspectedShip?.LocationDescription,
                IsActive = true,
            };

            if (ibsDTO.InspectedShip != null)
            {
                if (ibsDTO.InspectedShip.ShipId.HasValue && ibsDTO.InspectedShip.IsRegistered == true)
                {
                    newIbsDbEntry.InspectiedShip = Db.ShipsRegister.First(x => x.Id == ibsDTO.InspectedShip.ShipId.Value);
                }
                else
                {
                    newIbsDbEntry.InspectedUnregisteredShip = AddUnregisteredShip(ibsDTO.InspectedShip);
                }
            }

            AddCatchMeasures(newInspDbEntry, ibsDTO.CatchMeasures, SubjectRoleEnum.Inspected);
            AddFishingGears(newInspDbEntry, ibsDTO.FishingGears);
            AddPortVisit(newInspDbEntry, ibsDTO.LastPortVisit, SubjectRoleEnum.Inspected);

            AddPermits(newInspDbEntry, ibsDTO.PermitLicenses, SubjectRoleEnum.Inspected);
            AddLogBooks(newInspDbEntry, ibsDTO.LogBooks, SubjectRoleEnum.Inspected);

            Db.ShipInspections.Add(newIbsDbEntry);
        }

        private void AddRegisterITB(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionTransboardingDTO itbDTO = itemDTO as InspectionTransboardingDTO;

            AddCatchMeasures(newInspDbEntry, itbDTO.TransboardedCatchMeasures, SubjectRoleEnum.Inspected);
            AddFishingGears(newInspDbEntry, itbDTO.FishingGears);

            if (itbDTO.ReceivingShipInspection != null)
            {
                AddRegisterITBShip(itbDTO.ReceivingShipInspection, newInspDbEntry, SubjectRoleEnum.TransboardReceiver);
            }

            if (itbDTO.SendingShipInspection != null)
            {
                AddRegisterITBShip(itbDTO.SendingShipInspection, newInspDbEntry, SubjectRoleEnum.TransboardSender);
            }
        }

        private void AddRegisterITBShip(InspectionTransboardingShipDTO dto, InspectionRegister newInspDbEntry, SubjectRoleEnum shipType)
        {
            ShipInspection sendingIbsDbEntry = new ShipInspection
            {
                Inspection = newInspDbEntry,
                CaptainComment = dto.CaptainComment,
                InspectedShipCoordinates = dto.InspectedShip?.Location != null
                    ? new Point(dto.InspectedShip.Location.Longitude, dto.InspectedShip.Location.Latitude)
                    : null,
                InspectedShipType = shipType.ToString(),
                InspectedShipCatchZoneId = dto.InspectedShip?.CatchZoneId,
                InspectedShipLocation = dto.InspectedShip?.LocationDescription,
                IsActive = true,
            };

            if (dto.InspectedShip != null)
            {
                if (dto.InspectedShip.ShipId.HasValue && dto.InspectedShip.IsRegistered == true)
                {
                    sendingIbsDbEntry.InspectiedShip = Db.ShipsRegister.First(x => x.Id == dto.InspectedShip.ShipId.Value);
                }
                else
                {
                    sendingIbsDbEntry.InspectedUnregisteredShip = AddUnregisteredShip(dto.InspectedShip);
                }
            }

            AddPortVisit(newInspDbEntry, dto.LastPortVisit, shipType);
            AddCatchMeasures(newInspDbEntry, dto.CatchMeasures, shipType);
            AddInspectionChecks(newInspDbEntry, dto.Checks, shipType);

            AddPermits(newInspDbEntry, dto.PermitLicenses, shipType);
            AddLogBooks(newInspDbEntry, dto.LogBooks, shipType);

            if (dto.Personnel != null)
            {
                AddInspectedPersons(newInspDbEntry, dto.Personnel, shipType);
            }

            Db.ShipInspections.Add(sendingIbsDbEntry);
        }

        private void AddRegisterIVH(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionTransportVehicleDTO ivhDTO = itemDTO as InspectionTransportVehicleDTO;

            TransportVehicleInspection newIvhDbEntry = new TransportVehicleInspection
            {
                Inspection = newInspDbEntry,
                CountryId = ivhDTO.CountryId,
                IsSealedVehicle = ivhDTO.IsSealed,
                SealCondition = ivhDTO.SealCondition,
                SealInstitutionId = ivhDTO.SealInstitutionId,
                TractorLicensePlateNum = ivhDTO.TractorLicensePlateNum,
                TrailerLicensePlateNum = ivhDTO.TrailerLicensePlateNum,
                TransporterComment = ivhDTO.TransporterComment,
                VehicleTypeId = ivhDTO.VehicleTypeId,
                InspectionLocation = ivhDTO.InspectionAddress,
                TractorBrand = ivhDTO.TractorBrand,
                TractorModel = ivhDTO.TractorModel,
                TransportDestination = ivhDTO.TransportDestination,
                InpectionLocationCoordinates = ivhDTO.InspectionLocation != null
                    ? new Point(ivhDTO.InspectionLocation.Longitude, ivhDTO.InspectionLocation.Latitude)
                    : null,
                IsActive = true
            };

            AddDeclarationCatchMeasures(newInspDbEntry, ivhDTO.CatchMeasures);

            Db.TransportVehicleInspections.Add(newIvhDbEntry);
        }

        private void AddRegisterIFS(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionFirstSaleDTO ifsDTO = itemDTO as InspectionFirstSaleDTO;

            FirstSaleInspection newIfsDbEntry = new FirstSaleInspection
            {
                Inspection = newInspDbEntry,
                Address = ifsDTO.SubjectAddress,
                Name = ifsDTO.SubjectName,
                RepresentativeComment = ifsDTO.RepresentativeComment,
                IsActive = true
            };

            AddDeclarationCatchMeasures(newInspDbEntry, ifsDTO.CatchMeasures);

            Db.FirstSaleInspections.Add(newIfsDbEntry);
        }

        private void AddRegisterIAQ(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionAquacultureDTO iaqDTO = itemDTO as InspectionAquacultureDTO;

            AquacultureInspection newIaqDbEntry = new AquacultureInspection
            {
                Inspection = newInspDbEntry,
                AquacultureRegisterId = iaqDTO.AquacultureId,
                OtherFishingToolsDescription = iaqDTO.OtherFishingGear,
                RepresentativeComment = iaqDTO.RepresentativeComment,
                Coordinates = iaqDTO.Location != null
                    ? new Point(iaqDTO.Location.Longitude, iaqDTO.Location.Latitude)
                    : null,
                IsActive = true
            };

            AddCatchMeasures(newInspDbEntry, iaqDTO.CatchMeasures);

            Db.AquacultureInspections.Add(newIaqDbEntry);
        }

        private void AddRegisterIFP(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionFisherDTO ifpDTO = itemDTO as InspectionFisherDTO;

            FishermanInspection newIfpDbEntry = new FishermanInspection
            {
                Inspection = newInspDbEntry,
                TicketNum = ifpDTO.TicketNum,
                FishingRodCount = ifpDTO.FishingRodsCount,
                FishingHooksCount = ifpDTO.FishingHooksCount,
                FishermanComment = ifpDTO.FishermanComment,
                InspectionLocation = ifpDTO.InspectionAddress,
                InpectionLocationCoordinates = ifpDTO.InspectionLocation != null
                    ? new Point(ifpDTO.InspectionLocation.Longitude, ifpDTO.InspectionLocation.Latitude)
                    : null,
                IsActive = true
            };

            InspectionSubjectPersonnelDTO fisherDTO = ifpDTO.Personnel
                .SingleOrDefault(f => f.Type == InspectedPersonTypeEnum.CaptFshmn);

            if (fisherDTO != null)
            {
                newIfpDbEntry.UnregisteredPerson = AddUnregisteredPerson(fisherDTO);
            }

            AddCatchMeasures(newInspDbEntry, ifpDTO.CatchMeasures);

            Db.FishermanInspections.Add(newIfpDbEntry);
        }

        private void AddRegisterCWO(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionCheckWaterObjectDTO cwoDTO = itemDTO as InspectionCheckWaterObjectDTO;

            WaterObjectCheck newCwoDbEntry = new WaterObjectCheck
            {
                Inspection = newInspDbEntry,
                WaterObjectTypeId = cwoDTO.WaterObjectTypeId,
                WaterObjectCoordinates = cwoDTO.WaterObjectLocation != null
                    ? new Point(cwoDTO.WaterObjectLocation.Longitude, cwoDTO.WaterObjectLocation.Latitude)
                    : null,
                Name = cwoDTO.ObjectName,
                IsActive = true
            };

            if (cwoDTO.FishingGears != null)
            {
                foreach (WaterInspectionFishingGearDTO fishingGear in cwoDTO.FishingGears)
                {
                    FishingGearRegister gear = new FishingGearRegister
                    {
                        PermitLicenseId = null,
                        FishingGearTypeId = fishingGear.TypeId,
                        GearCount = fishingGear.Count,
                        NetEyeSize = fishingGear.NetEyeSize,
                        HookCount = fishingGear.HookCount,
                        Length = fishingGear.Length,
                        Height = fishingGear.Height,
                        IsActive = true,
                        Description = fishingGear.Description,
                        HouseLength = fishingGear.HouseLength,
                        HouseWidth = fishingGear.HouseWidth,
                        Inspection = newInspDbEntry,
                        TowelLength = fishingGear.TowelLength,
                    };
                    Db.FishingGearRegisters.Add(gear);

                    if (fishingGear.Marks != null)
                    {
                        foreach (FishingGearMarkDTO mark in fishingGear.Marks)
                        {
                            FishingGearMark entry = new FishingGearMark
                            {
                                FishingGear = gear,
                                MarkNum = mark.Number,
                                MarkStatusId = mark.StatusId,
                                Inspection = newInspDbEntry,
                            };
                            Db.FishingGearMarks.Add(entry);
                        }
                    }

                    InspectedFishingGear inspectedGearDb = new InspectedFishingGear
                    {
                        Inspection = newInspDbEntry,
                        InspectedFishingGearNavigation = gear,
                        IsStored = fishingGear.IsStored,
                        IsTaken = fishingGear.IsTaken,
                        StorageLocation = fishingGear.StorageLocation,
                        IsActive = true,
                    };
                    Db.InspectedFishingGears.Add(inspectedGearDb);
                }
            }

            if (cwoDTO.Vessels != null)
            {
                foreach (WaterInspectionVesselDTO vessel in cwoDTO.Vessels)
                {
                    Db.InspectionVessels.Add(new InspectionVessel
                    {
                        Color = vessel.Color,
                        IsStored = vessel.IsStored,
                        IsTaken = vessel.IsTaken,
                        StorageLocation = vessel.StorageLocation,
                        Length = vessel.Length,
                        Number = vessel.Number,
                        TotalCount = vessel.TotalCount,
                        Width = vessel.Width,
                        VesselTypeId = vessel.VesselTypeId,
                        Inspection = newInspDbEntry,
                        IsActive = true,
                    });
                }
            }

            if (cwoDTO.Engines != null)
            {
                foreach (WaterInspectionEngineDTO engine in cwoDTO.Engines)
                {
                    Db.InspectionEngines.Add(new InspectionEngine
                    {
                        IsStored = engine.IsStored,
                        IsTaken = engine.IsTaken,
                        StorageLocation = engine.StorageLocation,
                        TotalCount = engine.TotalCount,
                        EngineModel = engine.Model,
                        EnginePower = engine.Power,
                        EngineType = engine.Type,
                        Inspection = newInspDbEntry,
                        EngineDescription = engine.EngineDescription,
                        IsActive = true,
                    });
                }
            }

            AddCatchMeasures(newInspDbEntry, cwoDTO.Catches);

            Db.WaterObjectChecks.Add(newCwoDbEntry);
        }

        private void AddRegisterIGM(InspectionEditDTO itemDTO, InspectionRegister newInspDbEntry)
        {
            InspectionCheckToolMarkDTO igmDTO = itemDTO as InspectionCheckToolMarkDTO;

            FishingGearCheck newCmuDbEntry = new FishingGearCheck
            {
                Inspection = newInspDbEntry,
                PoundNetId = igmDTO.PoundNetId,
                CheckReasonId = igmDTO.CheckReasonId,
                RecheckReasonId = igmDTO.RecheckReasonId,
                RecheckDescription = igmDTO.OtherRecheckReason,
                IsActive = true,
            };

            if (igmDTO.InspectedShip != null)
            {
                if (igmDTO.InspectedShip.ShipId.HasValue && igmDTO.InspectedShip.IsRegistered == true)
                {
                    newCmuDbEntry.Ship = Db.ShipsRegister.First(x => x.Id == igmDTO.InspectedShip.ShipId.Value);
                }
                else
                {
                    newCmuDbEntry.UnregisteredShip = AddUnregisteredShip(igmDTO.InspectedShip);
                }
            }

            if (igmDTO.PermitId != null)
            {
                AddPermits(newInspDbEntry, new List<InspectionPermitDTO>
                {
                    new InspectionPermitDTO
                    {
                        PermitLicenseId = igmDTO.PermitId.Value,
                        CheckValue = InspectionToggleTypesEnum.Y,
                    }
                }, SubjectRoleEnum.Inspected);
            }

            AddFishingGears(newInspDbEntry, igmDTO.FishingGears);
            AddPortVisit(newInspDbEntry, igmDTO.Port, SubjectRoleEnum.Inspected);

            Db.FishingGearChecks.Add(newCmuDbEntry);
        }

        private void AddPermits(InspectionRegister inspection, List<InspectionPermitDTO> permits, SubjectRoleEnum role)
        {
            if (permits != null)
            {
                foreach (InspectionPermitDTO permit in permits)
                {
                    InspectionPermitLicense permLicen = new InspectionPermitLicense
                    {
                        UnregisteredLicenseNum = permit.LicenseNumber,
                        CheckPermitLicenseMatches = permit.CheckValue.Value.ToString(),
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

        private void AddLogBooks(InspectionRegister inspection, List<InspectionLogBookDTO> logBooks, SubjectRoleEnum role)
        {
            if (logBooks != null)
            {
                foreach (InspectionLogBookDTO logBook in logBooks)
                {
                    InspectionLogBookPage logBookPage = new InspectionLogBookPage
                    {
                        CheckLogBookMatches = logBook.CheckValue.Value.ToString(),
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

        private void AddPortVisit(InspectionRegister inspection, PortVisitDTO portVisit, SubjectRoleEnum role)
        {
            if (portVisit != null)
            {
                InspectionLastPortVisit portVisitDb = new InspectionLastPortVisit
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

        private void AddInspectionVesselActivities(InspectionRegister inspection, List<InspectionVesselActivityNomenclatureDTO> vesselActivities)
        {
            if (vesselActivities != null)
            {
                foreach (InspectionVesselActivityNomenclatureDTO activity in vesselActivities)
                {
                    InspectionVesselActivity activityDb = new InspectionVesselActivity
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

        private void AddObservationTools(InspectionRegister inspection, List<InspectionObservationToolDTO> observationTools)
        {
            if (observationTools != null)
            {
                foreach (InspectionObservationToolDTO tool in observationTools)
                {
                    InspectionObservationTool obsToolDb = new InspectionObservationTool
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

        private void AddCatchMeasures(InspectionRegister inspection, List<InspectionCatchMeasureDTO> catchMeasures, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
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

                    InspectionCatchMeasure measureDb = new InspectionCatchMeasure
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
                        IsActive = true
                    };
                    Db.InspectionCatchMeasures.Add(measureDb);
                }
            }
        }

        private void AddDeclarationCatchMeasures(InspectionRegister inspection, List<InspectedDeclarationCatchDTO> catchMeasures)
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

                    InspectionLogBookPage page = new InspectionLogBookPage
                    {
                        InspectionId = inspection.Id,
                        ShipId = fish.OriginShip?.ShipId,
                        UnregisteredShip = unregisteredShip,
                        LogBookType = fish.LogBookType.ToString(),
                        UnregisteredPageDate = fish.UnregisteredPageDate,
                        UnregisteredPageNum = fish.UnregisteredPageNum,
                        InspectedShipType = nameof(SubjectRoleEnum.Inspected),
                        CheckLogBookMatches = nameof(InspectionToggleTypesEnum.Y),
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

                    InspectionPatrolVehicle inspectionPatrolVehicleDb = new InspectionPatrolVehicle
                    {
                        InspectionId = inspection.Id,
                        PatrolUnregisteredVessel = vesselDb,
                        PatrolVesselCoordinates = patrolVehicle.Location != null
                            ? new Point(patrolVehicle.Location.Longitude, patrolVehicle.Location.Latitude)
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

                    InspectionInspector inspectorInspectionDb = new InspectionInspector
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

        private void AddFishingGears(InspectionRegister inspection, List<InspectedFishingGearDTO> inspectedFishingGears)
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
                                FishingGearMark entry = new FishingGearMark
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
                                FishingGearPinger entry = new FishingGearPinger
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

                    InspectedFishingGear inspectedGearDb = new InspectedFishingGear
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

        private void AddInspectedPersons(InspectionRegister inspection, List<InspectionSubjectPersonnelDTO> inspectedPersonnel, SubjectRoleEnum roleEnum)
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
                    InspectedPerson inspectedPersonDb = new InspectedPerson
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

        private void AddInspectionChecks(InspectionRegister inspection, List<InspectionCheckDTO> checks, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
        {
            if (checks != null)
            {
                foreach (InspectionCheckDTO check in checks)
                {
                    InspectionCheck checkDb = new InspectionCheck
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
                        InspectionObservationText textDb = new InspectionObservationText
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

        private UnregisteredPerson AddUnregisteredPerson(UnregisteredPersonDTO itemDTO)
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

        private UnregisteredVessel AddUnregisteredShip(VesselDTO itemDTO)
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

        #endregion

        #region Get

        private IQueryable<InspectionDTO> GetAllNoFilter(bool isActive, int? userId)
        {
            IQueryable<InspectionDTO> result = (
                from inspection in Db.InspectionsRegister
                join inspectionType in Db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                join inspectionState in Db.NinspectionStates on inspection.StateId equals inspectionState.Id
                where inspection.IsActive == isActive
                    && (!userId.HasValue || inspection.CreatedByUserId == userId)
                orderby inspection.CreatedOn descending
                select new InspectionDTO
                {
                    Id = inspection.Id,
                    ReportNumber = inspection.ReportNum,
                    StartDate = inspection.InspectionStart,
                    InspectionType = Enum.Parse<InspectionTypesEnum>(inspectionType.Code),
                    InspectionTypeName = inspectionType.Name,
                    InspectionState = Enum.Parse<InspectionStatesEnum>(inspectionState.Code),
                    InspectionStateName = inspectionState.Name,
                    Inspectors = string.Join(PersonSeparator,
                        from inspInspector in Db.InspectionInspectors
                        join inspector in Db.Inspectors on inspInspector.InspectorId equals inspector.Id
                        join unregPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPersonJoin
                        from unregPerson in unregPersonJoin.DefaultIfEmpty()
                        join user in Db.Users on inspector.UserId equals user.Id into userJoin
                        from user in userJoin.DefaultIfEmpty()
                        join person in Db.Persons on user.PersonId equals person.Id into personJoin
                        from person in personJoin.DefaultIfEmpty()
                        where inspInspector.IsActive
                            && inspInspector.InspectionId == inspection.Id
                        select person != null ? $"{person.FirstName} {(person.MiddleName == null ? string.Empty : person.MiddleName + " ")}{person.LastName}"
                            : unregPerson.IdentifierType == nameof(IdentifierTypeEnum.LEGAL) ? unregPerson.FirstName
                            : $"{unregPerson.FirstName} {(unregPerson.MiddleName == null ? string.Empty : unregPerson.MiddleName + " ")}{unregPerson.LastName}"
                    ),
                    InspectionSubjects = string.Join(PersonSeparator,
                        from inspPerson in Db.InspectedPersons
                        join unregPerson in Db.UnregisteredPersons on inspPerson.UnregisteredPersonId equals unregPerson.Id into unregPersonJoin
                        from unregPerson in unregPersonJoin.DefaultIfEmpty()
                        join legal in Db.Legals on inspPerson.LegalId equals legal.Id into legalJoin
                        from legal in legalJoin.DefaultIfEmpty()
                        join person in Db.Persons on inspPerson.PersonId equals person.Id into personJoin
                        from person in personJoin.DefaultIfEmpty()
                        where inspPerson.IsActive
                            && inspPerson.InspectionId == inspection.Id
                        select person != null ? $"{person.FirstName} {(person.MiddleName == null ? string.Empty : person.MiddleName + " ")}{person.LastName}"
                            : legal != null ? legal.Name
                            : unregPerson.IdentifierType == nameof(IdentifierTypeEnum.LEGAL) ? unregPerson.FirstName
                            : $"{unregPerson.FirstName} {(unregPerson.MiddleName == null ? string.Empty : unregPerson.MiddleName + " ")}{unregPerson.LastName}"
                    ),
                    LastUpdateDate = inspection.UpdatedOn ?? inspection.CreatedOn,
                    IsActive = inspection.IsActive
                }).AsSingleQuery();
            return result;
        }

        private IQueryable<InspectionDTO> GetAllFreeTextFilter(string freeTextSearch, bool isActive, int? userId)
        {
            freeTextSearch = freeTextSearch.ToLower();

            List<int> registeredInspectorIds = (
                from inspector in Db.Inspectors
                join userInspector in Db.Users on inspector.UserId equals userInspector.Id
                join userInfoInspector in Db.UserInfos on userInspector.Id equals userInfoInspector.UserId
                join userInspectorPerson in Db.Persons on userInspector.PersonId equals userInspectorPerson.Id
                where userInfoInspector.TerritoryUnitId.ToString() == freeTextSearch
                    || userInspector.Username.ToLower().StartsWith(freeTextSearch)
                    || userInspectorPerson.FirstName.ToLower().StartsWith(freeTextSearch)
                    || userInspectorPerson.MiddleName.ToLower().StartsWith(freeTextSearch)
                    || userInspectorPerson.LastName.ToLower().StartsWith(freeTextSearch)
                select inspector.Id
            ).ToList();

            List<int> unregisteredInspectorIds = (
                from inspector in Db.Inspectors
                join person in Db.Persons on inspector.UnregisteredPersonId equals person.Id
                where person.FirstName.ToLower().StartsWith(freeTextSearch)
                    || person.MiddleName.ToLower().StartsWith(freeTextSearch)
                    || person.LastName.ToLower().StartsWith(freeTextSearch)
                select inspector.Id
            ).ToList();

            List<int> legalSubjectIds = (
                from legal in Db.Legals
                join inspectedPerson in Db.InspectedPersons on legal.Id equals inspectedPerson.LegalId
                where legal.Name.ToLower().StartsWith(freeTextSearch)
                    || legal.Eik == freeTextSearch
                select inspectedPerson.Id
            ).ToList();

            List<int> personSubjectIds = (
                from person in Db.Persons
                join inspectedPerson in Db.InspectedPersons on person.Id equals inspectedPerson.PersonId
                where person.FirstName.ToLower().StartsWith(freeTextSearch)
                    || person.MiddleName.ToLower().StartsWith(freeTextSearch)
                    || person.LastName.ToLower().StartsWith(freeTextSearch)
                    || person.EgnLnc == freeTextSearch
                select inspectedPerson.Id
            ).ToList();

            List<int> subjectIds = legalSubjectIds.Concat(personSubjectIds).ToList();

            List<int> inspectorIds = registeredInspectorIds != null && unregisteredInspectorIds != null
                ? registeredInspectorIds.Concat(unregisteredInspectorIds).ToList()
                : registeredInspectorIds ?? unregisteredInspectorIds;

            HashSet<int> inspectionIds = (
                from inspection in Db.InspectionsRegister
                join inspectionType in Db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                join inspectionInspector in Db.InspectionInspectors on inspection.Id equals inspectionInspector.InspectionId into inspectionInspectorMathTable
                from inspectionInspectorMatch in inspectionInspectorMathTable.DefaultIfEmpty()
                join inspectedSubject in Db.InspectedPersons on inspection.Id equals inspectedSubject.InspectionId into inspectedSubjectsMatchTable
                from inspectedSubjectMatch in inspectedSubjectsMatchTable.DefaultIfEmpty()
                where inspection.IsActive == isActive
                    && (!userId.HasValue || inspection.CreatedByUserId == userId)
                    && (inspectionType.Code.ToLower() == freeTextSearch
                    || inspection.ReportNum.Contains(freeTextSearch)
                    || (inspectionInspectorMatch != null && inspectorIds.Contains(inspectionInspectorMatch.InspectorId))
                    || (inspectedSubjectMatch != null && subjectIds.Contains(inspectedSubjectMatch.Id)))
                select inspection.Id
            ).ToHashSet();

            IQueryable<InspectionDTO> result = (
                from inspection in Db.InspectionsRegister
                join inspectionType in Db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                join inspectionState in Db.NinspectionStates on inspection.StateId equals inspectionState.Id
                where inspectionIds == null || inspectionIds.Contains(inspection.Id)
                orderby inspection.CreatedOn descending
                select new InspectionDTO
                {
                    Id = inspection.Id,
                    ReportNumber = inspection.ReportNum,
                    StartDate = inspection.InspectionStart,
                    InspectionType = Enum.Parse<InspectionTypesEnum>(inspectionType.Code),
                    InspectionTypeName = inspectionType.Name,
                    InspectionState = Enum.Parse<InspectionStatesEnum>(inspectionState.Code),
                    InspectionStateName = inspectionState.Name,
                    Inspectors = string.Join(PersonSeparator,
                        from inspInspector in Db.InspectionInspectors
                        join inspector in Db.Inspectors on inspInspector.InspectorId equals inspector.Id
                        join unregPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPersonJoin
                        from unregPerson in unregPersonJoin.DefaultIfEmpty()
                        join user in Db.Users on inspector.UserId equals user.Id into userJoin
                        from user in userJoin.DefaultIfEmpty()
                        join person in Db.Persons on user.PersonId equals person.Id into personJoin
                        from person in personJoin.DefaultIfEmpty()
                        where inspInspector.IsActive
                            && inspInspector.InspectionId == inspection.Id
                        select person != null ? $"{person.FirstName} {(person.MiddleName == null ? string.Empty : person.MiddleName + " ")}{person.LastName}"
                            : unregPerson.IdentifierType == nameof(IdentifierTypeEnum.LEGAL) ? unregPerson.FirstName
                            : $"{unregPerson.FirstName} {(unregPerson.MiddleName == null ? string.Empty : unregPerson.MiddleName + " ")}{unregPerson.LastName}"
                    ),
                    InspectionSubjects = string.Join(PersonSeparator,
                        from inspPerson in Db.InspectedPersons
                        join unregPerson in Db.UnregisteredPersons on inspPerson.UnregisteredPersonId equals unregPerson.Id into unregPersonJoin
                        from unregPerson in unregPersonJoin.DefaultIfEmpty()
                        join legal in Db.Legals on inspPerson.LegalId equals legal.Id into legalJoin
                        from legal in legalJoin.DefaultIfEmpty()
                        join person in Db.Persons on inspPerson.PersonId equals person.Id into personJoin
                        from person in personJoin.DefaultIfEmpty()
                        where inspPerson.IsActive
                            && inspPerson.InspectionId == inspection.Id
                        select person != null ? $"{person.FirstName} {(person.MiddleName == null ? string.Empty : person.MiddleName + " ")}{person.LastName}"
                            : legal != null ? legal.Name
                            : unregPerson.IdentifierType == nameof(IdentifierTypeEnum.LEGAL) ? unregPerson.FirstName
                            : $"{unregPerson.FirstName} {(unregPerson.MiddleName == null ? string.Empty : unregPerson.MiddleName + " ")}{unregPerson.LastName}"
                    ),
                    LastUpdateDate = inspection.UpdatedOn ?? inspection.CreatedOn,
                    IsActive = inspection.IsActive
                }).AsSingleQuery();

            return result;
        }

        private IQueryable<InspectionDTO> GetAllFilter(InspectionsFilters filter, int? userId)
        {
            filter.Inspector = filter.Inspector?.ToLower();
            filter.SubjectName = filter.SubjectName?.ToLower();
            filter.SubjectIsLegal = filter.SubjectIsLegal.GetValueOrDefault();

            List<int> registeredInspectorIds = null;
            List<int> unregisteredInspectorIds = null;
            List<int> subjectIds = null;

            if (filter.TerritoryNode != null || !string.IsNullOrWhiteSpace(filter.Inspector))
            {
                registeredInspectorIds = (
                    from inspector in Db.Inspectors
                    join userInspector in Db.Users on inspector.UserId equals userInspector.Id
                    join userInfoInspector in Db.UserInfos on userInspector.Id equals userInfoInspector.UserId
                    join userInspectorPerson in Db.Persons on userInspector.PersonId equals userInspectorPerson.Id
                    where (filter.TerritoryNode == null
                            || userInfoInspector.TerritoryUnitId == filter.TerritoryNode)
                        && (string.IsNullOrWhiteSpace(filter.Inspector)
                            || userInspector.Username.ToLower().StartsWith(filter.Inspector)
                            || userInspectorPerson.FirstName.ToLower().StartsWith(filter.Inspector)
                            || userInspectorPerson.MiddleName.ToLower().StartsWith(filter.Inspector)
                            || userInspectorPerson.LastName.ToLower().StartsWith(filter.Inspector))
                    select inspector.Id
                ).ToList();
            }

            if (!string.IsNullOrEmpty(filter.Inspector))
            {
                unregisteredInspectorIds = (
                    from inspector in Db.Inspectors
                    join person in Db.Persons on inspector.UnregisteredPersonId equals person.Id
                    where person.FirstName.ToLower().StartsWith(filter.Inspector)
                        || person.MiddleName.ToLower().StartsWith(filter.Inspector)
                        || person.LastName.ToLower().StartsWith(filter.Inspector)
                    select inspector.Id
                ).ToList();
            }

            if (filter.SubjectIsLegal.Value && (!string.IsNullOrWhiteSpace(filter.SubjectName) || !string.IsNullOrWhiteSpace(filter.SubjectEIK)))
            {
                subjectIds = (
                    from legal in Db.Legals
                    join inspectedPerson in Db.InspectedPersons on legal.Id equals inspectedPerson.LegalId
                    where (string.IsNullOrWhiteSpace(filter.SubjectName)
                            || legal.Name.ToLower().StartsWith(filter.SubjectName))
                        && (string.IsNullOrWhiteSpace(filter.SubjectEIK)
                            || legal.Eik == filter.SubjectEIK)
                    select inspectedPerson.Id
                ).ToList();
            }
            else if (!filter.SubjectIsLegal.Value && (!string.IsNullOrWhiteSpace(filter.SubjectName) || !string.IsNullOrWhiteSpace(filter.SubjectEGN)))
            {
                subjectIds = (
                    from person in Db.Persons
                    join inspectedPerson in Db.InspectedPersons on person.Id equals inspectedPerson.PersonId
                    where (string.IsNullOrWhiteSpace(filter.SubjectName)
                            || person.FirstName.ToLower().StartsWith(filter.SubjectName)
                            || person.MiddleName.ToLower().StartsWith(filter.SubjectName)
                            || person.LastName.ToLower().StartsWith(filter.SubjectName))
                        && (string.IsNullOrWhiteSpace(filter.SubjectEGN)
                            || person.EgnLnc == filter.SubjectEGN)
                    select inspectedPerson.Id
                ).ToList();
            }

            List<int> inspectorIds = registeredInspectorIds != null && unregisteredInspectorIds != null
                ? registeredInspectorIds.Concat(unregisteredInspectorIds).ToList()
                : registeredInspectorIds ?? unregisteredInspectorIds;

            HashSet<int> inspectionIds;

            if (filter.ShipId == null)
            {
                inspectionIds = (
                    from inspection in Db.InspectionsRegister
                    join inspectionInspector in Db.InspectionInspectors on inspection.Id equals inspectionInspector.InspectionId into inspectionInspectorMathTable
                    from inspectionInspectorMatch in inspectionInspectorMathTable.DefaultIfEmpty()
                    join inspectedSubject in Db.InspectedPersons on inspection.Id equals inspectedSubject.InspectionId into inspectedSubjectsMatchTable
                    from inspectedSubjectMatch in inspectedSubjectsMatchTable.DefaultIfEmpty()
                    where (filter.ShowBothActiveAndInactive == true
                            || inspection.IsActive == !filter.ShowInactiveRecords)
                        && (!userId.HasValue || inspection.CreatedByUserId == userId)
                        && (filter.InspectionTypeId == null
                            || inspection.InspectionTypeId == filter.InspectionTypeId)
                        && (string.IsNullOrWhiteSpace(filter.ReportNumber)
                            || inspection.ReportNum.Contains(filter.ReportNumber))
                        && ((filter.DateFrom == null && filter.DateTo == null)
                            || (filter.DateFrom != null && filter.DateTo != null
                                && inspection.InspectionStart.Date <= filter.DateTo.Value.Date
                                && (inspection.InspectionEnd == null || inspection.InspectionEnd.Value.Date >= filter.DateFrom.Value.Date))
                            || (filter.DateFrom == null && filter.DateTo != null
                                && inspection.InspectionStart.Date <= filter.DateTo.Value.Date)
                            || (filter.DateFrom != null && filter.DateTo == null
                                && (inspection.InspectionEnd == null || inspection.InspectionEnd.Value.Date >= filter.DateFrom.Value.Date)))
                        && (!filter.UpdatedAfter.HasValue || (inspection.UpdatedOn ?? inspection.CreatedOn) > filter.UpdatedAfter.Value)
                        && (inspectorIds == null || (inspectionInspectorMatch != null && inspectorIds.Contains(inspectionInspectorMatch.InspectorId)))
                        && (subjectIds == null || (inspectedSubjectMatch != null && subjectIds.Contains(inspectedSubjectMatch.Id)))
                    select inspection.Id
                ).ToHashSet();
            }
            else
            {
                List<string> shipInspectionTypes = new List<string>
                {
                    nameof(InspectionTypesEnum.OFS),
                    nameof(InspectionTypesEnum.IBS),
                    nameof(InspectionTypesEnum.IBP),
                    nameof(InspectionTypesEnum.ITB),
                    nameof(InspectionTypesEnum.IGM),
                };

                int shipUid = Db.ShipsRegister
                    .Where(f => f.Id == filter.ShipId)
                    .Select(f => f.ShipUid)
                    .Single();

                List<int> shipIds = Db.ShipsRegister
                    .Where(f => f.ShipUid == shipUid)
                    .Select(f => f.Id)
                    .ToList();

                inspectionIds = (
                    from inspection in Db.InspectionsRegister
                    join ofs in Db.ObservationsAtSea on inspection.Id equals ofs.InspectionId into ofsGrp
                    from ofs in ofsGrp.DefaultIfEmpty()
                    join ibs in Db.ShipInspections on inspection.Id equals ibs.InspectionId into ibsGrp
                    from ibs in ibsGrp.DefaultIfEmpty()
                    join igm in Db.FishingGearChecks on inspection.Id equals igm.InspectionId into igmGrp
                    from igm in igmGrp.DefaultIfEmpty()
                    join inspectionType in Db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                    join inspectionInspector in Db.InspectionInspectors on inspection.Id equals inspectionInspector.InspectionId into inspectionInspectorMathTable
                    from inspectionInspectorMatch in inspectionInspectorMathTable.DefaultIfEmpty()
                    join inspectedSubject in Db.InspectedPersons on inspection.Id equals inspectedSubject.InspectionId into inspectedSubjectsMatchTable
                    from inspectedSubjectMatch in inspectedSubjectsMatchTable.DefaultIfEmpty()
                    where (filter.ShowBothActiveAndInactive == true
                            || inspection.IsActive == !filter.ShowInactiveRecords)
                        && (!userId.HasValue || inspection.CreatedByUserId == userId)
                        && (filter.InspectionTypeId == null
                            || inspectionType.Id == filter.InspectionTypeId)
                        && (string.IsNullOrWhiteSpace(filter.ReportNumber)
                            || inspection.ReportNum.Contains(filter.ReportNumber))
                        && ((filter.DateFrom == null && filter.DateTo == null)
                            || (filter.DateFrom != null && filter.DateTo != null
                                && inspection.InspectionStart.Date <= filter.DateTo.Value.Date
                                && (inspection.InspectionEnd == null || inspection.InspectionEnd.Value.Date >= filter.DateFrom.Value.Date))
                            || (filter.DateFrom == null && filter.DateTo != null
                                && inspection.InspectionStart.Date <= filter.DateTo.Value.Date)
                            || (filter.DateFrom != null && filter.DateTo == null
                                && (inspection.InspectionEnd == null || inspection.InspectionEnd.Value.Date >= filter.DateFrom.Value.Date)))
                        && (!filter.UpdatedAfter.HasValue || (inspection.UpdatedOn ?? inspection.CreatedOn) > filter.UpdatedAfter.Value)
                        && (inspectorIds == null || (inspectionInspectorMatch != null && inspectorIds.Contains(inspectionInspectorMatch.InspectorId)))
                        && (subjectIds == null || (inspectedSubjectMatch != null && subjectIds.Contains(inspectedSubjectMatch.Id)))
                        && (ofs != null || ibs != null || igm != null)
                        && (ofs == null || (ofs.ObservedShipId != null && shipIds.Contains(ofs.ObservedShipId.Value)))
                        && (ibs == null || (ibs.InspectiedShipId != null && shipIds.Contains(ibs.InspectiedShipId.Value)))
                        && (igm == null || (igm.ShipId != null && shipIds.Contains(igm.ShipId.Value)))
                        && shipInspectionTypes.Contains(inspectionType.Code)
                    select inspection.Id
                ).ToHashSet();
            }

            IQueryable<InspectionDTO> result = (
                from inspection in Db.InspectionsRegister
                join inspectionType in Db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                join inspectionState in Db.NinspectionStates on inspection.StateId equals inspectionState.Id
                where inspectionIds == null || inspectionIds.Contains(inspection.Id)
                orderby inspection.CreatedOn descending
                select new InspectionDTO
                {
                    Id = inspection.Id,
                    ReportNumber = inspection.ReportNum,
                    StartDate = inspection.InspectionStart,
                    InspectionType = Enum.Parse<InspectionTypesEnum>(inspectionType.Code),
                    InspectionTypeName = inspectionType.Name,
                    InspectionState = Enum.Parse<InspectionStatesEnum>(inspectionState.Code),
                    InspectionStateName = inspectionState.Name,
                    Inspectors = string.Join(PersonSeparator,
                        from inspInspector in Db.InspectionInspectors
                        join inspector in Db.Inspectors on inspInspector.InspectorId equals inspector.Id
                        join unregPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPersonJoin
                        from unregPerson in unregPersonJoin.DefaultIfEmpty()
                        join user in Db.Users on inspector.UserId equals user.Id into userJoin
                        from user in userJoin.DefaultIfEmpty()
                        join person in Db.Persons on user.PersonId equals person.Id into personJoin
                        from person in personJoin.DefaultIfEmpty()
                        where inspInspector.IsActive
                            && inspInspector.InspectionId == inspection.Id
                        select person != null ? $"{person.FirstName} {(person.MiddleName == null ? string.Empty : person.MiddleName + " ")}{person.LastName}"
                            : unregPerson.IdentifierType == nameof(IdentifierTypeEnum.LEGAL) ? unregPerson.FirstName
                            : $"{unregPerson.FirstName} {(unregPerson.MiddleName == null ? string.Empty : unregPerson.MiddleName + " ")}{unregPerson.LastName}"
                    ),
                    InspectionSubjects = string.Join(PersonSeparator,
                        from inspPerson in Db.InspectedPersons
                        join unregPerson in Db.UnregisteredPersons on inspPerson.UnregisteredPersonId equals unregPerson.Id into unregPersonJoin
                        from unregPerson in unregPersonJoin.DefaultIfEmpty()
                        join legal in Db.Legals on inspPerson.LegalId equals legal.Id into legalJoin
                        from legal in legalJoin.DefaultIfEmpty()
                        join person in Db.Persons on inspPerson.PersonId equals person.Id into personJoin
                        from person in personJoin.DefaultIfEmpty()
                        where inspPerson.IsActive
                            && inspPerson.InspectionId == inspection.Id
                        select person != null ? $"{person.FirstName} {(person.MiddleName == null ? string.Empty : person.MiddleName + " ")}{person.LastName}"
                            : legal != null ? legal.Name
                            : unregPerson.IdentifierType == nameof(IdentifierTypeEnum.LEGAL) ? unregPerson.FirstName
                            : $"{unregPerson.FirstName} {(unregPerson.MiddleName == null ? string.Empty : unregPerson.MiddleName + " ")}{unregPerson.LastName}"
                    ),
                    LastUpdateDate = inspection.UpdatedOn ?? inspection.CreatedOn,
                    IsActive = inspection.IsActive
                }).AsSingleQuery();

            return result;
        }

        private InspectionEditDTO GetRegisterOTH(InspectionEditDTO inspection)
        {
            InspectionConstativeProtocolDTO othDTO = (
                from cpi in Db.ConstativeProtocolInspections
                where cpi.InspectionId == inspection.Id
                select new InspectionConstativeProtocolDTO
                {
                    InspectedObjectName = cpi.InspectedObjectName,
                    InspectedPersonName = cpi.InspectedPersonName,
                    InspectorName = cpi.InspectorName,
                    Location = cpi.Location,
                    Witness1Name = cpi.Witness1Name,
                    Witness2Name = cpi.Witness2Name,
                }).Single();

            othDTO.FishingGears = (
                from ifg in Db.InspectedFishingGears
                join fishingGear in Db.FishingGearRegisters on ifg.InspectedFishingGearId equals fishingGear.Id
                where ifg.InspectionId == inspection.Id
                select new InspectedCPFishingGearDTO
                {
                    Id = fishingGear.Id,
                    Description = fishingGear.Description,
                    FishingGearId = fishingGear.FishingGearTypeId,
                    GearCount = fishingGear.GearCount,
                    Length = fishingGear.Length.Value,
                    IsStored = ifg.IsStored.Value,
                    IsTaken = ifg.IsTaken.Value,
                }
            ).ToList();

            othDTO.Catches = (
                from icm in Db.InspectionCatchMeasures
                where icm.InspectionId == inspection.Id
                select new InspectedCPCatchDTO
                {
                    Id = icm.Id,
                    FishId = icm.FishId.Value,
                    CatchQuantity = icm.CatchQuantity.Value,
                    IsDestroyed = icm.IsDestroyed.Value,
                    IsTaken = icm.IsTaken.Value,
                    IsDonated = icm.IsDonated.Value,
                    IsStored = icm.IsStored.Value,
                }
            ).ToList();

            return AssignFromBase(othDTO, inspection);
        }

        private InspectionEditDTO GetRegisterOFS(InspectionEditDTO inspection)
        {
            DateTime now = DateTime.Now;

            InspectionObservationAtSeaDTO ofsDTO = (
                from observation in Db.ObservationsAtSea
                where observation.InspectionId == inspection.Id
                select new InspectionObservationAtSeaDTO
                {
                    ObservedVessel = new VesselDuringInspectionDTO
                    {
                        ShipId = observation.ObservedShipId,
                        UnregisteredVesselId = observation.ObservedUnregisteredShipId,
                        Location = observation.ObservedShipCoordinates == null ? null : new LocationDTO
                        {
                            Latitude = observation.ObservedShipCoordinates.Y,
                            Longitude = observation.ObservedShipCoordinates.X
                        },
                        LocationDescription = observation.ObservedShipLocation,
                        CatchZoneId = observation.ObservedShipCatchZoneId,
                    },
                    Course = observation.Direction,
                    Speed = observation.Speed,
                    HasShipContact = observation.HasShipContact,
                    HasShipCommunication = observation.HasShipCommunication,
                    ShipCommunicationDescription = observation.ShipCommunicationDescr,
                }).Single();

            ofsDTO.ObservedVessel = GetInspectedShip(ofsDTO.ObservedVessel);

            ofsDTO.ObservedVesselActivities = GetVesselActivities(inspection.Id.Value);

            ofsDTO.ObservationTools = GetObservationTools(inspection.Id.Value);

            return AssignFromBase(ofsDTO, inspection);
        }

        private InspectionEditDTO GetRegisterIBS(InspectionEditDTO inspection)
        {
            DateTime now = DateTime.Now;

            InspectionAtSeaDTO ibsDTO = (
                from insp in Db.ShipInspections
                where insp.InspectionId == inspection.Id.Value
                    && insp.InspectedShipType == nameof(SubjectRoleEnum.Inspected)
                select new InspectionAtSeaDTO
                {
                    CaptainComment = insp.CaptainComment,
                    InspectedShip = new VesselDuringInspectionDTO
                    {
                        ShipId = insp.InspectiedShipId,
                        UnregisteredVesselId = insp.InspectedUnregisteredShipId,
                        Location = insp.InspectedShipCoordinates == null ? null : new LocationDTO
                        {
                            Longitude = insp.InspectedShipCoordinates.X,
                            Latitude = insp.InspectedShipCoordinates.Y
                        },
                        LocationDescription = insp.InspectedShipLocation,
                        CatchZoneId = insp.InspectedShipCatchZoneId,
                    },
                }).SingleOrDefault();

            ibsDTO.InspectedShip = GetInspectedShip(ibsDTO.InspectedShip);
            ibsDTO.FishingGears = GetFishingGears(inspection.Id.Value);
            ibsDTO.CatchMeasures = GetCatchMeasures(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.Personnel = GetPersonnel(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.LastPortVisit = GetLastPort(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.PermitLicenses = GetPermits(inspection.Id.Value, SubjectRoleEnum.Inspected);
            ibsDTO.LogBooks = GetLogBooks(inspection.Id.Value, SubjectRoleEnum.Inspected);

            return AssignFromBase(ibsDTO, inspection);
        }

        private InspectionEditDTO GetRegisterIBP(InspectionEditDTO inspection)
        {
            return GetRegisterITB(inspection);
        }

        private InspectionEditDTO GetRegisterITB(InspectionEditDTO inspection)
        {
            InspectionTransboardingShipDTO receivingShipInspection = GetRegisterITBShip(inspection.Id.Value, SubjectRoleEnum.TransboardReceiver);

            InspectionTransboardingShipDTO sendingShipInspection = GetRegisterITBShip(inspection.Id.Value, SubjectRoleEnum.TransboardSender);

            List<InspectedFishingGearDTO> fishingGears = GetFishingGears(inspection.Id.Value);

            InspectionTransboardingDTO itbDTO = new InspectionTransboardingDTO
            {
                TransboardedCatchMeasures = GetCatchMeasures(inspection.Id.Value, SubjectRoleEnum.Inspected),
                SendingShipInspection = sendingShipInspection,
                ReceivingShipInspection = receivingShipInspection,
                FishingGears = fishingGears,
            };

            return AssignFromBase(itbDTO, inspection);
        }

        private InspectionTransboardingShipDTO GetRegisterITBShip(int inspectionId, SubjectRoleEnum role)
        {
            DateTime now = DateTime.Now;

            InspectionTransboardingShipDTO ibsDTO = (
                from insp in Db.ShipInspections
                where insp.InspectionId == inspectionId
                    && insp.InspectedShipType == role.ToString()
                select new InspectionTransboardingShipDTO
                {
                    CaptainComment = insp.CaptainComment,
                    InspectedShip = new VesselDuringInspectionDTO
                    {
                        ShipId = insp.InspectiedShipId,
                        UnregisteredVesselId = insp.InspectedUnregisteredShipId,
                        Location = insp.InspectedShipCoordinates == null ? null : new LocationDTO
                        {
                            Longitude = insp.InspectedShipCoordinates.X,
                            Latitude = insp.InspectedShipCoordinates.Y
                        },
                        LocationDescription = insp.InspectedShipLocation,
                        CatchZoneId = insp.InspectedShipCatchZoneId,
                    },
                }).SingleOrDefault();

            if (ibsDTO != null)
            {
                ibsDTO.InspectedShip = GetInspectedShip(ibsDTO.InspectedShip);
                ibsDTO.CatchMeasures = GetCatchMeasures(inspectionId, role);
                ibsDTO.Personnel = GetPersonnel(inspectionId, role);
                ibsDTO.Checks = GetChecks(inspectionId, role);
                ibsDTO.LastPortVisit = GetLastPort(inspectionId, role);
                ibsDTO.PermitLicenses = GetPermits(inspectionId, role);
                ibsDTO.LogBooks = GetLogBooks(inspectionId, role);
            }

            return ibsDTO;
        }

        private InspectionEditDTO GetRegisterIVH(InspectionEditDTO inspection)
        {
            InspectionTransportVehicleDTO ivhDTO = (
                from insp in Db.TransportVehicleInspections
                where insp.InspectionId == inspection.Id.Value
                select new InspectionTransportVehicleDTO
                {
                    VehicleTypeId = insp.VehicleTypeId,
                    CountryId = insp.CountryId,
                    TractorBrand = insp.TractorBrand,
                    TractorModel = insp.TractorModel,
                    TractorLicensePlateNum = insp.TractorLicensePlateNum,
                    TrailerLicensePlateNum = insp.TrailerLicensePlateNum,
                    IsSealed = insp.IsSealedVehicle,
                    SealInstitutionId = insp.SealInstitutionId,
                    SealCondition = insp.SealCondition,
                    TransporterComment = insp.TransporterComment,
                    InspectionAddress = insp.InspectionLocation,
                    TransportDestination = insp.TransportDestination,
                    InspectionLocation = insp.InpectionLocationCoordinates != null
                       ? new LocationDTO { Longitude = insp.InpectionLocationCoordinates.X, Latitude = insp.InpectionLocationCoordinates.Y }
                       : null,
                }).Single();

            ivhDTO.CatchMeasures = GetDeclarationCatchMeasures(inspection.Id.Value);
            ivhDTO.Personnel = GetPersonnel(inspection.Id.Value);

            return AssignFromBase(ivhDTO, inspection);
        }

        private InspectionEditDTO GetRegisterIFS(InspectionEditDTO inspection)
        {
            InspectionFirstSaleDTO ifsDTO = (
                from insp in Db.FirstSaleInspections
                where insp.InspectionId == inspection.Id.Value
                select new InspectionFirstSaleDTO
                {
                    SubjectName = insp.Name,
                    SubjectAddress = insp.Address,
                    RepresentativeComment = insp.RepresentativeComment,
                }).Single();

            ifsDTO.CatchMeasures = GetDeclarationCatchMeasures(inspection.Id.Value);

            return AssignFromBase(ifsDTO, inspection);
        }

        private InspectionEditDTO GetRegisterIAQ(InspectionEditDTO inspection)
        {
            InspectionAquacultureDTO iaqDTO = (
                from insp in Db.AquacultureInspections
                where insp.InspectionId == inspection.Id.Value
                select new InspectionAquacultureDTO
                {
                    AquacultureId = insp.AquacultureRegisterId,
                    RepresentativeComment = insp.RepresentativeComment,
                    OtherFishingGear = insp.OtherFishingToolsDescription,
                    Location = insp.Coordinates != null
                       ? new LocationDTO { Longitude = insp.Coordinates.X, Latitude = insp.Coordinates.Y }
                       : null,
                }).Single();

            iaqDTO.CatchMeasures = GetCatchMeasures(inspection.Id.Value);

            return AssignFromBase(iaqDTO, inspection);
        }

        private InspectionEditDTO GetRegisterIFP(InspectionEditDTO inspection)
        {
            InspectionFisherDTO ifpDTO = (
                from insp in Db.FishermanInspections
                join unregisteredPerson in Db.UnregisteredPersons on insp.UnregisteredPersonId equals unregisteredPerson.Id into unregisteredPersonMatchTable
                from unregisteredPersonMatch in unregisteredPersonMatchTable.DefaultIfEmpty()
                where insp.InspectionId == inspection.Id.Value
                select new InspectionFisherDTO
                {
                    TicketNum = insp.TicketNum,
                    FishingRodsCount = insp.FishingRodCount,
                    FishingHooksCount = insp.FishingHooksCount,
                    FishermanComment = insp.FishermanComment,
                    InspectionAddress = insp.InspectionLocation,
                    InspectionLocation = insp.InpectionLocationCoordinates != null
                       ? new LocationDTO { Longitude = insp.InpectionLocationCoordinates.X, Latitude = insp.InpectionLocationCoordinates.Y }
                       : null,
                }).Single();

            ifpDTO.CatchMeasures = GetCatchMeasures(inspection.Id.Value);

            return AssignFromBase(ifpDTO, inspection);
        }

        private InspectionEditDTO GetRegisterCWO(InspectionEditDTO inspection)
        {
            InspectionCheckWaterObjectDTO cwoDTO = (
                from check in Db.WaterObjectChecks
                where check.InspectionId == inspection.Id.Value
                select new InspectionCheckWaterObjectDTO
                {
                    ObjectName = check.Name,
                    WaterObjectTypeId = check.WaterObjectTypeId,
                    WaterObjectLocation = new LocationDTO { Longitude = check.WaterObjectCoordinates.X, Latitude = check.WaterObjectCoordinates.Y },
                }).Single();

            cwoDTO.FishingGears = (
                from ifg in Db.InspectedFishingGears
                join ifgn in Db.FishingGearRegisters on ifg.InspectedFishingGearId equals ifgn.Id
                where ifg.InspectionId == inspection.Id.Value
                    && ifg.IsActive
                select new WaterInspectionFishingGearDTO
                {
                    Id = ifgn.Id,
                    TypeId = ifgn.FishingGearTypeId,
                    Count = ifgn.GearCount,
                    Length = ifgn.Length,
                    Height = ifgn.Height,
                    NetEyeSize = ifgn.NetEyeSize,
                    HookCount = ifgn.HookCount,
                    Description = ifgn.Description,
                    TowelLength = ifgn.TowelLength,
                    HouseLength = ifgn.HouseLength,
                    HouseWidth = ifgn.HouseWidth,
                    HasPingers = ifgn.HasPinger,
                    StorageLocation = ifg.StorageLocation,
                    IsTaken = ifg.IsTaken ?? false,
                    IsStored = ifg.IsStored ?? false,
                    IsActive = ifgn.IsActive,
                }
            ).ToList();

            fishingGearService.MapFishingGearMarksAndPingers(cwoDTO.FishingGears.ConvertAll(f => f as FishingGearDTO));

            foreach (WaterInspectionFishingGearDTO fishingGear in cwoDTO.FishingGears)
            {
                fishingGear.MarksNumbers = string.Join(", ", fishingGear.Marks.Select(x => x.Number));
            }

            cwoDTO.Vessels = (
                from vessel in Db.InspectionVessels
                where vessel.InspectionId == inspection.Id.Value
                select new WaterInspectionVesselDTO
                {
                    Id = vessel.Id,
                    Color = vessel.Color,
                    Length = vessel.Length,
                    Number = vessel.Number,
                    StorageLocation = vessel.StorageLocation,
                    TotalCount = vessel.TotalCount,
                    VesselTypeId = vessel.VesselTypeId,
                    Width = vessel.Width,
                    IsTaken = vessel.IsTaken ?? false,
                    IsStored = vessel.IsStored ?? false,
                }
            ).ToList();

            cwoDTO.Engines = (
                from engine in Db.InspectionEngines
                where engine.InspectionId == inspection.Id.Value
                select new WaterInspectionEngineDTO
                {
                    Id = engine.Id,
                    Model = engine.EngineModel,
                    Power = engine.EnginePower,
                    Type = engine.EngineType,
                    EngineDescription = engine.EngineDescription,
                    StorageLocation = engine.StorageLocation,
                    TotalCount = engine.TotalCount,
                    IsTaken = engine.IsTaken ?? false,
                    IsStored = engine.IsStored ?? false,
                }
            ).ToList();

            cwoDTO.Catches = GetCatchMeasures(inspection.Id.Value);

            return AssignFromBase(cwoDTO, inspection);
        }

        private InspectionEditDTO GetRegisterIGM(InspectionEditDTO inspection)
        {
            DateTime now = DateTime.Now;

            InspectionCheckToolMarkDTO igmDTO = (
                from check in Db.FishingGearChecks
                join ship in Db.ShipsRegister on check.ShipId equals ship.Id into shipMatchTable
                from shipMatch in shipMatchTable.DefaultIfEmpty()
                join unregisteredShip in Db.UnregisteredVessels on check.UnregisteredShipId equals unregisteredShip.Id into unregisteredShipMatchTable
                from unregisteredShipMatch in unregisteredShipMatchTable.DefaultIfEmpty()
                where check.InspectionId == inspection.Id.Value
                select new InspectionCheckToolMarkDTO
                {
                    InspectedShip = shipMatch == null ? new VesselDuringInspectionDTO
                    {
                        ShipId = null,
                        UnregisteredVesselId = unregisteredShipMatch.Id,
                        IsRegistered = false,
                        Name = unregisteredShipMatch.Name,
                        ExternalMark = unregisteredShipMatch.ExternalMark,
                        CFR = unregisteredShipMatch.Cfr,
                        UVI = unregisteredShipMatch.Uvi,
                        RegularCallsign = unregisteredShipMatch.IrcscallSign,
                        MMSI = unregisteredShipMatch.Mmsi,
                        FlagCountryId = unregisteredShipMatch.FlagCountryId,
                        PatrolVehicleTypeId = unregisteredShipMatch.PatrolVehicleTypeId,
                        VesselTypeId = unregisteredShipMatch.VesselTypeId,
                        IsActive = unregisteredShipMatch.IsActive,
                    } : new VesselDuringInspectionDTO
                    {
                        ShipId = shipMatch.Id,
                        UnregisteredVesselId = null,
                        IsRegistered = true,
                        Name = shipMatch.Name,
                        ExternalMark = shipMatch.ExternalMark,
                        CFR = shipMatch.Cfr,
                        UVI = shipMatch.Uvi,
                        RegularCallsign = shipMatch.IrcscallSign,
                        MMSI = shipMatch.Mmsi,
                        FlagCountryId = shipMatch.FlagCountryId,
                        PatrolVehicleTypeId = null,
                        VesselTypeId = shipMatch.VesselTypeId,
                        IsActive = shipMatch.ValidFrom <= now && shipMatch.ValidTo >= now,
                    },
                    PoundNetId = check.PoundNetId,
                    CheckReasonId = check.CheckReasonId,
                    RecheckReasonId = check.RecheckReasonId,
                    OtherRecheckReason = check.RecheckDescription,
                }).Single();

            List<InspectionPermitDTO> permits = GetPermits(inspection.Id.Value, SubjectRoleEnum.Inspected);

            if (permits.Count == 1)
            {
                igmDTO.PermitId = permits[0].PermitLicenseId;
            }

            igmDTO.FishingGears = GetFishingGears(inspection.Id.Value);
            igmDTO.Port = GetLastPort(inspection.Id.Value);

            return AssignFromBase(igmDTO, inspection);
        }

        private List<InspectionPermitDTO> GetPermits(int inspectionId, SubjectRoleEnum role)
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

        private List<InspectionLogBookDTO> GetLogBooks(int inspectionId, SubjectRoleEnum role)
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

        private List<InspectionCheckDTO> GetChecks(int inspectionId, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
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

        private List<InspectionSubjectPersonnelDTO> GetPersonnel(int inspectionId, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
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
                    InstitutionId = unregisteredVessel.InstitutionId,
                }
            ).ToList();

            return result;
        }

        private List<InspectionCatchMeasureDTO> GetCatchMeasures(int inspectionId, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
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

        private List<InspectedDeclarationCatchDTO> GetDeclarationCatchMeasures(int inspectionId)
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
                    LogBookType = Enum.Parse<DeclarationLogBookTypeEnum>(page.LogBookType),
                    LogBookPageId = page.LogBookType == nameof(DeclarationLogBookTypeEnum.ShipLogBook)
                        ? page.ShipLogBookPageId
                        : page.LogBookType == nameof(DeclarationLogBookTypeEnum.TransportationLogBook)
                        ? page.TransportationLogBookPageId
                        : page.LogBookType == nameof(DeclarationLogBookTypeEnum.FirstSaleLogBook)
                        ? page.FirstSaleLogBookPageId
                        : page.LogBookType == nameof(DeclarationLogBookTypeEnum.AdmissionLogBook)
                        ? page.AdmissionLogBookPageId
                        : null,
                    UnregisteredPageDate = page.UnregisteredPageDate,
                    UnregisteredPageNum = page.UnregisteredPageNum,
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

        private List<InspectedFishingGearDTO> GetFishingGears(int inspectionId)
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
                    HasAttachedAppliances = ifg.HasAttachedAppliances,
                    CheckInspectedMatchingRegisteredGear = ifg.CheckInspectedMatchingRegisteredGear,
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
                        IsActive = ifgn.IsActive
                    },
                    ifg.IsActive,
                }
            ).ToList();

            List<InspectedFishingGearDTO> result = new List<InspectedFishingGearDTO>(fishingGears.Count);

            fishingGearService.MapFishingGearMarksAndPingers(fishingGears.Select(f => f.InspectedFishingGear).Where(f => f != null).ToList());
            fishingGearService.MapFishingGearMarksAndPingers(fishingGears.Select(f => f.PermittedFishingGear).Where(f => f != null).ToList());

            foreach (var fishingGear in fishingGears)
            {
                if (fishingGear.PermittedFishingGear != null)
                {
                    fishingGear.PermittedFishingGear.MarksNumbers = string.Join(", ", fishingGear.PermittedFishingGear.Marks.Select(x => x.Number));
                }
                if (fishingGear.InspectedFishingGear != null)
                {
                    fishingGear.InspectedFishingGear.MarksNumbers = string.Join(", ", fishingGear.InspectedFishingGear.Marks.Select(x => x.Number));
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

        private List<InspectionVesselActivityNomenclatureDTO> GetVesselActivities(int inspectionId)
        {
            List<InspectionVesselActivityNomenclatureDTO> result = (
                from iva in Db.InspectionVesselActivities
                join va in Db.NvesselActivities on iva.VesselActivityId equals va.Id
                where iva.InspectionId == inspectionId
                    && iva.IsActive
                select new InspectionVesselActivityNomenclatureDTO
                {
                    Value = va.Id,
                    Code = va.Code,
                    DisplayName = va.Name,
                    Description = iva.ActivityDescr,
                    IsFishingActivity = va.IsFishingActivity,
                    HasAdditionalDescr = va.HasAdditionalDescr,
                    IsActive = iva.IsActive,
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

        private List<InspectionObservationToolDTO> GetObservationTools(int inspectionId)
        {
            List<InspectionObservationToolDTO> result = (
                from inspectionObsTool in Db.InspectionObservationTools
                where inspectionObsTool.InspectionId == inspectionId
                select new InspectionObservationToolDTO
                {
                    ObservationToolId = inspectionObsTool.ObservationToolId,
                    Description = inspectionObsTool.ObservationToolDesc,
                    IsOnBoard = inspectionObsTool.IsOnBoard,
                    IsActive = inspectionObsTool.IsActive
                }
            ).ToList();

            return result;
        }

        private PortVisitDTO GetLastPort(int inspectionId, SubjectRoleEnum role = SubjectRoleEnum.Inspected)
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

        private VesselDuringInspectionDTO GetInspectedShip(VesselDuringInspectionDTO vesselDTO)
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

        #endregion

        #region Other

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

        /// <summary>
        /// Често се генерира report number като "000000002" и се проваля качването към базата.
        /// В продукционна среда това не би трябвало да е проблем.
        /// </summary>
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

        private static InspectionEditDTO DeserializeInspectionDraft(string draftJson, InspectionTypesEnum inspectionType)
        {
            switch (inspectionType)
            {
                case InspectionTypesEnum.OFS: //OFS - Observation Fishing ship at Sea - Наблюдение на риболовен кораб в открито море
                    {
                        return CommonUtils.Deserialize<InspectionObservationAtSeaDTO>(draftJson);
                    }
                case InspectionTypesEnum.IBS: //IBS - Inspection on Board fishing Ship - Инспекция на борда на риболовен кораб ва вода
                    {
                        return CommonUtils.Deserialize<InspectionAtSeaDTO>(draftJson);
                    }
                case InspectionTypesEnum.IBP: //IBP - Inspection on Board fishing ship in Port - Инспекция на риболовен кораб в пристанище
                    {
                        return CommonUtils.Deserialize<InspectionTransboardingDTO>(draftJson);
                    }
                case InspectionTypesEnum.ITB: //ITB - Inspection TransBoarding - Инспекция при трансбордиране
                    {
                        return CommonUtils.Deserialize<InspectionTransboardingDTO>(draftJson);
                    }
                case InspectionTypesEnum.IVH: //IVH - Inspection of VeHicle - Инспекция на средство за транспорт
                    {
                        return CommonUtils.Deserialize<InspectionTransportVehicleDTO>(draftJson);
                    }
                case InspectionTypesEnum.IFS: //IFS - Inspection at first sale - Инспекция при първа продажба
                    {
                        return CommonUtils.Deserialize<InspectionFirstSaleDTO>(draftJson);
                    }
                case InspectionTypesEnum.IAQ: //IAQ - Inspection of AQuaculture property - Инспекция на аквакултурно стопанство
                    {
                        return CommonUtils.Deserialize<InspectionAquacultureDTO>(draftJson);
                    }
                case InspectionTypesEnum.IFP: //IFP - Inspection of Fishing Person - Инспекция на лица, извършващи любителски риболов
                    {
                        return CommonUtils.Deserialize<InspectionFisherDTO>(draftJson);
                    }
                case InspectionTypesEnum.CWO: //CWO - Check of Water Object - Проверка на воден обект без извършване на инспекция
                    {
                        return CommonUtils.Deserialize<InspectionCheckWaterObjectDTO>(draftJson);
                    }
                case InspectionTypesEnum.IGM: //CMU - Check and Mark of fishing Utility - Проверка и маркировка на риболовен уред
                    {
                        return CommonUtils.Deserialize<InspectionCheckToolMarkDTO>(draftJson);
                    }
            }

            return null;
        }

        private static TDto AssignFromBase<TDto>(TDto dto, InspectionEditDTO inspectionBaseDTO)
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
            return dto;
        }

        #endregion
    }
}
