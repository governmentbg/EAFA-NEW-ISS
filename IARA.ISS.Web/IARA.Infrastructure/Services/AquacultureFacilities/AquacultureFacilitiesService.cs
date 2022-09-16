using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.AquacultureFacilities.Installations;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Excel.Tools.Interfaces;
using IARA.Excel.Tools.Models;
using IARA.Infrastructure.Helpers;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.Applications;
using IARA.Interfaces.CatchSales;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Legals;
using IARA.Interfaces.Reports;
using IARA.RegixAbstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace IARA.Infrastructure.Services
{
    public partial class AquacultureFacilitiesService : Service, IAquacultureFacilitiesService
    {
        private readonly IApplicationService applicationService;
        private readonly IDeliveryService deliveryService;
        private readonly IChangeOfCircumstancesService changeOfCircumstancesService;
        private readonly IUsageDocumentsService usageDocumentsService;
        private readonly IApplicationStateMachine stateMachine;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly ILogBooksService logBooksService;
        private readonly IJasperReportExecutionService jasperReportExecutionService;
        private readonly IExcelExporterService excelExporterService;
        private readonly ILegalService legalService;
        private readonly IPersonService personService;

        public AquacultureFacilitiesService(IARADbContext db,
                                            IApplicationService applicationService,
                                            IDeliveryService deliveryService,
                                            IChangeOfCircumstancesService changeOfCircumstancesService,
                                            IUsageDocumentsService usageDocumentsService,
                                            IApplicationStateMachine stateMachine,
                                            IRegixApplicationInterfaceService regixApplicationService,
                                            ILogBooksService logBooksService,
                                            IJasperReportExecutionService jasperReportExecutionService,
                                            IExcelExporterService excelExporterService,
                                            ILegalService legalService,
                                            IPersonService personService)
            : base(db)
        {
            this.applicationService = applicationService;
            this.deliveryService = deliveryService;
            this.changeOfCircumstancesService = changeOfCircumstancesService;
            this.usageDocumentsService = usageDocumentsService;
            this.stateMachine = stateMachine;
            this.regixApplicationService = regixApplicationService;
            this.logBooksService = logBooksService;
            this.jasperReportExecutionService = jasperReportExecutionService;
            this.excelExporterService = excelExporterService;
            this.legalService = legalService;
            this.personService = personService;
        }

        public IQueryable<AquacultureFacilityDTO> GetAllAquacultures(AquacultureFacilitiesFilters filters)
        {
            IQueryable<AquacultureFacilityDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllAquacultures(showInactive);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredAquacultures(filters.FreeTextSearch, filters.ShowInactiveRecords, filters.TerritoryUnitId)
                    : GetParametersFilteredAquacultures(filters);
            }

            return result;
        }

        public AquacultureFacilityEditDTO GetAquaculture(int id)
        {
            AquacultureFacilityRegister dbAquaculture = (from aquaculture in Db.AquacultureFacilitiesRegister
                                                            .AsSplitQuery()
                                                            .Include(x => x.AquacultureStatus)
                                                         where aquaculture.Id == id
                                                         select aquaculture).First();

            AquacultureFacilityEditDTO result = new AquacultureFacilityEditDTO
            {
                Id = dbAquaculture.Id,
                ApplicationId = dbAquaculture.ApplicationId,
                Status = Enum.Parse<AquacultureStatusEnum>(dbAquaculture.AquacultureStatus.Code),
                RegNum = dbAquaculture.RegNum.Value,
                UrorNum = dbAquaculture.UrorNum,
                Name = dbAquaculture.Name,
                TerritoryUnitId = dbAquaculture.TerritoryUnitId,
                WaterAreaTypeId = dbAquaculture.WaterAreaTypeId,
                PopulatedAreaId = dbAquaculture.PopulatedAreaId,
                LocationDescription = dbAquaculture.LocationDescription,
                WaterSalinity = Enum.Parse<AquacultureSalinityEnum>(dbAquaculture.WaterSalinity),
                WaterTemperature = Enum.Parse<AquacultureTemperatureEnum>(dbAquaculture.WaterTemperature),
                System = Enum.Parse<AquacultureSystemEnum>(dbAquaculture.System),
                PowerSupplyTypeId = dbAquaculture.PowerSupplyTypeId,
                PowerSupplyName = dbAquaculture.PowerSupplyName,
                PowerSupplyDebit = dbAquaculture.PowerSupplyDebit,
                TotalWaterArea = dbAquaculture.TotalWaterArea,
                TotalProductionCapacity = dbAquaculture.TotalProductionCapacity,
                Comments = dbAquaculture.Comments
            };

            result.SubmittedFor = applicationService.GetRegisterSubmittedFor(dbAquaculture.ApplicationId, dbAquaculture.SubmittedForPersonId, dbAquaculture.SubmittedForLegalId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(dbAquaculture.ApplicationId, ApplicationHierarchyTypesEnum.Online);
            result.Coordinates = GetAquacultureCoordinates(dbAquaculture.Id);
            result.AquaticOrganismIds = GetAquacultureAquaticOrganismIds(dbAquaculture.Id);
            result.Installations = GetAquacultureInstallations(dbAquaculture.Id);
            result.UsageDocuments = GetAquacultureUsageDocuments(dbAquaculture.Id);
            result.WaterLawCertificates = GetAquacultureWaterLawCertificates(dbAquaculture.Id);
            result.OvosCertificates = GetAquacultureOvosCertificates(dbAquaculture.Id);
            result.BabhCertificates = GetAquacultureBabhCertificates(dbAquaculture.Id);
            result.CancellationHistory = GetAquacultureStatuses(dbAquaculture.Id);

            if (result.System == AquacultureSystemEnum.FullSystem)
            {
                result.HatcheryCapacity = dbAquaculture.HatcheryCapacity;
                result.HatcheryEquipment = GetAquacultureHatcheryEquipment(dbAquaculture.Id);
            }

            result.LogBooks = logBooksService.GetAquacultureFacilityLogBooks(dbAquaculture.Id);
            result.Files = Db.GetFiles(Db.AquacultureFacilityFiles, dbAquaculture.Id);

            return result;
        }

        public int AddAquaculture(AquacultureFacilityEditDTO aquaculture, bool ignoreLogBookConflicts)
        {
            AquacultureFacilityRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                int registerApplicationId = (from aq in Db.AquacultureFacilitiesRegister
                                             where aq.ApplicationId == aquaculture.ApplicationId
                                                   && aq.RecordType == nameof(RecordTypesEnum.Application)
                                             select aq.Id).First();

                entry = new AquacultureFacilityRegister
                {
                    ApplicationId = aquaculture.ApplicationId.Value,
                    RegisterApplicationId = registerApplicationId,
                    RecordType = nameof(RecordTypesEnum.Register),
                    AquacultureStatusId = GetAquacultureStatusId(aquaculture.Status.Value),
                    RegistrationDate = DateTime.Now,
                    Name = aquaculture.Name,
                    TerritoryUnitId = aquaculture.TerritoryUnitId.Value,
                    WaterAreaTypeId = aquaculture.WaterAreaTypeId.Value,
                    PopulatedAreaId = aquaculture.PopulatedAreaId,
                    LocationDescription = aquaculture.LocationDescription,
                    WaterSalinity = aquaculture.WaterSalinity.ToString(),
                    WaterTemperature = aquaculture.WaterTemperature.ToString(),
                    System = aquaculture.System.ToString(),
                    PowerSupplyTypeId = aquaculture.PowerSupplyTypeId.Value,
                    PowerSupplyName = aquaculture.PowerSupplyName,
                    PowerSupplyDebit = aquaculture.PowerSupplyDebit,
                    TotalWaterArea = aquaculture.TotalWaterArea.Value,
                    TotalProductionCapacity = aquaculture.TotalProductionCapacity.Value,
                    HatcheryCapacity = aquaculture.System == AquacultureSystemEnum.FullSystem ? aquaculture.HatcheryCapacity.Value : null,
                    Comments = aquaculture.Comments
                };

                entry.UrorNum = GenerateUrorNum(entry.TerritoryUnitId, entry.WaterAreaTypeId);
                Db.AquacultureFacilitiesRegister.Add(entry);

                Db.AddOrEditRegisterSubmittedFor(entry, aquaculture.SubmittedFor);
                Db.SaveChanges();

                AddAquacultureCoordinates(entry, aquaculture.Coordinates);
                AddAquaticOrganisms(entry, aquaculture.AquaticOrganismIds);
                AddAquacultureInstallations(entry, aquaculture.Installations);
                AddWaterLawCertificates(entry, aquaculture.WaterLawCertificates);
                AddOvosCertificates(entry, aquaculture.OvosCertificates);
                AddBabhCertificates(entry, aquaculture.BabhCertificates);

                if (aquaculture.System == AquacultureSystemEnum.FullSystem)
                {
                    AddAquacultureHatcheryEquipment(entry, aquaculture.HatcheryEquipment);
                }

                AddAquacultureLogBooks(entry.Id, aquaculture.LogBooks, ignoreLogBookConflicts);

                if (aquaculture.Files != null)
                {
                    foreach (FileInfoDTO file in aquaculture.Files)
                    {
                        Db.AddOrEditFile(entry, entry.AquacultureFacilityRegisterFiles, file);
                    }
                }

                AddUsageDocuments(entry, aquaculture.UsageDocuments);
                Db.SaveChanges();

                stateMachine.Act(entry.ApplicationId);

                scope.Complete();
            }

            return entry.Id;
        }

        public void EditAquaculture(AquacultureFacilityEditDTO aquaculture, bool ignoreLogBookConflicts)
        {
            using TransactionScope scope = new TransactionScope();

            AquacultureFacilityRegister dbAquaculture = (from aq in Db.AquacultureFacilitiesRegister
                                                            .AsSplitQuery()
                                                            .Include(x => x.AquacultureFacilityRegisterFiles)
                                                         where aq.Id == aquaculture.Id
                                                         select aq).First();

            Db.AddOrEditRegisterSubmittedFor(dbAquaculture, aquaculture.SubmittedFor);
            Db.SaveChanges();

            dbAquaculture.AquacultureStatusId = GetAquacultureStatusId(aquaculture.Status.Value);
            dbAquaculture.Name = aquaculture.Name;
            dbAquaculture.PopulatedAreaId = aquaculture.PopulatedAreaId;
            dbAquaculture.LocationDescription = aquaculture.LocationDescription;
            dbAquaculture.WaterSalinity = aquaculture.WaterSalinity.ToString();
            dbAquaculture.WaterTemperature = aquaculture.WaterTemperature.ToString();
            dbAquaculture.System = aquaculture.System.ToString();
            dbAquaculture.PowerSupplyTypeId = aquaculture.PowerSupplyTypeId.Value;
            dbAquaculture.PowerSupplyName = aquaculture.PowerSupplyName;
            dbAquaculture.PowerSupplyDebit = aquaculture.PowerSupplyDebit;
            dbAquaculture.TotalWaterArea = aquaculture.TotalWaterArea.Value;
            dbAquaculture.TotalProductionCapacity = aquaculture.TotalProductionCapacity.Value;
            dbAquaculture.HatcheryCapacity = aquaculture.System.Value == AquacultureSystemEnum.FullSystem ? aquaculture.HatcheryCapacity.Value : null;
            dbAquaculture.Comments = aquaculture.Comments;

            EditAquacultureCoordinates(dbAquaculture.Id, aquaculture.Coordinates);
            EditAquacultureAquaticOrganisms(dbAquaculture.Id, aquaculture.AquaticOrganismIds);
            EditAquacultureInstallations(dbAquaculture, aquaculture.Installations);
            EditAquacultureHatcheryEquipment(dbAquaculture.Id, aquaculture.HatcheryEquipment, aquaculture.System.Value);
            EditUsageDocuments(dbAquaculture, aquaculture.UsageDocuments);
            EditWaterLawCertificates(dbAquaculture.Id, aquaculture.WaterLawCertificates);
            EditOvosCertificates(dbAquaculture.Id, aquaculture.OvosCertificates);
            EditBabhCertificates(dbAquaculture.Id, aquaculture.BabhCertificates);
            EditAquacultureLogBooks(dbAquaculture.Id, aquaculture.LogBooks, ignoreLogBookConflicts);

            if (aquaculture.Files != null)
            {
                foreach (FileInfoDTO file in aquaculture.Files)
                {
                    Db.AddOrEditFile(dbAquaculture, dbAquaculture.AquacultureFacilityRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            scope.Complete();
        }

        public void UpdateAquacultureStatus(int aquacultureId, CancellationHistoryEntryDTO status, int? applicationId)
        {
            DateTime now = DateTime.Now;

            AquacultureFacilityStatus lastStatus = (from st in Db.AquacultureFacilityStatuses
                                                    where st.AquacultureFacilityId == aquacultureId
                                                        && st.ValidFrom <= now
                                                        && st.ValidTo > now
                                                    select st).SingleOrDefault();

            if (lastStatus != null)
            {
                lastStatus.ValidTo = now;
            }

            AquacultureFacilityStatus entry = new AquacultureFacilityStatus
            {
                AquacultureFacilityId = aquacultureId,
                IsCancelled = status.IsCancelled.Value,
                CancellationReasonId = status.CancellationReasonId.Value,
                DateOfChange = now,
                IssueOrderNum = status.IssueOrderNum,
                Description = status.Description,
                ValidFrom = now,
                ValidTo = DefaultConstants.MAX_VALID_DATE
            };

            AquacultureFacilityRegister aquaculture = (from aqua in Db.AquacultureFacilitiesRegister
                                                       where aqua.Id == aquacultureId
                                                       select aqua).First();

            if (status.IsCancelled.Value)
            {
                aquaculture.AquacultureStatusId = GetAquacultureStatusId(AquacultureStatusEnum.Canceled);
            }
            else
            {
                aquaculture.AquacultureStatusId = GetAquacultureStatusId(AquacultureStatusEnum.Approved);
            }

            Db.AquacultureFacilityStatuses.Add(entry);

            // complete deregistration application
            if (applicationId.HasValue)
            {
                stateMachine.Act(applicationId.Value);
            }

            Db.SaveChanges();
        }

        public void DeleteAquaculture(int id)
        {
            DeleteRecordWithId(Db.AquacultureFacilitiesRegister, id);
            Db.SaveChanges();
        }

        public void UndoDeleteAquaculture(int id)
        {
            UndoDeleteRecordWithId(Db.AquacultureFacilitiesRegister, id);
            Db.SaveChanges();
        }

        public Task<byte[]> DownloadAquacultureFacility(int aquacultureId)
        {
            return jasperReportExecutionService.GetAquacultureRegister(aquacultureId);
        }

        public Stream DownloadAquacultureFacilitiesExcel(ExcelExporterRequestModel<AquacultureFacilitiesFilters> request)
        {
            ExcelExporterData<AquacultureFacilityDTO> data = new ExcelExporterData<AquacultureFacilityDTO>
            {
                PrimaryKey = nameof(AquacultureFacilityDTO.Id),
                Query = GetAllAquacultures(request.Filters),
                HeaderNames = request.HeaderNames
            };

            return excelExporterService.BuildExcelFile(request, data);
        }

        public SimpleAuditDTO GetAquacultureInstallationSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.AquacultureFacilityInstallations, id);
            return audit;
        }

        public SimpleAuditDTO GetAquacultureInstallationNetCageAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.AquacultureInstallationNetCages, id);
            return audit;
        }

        public SimpleAuditDTO GetAquacultureUsageDocumentSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.UsageDocuments, id);
            return audit;
        }

        public SimpleAuditDTO GetAquacultureWaterLawCertificateSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.AquacultureWaterLawCertificates, id);
            return audit;
        }

        public SimpleAuditDTO GetAquacultureOvosCertificateSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.AquacultureOvosCertificates, id);
            return audit;
        }

        public SimpleAuditDTO GetAquacultureBabhCertificateSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.AquacultureBabhCertificates, id);
            return audit;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO simple = GetSimpleEntityAuditValues(Db.AquacultureFacilitiesRegister, id);
            return simple;
        }

        private IQueryable<AquacultureFacilityDTO> GetAllAquacultures(bool showInactive)
        {
            IQueryable<AquacultureFacilityDTO> result = from aqua in Db.AquacultureFacilitiesRegister
                                                        join appl in Db.Applications on aqua.ApplicationId equals appl.Id
                                                        join status in Db.NaquacultureStatuses on aqua.AquacultureStatusId equals status.Id
                                                        join legal in Db.Legals on aqua.SubmittedForLegalId equals legal.Id into leg
                                                        from legal in leg.DefaultIfEmpty()
                                                        join person in Db.Persons on aqua.SubmittedForPersonId equals person.Id into per
                                                        from person in per.DefaultIfEmpty()
                                                        join tu in Db.NterritoryUnits on aqua.TerritoryUnitId equals tu.Id
                                                        where aqua.RecordType == nameof(RecordTypesEnum.Register)
                                                            && aqua.IsActive == !showInactive
                                                        orderby aqua.RegNum.Value descending
                                                        select new AquacultureFacilityDTO
                                                        {
                                                            Id = aqua.Id,
                                                            ApplicationId = aqua.ApplicationId,
                                                            RegNum = aqua.RegNum.Value,
                                                            UrorNum = aqua.UrorNum,
                                                            RegistrationDate = aqua.RegistrationDate,
                                                            Name = aqua.Name,
                                                            Owner = legal != null ? legal.Name : person.FirstName + " " + person.LastName,
                                                            TerritoryUnit = tu.Name,
                                                            Status = Enum.Parse<AquacultureStatusEnum>(status.Code),
                                                            StatusName = status.Name,
                                                            DeliveryId = appl.DeliveryId,
                                                            IsActive = aqua.IsActive
                                                        };

            return result;
        }

        private IQueryable<AquacultureFacilityDTO> GetParametersFilteredAquacultures(AquacultureFacilitiesFilters filters)
        {
            var aquacultures = from aqua in Db.AquacultureFacilitiesRegister
                               join appl in Db.Applications on aqua.ApplicationId equals appl.Id
                               join status in Db.NaquacultureStatuses on aqua.AquacultureStatusId equals status.Id
                               where aqua.RecordType == nameof(RecordTypesEnum.Register)
                                    && aqua.IsActive == !filters.ShowInactiveRecords
                               select new
                               {
                                   aqua.Id,
                                   aqua.ApplicationId,
                                   aqua.RegNum,
                                   aqua.UrorNum,
                                   aqua.Name,
                                   aqua.RegistrationDate,
                                   aqua.TerritoryUnitId,
                                   aqua.PopulatedAreaId,
                                   aqua.WaterAreaTypeId,
                                   aqua.WaterSalinity,
                                   aqua.WaterTemperature,
                                   aqua.System,
                                   aqua.PowerSupplyTypeId,
                                   aqua.TotalWaterArea,
                                   aqua.TotalProductionCapacity,
                                   aqua.LocationDescription,
                                   aqua.SubmittedForLegalId,
                                   aqua.SubmittedForPersonId,
                                   StatusId = status.Id,
                                   Status = Enum.Parse<AquacultureStatusEnum>(status.Code),
                                   StatusName = status.Name,
                                   appl.DeliveryId,
                                   aqua.IsActive
                               };

            if (!string.IsNullOrEmpty(filters.RegNum))
            {
                aquacultures = aquacultures.Where(x => x.RegNum.ToString() == filters.RegNum.ToLower());
            }

            if (!string.IsNullOrEmpty(filters.UrorNum))
            {
                aquacultures = aquacultures.Where(x => x.UrorNum.Replace(" ", "").Contains(filters.UrorNum.Replace(" ", "")));
            }

            if (!string.IsNullOrEmpty(filters.Name))
            {
                aquacultures = aquacultures.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Eik))
            {
                aquacultures = from aqua in aquacultures
                               join legal in Db.Legals on aqua.SubmittedForLegalId equals legal.Id
                               where legal.Eik.ToLower().Contains(filters.Eik.ToLower())
                               select aqua;
            }

            if (filters.RegistrationDateFrom.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.RegistrationDate >= filters.RegistrationDateFrom.Value);
            }

            if (filters.RegistrationDateTo.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.RegistrationDate <= filters.RegistrationDateTo.Value);
            }

            if (filters.StatusIds != null && filters.StatusIds.Count > 0)
            {
                aquacultures = aquacultures.Where(x => filters.StatusIds.Contains(x.StatusId));
            }

            if (filters.TerritoryUnitId.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.TerritoryUnitId == filters.TerritoryUnitId.Value);
            }

            if (filters.WaterAreaTypeIds != null && filters.WaterAreaTypeIds.Count > 0)
            {
                aquacultures = aquacultures.Where(x => filters.WaterAreaTypeIds.Contains(x.WaterAreaTypeId));
            }

            if (filters.PopulatedAreaId.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.PopulatedAreaId == filters.PopulatedAreaId.Value);
            }

            if (!string.IsNullOrEmpty(filters.Location))
            {
                aquacultures = aquacultures.Where(x => x.LocationDescription.ToLower().Contains(filters.Location.ToLower()));
            }

            if (filters.WaterSalinityTypes != null && filters.WaterSalinityTypes.Count > 0)
            {
                aquacultures = aquacultures.Where(x => filters.WaterSalinityTypes.Contains(x.WaterSalinity));
            }

            if (filters.WaterTemperatureTypes != null && filters.WaterTemperatureTypes.Count > 0)
            {
                aquacultures = aquacultures.Where(x => filters.WaterTemperatureTypes.Contains(x.WaterTemperature));
            }

            if (filters.SystemTypes != null && filters.SystemTypes.Count > 0)
            {
                aquacultures = aquacultures.Where(x => filters.SystemTypes.Contains(x.System));
            }

            if (filters.AquaticOrganismId.HasValue)
            {
                aquacultures = from aqua in aquacultures
                               join aquaFish in Db.AquacultureFacilityFishes on aqua.Id equals aquaFish.AquacultureFacilityId
                               where filters.AquaticOrganismId == aquaFish.FishTypeId
                                    && aquaFish.IsActive
                               select aqua;
            }

            if (filters.PowerSupplyTypeId.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.PowerSupplyTypeId == filters.PowerSupplyTypeId.Value);
            }

            if (filters.InstallationTypeIds != null && filters.InstallationTypeIds.Count > 0)
            {
                List<int> aquacultureIds = (from aquaInstall in Db.AquacultureFacilityInstallations
                                            where filters.InstallationTypeIds.Contains(aquaInstall.InstallationTypeId)
                                                && aquaInstall.IsActive
                                            select aquaInstall.AquacultureFacilityId).ToList();

                aquacultures = aquacultures.Where(x => aquacultureIds.Contains(x.Id));
            }

            if (filters.TotalWaterAreaFrom.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.TotalWaterArea >= filters.TotalWaterAreaFrom.Value);
            }

            if (filters.TotalWaterAreaTo.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.TotalWaterArea <= filters.TotalWaterAreaTo.Value);
            }

            if (filters.TotalProductionCapacityFrom.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.TotalProductionCapacity >= filters.TotalProductionCapacityFrom.Value);
            }

            if (filters.TotalProductionCapacityTo.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.TotalProductionCapacity <= filters.TotalProductionCapacityTo.Value);
            }

            if (filters.PersonId.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.SubmittedForPersonId == filters.PersonId);
            }

            if (filters.LegalId.HasValue)
            {
                aquacultures = aquacultures.Where(x => x.SubmittedForLegalId == filters.LegalId);
            }

            IQueryable<AquacultureFacilityDTO> result = from aqua in aquacultures
                                                        join legal in Db.Legals on aqua.SubmittedForLegalId equals legal.Id into leg
                                                        from legal in leg.DefaultIfEmpty()
                                                        join person in Db.Persons on aqua.SubmittedForPersonId equals person.Id into per
                                                        from person in per.DefaultIfEmpty()
                                                        join tu in Db.NterritoryUnits on aqua.TerritoryUnitId equals tu.Id
                                                        orderby aqua.RegNum.Value descending
                                                        select new AquacultureFacilityDTO
                                                        {
                                                            Id = aqua.Id,
                                                            ApplicationId = aqua.ApplicationId,
                                                            RegNum = aqua.RegNum.Value,
                                                            UrorNum = aqua.UrorNum,
                                                            RegistrationDate = aqua.RegistrationDate,
                                                            Name = aqua.Name,
                                                            Owner = legal != null ? legal.Name : person.FirstName + " " + person.LastName,
                                                            TerritoryUnit = tu.Name,
                                                            Status = aqua.Status,
                                                            StatusName = aqua.StatusName,
                                                            DeliveryId = aqua.DeliveryId,
                                                            IsActive = aqua.IsActive
                                                        };

            return result;
        }

        private IQueryable<AquacultureFacilityDTO> GetFreeTextFilteredAquacultures(string text, bool showInactive, int? territoryUnitId)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<AquacultureFacilityDTO> result = from aqua in Db.AquacultureFacilitiesRegister
                                                        join status in Db.NaquacultureStatuses on aqua.AquacultureStatusId equals status.Id
                                                        join legal in Db.Legals on aqua.SubmittedForLegalId equals legal.Id into leg
                                                        from legal in leg.DefaultIfEmpty()
                                                        join person in Db.Persons on aqua.SubmittedForPersonId equals person.Id into per
                                                        from person in per.DefaultIfEmpty()
                                                        join tu in Db.NterritoryUnits on aqua.TerritoryUnitId equals tu.Id
                                                        join appl in Db.Applications on aqua.ApplicationId equals appl.Id
                                                        where aqua.RecordType == nameof(RecordTypesEnum.Register)
                                                            && aqua.IsActive == !showInactive
                                                            && (aqua.UrorNum.Replace(" ", "").Contains(text.Replace(" ", ""))
                                                                || (searchDate.HasValue && aqua.RegistrationDate == searchDate.Value)
                                                                || aqua.Name.ToLower().Contains(text)
                                                                || (legal != null ? legal.Name : person.FirstName + " " + person.LastName).ToLower().Contains(text)
                                                                || tu.Name.ToLower().Contains(text)
                                                                || status.Name.ToLower().Contains(text))
                                                            && (!territoryUnitId.HasValue || appl.TerritoryUnitId == territoryUnitId.Value)
                                                        orderby aqua.RegNum.Value descending
                                                        select new AquacultureFacilityDTO
                                                        {
                                                            Id = aqua.Id,
                                                            ApplicationId = aqua.ApplicationId,
                                                            RegNum = aqua.RegNum.Value,
                                                            UrorNum = aqua.UrorNum,
                                                            RegistrationDate = aqua.RegistrationDate,
                                                            Name = aqua.Name,
                                                            Owner = legal != null ? legal.Name : person.FirstName + " " + person.LastName,
                                                            TerritoryUnit = tu.Name,
                                                            Status = Enum.Parse<AquacultureStatusEnum>(status.Code),
                                                            StatusName = status.Name,
                                                            DeliveryId = appl.DeliveryId,
                                                            IsActive = aqua.IsActive
                                                        };

            return result;
        }

        private List<AquacultureCoordinateDTO> GetAquacultureCoordinates(int aquacultureId)
        {
            List<AquacultureCoordinateDTO> result = (from ac in Db.AquacultureFacilityCoordinates
                                                     where ac.AquacultureFacilityId == aquacultureId
                                                     orderby ac.PointNum
                                                     select new AquacultureCoordinateDTO
                                                     {
                                                         Id = ac.Id,
                                                         Longitude = new DMSType(ac.Coordinates.X).ToString(),
                                                         Latitude = new DMSType(ac.Coordinates.Y).ToString(),
                                                         IsActive = ac.IsActive
                                                     }).ToList();

            return result;
        }

        private List<int> GetAquacultureAquaticOrganismIds(int aquacultureId)
        {
            List<int> result = (from af in Db.AquacultureFacilityFishes
                                where af.AquacultureFacilityId == aquacultureId
                                    && af.IsActive
                                select af.FishTypeId).ToList();

            return result;
        }

        private List<UsageDocumentDTO> GetAquacultureUsageDocuments(int aquacultureId)
        {
            Dictionary<int, bool> ids = (from aquaDoc in Db.AquacultureUsageDocuments
                                         where aquaDoc.AquacultureFacilityId == aquacultureId
                                         select new
                                         {
                                             aquaDoc.UsageDocumentId,
                                             aquaDoc.IsActive
                                         }).ToDictionary(x => x.UsageDocumentId, y => y.IsActive);

            List<UsageDocumentDTO> result = usageDocumentsService.GetUsageDocuments(ids.Keys.ToList());

            foreach (UsageDocumentDTO document in result)
            {
                document.IsActive = ids[document.Id.Value];
            }

            return result;
        }

        private List<AquacultureWaterLawCertificateDTO> GetAquacultureWaterLawCertificates(int aquacultureId)
        {
            List<AquacultureWaterLawCertificateDTO> result = (from al in Db.AquacultureWaterLawCertificates
                                                              where al.AquacultureFacilityId == aquacultureId
                                                              orderby al.CertificateValidTo descending
                                                              select new AquacultureWaterLawCertificateDTO
                                                              {
                                                                  Id = al.Id,
                                                                  CertificateTypeId = al.CertificateTypeId,
                                                                  CertificateNum = al.CertificateNum,
                                                                  CertificateIssuer = al.CertificateIssuer,
                                                                  IsCertificateIndefinite = al.CertificateValidTo == null,
                                                                  CertificateValidFrom = al.CertificateValidFrom,
                                                                  CertificateValidTo = al.CertificateValidTo,
                                                                  Comments = al.Comments,
                                                                  IsActive = al.IsActive
                                                              }).ToList();

            return result;
        }

        private List<CommonDocumentDTO> GetAquacultureOvosCertificates(int aquacultureId)
        {
            List<CommonDocumentDTO> result = (from ao in Db.AquacultureOvosCertificates
                                              where ao.AquacultureFacilityId == aquacultureId
                                              orderby ao.CertificateValidTo descending
                                              select new CommonDocumentDTO
                                              {
                                                  Id = ao.Id,
                                                  Num = ao.CertificateNum,
                                                  Issuer = ao.CertificateIssuer,
                                                  IssueDate = ao.CertificateIssueDate,
                                                  IsIndefinite = ao.CertificateValidTo == null,
                                                  ValidFrom = ao.CertificateValidFrom,
                                                  ValidTo = ao.CertificateValidTo,
                                                  Comments = ao.Comments,
                                                  IsActive = ao.IsActive
                                              }).ToList();

            return result;
        }

        private List<CommonDocumentDTO> GetAquacultureBabhCertificates(int aquacultureId)
        {
            List<CommonDocumentDTO> result = (from ab in Db.AquacultureBabhCertificates
                                              where ab.AquacultureFacilityId == aquacultureId
                                              orderby ab.CertificateValidTo descending
                                              select new CommonDocumentDTO
                                              {
                                                  Id = ab.Id,
                                                  Num = ab.CertificateNum,
                                                  Issuer = ab.CertificateIssuer,
                                                  IssueDate = ab.CertificateIssueDate,
                                                  IsIndefinite = ab.CertificateValidTo == null,
                                                  ValidFrom = ab.CertificateValidFrom,
                                                  ValidTo = ab.CertificateValidTo,
                                                  Comments = ab.Comments,
                                                  IsActive = ab.IsActive
                                              }).ToList();

            return result;
        }

        private List<CancellationHistoryEntryDTO> GetAquacultureStatuses(int aquacultureId)
        {
            List<CancellationHistoryEntryDTO> result = (from status in Db.AquacultureFacilityStatuses
                                                        where status.AquacultureFacilityId == aquacultureId
                                                        orderby status.DateOfChange descending
                                                        select new CancellationHistoryEntryDTO
                                                        {
                                                            IsCancelled = status.IsCancelled,
                                                            CancellationReasonId = status.CancellationReasonId,
                                                            DateOfChange = status.DateOfChange,
                                                            IssueOrderNum = status.IssueOrderNum,
                                                            Description = status.Description
                                                        }).ToList();

            return result;
        }

        private List<AquacultureInstallationEditDTO> GetAquacultureInstallations(int aquacultureId)
        {
            List<AquacultureInstallationEditDTO> installations = (from aquaInstall in Db.AquacultureFacilityInstallations
                                                                  join installType in Db.NaquacultureInstallationTypes on aquaInstall.InstallationTypeId equals installType.Id
                                                                  where aquaInstall.AquacultureFacilityId == aquacultureId
                                                                  orderby aquaInstall.Id
                                                                  select new AquacultureInstallationEditDTO
                                                                  {
                                                                      Id = aquaInstall.Id,
                                                                      InstallationType = Enum.Parse<AquacultureInstallationTypeEnum>(installType.Code),
                                                                      Comments = aquaInstall.Comments,
                                                                      IsActive = aquaInstall.IsActive
                                                                  }).ToList();

            ILookup<int, AquacultureInstallationBasinDTO> basins = GetInstallationsBasins(installations);
            ILookup<int, AquacultureInstallationNetCageDTO> netCages = GetInstallationsNetCages(installations);
            ILookup<int, AquacultureInstallationCollectorDTO> collectors = GetInstallationsCollectors(installations);
            ILookup<int, AquacultureInstallationRaftDTO> rafts = GetInstallationsRafts(installations);
            ILookup<int, AquacultureInstallationRecirculatorySystemDTO> recirculatorySystems = GetInstallationsRecirculatorySystems(installations);
            Dictionary<int, AquacultureInstallationAquariumDTO> aquariums = GetInstallationsAquariums(installations);
            Dictionary<int, AquacultureInstallationDamDTO> dams = GetInstallationsDams(installations);

            foreach (AquacultureInstallationEditDTO installation in installations)
            {
                switch (installation.InstallationType.Value)
                {
                    case AquacultureInstallationTypeEnum.Basins:
                        installation.Basins = basins[installation.Id.Value].ToList();
                        break;
                    case AquacultureInstallationTypeEnum.NetCages:
                        installation.NetCages = netCages[installation.Id.Value].ToList();
                        break;
                    case AquacultureInstallationTypeEnum.Aquariums:
                        installation.Aquariums = aquariums[installation.Id.Value];
                        break;
                    case AquacultureInstallationTypeEnum.Collectors:
                        installation.Collectors = collectors[installation.Id.Value].ToList();
                        break;
                    case AquacultureInstallationTypeEnum.Rafts:
                        installation.Rafts = rafts[installation.Id.Value].ToList();
                        break;
                    case AquacultureInstallationTypeEnum.Dams:
                        installation.Dams = dams[installation.Id.Value];
                        break;
                    case AquacultureInstallationTypeEnum.RecirculatorySystems:
                        installation.RecirculatorySystems = recirculatorySystems[installation.Id.Value].ToList();
                        break;
                }
            }

            return installations;
        }

        private ILookup<int, AquacultureInstallationBasinDTO> GetInstallationsBasins(List<AquacultureInstallationEditDTO> installations)
        {
            List<int> installationIds = installations.Where(x => x.InstallationType == AquacultureInstallationTypeEnum.Basins).Select(x => x.Id.Value).ToList();

            ILookup<int, AquacultureInstallationBasinDTO> basins = (from aquaBasin in Db.AquacultureInstallationBasins
                                                                    join basin in Db.InstallationBasins on aquaBasin.InstallationBasinId equals basin.Id
                                                                    where installationIds.Contains(aquaBasin.AquacutureInstallationId)
                                                                    select new
                                                                    {
                                                                        InstallationId = aquaBasin.AquacutureInstallationId,
                                                                        Basin = new AquacultureInstallationBasinDTO
                                                                        {
                                                                            Id = aquaBasin.Id,
                                                                            BasinPurposeTypeId = basin.BasinPurposeTypeId,
                                                                            BasinMaterialTypeId = basin.BasinMaterialTypeId,
                                                                            Count = basin.Count,
                                                                            Area = basin.Area,
                                                                            Volume = basin.Volume,
                                                                            IsActive = aquaBasin.IsActive
                                                                        }
                                                                    }).ToLookup(x => x.InstallationId, y => y.Basin);

            return basins;
        }

        private ILookup<int, AquacultureInstallationNetCageDTO> GetInstallationsNetCages(List<AquacultureInstallationEditDTO> installations)
        {
            List<int> installationIds = installations.Where(x => x.InstallationType == AquacultureInstallationTypeEnum.NetCages).Select(x => x.Id.Value).ToList();

            ILookup<int, AquacultureInstallationNetCageDTO> netCages = (
                from aquaNetCage in Db.AquacultureInstallationNetCages
                where installationIds.Contains(aquaNetCage.AquacultureInstallationId)
                select new
                {
                    InstallationId = aquaNetCage.AquacultureInstallationId,
                    NetCage = new AquacultureInstallationNetCageDTO
                    {
                        Id = aquaNetCage.Id,
                        NetCageTypeId = aquaNetCage.NetCageTypeId,
                        Shape = Enum.Parse<AquacultureInstallationNetCageShapesEnum>(aquaNetCage.NetCageShape),
                        Radius = aquaNetCage.Radius,
                        Length = aquaNetCage.Length,
                        Width = aquaNetCage.Width,
                        Height = aquaNetCage.Height,
                        Area = aquaNetCage.Area,
                        Volume = aquaNetCage.Volume,
                        Count = aquaNetCage.Count,
                        IsActive = aquaNetCage.IsActive
                    }
                }
            ).ToLookup(x => x.InstallationId, y => y.NetCage);

            return netCages;
        }

        private ILookup<int, AquacultureInstallationCollectorDTO> GetInstallationsCollectors(List<AquacultureInstallationEditDTO> installations)
        {
            List<int> installationIds = installations.Where(x => x.InstallationType == AquacultureInstallationTypeEnum.Collectors).Select(x => x.Id.Value).ToList();

            ILookup<int, AquacultureInstallationCollectorDTO> collectors = (from collector in Db.AquacultureInstallationCollectors
                                                                            where installationIds.Contains(collector.AquacultureInstallationId)
                                                                            select new
                                                                            {
                                                                                InstallationId = collector.AquacultureInstallationId,
                                                                                Collector = new AquacultureInstallationCollectorDTO
                                                                                {
                                                                                    Id = collector.Id,
                                                                                    CollectorTypeId = collector.CollectorTypeId,
                                                                                    TotalCount = collector.TotalCount,
                                                                                    TotalArea = collector.TotalArea,
                                                                                    IsActive = collector.IsActive
                                                                                }
                                                                            }).ToLookup(x => x.InstallationId, y => y.Collector);

            return collectors;
        }

        private ILookup<int, AquacultureInstallationRaftDTO> GetInstallationsRafts(List<AquacultureInstallationEditDTO> installations)
        {
            List<int> installationIds = installations.Where(x => x.InstallationType == AquacultureInstallationTypeEnum.Rafts).Select(x => x.Id.Value).ToList();

            ILookup<int, AquacultureInstallationRaftDTO> rafts = (from raft in Db.AquacultureInstallationRafts
                                                                  where installationIds.Contains(raft.AquacultureInstallationId)
                                                                  select new
                                                                  {
                                                                      InstallationId = raft.AquacultureInstallationId,
                                                                      Raft = new AquacultureInstallationRaftDTO
                                                                      {
                                                                          Id = raft.Id,
                                                                          Length = raft.Length,
                                                                          Width = raft.Width,
                                                                          Area = raft.Area,
                                                                          Count = raft.Count,
                                                                          IsActive = raft.IsActive
                                                                      }
                                                                  }).ToLookup(x => x.InstallationId, y => y.Raft);

            return rafts;
        }

        private ILookup<int, AquacultureInstallationRecirculatorySystemDTO> GetInstallationsRecirculatorySystems(List<AquacultureInstallationEditDTO> installations)
        {
            List<int> installationIds = installations.Where(x => x.InstallationType == AquacultureInstallationTypeEnum.RecirculatorySystems).Select(x => x.Id.Value).ToList();

            ILookup<int, AquacultureInstallationRecirculatorySystemDTO> systems = (
                from aquaSystem in Db.AquacultureInstallationRecirculatorySystems
                join basin in Db.InstallationBasins on aquaSystem.InstallationBasinId equals basin.Id
                where installationIds.Contains(aquaSystem.AquacultureInstallationId)
                select new
                {
                    InstallationId = aquaSystem.AquacultureInstallationId,
                    Raft = new AquacultureInstallationRecirculatorySystemDTO
                    {
                        Id = aquaSystem.Id,
                        BasinPurposeTypeId = basin.BasinPurposeTypeId,
                        BasinMaterialTypeId = basin.BasinMaterialTypeId,
                        Area = basin.Area,
                        Volume = basin.Volume,
                        Count = basin.Count,
                        IsActive = aquaSystem.IsActive
                    }
                }
            ).ToLookup(x => x.InstallationId, y => y.Raft);

            return systems;
        }

        private Dictionary<int, AquacultureInstallationAquariumDTO> GetInstallationsAquariums(List<AquacultureInstallationEditDTO> installations)
        {
            List<int> installationIds = installations.Where(x => x.InstallationType == AquacultureInstallationTypeEnum.Aquariums).Select(x => x.Id.Value).ToList();

            Dictionary<int, AquacultureInstallationAquariumDTO> aquariums = (from aqua in Db.AquacultureFacilityInstallations
                                                                             join aquarium in Db.InstallationAquariums on aqua.InstallationAquariumId equals aquarium.Id
                                                                             where installationIds.Contains(aqua.Id)
                                                                             select new
                                                                             {
                                                                                 InstallationId = aqua.Id,
                                                                                 Aquarium = new AquacultureInstallationAquariumDTO
                                                                                 {
                                                                                     Id = aquarium.Id,
                                                                                     Count = aquarium.Count,
                                                                                     Volume = aquarium.Volume
                                                                                 }
                                                                             }).ToDictionary(x => x.InstallationId, y => y.Aquarium);

            return aquariums;
        }

        private Dictionary<int, AquacultureInstallationDamDTO> GetInstallationsDams(List<AquacultureInstallationEditDTO> installations)
        {
            List<int> installationIds = installations.Where(x => x.InstallationType == AquacultureInstallationTypeEnum.Dams).Select(x => x.Id.Value).ToList();

            Dictionary<int, AquacultureInstallationDamDTO> dams = (from aqua in Db.AquacultureFacilityInstallations
                                                                   join dam in Db.InstallationDams on aqua.InstallationDamId equals dam.Id
                                                                   where installationIds.Contains(aqua.Id)
                                                                   select new
                                                                   {
                                                                       InstallationId = aqua.Id,
                                                                       Aquarium = new AquacultureInstallationDamDTO
                                                                       {
                                                                           Id = dam.Id,
                                                                           Area = dam.Area
                                                                       }
                                                                   }).ToDictionary(x => x.InstallationId, y => y.Aquarium);

            return dams;
        }

        private List<AquacultureInstallationBasin> GetInstallationBasins(int installationId)
        {
            List<AquacultureInstallationBasin> basins = (from basin in Db.AquacultureInstallationBasins
                                                            .Include(x => x.InstallationBasin)
                                                         where basin.AquacutureInstallationId == installationId
                                                         select basin).ToList();

            return basins;
        }

        private List<AquacultureInstallationNetCage> GetInstallationNetCages(int installationId)
        {
            List<AquacultureInstallationNetCage> netCages = (from netCage in Db.AquacultureInstallationNetCages
                                                             where netCage.AquacultureInstallationId == installationId
                                                             select netCage).ToList();

            return netCages;
        }

        private List<AquacultureInstallationCollector> GetInstallationCollectors(int installationId)
        {
            List<AquacultureInstallationCollector> collectors = (from coll in Db.AquacultureInstallationCollectors
                                                                 where coll.AquacultureInstallationId == installationId
                                                                 select coll).ToList();

            return collectors;
        }

        private List<AquacultureInstallationRaft> GetInstallationRafts(int installationId)
        {
            List<AquacultureInstallationRaft> rafts = (from rft in Db.AquacultureInstallationRafts
                                                       where rft.AquacultureInstallationId == installationId
                                                       select rft).ToList();

            return rafts;
        }

        private List<AquacultureInstallationRecirculatorySystem> GetInstallationRecirculatorySystems(int installationId)
        {
            List<AquacultureInstallationRecirculatorySystem> systems = (from sys in Db.AquacultureInstallationRecirculatorySystems
                                                                            .Include(x => x.InstallationBasin)
                                                                        where sys.AquacultureInstallationId == installationId
                                                                        select sys).ToList();

            return systems;
        }

        private List<AquacultureHatcheryEquipmentDTO> GetAquacultureHatcheryEquipment(int aquacultureId)
        {
            List<AquacultureHatcheryEquipmentDTO> result = (from hatch in Db.AquacultureHatcheryEquipment
                                                            where hatch.AquacultureFacilityId == aquacultureId
                                                            select new AquacultureHatcheryEquipmentDTO
                                                            {
                                                                Id = hatch.Id,
                                                                EquipmentTypeId = hatch.EquipmentTypeId,
                                                                Count = hatch.Count,
                                                                Volume = hatch.Volume,
                                                                IsActive = hatch.IsActive
                                                            }).ToList();

            return result;
        }

        private string GenerateUrorNum(int territoryUnitId, int waterAreaTypeId)
        {
            NterritoryUnit territoryUnit = (from tu in Db.NterritoryUnits
                                            where tu.Id == territoryUnitId
                                            select tu).First();

            NaquacultureWaterAreaType waterAreaType = (from wa in Db.NaquacultureWaterAreaTypes
                                                       where wa.Id == waterAreaTypeId
                                                       select wa).First();

            ++territoryUnit.AquaculturesRegisterSequence;

            string sequenceNum = territoryUnit.AquaculturesRegisterSequence.ToString();
            int diff = 5 - sequenceNum.Length;
            if (diff != 0)
            {
                sequenceNum = $"{new string('0', diff)}{sequenceNum}";
            }

            return $"{territoryUnit.Code} {waterAreaType.Code} {sequenceNum}";
        }

        private void AddAquacultureCoordinates(AquacultureFacilityRegister aquaculture, List<AquacultureCoordinateDTO> coordinates)
        {
            short pointNum = 1;

            if (coordinates != null)
            {
                foreach (AquacultureCoordinateDTO coordinate in coordinates)
                {
                    CoordinatesDMS coord = CoordinatesDMS.Parse(coordinate.Longitude, coordinate.Latitude);

                    AquacultureFacilityCoordinate entry = new AquacultureFacilityCoordinate
                    {
                        AquacultureFacility = aquaculture,
                        PointNum = pointNum++,
                        Coordinates = new Point(coord.Longitude.ToDecimal(), coord.Latitude.ToDecimal()),
                        IsActive = coordinate.IsActive.Value
                    };

                    Db.AquacultureFacilityCoordinates.Add(entry);
                }
            }
        }

        private void AddAquaticOrganisms(AquacultureFacilityRegister aquaculture, List<int> aquaticOrganismIds)
        {
            foreach (int aquaticOrganismId in aquaticOrganismIds)
            {
                AquacultureFacilityFish entry = new AquacultureFacilityFish
                {
                    AquacultureFacility = aquaculture,
                    FishTypeId = aquaticOrganismId
                };

                Db.AquacultureFacilityFishes.Add(entry);
            }
        }

        private void AddUsageDocuments(AquacultureFacilityRegister aquaculture, List<UsageDocumentDTO> usageDocuments)
        {
            foreach (UsageDocumentDTO document in usageDocuments)
            {
                UsageDocument newDocument = Db.AddUsageDocument(document, aquaculture);

                AquacultureUsageDocument entry = new AquacultureUsageDocument
                {
                    AquacultureFacility = aquaculture,
                    UsageDocument = newDocument,
                    IsActive = document.IsActive.Value
                };

                Db.AquacultureUsageDocuments.Add(entry);
            }
        }

        private void AddWaterLawCertificates(AquacultureFacilityRegister aquaculture, List<AquacultureWaterLawCertificateDTO> certificates)
        {
            foreach (AquacultureWaterLawCertificateDTO certificate in certificates)
            {
                AquacultureWaterLawCertificate entry = new AquacultureWaterLawCertificate
                {
                    AquacultureFacility = aquaculture,
                    CertificateTypeId = certificate.CertificateTypeId.Value,
                    CertificateNum = certificate.CertificateNum,
                    CertificateIssuer = certificate.CertificateIssuer,
                    CertificateValidFrom = certificate.CertificateValidFrom.Value,
                    CertificateValidTo = certificate.IsCertificateIndefinite.Value ? null : certificate.CertificateValidTo,
                    Comments = certificate.Comments,
                    IsActive = certificate.IsActive.Value
                };

                Db.AquacultureWaterLawCertificates.Add(entry);
            }
        }

        private void AddOvosCertificates(AquacultureFacilityRegister aquaculture, List<CommonDocumentDTO> certificates)
        {
            foreach (CommonDocumentDTO certificate in certificates)
            {
                AquacultureOvosCertificate entry = new AquacultureOvosCertificate
                {
                    AquacultureFacility = aquaculture,
                    CertificateNum = certificate.Num,
                    CertificateIssuer = certificate.Issuer,
                    CertificateIssueDate = certificate.IssueDate.Value,
                    CertificateValidFrom = certificate.ValidFrom.Value,
                    CertificateValidTo = certificate.IsIndefinite.Value ? null : certificate.ValidTo,
                    Comments = certificate.Comments,
                    IsActive = certificate.IsActive.Value
                };

                Db.AquacultureOvosCertificates.Add(entry);
            }
        }

        private void AddBabhCertificates(AquacultureFacilityRegister aquaculture, List<CommonDocumentDTO> certificates)
        {
            if (certificates != null)
            {
                foreach (CommonDocumentDTO certificate in certificates)
                {
                    AquacultureBabhCertificate entry = new AquacultureBabhCertificate
                    {
                        AquacultureFacility = aquaculture,
                        CertificateNum = certificate.Num,
                        CertificateIssuer = certificate.Issuer,
                        CertificateIssueDate = certificate.IssueDate.Value,
                        CertificateValidFrom = certificate.ValidFrom.Value,
                        CertificateValidTo = certificate.IsIndefinite.Value ? null : certificate.ValidTo,
                        Comments = certificate.Comments,
                        IsActive = certificate.IsActive.Value
                    };

                    Db.AquacultureBabhCertificates.Add(entry);
                }
            }
        }

        private void AddAquacultureHatcheryEquipment(AquacultureFacilityRegister aquaculture, List<AquacultureHatcheryEquipmentDTO> equipment)
        {
            foreach (AquacultureHatcheryEquipmentDTO equip in equipment)
            {
                AquacultureHatcheryEquipment entry = new AquacultureHatcheryEquipment
                {
                    AquacultureFacility = aquaculture,
                    EquipmentTypeId = equip.EquipmentTypeId.Value,
                    Count = equip.Count.Value,
                    Volume = equip.Volume.Value,
                    IsActive = equip.IsActive.Value
                };

                Db.AquacultureHatcheryEquipment.Add(entry);
            }
        }

        private void AddAquacultureInstallations(AquacultureFacilityRegister aquaculture, List<AquacultureInstallationEditDTO> installations)
        {
            Dictionary<AquacultureInstallationTypeEnum, int> types = GetInstallationTypesCodeToIdDictionary();

            foreach (AquacultureInstallationEditDTO installation in installations)
            {
                AquacultureFacilityInstallation entry = new AquacultureFacilityInstallation
                {
                    AquacultureFacility = aquaculture,
                    InstallationTypeId = types[installation.InstallationType.Value],
                    Comments = installation.Comments,
                    IsActive = installation.IsActive.Value
                };

                Db.AquacultureFacilityInstallations.Add(entry);

                switch (installation.InstallationType.Value)
                {
                    case AquacultureInstallationTypeEnum.Basins:
                        AddAquacultureInstallationBasins(entry, installation.Basins);
                        break;
                    case AquacultureInstallationTypeEnum.NetCages:
                        AddAquacultureInstallationNetCages(entry, installation.NetCages);
                        break;
                    case AquacultureInstallationTypeEnum.Aquariums:
                        AddAquacultureInstallationAquariums(entry, installation.Aquariums);
                        break;
                    case AquacultureInstallationTypeEnum.Collectors:
                        AddAquacultureInstallationCollectors(entry, installation.Collectors);
                        break;
                    case AquacultureInstallationTypeEnum.Rafts:
                        AddAquacultureInstallationRafts(entry, installation.Rafts);
                        break;
                    case AquacultureInstallationTypeEnum.Dams:
                        AddAquacultureInstallationDams(entry, installation.Dams);
                        break;
                    case AquacultureInstallationTypeEnum.RecirculatorySystems:
                        AddAquacultureInstallationRecirculatorySystems(entry, installation.RecirculatorySystems);
                        break;
                }
            }
        }

        private void AddAquacultureInstallationBasins(AquacultureFacilityInstallation installation, List<AquacultureInstallationBasinDTO> basins)
        {
            foreach (AquacultureInstallationBasinDTO basin in basins)
            {
                AddAquacultureInstallationBasinEntry(installation, basin);
            }
        }

        private void AddAquacultureInstallationBasinEntry(AquacultureFacilityInstallation installation, AquacultureInstallationBasinDTO basin)
        {
            InstallationBasin entry = new InstallationBasin
            {
                BasinPurposeTypeId = basin.BasinPurposeTypeId.Value,
                BasinMaterialTypeId = basin.BasinMaterialTypeId.Value,
                Area = basin.Area.Value,
                Volume = basin.Volume.Value,
                Count = basin.Count.Value
            };

            AquacultureInstallationBasin aquaBasin = new AquacultureInstallationBasin
            {
                AquacutureInstallation = installation,
                InstallationBasin = entry,
                IsActive = basin.IsActive.Value
            };

            Db.AquacultureInstallationBasins.Add(aquaBasin);
        }

        private void AddAquacultureInstallationNetCages(AquacultureFacilityInstallation installation, List<AquacultureInstallationNetCageDTO> netCages)
        {
            foreach (AquacultureInstallationNetCageDTO netCage in netCages)
            {
                AddAquacultureInstallationNetCageEntry(installation, netCage);
            }
        }

        private void AddAquacultureInstallationNetCageEntry(AquacultureFacilityInstallation installation, AquacultureInstallationNetCageDTO netCage)
        {
            AquacultureInstallationNetCage entry = new AquacultureInstallationNetCage
            {
                AquacultureInstallation = installation,
                NetCageTypeId = netCage.NetCageTypeId.Value,
                NetCageShape = netCage.Shape.ToString(),
                Area = netCage.Area.Value,
                Volume = netCage.Volume.Value,
                Count = netCage.Count.Value,
                IsActive = netCage.IsActive.Value
            };

            if (netCage.Shape == AquacultureInstallationNetCageShapesEnum.Circular)
            {
                entry.Radius = netCage.Radius;
                entry.Height = netCage.Height;
            }
            else if (netCage.Shape == AquacultureInstallationNetCageShapesEnum.Rectangular)
            {
                entry.Length = netCage.Length;
                entry.Width = netCage.Width;
                entry.Height = netCage.Height;
            }

            Db.AquacultureInstallationNetCages.Add(entry);
        }

        private void AddAquacultureInstallationAquariums(AquacultureFacilityInstallation installation, AquacultureInstallationAquariumDTO aquarium)
        {
            installation.InstallationAquarium = new InstallationAquarium
            {
                Volume = aquarium.Volume.Value,
                Count = aquarium.Count.Value
            };
        }

        private void AddAquacultureInstallationCollectors(AquacultureFacilityInstallation installation, List<AquacultureInstallationCollectorDTO> collectors)
        {
            foreach (AquacultureInstallationCollectorDTO collector in collectors)
            {
                AddAquacultureInstallationCollectorEntry(installation, collector);
            }
        }

        private void AddAquacultureInstallationCollectorEntry(AquacultureFacilityInstallation installation, AquacultureInstallationCollectorDTO collector)
        {
            AquacultureInstallationCollector entry = new AquacultureInstallationCollector
            {
                AquacultureInstallation = installation,
                CollectorTypeId = collector.CollectorTypeId.Value,
                TotalCount = collector.TotalCount.Value,
                TotalArea = collector.TotalArea.Value,
                IsActive = collector.IsActive.Value
            };

            Db.AquacultureInstallationCollectors.Add(entry);
        }

        private void AddAquacultureInstallationRafts(AquacultureFacilityInstallation installation, List<AquacultureInstallationRaftDTO> rafts)
        {
            foreach (AquacultureInstallationRaftDTO raft in rafts)
            {
                AddAquacultureInstallationRaftEntry(installation, raft);
            }
        }

        private void AddAquacultureInstallationRaftEntry(AquacultureFacilityInstallation installation, AquacultureInstallationRaftDTO raft)
        {
            AquacultureInstallationRaft entry = new AquacultureInstallationRaft
            {
                AquacultureInstallation = installation,
                Length = raft.Length.Value,
                Width = raft.Width.Value,
                Area = raft.Area.Value,
                Count = raft.Count.Value,
                IsActive = raft.IsActive.Value
            };

            Db.AquacultureInstallationRafts.Add(entry);
        }

        private void AddAquacultureInstallationDams(AquacultureFacilityInstallation installation, AquacultureInstallationDamDTO dam)
        {
            installation.InstallationDam = new InstallationDam
            {
                Area = dam.Area.Value,
                Count = 1
            };
        }

        private void AddAquacultureInstallationRecirculatorySystems(AquacultureFacilityInstallation installation, List<AquacultureInstallationRecirculatorySystemDTO> systems)
        {
            foreach (AquacultureInstallationRecirculatorySystemDTO system in systems)
            {
                AddAquacultureInstallationRecirculatorySystemEntry(installation, system);
            }
        }

        private void AddAquacultureInstallationRecirculatorySystemEntry(AquacultureFacilityInstallation installation, AquacultureInstallationRecirculatorySystemDTO system)
        {
            InstallationBasin entry = new InstallationBasin
            {
                BasinPurposeTypeId = system.BasinPurposeTypeId.Value,
                BasinMaterialTypeId = system.BasinMaterialTypeId.Value,
                Area = system.Area.Value,
                Volume = system.Volume.Value,
                Count = system.Count.Value
            };

            AquacultureInstallationRecirculatorySystem aquaSystem = new AquacultureInstallationRecirculatorySystem
            {
                AquacultureInstallation = installation,
                InstallationBasin = entry,
                IsActive = system.IsActive.Value
            };

            Db.AquacultureInstallationRecirculatorySystems.Add(aquaSystem);
        }

        private void AddAquacultureLogBooks(int aquacultureId, List<LogBookEditDTO> logBooks, bool ignoreLogBookConflicts)
        {
            if (logBooks != null)
            {
                foreach (LogBookEditDTO logBook in logBooks)
                {
                    Db.AddAquacultureLogBook(logBook, aquacultureId, ignoreLogBookConflicts);
                }
            }
        }

        private void EditAquacultureCoordinates(int aquacultureId, List<AquacultureCoordinateDTO> coordinates)
        {
            short counter = 1;

            List<AquacultureFacilityCoordinate> dbCoordinates = Db.AquacultureFacilityCoordinates.Where(x => x.AquacultureFacilityId == aquacultureId).ToList();

            if (coordinates != null)
            {
                foreach (AquacultureCoordinateDTO coordinate in coordinates)
                {
                    CoordinatesDMS dms = CoordinatesDMS.Parse(coordinate.Longitude, coordinate.Latitude);
                    Point point = new Point(dms.Longitude.ToDecimal(), dms.Latitude.ToDecimal());

                    // нова координата
                    if (coordinate.Id == null)
                    {
                        AquacultureFacilityCoordinate entry = new AquacultureFacilityCoordinate
                        {
                            AquacultureFacilityId = aquacultureId,
                            Coordinates = point,
                            PointNum = counter++
                        };

                        Db.AquacultureFacilityCoordinates.Add(entry);
                    }
                    // стара координата
                    else
                    {
                        AquacultureFacilityCoordinate entry = dbCoordinates.Where(x => x.Id == coordinate.Id).First();
                        entry.Coordinates = point;
                        entry.PointNum = counter++;
                    }
                }
            }
            else
            {
                foreach (AquacultureFacilityCoordinate coordinate in dbCoordinates)
                {
                    coordinate.IsActive = false;
                }
            }
        }

        private void EditAquacultureAquaticOrganisms(int aquacultureId, List<int> aquaticOrganismIds)
        {
            List<AquacultureFacilityFish> currentAquaOrganisms = (from aquaFish in Db.AquacultureFacilityFishes
                                                                  where aquaFish.AquacultureFacilityId == aquacultureId
                                                                  select aquaFish).ToList();

            if (aquaticOrganismIds != null)
            {
                List<int> currentAquaOrganismIds = currentAquaOrganisms.Select(x => x.FishTypeId).ToList();
                List<int> aquaOrganismIdsToAdd = aquaticOrganismIds.Where(x => !currentAquaOrganismIds.Contains(x)).ToList();
                List<int> aquaOrganismIdsToRemove = currentAquaOrganismIds.Where(x => !aquaticOrganismIds.Contains(x)).ToList();

                foreach (int aquaOrganismId in aquaOrganismIdsToAdd)
                {
                    AquacultureFacilityFish aquaOrganism = currentAquaOrganisms.Where(x => x.FishTypeId == aquaOrganismId).FirstOrDefault();
                    if (aquaOrganism != null)
                    {
                        aquaOrganism.IsActive = true;
                    }
                    else
                    {
                        AquacultureFacilityFish entry = new AquacultureFacilityFish
                        {
                            AquacultureFacilityId = aquacultureId,
                            FishTypeId = aquaOrganismId
                        };

                        Db.AquacultureFacilityFishes.Add(entry);
                    }
                }

                foreach (int aquaOrganismId in aquaOrganismIdsToRemove)
                {
                    AquacultureFacilityFish aquaOrganism = currentAquaOrganisms.Where(x => x.FishTypeId == aquaOrganismId).First();
                    aquaOrganism.IsActive = false;
                }
            }
            else
            {
                foreach (AquacultureFacilityFish aquaOrganism in currentAquaOrganisms)
                {
                    aquaOrganism.IsActive = false;
                }
            }
        }

        private void EditAquacultureHatcheryEquipment(int aquacultureId, List<AquacultureHatcheryEquipmentDTO> equipment, AquacultureSystemEnum system)
        {
            List<AquacultureHatcheryEquipment> currentEquipment = (from hatch in Db.AquacultureHatcheryEquipment
                                                                   where hatch.AquacultureFacilityId == aquacultureId
                                                                   select hatch).ToList();

            if (system == AquacultureSystemEnum.NonFullSystem || equipment == null)
            {
                foreach (AquacultureHatcheryEquipment equip in currentEquipment)
                {
                    equip.IsActive = false;
                }
            }
            else
            {
                foreach (AquacultureHatcheryEquipmentDTO equip in equipment)
                {
                    if (equip.Id != null)
                    {
                        AquacultureHatcheryEquipment dbEquip = currentEquipment.Where(x => x.Id == equip.Id).First();

                        dbEquip.EquipmentTypeId = equip.EquipmentTypeId.Value;
                        dbEquip.Count = equip.Count.Value;
                        dbEquip.Volume = equip.Volume.Value;
                        dbEquip.IsActive = equip.IsActive.Value;
                    }
                    else
                    {
                        AquacultureHatcheryEquipment entry = new AquacultureHatcheryEquipment
                        {
                            AquacultureFacilityId = aquacultureId,
                            EquipmentTypeId = equip.EquipmentTypeId.Value,
                            Count = equip.Count.Value,
                            Volume = equip.Volume.Value,
                            IsActive = equip.IsActive.Value
                        };

                        Db.AquacultureHatcheryEquipment.Add(entry);
                    }
                }
            }
        }

        private void EditUsageDocuments(AquacultureFacilityRegister aquaculture, List<UsageDocumentDTO> documents)
        {
            List<AquacultureUsageDocument> currentUsageDocuments = (from usgDoc in Db.AquacultureUsageDocuments
                                                                    where usgDoc.AquacultureFacilityId == aquaculture.Id
                                                                    select usgDoc).ToList();

            foreach (AquacultureUsageDocument document in currentUsageDocuments)
            {
                document.IsActive = false;
            }

            if (documents != null)
            {
                foreach (UsageDocumentDTO document in documents)
                {
                    if (document.Id != null)
                    {
                        AquacultureUsageDocument dbDoc = currentUsageDocuments.Where(x => x.UsageDocumentId == document.Id).First();
                        dbDoc.UsageDocument = Db.EditUsageDocument(document, aquaculture);
                        dbDoc.IsActive = document.IsActive.Value;
                    }
                    else
                    {
                        UsageDocument newDocument = Db.AddUsageDocument(document, aquaculture);

                        AquacultureUsageDocument entry = new AquacultureUsageDocument
                        {
                            AquacultureFacility = aquaculture,
                            UsageDocument = newDocument
                        };

                        Db.AquacultureUsageDocuments.Add(entry);
                    }
                }
            }
        }

        private void EditWaterLawCertificates(int aquacultureId, List<AquacultureWaterLawCertificateDTO> certificates)
        {
            List<AquacultureWaterLawCertificate> currentCertificates = (from cert in Db.AquacultureWaterLawCertificates
                                                                        where cert.AquacultureFacilityId == aquacultureId
                                                                        select cert).ToList();

            foreach (AquacultureWaterLawCertificate certificate in currentCertificates)
            {
                certificate.IsActive = false;
            }

            if (certificates != null)
            {
                foreach (AquacultureWaterLawCertificateDTO certificate in certificates)
                {
                    if (certificate.Id != null)
                    {
                        AquacultureWaterLawCertificate dbCertificate = currentCertificates.Where(x => x.Id == certificate.Id).First();

                        dbCertificate.CertificateTypeId = certificate.CertificateTypeId.Value;
                        dbCertificate.CertificateNum = certificate.CertificateNum;
                        dbCertificate.CertificateIssuer = certificate.CertificateIssuer;
                        dbCertificate.CertificateValidFrom = certificate.CertificateValidFrom.Value;
                        dbCertificate.CertificateValidTo = certificate.IsCertificateIndefinite.Value ? default(DateTime?) : certificate.CertificateValidTo.Value;
                        dbCertificate.Comments = certificate.Comments;
                        dbCertificate.IsActive = certificate.IsActive.Value;
                    }
                    else
                    {
                        AquacultureWaterLawCertificate entry = new AquacultureWaterLawCertificate
                        {
                            AquacultureFacilityId = aquacultureId,
                            CertificateTypeId = certificate.CertificateTypeId.Value,
                            CertificateNum = certificate.CertificateNum,
                            CertificateIssuer = certificate.CertificateIssuer,
                            CertificateValidFrom = certificate.CertificateValidFrom.Value,
                            CertificateValidTo = certificate.IsCertificateIndefinite.Value ? default(DateTime?) : certificate.CertificateValidTo.Value,
                            Comments = certificate.Comments,
                            IsActive = certificate.IsActive.Value
                        };

                        Db.AquacultureWaterLawCertificates.Add(entry);
                    }
                }
            }
        }

        private void EditOvosCertificates(int aquacultureId, List<CommonDocumentDTO> certificates)
        {
            List<AquacultureOvosCertificate> currentCertificates = (from cert in Db.AquacultureOvosCertificates
                                                                    where cert.AquacultureFacilityId == aquacultureId
                                                                    select cert).ToList();

            foreach (AquacultureOvosCertificate certificate in currentCertificates)
            {
                certificate.IsActive = false;
            }

            if (certificates != null)
            {
                foreach (CommonDocumentDTO certificate in certificates)
                {
                    if (certificate.Id != null)
                    {
                        AquacultureOvosCertificate dbCertificate = currentCertificates.Where(x => x.Id == certificate.Id).First();

                        dbCertificate.CertificateNum = certificate.Num;
                        dbCertificate.CertificateIssuer = certificate.Issuer;
                        dbCertificate.CertificateIssueDate = certificate.IssueDate.Value;
                        dbCertificate.CertificateValidFrom = certificate.ValidFrom.Value;
                        dbCertificate.CertificateValidTo = certificate.IsIndefinite.Value ? default(DateTime?) : certificate.ValidTo.Value;
                        dbCertificate.Comments = certificate.Comments;
                        dbCertificate.IsActive = certificate.IsActive.Value;
                    }
                    else
                    {
                        AquacultureOvosCertificate entry = new AquacultureOvosCertificate
                        {
                            CertificateNum = certificate.Num,
                            CertificateIssuer = certificate.Issuer,
                            CertificateIssueDate = certificate.IssueDate.Value,
                            CertificateValidFrom = certificate.ValidFrom.Value,
                            CertificateValidTo = certificate.IsIndefinite.Value ? default(DateTime?) : certificate.ValidTo.Value,
                            Comments = certificate.Comments,
                            IsActive = certificate.IsActive.Value
                        };

                        Db.AquacultureOvosCertificates.Add(entry);
                    }
                }
            }
        }

        private void EditBabhCertificates(int aquacultureId, List<CommonDocumentDTO> certificates)
        {
            List<AquacultureBabhCertificate> currentCertificates = (from cert in Db.AquacultureBabhCertificates
                                                                    where cert.AquacultureFacilityId == aquacultureId
                                                                    select cert).ToList();

            foreach (AquacultureBabhCertificate certificate in currentCertificates)
            {
                certificate.IsActive = false;
            }

            if (certificates != null)
            {
                foreach (CommonDocumentDTO certificate in certificates)
                {
                    if (certificate.Id != null)
                    {
                        AquacultureBabhCertificate dbCertificate = currentCertificates.Where(x => x.Id == certificate.Id).First();

                        dbCertificate.CertificateNum = certificate.Num;
                        dbCertificate.CertificateIssuer = certificate.Issuer;
                        dbCertificate.CertificateIssueDate = certificate.IssueDate.Value;
                        dbCertificate.CertificateValidFrom = certificate.ValidFrom.Value;
                        dbCertificate.CertificateValidTo = certificate.IsIndefinite.Value ? default(DateTime?) : certificate.ValidTo.Value;
                        dbCertificate.Comments = certificate.Comments;
                        dbCertificate.IsActive = certificate.IsActive.Value;
                    }
                    else
                    {
                        AquacultureBabhCertificate entry = new AquacultureBabhCertificate
                        {
                            AquacultureFacilityId = aquacultureId,
                            CertificateNum = certificate.Num,
                            CertificateIssuer = certificate.Issuer,
                            CertificateIssueDate = certificate.IssueDate.Value,
                            CertificateValidFrom = certificate.ValidFrom.Value,
                            CertificateValidTo = certificate.IsIndefinite.Value ? default(DateTime?) : certificate.ValidTo.Value,
                            Comments = certificate.Comments,
                            IsActive = certificate.IsActive.Value
                        };

                        Db.AquacultureBabhCertificates.Add(entry);
                    }
                }
            }
        }

        private void EditAquacultureInstallations(AquacultureFacilityRegister aquaculture, List<AquacultureInstallationEditDTO> installations)
        {
            Dictionary<AquacultureInstallationTypeEnum, int> installationTypes;
            List<AquacultureFacilityInstallation> dbInstallations;

            // ако има стари записи
            if (installations.Any(x => x.Id != null))
            {
                installationTypes = GetInstallationTypesCodeToIdDictionary();

                dbInstallations = (from aqua in Db.AquacultureFacilityInstallations
                                   where aqua.AquacultureFacilityId == aquaculture.Id
                                   select aqua).ToList();
            }
            // само нови записи
            else
            {
                installationTypes = new Dictionary<AquacultureInstallationTypeEnum, int>();
                dbInstallations = new List<AquacultureFacilityInstallation>();
            }

            List<AquacultureInstallationEditDTO> newInstallations = installations.Where(x => x.Id == null).ToList();
            List<AquacultureInstallationEditDTO> oldInstallations = installations.Where(x => x.Id != null).ToList();

            if (newInstallations.Count != 0)
            {
                AddAquacultureInstallations(aquaculture, newInstallations);
            }

            foreach (AquacultureInstallationEditDTO installation in oldInstallations)
            {
                AquacultureFacilityInstallation entry = dbInstallations.Where(x => x.Id == installation.Id).First();
                entry.InstallationTypeId = installationTypes[installation.InstallationType.Value];
                entry.Comments = installation.Comments;
                entry.IsActive = installation.IsActive.Value;

                // check if there is change in type
                InvalidateAllAquacultureInstallationDetailsExceptType(entry, installation.InstallationType.Value);

                switch (installation.InstallationType.Value)
                {
                    case AquacultureInstallationTypeEnum.Basins:
                        EditAquacultureInstallationBasins(entry, installation.Basins);
                        break;
                    case AquacultureInstallationTypeEnum.NetCages:
                        EditAquacultureInstallationNetCages(entry, installation.NetCages);
                        break;
                    case AquacultureInstallationTypeEnum.Aquariums:
                        EditAquacultureInstallationAquariums(entry, installation.Aquariums);
                        break;
                    case AquacultureInstallationTypeEnum.Collectors:
                        EditAquacultureInstallationCollectors(entry, installation.Collectors);
                        break;
                    case AquacultureInstallationTypeEnum.Rafts:
                        EditAquacultureInstallationRafts(entry, installation.Rafts);
                        break;
                    case AquacultureInstallationTypeEnum.Dams:
                        EditAquacultureInstallationDams(entry, installation.Dams);
                        break;
                    case AquacultureInstallationTypeEnum.RecirculatorySystems:
                        EditAquacultureInstallationRecirculatorySystems(entry, installation.RecirculatorySystems);
                        break;
                }
            }
        }

        private void InvalidateAllAquacultureInstallationDetailsExceptType(AquacultureFacilityInstallation installation, AquacultureInstallationTypeEnum type)
        {
            if (type != AquacultureInstallationTypeEnum.Basins)
            {
                List<AquacultureInstallationBasin> basins = GetInstallationBasins(installation.Id);

                foreach (AquacultureInstallationBasin basin in basins)
                {
                    basin.IsActive = false;
                }
            }

            if (type != AquacultureInstallationTypeEnum.NetCages)
            {
                List<AquacultureInstallationNetCage> netCages = GetInstallationNetCages(installation.Id);

                foreach (AquacultureInstallationNetCage netCage in netCages)
                {
                    netCage.IsActive = false;
                }
            }

            if (type != AquacultureInstallationTypeEnum.Aquariums)
            {
                installation.InstallationAquariumId = null;
            }

            if (type != AquacultureInstallationTypeEnum.Collectors)
            {
                List<AquacultureInstallationCollector> collectors = GetInstallationCollectors(installation.Id);

                foreach (AquacultureInstallationCollector collector in collectors)
                {
                    collector.IsActive = false;
                }
            }

            if (type != AquacultureInstallationTypeEnum.Rafts)
            {
                List<AquacultureInstallationRaft> rafts = GetInstallationRafts(installation.Id);

                foreach (AquacultureInstallationRaft raft in rafts)
                {
                    raft.IsActive = false;
                }
            }

            if (type != AquacultureInstallationTypeEnum.Dams)
            {
                installation.InstallationDamId = null;
            }

            if (type != AquacultureInstallationTypeEnum.RecirculatorySystems)
            {
                List<AquacultureInstallationRecirculatorySystem> systems = GetInstallationRecirculatorySystems(installation.Id);

                foreach (AquacultureInstallationRecirculatorySystem system in systems)
                {
                    system.IsActive = false;
                }
            }
        }

        private void EditAquacultureInstallationBasins(AquacultureFacilityInstallation installation, List<AquacultureInstallationBasinDTO> basins)
        {
            List<AquacultureInstallationBasin> dbBasins = basins.Any(x => x.Id != null)
                ? GetInstallationBasins(installation.Id)
                : new List<AquacultureInstallationBasin>();

            foreach (AquacultureInstallationBasinDTO basin in basins)
            {
                if (basin.Id == null)
                {
                    AddAquacultureInstallationBasinEntry(installation, basin);
                }
                else
                {
                    AquacultureInstallationBasin dbBasin = dbBasins.Where(x => x.Id == basin.Id).First();
                    dbBasin.IsActive = basin.IsActive.Value;

                    dbBasin.InstallationBasin.BasinPurposeTypeId = basin.BasinPurposeTypeId.Value;
                    dbBasin.InstallationBasin.BasinMaterialTypeId = basin.BasinMaterialTypeId.Value;
                    dbBasin.InstallationBasin.Area = basin.Area.Value;
                    dbBasin.InstallationBasin.Volume = basin.Volume.Value;
                    dbBasin.InstallationBasin.Count = basin.Count.Value;
                }
            }
        }

        private void EditAquacultureInstallationNetCages(AquacultureFacilityInstallation installation, List<AquacultureInstallationNetCageDTO> netCages)
        {
            List<AquacultureInstallationNetCage> dbNetCages = netCages.Any(x => x.Id != null)
                ? GetInstallationNetCages(installation.Id)
                : new List<AquacultureInstallationNetCage>();

            foreach (AquacultureInstallationNetCageDTO netCage in netCages)
            {
                if (netCage.Id == null)
                {
                    AddAquacultureInstallationNetCageEntry(installation, netCage);
                }
                else
                {
                    AquacultureInstallationNetCage dbNetCage = dbNetCages.Where(x => x.Id == netCage.Id).First();
                    dbNetCage.NetCageTypeId = netCage.NetCageTypeId.Value;
                    dbNetCage.NetCageShape = netCage.Shape.ToString();
                    dbNetCage.Area = netCage.Area.Value;
                    dbNetCage.Volume = netCage.Volume.Value;
                    dbNetCage.Count = netCage.Count.Value;
                    dbNetCage.IsActive = netCage.IsActive.Value;

                    if (netCage.Shape == AquacultureInstallationNetCageShapesEnum.Circular)
                    {
                        dbNetCage.Radius = netCage.Radius.Value;
                        dbNetCage.Length = null;
                        dbNetCage.Width = null;
                        dbNetCage.Height = netCage.Height.Value;
                    }
                    else if (netCage.Shape == AquacultureInstallationNetCageShapesEnum.Rectangular)
                    {
                        dbNetCage.Radius = null;
                        dbNetCage.Length = netCage.Length.Value;
                        dbNetCage.Width = netCage.Width.Value;
                        dbNetCage.Height = netCage.Height.Value;
                    }
                }
            }
        }

        private void EditAquacultureInstallationAquariums(AquacultureFacilityInstallation installation, AquacultureInstallationAquariumDTO aquariums)
        {
            if (installation.InstallationAquariumId.HasValue)
            {
                InstallationAquarium dbAquarium = (from aqua in Db.InstallationAquariums
                                                   where aqua.Id == installation.InstallationAquariumId.Value
                                                   select aqua).First();

                dbAquarium.Count = aquariums.Count.Value;
                dbAquarium.Volume = aquariums.Volume.Value;
            }
            else
            {
                AddAquacultureInstallationAquariums(installation, aquariums);
            }
        }

        private void EditAquacultureInstallationCollectors(AquacultureFacilityInstallation installation, List<AquacultureInstallationCollectorDTO> collectors)
        {
            List<AquacultureInstallationCollector> dbCollectors = collectors.Any(x => x.Id != null)
                ? GetInstallationCollectors(installation.Id)
                : new List<AquacultureInstallationCollector>();

            foreach (AquacultureInstallationCollectorDTO collector in collectors)
            {
                if (collector.Id == null)
                {
                    AddAquacultureInstallationCollectorEntry(installation, collector);
                }
                else
                {
                    AquacultureInstallationCollector dbCollector = dbCollectors.Where(x => x.Id == collector.Id).First();
                    dbCollector.CollectorTypeId = collector.CollectorTypeId.Value;
                    dbCollector.TotalCount = collector.TotalCount.Value;
                    dbCollector.TotalArea = collector.TotalArea.Value;
                    dbCollector.IsActive = collector.IsActive.Value;
                }
            }
        }

        private void EditAquacultureInstallationRafts(AquacultureFacilityInstallation installation, List<AquacultureInstallationRaftDTO> rafts)
        {
            List<AquacultureInstallationRaft> dbRafts = rafts.Any(x => x.Id != null)
                ? GetInstallationRafts(installation.Id)
                : new List<AquacultureInstallationRaft>();

            foreach (AquacultureInstallationRaftDTO raft in rafts)
            {
                if (raft.Id == null)
                {
                    AddAquacultureInstallationRaftEntry(installation, raft);
                }
                else
                {
                    AquacultureInstallationRaft dbRaft = dbRafts.Where(x => x.Id == raft.Id).First();
                    dbRaft.Length = raft.Length.Value;
                    dbRaft.Width = raft.Width.Value;
                    dbRaft.Area = raft.Area.Value;
                    dbRaft.Count = raft.Count.Value;
                    dbRaft.IsActive = raft.IsActive.Value;
                }
            }
        }

        private void EditAquacultureInstallationDams(AquacultureFacilityInstallation installation, AquacultureInstallationDamDTO dams)
        {
            if (installation.InstallationDamId.HasValue)
            {
                InstallationDam dbDam = (from dam in Db.InstallationDams
                                         where dam.Id == installation.InstallationDamId.Value
                                         select dam).First();

                dbDam.Area = dams.Area.Value;
            }
            else
            {
                AddAquacultureInstallationDams(installation, dams);
            }
        }

        private void EditAquacultureInstallationRecirculatorySystems(AquacultureFacilityInstallation installation, List<AquacultureInstallationRecirculatorySystemDTO> recirculatorySystems)
        {
            List<AquacultureInstallationRecirculatorySystem> dbSystems = recirculatorySystems.Any(x => x.Id != null)
                ? GetInstallationRecirculatorySystems(installation.Id)
                : new List<AquacultureInstallationRecirculatorySystem>();

            foreach (AquacultureInstallationRecirculatorySystemDTO system in recirculatorySystems)
            {
                if (system.Id == null)
                {
                    AddAquacultureInstallationRecirculatorySystemEntry(installation, system);
                }
                else
                {
                    AquacultureInstallationRecirculatorySystem dbSystem = dbSystems.Where(x => x.Id == system.Id).First();
                    dbSystem.IsActive = system.IsActive.Value;

                    dbSystem.InstallationBasin.BasinPurposeTypeId = system.BasinPurposeTypeId.Value;
                    dbSystem.InstallationBasin.BasinMaterialTypeId = system.BasinMaterialTypeId.Value;
                    dbSystem.InstallationBasin.Area = system.Area.Value;
                    dbSystem.InstallationBasin.Volume = system.Volume.Value;
                    dbSystem.InstallationBasin.Count = system.Count.Value;
                }
            }
        }

        private void EditAquacultureLogBooks(int aquacultureId, List<LogBookEditDTO> logBooks, bool ignoreLogBookConflicts)
        {
            if (logBooks != null)
            {
                foreach (LogBookEditDTO logBook in logBooks)
                {
                    if (logBook.LogBookId.HasValue)
                    {
                        Db.EditAquacultureLogBook(logBook, ignoreLogBookConflicts);
                    }
                    else
                    {
                        Db.AddAquacultureLogBook(logBook, aquacultureId, ignoreLogBookConflicts);
                    }
                }
            }
            else
            {
                List<LogBook> dbEntries = (from logBook in Db.LogBooks
                                           where logBook.AquacultureFacilityId == aquacultureId
                                                && logBook.IsActive
                                           select logBook).ToList();

                foreach (LogBook entry in dbEntries)
                {
                    entry.IsActive = false;
                }
            }
        }

        private Dictionary<AquacultureInstallationTypeEnum, int> GetInstallationTypesCodeToIdDictionary()
        {
            Dictionary<AquacultureInstallationTypeEnum, int> result = (from type in Db.NaquacultureInstallationTypes
                                                                       select new
                                                                       {
                                                                           Type = Enum.Parse<AquacultureInstallationTypeEnum>(type.Code),
                                                                           type.Id
                                                                       }).ToDictionary(x => x.Type, y => y.Id);

            return result;
        }

        private int GetAquacultureStatusId(AquacultureStatusEnum status)
        {
            DateTime now = DateTime.Now;

            int id = (from st in Db.NaquacultureStatuses
                      where st.Code == status.ToString()
                        && st.ValidFrom <= now
                        && st.ValidTo > now
                      select st.Id).Single();

            return id;
        }
    }
}
