using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
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
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.ControlActivity.Inspections;
using IARA.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.ControlActivity.Inspections
{
    public class CommonInspectionService : Service, ICommonInspectionService
    {
        private const string PersonSeparator = ", ";

        private readonly IFishingGearsService fishingGearService;
        private readonly IJasperReportExecutionService jasperReport;

        public CommonInspectionService(IARADbContext dbContext, IFishingGearsService fishingGearService, IJasperReportExecutionService jasperReport)
            : base(dbContext)
        {
            this.fishingGearService = fishingGearService;
            this.jasperReport = jasperReport;
        }

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

        public int AddRegisterEntry(InspectionDraftDTO itemDTO, InspectionTypesEnum inspectionType, int userId)
        {
            using TransactionScope scope = new();

            List<FileInfoDTO> files = itemDTO.Files;

            itemDTO.Files = null;

            int inspectionTypeId = Db.NinspectionTypes.Single(f => f.Code == inspectionType.ToString()).Id;
            int draftStateId = Db.NinspectionStates.Single(f => f.Code == nameof(InspectionStatesEnum.Draft)).Id;

            InspectionRegister newInspDbEntry = new()
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

        public void EditRegisterEntry(InspectionDraftDTO itemDTO)
        {
            using TransactionScope scope = new();

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

            inspDbEntry.InspectionStart = itemDTO.StartDate ?? DateTime.Today;
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

            using TransactionScope scope = new();

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

        public bool IsInspector(int userId)
        {
            return Db.Inspectors.Any(f => f.IsActive && f.UserId == userId);
        }

        public IQueryable<NomenclatureDTO> GetPatrolVehicles(bool isWaterVehicle)
        {
            PatrolVehicleTypeEnum type = isWaterVehicle ? PatrolVehicleTypeEnum.Marine : PatrolVehicleTypeEnum.Ground;

            IQueryable<NomenclatureDTO> result =
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
                };

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
            DateTime now = DateTime.Now;

            int shipUid = Db.ShipsRegister.First(f => f.Id == shipId).ShipUid;
            List<int> shipIds = Db.ShipsRegister.Where(f => f.ShipUid == shipUid)
                .Select(f => f.Id)
                .ToList();

            var owners = (from owner in Db.ShipOwners
                          where shipIds.Contains(owner.ShipRegisterId)
                              && owner.IsActive
                          select new
                          {
                              owner.Id,
                              PersonId = owner.OwnerPersonId,
                              LegalId = owner.OwnerLegalId,
                          }).ToList();

            var permits = (
                from plr in Db.CommercialFishingPermitLicensesRegisters
                join ship in Db.ShipsRegister on plr.ShipId equals ship.Id
                where plr.RecordType == nameof(RecordTypesEnum.Register)
                    && plr.PermitLicenseValidFrom.Value < now && plr.PermitLicenseValidTo.Value > now
                    && plr.IsActive
                    && ship.ShipUid == shipUid
                select new
                {
                    plr.Id,
                    PersonId = plr.SubmittedForPersonId,
                    LegalId = plr.SubmittedForLegalId,
                    CaptainId = plr.QualifiedFisherId,
                }
            ).ToList();

            var captains = (from captain in Db.FishermenRegisters
                            where permits.Select(f => f.CaptainId).Contains(captain.Id)
                            select new
                            {
                                captain.PersonId,
                                CaptainId = captain.Id
                            }).ToList();

            HashSet<int> personIds = owners.Where(f => f.PersonId.HasValue).Select(f => f.PersonId.Value)
                .Concat(permits.Where(f => f.PersonId.HasValue).Select(f => f.PersonId.Value))
                .Concat(captains.Select(f => f.PersonId))
                .ToHashSet();

            HashSet<int> legalIds = owners.Where(f => f.LegalId.HasValue).Select(f => f.LegalId.Value)
                .Concat(permits.Where(f => f.LegalId.HasValue).Select(f => f.LegalId.Value))
                .ToHashSet();

            var persons = (
                from person in Db.Persons
                join address in
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
                    } on person.Id equals address.Id into agrp
                from address in agrp.DefaultIfEmpty()
                where personIds.Contains(person.Id)
                select new
                {
                    person.Id,
                    person.FirstName,
                    person.MiddleName,
                    person.LastName,
                    CountryId = person.CitizenshipCountryId,
                    EgnLnc = new EgnLncDTO
                    {
                        EgnLnc = person.EgnLnc,
                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                    },
                    Address = address ?? null,
                }).ToList();

            var legals = (
                from legal in Db.Legals
                join address in
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
                    } on legal.Id equals address.Id into agrp
                from address in agrp.DefaultIfEmpty()
                where legalIds.Contains(legal.Id)
                select new
                {
                    legal.Id,
                    legal.Name,
                    legal.Eik,
                    Address = address ?? null,
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
                }).ToList();

            personnel.AddRange((from owner in owners
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
                                }).ToList());

            personnel.AddRange((from permit in permits
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
                                }).ToList());

            personnel.AddRange((from permit in permits
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
                                }).ToList());

            personnel.AddRange((from permit in permits
                                join captain in captains on permit.CaptainId equals captain.CaptainId
                                join person in persons on captain.PersonId equals person.Id
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
                                }).ToList());

            return personnel
                .GroupBy(f => new { f.Code, f.Type })
                .Select(f => f.OrderByDescending(f => f.Value).First())
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
                    LineCount = fishingGear.LineCount,
                    NetNominalLength = fishingGear.NetNominalLength,
                    NetsInFleetCount = fishingGear.NumberOfNetsInFleet,
                    TrawlModel = fishingGear.TrawlModel,
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
                    && !plr.IsSuspended
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
                }).ToList();

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
                }).ToList();

            return result;
        }

        public List<InspectionPermitLicenseDTO> GetShipPermits(int shipId)
        {
            DateTime now = DateTime.Now;

            int shipUid = Db.ShipsRegister.First(f => f.Id == shipId).ShipUid;

            List<InspectionPermitLicenseDTO> result = (
                from pr in Db.CommercialFishingPermitRegisters
                join prType in Db.NcommercialFishingPermitTypes on pr.PermitTypeId equals prType.Id
                join ship in Db.ShipsRegister on pr.ShipId equals ship.Id
                where pr.RecordType == nameof(RecordTypesEnum.Register)
                    && ((pr.PermitValidFrom.Value < now && pr.PermitValidTo.Value > now) || pr.IsPermitUnlimited.Value)
                    && pr.IsActive
                    && !pr.IsSuspended
                    && ship.ShipUid == shipUid
                select new InspectionPermitLicenseDTO
                {
                    Id = pr.Id,
                    TypeId = prType.Id,
                    TypeName = prType.Name,
                    PermitNumber = pr.RegistrationNum,
                    ValidFrom = pr.PermitValidFrom,
                    ValidTo = pr.PermitValidTo,
                }).ToList();

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
                    && logBookStatus.Code != nameof(LogBookStatusesEnum.SuspLic)
                    && logBook.ShipId != null
                    && shipIds.Contains(logBook.ShipId.Value)
                select new InspectionShipLogBookDTO
                {
                    Id = logBook.Id,
                    Number = logBook.LogNum,
                    IssuedOn = logBook.IssueDate,
                    StartPage = logBook.StartPageNum,
                    EndPage = logBook.EndPageNum,
                }).ToList();

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
                }).ToList();

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
                join legalAddress in
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
                    } on buyer.SubmittedForLegalId equals legalAddress.Id into agrp
                from legalAddress in agrp.DefaultIfEmpty()
                join personAddress in
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
                    } on buyer.SubmittedForPersonId equals personAddress.Id into pagrp
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
                }).ToList();

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
                    DisplayName = (aqua.UrorNum != null ? (aqua.UrorNum + " - ") : "") + aqua.Name + (aqua.RegNum != null ? " (" + aqua.RegNum + ")" : ""),
                }).ToList();

            return result;
        }

        public List<DeclarationLogBookPageDTO> GetDeclarationLogBookPages(DeclarationLogBookTypeEnum type, int? shipId, int? aquacultureId)
        {
            HashSet<int> shipLogBookIds = null;

            if (type != DeclarationLogBookTypeEnum.AquacultureLogBook)
            {
                if (shipId == null)
                {
                    return new List<DeclarationLogBookPageDTO>();
                }

                int shipUid = this.Db.ShipsRegister
                    .Where(f => f.Id == shipId)
                    .Select(f => f.ShipUid)
                    .First();

                shipLogBookIds = (
                    from ship in this.Db.ShipsRegister
                    join logBook in this.Db.LogBooks on ship.Id equals logBook.ShipId
                    where ship.ShipUid == shipUid
                    select logBook.Id
                ).ToHashSet();
            }

            List<DeclarationLogBookPageDTO> result = null;

            switch (type)
            {
                case DeclarationLogBookTypeEnum.FirstSaleLogBook:
                    {
                        result = (
                            from fslbp in this.Db.FirstSaleLogBookPages
                            where shipLogBookIds.Contains(fslbp.LogBookId)
                                && fslbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                            select new DeclarationLogBookPageDTO
                            {
                                Id = fslbp.Id,
                                Num = fslbp.PageNum.ToString(),
                                Date = fslbp.SaleDate.Value,
                            }).ToList();

                        List<DeclarationLogBookPageFishDTO> fishes = (
                            from fish in Db.LogBookPageProducts
                            where result.Select(f => f.Id).Contains(fish.FirstSaleLogBookPageId.Value)
                            select new DeclarationLogBookPageFishDTO
                            {
                                Id = fish.Id,
                                LogBookId = fish.FirstSaleLogBookPageId.Value,
                                FishId = fish.FishId,
                                PresentationId = fish.ProductPresentationId,
                                Quantity = fish.QuantityKg,
                            }
                        ).ToList();

                        foreach (DeclarationLogBookPageDTO logBook in result)
                        {
                            logBook.Fishes = fishes.FindAll(f => f.LogBookId == logBook.Id);
                        }
                    }
                    break;
                case DeclarationLogBookTypeEnum.TransportationLogBook:
                    {
                        result = (
                            from tlbp in this.Db.TransportationLogBookPages
                            where shipLogBookIds.Contains(tlbp.LogBookId)
                                && tlbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                            select new DeclarationLogBookPageDTO
                            {
                                Id = tlbp.Id,
                                Num = tlbp.PageNum.ToString(),
                                Date = tlbp.LoadingDate.Value,
                            }).ToList();

                        List<DeclarationLogBookPageFishDTO> fishes = (
                            from fish in Db.LogBookPageProducts
                            where result.Select(f => f.Id).Contains(fish.TransportationLogBookPageId.Value)
                            select new DeclarationLogBookPageFishDTO
                            {
                                Id = fish.Id,
                                LogBookId = fish.TransportationLogBookPageId.Value,
                                FishId = fish.FishId,
                                PresentationId = fish.ProductPresentationId,
                                Quantity = fish.QuantityKg,
                            }
                        ).ToList();

                        foreach (DeclarationLogBookPageDTO logBook in result)
                        {
                            logBook.Fishes = fishes.FindAll(f => f.LogBookId == logBook.Id);
                        }
                    }
                    break;
                case DeclarationLogBookTypeEnum.AdmissionLogBook:
                    {
                        result = (
                            from albp in this.Db.AdmissionLogBookPages
                            where shipLogBookIds.Contains(albp.LogBookId)
                                && albp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                            select new DeclarationLogBookPageDTO
                            {
                                Id = albp.Id,
                                Num = albp.PageNum.ToString(),
                                Date = albp.HandoverDate.Value,
                            }).ToList();

                        List<DeclarationLogBookPageFishDTO> fishes = (
                            from fish in Db.LogBookPageProducts
                            where result.Select(f => f.Id).Contains(fish.AdmissionLogBookPageId.Value)
                            select new DeclarationLogBookPageFishDTO
                            {
                                Id = fish.Id,
                                LogBookId = fish.AdmissionLogBookPageId.Value,
                                FishId = fish.FishId,
                                PresentationId = fish.ProductPresentationId,
                                Quantity = fish.QuantityKg,
                            }
                        ).ToList();

                        foreach (DeclarationLogBookPageDTO logBook in result)
                        {
                            logBook.Fishes = fishes.FindAll(f => f.LogBookId == logBook.Id);
                        }
                    }
                    break;
                case DeclarationLogBookTypeEnum.ShipLogBook:
                    {
                        result = (
                            from slbp in this.Db.ShipLogBookPages
                            where shipLogBookIds.Contains(slbp.LogBookId)
                                && slbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                            select new DeclarationLogBookPageDTO
                            {
                                Id = slbp.Id,
                                Num = slbp.PageNum,
                                Date = slbp.PageFillDate.Value,
                            }).ToList();

                        List<DeclarationLogBookPageFishDTO> fishes = (
                            from declaration in Db.OriginDeclarations
                            join fish in Db.OriginDeclarationFish on declaration.Id equals fish.OriginDeclarationId
                            where result.Select(f => f.Id).Contains(declaration.LogBookPageId)
                            select new DeclarationLogBookPageFishDTO
                            {
                                Id = fish.Id,
                                LogBookId = declaration.LogBookPageId,
                                FishId = fish.FishId,
                                PresentationId = fish.CatchFishPresentationId,
                                Quantity = fish.Quantity,
                            }
                        ).ToList();

                        foreach (DeclarationLogBookPageDTO logBook in result)
                        {
                            logBook.Fishes = fishes.FindAll(f => f.LogBookId == logBook.Id);
                        }
                    }
                    break;
                case DeclarationLogBookTypeEnum.AquacultureLogBook:
                    {
                        if (aquacultureId == null)
                        {
                            return new List<DeclarationLogBookPageDTO>();
                        }

                        HashSet<int> aquacultureLogBookIds = (
                            from logBook in Db.LogBooks
                            where logBook.AquacultureFacilityId == aquacultureId.Value
                            select logBook.Id
                        ).ToHashSet();

                        result = (
                            from albp in this.Db.AquacultureLogBookPages
                            where aquacultureLogBookIds.Contains(albp.LogBookId)
                                && albp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                            select new DeclarationLogBookPageDTO
                            {
                                Id = albp.Id,
                                Num = albp.PageNum.ToString(),
                                Date = albp.FillingDate.Value,
                            }).ToList();

                        List<DeclarationLogBookPageFishDTO> fishes = (
                            from fish in Db.LogBookPageProducts
                            where result.Select(f => f.Id).Contains(fish.AquacultureLogBookPageId.Value)
                            select new DeclarationLogBookPageFishDTO
                            {
                                Id = fish.Id,
                                LogBookId = fish.AquacultureLogBookPageId.Value,
                                FishId = fish.FishId,
                                PresentationId = fish.ProductPresentationId,
                                Quantity = fish.QuantityKg,
                            }
                        ).ToList();

                        foreach (DeclarationLogBookPageDTO logBook in result)
                        {
                            logBook.Fishes = fishes.FindAll(f => f.LogBookId == logBook.Id);
                        }
                    }
                    break;
                default:
                    {
                        throw new Exception($"{nameof(DeclarationLogBookTypeEnum)} provided to " +
                            $"{nameof(GetDeclarationLogBookPages)} inside " +
                            $"{nameof(CommonInspectionService)} was of wrong type.");
                    }
            }

            return result;
        }

        public InspectionSubjectPersonnelDTO GetAquacultureOwner(int aquacultureId)
        {
            InspectionSubjectPersonnelDTO result = (
                from aqua in Db.AquacultureFacilitiesRegister
                join owner in Db.Legals on aqua.SubmittedForLegalId equals owner.Id
                join address in
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
                    } on owner.Id equals address.Id into agrp
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
                }).First();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.InspectionsRegister, id);
        }

        private IQueryable<InspectionDTO> GetAllNoFilter(bool isActive, int? userId)
        {
            IQueryable<InspectionDTO> result = (
                from inspection in Db.InspectionsRegister
                join inspectionType in Db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                join inspectionState in Db.NinspectionStates on inspection.StateId equals inspectionState.Id
                where inspection.IsActive == isActive
                    && (!userId.HasValue || inspection.CreatedByUserId == userId)
                    && inspectionType.Code != nameof(InspectionTypesEnum.EXT)
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
                where (inspectionIds == null || inspectionIds.Contains(inspection.Id))
                   && inspectionType.Code != nameof(InspectionTypesEnum.EXT)
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
                List<string> shipInspectionTypes = new()
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
                where (inspectionIds == null || inspectionIds.Contains(inspection.Id))
                   && inspectionType.Code != nameof(InspectionTypesEnum.EXT)
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
    }
}
