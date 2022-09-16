using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;
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
    public class PenalDecreesService : Service, IPenalDecreesService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        private readonly IAuanRegisterService auanService;
        private readonly IJasperReportExecutionService jasperReportsService;

        public PenalDecreesService(IARADbContext db,
                                   IPersonService personService,
                                   ILegalService legalService,
                                   IAuanRegisterService auanService,
                                   IJasperReportExecutionService jasperReportsService)
            : base(db)
        {
            this.personService = personService;
            this.legalService = legalService;
            this.auanService = auanService;
            this.jasperReportsService = jasperReportsService;
        }

        public IQueryable<PenalDecreeDTO> GetAllPenalDecrees(PenalDecreesFilters filters)
        {
            IQueryable<PenalDecreeDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPenalDecrees(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredPenalDecrees(filters)
                    : GetFreeTextFilteredPenalDecrees(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public PenalDecreeEditDTO GetPenalDecree(int id)
        {
            var query = (from decree in Db.PenalDecreesRegisters
                         where decree.Id == id
                         select new
                         {
                             decree.Id,
                             decree.PenalDecreeTypeId,
                             decree.AuanRegisterId,
                             decree.DecreeNum,
                             decree.IssueDate,
                             decree.EffectiveDate,
                             decree.FineAmount,
                             decree.CompensationAmount,
                             decree.SanctionDescription,
                             decree.IsRecurrentViolation,
                             decree.Comments,
                             decree.DeliveryId,
                             decree.IssuerPosition,
                             decree.MinorCircumstancesDescription,
                             decree.ConstatationComments,
                             decree.IssuerUserId
                         }).First();

            PenalDecreeEditDTO result = new PenalDecreeEditDTO
            {
                Id = query.Id,
                TypeId = query.PenalDecreeTypeId,
                AuanId = query.AuanRegisterId,
                DecreeNum = query.DecreeNum,
                IssueDate = query.IssueDate,
                EffectiveDate = query.EffectiveDate,
                FineAmount = query.FineAmount,
                CompensationAmount = query.CompensationAmount,
                SanctionDescription = query.SanctionDescription,
                IsRecurrentViolation = query.IsRecurrentViolation,
                Comments = query.Comments,
                IssuerUserId = query.IssuerUserId,
                IssuerPosition = query.IssuerPosition,
                MinorCircumstancesDescription = query.MinorCircumstancesDescription,
                ConstatationComments = query.ConstatationComments
            };

            result.SanctionTypeIds = GetSanctionTypeIds(result.Id.Value);
            result.SeizedFish = GetPenalDecreeSeizedFish(result.Id.Value);
            result.SeizedAppliance = GetPenalDecreeSeizedAppliance(result.Id.Value);
            result.SeizedFishingGear = GetPenalDecreeSeizedFishingGear(result.Id.Value);
            result.FishCompensations = GetPenalDecreeFishCompensations(result.Id.Value);
            result.Statuses = GetPenalDecreeStatuses(result.Id.Value);
            result.AuanViolatedRegulations = GetViolatedRegulations(result.Id.Value, ViolatedRegulationSectionTypesEnum.AUAN);
            result.DecreeViolatedRegulations = GetViolatedRegulations(result.Id.Value, ViolatedRegulationSectionTypesEnum.Sanction);
            result.FishCompensationViolatedRegulations = GetViolatedRegulations(result.Id.Value, ViolatedRegulationSectionTypesEnum.FishCompensation);

            if (query.DeliveryId.HasValue)
            {
                result.DeliveryData = GetDeliveryData(query.DeliveryId.Value);
            }

            result.Files = Db.GetFiles(Db.PenalDecreesRegisterFiles, result.Id.Value);

            return result;
        }

        public int AddPenalDecree(PenalDecreeEditDTO decree)
        {
            using TransactionScope scope = new TransactionScope();

            PenalDecreesRegister entry = new PenalDecreesRegister
            {
                AuanRegisterId = decree.AuanId.Value,
                PenalDecreeTypeId = decree.TypeId.Value,
                DecreeNum = decree.DecreeNum,
                IssueDate = decree.IssueDate.Value,
                EffectiveDate = decree.EffectiveDate.Value,
                FineAmount = decree.FineAmount,
                CompensationAmount = decree.CompensationAmount,
                SanctionDescription = decree.SanctionDescription,
                IsRecurrentViolation = decree.IsRecurrentViolation,
                Comments = decree.Comments,
                IssuerPosition = decree.IssuerPosition,
                MinorCircumstancesDescription = decree.MinorCircumstancesDescription,
                ConstatationComments = decree.ConstatationComments,
                IssuerUserId = decree.IssuerUserId.Value
            };

            Db.PenalDecreesRegisters.Add(entry);

            AddSanctionTypes(entry, decree.SanctionTypeIds);
            AddSeizedFish(entry, decree.SeizedFish);
            AddSeizedFish(entry, decree.SeizedAppliance);
            AddSeizedFishingGear(entry, decree.SeizedFishingGear);
            AddFishCompensation(entry, decree.FishCompensations);
            AddViolatedRegulations(entry, decree.AuanViolatedRegulations, ViolatedRegulationSectionTypesEnum.AUAN);
            AddViolatedRegulations(entry, decree.DecreeViolatedRegulations, ViolatedRegulationSectionTypesEnum.Sanction);
            AddViolatedRegulations(entry, decree.FishCompensationViolatedRegulations, ViolatedRegulationSectionTypesEnum.FishCompensation);
            AddDecreeStatus(entry, decree.Statuses);

            if (decree.DeliveryData != null)
            {
                AddDeliveryData(entry, decree.DeliveryData);
            }

            if (decree.Files != null)
            {
                foreach (FileInfoDTO file in decree.Files)
                {
                    Db.AddOrEditFile(entry, entry.PenalDecreesRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            scope.Complete();
            return entry.Id;
        }

        public void EditPenalDecree(PenalDecreeEditDTO decree)
        {
            using TransactionScope scope = new TransactionScope();

            PenalDecreesRegister entry = (from pd in Db.PenalDecreesRegisters
                                            .Include(x => x.PenalDecreesRegisterFiles)
                                          where pd.Id == decree.Id.Value
                                          select pd).First();

            entry.DecreeNum = decree.DecreeNum;
            entry.IssueDate = decree.IssueDate.Value;
            entry.EffectiveDate = decree.EffectiveDate.Value;
            entry.FineAmount = decree.FineAmount;
            entry.CompensationAmount = decree.CompensationAmount;
            entry.IsRecurrentViolation = decree.IsRecurrentViolation;
            entry.Comments = decree.Comments;
            entry.SanctionDescription = decree.SanctionDescription;
            entry.MinorCircumstancesDescription = decree.MinorCircumstancesDescription;
            entry.ConstatationComments = decree.ConstatationComments;
            entry.IssuerPosition = decree.IssuerPosition;
            entry.IssuerUserId = decree.IssuerUserId.Value;

            EditDecreeSanctions(entry.Id, decree.SanctionTypeIds);
            EditSeizedFish(entry, decree.SeizedFish);
            EditSeizedFish(entry, decree.SeizedAppliance);
            EditSeizedFishingGear(entry, decree.SeizedFishingGear);
            EditFishCompensation(entry, decree.FishCompensations);
            EditViolatedRegulations(entry, decree.AuanViolatedRegulations, ViolatedRegulationSectionTypesEnum.AUAN);
            EditViolatedRegulations(entry, decree.DecreeViolatedRegulations, ViolatedRegulationSectionTypesEnum.Sanction);
            EditViolatedRegulations(entry, decree.FishCompensationViolatedRegulations, ViolatedRegulationSectionTypesEnum.FishCompensation);
            EditDecreeStatuses(entry, decree.Statuses);

            if (decree.DeliveryData != null)
            {
                EditDeliveryData(entry, decree.DeliveryData);
            }

            if (decree.Files != null)
            {
                foreach (FileInfoDTO file in decree.Files)
                {
                    Db.AddOrEditFile(entry, entry.PenalDecreesRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            scope.Complete();
        }

        public void DeletePenalDecree(int id)
        {
            DeleteRecordWithId(Db.PenalDecreesRegisters, id);
            Db.SaveChanges();
        }

        public void UndoDeletePenalDecree(int id)
        {
            UndoDeleteRecordWithId(Db.PenalDecreesRegisters, id);
            Db.SaveChanges();
        }

        public PenalDecreeAuanDataDTO GetPenalDecreeAuanData(int auanId)
        {
            var query = (from auan in Db.AuanRegister
                         join inspection in this.Db.InspectionsRegister on auan.InspectionId equals inspection.Id
                         join inspInsp in this.Db.InspectionInspectors on inspection.Id equals inspInsp.InspectionId into insp
                         from inspInsp in insp.DefaultIfEmpty()
                         join inspector in this.Db.Inspectors on inspInsp.InspectorId equals inspector.Id into i
                         from inspector in i.DefaultIfEmpty()
                         join unregPerson in this.Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPer
                         from unregPerson in unregPer.DefaultIfEmpty()
                         join user in this.Db.Users on inspector.UserId equals user.Id into us
                         from user in us.DefaultIfEmpty()
                         join person in this.Db.Persons on user.PersonId equals person.Id into per
                         from person in per.DefaultIfEmpty()
                         where auan.Id == auanId
                           && (inspInsp == null || inspInsp.IsInCharge)
                         select new
                         {
                             auan.AuanNum,
                             auan.DraftDate,
                             auan.LocationDescription,
                             auan.ConstatationComments,
                             auan.OffenderComments,
                             Drafter = unregPerson != null
                                             ? unregPerson.FirstName + " " + unregPerson.LastName
                                             : person.FirstName + " " + person.LastName,
                             auan.InspectedPersonId,
                             auan.InspectedLegalId,
                             auan.InspectedPersonWorkPlace,
                             auan.InspectedPersonWorkPosition,
                             inspection.TerritoryUnitId
                         }).First();

            PenalDecreeAuanDataDTO data = new PenalDecreeAuanDataDTO
            {
                AuanNum = query.AuanNum,
                DraftDate = query.DraftDate,
                LocationDescription = query.LocationDescription,
                ConstatationComments = query.ConstatationComments,
                OffenderComments = query.OffenderComments,
                Drafter = query.Drafter,
                TerritoryUnitId = query.TerritoryUnitId
            };

            if (query.InspectedPersonId.HasValue)
            {
                data.InspectedEntity = new AuanInspectedEntityDTO
                {
                    IsUnregisteredPerson = false,
                    IsPerson = true,
                    PersonWorkPlace = query.InspectedPersonWorkPlace,
                    PersonWorkPosition = query.InspectedPersonWorkPosition
                };

                data.InspectedEntity.Person = this.personService.GetRegixPersonData(query.InspectedPersonId.Value);
                data.InspectedEntity.Addresses = this.personService.GetAddressRegistrations(query.InspectedPersonId.Value);
            }
            else if (query.InspectedLegalId.HasValue)
            {
                data.InspectedEntity = new AuanInspectedEntityDTO
                {
                    IsUnregisteredPerson = false,
                    IsPerson = false
                };

                data.InspectedEntity.Legal = this.legalService.GetRegixLegalData(query.InspectedLegalId.Value);
                data.InspectedEntity.Addresses = this.legalService.GetAddressRegistrations(query.InspectedLegalId.Value);
            }

            data.ConfiscatedFish = this.GetAuanConfiscatedFish(auanId);
            data.ConfiscatedAppliance = this.GetAuanConfiscatedAppliance(auanId);
            data.ConfiscatedFishingGear = this.GetAuanConfiscatedFishingGear(auanId);
            data.ViolatedRegulations = this.GetAuanViolatedRegulations(auanId);

            return data;
        }

        public async Task<DownloadableFileDTO> GetRegisterFileForDownload(int decreeId)
        {
            DownloadableFileDTO downloadableFile = new DownloadableFileDTO();
            downloadableFile.MimeType = "application/pdf";

            PenalDecreeTypeEnum decreeType = (from decree in Db.PenalDecreesRegisters
                                              join type in Db.NpenalDecreeTypes on decree.PenalDecreeTypeId equals type.Id
                                              where decree.Id == decreeId
                                              select Enum.Parse<PenalDecreeTypeEnum>(type.Code)).First();

            switch (decreeType)
            {
                case PenalDecreeTypeEnum.PenalDecree:
                    {
                        downloadableFile.FileName = $"Наказателно-постановление.pdf".Replace("  ", "");
                        downloadableFile.Bytes = await jasperReportsService.GetPenalDecreesRegister(decreeId);
                    }
                    break;
                case PenalDecreeTypeEnum.Agreement:
                    {
                        downloadableFile.FileName = $"Споразумение.pdf".Replace("  ", "");
                        downloadableFile.Bytes = await jasperReportsService.GetPenalDecreesAgreement(decreeId);
                    }
                    break;
                case PenalDecreeTypeEnum.Warning:
                    {
                        downloadableFile.FileName = $"Предупреждение.pdf".Replace("  ", "");
                        downloadableFile.Bytes = await jasperReportsService.GetPenalDecreesWarning(decreeId);
                    }
                    break;
            }

            return downloadableFile;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.PenalDecreesRegisters, id);
            return audit;
        }

        public SimpleAuditDTO GetPenalDecreeStatusSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.PenalDecreeStatuses, id);
            return audit;
        }

        private IQueryable<PenalDecreeDTO> GetAllPenalDecrees(bool showInactive)
        {
            IQueryable<PenalDecreeDTO> result = from decree in Db.PenalDecreesRegisters
                                                join type in Db.NpenalDecreeTypes on decree.PenalDecreeTypeId equals type.Id
                                                join auan in Db.AuanRegister on decree.AuanRegisterId equals auan.Id
                                                join inspPerson in Db.Persons on auan.InspectedPersonId equals inspPerson.Id into inspPer
                                                from inspPerson in inspPer.DefaultIfEmpty()
                                                join inspLegal in Db.Legals on auan.InspectedLegalId equals inspLegal.Id into inspLeg
                                                from inspLegal in inspLeg.DefaultIfEmpty()
                                                where decree.IsActive == !showInactive
                                                orderby decree.IssueDate descending
                                                select new PenalDecreeDTO
                                                {
                                                    Id = decree.Id,
                                                    TypeId = decree.PenalDecreeTypeId,
                                                    DecreeType = Enum.Parse<PenalDecreeTypeEnum>(type.Code),
                                                    DecreeName = type.Name,
                                                    AuanId = decree.AuanRegisterId,
                                                    DecreeNum = decree.DecreeNum,
                                                    Status = decree.PenalDecreeStatuses.OrderByDescending(x => x.CreatedOn).Select(x => x.StatusType.Name).FirstOrDefault(), //TODO ????
                                                    IssueDate = decree.IssueDate,
                                                    InspectedEntity = inspPerson != null ? inspPerson.FirstName + " " + inspPerson.LastName : inspLegal.Name,
                                                    IsActive = decree.IsActive
                                                };

            return result;
        }

        private IQueryable<PenalDecreeDTO> GetParametersFilteredPenalDecrees(PenalDecreesFilters filters)
        {
            var query = from decree in Db.PenalDecreesRegisters
                        join type in Db.NpenalDecreeTypes on decree.PenalDecreeTypeId equals type.Id
                        join auan in Db.AuanRegister on decree.AuanRegisterId equals auan.Id
                        join inspPerson in Db.Persons on auan.InspectedPersonId equals inspPerson.Id into inspPer
                        from inspPerson in inspPer.DefaultIfEmpty()
                        join inspLegal in Db.Legals on auan.InspectedLegalId equals inspLegal.Id into inspLeg
                        from inspLegal in inspLeg.DefaultIfEmpty()
                        join inspInsp in Db.InspectionInspectors on auan.InspectionId equals inspInsp.InspectionId
                        join inspection in Db.InspectionsRegister on inspInsp.InspectionId equals inspection.Id
                        join inspector in Db.Inspectors on inspInsp.InspectorId equals inspector.Id
                        join unregPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPer
                        from unregPerson in unregPer.DefaultIfEmpty()
                        join user in Db.Users on inspector.UserId equals user.Id into us
                        from user in us.DefaultIfEmpty()
                        join person in Db.Persons on user.PersonId equals person.Id into per
                        from person in per.DefaultIfEmpty()
                        where decree.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            decree.Id,
                            decree.PenalDecreeTypeId,
                            Type = type.Code,
                            Name = type.Name,
                            decree.AuanRegisterId,
                            decree.DecreeNum,
                            inspection.TerritoryUnitId,
                            decree.IssueDate,
                            DrafterId = inspector.Id,
                            Drafter = unregPerson != null
                                      ? unregPerson.FirstName + " " + unregPerson.LastName
                                      : person.FirstName + " " + person.LastName,
                            InspectedEntity = inspPerson != null ? inspPerson.FirstName + " " + inspPerson.LastName : inspLegal.Name,
                            InspectedPersonFirstName = inspPerson.FirstName,
                            InspectedPersonMiddleName = inspPerson.MiddleName,
                            InspectedPersonLastName = inspPerson.LastName,
                            InspectedPersonIdentifier = inspPerson != null ? inspPerson.EgnLnc : inspLegal.Eik,
                            auan.LocationDescription,
                            Status = decree.PenalDecreeStatuses.OrderByDescending(x => x.CreatedOn).Select(x => x.StatusType.Name).FirstOrDefault(),
                            StatusTypeId = decree.PenalDecreeStatuses.OrderByDescending(x => x.CreatedOn).Select(x => x.StatusTypeId).FirstOrDefault(),
                            decree.DeliveryId,
                            decree.FineAmount,
                            decree.IsActive
                        };

            if (!string.IsNullOrEmpty(filters.PenalDecreeNum))
            {
                query = query.Where(x => x.DecreeNum.ToLower().Contains(filters.PenalDecreeNum.ToLower()));
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

            if (filters.IssueDateFrom.HasValue)
            {
                query = query.Where(x => x.IssueDate >= filters.IssueDateFrom.Value);
            }

            if (filters.IssueDateTo.HasValue)
            {
                query = query.Where(x => x.IssueDate <= filters.IssueDateTo.Value);
            }

            if (filters.SanctionTypeIds != null && filters.SanctionTypeIds.Count > 0)
            {
                List<int> decreeIds = (from sanction in Db.PenalDecreeSanctions
                                       where filters.SanctionTypeIds.Contains(sanction.SanctionTypeId)
                                            && sanction.IsActive
                                       select sanction.PenalDecreeId).ToList();

                query = query.Where(x => decreeIds.Contains(x.Id));
            }

            if (filters.StatusTypeIds != null && filters.StatusTypeIds.Count > 0)
            {
                query = query.Where(x => filters.StatusTypeIds.Contains(x.StatusTypeId));
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
                query = from decree in query
                        join fish in Db.AuanConfiscatedFishes on decree.Id equals fish.PenalDecreeRegisterId
                        where filters.FishId == fish.FishId
                            && fish.IsActive
                        select decree;
            }

            if (filters.FishingGearId.HasValue)
            {
                query = from decree in query
                        join gear in Db.PenalDecreeSeizedFishingGears on decree.Id equals gear.PenalDecreeId
                        where filters.FishingGearId == gear.FishingGearId
                            && gear.IsActive
                        select decree;
            }

            if (filters.ApplianceId.HasValue)
            {
                query = from decree in query
                        join fish in Db.AuanConfiscatedFishes on decree.Id equals fish.PenalDecreeRegisterId
                        where filters.ApplianceId == fish.ApplicanceId
                            && fish.IsActive
                        select decree;
            }

            if (filters.IsDelivered.HasValue)
            {
                query = from decree in query
                        join delivery in Db.InspDelivery on decree.DeliveryId equals delivery.Id
                        where filters.IsDelivered == delivery.IsDelivered
                            && delivery.IsActive
                        select decree;
            }

            if (filters.FineAmountFrom.HasValue)
            {
                query = query.Where(x => x.FineAmount >= filters.FineAmountFrom.Value);
            }

            if (filters.FineAmountTo.HasValue)
            {
                query = query.Where(x => x.FineAmount <= filters.FineAmountTo.Value);
            }

            if (filters.AuanId.HasValue)
            {
                query = query.Where(x => x.AuanRegisterId == filters.AuanId);
            }

            IQueryable<PenalDecreeDTO> result = from decree in query
                                                orderby decree.IssueDate descending
                                                select new PenalDecreeDTO
                                                {
                                                    Id = decree.Id,
                                                    AuanId = decree.AuanRegisterId,
                                                    TypeId = decree.PenalDecreeTypeId,
                                                    DecreeType = Enum.Parse<PenalDecreeTypeEnum>(decree.Type),
                                                    DecreeName = decree.Name,
                                                    DecreeNum = decree.DecreeNum,
                                                    IssueDate = decree.IssueDate,
                                                    InspectedEntity = decree.InspectedEntity,
                                                    Status = decree.Status,
                                                    IsActive = decree.IsActive
                                                };

            return result;
        }

        private IQueryable<PenalDecreeDTO> GetFreeTextFilteredPenalDecrees(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<PenalDecreeDTO> result = from decree in Db.PenalDecreesRegisters
                                                join type in Db.NpenalDecreeTypes on decree.PenalDecreeTypeId equals type.Id
                                                join auan in Db.AuanRegister on decree.AuanRegisterId equals auan.Id
                                                join inspPerson in Db.Persons on auan.InspectedPersonId equals inspPerson.Id into inspPer
                                                from inspPerson in inspPer.DefaultIfEmpty()
                                                join inspLegal in Db.Legals on auan.InspectedLegalId equals inspLegal.Id into inspLeg
                                                from inspLegal in inspLeg.DefaultIfEmpty()
                                                where decree.IsActive == !showInactive
                                                    && (decree.DecreeNum.ToLower().Contains(text)
                                                        || (inspPerson != null ? inspPerson.FirstName + " " + inspPerson.LastName : inspLegal.Name).ToLower().Contains(text)
                                                        || (searchDate.HasValue && decree.IssueDate == searchDate.Value)
                                                        || type.Name.ToLower().Contains(text))
                                                orderby decree.IssueDate descending
                                                select new PenalDecreeDTO
                                                {
                                                    Id = decree.Id,
                                                    TypeId = decree.PenalDecreeTypeId,
                                                    DecreeType = Enum.Parse<PenalDecreeTypeEnum>(type.Code),
                                                    DecreeName = type.Name,
                                                    AuanId = decree.AuanRegisterId,
                                                    DecreeNum = decree.DecreeNum,
                                                    IssueDate = decree.IssueDate,
                                                    InspectedEntity = inspPerson != null ? inspPerson.FirstName + " " + inspPerson.LastName : inspLegal.Name,
                                                    Status = decree.PenalDecreeStatuses.OrderByDescending(x => x.CreatedOn).Select(x => x.StatusType.Name).FirstOrDefault(),
                                                    IsActive = decree.IsActive
                                                };

            return result;
        }

        private PenalDecreeDeliveryDataDTO GetDeliveryData(int deliveryId)
        {
            var delivery = (from del in Db.InspDelivery
                            join type in Db.NinspDeliveryTypes on del.DeliveryTypeId equals type.Id
                            join confirmationType in Db.NinspDeliveryConfirmationTypes on del.ConfirmationTypeId equals confirmationType.Id into confirmType
                            from confirmationType in confirmType.DefaultIfEmpty()
                            where del.Id == deliveryId
                            select new
                            {
                                del.Id,
                                DeliveryType = Enum.Parse<InspDeliveryTypesEnum>(type.Code),
                                del.SentDate,
                                del.IsDelivered,
                                del.DeliveryDate,
                                del.ReferenceNum,
                                ConfirmationType = confirmationType != null
                                            ? Enum.Parse<InspDeliveryConfirmationTypesEnum>(confirmationType.Code)
                                            : default(InspDeliveryConfirmationTypesEnum?),
                            }).First();

            PenalDecreeDeliveryDataDTO result = new PenalDecreeDeliveryDataDTO
            {
                Id = delivery.Id,
                DeliveryType = delivery.DeliveryType,
                SentDate = delivery.SentDate,
                IsDelivered = delivery.IsDelivered,
                DeliveryDate = delivery.DeliveryDate,
                ConfirmationType = delivery.ConfirmationType,
                ReferenceNum = delivery.ReferenceNum
            };

            if (result.DeliveryType != InspDeliveryTypesEnum.DecreeReturn || result.DeliveryType != InspDeliveryTypesEnum.DecreeTag)
            {
                result.RefusalWitnesses = this.auanService.GetWitnesses(null, deliveryId);
            }

            return result;
        }

        private List<PenalDecreeStatusEditDTO> GetPenalDecreeStatuses(int decreeId)
        {
            List<PenalDecreeStatusEditDTO> statuses = (from status in Db.PenalDecreeStatuses
                                                       where status.PenalDecreeId == decreeId
                                                       orderby status.Id descending
                                                       select new PenalDecreeStatusEditDTO
                                                       {
                                                           Id = status.Id,
                                                           StatusId = status.StatusTypeId,
                                                           StatusType = Enum.Parse<PenalDecreeStatusTypesEnum>(status.StatusType.Code),
                                                           DateOfChange = status.UpdatedOn != null
                                                                        ? status.UpdatedOn
                                                                        : status.CreatedOn,
                                                           StatusName = status.StatusType.Name,
                                                           CourtId = status.CourtId,
                                                           AppealDate = status.AppealDate,
                                                           CaseNum = status.CaseNum,
                                                           ComplaintDueDate = status.ComplaintDueDate,
                                                           RemunerationAmount = status.RemunerationAmount,
                                                           EnactmentDate = status.EnactmentDate,
                                                           PenalAuthorityTypeId = status.PenalAuthorityTypeId,
                                                           PenalAuthorityName = status.PenalAuthorityName,
                                                           Amendments = status.Amendments,
                                                           ConfiscationInstitutionId = status.ConfiscationInsitutionId,
                                                           PaidAmount = status.PaidAmount,
                                                           IsActive = status.IsActive
                                                       }).ToList();

            ILookup<int, PenalDecreePaymentScheduleDTO> payments = GetPenalDecreePaymentSchedule(statuses);

            foreach (PenalDecreeStatusEditDTO status in statuses)
            {
                if (status.StatusType == PenalDecreeStatusTypesEnum.Rescheduled)
                {
                    status.PaymentSchedule = payments[status.Id.Value].ToList();
                }
            }

            return statuses;
        }

        private List<int> GetSanctionTypeIds(int decreeId)
        {
            List<int> result = (from sanction in Db.PenalDecreeSanctions
                                where sanction.PenalDecreeId == decreeId
                                    && sanction.IsActive
                                select sanction.SanctionTypeId).ToList();

            return result;
        }

        private List<AuanViolatedRegulationDTO> GetAuanViolatedRegulations(int auanId)
        {
            List<AuanViolatedRegulationDTO> result = (from reg in Db.AuanViolatedRegulations
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

        private List<AuanViolatedRegulationDTO> GetViolatedRegulations(int decreeId, ViolatedRegulationSectionTypesEnum sanctionType)
        {
            List<AuanViolatedRegulationDTO> result = (from reg in Db.AuanViolatedRegulations
                                                      where reg.PenalDecreeRegisterId == decreeId
                                                        && reg.SectionType == sanctionType.ToString()
                                                      select new AuanViolatedRegulationDTO
                                                      {
                                                          Id = reg.Id,
                                                          Article = reg.Article,
                                                          Paragraph = reg.Paragraph,
                                                          Section = reg.Section,
                                                          Letter = reg.Letter,
                                                          Type = Enum.Parse<AuanViolatedRegulationTypesEnum>(reg.RegulationType),
                                                          SectionType = Enum.Parse<ViolatedRegulationSectionTypesEnum>(reg.SectionType),
                                                          IsActive = reg.IsActive
                                                      }).ToList();

            return result;
        }

        private List<PenalDecreeSeizedFishDTO> GetAuanConfiscatedFish(int auanId)
        {
            List<PenalDecreeSeizedFishDTO> result = (from fish in Db.AuanConfiscatedFishes
                                                     where fish.AuanregisterId == auanId
                                                        && fish.FishId.HasValue
                                                     select new PenalDecreeSeizedFishDTO
                                                     {
                                                         Id = fish.Id,
                                                         FishTypeId = fish.FishId,
                                                         Weight = fish.Weight,
                                                         Count = fish.Count,
                                                         ConfiscationActionId = fish.ConfiscationActionId,
                                                         ApplianceId = fish.ApplicanceId,
                                                         TurbotSizeGroupId = fish.TurbotSizeGroupId,
                                                         Comments = fish.Comments,
                                                         IsActive = fish.IsActive
                                                     }).ToList();

            return result;
        }

        private List<PenalDecreeSeizedFishDTO> GetAuanConfiscatedAppliance(int auanId)
        {
            List<PenalDecreeSeizedFishDTO> result = (from fish in Db.AuanConfiscatedFishes
                                                     where fish.AuanregisterId == auanId
                                                        && fish.ApplicanceId.HasValue
                                                     select new PenalDecreeSeizedFishDTO
                                                     {
                                                         Id = fish.Id,
                                                         Count = fish.Count,
                                                         ConfiscationActionId = fish.ConfiscationActionId,
                                                         ApplianceId = fish.ApplicanceId,
                                                         Comments = fish.Comments,
                                                         IsActive = fish.IsActive
                                                     }).ToList();

            return result;
        }

        private List<PenalDecreeSeizedFishingGearDTO> GetAuanConfiscatedFishingGear(int auanId)
        {
            List<PenalDecreeSeizedFishingGearDTO> result = (from gear in Db.AuanConfiscatedFishingGear
                                                            where gear.AuanregisterId == auanId
                                                            select new PenalDecreeSeizedFishingGearDTO
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

        private List<PenalDecreeSeizedFishDTO> GetPenalDecreeSeizedFish(int decreeId)
        {
            List<PenalDecreeSeizedFishDTO> result = (from fish in Db.AuanConfiscatedFishes
                                                     where fish.PenalDecreeRegisterId == decreeId
                                                        && fish.FishId.HasValue
                                                     select new PenalDecreeSeizedFishDTO
                                                     {
                                                         Id = fish.Id,
                                                         FishTypeId = fish.FishId,
                                                         Weight = fish.Weight,
                                                         Count = fish.Count,
                                                         ConfiscationActionId = fish.ConfiscationActionId,
                                                         ApplianceId = fish.ApplicanceId,
                                                         TurbotSizeGroupId = fish.TurbotSizeGroupId,
                                                         Comments = fish.Comments,
                                                         StorageTerritoryUnitId = fish.TerritoryUnitId,
                                                         IsActive = fish.IsActive
                                                     }).ToList();

            return result;
        }

        private List<PenalDecreeSeizedFishDTO> GetPenalDecreeSeizedAppliance(int decreeId)
        {
            List<PenalDecreeSeizedFishDTO> result = (from applicance in Db.AuanConfiscatedFishes
                                                     where applicance.PenalDecreeRegisterId == decreeId
                                                        && applicance.ApplicanceId.HasValue
                                                     select new PenalDecreeSeizedFishDTO
                                                     {
                                                         Id = applicance.Id,
                                                         Count = applicance.Count,
                                                         ConfiscationActionId = applicance.ConfiscationActionId,
                                                         ApplianceId = applicance.ApplicanceId,
                                                         Comments = applicance.Comments,
                                                         StorageTerritoryUnitId = applicance.TerritoryUnitId,
                                                         IsActive = applicance.IsActive
                                                     }).ToList();

            return result;
        }

        private List<PenalDecreeSeizedFishingGearDTO> GetPenalDecreeSeizedFishingGear(int decreeId)
        {
            List<PenalDecreeSeizedFishingGearDTO> result = (from gear in Db.PenalDecreeSeizedFishingGears
                                                            where gear.PenalDecreeId == decreeId
                                                            select new PenalDecreeSeizedFishingGearDTO
                                                            {
                                                                Id = gear.Id,
                                                                FishingGearId = gear.FishingGearId,
                                                                Count = gear.Count,
                                                                ConfiscationActionId = gear.ConfiscationActionId,
                                                                Comments = gear.Comments,
                                                                StorageTerritoryUnitId = gear.TerritoryUnitId,
                                                                IsActive = gear.IsActive
                                                            }).ToList();

            return result;
        }

        private List<PenalDecreeFishCompensationDTO> GetPenalDecreeFishCompensations(int decreeId)
        {
            List<PenalDecreeFishCompensationDTO> result = (from compensation in Db.PenalDecreeFishCompensations
                                                           where compensation.PenalDecreeRegisterId == decreeId
                                                           select new PenalDecreeFishCompensationDTO
                                                           {
                                                               Id = compensation.Id,
                                                               FishId = compensation.FishId,
                                                               Count = compensation.Count,
                                                               Weight = compensation.Weight,
                                                               UnitPrice = compensation.UnitPrice,
                                                               TotalPrice = compensation.TotalPrice,
                                                               IsActive = compensation.IsActive
                                                           }).ToList();

            return result;
        }

        private ILookup<int, PenalDecreePaymentScheduleDTO> GetPenalDecreePaymentSchedule(List<PenalDecreeStatusEditDTO> statuses)
        {
            List<int> statusIds = statuses.Where(x => x.StatusType == PenalDecreeStatusTypesEnum.Rescheduled).Select(x => x.Id.Value).ToList();

            ILookup<int, PenalDecreePaymentScheduleDTO> result = (from payment in Db.PenalDecreePaymentSchedules
                                                                  where statusIds.Contains(payment.PenalDecreeStatusId)
                                                                  select new
                                                                  {
                                                                      StatusId = payment.PenalDecreeStatusId,
                                                                      Payment = new PenalDecreePaymentScheduleDTO
                                                                      {
                                                                          Id = payment.Id,
                                                                          Date = payment.Date,
                                                                          OwedAmount = payment.OwedAmount,
                                                                          PaidAmount = payment.PaidAmount,
                                                                          IsActive = payment.IsActive
                                                                      }
                                                                  }).ToLookup(x => x.StatusId, y => y.Payment);

            return result;
        }

        private void AddSanctionTypes(PenalDecreesRegister decree, List<int> sanctionIds)
        {
            if (sanctionIds != null)
            {
                foreach (int sanctionId in sanctionIds)
                {
                    PenalDecreeSanction entry = new PenalDecreeSanction
                    {
                        PenalDecree = decree,
                        SanctionTypeId = sanctionId
                    };

                    Db.PenalDecreeSanctions.Add(entry);
                }
            }
        }

        private void AddDecreeStatus(PenalDecreesRegister decree, List<PenalDecreeStatusEditDTO> statuses)
        {
            if (statuses != null)
            {
                foreach (PenalDecreeStatusEditDTO status in statuses)
                {
                    AddDecreeStatusEntry(decree, status);
                }
            }
        }

        private void AddSeizedFish(PenalDecreesRegister decree, List<PenalDecreeSeizedFishDTO> fishes)
        {
            if (fishes != null)
            {
                foreach (PenalDecreeSeizedFishDTO fish in fishes)
                {
                    AddSeizedFishEntry(decree, fish);
                }
            }
        }

        private void AddSeizedFishingGear(PenalDecreesRegister decree, List<PenalDecreeSeizedFishingGearDTO> gears)
        {
            if (gears != null)
            {
                foreach (PenalDecreeSeizedFishingGearDTO gear in gears)
                {
                    AddSeizedFishingGearEntry(decree, gear);
                }
            }
        }

        private void AddFishCompensation(PenalDecreesRegister decree, List<PenalDecreeFishCompensationDTO> compensations)
        {
            if (compensations != null)
            {
                foreach (PenalDecreeFishCompensationDTO compensation in compensations)
                {
                    AddFishCompensationEntry(decree, compensation);
                }
            }
        }

        private void AddViolatedRegulations(PenalDecreesRegister decree, List<AuanViolatedRegulationDTO> regulations, ViolatedRegulationSectionTypesEnum sectionType)
        {
            if (regulations != null)
            {
                foreach (AuanViolatedRegulationDTO regulation in regulations)
                {
                    this.AddViolatedRegulationEntry(decree, regulation, sectionType);
                }
            }
        }

        private void AddDeliveryData(PenalDecreesRegister decree, PenalDecreeDeliveryDataDTO delivery)
        {
            InspDelivery entry = new InspDelivery
            {
                DeliveryTypeId = GetDeliveryTypeIdByCode(delivery.DeliveryType.Value),
                IsDelivered = delivery.IsDelivered.Value,
                DeliveryDate = delivery.DeliveryDate
            };

            if (delivery.IsDelivered.Value)
            {
                entry.ConfirmationTypeId = GetConfirmationTypeIdByCode(delivery.ConfirmationType.Value);
            }

            if (delivery.DeliveryType.Value == InspDeliveryTypesEnum.DecreeReturn || delivery.DeliveryType.Value == InspDeliveryTypesEnum.DecreeTag)
            {
                entry.SentDate = delivery.SentDate;
                entry.ReferenceNum = delivery.ReferenceNum;
            }
            else
            {
                EditDeliveryWitnesses(entry, delivery.RefusalWitnesses);
            }

            decree.Delivery = entry;
        }

        private void EditDecreeSanctions(int decreeId, List<int> sanctionIds)
        {
            List<PenalDecreeSanction> currentSanctions = (from sanction in Db.PenalDecreeSanctions
                                                          where sanction.PenalDecreeId == decreeId
                                                          select sanction).ToList();

            if (sanctionIds != null)
            {
                List<int> currentSanctionIds = currentSanctions.Select(x => x.SanctionTypeId).ToList();
                List<int> sanctionIdsToAdd = sanctionIds.Where(x => !currentSanctionIds.Contains(x)).ToList();
                List<int> sanctionIdsToRemove = currentSanctionIds.Where(x => !sanctionIds.Contains(x)).ToList();

                foreach (int sanctionId in sanctionIdsToAdd)
                {
                    PenalDecreeSanction sanction = currentSanctions.Where(x => x.SanctionTypeId == sanctionId).SingleOrDefault();
                    if (sanction != null)
                    {
                        sanction.IsActive = true;
                    }
                    else
                    {
                        PenalDecreeSanction entry = new PenalDecreeSanction
                        {
                            PenalDecreeId = decreeId,
                            SanctionTypeId = sanctionId
                        };

                        Db.PenalDecreeSanctions.Add(entry);
                    }
                }

                foreach (int sanctionId in sanctionIdsToRemove)
                {
                    PenalDecreeSanction sanction = currentSanctions.Where(x => x.SanctionTypeId == sanctionId).Single();
                    sanction.IsActive = false;
                }
            }
            else
            {
                foreach (PenalDecreeSanction sanction in currentSanctions)
                {
                    sanction.IsActive = false;
                }
            }
        }
        private void EditDeliveryData(PenalDecreesRegister decree, PenalDecreeDeliveryDataDTO delivery)
        {
            if (delivery.Id == null)
            {
                this.AddDeliveryData(decree, delivery);
            }
            else
            {
                InspDelivery entry = (from del in Db.InspDelivery
                                      where del.Id == delivery.Id.Value
                                      select del).FirstOrDefault();

                entry.DeliveryTypeId = GetDeliveryTypeIdByCode(delivery.DeliveryType.Value);
                entry.IsDelivered = delivery.IsDelivered.Value;
                entry.DeliveryDate = delivery.DeliveryDate;

                if (delivery.IsDelivered.Value)
                {
                    entry.ConfirmationTypeId = GetConfirmationTypeIdByCode(delivery.ConfirmationType.Value);
                }

                if (delivery.DeliveryType.Value == InspDeliveryTypesEnum.DecreeReturn || delivery.DeliveryType.Value == InspDeliveryTypesEnum.DecreeTag)
                {
                    entry.SentDate = delivery.SentDate;
                    entry.ReferenceNum = delivery.ReferenceNum;
                }
                else
                {
                    this.EditDeliveryWitnesses(entry, delivery.RefusalWitnesses);
                }
            }
        }

        private void EditDecreeStatuses(PenalDecreesRegister decree, List<PenalDecreeStatusEditDTO> statuses)
        {
            if (statuses != null)
            {
                List<PenalDecreeStatus> dbStatuses = statuses.Any(x => x.Id != null)
                    ? Db.PenalDecreeStatuses.Where(x => x.PenalDecreeId == decree.Id).ToList()
                    : new List<PenalDecreeStatus>();

                foreach (PenalDecreeStatusEditDTO status in statuses)
                {
                    if (status.Id == null)
                    {
                        AddDecreeStatusEntry(decree, status);
                    }
                    else
                    {
                        Dictionary<PenalDecreeStatusTypesEnum, int> statusTypes = GetStatusTypesCodeToIdDictionary();

                        PenalDecreeStatus dbStatus = dbStatuses.Where(x => x.Id == status.Id).Single();
                        dbStatus.StatusTypeId = statusTypes[status.StatusType.Value];
                        dbStatus.CourtId = status.CourtId;
                        dbStatus.AppealDate = status.AppealDate;
                        dbStatus.CaseNum = status.CaseNum;
                        dbStatus.ComplaintDueDate = status.ComplaintDueDate;
                        dbStatus.RemunerationAmount = status.RemunerationAmount;
                        dbStatus.EnactmentDate = status.EnactmentDate;
                        dbStatus.PenalAuthorityTypeId = status.PenalAuthorityTypeId;
                        dbStatus.PenalAuthorityName = status.PenalAuthorityName;
                        dbStatus.Amendments = status.Amendments;
                        dbStatus.ConfiscationInsitutionId = status.ConfiscationInstitutionId;
                        dbStatus.PaidAmount = status.PaidAmount;
                        dbStatus.IsActive = status.IsActive.Value;

                        if (status.StatusType == PenalDecreeStatusTypesEnum.Rescheduled)
                        {
                            EditPenalDecreePaymentSchedule(dbStatus, status.PaymentSchedule);
                        }
                    }
                }
            }
            else
            {
                List<PenalDecreeStatus> dbStatuses = Db.PenalDecreeStatuses.Where(x => x.PenalDecreeId == decree.Id).ToList();

                foreach (PenalDecreeStatus status in dbStatuses)
                {
                    status.IsActive = false;
                }
            }
        }

        private void EditSeizedFish(PenalDecreesRegister decree, List<PenalDecreeSeizedFishDTO> fishes)
        {
            if (fishes != null)
            {
                List<AuanconfiscatedFish> dbFishes = fishes.Any(x => x.Id != null)
                    ? Db.AuanConfiscatedFishes.Where(x => x.PenalDecreeRegisterId == decree.Id).ToList()
                    : new List<AuanconfiscatedFish>();

                foreach (PenalDecreeSeizedFishDTO fish in fishes)
                {
                    if (fish.Id == null)
                    {
                        AddSeizedFishEntry(decree, fish);
                    }
                    else
                    {
                        AuanconfiscatedFish dbSeizedFish = dbFishes.Where(x => x.Id == fish.Id).Single();
                        dbSeizedFish.FishId = fish.FishTypeId;
                        dbSeizedFish.Weight = fish.Weight;
                        dbSeizedFish.Count = fish.Count;
                        dbSeizedFish.Comments = fish.Comments;
                        dbSeizedFish.ConfiscationActionId = fish.ConfiscationActionId.Value;
                        dbSeizedFish.ApplicanceId = fish.ApplianceId;
                        dbSeizedFish.TurbotSizeGroupId = fish.TurbotSizeGroupId;
                        dbSeizedFish.TerritoryUnitId = fish.TerritoryUnitId;
                        dbSeizedFish.IsActive = fish.IsActive.Value;
                    }
                }
            }
            else
            {
                List<AuanconfiscatedFish> dbFishes = Db.AuanConfiscatedFishes.Where(x => x.PenalDecreeRegisterId == decree.Id).ToList();

                foreach (AuanconfiscatedFish fish in dbFishes)
                {
                    fish.IsActive = false;
                }
            }
        }

        private void EditSeizedFishingGear(PenalDecreesRegister decree, List<PenalDecreeSeizedFishingGearDTO> gears)
        {
            if (gears != null)
            {
                List<PenalDecreeSeizedFishingGear> dbFishingGears = gears.Any(x => x.Id != null)
                    ? Db.PenalDecreeSeizedFishingGears.Where(x => x.PenalDecreeId == decree.Id).ToList()
                    : new List<PenalDecreeSeizedFishingGear>();

                foreach (PenalDecreeSeizedFishingGearDTO gear in gears)
                {
                    if (gear.Id == null)
                    {
                        AddSeizedFishingGearEntry(decree, gear);
                    }
                    else
                    {
                        PenalDecreeSeizedFishingGear dbSizedFishingGear = dbFishingGears.Where(x => x.Id == gear.Id).Single();
                        dbSizedFishingGear.FishingGearId = gear.FishingGearId.Value;
                        dbSizedFishingGear.Count = gear.Count.Value;
                        dbSizedFishingGear.ConfiscationActionId = gear.ConfiscationActionId.Value;
                        dbSizedFishingGear.Comments = gear.Comments;
                        dbSizedFishingGear.TerritoryUnitId = gear.StorageTerritoryUnitId;
                        dbSizedFishingGear.IsActive = gear.IsActive.Value;
                    }
                }
            }
            else
            {
                List<PenalDecreeSeizedFishingGear> dbFishingGears = Db.PenalDecreeSeizedFishingGears.Where(x => x.PenalDecreeId == decree.Id).ToList();

                foreach (PenalDecreeSeizedFishingGear gear in dbFishingGears)
                {
                    gear.IsActive = false;
                }
            }
        }

        private void EditFishCompensation(PenalDecreesRegister decree, List<PenalDecreeFishCompensationDTO> compensations)
        {
            if (compensations != null)
            {
                List<PenalDecreeFishCompensation> dbCompensations = compensations.Any(x => x.Id != null)
                    ? Db.PenalDecreeFishCompensations.Where(x => x.PenalDecreeRegisterId == decree.Id).ToList()
                    : new List<PenalDecreeFishCompensation>();

                foreach (PenalDecreeFishCompensationDTO compensation in compensations)
                {
                    if (compensation.Id == null)
                    {
                        AddFishCompensationEntry(decree, compensation);
                    }
                    else
                    {
                        PenalDecreeFishCompensation dbFishCompensation = dbCompensations.Where(x => x.Id == compensation.Id).Single();
                        dbFishCompensation.FishId = compensation.FishId.Value;
                        dbFishCompensation.Count = compensation.Count;
                        dbFishCompensation.Weight = compensation.Weight;
                        dbFishCompensation.UnitPrice = compensation.UnitPrice.Value;
                        dbFishCompensation.TotalPrice = compensation.TotalPrice.Value;
                        dbFishCompensation.IsActive = compensation.IsActive.Value;
                    }
                }
            }
            else
            {
                List<PenalDecreeFishCompensation> dbCompensations = Db.PenalDecreeFishCompensations.Where(x => x.PenalDecreeRegisterId == decree.Id).ToList();

                foreach (PenalDecreeFishCompensation compensation in dbCompensations)
                {
                    compensation.IsActive = false;
                }
            }
        }

        private void EditViolatedRegulations(PenalDecreesRegister decree, List<AuanViolatedRegulationDTO> regulations, ViolatedRegulationSectionTypesEnum sectionType)
        {
            if (regulations != null)
            {
                List<AuanviolatedRegulation> dbRegulations = regulations.Any(x => x.Id != null)
                    ? this.Db.AuanViolatedRegulations.Where(x => x.PenalDecreeRegisterId == decree.Id && x.SectionType == sectionType.ToString()).ToList()
                    : new List<AuanviolatedRegulation>();

                foreach (AuanViolatedRegulationDTO regulation in regulations)
                {
                    if (regulation.Id == null)
                    {
                        this.AddViolatedRegulationEntry(decree, regulation, sectionType);
                    }
                    else
                    {
                        AuanviolatedRegulation dbRegulation = dbRegulations.Where(x => x.Id == regulation.Id).Single();
                        dbRegulation.Article = regulation.Article;
                        dbRegulation.Paragraph = regulation.Paragraph;
                        dbRegulation.Section = regulation.Section;
                        dbRegulation.Letter = regulation.Letter;
                        dbRegulation.RegulationType = regulation.Type.ToString();
                        dbRegulation.SectionType = sectionType.ToString();
                        dbRegulation.IsActive = regulation.IsActive.Value;
                    }
                }
            }
            else
            {
                List<AuanviolatedRegulation> dbRegulations = this.Db.AuanViolatedRegulations
                    .Where(x => x.PenalDecreeRegisterId == decree.Id && x.SectionType == sectionType.ToString()).ToList();

                foreach (AuanviolatedRegulation regulation in dbRegulations)
                {
                    regulation.IsActive = false;
                }
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

        private void EditPenalDecreePaymentSchedule(PenalDecreeStatus status, List<PenalDecreePaymentScheduleDTO> payments)
        {
            if (payments != null)
            {
                List<PenalDecreePaymentSchedule> dbPayments = payments.Any(x => x.Id != null)
                    ? Db.PenalDecreePaymentSchedules.Where(x => x.PenalDecreeStatusId == status.Id).ToList()
                    : new List<PenalDecreePaymentSchedule>();

                foreach (PenalDecreePaymentScheduleDTO payment in payments)
                {
                    if (payment.Id == null)
                    {
                        AddPaymentScheduleEntry(status, payment);
                    }
                    else
                    {
                        PenalDecreePaymentSchedule dbPayment = dbPayments.Where(x => x.Id == payment.Id).Single();
                        dbPayment.OwedAmount = payment.OwedAmount.Value;
                        dbPayment.PaidAmount = payment.PaidAmount;
                        dbPayment.Date = payment.Date.Value;
                        dbPayment.IsActive = payment.IsActive.Value;
                    }
                }
            }
            else
            {
                List<PenalDecreePaymentSchedule> dbPayments = Db.PenalDecreePaymentSchedules.Where(x => x.PenalDecreeStatusId == status.Id).ToList();

                foreach (PenalDecreePaymentSchedule payment in dbPayments)
                {
                    payment.IsActive = false;
                }
            }
        }

        private void AddDecreeStatusEntry(PenalDecreesRegister decree, PenalDecreeStatusEditDTO status)
        {
            Dictionary<PenalDecreeStatusTypesEnum, int> types = GetStatusTypesCodeToIdDictionary();

            PenalDecreeStatus entry = new PenalDecreeStatus
            {
                PenalDecree = decree,
                StatusTypeId = types[status.StatusType.Value],
                CourtId = status.CourtId,
                AppealDate = status.AppealDate,
                CaseNum = status.CaseNum,
                ComplaintDueDate = status.ComplaintDueDate,
                RemunerationAmount = status.RemunerationAmount,
                EnactmentDate = status.EnactmentDate,
                PenalAuthorityTypeId = status.PenalAuthorityTypeId,
                PenalAuthorityName = status.PenalAuthorityName,
                Amendments = status.Amendments,
                ConfiscationInsitutionId = status.ConfiscationInstitutionId,
                PaidAmount = status.PaidAmount,
                IsActive = status.IsActive.Value
            };

            if (status.StatusType == PenalDecreeStatusTypesEnum.Rescheduled)
            {
                EditPenalDecreePaymentSchedule(entry, status.PaymentSchedule);
            }

            Db.PenalDecreeStatuses.Add(entry);
        }

        private void AddSeizedFishEntry(PenalDecreesRegister decree, PenalDecreeSeizedFishDTO fish)
        {
            AuanconfiscatedFish entry = new AuanconfiscatedFish
            {
                PenalDecreeRegister = decree,
                FishId = fish.FishTypeId,
                Weight = fish.Weight,
                Count = fish.Count,
                ConfiscationActionId = fish.ConfiscationActionId.Value,
                ApplicanceId = fish.ApplianceId,
                TurbotSizeGroupId = fish.TurbotSizeGroupId,
                Comments = fish.Comments,
                TerritoryUnitId = fish.StorageTerritoryUnitId,
                IsActive = fish.IsActive.Value
            };

            Db.AuanConfiscatedFishes.Add(entry);
        }

        private void AddSeizedFishingGearEntry(PenalDecreesRegister decree, PenalDecreeSeizedFishingGearDTO gear)
        {
            PenalDecreeSeizedFishingGear entry = new PenalDecreeSeizedFishingGear
            {
                PenalDecree = decree,
                FishingGearId = gear.FishingGearId.Value,
                Count = gear.Count.Value,
                ConfiscationActionId = gear.ConfiscationActionId.Value,
                Comments = gear.Comments,
                TerritoryUnitId = gear.StorageTerritoryUnitId,
                IsActive = gear.IsActive.Value
            };

            Db.PenalDecreeSeizedFishingGears.Add(entry);
        }

        private void AddFishCompensationEntry(PenalDecreesRegister decree, PenalDecreeFishCompensationDTO compensation)
        {
            PenalDecreeFishCompensation entry = new PenalDecreeFishCompensation
            {
                PenalDecreeRegister = decree,
                FishId = compensation.FishId.Value,
                Count = compensation.Count,
                Weight = compensation.Weight,
                UnitPrice = compensation.UnitPrice.Value,
                TotalPrice = compensation.TotalPrice.Value,
                IsActive = compensation.IsActive.Value
            };

            Db.PenalDecreeFishCompensations.Add(entry);
        }

        private void AddViolatedRegulationEntry(PenalDecreesRegister decree, AuanViolatedRegulationDTO regulation, ViolatedRegulationSectionTypesEnum sectionType)
        {
            AuanviolatedRegulation entry = new AuanviolatedRegulation
            {
                PenalDecreeRegister = decree,
                Article = regulation.Article,
                Paragraph = regulation.Paragraph,
                Section = regulation.Section,
                Letter = regulation.Letter,
                RegulationType = regulation.Type.ToString(),
                SectionType = sectionType.ToString(),
                IsActive = regulation.IsActive.Value
            };

            this.Db.AuanViolatedRegulations.Add(entry);
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

        private void AddPaymentScheduleEntry(PenalDecreeStatus status, PenalDecreePaymentScheduleDTO payment)
        {
            PenalDecreePaymentSchedule entry = new PenalDecreePaymentSchedule
            {
                PenalDecreeStatus = status,
                Date = payment.Date.Value,
                OwedAmount = payment.OwedAmount.Value,
                PaidAmount = payment.PaidAmount,
                IsActive = payment.IsActive.Value
            };

            Db.PenalDecreePaymentSchedules.Add(entry);
        }

        private int GetDeliveryTypeIdByCode(InspDeliveryTypesEnum deliveryType)
        {
            int deliveryTypeId = (from type in Db.NinspDeliveryTypes
                                  where type.Code == deliveryType.ToString()
                                  select type.Id).Single();

            return deliveryTypeId;
        }
        private int GetConfirmationTypeIdByCode(InspDeliveryConfirmationTypesEnum confirmationType)
        {
            int deliveryTypeId = (from type in Db.NinspDeliveryConfirmationTypes
                                  where type.Code == confirmationType.ToString()
                                  select type.Id).Single();

            return deliveryTypeId;
        }

        private Dictionary<PenalDecreeStatusTypesEnum, int> GetStatusTypesCodeToIdDictionary()
        {
            Dictionary<PenalDecreeStatusTypesEnum, int> result = (from type in Db.NpenalDecreeStatusTypes
                                                                  select new
                                                                  {
                                                                      Type = Enum.Parse<PenalDecreeStatusTypesEnum>(type.Code),
                                                                      type.Id
                                                                  }).ToDictionary(x => x.Type, y => y.Id);

            return result;
        }
    }
}
