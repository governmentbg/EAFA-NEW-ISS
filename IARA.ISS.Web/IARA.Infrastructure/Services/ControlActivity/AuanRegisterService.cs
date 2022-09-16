using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.ControlActivity;
using IARA.Interfaces.Legals;
using IARA.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.ControlActivity
{
    public class AuanRegisterService : Service, IAuanRegisterService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        private readonly IAddressService addressService;
        private readonly IJasperReportExecutionService jasperReportsService;

        public AuanRegisterService(IARADbContext db,
                                   IPersonService personService,
                                   ILegalService legalService,
                                   IAddressService addressService,
                                   IJasperReportExecutionService jasperReportsService)
            : base(db)
        {
            this.personService = personService;
            this.legalService = legalService;
            this.addressService = addressService;
            this.jasperReportsService = jasperReportsService;
        }

        public IQueryable<AuanRegisterDTO> GetAllAuans(AuanRegisterFilters filters)
        {
            IQueryable<AuanRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = this.GetAllAuans(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? this.GetParametersFilteredAuans(filters)
                    : this.GetFreeTextFilteredAuans(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public AuanRegisterEditDTO GetAuan(int id)
        {
            var query = (from auan in this.Db.AuanRegister
                         where auan.Id == id
                         select new
                         {
                             auan.Id,
                             auan.InspectionId,
                             auan.AuanNum,
                             auan.DraftDate,
                             auan.LocationDescription,
                             auan.InspectedPersonId,
                             auan.InspectedLegalId,
                             auan.InspectedPersonWorkPlace,
                             auan.InspectedPersonWorkPosition,
                             auan.ConstatationComments,
                             auan.OffenderComments,
                             auan.HasObjection,
                             auan.ObjectionDate,
                             auan.ResolutionType,
                             auan.ResolutionNum,
                             auan.ResolutionDate,
                             auan.DeliveryId,
                             auan.StatusId
                         }).First();

            AuanRegisterEditDTO result = new AuanRegisterEditDTO
            {
                Id = query.Id,
                InspectionId = query.InspectionId,
                AuanNum = query.AuanNum,
                DraftDate = query.DraftDate,
                LocationDescription = query.LocationDescription,
                ConstatationComments = query.ConstatationComments,
                OffenderComments = query.OffenderComments,
                HasObjection = query.HasObjection,
                ObjectionDate = query.ObjectionDate,
                ResolutionType = !string.IsNullOrEmpty(query.ResolutionType)
                    ? Enum.Parse<AuanObjectionResolutionTypesEnum>(query.ResolutionType)
                    : default,
                ResolutionNum = query.ResolutionNum,
                ResolutionDate = query.ResolutionDate,
                StatusId = query.StatusId
            };

            if (query.InspectedPersonId.HasValue)
            {
                result.InspectedEntity = new AuanInspectedEntityDTO
                {
                    IsUnregisteredPerson = false,
                    IsPerson = true,
                    PersonWorkPlace = query.InspectedPersonWorkPlace,
                    PersonWorkPosition = query.InspectedPersonWorkPosition
                };

                result.InspectedEntity.Person = this.personService.GetRegixPersonData(query.InspectedPersonId.Value);
                result.InspectedEntity.Addresses = this.personService.GetAddressRegistrations(query.InspectedPersonId.Value);
            }
            else if (query.InspectedLegalId.HasValue)
            {
                result.InspectedEntity = new AuanInspectedEntityDTO
                {
                    IsUnregisteredPerson = false,
                    IsPerson = false
                };

                result.InspectedEntity.Legal = this.legalService.GetRegixLegalData(query.InspectedLegalId.Value);
                result.InspectedEntity.Addresses = this.legalService.GetAddressRegistrations(query.InspectedLegalId.Value);
            }

            result.AuanWitnesses = this.GetWitnesses(result.Id);
            result.ViolatedRegulations = this.GetViolatedRegulations(result.Id.Value);
            result.ConfiscatedFish = this.GetConfiscatedFish(result.Id.Value);
            result.ConfiscatedAppliance = this.GetConfiscatedAppliance(result.Id.Value);
            result.ConfiscatedFishingGear = this.GetConfiscatedFishingGear(result.Id.Value);
            result.DeliveryData = this.GetDeliveryData(query.DeliveryId);
            result.Files = this.Db.GetFiles(this.Db.AuanRegisterFiles, result.Id.Value);

            return result;
        }

        public int AddAuan(AuanRegisterEditDTO auan)
        {
            using TransactionScope scope = new TransactionScope();

            Auanregister entry = new Auanregister
            {
                InspectionId = auan.InspectionId.Value,
                AuanNum = auan.AuanNum,
                DraftDate = auan.DraftDate.Value,
                LocationDescription = auan.LocationDescription,
                ConstatationComments = auan.ConstatationComments,
                OffenderComments = auan.OffenderComments,
                StatusId = auan.StatusId
            };

            if (auan.InspectedEntity.IsPerson == true)
            {
                entry.InspectedPerson = this.Db.AddOrEditPerson(auan.InspectedEntity.Person, auan.InspectedEntity.Addresses);
                entry.InspectedPersonWorkPlace = auan.InspectedEntity.PersonWorkPlace;
                entry.InspectedPersonWorkPosition = auan.InspectedEntity.PersonWorkPosition;

                this.Db.SaveChanges();
            }
            else if (auan.InspectedEntity.IsPerson == false)
            {
                entry.InspectedLegal = this.Db.AddOrEditLegal(new ApplicationRegisterDataDTO
                {
                    RecordType = RecordTypesEnum.Register
                }, auan.InspectedEntity.Legal, auan.InspectedEntity.Addresses);

                this.Db.SaveChanges();
            }

            this.Db.AuanRegister.Add(entry);

            this.AddViolatedRegulations(entry, auan.ViolatedRegulations);
            this.AddWitnesses(entry, auan.AuanWitnesses);
            this.AddConfiscatedFish(entry, auan.ConfiscatedFish);
            this.AddConfiscatedFish(entry, auan.ConfiscatedAppliance);
            this.AddConfiscatedFishingGear(entry, auan.ConfiscatedFishingGear);
            this.AddDeliveryData(entry, auan.DeliveryData);

            if (auan.HasObjection == true)
            {
                entry.HasObjection = true;
                entry.ObjectionDate = auan.ObjectionDate.Value;
                entry.ResolutionType = auan.ResolutionType.ToString();
                entry.ResolutionNum = auan.ResolutionNum;
                entry.ResolutionDate = auan.ResolutionDate.Value;
            }
            else
            {
                entry.HasObjection = false;
                entry.ObjectionDate = null;
                entry.ResolutionType = null;
                entry.ResolutionNum = null;
                entry.ResolutionDate = null;
            }

            if (auan.Files != null)
            {
                foreach (FileInfoDTO file in auan.Files)
                {
                    this.Db.AddOrEditFile(entry, entry.AuanregisterFiles, file);
                }
            }

            this.Db.SaveChanges();

            scope.Complete();
            return entry.Id;
        }

        public void EditAuan(AuanRegisterEditDTO auan)
        {
            using TransactionScope scope = new TransactionScope();

            Auanregister entry = (from au in this.Db.AuanRegister
                                    .Include(x => x.AuanregisterFiles)
                                  where au.Id == auan.Id.Value
                                  select au).First();

            entry.AuanNum = auan.AuanNum;
            entry.DraftDate = auan.DraftDate.Value;
            entry.LocationDescription = auan.LocationDescription;
            entry.ConstatationComments = auan.ConstatationComments;
            entry.OffenderComments = auan.OffenderComments;
            entry.StatusId = auan.StatusId;

            if (auan.InspectedEntity.IsPerson == true)
            {
                entry.InspectedLegalId = null;

                entry.InspectedPerson = this.Db.AddOrEditPerson(auan.InspectedEntity.Person, auan.InspectedEntity.Addresses, entry.InspectedPersonId);
                entry.InspectedPersonWorkPlace = auan.InspectedEntity.PersonWorkPlace;
                entry.InspectedPersonWorkPosition = auan.InspectedEntity.PersonWorkPosition;

                this.Db.SaveChanges();
            }
            else if (auan.InspectedEntity.IsPerson == false)
            {
                entry.InspectedPersonId = null;
                entry.InspectedPersonWorkPlace = null;
                entry.InspectedPersonWorkPosition = null;

                entry.InspectedLegal = this.Db.AddOrEditLegal(new ApplicationRegisterDataDTO
                {
                    RecordType = RecordTypesEnum.Register
                }, auan.InspectedEntity.Legal, auan.InspectedEntity.Addresses, entry.InspectedLegalId);

                this.Db.SaveChanges();
            }

            this.EditViolatedRegulations(entry, auan.ViolatedRegulations);
            this.EditAuanWitnesses(entry, auan.AuanWitnesses);
            this.EditConfiscatedFish(entry, auan.ConfiscatedFish);
            this.EditConfiscatedFish(entry, auan.ConfiscatedAppliance);
            this.EditConfiscatedFishingGear(entry, auan.ConfiscatedFishingGear);
            this.EditDeliveryData(auan.DeliveryData);

            if (auan.HasObjection.Value)
            {
                entry.HasObjection = true;
                entry.ObjectionDate = auan.ObjectionDate.Value;
                entry.ResolutionType = auan.ResolutionType.ToString();
                entry.ResolutionNum = auan.ResolutionNum;
                entry.ResolutionDate = auan.ResolutionDate.Value;
            }
            else
            {
                entry.HasObjection = false;
                entry.ObjectionDate = null;
                entry.ResolutionType = null;
                entry.ResolutionNum = null;
                entry.ResolutionDate = null;
            }

            if (auan.Files != null)
            {
                foreach (FileInfoDTO file in auan.Files)
                {
                    this.Db.AddOrEditFile(entry, entry.AuanregisterFiles, file);
                }
            }

            this.Db.SaveChanges();
            scope.Complete();
        }

        public void DeleteAuan(int id)
        {
            this.DeleteRecordWithId(this.Db.AuanRegister, id);
            this.Db.SaveChanges();
        }

        public void UndoDeleteAuan(int id)
        {
            this.UndoDeleteRecordWithId(this.Db.AuanRegister, id);
            this.Db.SaveChanges();
        }

        public AuanReportDataDTO GetAuanReportDataFromInspection(int inspectionId)
        {
            AuanReportDataDTO data = (from inspection in this.Db.InspectionsRegister
                                      where inspection.Id == inspectionId
                                      select new AuanReportDataDTO
                                      {
                                          ReportNum = inspection.ReportNum,
                                          InspectionTypeId = inspection.InspectionTypeId,
                                          TerritoryUnitId = inspection.TerritoryUnitId
                                      }).First();

            data.Drafter = this.GetAuanInspectionDrafter(inspectionId);
            data.InspectedEntities = this.GetAuanInspectedEntitiesFromInspection(inspectionId);

            return data;
        }

        public List<NomenclatureDTO> GetAllDrafters()
        {
            List<int> inspectionIds = (from auan in this.Db.AuanRegister
                                       select auan.InspectionId).ToList();

            Dictionary<int, string> drafters = this.GetAuanInspectionsDrafters(inspectionIds);

            List<NomenclatureDTO> result = (from drafter in drafters
                                            select new NomenclatureDTO
                                            {
                                                Value = drafter.Key,
                                                DisplayName = drafter.Value,
                                                IsActive = true
                                            }).ToList();

            return result;
        }

        public List<AuanWitnessDTO> GetWitnesses(int? auanId = null, int? deliveryId = null)
        {
            List<AuanWitnessDTO> witnesses = (from witness in Db.Auanwitnesses
                                              where (auanId != null && witness.AuanregisterId == auanId)
                                                || (deliveryId != null && witness.InspDeliveryId == deliveryId)
                                                && witness.IsActive == true
                                              orderby witness.OrderNum
                                              select new AuanWitnessDTO
                                              {
                                                  Id = witness.Id,
                                                  WitnessNames = witness.WitnessNames,
                                                  DateOfBirth = witness.DateOfBirth,
                                                  OrderNum = witness.OrderNum,
                                                  AddressId = witness.AddressId,
                                                  IsActive = true
                                              }).ToList();

            foreach (AuanWitnessDTO witness in witnesses)
            {
                witness.Address = addressService.GetAddressRegistration(witness.AddressId.Value);
            }

            return witnesses;
        }

        public Task<byte[]> DownloadAuan(int auanId)
        {
            //DownloadableFileDTO downloadableFile = new DownloadableFileDTO();
            //downloadableFile.MimeType = "application/pdf";
            //downloadableFile.FileName = $"АУАН.pdf".Replace("  ", "");
            //downloadableFile.Bytes = await jasperReportsService.GetPenalDecreesRegister(auanId);
            return jasperReportsService.GetAuanRegister(auanId);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = this.GetSimpleEntityAuditValues(this.Db.AuanRegister, id);
            return audit;
        }

        private IQueryable<AuanRegisterDTO> GetAllAuans(bool showInactive)
        {
            IQueryable<AuanRegisterDTO> result = from auan in this.Db.AuanRegister
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
                                                 where auan.IsActive == !showInactive
                                                  && (inspInsp == null || inspInsp.IsInCharge)
                                                 orderby auan.DraftDate descending, auan.Id
                                                 select new AuanRegisterDTO
                                                 {
                                                     Id = auan.Id,
                                                     InspectionId = auan.InspectionId,
                                                     AuanNum = auan.AuanNum,
                                                     InspectedEntity = inspPerson != null ? inspPerson.FirstName + " " + inspPerson.LastName : inspLegal.Name,
                                                     Drafter = unregPerson != null
                                                               ? unregPerson.FirstName + " " + unregPerson.LastName
                                                               : person.FirstName + " " + person.LastName,
                                                     DraftDate = auan.DraftDate,
                                                     IsActive = auan.IsActive
                                                 };

            return result;
        }

        private IQueryable<AuanRegisterDTO> GetParametersFilteredAuans(AuanRegisterFilters filters)
        {
            var query = from auan in this.Db.AuanRegister
                        join inspPerson in this.Db.Persons on auan.InspectedPersonId equals inspPerson.Id into inspPer
                        from inspPerson in inspPer.DefaultIfEmpty()
                        join inspLegal in this.Db.Legals on auan.InspectedLegalId equals inspLegal.Id into inspLeg
                        from inspLegal in inspLeg.DefaultIfEmpty()
                        join inspect in this.Db.InspectionsRegister on auan.InspectionId equals inspect.Id
                        join inspInsp in this.Db.InspectionInspectors on auan.InspectionId equals inspInsp.InspectionId into inspection
                        from inspInsp in inspection.DefaultIfEmpty()
                        join inspector in this.Db.Inspectors on inspInsp.InspectorId equals inspector.Id into insp
                        from inspector in insp.DefaultIfEmpty()
                        join inspType in this.Db.NinspectionTypes on inspect.InspectionTypeId equals inspType.Id
                        join unregPerson in this.Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPer
                        from unregPerson in unregPer.DefaultIfEmpty()
                        join user in this.Db.Users on inspector.UserId equals user.Id into us
                        from user in us.DefaultIfEmpty()
                        join person in this.Db.Persons on user.PersonId equals person.Id into per
                        from person in per.DefaultIfEmpty()
                        join createdByUser in this.Db.Users on inspect.CreatedByUserId equals createdByUser.Id
                        join createdByUserInfo in this.Db.UserInfos on createdByUser.Id equals createdByUserInfo.UserId
                        where auan.IsActive == !filters.ShowInactiveRecords
                          && (inspInsp == null || inspInsp.IsInCharge)
                        select new
                        {
                            auan.Id,
                            auan.InspectionId,
                            auan.AuanNum,
                            auan.LocationDescription,
                            inspect.TerritoryUnitId,
                            InspectionTypeId = inspType.Id,
                            InspectedEntity = inspPerson != null ? inspPerson.FirstName + " " + inspPerson.LastName : inspLegal.Name,
                            InspectedPersonFirstName = inspPerson.FirstName,
                            InspectedPersonMiddleName = inspPerson.MiddleName,
                            InspectedPersonLastName = inspPerson.LastName,
                            InspectedPersonIdentifier = inspPerson != null ? inspPerson.EgnLnc : inspLegal.Eik,
                            DrafterId = inspector.Id,
                            Drafter = unregPerson != null
                                      ? unregPerson.FirstName + " " + unregPerson.LastName
                                      : person.FirstName + " " + person.LastName,
                            auan.DraftDate,
                            InspectedEntityPersonId = inspPerson.Id,
                            InspectedEntityLegalId = inspLegal.Id,
                            DeliveryId = auan.DeliveryId,
                            auan.IsActive
                        };

            if (!string.IsNullOrEmpty(filters.AuanNum))
            {
                query = query.Where(x => x.AuanNum.ToLower().Contains(filters.AuanNum.ToLower()));
            }

            if (filters.DrafterId.HasValue)
            {
                query = query.Where(x => x.DrafterId == filters.DrafterId.Value);
            }

            if (!string.IsNullOrEmpty(filters.DrafterName))
            {
                query = query.Where(x => x.Drafter.ToLower().Contains(filters.DrafterName.ToLower()));
            }

            if (filters.TerritoryUnitId.HasValue)
            {
                query = query.Where(x => x.TerritoryUnitId == filters.TerritoryUnitId.Value);
            }

            if (filters.DraftDateFrom.HasValue)
            {
                query = query.Where(x => x.DraftDate >= filters.DraftDateFrom.Value);
            }

            if (filters.DraftDateTo.HasValue)
            {
                query = query.Where(x => x.DraftDate <= filters.DraftDateTo.Value);
            }

            if (filters.InspectionTypeId.HasValue)
            {
                query = query.Where(x => x.InspectionTypeId == filters.InspectionTypeId.Value);
            }

            if (!string.IsNullOrEmpty(filters.LocationDescription))
            {
                query = query.Where(x => x.LocationDescription.ToLower().Contains(filters.LocationDescription.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.InspectedEntityFirstName))
            {
                query = query.Where(x => x.InspectedPersonFirstName.ToLower().Contains(filters.InspectedEntityFirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.InspectedEntityMiddleName))
            {
                query = query.Where(x => x.InspectedPersonMiddleName.ToLower().Contains(filters.InspectedEntityMiddleName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.InspectedEntityLastName))
            {
                query = query.Where(x => x.InspectedPersonLastName.ToLower().Contains(filters.InspectedEntityLastName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Identifier))
            {
                query = query.Where(x => x.InspectedPersonIdentifier.ToLower().Contains(filters.Identifier.ToLower()));
            }

            if (filters.FishId.HasValue)
            {
                query = from auan in query
                        join fish in Db.AuanConfiscatedFishes on auan.Id equals fish.AuanregisterId
                        where filters.FishId == fish.FishId
                            && fish.IsActive
                        select auan;
            }

            if (filters.FishingGearId.HasValue)
            {
                query = from auan in query
                        join gear in Db.AuanConfiscatedFishingGear on auan.Id equals gear.AuanregisterId
                        where filters.FishingGearId == gear.FishingGearId
                            && gear.IsActive
                        select auan;
            }

            if (filters.ApplianceId.HasValue)
            {
                query = from auan in query
                        join fish in Db.AuanConfiscatedFishes on auan.Id equals fish.AuanregisterId
                        where filters.ApplianceId == fish.ApplicanceId
                            && fish.IsActive
                        select auan;
            }

            if (filters.IsDelivered.HasValue)
            {
                query = from auan in query
                        join delivery in Db.InspDelivery on auan.DeliveryId equals delivery.Id
                        where filters.IsDelivered == delivery.IsDelivered
                            && delivery.IsActive
                        select auan;
            }

            if (filters.PersonId.HasValue)
            {
                query = from auan in query
                        where auan.DrafterId == filters.PersonId
                            || auan.InspectedEntityPersonId == filters.PersonId
                        select auan;
            }

            if (filters.LegalId.HasValue)
            {
                query = from auan in query
                        where auan.InspectedEntityLegalId == filters.LegalId
                        select auan;
            }

            IQueryable<AuanRegisterDTO> result = from auan in query
                                                 orderby auan.DraftDate descending, auan.Id
                                                 select new AuanRegisterDTO
                                                 {
                                                     Id = auan.Id,
                                                     InspectionId = auan.InspectionId,
                                                     AuanNum = auan.AuanNum,
                                                     InspectedEntity = auan.InspectedEntity,
                                                     Drafter = auan.Drafter,
                                                     DraftDate = auan.DraftDate,
                                                     IsActive = auan.IsActive
                                                 };

            return result;
        }

        private IQueryable<AuanRegisterDTO> GetFreeTextFilteredAuans(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<AuanRegisterDTO> result = from auan in this.Db.AuanRegister
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
                                                 where auan.IsActive == !showInactive
                                                    && (inspInsp == null || inspInsp.IsInCharge)
                                                    && (auan.AuanNum.ToLower().Contains(text)
                                                        || (unregPerson != null ? unregPerson.FirstName + " " + unregPerson.LastName : person.FirstName + " " + person.LastName).ToLower().Contains(text)
                                                        || (inspPerson != null ? inspPerson.FirstName + " " + inspPerson.LastName : inspLegal.Name).ToLower().Contains(text)
                                                        || (searchDate.HasValue && auan.DraftDate == searchDate.Value))
                                                 orderby auan.DraftDate descending, auan.Id
                                                 select new AuanRegisterDTO
                                                 {
                                                     Id = auan.Id,
                                                     InspectionId = auan.InspectionId,
                                                     AuanNum = auan.AuanNum,
                                                     InspectedEntity = inspPerson != null ? inspPerson.FirstName + " " + inspPerson.LastName : inspLegal.Name,
                                                     Drafter = unregPerson != null ? unregPerson.FirstName + " " + unregPerson.LastName : person.FirstName + " " + person.LastName,
                                                     DraftDate = auan.DraftDate,
                                                     IsActive = auan.IsActive
                                                 };

            return result;
        }

        private string GetAuanInspectionDrafter(int inspectionId)
        {
            var inspectionDrafter = this.GetAuanInspectionsDrafters(new List<int> { inspectionId }).SingleOrDefault();
            string drafter = inspectionDrafter.Value;
            return drafter;
        }

        private Dictionary<int, string> GetAuanInspectionsDrafters(List<int> inspectionIds)
        {
            Dictionary<int, string> drafters = (from inspInsp in this.Db.InspectionInspectors
                                                join inspector in this.Db.Inspectors on inspInsp.InspectorId equals inspector.Id
                                                join unregPerson in this.Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPer
                                                from unregPerson in unregPer.DefaultIfEmpty()
                                                join user in this.Db.Users on inspector.UserId equals user.Id into us
                                                from user in us.DefaultIfEmpty()
                                                join person in this.Db.Persons on user.PersonId equals person.Id into per
                                                from person in per.DefaultIfEmpty()
                                                where inspectionIds.Contains(inspInsp.InspectionId)
                                                    && inspInsp.IsInCharge
                                                select new
                                                {
                                                    InspectorId = inspector.Id,
                                                    Drafter = unregPerson != null ? $"{ unregPerson.FirstName} {unregPerson.LastName}" : $"{person.FirstName} {person.LastName}"
                                                })
                                                .Distinct()
                                                .ToDictionary(x => x.InspectorId, y => y.Drafter); //??????

            return drafters;
        }

        private List<AuanInspectedEntityDTO> GetAuanInspectedEntitiesFromInspection(int inspectionId)
        {
            List<AuanInspectedEntityHelper> entities = (from inspectedPerson in this.Db.InspectedPersons
                                                        join person in this.Db.Persons on inspectedPerson.PersonId equals person.Id into per
                                                        from person in per.DefaultIfEmpty()
                                                        join legal in this.Db.Legals on inspectedPerson.LegalId equals legal.Id into leg
                                                        from legal in leg.DefaultIfEmpty()
                                                        join buyer in this.Db.BuyerRegisters on inspectedPerson.BuyerId equals buyer.Id into buy
                                                        from buyer in buy.DefaultIfEmpty()
                                                        join fisherman in this.Db.FishermenRegisters on inspectedPerson.CaptainFishermenId equals fisherman.Id into fish
                                                        from fisherman in fish.DefaultIfEmpty()
                                                        join unregPerson in this.Db.UnregisteredPersons on inspectedPerson.UnregisteredPersonId equals unregPerson.Id into unr
                                                        from unregPerson in unr.DefaultIfEmpty()
                                                        where inspectedPerson.InspectionId == inspectionId
                                                        select new AuanInspectedEntityHelper
                                                        {
                                                            UnregisteredPerson = unregPerson != null
                                                                ? new UnregisteredPersonDTO
                                                                {
                                                                    Id = unregPerson.Id,
                                                                    EgnLnc = new EgnLncDTO
                                                                    {
                                                                        EgnLnc = unregPerson.EgnLnc,
                                                                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(unregPerson.IdentifierType),
                                                                    },
                                                                    FirstName = unregPerson.FirstName,
                                                                    LastName = unregPerson.LastName,
                                                                    CitizenshipId = unregPerson.CitizenshipCountryId,
                                                                    HasBulgarianAddressRegistration = unregPerson.HasBulgarianAddressRegistration,
                                                                    Address = unregPerson.Address,
                                                                    Eik = unregPerson.EgnLnc,
                                                                    Comment = unregPerson.Comments
                                                                }
                                                                : null,
                                                            PersonId = person != null
                                                                ? person.Id
                                                                : fisherman != null
                                                                    ? fisherman.PersonId
                                                                    : default,
                                                            LegalId = legal != null
                                                                ? legal.Id
                                                                : buyer != null
                                                                    ? buyer.SubmittedForLegalId
                                                                    : default
                                                        }).ToList();

            List<int> personIds = entities.Where(x => x.UnregisteredPerson == null && x.PersonId.HasValue).Select(x => x.PersonId.Value).ToList();
            List<int> legalIds = entities.Where(x => x.UnregisteredPerson == null && x.LegalId.HasValue).Select(x => x.LegalId.Value).ToList();

            Dictionary<int, RegixPersonDataDTO> persons = this.personService.GetRegixPersonsData(personIds);
            Dictionary<int, RegixLegalDataDTO> legals = this.legalService.GetRegixLegalsData(legalIds);
            ILookup<int, AddressRegistrationDTO> personAddresses = this.personService.GetAddressRegistrations(personIds);
            ILookup<int, AddressRegistrationDTO> legalAddresses = this.legalService.GetAddressRegistrations(legalIds);

            foreach (AuanInspectedEntityHelper entity in entities)
            {
                entity.IsUnregisteredPerson = entity.UnregisteredPerson != null;
                if (!entity.IsUnregisteredPerson.Value)
                {
                    entity.IsPerson = entity.PersonId.HasValue;

                    if (entity.IsPerson.Value)
                    {
                        entity.Person = persons[entity.PersonId.Value];
                        entity.Addresses = personAddresses[entity.PersonId.Value].ToList();
                    }
                    else
                    {
                        entity.Legal = legals[entity.LegalId.Value];
                        entity.Addresses = legalAddresses[entity.LegalId.Value].ToList();
                    }
                }
                else
                {
                    entity.IsPerson = entity.UnregisteredPerson.EgnLnc.IdentifierType != IdentifierTypeEnum.LEGAL; //??
                }
            }

            return entities.Select(x => x as AuanInspectedEntityDTO).ToList();
        }

        private List<AuanViolatedRegulationDTO> GetViolatedRegulations(int auanId)
        {
            List<AuanViolatedRegulationDTO> result = (from reg in this.Db.AuanViolatedRegulations
                                                      where reg.AuanregisterId == auanId
                                                      select new AuanViolatedRegulationDTO
                                                      {
                                                          Id = reg.Id,
                                                          Article = reg.Article,
                                                          Paragraph = reg.Paragraph,
                                                          Section = reg.Section,
                                                          Letter = reg.Letter,
                                                          Type = Enum.Parse<AuanViolatedRegulationTypesEnum>(reg.RegulationType),
                                                          IsActive = reg.IsActive
                                                      }).ToList();

            return result;
        }

        private List<AuanConfiscatedFishDTO> GetConfiscatedFish(int auanId)
        {
            List<AuanConfiscatedFishDTO> result = (from fish in this.Db.AuanConfiscatedFishes
                                                   where fish.AuanregisterId == auanId
                                                        && fish.FishId.HasValue
                                                   select new AuanConfiscatedFishDTO
                                                   {
                                                       Id = fish.Id,
                                                       FishTypeId = fish.FishId,
                                                       Weight = fish.Weight,
                                                       Count = fish.Count,
                                                       ConfiscationActionId = fish.ConfiscationActionId,
                                                       TurbotSizeGroupId = fish.TurbotSizeGroupId,
                                                       Comments = fish.Comments,
                                                       IsActive = fish.IsActive
                                                   }).ToList();

            return result;
        }

        private List<AuanConfiscatedFishDTO> GetConfiscatedAppliance(int auanId)
        {
            List<AuanConfiscatedFishDTO> result = (from appliance in this.Db.AuanConfiscatedFishes
                                                   where appliance.AuanregisterId == auanId
                                                        && appliance.ApplicanceId.HasValue
                                                   select new AuanConfiscatedFishDTO
                                                   {
                                                       Id = appliance.Id,
                                                       ApplianceId = appliance.ApplicanceId,
                                                       Count = appliance.Count,
                                                       ConfiscationActionId = appliance.ConfiscationActionId,
                                                       TurbotSizeGroupId = appliance.TurbotSizeGroupId,
                                                       Comments = appliance.Comments,
                                                       IsActive = appliance.IsActive
                                                   }).ToList();

            return result;
        }

        private List<AuanConfiscatedFishingGearDTO> GetConfiscatedFishingGear(int auanId)
        {
            List<AuanConfiscatedFishingGearDTO> result = (from gear in this.Db.AuanConfiscatedFishingGear
                                                          where gear.AuanregisterId == auanId
                                                          select new AuanConfiscatedFishingGearDTO
                                                          {
                                                              Id = gear.Id,
                                                              FishingGearId = gear.FishingGearId,
                                                              Count = gear.Count,
                                                              Length = gear.Length,
                                                              NetEyeSize = gear.NetEyeSize,
                                                              ConfiscationActionId = gear.ConfiscationActionId,
                                                              Comments = gear.Comments,
                                                              IsActive = gear.IsActive
                                                          }).ToList();

            return result;
        }

        private AuanDeliveryDataDTO GetDeliveryData(int deliveryId)
        {
            var delivery = (from del in this.Db.InspDelivery
                            join type in this.Db.NinspDeliveryTypes on del.DeliveryTypeId equals type.Id
                            where del.Id == deliveryId
                            select new
                            {
                                del.Id,
                                DeliveryType = Enum.Parse<InspDeliveryTypesEnum>(type.Code),
                                del.SentDate,
                                del.ReferenceNum,
                                del.StateService,
                                del.TerritoryUnitId,
                                del.AddressId,
                                del.IsDelivered,
                                del.DeliveryDate,
                                del.RefusalDate,
                                del.IsEdeliveryRequested
                            }).First();

            AuanDeliveryDataDTO result = new AuanDeliveryDataDTO
            {
                Id = delivery.Id,
                DeliveryType = delivery.DeliveryType,
                SentDate = delivery.SentDate,
                ReferenceNum = delivery.ReferenceNum,
                StateService = delivery.StateService,
                TerritoryUnitId = delivery.TerritoryUnitId,
                IsDelivered = delivery.IsDelivered,
                DeliveryDate = delivery.DeliveryDate,
                RefusalDate = delivery.RefusalDate,
                IsEDeliveryRequested = delivery.IsEdeliveryRequested
            };

            if (result.DeliveryType == InspDeliveryTypesEnum.ByMail)
            {
                result.Address = this.addressService.GetAddressRegistration(delivery.AddressId.Value);
            }

            if (result.DeliveryType == InspDeliveryTypesEnum.Refusal)
            {
                result.RefusalWitnesses = this.GetWitnesses(null, deliveryId);
            }

            return result;
        }

        private void AddViolatedRegulations(Auanregister auan, List<AuanViolatedRegulationDTO> regulations)
        {
            if (regulations != null)
            {
                foreach (AuanViolatedRegulationDTO regulation in regulations)
                {
                    this.AddViolatedRegulationEntry(auan, regulation);
                }
            }
        }

        private void AddWitnesses(Auanregister auan, List<AuanWitnessDTO> witnesses)
        {
            if (witnesses != null)
            {
                foreach (AuanWitnessDTO witness in witnesses)
                {
                    this.AddWitnessEntry(auan, witness);
                }
            }
        }

        private void AddConfiscatedFish(Auanregister auan, List<AuanConfiscatedFishDTO> fishes)
        {
            if (fishes != null)
            {
                foreach (AuanConfiscatedFishDTO fish in fishes)
                {
                    this.AddConfiscatedFishEntry(auan, fish);
                }
            }
        }

        private void AddConfiscatedFishingGear(Auanregister auan, List<AuanConfiscatedFishingGearDTO> gears)
        {
            if (gears != null)
            {
                foreach (AuanConfiscatedFishingGearDTO gear in gears)
                {
                    this.AddConfiscatedFishingGearEntry(auan, gear);
                }
            }
        }

        private void AddDeliveryData(Auanregister auan, AuanDeliveryDataDTO delivery)
        {
            InspDelivery entry = new InspDelivery
            {
                DeliveryTypeId = this.GetDeliveryTypeIdByCode(delivery.DeliveryType.Value),
                IsDelivered = delivery.IsDelivered.Value,
                IsEdeliveryRequested = delivery.IsEDeliveryRequested
            };

            this.EditDeliveryDataByType(entry, delivery);

            auan.Delivery = entry;
        }

        private void EditViolatedRegulations(Auanregister auan, List<AuanViolatedRegulationDTO> regulations)
        {
            if (regulations != null)
            {
                List<AuanviolatedRegulation> dbRegulations = regulations.Any(x => x.Id != null)
                    ? this.Db.AuanViolatedRegulations.Where(x => x.AuanregisterId == auan.Id).ToList()
                    : new List<AuanviolatedRegulation>();

                foreach (AuanViolatedRegulationDTO regulation in regulations)
                {
                    if (regulation.Id == null)
                    {
                        this.AddViolatedRegulationEntry(auan, regulation);
                    }
                    else
                    {
                        AuanviolatedRegulation dbRegulation = dbRegulations.Where(x => x.Id == regulation.Id).Single();
                        dbRegulation.Article = regulation.Article;
                        dbRegulation.Paragraph = regulation.Paragraph;
                        dbRegulation.Section = regulation.Section;
                        dbRegulation.Letter = regulation.Letter;
                        dbRegulation.RegulationType = regulation.Type.ToString();
                        dbRegulation.IsActive = regulation.IsActive.Value;
                    }
                }
            }
            else
            {
                List<AuanviolatedRegulation> dbRegulations = this.Db.AuanViolatedRegulations.Where(x => x.AuanregisterId == auan.Id).ToList();

                foreach (AuanviolatedRegulation regulation in dbRegulations)
                {
                    regulation.IsActive = false;
                }
            }
        }

        private void EditAuanWitnesses(Auanregister auan, List<AuanWitnessDTO> witnesses)
        {
            List<int> witnessesIds = witnesses.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToList();

            List<Auanwitness> currentWitnesses = (from witness in Db.Auanwitnesses
                                                  where witness.AuanregisterId == auan.Id
                                                  select witness).ToList();

            List<Auanwitness> witnessesToDelete = currentWitnesses.Where(x => x.IsActive && !witnessesIds.Contains(x.Id)).ToList();
            List<Auanwitness> witnessesToEdit = currentWitnesses.Where(x => witnessesIds.Contains(x.Id)).ToList();
            List<AuanWitnessDTO> witnessesToAdd = witnesses.Where(x => x.Id == default).ToList();

            foreach (Auanwitness witness in witnessesToDelete)
            {
                witness.IsActive = false;
            }

            foreach (Auanwitness witness in witnessesToEdit)
            {
                AuanWitnessDTO newWitness = witnesses.Where(x => x.Id == witness.Id).Single();
                witness.WitnessNames = newWitness.WitnessNames;
                witness.OrderNum = newWitness.OrderNum.Value;
                witness.IsActive = newWitness.IsActive.Value;

                Address witnessAddress = Db.AddOrEditAddress(newWitness.Address, true);
                witness.Address = witnessAddress;
            }

            foreach (AuanWitnessDTO witness in witnessesToAdd)
            {
                this.AddWitnessEntry(auan, witness);
            }
        }


        private void EditDeliveryWitnesses(InspDelivery delivery, List<AuanWitnessDTO> witnesses)
        {
            List<int> witnessesIds = witnesses.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToList();

            List<Auanwitness> currentWitnesses = (from witness in Db.Auanwitnesses
                                                  where witness.InspDeliveryId == delivery.Id
                                                  select witness).ToList();

            List<Auanwitness> witnessesToDelete = currentWitnesses.Where(x => x.IsActive && !witnessesIds.Contains(x.Id)).ToList();
            List<Auanwitness> witnessesToEdit = currentWitnesses.Where(x => witnessesIds.Contains(x.Id)).ToList();
            List<AuanWitnessDTO> witnessesToAdd = witnesses.Where(x => x.Id == default).ToList();

            foreach (Auanwitness witness in witnessesToDelete)
            {
                witness.IsActive = false;
            }

            foreach (Auanwitness witness in witnessesToEdit)
            {
                AuanWitnessDTO newWitness = witnesses.Where(x => x.Id == witness.Id).Single();
                witness.WitnessNames = newWitness.WitnessNames;
                witness.OrderNum = newWitness.OrderNum.Value;
                witness.IsActive = newWitness.IsActive.Value;

                Address witnessAddress = Db.AddOrEditAddress(newWitness.Address, true);
                witness.Address = witnessAddress;
            }

            foreach (AuanWitnessDTO witness in witnessesToAdd)
            {
                this.AddDeliveryWitnessEntry(delivery, witness);
            }
        }


        private void EditConfiscatedFish(Auanregister auan, List<AuanConfiscatedFishDTO> fishes)
        {
            if (fishes != null)
            {
                List<AuanconfiscatedFish> dbFishes = fishes.Any(x => x.Id != null)
                    ? this.Db.AuanConfiscatedFishes.Where(x => x.AuanregisterId == auan.Id).ToList()
                    : new List<AuanconfiscatedFish>();

                foreach (AuanConfiscatedFishDTO fish in fishes)
                {
                    if (fish.Id == null)
                    {
                        this.AddConfiscatedFishEntry(auan, fish);
                    }
                    else
                    {
                        AuanconfiscatedFish dbConfiscatedFish = dbFishes.Where(x => x.Id == fish.Id).Single();
                        dbConfiscatedFish.FishId = fish.FishTypeId;
                        dbConfiscatedFish.Weight = fish.Weight;
                        dbConfiscatedFish.Count = fish.Count;
                        dbConfiscatedFish.ConfiscationActionId = fish.ConfiscationActionId.Value;
                        dbConfiscatedFish.ApplicanceId = fish.ApplianceId;
                        dbConfiscatedFish.TurbotSizeGroupId = fish.TurbotSizeGroupId;
                        dbConfiscatedFish.Comments = fish.Comments;
                        dbConfiscatedFish.IsActive = fish.IsActive.Value;
                    }
                }
            }
            else
            {
                List<AuanconfiscatedFish> dbFishes = this.Db.AuanConfiscatedFishes.Where(x => x.AuanregisterId == auan.Id).ToList();

                foreach (AuanconfiscatedFish fish in dbFishes)
                {
                    fish.IsActive = false;
                }
            }
        }

        private void EditConfiscatedFishingGear(Auanregister auan, List<AuanConfiscatedFishingGearDTO> gears)
        {
            if (gears != null)
            {
                List<AuanconfiscatedFishingGear> dbFishingGears = gears.Any(x => x.Id != null)
                    ? this.Db.AuanConfiscatedFishingGear.Where(x => x.AuanregisterId == auan.Id).ToList()
                    : new List<AuanconfiscatedFishingGear>();

                foreach (AuanConfiscatedFishingGearDTO gear in gears)
                {
                    if (gear.Id == null)
                    {
                        this.AddConfiscatedFishingGearEntry(auan, gear);
                    }
                    else
                    {
                        AuanconfiscatedFishingGear dbConfiscatedFishingGear = dbFishingGears.Where(x => x.Id == gear.Id).Single();
                        dbConfiscatedFishingGear.FishingGearId = gear.FishingGearId.Value;
                        dbConfiscatedFishingGear.Count = gear.Count.Value;
                        dbConfiscatedFishingGear.Length = gear.Length;
                        dbConfiscatedFishingGear.NetEyeSize = gear.NetEyeSize;
                        dbConfiscatedFishingGear.ConfiscationActionId = gear.ConfiscationActionId.Value;
                        dbConfiscatedFishingGear.Comments = gear.Comments;
                        dbConfiscatedFishingGear.IsActive = gear.IsActive.Value;
                    }
                }
            }
            else
            {
                List<AuanconfiscatedFishingGear> dbFishingGears = this.Db.AuanConfiscatedFishingGear.Where(x => x.AuanregisterId == auan.Id).ToList();

                foreach (AuanconfiscatedFishingGear gear in dbFishingGears)
                {
                    gear.IsActive = false;
                }
            }
        }

        private void EditDeliveryData(AuanDeliveryDataDTO delivery)
        {
            InspDelivery entry = (from del in this.Db.InspDelivery
                                  where del.Id == delivery.Id.Value
                                  select del).Single();

            entry.DeliveryTypeId = this.GetDeliveryTypeIdByCode(delivery.DeliveryType.Value);
            entry.IsDelivered = delivery.IsDelivered.Value;
            entry.IsEdeliveryRequested = delivery.IsEDeliveryRequested;

            if (!entry.IsDelivered)
            {
                entry.ConfirmationTypeId = null;
            }

            this.EditDeliveryDataByType(entry, delivery);
        }

        private void EditDeliveryDataByType(InspDelivery entry, AuanDeliveryDataDTO delivery)
        {
            switch (delivery.DeliveryType.Value)
            {
                case InspDeliveryTypesEnum.ByMail:
                    entry.SentDate = delivery.SentDate.Value;
                    entry.ReferenceNum = delivery.ReferenceNum;
                    entry.Address = this.Db.AddOrEditAddress(delivery.Address, deferSave: true, idToEdit: entry.AddressId);

                    if (delivery.Id.HasValue)
                    {
                        entry.StateService = null;
                        entry.TerritoryUnitId = null;
                        entry.DeliveryDate = null;
                        entry.RefusalDate = null;
                    }
                    break;
                case InspDeliveryTypesEnum.StateService:
                    entry.ReferenceNum = delivery.ReferenceNum;
                    entry.StateService = delivery.StateService;

                    if (delivery.Id.HasValue)
                    {
                        entry.SentDate = null;
                        entry.TerritoryUnitId = null;
                        entry.AddressId = null;
                        entry.DeliveryDate = null;
                        entry.RefusalDate = null;
                    }
                    break;
                case InspDeliveryTypesEnum.Office:
                    entry.TerritoryUnitId = delivery.TerritoryUnitId.Value;

                    if (delivery.Id.HasValue)
                    {
                        entry.SentDate = null;
                        entry.ReferenceNum = null;
                        entry.AddressId = null;
                        entry.StateService = null;
                        entry.DeliveryDate = null;
                        entry.RefusalDate = null;
                    }
                    break;
                case InspDeliveryTypesEnum.Refusal:
                    entry.RefusalDate = delivery.RefusalDate.Value;
                    this.EditDeliveryWitnesses(entry, delivery.RefusalWitnesses);

                    if (delivery.Id.HasValue)
                    {
                        entry.DeliveryDate = null;
                        entry.SentDate = null;
                        entry.ReferenceNum = null;
                        entry.AddressId = null;
                        entry.StateService = null;
                        entry.TerritoryUnitId = null;
                    }
                    break;
                case InspDeliveryTypesEnum.Personal:
                    entry.DeliveryDate = delivery.DeliveryDate.Value;

                    if (delivery.Id.HasValue)
                    {
                        entry.RefusalDate = null;
                        entry.SentDate = null;
                        entry.ReferenceNum = null;
                        entry.AddressId = null;
                        entry.StateService = null;
                        entry.TerritoryUnitId = null;
                    }
                    break;
            }
        }

        private void AddViolatedRegulationEntry(Auanregister auan, AuanViolatedRegulationDTO regulation)
        {
            AuanviolatedRegulation entry = new AuanviolatedRegulation
            {
                Auanregister = auan,
                Article = regulation.Article,
                Paragraph = regulation.Paragraph,
                Section = regulation.Section,
                Letter = regulation.Letter,
                RegulationType = regulation.Type.ToString(),
                IsActive = regulation.IsActive.Value
            };

            this.Db.AuanViolatedRegulations.Add(entry);
        }

        private void AddWitnessEntry(Auanregister auan, AuanWitnessDTO witness)
        {
            Auanwitness entry = new Auanwitness
            {
                Auanregister = auan,
                WitnessNames = witness.WitnessNames,
                DateOfBirth = witness.DateOfBirth,
                OrderNum = witness.OrderNum.Value,
                IsActive = witness.IsActive.Value
            };

            Address witnessAddress = Db.AddOrEditAddress(witness.Address, true);
            entry.Address = witnessAddress;

            this.Db.Auanwitnesses.Add(entry);
        }

        private void AddDeliveryWitnessEntry(InspDelivery delivery, AuanWitnessDTO witness)
        {
            Auanwitness entry = new Auanwitness
            {
                InspDelivery = delivery,
                WitnessNames = witness.WitnessNames,
                DateOfBirth = witness.DateOfBirth,
                OrderNum = witness.OrderNum.Value,
                IsActive = witness.IsActive.Value
            };

            Address witnessAddress = Db.AddOrEditAddress(witness.Address, true);
            entry.Address = witnessAddress;

            this.Db.Auanwitnesses.Add(entry);
        }

        private void AddConfiscatedFishEntry(Auanregister auan, AuanConfiscatedFishDTO fish)
        {
            AuanconfiscatedFish entry = new AuanconfiscatedFish
            {
                Auanregister = auan,
                FishId = fish.FishTypeId,
                Weight = fish.Weight,
                Count = fish.Count,
                ApplicanceId = fish.ApplianceId,
                TurbotSizeGroupId = fish.TurbotSizeGroupId,
                ConfiscationActionId = fish.ConfiscationActionId.Value,
                Comments = fish.Comments,
                IsActive = fish.IsActive.Value
            };

            this.Db.AuanConfiscatedFishes.Add(entry);
        }

        private void AddConfiscatedFishingGearEntry(Auanregister auan, AuanConfiscatedFishingGearDTO gear)
        {
            AuanconfiscatedFishingGear entry = new AuanconfiscatedFishingGear
            {
                Auanregister = auan,
                FishingGearId = gear.FishingGearId.Value,
                Count = gear.Count.Value,
                Length = gear.Length,
                NetEyeSize = gear.NetEyeSize,
                ConfiscationActionId = gear.ConfiscationActionId.Value,
                Comments = gear.Comments,
                IsActive = gear.IsActive.Value
            };

            this.Db.AuanConfiscatedFishingGear.Add(entry);
        }

        private int GetDeliveryTypeIdByCode(InspDeliveryTypesEnum deliveryType)
        {
            int deliveryTypeId = (from type in this.Db.NinspDeliveryTypes
                                  where type.Code == deliveryType.ToString()
                                  select type.Id).Single();

            return deliveryTypeId;
        }
    }

    internal class AuanInspectedEntityHelper : AuanInspectedEntityDTO
    {
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
    }
}
