using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.EntityModels.Interfaces;
using IARA.Infrastructure.Helpers;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.CatchSales;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.FSM;
using IARA.Interfaces.FVMSIntegrations;
using IARA.Interfaces.Legals;
using IARA.Interfaces.Nomenclatures;
using IARA.Interfaces.Reports;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;
using TL.EDelivery;

namespace IARA.Infrastructure.Services.CommercialFishing
{
    public class CommercialFishingService : Service, ICommercialFishingService
    {
        private readonly IApplicationService applicationService;
        private readonly IApplicationStateMachine stateMachine;
        private readonly IDeliveryService deliveryService;
        private readonly IFishingGearsService fishingGearsService;
        private readonly ILogBooksService logBooksService;
        private readonly IApplicationService applicationsService;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IJasperReportExecutionService jasperReportsService;
        private readonly IDuplicatesRegisterService duplicatesRegisterService;
        private readonly ScopedServiceProviderFactory serviceProviderFactory;
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        private readonly IShipsRegisterService shipsRegisterService;

        /// <summary>
        /// Платени межени уреди за Черно море
        /// </summary>
        private readonly List<string> PAID_NET_FISHING_GEAR_CODES;
        /// <summary>
        /// Платени волти и чепарета за Черно море
        /// </summary>
        private readonly List<string> PAID_POLE_AND_LINES_GEAR_CODES;
        /// <summary>
        /// Платени парагади за Черно море
        /// </summary>
        private readonly List<string> PAID_LONGLINES_GEAR_CODES;

        /// <summary>
        /// Платени мрежени уреди за р. Дунав
        /// </summary>
        private readonly List<string> PAID_DANUBE_NET_GEAR_CODES;

        public CommercialFishingService(IARADbContext db,
                                        IApplicationService applicationService,
                                        IApplicationStateMachine stateMachine,
                                        IDeliveryService deliveryService,
                                        IFishingGearsService fishingGearsService,
                                        ILogBooksService logBooksService,
                                        IApplicationService applicationsService,
                                        IRegixApplicationInterfaceService regixApplicationService,
                                        IJasperReportExecutionService jasperReportsService,
                                        IDuplicatesRegisterService duplicatesRegisterService,
                                        ScopedServiceProviderFactory serviceProviderFactory,
                                        IPersonService personService,
                                        ILegalService legalService,
                                        IShipsRegisterService shipsRegisterService)
            : base(db)
        {
            this.applicationService = applicationService;
            this.stateMachine = stateMachine;
            this.deliveryService = deliveryService;
            this.fishingGearsService = fishingGearsService;
            this.logBooksService = logBooksService;
            this.applicationsService = applicationsService;
            this.regixApplicationService = regixApplicationService;
            this.jasperReportsService = jasperReportsService;
            this.duplicatesRegisterService = duplicatesRegisterService;
            this.serviceProviderFactory = serviceProviderFactory;
            this.personService = personService;
            this.legalService = legalService;
            this.shipsRegisterService = shipsRegisterService;

            PAID_NET_FISHING_GEAR_CODES = new List<string>
            {
                nameof(FishingGearTypesEnum.GNS),
                nameof(FishingGearTypesEnum.GND),
                nameof(FishingGearTypesEnum.GNC),
                nameof(FishingGearTypesEnum.GTR),
                nameof(FishingGearTypesEnum.GTN),
                nameof(FishingGearTypesEnum.LNB),
                nameof(FishingGearTypesEnum.LNP),
                nameof(FishingGearTypesEnum.LN),
                nameof(FishingGearTypesEnum.GNF),
                nameof(FishingGearTypesEnum.GEN)
            };

            PAID_POLE_AND_LINES_GEAR_CODES = new List<string>
            {
                nameof(FishingGearTypesEnum.LHP),
                nameof(FishingGearTypesEnum.LHM)
            };

            PAID_LONGLINES_GEAR_CODES = new List<string>
            {
                nameof(FishingGearTypesEnum.LLS),
                nameof(FishingGearTypesEnum.LLD),
                nameof(FishingGearTypesEnum.LL)
            };

            PAID_DANUBE_NET_GEAR_CODES = new List<string>
            {
                nameof(FishingGearTypesEnum.GNS),
                nameof(FishingGearTypesEnum.GND)
            };
        }

        public IQueryable<CommercialFishingPermitRegisterDTO> GetAllCommercialFishingPermits(CommercialFishingRegisterFilters filters)
        {
            IQueryable<CommercialFishingPermitRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPermits(showInactive);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredPermits(filters.FreeTextSearch, filters.ShowInactiveRecords, filters.PermitTerritoryUnitId, filters.PermitLicenseTerritoryUnitId)
                    : GetParametersFilteredPermits(filters);
            }

            return result;
        }

        public List<CommercialFishingPermitLicenseRegisterDTO> GetPermitLicensesForTable(IEnumerable<int> permitIds, CommercialFishingRegisterFilters filters)
        {
            List<CommercialFishingPermitLicenseRegisterDTO> result;
            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPermitLicenses(permitIds, showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredPermitLicenses(permitIds, filters)
                    : GetFreeTextFilteredPermitLicenses(permitIds, filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            List<int> permitLicenseIds = result.Select(x => x.Id).ToList();
            ILookup<int, CommercialFishingLogbookRegisterDTO> permitLicensesLogBooks = GetPermitLicensesLogBooksLookup(permitLicenseIds);
            foreach (CommercialFishingPermitLicenseRegisterDTO permitLicense in result)
            {
                permitLicense.Logbooks = permitLicensesLogBooks[permitLicense.Id].ToList();
            }

            return result;
        }

        public CommercialFishingEditDTO GetPermit(int id)
        {
            PermitRegister dbPermit = (from permit in Db.CommercialFishingPermitRegisters
                                                        .AsSplitQuery()
                                                        .Include(x => x.PermitType)
                                                        .Include(x => x.ShipGroundsForUse)
                                                        .Include(x => x.PoundNetGroundsForUse)
                                                        .Include(x => x.PermitRegisterFiles)
                                       where permit.Id == id && permit.RecordType == nameof(RecordTypesEnum.Register)
                                       select permit).First();

            CommercialFishingEditDTO dto = MapDbPermitToDTO(dbPermit);
            return dto;
        }

        public CommercialFishingEditDTO GetPermitLicense(int id)
        {
            PermitLicensesRegister dbPermitLicense = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                              .AsSplitQuery()
                                                                              .Include(x => x.ShipGroundsForUse)
                                                                              .Include(x => x.PermitLicenseType)
                                                                              .Include(x => x.PermitLicensesRegisterFiles)
                                                      where permitLicense.Id == id && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                      select permitLicense).AsSplitQuery().First();

            CommercialFishingEditDTO dto = MapDbPermitLicenseToDTO(dbPermitLicense);

            return dto;
        }

        public CommercialFishingEditDTO GetPermitByApplicationId(int applicationId)
        {
            PermitRegister dbPermit = (from permit in Db.CommercialFishingPermitRegisters
                                                        .AsSplitQuery()
                                                        .Include(x => x.PermitType)
                                                        .Include(x => x.ShipGroundsForUse)
                                                        .Include(x => x.PoundNetGroundsForUse)
                                                        .Include(x => x.PermitRegisterFiles)
                                       where permit.ApplicationId == applicationId
                                             && permit.RecordType == nameof(RecordTypesEnum.Register)
                                             && permit.IsActive
                                       select permit).AsSplitQuery().Single();

            CommercialFishingEditDTO dto = MapDbPermitToDTO(dbPermit);

            return dto;
        }

        public CommercialFishingEditDTO GetPermitLicenseByApplicationId(int applicationId)
        {
            PermitLicensesRegister dbPermitLicense = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                              .AsSplitQuery()
                                                                              .Include(x => x.ShipGroundsForUse)
                                                                              .Include(x => x.PermitLicenseType)
                                                                              .Include(x => x.PermitLicensesRegisterFiles)
                                                      where permitLicense.ApplicationId == applicationId
                                                            && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                            && permitLicense.IsActive
                                                      select permitLicense).AsSplitQuery().Single();

            CommercialFishingEditDTO dto = MapDbPermitLicenseToDTO(dbPermitLicense);

            return dto;
        }

        public CommercialFishingApplicationEditDTO GetPermitLicenseApplicationDataFromPermit(string permitNumber, int applicationId)
        {
            int permitId = GetPermitIdByRegistrationNumber(permitNumber); // TODO if BOOM - throw exception && catch in contoller + UI
            CommercialFishingApplicationEditDTO permitLicense = GetPermitLicenseApplicationFromPermit(permitId, applicationId);

            return permitLicense;
        }

        public CommercialFishingApplicationEditDTO GetPermitLicenseApplicationDataFromPermit(int permitId, int applicationId)
        {
            CommercialFishingApplicationEditDTO permitLicense = GetPermitLicenseApplicationFromPermit(permitId, applicationId);

            return permitLicense;
        }

        public CommercialFishingEditDTO GetPermitApplicationDataForRegister(int applicationId)
        {
            PermitRegister dbPermit = (from permit in Db.CommercialFishingPermitRegisters
                                                        .AsSplitQuery()
                                                        .Include(x => x.PermitType)
                                                        .Include(x => x.ShipGroundsForUse)
                                                        .Include(x => x.PoundNetGroundsForUse)
                                                        .Include(x => x.PermitRegisterFiles)
                                       where permit.ApplicationId == applicationId
                                             && permit.RecordType == nameof(RecordTypesEnum.Application)
                                       select permit).AsSplitQuery().Single();

            CommercialFishingEditDTO result = MapDbPermitToDTO(dbPermit, isApplication: true);

            return result;
        }

        public CommercialFishingEditDTO GetPermitLicenseApplicationDataForRegister(int applicationId)
        {
            PermitLicensesRegister dbPermitLicense = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                              .AsSplitQuery()
                                                                              .Include(x => x.ShipGroundsForUse)
                                                                              .Include(x => x.PermitLicenseType)
                                                                              .Include(x => x.PermitLicensesRegisterFiles)
                                                      where permitLicense.ApplicationId == applicationId
                                                            && permitLicense.RecordType == nameof(RecordTypesEnum.Application)
                                                            && permitLicense.IsActive
                                                      select permitLicense).AsSplitQuery().Single();

            CommercialFishingEditDTO result = MapDbPermitLicenseToDTO(dbPermitLicense, isApplication: true);
            result.LogBooks = GetUnfinishedLogBooksFromOldLicenses(dbPermitLicense.PermitId);

            return result;
        }

        public async Task<DownloadableFileDTO> GetPermitRegisterFileForDownload(int registerId, CommercialFishingTypesEnum permitType, bool isDuplicate = false)
        {
            DownloadableFileDTO downloadableFile = new DownloadableFileDTO
            {
                MimeType = "application/pdf"
            };

            switch (permitType)
            {
                case CommercialFishingTypesEnum.Permit:
                    {
                        var permitData = (from permit in Db.CommercialFishingPermitRegisters
                                          join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                                          join waterType in Db.NwaterTypes on permit.WaterTypeId equals waterType.Id
                                          where permit.Id == registerId
                                          select new
                                          {
                                              ShipName = ship.Name,
                                              WaterType = Enum.Parse<WaterTypesEnum>(waterType.Code)
                                          }).First();
                        downloadableFile.FileName = $"{AppResources.basicUnlimitedCommFishPermit}_{permitData.ShipName}".Replace(" ", "");

                        if (permitData.WaterType == WaterTypesEnum.DANUBE)
                        {
                            downloadableFile.Bytes = await jasperReportsService.GetDanubePermitRegister(registerId, isDuplicate);
                        }
                        else
                        {
                            downloadableFile.Bytes = await jasperReportsService.GetBlackSeaPermitRegister(registerId, isDuplicate);
                        }
                    }
                    break;
                case CommercialFishingTypesEnum.PoundNetPermit:
                    {
                        string poundnetName = (from permit in Db.CommercialFishingPermitRegisters
                                               join poundNet in Db.PoundNetRegisters on permit.PoundNetId equals poundNet.Id
                                               where permit.Id == registerId
                                               select poundNet.Name).First();
                        downloadableFile.FileName = $"{AppResources.poundNetCommFishPermit}_{poundnetName}".Replace(" ", "");
                        downloadableFile.Bytes = await jasperReportsService.GetPoundNetPermitRegister(registerId, isDuplicate);
                    }
                    break;
                case CommercialFishingTypesEnum.ThirdCountryPermit:
                    {
                        var permitData = (from permit in Db.CommercialFishingPermitRegisters
                                          join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                                          join waterType in Db.NwaterTypes on permit.WaterTypeId equals waterType.Id
                                          where permit.Id == registerId
                                          select new
                                          {
                                              ShipName = ship.Name,
                                              WaterType = Enum.Parse<WaterTypesEnum>(waterType.Code)
                                          }).First();
                        downloadableFile.FileName = $"{AppResources.basicThirdCountyCommFishPermit}_{permitData.ShipName}".Replace(" ", "");

                        if (permitData.WaterType == WaterTypesEnum.DANUBE)
                        {
                            downloadableFile.Bytes = await jasperReportsService.GetDanubeThirdCountryPermitRegister(registerId, isDuplicate);
                        }
                        else
                        {
                            downloadableFile.Bytes = await jasperReportsService.GetBlackSeaThirdCountryPermitRegister(registerId, isDuplicate);
                        }
                    }
                    break;
            }

            return downloadableFile;
        }

        public async Task<DownloadableFileDTO> GetPermitLicenseRegisterFileForDownload(int registerId, CommercialFishingTypesEnum permitLicenseType, bool isDuplicate = false)
        {
            DownloadableFileDTO downloadableFile = new DownloadableFileDTO
            {
                MimeType = "application/pdf"
            };

            switch (permitLicenseType)
            {
                case CommercialFishingTypesEnum.PermitLicense:
                    {
                        string shipName = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                           join ship in Db.ShipsRegister on permitLicense.ShipId equals ship.Id
                                           where permitLicense.Id == registerId
                                           select ship.Name).First();
                        downloadableFile.FileName = $"{AppResources.commFishPermitLicense}_{shipName}".Replace(" ", "");
                        downloadableFile.Bytes = await jasperReportsService.GetPermitLicenseRegister(registerId, isDuplicate);
                    }
                    break;
                case CommercialFishingTypesEnum.PoundNetPermitLicense:
                    {
                        string poundnetName = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                               join poundNet in Db.PoundNetRegisters on permitLicense.PoundNetId equals poundNet.Id
                                               where permitLicense.Id == registerId
                                               select poundNet.Name).First();
                        downloadableFile.FileName = $"{AppResources.poundNetCommFishPermitLicense}_{poundnetName}".Replace(" ", "");
                        downloadableFile.Bytes = await jasperReportsService.GetPoundNetPermitLicenseRegister(registerId, isDuplicate);
                    }
                    break;
                case CommercialFishingTypesEnum.QuataSpeciesPermitLicense:
                    {
                        string shipName = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                           join ship in Db.ShipsRegister on permitLicense.ShipId equals ship.Id
                                           where permitLicense.Id == registerId
                                           select ship.Name).First();
                        downloadableFile.FileName = $"{AppResources.commFishQuotaPermitLicense}_{shipName}".Replace(" ", "");
                        downloadableFile.Bytes = await jasperReportsService.GetQuotaSpeciesPermitLicenseRegister(registerId, isDuplicate);
                    }
                    break;
            }

            return downloadableFile;
        }

        public CommercialFishingTypesEnum GetPermitType(int registerId)
        {
            CommercialFishingTypesEnum permitType = (from permit in Db.CommercialFishingPermitRegisters
                                                     join pType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals pType.Id
                                                     where permit.Id == registerId
                                                     select Enum.Parse<CommercialFishingTypesEnum>(pType.Code)).First();

            return permitType;
        }

        public CommercialFishingTypesEnum GetPermitLicenseType(int registerId)
        {
            CommercialFishingTypesEnum permitLiceseType = (from permitLicese in Db.CommercialFishingPermitLicensesRegisters
                                                           join plType in Db.NcommercialFishingPermitLicenseTypes on permitLicese.PermitLicenseTypeId equals plType.Id
                                                           where permitLicese.Id == registerId
                                                           select Enum.Parse<CommercialFishingTypesEnum>(plType.Code)).First();

            return permitLiceseType;
        }

        public int AddPermit(CommercialFishingEditDTO permit)
        {
            PermitRegister entry;
            int applicationId;
            string registrationNumber;

            using (TransactionScope scope = new TransactionScope())
            {
                var registerApplicationData = (from p in Db.CommercialFishingPermitRegisters
                                               join appl in Db.Applications on p.ApplicationId equals appl.Id
                                               where p.ApplicationId == permit.ApplicationId
                                                     && p.RecordType == nameof(RecordTypesEnum.Application)
                                               select new
                                               {
                                                   Id = p.Id,
                                                   ApplicationId = p.ApplicationId,
                                                   TerritoryUnitId = appl.TerritoryUnitId
                                               }).Single();

                applicationId = registerApplicationData.ApplicationId;

                int permitTypeId = GetTypeIdByPageCode(permit.PageCode);

                registrationNumber = GeneratePermitRegisterNumber(registerApplicationData.TerritoryUnitId.Value, permitTypeId, permit.WaterTypeId.Value);

                entry = new PermitRegister
                {
                    ApplicationId = permit.ApplicationId,
                    RegisterApplicationId = registerApplicationData.Id,
                    RecordType = nameof(RecordTypesEnum.Register),
                    PermitTypeId = permitTypeId,
                    RegistrationNum = registrationNumber,
                    ShipId = permit.ShipId.Value,
                    QualifiedFisherId = permit.QualifiedFisherId.Value,
                    WaterTypeId = permit.WaterTypeId.Value,
                    IssueDate = permit.IssueDate,
                    IsPermitUnlimited = permit.IsPermitUnlimited.Value,
                    PermitValidFrom = permit.ValidFrom,
                    IsQualifiedFisherSameAsSubmittedFor = permit.QualifiedFisherSameAsSubmittedFor
                };

                if (!permit.IsPermitUnlimited.Value)
                {
                    entry.PermitValidTo = permit.ValidTo;
                }
                else
                {
                    entry.PermitValidTo = null;
                }

                Db.AddOrEditRegisterSubmittedFor(entry, permit.SubmittedFor);

                Db.SaveChanges();

                entry.IsHolderShipOwner = permit.IsHolderShipOwner.Value;

                if (permit.PageCode == PageCodeEnum.PoundnetCommFish)
                {
                    SetPermitPoudnetIdAndGroundsForUse(entry,
                                                       permit.PoundNetId.Value,
                                                       permit.PoundNetGroundForUse);

                    AddOrEditHolderShipGroundsForUse(entry, permit.ShipGroundForUse);
                }

                if (permit.Files != null)
                {
                    foreach (FileInfoDTO file in permit.Files)
                    {
                        Db.AddOrEditFile(entry, entry.PermitRegisterFiles, file);
                    }
                }

                Db.CommercialFishingPermitRegisters.Add(entry);

                Db.SaveChanges();

                stateMachine.Act(applicationId);

                List<int> shipValidPermitIds = GetValidShipPermitIds(entry.ShipId);
                
                if (shipValidPermitIds.Count == 0)
                {
                    entry.ShipId = shipsRegisterService.EditShipRsr(permit.ShipId.Value, true);
                }

                scope.Complete();
            }

            PermitDataChanged(serviceProviderFactory, registrationNumber);

            return entry.Id;
        }

        public int AddPermitLicense(CommercialFishingEditDTO permitLicense, bool ignoreLogBookConflicts)
        {
            PermitLicensesRegister entry;

            int? registerPermitId;

            using (TransactionScope scope = new TransactionScope())
            {
                DateTime now = DateTime.Now;

                int permitLicenseTypeId = GetTypeIdByPageCode(permitLicense.PageCode);

                int? territoryUnitId = (from appl in Db.Applications
                                        where appl.Id == permitLicense.ApplicationId
                                        select appl.TerritoryUnitId).First();

                registerPermitId = GetRegisterPermitId(permitLicense.PermitLicensePermitId.Value);

                if (!registerPermitId.HasValue)
                {
                    throw new NoPermitRegisterForPermitLicenseException("Must have permit register for ship, before adding permit license");
                }

                entry = new PermitLicensesRegister
                {
                    ApplicationId = permitLicense.ApplicationId,
                    RecordType = nameof(RecordTypesEnum.Register),
                    PermitLicenseTypeId = permitLicenseTypeId,
                    PermitId = registerPermitId.Value,
                    ShipId = permitLicense.ShipId.Value,
                    QualifiedFisherId = permitLicense.QualifiedFisherId.Value,
                    WaterTypeId = permitLicense.WaterTypeId.Value,
                    IssueDate = permitLicense.IssueDate,
                    PermitLicenseValidFrom = permitLicense.ValidFrom,
                    PermitLicenseValidTo = permitLicense.ValidTo.Value,
                    IsHolderShipOwner = permitLicense.IsHolderShipOwner.Value,
                    IsQualifiedFisherSameAsSubmittedFor = permitLicense.QualifiedFisherSameAsSubmittedFor
                };

                Db.AddOrEditRegisterSubmittedFor(entry, permitLicense.SubmittedFor);

                Db.SaveChanges();

                if (permitLicense.PageCode == PageCodeEnum.PoundnetCommFishLic)
                {
                    entry.PoundNetId = permitLicense.PoundNetId;
                }
                else
                {
                    if (permitLicense.PageCode == PageCodeEnum.CatchQuataSpecies)
                    {
                        AddOrUpdatePermitLicenseAquaticOrganismTypes(entry, permitLicense.QuotaAquaticOrganisms);
                        entry.UnloaderPhoneNumber = permitLicense.UnloaderPhoneNumber;
                    }
                }

                int registerApplicationId = (from p in Db.CommercialFishingPermitLicensesRegisters
                                             join appl in Db.Applications on p.ApplicationId equals appl.Id
                                             where p.ApplicationId == permitLicense.ApplicationId
                                                   && p.RecordType == nameof(RecordTypesEnum.Application)
                                                   && p.IsActive
                                                   && appl.IsActive
                                             select p.Id).Single();

                entry.RegisterApplicationId = registerApplicationId;

                entry.RegistrationNum = GeneratePermitLicenseRegisterNumber(territoryUnitId.Value,
                                                                                     entry.PermitId,
                                                                                     permitLicense.PageCode == PageCodeEnum.CatchQuataSpecies);

                AddOrEditHolderShipGroundsForUse(entry, permitLicense.ShipGroundForUse);

                Db.CommercialFishingPermitLicensesRegisters.Add(entry);
                Db.SaveChanges();

                if (permitLicense.PageCode != PageCodeEnum.CatchQuataSpecies)
                {
                    AddOrUpdatePermitLicenseAquaticOrganismTypes(entry, permitLicense.AquaticOrganismTypeIds);
                }

                // Update all NEW fishing gear marks to be with status REGISTERED, because they are probably paid, so no need to be with status NEW
                List<FishingGearMarkDTO> newMarks = permitLicense.FishingGears.Where(x => x.Marks != null)
                                                                              .SelectMany(x => x.Marks)
                                                                              .Where(x => x.SelectedStatus == FishingGearMarkStatusesEnum.NEW)
                                                                              .Select(x => x).ToList();

                int registeredStatusId = (from markStatus in Db.NfishingGearMarkStatuses
                                          where markStatus.Code == nameof(FishingGearMarkStatusesEnum.REGISTERED)
                                                && markStatus.ValidFrom <= now
                                                && markStatus.ValidTo > now
                                          select markStatus.Id).Single();

                foreach (FishingGearMarkDTO mark in newMarks)
                {
                    mark.StatusId = registeredStatusId;
                    mark.SelectedStatus = FishingGearMarkStatusesEnum.REGISTERED;
                }

                AddOrEditFishingGears(entry, permitLicense.FishingGears);

                if (permitLicense.LogBooks != null)
                {
                    foreach (CommercialFishingLogBookEditDTO licenseLogBook in permitLicense.LogBooks)
                    {
                        AddOrEditLogBook(licenseLogBook, entry, ignoreLogBookConflicts);
                    }
                }

                if (permitLicense.Files != null)
                {
                    foreach (FileInfoDTO file in permitLicense.Files)
                    {
                        Db.AddOrEditFile(entry, entry.PermitLicensesRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                stateMachine.Act(permitLicense.ApplicationId);

                scope.Complete();
            }

            PermitDataChanged(serviceProviderFactory, registerPermitId.Value);

            return entry.Id;
        }

        public void EditPermit(CommercialFishingEditDTO permit, int currentUserId)
        {
            PermitRegister dbPermit;
            using (TransactionScope scope = new TransactionScope())
            {
                dbPermit = (from p in Db.CommercialFishingPermitRegisters
                                        .AsSplitQuery()
                                        .Include(x => x.PermitRegisterFiles)
                                        .Include(x => x.PoundNetGroundsForUse)
                                        .Include(x => x.ShipGroundsForUse)
                            where p.Id == permit.Id
                            select p).AsSplitQuery().First();

                dbPermit.IssueDate = permit.IssueDate;
                dbPermit.IsPermitUnlimited = permit.IsPermitUnlimited;
                dbPermit.PermitValidFrom = permit.ValidFrom;

                if (permit.Suspensions != null)
                {
                    AddOrEditPermitSuspensions(dbPermit, permit.Suspensions, currentUserId);
                }

                dbPermit.QualifiedFisherId = permit.QualifiedFisherId.Value;

                dbPermit.WaterTypeId = permit.WaterTypeId.Value;
                dbPermit.IsQualifiedFisherSameAsSubmittedFor = permit.QualifiedFisherSameAsSubmittedFor;

                if (!permit.IsPermitUnlimited.Value)
                {
                    dbPermit.PermitValidTo = permit.ValidTo.Value;
                }
                else
                {
                    dbPermit.PermitValidTo = null;
                }

                Db.AddOrEditRegisterSubmittedFor(dbPermit, permit.SubmittedFor);

                if (permit.PageCode == PageCodeEnum.PoundnetCommFish)
                {
                    SetPermitPoudnetIdAndGroundsForUse(dbPermit,
                                                       permit.PoundNetId.Value,
                                                       permit.PoundNetGroundForUse);

                    dbPermit.IsHolderShipOwner = permit.IsHolderShipOwner.Value;
                    AddOrEditHolderShipGroundsForUse(dbPermit, permit.ShipGroundForUse);
                }

                if (permit.Files != null)
                {
                    foreach (FileInfoDTO file in permit.Files)
                    {
                        Db.AddOrEditFile(dbPermit, dbPermit.PermitRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                scope.Complete();
            }

            PermitDataChanged(serviceProviderFactory, dbPermit.RegistrationNum);
        }

        public void EditPermitLicense(CommercialFishingEditDTO permitLicense, int currentUserId, bool ignoreLogBookConflicts)
        {
            PermitLicensesRegister dbPermitLicense;
            using (TransactionScope scope = new TransactionScope())
            {
                DateTime now = DateTime.Now;

                dbPermitLicense = (from p in Db.CommercialFishingPermitLicensesRegisters
                                               .AsSplitQuery()
                                               .Include(x => x.PermitLicensesRegisterFiles)
                                               .Include(x => x.ShipGroundsForUse)
                                   where p.Id == permitLicense.Id
                                   select p).AsSplitQuery().First();

                dbPermitLicense.IssueDate = permitLicense.IssueDate;
                dbPermitLicense.PermitLicenseValidFrom = permitLicense.ValidFrom;
                dbPermitLicense.PermitLicenseValidTo = permitLicense.ValidTo.Value;

                if (permitLicense.Suspensions != null)
                {
                    AddOrEditPermitLicenseSuspensions(dbPermitLicense, permitLicense.Suspensions, currentUserId);
                }

                dbPermitLicense.QualifiedFisherId = permitLicense.QualifiedFisherId.Value;

                if (permitLicense.PageCode == PageCodeEnum.CatchQuataSpecies)
                {
                    AddOrUpdatePermitLicenseAquaticOrganismTypes(dbPermitLicense, permitLicense.QuotaAquaticOrganisms);
                    dbPermitLicense.UnloaderPhoneNumber = permitLicense.UnloaderPhoneNumber;
                }

                dbPermitLicense.WaterTypeId = permitLicense.WaterTypeId.Value;
                dbPermitLicense.IsQualifiedFisherSameAsSubmittedFor = permitLicense.QualifiedFisherSameAsSubmittedFor;
                dbPermitLicense.IsHolderShipOwner = permitLicense.IsHolderShipOwner.Value;

                Db.AddOrEditRegisterSubmittedFor(dbPermitLicense, permitLicense.SubmittedFor);

                AddOrEditHolderShipGroundsForUse(dbPermitLicense, permitLicense.ShipGroundForUse);

                if (permitLicense.PageCode != PageCodeEnum.CatchQuataSpecies)
                {
                    AddOrUpdatePermitLicenseAquaticOrganismTypes(dbPermitLicense, permitLicense.AquaticOrganismTypeIds);
                }

                AddOrEditFishingGears(dbPermitLicense, permitLicense.FishingGears);

                if (permitLicense.LogBooks != null)
                {
                    foreach (CommercialFishingLogBookEditDTO licenseLogBook in permitLicense.LogBooks.OrderBy(x => x.LogBookIsActive ? 1 : 0))
                    {
                        AddOrEditLogBook(licenseLogBook, dbPermitLicense, ignoreLogBookConflicts);
                    }
                }

                if (permitLicense.Files != null)
                {
                    foreach (FileInfoDTO file in permitLicense.Files)
                    {
                        Db.AddOrEditFile(dbPermitLicense, dbPermitLicense.PermitLicensesRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                scope.Complete();
            }

            PermitDataChanged(serviceProviderFactory, dbPermitLicense.PermitId);
        }

        public CommercialFishingApplicationEditDTO GetPermitApplication(int applicationId)
        {
            PermitRegister dbPermit = (from permit in Db.CommercialFishingPermitRegisters
                                                        .AsSplitQuery()
                                                        .Include(x => x.ShipGroundsForUse)
                                                        .Include(x => x.PoundNetGroundsForUse)
                                                        .Include(x => x.PermitRegisterFiles)
                                       where permit.ApplicationId == applicationId
                                           && permit.RecordType == nameof(RecordTypesEnum.Application)
                                       select permit).AsSplitQuery().SingleOrDefault();

            CommercialFishingApplicationEditDTO result = null;

            if (dbPermit == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<CommercialFishingApplicationEditDTO>(draft);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                    result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                    if (result.IsPaid && result.PaymentInformation == null)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
                else
                {
                    result = new CommercialFishingApplicationEditDTO
                    {
                        IsPaid = applicationService.IsApplicationPaid(applicationId),
                        HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };

                    if (result.IsPaid)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
            }
            else
            {
                result = MapdbPermitToCommercialFishingApplicationDTOCommonFileds(dbPermit);

                if (result.PageCode == PageCodeEnum.PoundnetCommFish)
                {
                    result.PoundNetGroundForUse = MapGroundForUseEntityToDto(dbPermit.PoundNetGroundsForUse);
                }

                result.Files = Db.GetFiles(Db.CommercialFishingPermitRegisterFiles, result.Id.Value);
            }

            return result;
        }

        public CommercialFishingApplicationEditDTO GetPermitLicenseApplication(int applicationId)
        {
            PermitLicensesRegister dbPermitLicense = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                              .AsSplitQuery()
                                                                              .Include(x => x.ShipGroundsForUse)
                                                                              .Include(x => x.PermitLicensesRegisterFiles)
                                                      where permitLicense.ApplicationId == applicationId
                                                          && permitLicense.RecordType == nameof(RecordTypesEnum.Application)
                                                      select permitLicense).AsSplitQuery().SingleOrDefault();

            CommercialFishingApplicationEditDTO result = null;

            if (dbPermitLicense == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<CommercialFishingApplicationEditDTO>(draft);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                    result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                    if (result.IsPaid && result.PaymentInformation == null)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
                else
                {
                    result = new CommercialFishingApplicationEditDTO
                    {
                        IsPaid = applicationService.IsApplicationPaid(applicationId),
                        HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };

                    if (result.IsPaid)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
            }
            else
            {
                result = MapDbPermitLicenseToPermitLicenseApplicationDto(dbPermitLicense);
            }

            return result;
        }

        public CommercialFishingApplicationEditDTO GetPermitLicenseForRenewal(int permitLicenseId)
        {
            PermitLicensesRegister dbPermitLicense = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                              .AsSplitQuery()
                                                                              .Include(x => x.ShipGroundsForUse)
                                                                              .Include(x => x.PermitLicensesRegisterFiles)
                                                      where permitLicense.Id == permitLicenseId
                                                      select permitLicense).AsSplitQuery().First();

            CommercialFishingApplicationEditDTO result = MapDbPermitLicenseToPermitLicenseApplicationDto(dbPermitLicense, true);

            return result;
        }

        public RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> GetPermitRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> result = MapCommonRegixDataFieldsToDTO(applicationId);
            return result;
        }

        public RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> GetPermitLicenseRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> result = MapCommonRegixDataFieldsToDTO(applicationId);
            return result;
        }

        public async Task<List<CommercialFishingValidationErrorsEnum>> ValidateApplicationData(PageCodeEnum pageCode,
                                                                                               ApplicationSubmittedForRegixDataDTO submittedFor,
                                                                                               ApplicationSubmittedByRegixDataDTO submittedBy,
                                                                                               QualifiedFisherBasicDataDTO qualifiedFisher,
                                                                                               int shipId,
                                                                                               int deliveryTypeId,
                                                                                               int waterTypeId,
                                                                                               bool? isHolderShipOwner,
                                                                                               string permitRegistrationNumber = null)
        {
            List<CommercialFishingValidationErrorsEnum> validationErrors = new List<CommercialFishingValidationErrorsEnum>();

            int shipUId = GetShipUId(shipId);
            List<int> shipIds = GetShipIds(shipUId);
            WaterTypesEnum selectedWaterType = (from waterType in Db.NwaterTypes
                                                where waterType.Id == waterTypeId
                                                select Enum.Parse<WaterTypesEnum>(waterType.Code)).First();

            // Избраният кораб трябва да отговаря на определни условия при различните типове разрешителни/удостоверения
            // и ако не отговаря на условията, няма да е възможно добавянето на запис за разрешително/удостоверение
            switch (pageCode)
            {
                case PageCodeEnum.CommFish:
                    {
                        bool isShipThirdCountry = IsShipThirdCountry(shipUId);
                        bool hasValidPermit = HasShipValidPermit(shipIds, selectedWaterType);

                        if (hasValidPermit && selectedWaterType == WaterTypesEnum.BLACK_SEA)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipAlreadyHasValidBlackSeaPermit);
                        }
                        else if (hasValidPermit && selectedWaterType == WaterTypesEnum.DANUBE)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipAlreadyHasValidDanubePermit);
                        }

                        if (isShipThirdCountry)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipIsThirdCountry);
                        }

                    }
                    break;
                case PageCodeEnum.PoundnetCommFish:
                    {
                        bool hasValidPoundNetPermit = HasShipValidPoundNetPermit(shipIds);
                        bool isShipThirdCountry = IsShipThirdCountry(shipUId);

                        if (hasValidPoundNetPermit)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipAlreadyHasValidPoundNetPermit);
                        }
                        else if (isShipThirdCountry)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipIsThirdCountry);
                        }
                    }
                    break;
                case PageCodeEnum.RightToFishThirdCountry:
                    {
                        bool isShipThirdCountry = IsShipThirdCountry(shipUId);
                        bool hasValidPermit = HasShipValidPermit(shipIds, selectedWaterType);

                        if (isShipThirdCountry && hasValidPermit && selectedWaterType == WaterTypesEnum.BLACK_SEA)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipAlreadyHasValidBlackSeaPermit);
                        }
                        else if (isShipThirdCountry && hasValidPermit && selectedWaterType == WaterTypesEnum.DANUBE)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipAlreadyHasValidDanubePermit);
                        }
                        else if (!isShipThirdCountry)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipIsNotThirdCountry);
                        }
                    }
                    break;
                case PageCodeEnum.RightToFishResource:
                    {
                        bool hasPermitApplication = HasShipPermitApplication(shipIds, selectedWaterType);

                        if (!hasPermitApplication && selectedWaterType == WaterTypesEnum.BLACK_SEA)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipHasNoValidBlackSeaPermit);
                        }
                        else if (!hasPermitApplication && selectedWaterType == WaterTypesEnum.DANUBE)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipHasNoValidDanubePermit);
                        }
                    }
                    break;
                case PageCodeEnum.PoundnetCommFishLic:
                    {
                        bool hasPoundNetPermitApplication = HasShipPoundNetPermitApplication(shipIds);

                        if (!hasPoundNetPermitApplication)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipHasNoPoundNetPermit);
                        }

                        bool isShipThirdCountry = IsShipThirdCountry(shipUId);

                        if (isShipThirdCountry)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipIsThirdCountry);
                        }
                    }
                    break;
                case PageCodeEnum.CatchQuataSpecies:
                    {
                        bool isShipThirdCountry = IsShipThirdCountry(shipUId);
                        bool hasPermitApplication = HasShipPermitApplication(shipIds, selectedWaterType);

                        if (!hasPermitApplication && selectedWaterType == WaterTypesEnum.BLACK_SEA)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipHasNoValidBlackSeaPermit);
                        }
                        else if (!hasPermitApplication && selectedWaterType == WaterTypesEnum.DANUBE)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipHasNoValidDanubePermit);
                        }

                        if (isShipThirdCountry)
                        {
                            validationErrors.Add(CommercialFishingValidationErrorsEnum.ShipIsThirdCountry);
                        }
                    }
                    break;
            }

            // При обикновено разрешително, такова за далян и удостоверение за далян за стопански риболов получателят трябва да е собственик на кораба
            // (За даляни само ако е отбелязано, че титулярът е собственик на кораба)
            if (pageCode == PageCodeEnum.CommFish
                || ((pageCode == PageCodeEnum.PoundnetCommFish || pageCode == PageCodeEnum.PoundnetCommFishLic)
                     && isHolderShipOwner.Value)
               )
            {
                if (SubmittedForIsShipOwnerCheck(submittedFor, submittedBy, shipUId) == false) // валидационна грешка
                {
                    validationErrors.Add(CommercialFishingValidationErrorsEnum.PermitSubmittedForNotShipOwner);
                }
            }

            // Капитанът трябва да бъде правоспособен рибар
            if (CaptainIsQualifiedFisherCheck(qualifiedFisher) == false)  // валидационна грешка
            {
                validationErrors.Add(CommercialFishingValidationErrorsEnum.CaptainNotQualifiedFisherCheck);
            }

            // При избран начин на връчване e-delivery, тогава трябва получателят на услугата да има достъп до e-delivery системата
            if (await deliveryService.HasSubmittedForEDelivery(deliveryTypeId, submittedFor, submittedBy) == false)
            {
                validationErrors.Add(CommercialFishingValidationErrorsEnum.NoEDeliveryRegistration);
            }

            // При удостоверения в публичното приложение (тоест при наличие на номер на разрешително, а не ID),
            // трябва номерът да е на съществуващ регистровв запис на разрешително
            if (!string.IsNullOrWhiteSpace(permitRegistrationNumber)
                && (pageCode == PageCodeEnum.PoundnetCommFishLic || pageCode == PageCodeEnum.RightToFishResource || pageCode == PageCodeEnum.CatchQuataSpecies)
                )
            {
                int? permitId = GetPermitIdByRegistraionNumber(permitRegistrationNumber, shipIds);
                if (!permitId.HasValue)
                {
                    validationErrors.Add(CommercialFishingValidationErrorsEnum.InvalidPermitRegistrationNumber);
                }
            }

            return validationErrors;
        }

        public List<CommercialFishingValidationErrorsEnum> ValidateApplicationRegiXData(PageCodeEnum pageCode,
                                                                                        ApplicationSubmittedForRegixDataDTO submittedFor,
                                                                                        ApplicationSubmittedByRegixDataDTO submittedBy,
                                                                                        int id,
                                                                                        bool isPermit)
        {
            List<CommercialFishingValidationErrorsEnum> validationErrors = new List<CommercialFishingValidationErrorsEnum>();

            // При обикновено разрешително за стопански риболов получателят трябва да е собственик на кораба
            if (pageCode == PageCodeEnum.CommFish || pageCode == PageCodeEnum.PoundnetCommFish || pageCode == PageCodeEnum.PoundnetCommFishLic)
            {
                int shipId;

                if (isPermit)
                {
                    shipId = (from permit in Db.CommercialFishingPermitRegisters
                              where permit.Id == id
                              select permit.ShipId).First();
                }
                else
                {
                    shipId = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                              where permitLicense.Id == id
                              select permitLicense.ShipId).First();
                }

                int shipUId = GetShipUId(shipId);

                if (SubmittedForIsShipOwnerCheck(submittedFor, submittedBy, shipUId) == false) // валидационна грешка
                {
                    validationErrors.Add(CommercialFishingValidationErrorsEnum.PermitSubmittedForNotShipOwner);
                }
            }

            return validationErrors;
        }

        public int AddPermitApplication(CommercialFishingApplicationEditDTO permit, ApplicationStatusesEnum? nextManualStatus)
        {
            PermitRegister dbPermit;

            using (TransactionScope scope = new TransactionScope())
            {
                dbPermit = AddPermitApplicationNoTransaction(permit);

                scope.Complete();
            }

            if (nextManualStatus == null)
            {
                List<FileInfoDTO> permitFiles = permit.Files;
                permit.Files = null;
                stateMachine.Act(id: dbPermit.ApplicationId, draftContent: CommonUtils.Serialize(permit), files: permitFiles);
            }
            else
            {
                stateMachine.Act(dbPermit.ApplicationId, nextManualStatus);
            }

            return dbPermit.Id;
        }

        public CommercialFishingApplicationEditDTO AddPermitApplicationAndStartPermitLicenseApplication(CommercialFishingApplicationEditDTO permit,
                                                                                                        int currentUserId,
                                                                                                        ApplicationStatusesEnum? nextManualStatus)
        {
            PermitRegister dbPermit;

            using (TransactionScope scope = new TransactionScope())
            {
                dbPermit = AddPermitApplicationNoTransaction(permit);
                scope.Complete();
            }

            List<FileInfoDTO> permitFiles = permit.Files;
            permit.Files = null;
            stateMachine.Act(dbPermit.ApplicationId, CommonUtils.Serialize(permit), permitFiles, nextManualStatus);

            Application dbPermitApplication = (from appl in Db.Applications
                                               where appl.Id == dbPermit.ApplicationId
                                               select appl).First();

            ApplicationHierarchyTypesEnum applicationHierarchyType = (from applHierType in Db.NapplicationStatusHierarchyTypes
                                                                      where applHierType.Id == dbPermitApplication.ApplicationStatusHierTypeId
                                                                      select Enum.Parse<ApplicationHierarchyTypesEnum>(applHierType.Code)).Single();

            Tuple<int, string> permitLicenseApplicationData = AddApplicationForPermitLicense(dbPermitApplication,
                                                                                              currentUserId,
                                                                                              applicationHierarchyType);

            Application permitLicenseApplication = (from appl in Db.Applications
                                                    where appl.Id == permitLicenseApplicationData.Item1
                                                    select appl).First();

            // Ако е на хартия подадено, трябва да преминем през входиране и присвояване на служител от ИАРА
            if (applicationHierarchyType == ApplicationHierarchyTypesEnum.OnPaper)
            {
                stateMachine.Act(permitLicenseApplication.Id, new EventisDataDTO { EventisNumber = dbPermitApplication.EventisNum });

                permitLicenseApplication.AssignedUserId = dbPermitApplication.AssignedUserId;
                permitLicenseApplication.TerritoryUnitId = dbPermitApplication.TerritoryUnitId;

                stateMachine.Act(permitLicenseApplication.Id, draftContent: null, files: null);
            }

            CommercialFishingApplicationEditDTO permitLincese = GetPermitLicenseApplicationFromPermit(dbPermit.Id, permitLicenseApplication.Id);

            return permitLincese;
        }

        public int AddPermitLicenseApplication(CommercialFishingApplicationEditDTO permitLicense,
                                               bool isFromPublicApp = false,
                                               ApplicationStatusesEnum? nextManualStatus = null)
        {
            Application application;
            PermitLicensesRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                DateTime now = DateTime.Now;

                int permitLicenseTypeId = GetTypeIdByPageCode(permitLicense.PageCode.Value);

                int permitLicensePermitId;
                if (isFromPublicApp)
                {
                    int shipUId = GetShipUId(permitLicense.ShipId.Value);
                    List<int> shipIds = GetShipIds(shipUId);

                    permitLicensePermitId = GetPermitIdByRegistraionNumber(permitLicense.PermitLicensePermitNumber, shipIds).Value;
                }
                else
                {
                    permitLicensePermitId = permitLicense.PermitLicensePermitId.Value;
                }

                entry = new PermitLicensesRegister
                {
                    ApplicationId = permitLicense.ApplicationId.Value,
                    RecordType = nameof(RecordTypesEnum.Application),
                    PermitLicenseTypeId = permitLicenseTypeId,
                    PermitId = permitLicensePermitId,
                    ShipId = permitLicense.ShipId.Value,
                    WaterTypeId = permitLicense.WaterTypeId.Value
                };

                application = (from appl in Db.Applications
                               where appl.Id == entry.ApplicationId
                               select appl).First();

                FillAddApplicationRegisterCommonFields(entry, permitLicense, application);

                if (permitLicense.PageCode == PageCodeEnum.PoundnetCommFishLic)
                {
                    entry.PoundNetId = permitLicense.PoundNetId;
                }
                else
                {
                    if (permitLicense.PageCode == PageCodeEnum.CatchQuataSpecies)
                    {
                        AddOrUpdatePermitLicenseAquaticOrganismTypes(entry, permitLicense.QuotaAquaticOrganisms);
                        entry.UnloaderPhoneNumber = permitLicense.UnloaderPhoneNumber;
                    }
                }

                Db.CommercialFishingPermitLicensesRegisters.Add(entry);

                entry.IsHolderShipOwner = permitLicense.IsHolderShipOwner.Value;

                AddOrEditHolderShipGroundsForUse(entry, permitLicense.ShipGroundForUse);

                Db.SaveChanges();

                if (permitLicense.PageCode != PageCodeEnum.CatchQuataSpecies)
                {
                    AddOrUpdatePermitLicenseAquaticOrganismTypes(entry, permitLicense.AquaticOrganismTypeIds);
                }

                AddOrEditFishingGears(entry, permitLicense.FishingGears);

                bool isPaid = (from appl in Db.Applications
                               join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                               where appl.Id == permitLicense.ApplicationId.Value
                               select applType.IsPaid).First();

                if (isPaid)
                {
                    List<PaymentTariffDTO> appliedTariffs = GetPermitLicenseAppliedTariffs(permitLicense);

                    ApplicationPayment applicationPayment = GetApplicationPayment(permitLicense.ApplicationId.Value);

                    List<PaymentTariffDTO> appliedCalculatedTariffs = appliedTariffs.Where(x => x.IsCalculated).ToList();

                    foreach (PaymentTariffDTO paymentTariff in appliedCalculatedTariffs)
                    {
                        Db.AddOrEditApplicationPaymentTariff(applicationPayment, paymentTariff, null);
                    }
                }

                if (permitLicense.Files != null)
                {
                    foreach (FileInfoDTO file in permitLicense.Files)
                    {
                        Db.AddOrEditFile(entry, entry.PermitLicensesRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                scope.Complete();
            }

            if (nextManualStatus == null)
            {
                List<FileInfoDTO> permitFiles = permitLicense.Files;
                permitLicense.Files = null;
                stateMachine.Act(id: entry.ApplicationId, draftContent: CommonUtils.Serialize(permitLicense), files: permitFiles);
            }
            else
            {
                stateMachine.Act(entry.ApplicationId, nextManualStatus);
            }

            return entry.Id;
        }

        private List<PaymentTariffDTO> GetPermitLicenseAppliedTariffs(CommercialFishingApplicationEditDTO permitLicense)
        {
            PermitLicenseTariffCalculationParameters tariffsParameters = new PermitLicenseTariffCalculationParameters
            {
                ApplicationId = permitLicense.ApplicationId.Value,
                PageCode = permitLicense.PageCode.Value,
                ShipId = permitLicense.ShipId.Value,
                WaterTypeId = permitLicense.WaterTypeId.Value,
                FishingGears = permitLicense.FishingGears,
                PoundNetId = permitLicense.PoundNetId
            };

            if (permitLicense.PageCode == PageCodeEnum.CatchQuataSpecies)
            {
                tariffsParameters.AquaticOrganismTypeIds = permitLicense.QuotaAquaticOrganisms.Select(x => x.AquaticOrganismId).ToList();
            }
            else
            {
                tariffsParameters.AquaticOrganismTypeIds = permitLicense.AquaticOrganismTypeIds;
            }

            List<PaymentTariffDTO> appliedTariffs = CalculatePermitLicenseAppliedTariffs(tariffsParameters);

            return appliedTariffs;
        }

        public void EditPermitApplication(CommercialFishingApplicationEditDTO permit, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                PermitRegister dbPermit = (from p in Db.CommercialFishingPermitRegisters
                                                       .AsSplitQuery()
                                                       .Include(x => x.PermitRegisterFiles)
                                                       .Include(x => x.PoundNetGroundsForUse)
                                                       .Include(x => x.ShipGroundsForUse)
                                           where p.Id == permit.Id
                                           select p).AsSplitQuery().First();

                application = (from appl in Db.Applications
                               where appl.Id == dbPermit.ApplicationId
                               select appl).First();

                if (permit.PageCode == PageCodeEnum.PoundnetCommFish)
                {
                    SetPermitPoudnetIdAndGroundsForUse(dbPermit,
                                                       permit.PoundNetId.Value,
                                                       permit.PoundNetGroundForUse);

                    AddOrEditHolderShipGroundsForUse(dbPermit, permit.ShipGroundForUse);
                }

                FillAddApplicationRegisterCommonFields(dbPermit, permit, application);

                Db.EditDeliveryData(permit.DeliveryData, application.DeliveryId.Value);

                if (permit.Files != null)
                {
                    foreach (FileInfoDTO file in permit.Files)
                    {
                        Db.AddOrEditFile(dbPermit, dbPermit.PermitRegisterFiles, file);
                    }
                }

                scope.Complete();
            }

            List<FileInfoDTO> permitFiles = permit.Files;
            permit.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(permit), permitFiles, manualStatus, permit.StatusReason);
        }

        public void EditPermitLicenseApplication(CommercialFishingApplicationEditDTO permitLicense, bool isFromPublicApp = false, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                int permitLicensePermitId;
                if (isFromPublicApp)
                {
                    int shipUId = GetShipUId(permitLicense.ShipId.Value);
                    List<int> shipIds = GetShipIds(shipUId);
                    permitLicensePermitId = GetPermitIdByRegistraionNumber(permitLicense.PermitLicensePermitNumber, shipIds).Value;
                }
                else
                {
                    permitLicensePermitId = permitLicense.PermitLicensePermitId.Value;
                }

                PermitLicensesRegister dbPermitLicense = (from p in Db.CommercialFishingPermitLicensesRegisters
                                                                      .AsSplitQuery()
                                                                      .Include(x => x.PermitLicensesRegisterFiles)
                                                                      .Include(x => x.ShipGroundsForUse)
                                                          where p.Id == permitLicense.Id
                                                          select p).AsSplitQuery().First();

                application = (from appl in Db.Applications
                               where appl.Id == dbPermitLicense.ApplicationId
                               select appl).First();

                dbPermitLicense.ShipId = permitLicense.ShipId.Value;
                dbPermitLicense.PermitId = permitLicensePermitId;

                if (permitLicense.PageCode == PageCodeEnum.PoundnetCommFishLic)
                {
                    dbPermitLicense.PoundNetId = permitLicense.PoundNetId;
                }
                else
                {
                    if (permitLicense.PageCode == PageCodeEnum.CatchQuataSpecies)
                    {
                        AddOrUpdatePermitLicenseAquaticOrganismTypes(dbPermitLicense, permitLicense.QuotaAquaticOrganisms);
                        dbPermitLicense.UnloaderPhoneNumber = permitLicense.UnloaderPhoneNumber;
                    }
                }

                FillAddApplicationRegisterCommonFields(dbPermitLicense, permitLicense, application);

                AddOrEditHolderShipGroundsForUse(dbPermitLicense, permitLicense.ShipGroundForUse);

                if (permitLicense.PageCode != PageCodeEnum.CatchQuataSpecies)
                {
                    AddOrUpdatePermitLicenseAquaticOrganismTypes(dbPermitLicense, permitLicense.AquaticOrganismTypeIds);
                }

                bool isPaid = (from appl in Db.Applications
                               join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                               where appl.Id == permitLicense.ApplicationId.Value
                               select applType.IsPaid).First();

                if (isPaid)
                {
                    ApplicationPayment applicationPayment = GetApplicationPayment(permitLicense.ApplicationId.Value);

                    PaymentStatusesEnum paymentStatus = (from ps in Db.NPaymentStatuses
                                                         where ps.Id == applicationPayment.PaymentStatusId
                                                         select Enum.Parse<PaymentStatusesEnum>(ps.Code)).First();

                    // TODO - the case when the application is ALREADY PAID but it is returned for corrections
                    // and the application must be paid AGAIN with a new payment value

                    if (paymentStatus != PaymentStatusesEnum.PaidOK && paymentStatus != PaymentStatusesEnum.NotNeeded)
                    {
                        List<PaymentTariffDTO> appliedTariffs = GetPermitLicenseAppliedTariffs(permitLicense);
                        List<ApplicationPaymentTariff> oldAppliedTariffs = GetApplicationPaymenTariffs(permitLicense.ApplicationId.Value);

                        List<PaymentTariffDTO> appliedCalculatedTariffs = appliedTariffs.Where(x => x.IsCalculated).ToList();

                        foreach (PaymentTariffDTO paymentTariff in appliedCalculatedTariffs)
                        {
                            int? oldPaymentTariffId = oldAppliedTariffs.Where(x => x.TariffId == paymentTariff.TariffId).Select(x => x.Id).SingleOrDefault();
                            Db.AddOrEditApplicationPaymentTariff(applicationPayment, paymentTariff, oldPaymentTariffId);
                        }

                        List<ApplicationPaymentTariff> oldAppliedTariffsToDelete = oldAppliedTariffs.Where(x => !appliedCalculatedTariffs
                                                                                                                .Any(y => y.TariffId == x.TariffId))
                                                                                                    .Select(x => x).ToList();

                        foreach (ApplicationPaymentTariff paymentTariff in oldAppliedTariffsToDelete)
                        {
                            Db.AddOrEditApplicationPaymentTariff(applicationPayment, null, paymentTariff.Id);
                        }
                    }
                }

                Db.EditDeliveryData(permitLicense.DeliveryData, application.DeliveryId.Value);

                AddOrEditFishingGears(dbPermitLicense, permitLicense.FishingGears);

                if (permitLicense.Files != null)
                {
                    foreach (FileInfoDTO file in permitLicense.Files)
                    {
                        Db.AddOrEditFile(dbPermitLicense, dbPermitLicense.PermitLicensesRegisterFiles, file);
                    }
                }

                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> permitLicenseFiles = permitLicense.Files;
            permitLicense.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(permitLicense), permitLicenseFiles, manualStatus, permitLicense.StatusReason);
        }

        public void EditPermitApplicationRegixData(CommercialFishingRegixDataDTO permit)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {

                PermitRegister dbPermit = (from p in Db.CommercialFishingPermitRegisters
                                           where p.Id == permit.Id
                                           select p).First();

                application = (from appl in Db.Applications
                               where appl.Id == dbPermit.ApplicationId
                               select appl).First();

                Db.AddOrEditApplicationSubmittedByRegixData(application, permit.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedForRegixData(application, permit.SubmittedFor);
                Db.SaveChanges();

                dbPermit.SubmittedForPersonId = application.SubmittedForPersonId;
                dbPermit.SubmittedForLegalId = application.SubmittedForLegalId;

                scope.Complete();
            }

            stateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public void EditPermitLicenseApplicationRegixData(CommercialFishingRegixDataDTO permitLicense)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                PermitLicensesRegister dbPermitLicense = (from p in Db.CommercialFishingPermitLicensesRegisters
                                                          where p.Id == permitLicense.Id
                                                          select p).First();

                application = (from appl in Db.Applications
                               where appl.Id == dbPermitLicense.ApplicationId
                               select appl).First();

                Db.AddOrEditApplicationSubmittedByRegixData(application, permitLicense.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedForRegixData(application, permitLicense.SubmittedFor);
                Db.SaveChanges();

                dbPermitLicense.SubmittedForPersonId = application.SubmittedForPersonId;
                dbPermitLicense.SubmittedForLegalId = application.SubmittedForLegalId;


                scope.Complete();
            }

            stateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public SimpleAuditDTO GetPermitLicenseSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CommercialFishingPermitLicensesRegisters, id);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CommercialFishingPermitRegisters, id);
        }

        public SimpleAuditDTO GetPermitSuspensionSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CommercialFishingPermitSuspensionChangeHistories, id);
        }

        public SimpleAuditDTO GetPermitLicenseSuspensionSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CommercialFishingPermitLicenseSuspensionChangeHistories, id);
        }

        public CommercialFishingRegixDataDTO GetApplicationRegixData(int applicationId)
        {
            CommercialFishingApplicationDataIds dataIds = GetApplicationIds(applicationId);

            CommercialFishingRegixDataDTO regixData = new CommercialFishingRegixDataDTO
            {
                Id = dataIds.PermitId,
                ApplicationId = dataIds.ApplicationId,
                PageCode = dataIds.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(dataIds.ApplicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(dataIds.ApplicationId);

            return regixData;
        }

        public List<PermitLicenseForRenewalDTO> GetPermitLicensesForRenewal(string permitNumber, PageCodeEnum pageCode)
        {
            int permitId = GetPermitIdByRegistrationNumber(permitNumber); // TODO if BOOM - throw exception && catch in contoller + UI
            return GetPermitLicensesForRenewal(permitId, pageCode);
        }


        public List<PermitLicenseForRenewalDTO> GetPermitLicensesForRenewal(int permitId, PageCodeEnum pageCode)
        {
            List<PermitLicenseForRenewalDTO> results = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                        join appl in Db.Applications on permitLicense.ApplicationId equals appl.Id
                                                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                        join submittedForPerson in Db.Persons on permitLicense.SubmittedForPersonId equals submittedForPerson.Id into sbPerson
                                                        from submittedForPerson in sbPerson.DefaultIfEmpty()
                                                        join submittedForLegal in Db.Legals on permitLicense.SubmittedForLegalId equals submittedForLegal.Id into sbLegal
                                                        from submittedForLegal in sbLegal.DefaultIfEmpty()
                                                        join captain in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals captain.Id
                                                        join captainPerson in Db.Persons on captain.PersonId equals captainPerson.Id
                                                        where permitLicense.PermitId == permitId
                                                              && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                              && applType.PageCode == pageCode.ToString()
                                                        orderby permitLicense.PermitLicenseValidFrom descending
                                                        select new PermitLicenseForRenewalDTO
                                                        {
                                                            Id = permitLicense.Id,
                                                            RegistrationNumber = permitLicense.RegistrationNum,
                                                            ValidFrom = permitLicense.PermitLicenseValidFrom.Value,
                                                            ValidTo = permitLicense.PermitLicenseValidTo.HasValue
                                                                      ? permitLicense.PermitLicenseValidTo.Value
                                                                      : DefaultConstants.MAX_VALID_DATE,
                                                            HolderNames = submittedForPerson != null
                                                                          ? $"{submittedForPerson.FirstName} {submittedForPerson.LastName}"
                                                                          : $"{submittedForLegal.Name} ({submittedForLegal.Eik})",
                                                            Captain = $"{captainPerson.FirstName} {captainPerson.LastName} ({captain.RegistrationNum})"
                                                        }).ToList();

            List<int> permitLicenseIds = results.Select(x => x.Id).ToList();

            ILookup<int, string> permitLicenseAquaticOrganisms = (from permitLicenseFish in Db.PermitLicenseRegisterFish
                                                                  join auqaticOrganism in Db.Nfishes on permitLicenseFish.FishId equals auqaticOrganism.Id
                                                                  where permitLicenseIds.Contains(permitLicenseFish.PermitLicenseRegisterId)
                                                                  select new
                                                                  {
                                                                      PermitLicenseId = permitLicenseFish.PermitLicenseRegisterId,
                                                                      AquaticOrganism = $"{auqaticOrganism.Name} ({auqaticOrganism.Code})"
                                                                  }).ToLookup(x => x.PermitLicenseId, y => y.AquaticOrganism);

            ILookup<int, string> permitLicenseFishingGears = (from fishingGearRegister in Db.FishingGearRegisters
                                                              join fishingGear in Db.NfishingGears on fishingGearRegister.FishingGearTypeId equals fishingGear.Id
                                                              where fishingGearRegister.PermitLicenseId.HasValue
                                                                    && permitLicenseIds.Contains(fishingGearRegister.PermitLicenseId.Value)
                                                              select new
                                                              {
                                                                  PermitLicenseId = fishingGearRegister.PermitLicenseId.Value,
                                                                  FishingGear = $"{fishingGear.Name} ({fishingGear.Code})"
                                                              }).ToLookup(x => x.PermitLicenseId, y => y.FishingGear);

            foreach (PermitLicenseForRenewalDTO permitLicense in results)
            {
                permitLicense.AuqticOrganisms = string.Join("; ", permitLicenseAquaticOrganisms[permitLicense.Id]);
                permitLicense.FishingGears = string.Join("; ", permitLicenseFishingGears[permitLicense.Id]);
            }

            return results;
        }

        public void UpdatePermitsIsSuspendedFlag()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                DateTime now = DateTime.Now;

                HashSet<int> permitIdsToBeSuspended = (from permit in Db.CommercialFishingPermitRegisters
                                                       join permitSuspension in Db.CommercialFishingPermitSuspensionChangeHistories on permit.Id equals permitSuspension.PermitId
                                                       where permit.RecordType == nameof(RecordTypesEnum.Register)
                                                             && permit.IsActive
                                                             && !permit.IsSuspended
                                                             && permitSuspension.IsActive
                                                             && permitSuspension.SuspensionValidFrom <= now
                                                             && permitSuspension.SuspensionValidTo > now
                                                       select permit.Id).ToHashSet();

                IQueryable<PermitRegister> permitsToBeSuspended = from permit in Db.CommercialFishingPermitRegisters
                                                                  where permitIdsToBeSuspended.Contains(permit.Id)
                                                                  select permit;

                foreach (PermitRegister permitRegister in permitsToBeSuspended)
                {
                    if (!permitRegister.IsSuspended)
                    {
                        List<int> shipValidPermitIds = GetValidShipPermitIds(permitRegister.ShipId);
                        if (shipValidPermitIds.Count == 1)
                        {
                            shipsRegisterService.EditShipRsr(permitRegister.ShipId, false);
                        }

                        permitRegister.IsSuspended = true;
                        Db.SaveChanges();
                    }
                }

                HashSet<int> permitIdsToBeValid = (from permit in Db.CommercialFishingPermitRegisters
                                                   where permit.RecordType == nameof(RecordTypesEnum.Register)
                                                         && permit.IsActive
                                                         && permit.IsSuspended
                                                         && (from permitSusHist in Db.CommercialFishingPermitSuspensionChangeHistories
                                                             where permitSusHist.PermitId == permit.Id
                                                                   && permitSusHist.IsActive
                                                                   && permitSusHist.SuspensionValidFrom <= now
                                                                   && permitSusHist.SuspensionValidTo > now
                                                             select permitSusHist.Id).Count() == 0
                                                   select permit.Id).ToHashSet();

                IQueryable<PermitRegister> permitsToBeValid = from permit in Db.CommercialFishingPermitRegisters
                                                              where permitIdsToBeValid.Contains(permit.Id)
                                                              select permit;

                foreach (PermitRegister permitRegister in permitsToBeValid)
                {
                    if (permitRegister.IsSuspended)
                    {
                        List<int> shipValidPermitIds = GetValidShipPermitIds(permitRegister.ShipId);
                        if (shipValidPermitIds.Count == 0)
                        {
                            permitRegister.ShipId = shipsRegisterService.EditShipRsr(permitRegister.ShipId, true);
                        }
                        
                        permitRegister.IsSuspended = false;
                        Db.SaveChanges();
                    }
                }

                Db.SaveChanges();

                scope.Complete();
            }
        }

        public void UpdatePermitLicensesIsSuspendedFlag()
        {
            DateTime now = DateTime.Now;

            HashSet<int> permitLicensesIdsToBeSuspended = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                           join permitLicenseSuspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories on permitLicense.Id equals permitLicenseSuspension.PermitLicenseId
                                                           where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                 && permitLicense.IsActive
                                                                 && !permitLicense.IsSuspended
                                                                 && permitLicenseSuspension.IsActive
                                                                 && permitLicenseSuspension.SuspensionValidFrom <= now
                                                                 && permitLicenseSuspension.SuspensionValidTo > now
                                                           select permitLicense.Id).ToHashSet();

            IQueryable<PermitLicensesRegister> permitLicensesToBeSuspended = from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                             where permitLicensesIdsToBeSuspended.Contains(permitLicense.Id)
                                                                             select permitLicense;

            foreach (PermitLicensesRegister permitLicenseRegister in permitLicensesToBeSuspended)
            {
                permitLicenseRegister.IsSuspended = true;
            }

            HashSet<int> permitLicenseIdsToBeValid = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                      where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                            && permitLicense.IsActive
                                                            && permitLicense.IsSuspended
                                                            && (from permitLicenseSusHist in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                where permitLicenseSusHist.PermitLicenseId == permitLicense.Id
                                                                      && permitLicenseSusHist.IsActive
                                                                      && permitLicenseSusHist.SuspensionValidFrom <= now
                                                                      && permitLicenseSusHist.SuspensionValidTo > now
                                                                select permitLicenseSusHist.Id).Count() == 0
                                                      select permitLicense.Id).ToHashSet();

            IQueryable<PermitLicensesRegister> permitLicensesToBeValid = from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                         where permitLicenseIdsToBeValid.Contains(permitLicense.Id)
                                                                         select permitLicense;

            foreach (PermitLicensesRegister permitLicenseRegister in permitLicensesToBeValid)
            {
                permitLicenseRegister.IsSuspended = false;
            }

            Db.SaveChanges();
        }

        public List<PaymentTariffDTO> CalculatePermitLicenseAppliedTariffs(PermitLicenseTariffCalculationParameters tariffCalculationParameters)
        {
            List<PaymentTariffDTO> appliedTariffs = new List<PaymentTariffDTO>();

            int applicationTypeId = (from appl in Db.Applications
                                     where appl.Id == tariffCalculationParameters.ApplicationId
                                     select appl.ApplicationTypeId).First();

            List<TariffNomenclatureDTO> possibleTariffs = applicationService.GetApplicationTypeActiveTariffs(applicationTypeId);

            List<TariffNomenclatureDTO> possibleCalculatedTariffs = possibleTariffs.Where(x => x.IsCalculated).Select(x => x).ToList();
            List<TariffNomenclatureDTO> alreadyAppliedTariffs = possibleTariffs.Where(x => !x.IsCalculated).Select(x => x).ToList();

            foreach (TariffNomenclatureDTO tariff in alreadyAppliedTariffs)
            {
                PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, false);
                appliedTariffs.Add(appliedTariff);
            }

            // Тарифи, ако удостоверението е за Далян
            if (tariffCalculationParameters.PageCode.Value == PageCodeEnum.PoundnetCommFishLic)
            {
                if (tariffCalculationParameters.PoundNetId.HasValue)
                {
                    decimal? poundNetPermitLicensePrice = (from poundNet in Db.PoundNetRegisters
                                                           where poundNet.Id == tariffCalculationParameters.PoundNetId.Value
                                                           select poundNet.PermitLicencePrice).First();
                    TariffNomenclatureDTO tariff;

                    if (poundNetPermitLicensePrice.HasValue) // Ако има изрично поставена цена, вземаме направо само нея с тарифа за концесия
                    {
                        tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_267_2_Poundnet_Comm_Fish_Lic_concession)).Single();
                    }
                    else
                    {
                        string poundNetCategoryCode = (from poundNet in Db.PoundNetRegisters
                                                       join categoryType in Db.NpoundNetCategoryTypes on poundNet.CategoryTypeId equals categoryType.Id
                                                       where poundNet.Id == tariffCalculationParameters.PoundNetId.Value
                                                       select categoryType.Code).First();

                        bool isPoundNetCategoryCastSucc = Enum.TryParse<PoundNetCategoryTypesEnum>(poundNetCategoryCode, out PoundNetCategoryTypesEnum poundNetCategory);

                        if (isPoundNetCategoryCastSucc) // Ако кодът на категорията е познат
                        {
                            if (poundNetCategory == PoundNetCategoryTypesEnum.First) // Ако е първа категория, има тарифа
                            {
                                tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Sea_Aqua_Pound_Net_Cat_1)).Single();
                            }
                            else if (poundNetCategory == PoundNetCategoryTypesEnum.Second) // Ако е втора категория, има тарифа
                            {
                                tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Sea_Aqua_Pound_Net_Cat_2)).Single();
                            }
                            else // TODO this case is maaybe not needed - but what tariff if there is no price AND no first/secord category for the pound net ???
                            {
                                tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_267_2_Poundnet_Comm_Fish_Lic)).Single();
                            }
                        }
                        else // TODO this case is maaybe not needed - but what tariff if there is no price AND no first/secord category for the pound net ???
                        {
                            tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_267_2_Poundnet_Comm_Fish_Lic)).Single();
                        }
                    }

                    PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true, poundNetPermitLicensePrice);
                    appliedTariffs.Add(appliedTariff);
                }
            }
            else if (tariffCalculationParameters.PageCode == PageCodeEnum.RightToFishResource)
            {
                if (tariffCalculationParameters.WaterTypeId.HasValue)
                {
                    WaterTypesEnum waterType = (from wt in Db.NwaterTypes
                                                where wt.Id == tariffCalculationParameters.WaterTypeId.Value
                                                select Enum.Parse<WaterTypesEnum>(wt.Code)).First();

                    switch (waterType)
                    {
                        case WaterTypesEnum.BLACK_SEA:
                            {
                                // Тарифи, свързани с кораб и риболовни уреди
                                if (tariffCalculationParameters.ShipId.HasValue)
                                {
                                    decimal shipTonage = (from ship in Db.ShipsRegister
                                                          where ship.Id == tariffCalculationParameters.ShipId
                                                          select ship.GrossTonnage).First();

                                    if (shipTonage <= 10)
                                    {
                                        if (tariffCalculationParameters.FishingGears != null && tariffCalculationParameters.FishingGears.Count > 0)
                                        {
                                            List<int> fishingGearIds = tariffCalculationParameters.FishingGears.Select(x => x.TypeId).ToList();

                                            bool hasPaidNetsPoleAndLines = (from fishingGear in Db.NfishingGears
                                                                            where fishingGearIds.Contains(fishingGear.Id)
                                                                                  && (PAID_POLE_AND_LINES_GEAR_CODES.Contains(fishingGear.Code)
                                                                                       || PAID_NET_FISHING_GEAR_CODES.Contains(fishingGear.Code))
                                                                            select fishingGear.Id).Any();

                                            if (hasPaidNetsPoleAndLines) // a-1805-ShipTill10-Nets
                                            {
                                                TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Ship_Till10_Nets)).Single();
                                                PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                                appliedTariffs.Add(appliedTariff);
                                            }

                                            bool hasPaidLonglines = (from fishingGear in Db.NfishingGears
                                                                     where fishingGearIds.Contains(fishingGear.Id)
                                                                           && PAID_LONGLINES_GEAR_CODES.Contains(fishingGear.Code)
                                                                     select fishingGear.Id).Any();
                                            if (hasPaidLonglines) //a-1805-ShipTill10-Longliners
                                            {
                                                TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Ship_Till10_Longliners)).Single();
                                                PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                                appliedTariffs.Add(appliedTariff);
                                            }

                                            // TODO check for a-1805-ShipTill10-Fishing-Gears
                                        }
                                    }
                                    else if (shipTonage > 10 && shipTonage <= 25) // a-1805-ShipBetween10And25
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Ship_Between_10_And_25)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }
                                    else if (shipTonage > 25 && shipTonage <= 40) // a-1805-ShipBetween25And40
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Ship_Between_25_And_40)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }
                                    else // a-1805-ShipBetweenOver40
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Ship_Between_Over40)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }
                                }

                                // Тарифи, свързани с водните организми

                                if (tariffCalculationParameters.AquaticOrganismTypeIds != null && tariffCalculationParameters.AquaticOrganismTypeIds.Count > 0)
                                {
                                    bool hasWhelk = (from fish in Db.Nfishes
                                                     where tariffCalculationParameters.AquaticOrganismTypeIds.Contains(fish.Id)
                                                           && fish.Code == nameof(FishCodesEnum.RPN)
                                                     select fish.Id).Any();

                                    if (hasWhelk) // a-1805-Rapan
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Rapan)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }

                                    bool hasMusselOrShrimp = (from fish in Db.Nfishes
                                                              where tariffCalculationParameters.AquaticOrganismTypeIds.Contains(fish.Id)
                                                                    && (fish.Code == nameof(FishCodesEnum.MSM) || fish.Code == nameof(FishCodesEnum.CLS)) // TODO скариди, които са с код 79
                                                              select fish.Id).Any();

                                    if (hasMusselOrShrimp) // a-1805-Mussels-Shrimps
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Mussels_Shrimps)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }

                                    bool hasOtherAquaticOrganisms = (from fish in Db.Nfishes
                                                                     where tariffCalculationParameters.AquaticOrganismTypeIds.Contains(fish.Id)
                                                                           && fish.Code != nameof(FishCodesEnum.MSM)
                                                                           && fish.Code != nameof(FishCodesEnum.CLS)
                                                                           && fish.Code != nameof(FishCodesEnum.RPN)
                                                                     select fish.Id).Any();

                                    if (hasOtherAquaticOrganisms) // a-1805-Other-Species-Fishing
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Other_Species_Fishing)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }
                                }
                            }
                            break;
                        case WaterTypesEnum.DANUBE:
                            {
                                // Тарифи за риболовни уреди
                                if (tariffCalculationParameters.FishingGears != null && tariffCalculationParameters.FishingGears.Count > 0)
                                {
                                    List<int> fishingGearIds = tariffCalculationParameters.FishingGears.Select(x => x.TypeId).ToList();
                                    bool hasPaidNetGears = (from fishingGear in Db.NfishingGears
                                                            where fishingGearIds.Contains(fishingGear.Id)
                                                                  && PAID_DANUBE_NET_GEAR_CODES.Contains(fishingGear.Code)
                                                            select fishingGear.Id).Any();

                                    bool hasPotsGears = (from fishingGear in Db.NfishingGears
                                                         where fishingGearIds.Contains(fishingGear.Id)
                                                               && fishingGear.Code == nameof(FishingGearTypesEnum.FPO)
                                                         select fishingGear.Id).Any();

                                    if (hasPotsGears && hasPaidNetGears) // a-1805-Dunav-Ship-NetsAndFishingTraps
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Dunav_Ship_Nets_And_Fishing_Traps)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }
                                    else if (hasPotsGears) // a-1805-Dunav-Ship-FishingTraps
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Dunav_Ship_Fishing_Traps)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }
                                    else if (hasPaidNetGears) //a-1805-Dunav-Ship-Nets
                                    {
                                        TariffNomenclatureDTO tariff = possibleCalculatedTariffs.Where(x => x.Code == nameof(TariffCodesEnum.a_1805_Dunav_Ship_Nets)).Single();
                                        PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, 1, true);
                                        appliedTariffs.Add(appliedTariff);
                                    }
                                }

                                // Тарифи за водни организми
                                if (tariffCalculationParameters.AquaticOrganismTypeIds != null && tariffCalculationParameters.AquaticOrganismTypeIds.Count > 0)
                                {
                                    // TODO ще има ли за есетровите риби тарифа ???
                                }
                            }
                            break;
                    }

                    // TODO тарифи за метри на мрежените уреди + за марки на уреди (TODO тези ДВЕ тарифи за р. Дунав И Черно море ли са?)
                }

                // Тарифи за маркиране на уреди: мрежените на 100м, а оснаталите на бройка
                if (tariffCalculationParameters.FishingGears != null && tariffCalculationParameters.FishingGears.Count > 0)
                {
                    appliedTariffs.AddRange(GetFishingGearMarksTariffs(tariffCalculationParameters.FishingGears, possibleCalculatedTariffs));
                }
            }
            else if (tariffCalculationParameters.PageCode == PageCodeEnum.CatchQuataSpecies)
            {
                // Тарифи за маркиране на уреди: мрежените на 100м, а оснаталите на бройка
                if (tariffCalculationParameters.FishingGears != null && tariffCalculationParameters.FishingGears.Count > 0)
                {
                    appliedTariffs.AddRange(GetFishingGearMarksTariffs(tariffCalculationParameters.FishingGears, possibleCalculatedTariffs));
                }
            }

            return appliedTariffs;
        }

        public async Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            PageCodeEnum[] permitPageCodes = new PageCodeEnum[]
                {
                    PageCodeEnum.PoundnetCommFish,
                    PageCodeEnum.DupPoundnetCommFish,
                    PageCodeEnum.CommFish,
                    PageCodeEnum.DupCommFish,
                    PageCodeEnum.RightToFishThirdCountry,
                    PageCodeEnum.DupRightToFishThirdCountry
                };

            PageCodeEnum[] permitLicensePageCodes = new PageCodeEnum[]
                {
                    PageCodeEnum.PoundnetCommFishLic,
                    PageCodeEnum.DupPoundnetCommFishLic,
                    PageCodeEnum.CatchQuataSpecies,
                    PageCodeEnum.DupCatchQuataSpecies,
                    PageCodeEnum.RightToFishResource,
                    PageCodeEnum.DupRightToFishResource
                };

            ApplicationEDeliveryInfo info = null;

            if (permitPageCodes.Contains(pageCode))
            {
                var data = (from permit in Db.CommercialFishingPermitRegisters
                            join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                            join appl in Db.Applications on permit.ApplicationId equals appl.Id
                            join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                            join submittedForLegal in Db.Legals on permit.SubmittedForLegalId equals submittedForLegal.Id into leg
                            from submittedForLegal in leg.DefaultIfEmpty()
                            join submittedForPerson in Db.Persons on permit.SubmittedForPersonId equals submittedForPerson.Id into per
                            from submittedForPerson in per.DefaultIfEmpty()
                            join user in Db.Users on permit.CreatedBy equals user.Username
                            join person in Db.Persons on user.PersonId equals person.Id
                            where permit.ApplicationId == applicationId
                                && permit.RecordType == nameof(RecordTypesEnum.Register)
                            select new
                            {
                                permit.Id,
                                ApplicationType = applType.Name,
                                ApplicationTypeCode = applType.Code,
                                SubmittedForLegalId = submittedForLegal != null ? (int?)submittedForLegal.Id : null,
                                SubmittedForPersonId = submittedForPerson != null ? (int?)submittedForPerson.Id : null,
                                CreatedByPersonEGN = person.EgnLnc,
                                PermitType = Enum.Parse<CommercialFishingTypesEnum>(permitType.Code)
                            }).First();

                RegixPersonDataDTO subForPerson = null;
                RegixLegalDataDTO subForLegal = null;

                if (data.SubmittedForPersonId.HasValue)
                {
                    subForPerson = personService.GetRegixPersonData(data.SubmittedForPersonId.Value);
                }
                else
                {
                    subForLegal = legalService.GetRegixLegalData(data.SubmittedForLegalId.Value);
                }

                bool isDuplicate = pageCode == PageCodeEnum.DupPoundnetCommFish
                                   || pageCode == PageCodeEnum.DupCommFish
                                   || pageCode == PageCodeEnum.DupRightToFishThirdCountry;

                DownloadableFileDTO pdf = await GetPermitRegisterFileForDownload(data.Id, data.PermitType, isDuplicate);

                info = new ApplicationEDeliveryInfo
                {
                    Subject = data.ApplicationType,
                    DocBytes = pdf.Bytes,
                    DocNameWithExtension = pdf.FileName,
                    DocRegNumber = applicationId.ToString(),
                    ReceiverType = eProfileType.LegalPerson,
                    ReceiverUniqueIdentifier = subForPerson.EgnLnc.EgnLnc,
                    ReceiverPhone = subForPerson.Phone,
                    ReceiverEmail = subForPerson.Email,
                    ServiceOID = data.ApplicationTypeCode,
                    OperatorEGN = data.CreatedByPersonEGN
                };
            }
            else if (permitLicensePageCodes.Contains(pageCode))
            {
                var data = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                            join permitLicenseType in Db.NcommercialFishingPermitLicenseTypes on permitLicense.PermitLicenseTypeId equals permitLicenseType.Id
                            join appl in Db.Applications on permitLicense.ApplicationId equals appl.Id
                            join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                            join submittedForLegal in Db.Legals on permitLicense.SubmittedForLegalId equals submittedForLegal.Id into leg
                            from submittedForLegal in leg.DefaultIfEmpty()
                            join submittedForPerson in Db.Persons on permitLicense.SubmittedForPersonId equals submittedForPerson.Id into per
                            from submittedForPerson in per.DefaultIfEmpty()
                            join user in Db.Users on permitLicense.CreatedBy equals user.Username
                            join person in Db.Persons on user.PersonId equals person.Id
                            where permitLicense.ApplicationId == applicationId
                                && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                            select new
                            {
                                permitLicense.Id,
                                ApplicationType = applType.Name,
                                ApplicationTypeCode = applType.Code,
                                SubmittedForLegalId = submittedForLegal != null ? (int?)submittedForLegal.Id : null,
                                SubmittedForPersonId = submittedForPerson != null ? (int?)submittedForPerson.Id : null,
                                CreatedByPersonEGN = person.EgnLnc,
                                PermitLicenseType = Enum.Parse<CommercialFishingTypesEnum>(permitLicenseType.Code)
                            }).First();

                RegixPersonDataDTO subForPerson = null;
                RegixLegalDataDTO subForLegal = null;

                if (data.SubmittedForPersonId.HasValue)
                {
                    subForPerson = personService.GetRegixPersonData(data.SubmittedForPersonId.Value);
                }
                else
                {
                    subForLegal = legalService.GetRegixLegalData(data.SubmittedForLegalId.Value);
                }

                bool isDuplicate = pageCode == PageCodeEnum.DupPoundnetCommFish
                                   || pageCode == PageCodeEnum.DupCommFish
                                   || pageCode == PageCodeEnum.DupRightToFishThirdCountry;

                DownloadableFileDTO pdf = await GetPermitLicenseRegisterFileForDownload(data.Id, data.PermitLicenseType, isDuplicate);

                info = new ApplicationEDeliveryInfo
                {
                    Subject = data.ApplicationType,
                    DocBytes = pdf.Bytes,
                    DocNameWithExtension = pdf.FileName,
                    DocRegNumber = applicationId.ToString(),
                    ReceiverType = eProfileType.LegalPerson,
                    ReceiverUniqueIdentifier = subForPerson.EgnLnc.EgnLnc,
                    ReceiverPhone = subForPerson.Phone,
                    ReceiverEmail = subForPerson.Email,
                    ServiceOID = data.ApplicationTypeCode,
                    OperatorEGN = data.CreatedByPersonEGN
                };
            }
            else
            {
                throw new ArgumentException("Nothing to deliver for page code: " + pageCode.ToString());
            }

            return info;
        }

        private List<PaymentTariffDTO> GetFishingGearMarksTariffs(List<FishingGearDTO> fishingGears, List<TariffNomenclatureDTO> possibleTariffs)
        {
            List<PaymentTariffDTO> appliedTariffs = new List<PaymentTariffDTO>();
            List<FishingGearDTO> markedFishngGears = fishingGears.Where(x => x.Marks != null
                                                                             && x.Marks.Count > 0
                                                                             && x.Marks
                                                                                 .Any(x => x.SelectedStatus == FishingGearMarkStatusesEnum.NEW)
                                                                       ).ToList();

            if (markedFishngGears.Count > 0)
            {
                TariffNomenclatureDTO tariff = possibleTariffs.Where(x => x.Code == nameof(TariffCodesEnum.Mark_Gear_100m)).Single();
                int marksToBePaidCount = 0;

                List<FishingGearMarkDTO> newMarks = markedFishngGears.SelectMany(x => x.Marks).Where(x => x.SelectedStatus == FishingGearMarkStatusesEnum.NEW).ToList();

                // Намиране на всички марки, които са били вече платени като част от предишно удостоверение - те няма нужда да се плащат повторно
                List<string> markNumbers = newMarks.Select(x => x.Number).ToList();
                List<string> existingMarksNumbers = (from mark in Db.FishingGearMarks
                                                     join markStatus in Db.NfishingGearMarkStatuses on mark.MarkStatusId equals markStatus.Id
                                                     where markNumbers.Contains(mark.MarkNum)
                                                           && markStatus.Code == nameof(FishingGearMarkStatusesEnum.REGISTERED)
                                                     select mark.MarkNum).ToList();
                marksToBePaidCount = markNumbers.Except(existingMarksNumbers).Count();

                if (marksToBePaidCount > 0)
                {
                    PaymentTariffDTO appliedTariff = CreatePaymentTariff(tariff, marksToBePaidCount, true);
                    appliedTariffs.Add(appliedTariff);
                }
            }

            return appliedTariffs;
        }

        /// <summary>
        /// Creates PaymentTariffDTO from parameters
        /// </summary>
        /// <param name="tariff">Basic information for the tariff</param>
        /// <param name="quantity">How many times is the tariff applied</param>
        /// <param name="isCalculated">Is the tariff set as dynamicly calculated in the DB</param>
        /// <param name="customPrice">Pass this price if there is custom logic and the tariff is with different price depending on something else</param>
        /// <returns>Returns new PaymentTariffDTO</returns>
        private PaymentTariffDTO CreatePaymentTariff(TariffNomenclatureDTO tariff, decimal quantity, bool isCalculated = false, decimal? customPrice = null)
        {
            PaymentTariffDTO appliedTariff = new PaymentTariffDTO
            {
                Quantity = quantity,
                TariffBasedOnPlea = tariff.BasedOnPlea,
                TariffName = tariff.DisplayName,
                TariffDescription = tariff.Description,
                TariffId = tariff.Value,
                UnitPrice = customPrice.HasValue ? customPrice.Value : tariff.Price,
                IsCalculated = isCalculated
            };

            appliedTariff.Price = appliedTariff.Quantity * appliedTariff.UnitPrice;

            return appliedTariff;
        }

        private List<CommercialFishingLogBookEditDTO> GetUnfinishedLogBooksFromOldLicenses(int permitId)
        {
            DateTime now = DateTime.Now;

            int? permitRegisterId = GetRegisterPermitId(permitId);

            if (permitRegisterId.HasValue) // Все още няма регистров запис за разрешително, а би трябвало да има
            {

                List<CommercialFishingLogBookEditDTO> results = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                 join logBookPermitLicense in Db.LogBookPermitLicenses on permitLicense.Id equals logBookPermitLicense.PermitLicenseRegisterId
                                                                 join logBook in Db.LogBooks on logBookPermitLicense.LogBookId equals logBook.Id
                                                                 join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                                                 where permitLicense.PermitId == permitRegisterId.Value
                                                                       && logBook.FinishDate == null
                                                                       && logBook.IsActive
                                                                       && logBookStatus.Code != nameof(LogBookStatusesEnum.Finished)
                                                                       && logBookPermitLicense.LogBookValidFrom <= now
                                                                       && logBookPermitLicense.LogBookValidTo > now
                                                                 select new CommercialFishingLogBookEditDTO
                                                                 {
                                                                     LastLogBookLicenseId = logBookPermitLicense.Id,
                                                                     LastPermitLicenseId = logBookPermitLicense.PermitLicenseRegisterId,
                                                                     LogBookId = logBook.Id,
                                                                     LogbookNumber = logBook.LogNum,
                                                                     LogBookTypeId = logBook.LogBookTypeId,
                                                                     IsOnline = logBook.IsOnline,
                                                                     IssueDate = logBook.IssueDate,
                                                                     FinishDate = logBook.FinishDate,
                                                                     StartPageNumber = logBook.StartPageNum,
                                                                     EndPageNumber = logBook.EndPageNum,
                                                                     Comment = logBook.Comments,
                                                                     Price = logBook.Price,
                                                                     PermitLicenseIsActive = logBookPermitLicense.IsActive,
                                                                     LogBookIsActive = logBook.IsActive,
                                                                     IsActive = logBookPermitLicense.IsActive,
                                                                     IsForRenewal = true,
                                                                     OwnerType = logBook.LogBookOwnerType != null
                                                                                 ? Enum.Parse<LogBookPagePersonTypesEnum>(logBook.LogBookOwnerType)
                                                                                 : null,
                                                                     LastPageNumber = logBook.LastPageNum
                                                                 }).ToList();

                foreach (CommercialFishingLogBookEditDTO result in results)
                {
                    LogBookTypesEnum logBookType = (from lbType in Db.NlogBookTypes
                                                    where lbType.Id == result.LogBookTypeId
                                                    select Enum.Parse<LogBookTypesEnum>(lbType.Code)).First();

                    switch (logBookType)
                    {
                        case LogBookTypesEnum.Ship:
                            result.ShipPagesAndDeclarations = logBooksService.GetShipLogBookPagesAndDeclarations(result.LogBookId.Value,
                                                                                                                 result.LastPermitLicenseId);
                            break;
                        case LogBookTypesEnum.Admission:
                            result.AdmissionPagesAndDeclarations = logBooksService.GetAdmissionLogBookPagesAndDeclarations(result.LogBookId.Value,
                                                                                                                           result.LastPermitLicenseId);
                            break;
                        case LogBookTypesEnum.Transportation:
                            result.TransportationPagesAndDeclarations = logBooksService.GetTransportationLogBookPagesAndDeclarations(result.LogBookId.Value,
                                                                                                                                     result.LastPermitLicenseId);
                            break;
                    }
                }

                return results;
            }
            else
            {
                return new List<CommercialFishingLogBookEditDTO>();
            }
        }

        private void PermitDataChanged(ScopedServiceProviderFactory serviceProviderFactory, string registrationNumber)
        {
            using (IScopedServiceProvider serviceProvider = serviceProviderFactory.GetServiceProvider())
            {
                IPermitsAndLicencesService permitsAndLicencesService = serviceProvider.GetRequiredService<IPermitsAndLicencesService>();
                IFVMSReceiverIntegrationService fvmsIntegrationService = serviceProvider.GetRequiredService<IFVMSReceiverIntegrationService>();
                FVMSModels.ExternalModels.Permit fvmsPermit = permitsAndLicencesService.GetPermit(registrationNumber);
                fvmsIntegrationService.EnqueuePermitChange(fvmsPermit);
            }
        }

        private void PermitDataChanged(ScopedServiceProviderFactory serviceProviderFactory, int permitId)
        {
            using (IScopedServiceProvider serviceProvider = serviceProviderFactory.GetServiceProvider())
            {
                IPermitsAndLicencesService permitsAndLicencesService = serviceProvider.GetRequiredService<IPermitsAndLicencesService>();
                IFVMSReceiverIntegrationService fvmsIntegrationService = serviceProvider.GetRequiredService<IFVMSReceiverIntegrationService>();
                IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();

                string registrationNumber = db.CommercialFishingPermitRegisters
                                              .Where(x => x.Id == permitId)
                                              .Select(x => x.RegistrationNum)
                                              .First();

                FVMSModels.ExternalModels.Permit fvmsPermit = permitsAndLicencesService.GetPermit(registrationNumber);
                fvmsIntegrationService.EnqueuePermitChange(fvmsPermit);
            }
        }

        private bool HasShipValidPermit(List<int> shipIds, WaterTypesEnum selectedWaterType)
        {
            DateTime now = DateTime.Now;

            return (from permit in Db.CommercialFishingPermitRegisters
                    join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                    join appl in Db.Applications on permit.ApplicationId equals appl.Id
                    join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                    join waterType in Db.NwaterTypes on permit.WaterTypeId equals waterType.Id
                    where shipIds.Contains(permit.ShipId)
                          && waterType.Code == selectedWaterType.ToString()
                          && permit.IsActive
                          && !permit.IsSuspended
                          && permit.RecordType == nameof(RecordTypesEnum.Register)
                             && permit.PermitValidFrom <= now
                             && (permit.PermitValidTo > now
                                 || permit.IsPermitUnlimited.Value)
                          && (permitType.Code == nameof(CommercialFishingTypesEnum.Permit) || permitType.Code == nameof(CommercialFishingTypesEnum.ThirdCountryPermit))
                          && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                          && appl.IsActive
                    select permit.Id).Any();
        }

        private bool IsShipThirdCountry(int shipUId)
        {
            return (from ship in Db.ShipsRegister
                    where ship.ShipUid == shipUId
                    select ship.IsThirdPartyShip).First();
        }

        private bool HasShipValidPoundNetPermit(List<int> shipIds)
        {
            DateTime now = DateTime.Now;

            return (from permit in Db.CommercialFishingPermitRegisters
                    join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                    join appl in Db.Applications on permit.ApplicationId equals appl.Id
                    join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                    where shipIds.Contains(permit.ShipId)
                          && permit.RecordType == nameof(RecordTypesEnum.Register)
                             && permit.PermitValidFrom <= now
                             && (permit.PermitValidTo > now
                                 || permit.IsPermitUnlimited.Value)
                          && permitType.Code == nameof(CommercialFishingTypesEnum.PoundNetPermit)
                          && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                          && permit.IsActive
                          && appl.IsActive
                          && !permit.IsSuspended
                    select permit.Id).Any();
        }

        private bool HasShipPermitApplication(List<int> shipIds, WaterTypesEnum selectedWaterType)
        {
            bool hasPermitApplication = (from permit in Db.CommercialFishingPermitRegisters
                                         join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                         join appl in Db.Applications on permit.ApplicationId equals appl.Id
                                         join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                                         join waterType in Db.NwaterTypes on permit.WaterTypeId equals waterType.Id
                                         where shipIds.Contains(permit.ShipId)
                                               && waterType.Code == selectedWaterType.ToString()
                                               && permit.RecordType == nameof(RecordTypesEnum.Application)
                                               && permitType.Code == nameof(CommercialFishingTypesEnum.Permit)
                                               && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                                               && permit.IsActive
                                               && appl.IsActive
                                               && !permit.IsSuspended
                                         select permit.Id).Any();

            return hasPermitApplication;
        }

        private bool HasShipPoundNetPermitApplication(List<int> shipIds)
        {
            return (from permit in Db.CommercialFishingPermitRegisters
                    join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                    join appl in Db.Applications on permit.ApplicationId equals appl.Id
                    join applStatus in Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                    where shipIds.Contains(permit.ShipId)
                          && permit.RecordType == nameof(RecordTypesEnum.Application)
                          && permitType.Code == nameof(CommercialFishingTypesEnum.PoundNetPermit)
                          && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                          && permit.IsActive
                          && appl.IsActive
                          && !permit.IsSuspended
                    select permit.Id).Any();
        }

        private bool SubmittedForIsShipOwnerCheck(ApplicationSubmittedForRegixDataDTO submittedFor, ApplicationSubmittedByRegixDataDTO submittedBy, int shipUId)
        {
            var shipOwners = (from ship in Db.ShipsRegister
                              join shipOwner in Db.ShipOwners on ship.Id equals shipOwner.ShipRegisterId
                              join shipOwnerPerson in Db.Persons on shipOwner.OwnerPersonId equals shipOwnerPerson.Id into shipOPeron
                              from shipOwnerPerson in shipOPeron.DefaultIfEmpty()
                              join shipOwnerLegal in Db.Legals on shipOwner.OwnerLegalId equals shipOwnerLegal.Id into shipOLegal
                              from shipOwnerLegal in shipOLegal.DefaultIfEmpty()
                              where ship.ShipUid == shipUId && shipOwner.IsShipHolder && shipOwner.IsActive
                              select new
                              {
                                  OwnerIsPerson = shipOwner.OwnerIsPerson,
                                  Person = shipOwnerPerson != null
                                           ? new
                                           {
                                               EngLnchData = new EgnLncDTO
                                               {
                                                   EgnLnc = shipOwnerPerson.EgnLnc,
                                                   IdentifierType = Enum.Parse<IdentifierTypeEnum>(shipOwnerPerson.IdentifierType)
                                               },
                                               FirstName = shipOwnerPerson.FirstName,
                                               MiddleName = shipOwnerPerson.MiddleName,
                                               LastName = shipOwnerPerson.LastName
                                           }
                                           : null,
                                  Legal = shipOwnerLegal != null
                                          ? new
                                          {
                                              EIK = shipOwnerLegal.Eik,
                                              Name = shipOwnerLegal.Name
                                          }
                                          : null
                              }).ToList();

            bool isSubmittedForShipOwner = false;

            RegixPersonDataDTO personRegixData = null;

            if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.Personal)
            {
                personRegixData = submittedBy.Person;
            }
            else if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.PersonalRepresentative)
            {
                personRegixData = submittedFor.Person;
            }

            if (personRegixData != null)
            {
                isSubmittedForShipOwner = (from shipOwner in shipOwners
                                           where shipOwner.OwnerIsPerson
                                                 && shipOwner.Person.EngLnchData.EgnLnc == personRegixData.EgnLnc.EgnLnc
                                                 && shipOwner.Person.EngLnchData.IdentifierType == personRegixData.EgnLnc.IdentifierType
                                                 && shipOwner.Person.FirstName == personRegixData.FirstName
                                                 && shipOwner.Person.MiddleName == personRegixData.MiddleName
                                                 && shipOwner.Person.LastName == personRegixData.LastName
                                           select shipOwner).Any();

            }
            else // Предполагаме, че щом няма person, то трябва да има юридическо лице (legal) за получател на заявлнието
            {
                RegixLegalDataDTO submittedForLegalRegixData = submittedFor.Legal;
                isSubmittedForShipOwner = (from shipOwner in shipOwners
                                           where !shipOwner.OwnerIsPerson
                                                 && shipOwner.Legal.EIK == submittedForLegalRegixData.EIK
                                                 && shipOwner.Legal.Name == submittedForLegalRegixData.Name
                                           select shipOwner).Any();
            }

            return isSubmittedForShipOwner;
        }

        private bool CaptainIsQualifiedFisherCheck(QualifiedFisherBasicDataDTO qualifiedFisher)
        {
            bool isCaptainQualifiedFisher = (from fisher in Db.FishermenRegisters
                                             join person in Db.Persons on fisher.PersonId equals person.Id
                                             where person.EgnLnc == qualifiedFisher.Identifier.EgnLnc
                                                   && person.IdentifierType == qualifiedFisher.Identifier.IdentifierType.ToString()
                                                   && person.FirstName == qualifiedFisher.FirstName
                                                   && person.LastName == qualifiedFisher.LastName
                                                   && fisher.IsActive
                                                   && fisher.RecordType == nameof(RecordTypesEnum.Register)
                                                   && (fisher.IsWithMaritimeEducation || (fisher.HasPassedExam.HasValue && fisher.HasPassedExam.Value))
                                             select fisher.Id).Any();

            return isCaptainQualifiedFisher;
        }

        private int? GetRegisterPermitId(int permitId)
        {
            var applicationPermitData = (from permit in Db.CommercialFishingPermitRegisters
                                         where permit.Id == permitId
                                         select new
                                         {
                                             ApplicationId = permit.ApplicationId,
                                             RecordType = Enum.Parse<RecordTypesEnum>(permit.RecordType)
                                         }).First();

            int? registerPermitId;

            if (applicationPermitData.RecordType == RecordTypesEnum.Register)
            {
                registerPermitId = permitId;
            }
            else
            {
                registerPermitId = (from permit in Db.CommercialFishingPermitRegisters
                                    where permit.ApplicationId == applicationPermitData.ApplicationId
                                          && permit.RecordType == nameof(RecordTypesEnum.Register)
                                    select permit.Id).SingleOrDefault();
            }

            return registerPermitId;
        }

        private List<SuspensionDataDTO> GetPermitSuspensions(int permitId)
        {
            List<SuspensionDataDTO> results = (from permitSuspension in Db.CommercialFishingPermitSuspensionChangeHistories
                                               join reason in Db.NsuspensionReasons on permitSuspension.ReasonId equals reason.Id
                                               join suspensionType in Db.NsuspensionTypes on reason.SuspensionTypeId equals suspensionType.Id
                                               where permitSuspension.PermitId == permitId
                                               select new SuspensionDataDTO
                                               {
                                                   Id = permitSuspension.Id,
                                                   SuspensionTypeId = reason.SuspensionTypeId,
                                                   SuspensionTypeName = suspensionType.Name,
                                                   ReasonId = reason.Id,
                                                   ReasonName = reason.Name,
                                                   ValidFrom = permitSuspension.SuspensionValidFrom,
                                                   ValidTo = permitSuspension.SuspensionValidTo,
                                                   EnactmentDate = permitSuspension.EnactmentDate,
                                                   OrderNumber = permitSuspension.OrderNumber,
                                                   IsActive = permitSuspension.IsActive
                                               }).ToList();

            return results;
        }

        private List<SuspensionDataDTO> GetPermitLicenseSuspensions(int permitLicenseId)
        {
            List<SuspensionDataDTO> results = (from permitSuspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                               join reason in Db.NsuspensionReasons on permitSuspension.ReasonId equals reason.Id
                                               join suspensionType in Db.NsuspensionTypes on reason.SuspensionTypeId equals suspensionType.Id
                                               where permitSuspension.PermitLicenseId == permitLicenseId
                                               select new SuspensionDataDTO
                                               {
                                                   Id = permitSuspension.Id,
                                                   SuspensionTypeId = reason.SuspensionTypeId,
                                                   SuspensionTypeName = suspensionType.Name,
                                                   ReasonId = reason.Id,
                                                   ReasonName = reason.Name,
                                                   ValidFrom = permitSuspension.SuspensionValidFrom,
                                                   ValidTo = permitSuspension.SuspensionValidTo,
                                                   EnactmentDate = permitSuspension.EnactmentDate,
                                                   OrderNumber = permitSuspension.OrderNumber,
                                                   IsActive = permitSuspension.IsActive
                                               }).ToList();

            return results;
        }

        private void AddOrEditPermitSuspensions(PermitRegister dbPermit, List<SuspensionDataDTO> suspensions, int currentUserId)
        {
            DateTime now = DateTime.Now;
            bool isPermitSuspended = false;

            foreach (SuspensionDataDTO suspension in suspensions)
            {
                suspension.ValidFrom = suspension.ValidFrom.HasValue ? suspension.ValidFrom.Value : suspension.EnactmentDate.Value;
                suspension.ValidTo = suspension.ValidTo.HasValue ? suspension.ValidTo.Value : DefaultConstants.MAX_VALID_DATE;

                if (suspension.Id == null) // New suspension to add
                {
                    PermitSuspensionChangeHistory entry = new PermitSuspensionChangeHistory
                    {
                        Permit = dbPermit,
                        SuspensionValidFrom = suspension.ValidFrom.Value,
                        SuspensionValidTo = suspension.ValidTo.Value,
                        EnactmentDate = suspension.EnactmentDate.Value,
                        OrderNumber = suspension.OrderNumber,
                        ReasonId = suspension.ReasonId.Value,
                        ModifiedByUserId = currentUserId
                    };

                    Db.CommercialFishingPermitSuspensionChangeHistories.Add(entry);
                }
                else
                {
                    PermitSuspensionChangeHistory dbPermitSuspension = (from permitSuspension in Db.CommercialFishingPermitSuspensionChangeHistories
                                                                        where permitSuspension.Id == suspension.Id
                                                                        select permitSuspension).First();

                    dbPermitSuspension.SuspensionValidFrom = suspension.ValidFrom.Value;
                    dbPermitSuspension.SuspensionValidTo = suspension.ValidTo.Value;
                    dbPermitSuspension.EnactmentDate = suspension.EnactmentDate.Value;
                    dbPermitSuspension.OrderNumber = suspension.OrderNumber;
                    dbPermitSuspension.ReasonId = suspension.ReasonId.Value;
                    dbPermitSuspension.IsActive = suspension.IsActive;
                    dbPermitSuspension.ModifiedByUserId = currentUserId;
                }
            }

            if (suspensions.Any(x => x.ValidFrom <= now && x.ValidTo > now && x.IsActive))
            {
                isPermitSuspended = true;
            }
            else
            {
                isPermitSuspended = false;
            }

            if (dbPermit.IsSuspended != isPermitSuspended) // there is a change in the isSuspended flag, so maybe a ship MOD is required
            {
                List<int> shipValidPermitIds = GetValidShipPermitIds(dbPermit.ShipId);
                if (isPermitSuspended && shipValidPermitIds.Count == 1) // this was the only valid permit for the ship
                {
                    shipsRegisterService.EditShipRsr(dbPermit.ShipId, false);
                }
                else if (!isPermitSuspended && shipValidPermitIds.Count == 0) // there was no valid permit, this is the newly valid one
                {
                    dbPermit.ShipId = shipsRegisterService.EditShipRsr(dbPermit.ShipId, true);
                }
                
                dbPermit.IsSuspended = isPermitSuspended;
            }
        }

        private void AddOrEditPermitLicenseSuspensions(PermitLicensesRegister dbPermitLicense, List<SuspensionDataDTO> suspensions, int currentUserId)
        {
            DateTime now = DateTime.Now;
            bool isPermitLicenseSuspended = false;

            foreach (SuspensionDataDTO suspension in suspensions)
            {
                suspension.ValidFrom = suspension.ValidFrom.HasValue ? suspension.ValidFrom.Value : suspension.EnactmentDate.Value;
                suspension.ValidTo = suspension.ValidTo.HasValue ? suspension.ValidTo.Value : DefaultConstants.MAX_VALID_DATE;

                if (suspension.Id == null)
                {
                    PermitLicenseSuspensionChangeHistory entry = new PermitLicenseSuspensionChangeHistory
                    {
                        PermitLicense = dbPermitLicense,
                        SuspensionValidFrom = suspension.ValidFrom.Value,
                        SuspensionValidTo = suspension.ValidTo.Value,
                        EnactmentDate = suspension.EnactmentDate.Value,
                        OrderNumber = suspension.OrderNumber,
                        ReasonId = suspension.ReasonId.Value,
                        ModifiedByUserId = currentUserId
                    };

                    Db.CommercialFishingPermitLicenseSuspensionChangeHistories.Add(entry);
                }
                else
                {
                    PermitLicenseSuspensionChangeHistory dbPermitSuspension = (from permitSuspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                               where permitSuspension.Id == suspension.Id
                                                                               select permitSuspension).First();

                    dbPermitSuspension.SuspensionValidFrom = suspension.ValidFrom.Value;
                    dbPermitSuspension.SuspensionValidTo = suspension.ValidTo.Value;
                    dbPermitSuspension.EnactmentDate = suspension.EnactmentDate.Value;
                    dbPermitSuspension.OrderNumber = suspension.OrderNumber;
                    dbPermitSuspension.ReasonId = suspension.ReasonId.Value;
                    dbPermitSuspension.IsActive = suspension.IsActive;
                    dbPermitSuspension.ModifiedByUserId = currentUserId;
                }
            }

            if (suspensions.Any(x => x.ValidFrom <= now && x.ValidTo > now && x.IsActive))
            {
                isPermitLicenseSuspended = true;
            }
            else
            {
                isPermitLicenseSuspended = false;
            }

            dbPermitLicense.IsSuspended = isPermitLicenseSuspended;
        }

        private void AddOrUpdatePermitLicenseAquaticOrganismTypes(PermitLicensesRegister dbPermitLicense, List<int> aquaticOrganismTypeIds)
        {
            List<int> dbPermitAquaticOrganismTypesIds = (from aquaticOrganism in Db.PermitLicenseRegisterFish
                                                         where aquaticOrganism.PermitLicenseRegisterId == dbPermitLicense.Id
                                                               && aquaticOrganism.IsActive
                                                         select aquaticOrganism.FishId).ToList();

            IEnumerable<int> aquaticOrganismsToDelete = dbPermitAquaticOrganismTypesIds.Except(aquaticOrganismTypeIds);
            IEnumerable<int> aquaticOrganismsToAdd = aquaticOrganismTypeIds.Except(dbPermitAquaticOrganismTypesIds);

            foreach (int aquaticOrganismTypeId in aquaticOrganismsToDelete)
            {
                PermitLicenseRegisterFish permitLicenseAquaticOrganism = Db.PermitLicenseRegisterFish
                                                                            .Single(x => x.FishId == aquaticOrganismTypeId
                                                                                         && x.PermitLicenseRegisterId == dbPermitLicense.Id
                                                                                         && x.IsActive);
                permitLicenseAquaticOrganism.IsActive = false;
            }

            foreach (int aquaticOrganismTypeId in aquaticOrganismsToAdd)
            {
                PermitLicenseRegisterFish permitLicenseAquaticOrganism = Db.PermitLicenseRegisterFish
                                                                           .SingleOrDefault(x => x.FishId == aquaticOrganismTypeId
                                                                                        && x.PermitLicenseRegisterId == dbPermitLicense.Id
                                                                                        && !x.IsActive);
                if (permitLicenseAquaticOrganism == null)
                {
                    PermitLicenseRegisterFish permitLicenseRegisterAquaticOrganism = new PermitLicenseRegisterFish
                    {
                        FishId = aquaticOrganismTypeId,
                        PermitLicenseRegister = dbPermitLicense
                    };
                    Db.PermitLicenseRegisterFish.Add(permitLicenseRegisterAquaticOrganism);
                }
                else
                {
                    permitLicenseAquaticOrganism.IsActive = true;
                }
            }
        }

        private void AddOrUpdatePermitLicenseAquaticOrganismTypes(PermitLicensesRegister dbPermitLicense, List<QuotaAquaticOrganismDTO> aquaticOrganismTypes)
        {
            List<QuotaAquaticOrganismDTO> dbPermitAquaticOrganismTypes = (from aquaticOrganism in Db.PermitLicenseRegisterFish
                                                                          where aquaticOrganism.PermitLicenseRegisterId == dbPermitLicense.Id
                                                                                && aquaticOrganism.IsActive
                                                                          select new QuotaAquaticOrganismDTO
                                                                          {
                                                                              AquaticOrganismId = aquaticOrganism.FishId,
                                                                              PortId = aquaticOrganism.PortOfUnloadingId.Value
                                                                          }).ToList();

            IEnumerable<int> aquaticOrganismsToDeleteIds = dbPermitAquaticOrganismTypes.Select(x => x.AquaticOrganismId).Except(aquaticOrganismTypes.Select(x => x.AquaticOrganismId));

            foreach (int aquaticOrganismTypeId in aquaticOrganismsToDeleteIds)
            {
                PermitLicenseRegisterFish permitLicenseAquaticOrganism = Db.PermitLicenseRegisterFish
                                                                            .Single(x => x.FishId == aquaticOrganismTypeId
                                                                                         && x.PermitLicenseRegisterId == dbPermitLicense.Id
                                                                                         && x.IsActive);
                permitLicenseAquaticOrganism.IsActive = false;
            }

            IEnumerable<QuotaAquaticOrganismDTO> aquaticOrganismsToAddOrUpdate = aquaticOrganismTypes.Except(dbPermitAquaticOrganismTypes);

            foreach (QuotaAquaticOrganismDTO aquaticOrganismType in aquaticOrganismsToAddOrUpdate)
            {
                PermitLicenseRegisterFish permitLicenseAquaticOrganism = Db.PermitLicenseRegisterFish
                                                                           .SingleOrDefault(x => x.FishId == aquaticOrganismType.AquaticOrganismId
                                                                                                 && x.PermitLicenseRegisterId == dbPermitLicense.Id);
                if (permitLicenseAquaticOrganism == null)
                {
                    PermitLicenseRegisterFish permitLicenseRegisterAquaticOrganism = new PermitLicenseRegisterFish
                    {
                        FishId = aquaticOrganismType.AquaticOrganismId,
                        PortOfUnloadingId = aquaticOrganismType.PortId,
                        PermitLicenseRegister = dbPermitLicense
                    };
                    Db.PermitLicenseRegisterFish.Add(permitLicenseRegisterAquaticOrganism);
                }
                else
                {
                    permitLicenseAquaticOrganism.PortOfUnloadingId = aquaticOrganismType.PortId;
                    permitLicenseAquaticOrganism.IsActive = true;
                }
            }
        }

        private void AddOrEditFishingGears(PermitLicensesRegister entry, List<FishingGearDTO> fishingGears)
        {
            if (fishingGears != null)
            {
                foreach (FishingGearDTO fishingGear in fishingGears)
                {
                    fishingGearsService.AddOrEditFishingGear(fishingGear, entry.Id, null);
                }
            }
        }

        private void FillAddApplicationRegisterCommonFields(ICommercialFishingRegister entry,
                                                            CommercialFishingApplicationEditDTO register,
                                                            Application application)
        {
            Db.AddOrEditApplicationSubmittedBy(application, register.SubmittedBy);
            Db.SaveChanges();

            Db.AddOrEditApplicationSubmittedFor(application, register.SubmittedFor);
            Db.SaveChanges();

            entry.SubmittedForPersonId = application.SubmittedForPersonId;
            entry.SubmittedForLegalId = application.SubmittedForLegalId;

            entry.ShipId = register.ShipId.Value;
            entry.IsQualifiedFisherSameAsSubmittedFor = register.QualifiedFisherSameAsSubmittedFor;

            int qualifiedFisherId;

            if (register.QualifiedFisherSameAsSubmittedFor.Value
                && (register.SubmittedFor.Person != null
                    || register.SubmittedFor.SubmittedByRole == SubmittedByRolesEnum.Personal))
            {
                // Ако е същият като получателя, тогава за submittedFor все пак правя ли нов Person ??? TODO
                var identifierDate = (from person in Db.Persons
                                      where person.Id == entry.SubmittedForPersonId
                                      select new
                                      {
                                          EgnLnch = person.EgnLnc,
                                          Type = person.IdentifierType
                                      }).First();

                qualifiedFisherId = (from fisher in Db.FishermenRegisters
                                     join person in Db.Persons on fisher.PersonId equals person.Id
                                     where fisher.RecordType == nameof(RecordTypesEnum.Register)
                                           && person.EgnLnc == identifierDate.EgnLnch
                                           && person.IdentifierType == identifierDate.Type
                                           && fisher.IsActive
                                     select fisher.Id).Single();
            }
            else
            {
                qualifiedFisherId = (from fisher in Db.FishermenRegisters
                                     join person in Db.Persons on fisher.PersonId equals person.Id
                                     where fisher.RecordType == nameof(RecordTypesEnum.Register)
                                           && person.EgnLnc == register.QualifiedFisherIdentifier.EgnLnc
                                           && person.IdentifierType == register.QualifiedFisherIdentifier.IdentifierType.ToString()
                                           && fisher.IsActive
                                     select fisher.Id).Single();
            }

            entry.QualifiedFisherId = qualifiedFisherId;

            ApplicationDelivery delivery = Db.AddDeliveryData(register.DeliveryData);
            application.Delivery = delivery;
        }

        private CommercialFishingApplicationEditDTO MapDbPermitLicenseToPermitLicenseApplicationDto(PermitLicensesRegister dbPermitLicense, bool noIds = false)
        {
            CommercialFishingApplicationEditDTO dto = MapdbPermitToCommercialFishingApplicationDTOCommonFileds(dbPermitLicense, noIds);
            dto.PermitLicensePermitId = dbPermitLicense.PermitId;
            dto.PermitLicensePermitNumber = (from permit in Db.CommercialFishingPermitRegisters
                                             where permit.Id == dbPermitLicense.PermitId
                                             select permit.RegistrationNum).Single();

            if (dto.PageCode == PageCodeEnum.CatchQuataSpecies)
            {
                dto.QuotaAquaticOrganisms = GetPermitLicenseAquaticOrganismTypes(dbPermitLicense.Id);
                dto.UnloaderPhoneNumber = dbPermitLicense.UnloaderPhoneNumber;
            }
            else if (dto.PageCode == PageCodeEnum.PoundnetCommFishLic)
            {
                dto.PoundNetId = dbPermitLicense.PoundNetId;
            }

            if (dto.PageCode != PageCodeEnum.CatchQuataSpecies)
            {
                dto.AquaticOrganismTypeIds = GetPermitLicenseAquaticOrganismTypeIds(dbPermitLicense.Id);
            }

            dto.FishingGears = fishingGearsService.GetCommercialFishingPermitLicenseFishingGears(dbPermitLicense.Id, noIds);

            dto.Files = Db.GetFiles(Db.CommercialFishingPermitLicensesRegisterFile, dbPermitLicense.Id);

            return dto;
        }

        private CommercialFishingApplicationEditDTO MapdbPermitToCommercialFishingApplicationDTOCommonFileds(ICommercialFishingRegister dbEntry, bool noIds = false)
        {
            CommercialFishingApplicationEditDTO result = new CommercialFishingApplicationEditDTO
            {
                Id = noIds ? default(int?) : dbEntry.Id,
                ApplicationId = noIds ? default(int?) : dbEntry.ApplicationId,
                ShipId = dbEntry.ShipId,
                WaterTypeId = dbEntry.WaterTypeId,
                IsPaid = applicationService.IsApplicationPaid(dbEntry.ApplicationId),
                HasDelivery = deliveryService.HasApplicationDelivery(dbEntry.ApplicationId),
                IsOnlineApplication = applicationService.IsApplicationHierarchyType(dbEntry.ApplicationId, ApplicationHierarchyTypesEnum.Online)
            };

            MapQualifiedFisherFiledsToDto(dbEntry, result);

            result.SubmittedBy = applicationService.GetApplicationSubmittedBy(dbEntry.ApplicationId);
            result.SubmittedFor = applicationService.GetApplicationSubmittedFor(dbEntry.ApplicationId);

            if (noIds)
            {
                if (result.SubmittedFor.Legal != null)
                {
                    result.SubmittedFor.Legal.Id = default;
                }

                if (result.SubmittedFor.SubmittedByLetterOfAttorney != null)
                {
                    result.SubmittedFor.SubmittedByLetterOfAttorney.Id = default;
                }
            }

            result.IsHolderShipOwner = dbEntry.IsHolderShipOwner;

            if (dbEntry.ShipGroundsForUse != null)
            {
                result.ShipGroundForUse = MapGroundForUseEntityToDto(dbEntry.ShipGroundsForUse, noIds);
            }

            var applicationData = (from appl in Db.Applications
                                   join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                   where appl.Id == dbEntry.ApplicationId
                                   select new
                                   {
                                       DeliveryId = appl.DeliveryId,
                                       PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                       StatusReason = appl.StatusReason
                                   }).First();

            if (applicationData.DeliveryId != null)
            {
                result.DeliveryData = deliveryService.GetDeliveryData(applicationData.DeliveryId.Value);

                if (noIds)
                {
                    result.DeliveryData.Id = default;
                }
            }

            result.PageCode = applicationData.PageCode;
            result.StatusReason = applicationData.StatusReason;

            if (result.PageCode == PageCodeEnum.PoundnetCommFish || result.PageCode == PageCodeEnum.PoundnetCommFishLic)
            {
                result.PoundNetId = dbEntry.PoundNetId;
            }

            if (!noIds)
            {
                bool isPaid = (from appl in Db.Applications
                               join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                               where appl.Id == dbEntry.ApplicationId
                               select applType.IsPaid).First();

                if (isPaid)
                {
                    result.PaymentInformation = applicationsService.GetApplicationPaymentInformation(dbEntry.ApplicationId);
                }
            }

            return result;
        }

        private void MapQualifiedFisherFiledsToDto(ICommercialFishingRegister dbEntry, CommercialFishingApplicationEditDTO dto)
        {
            var qualifiedFisherData = (from fisher in Db.FishermenRegisters
                                       join person in Db.Persons on fisher.PersonId equals person.Id
                                       where fisher.Id == dbEntry.QualifiedFisherId
                                       select new
                                       {
                                           Id = fisher.Id,
                                           Identifier = new EgnLncDTO
                                           {
                                               EgnLnc = person.EgnLnc,
                                               IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                           },
                                           FirstName = person.FirstName,
                                           MiddleName = person.MiddleName,
                                           LastName = person.LastName
                                       }).First();

            dto.QualifiedFisherId = qualifiedFisherData.Id;
            dto.QualifiedFisherIdentifier = qualifiedFisherData.Identifier;
            dto.QualifiedFisherFirstName = qualifiedFisherData.FirstName;
            dto.QualifiedFisherMiddleName = qualifiedFisherData.MiddleName;
            dto.QualifiedFisherLastName = qualifiedFisherData.LastName;
            dto.QualifiedFisherSameAsSubmittedFor = dbEntry.IsQualifiedFisherSameAsSubmittedFor ?? false;
        }

        private HolderGroundForUseDTO MapGroundForUseEntityToDto(HolderGroundsForUse entity, bool noIds = false)
        {
            return new HolderGroundForUseDTO
            {
                Id = noIds ? default(int?) : entity.Id,
                TypeId = entity.GroundsForUseTypeId,
                GroundForUseValidFrom = entity.GroundsForUseValidFrom,
                GroundForUseValidTo = entity.GroundsForUseValidTo,
                IsGroundForUseUnlimited = entity.IsGroundsForUseUnlimited,
                Number = entity.Number
            };
        }

        private IQueryable<CommercialFishingPermitRegisterDTO> GetParametersFilteredPermits(CommercialFishingRegisterFilters filters)
        {
            DateTime now = DateTime.Now;

            var query = from permit in Db.CommercialFishingPermitRegisters
                        join appl in Db.Applications on permit.ApplicationId equals appl.Id
                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                        join territoryUnit in Db.NterritoryUnits on appl.TerritoryUnitId equals territoryUnit.Id
                        join person in Db.Persons on permit.SubmittedForPersonId equals person.Id into p
                        from submittedForPerson in p.DefaultIfEmpty()
                        join legal in Db.Legals on permit.SubmittedForLegalId equals legal.Id into l
                        from submittedForLegal in l.DefaultIfEmpty()
                        join fisher in Db.FishermenRegisters on permit.QualifiedFisherId equals fisher.Id
                        join personFisher in Db.Persons on fisher.PersonId equals personFisher.Id
                        join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                        join poundnet in Db.PoundNetRegisters on permit.PoundNetId equals poundnet.Id into pNet
                        from poundNet in pNet.DefaultIfEmpty()
                        join type in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals type.Id
                        where permit.RecordType == nameof(RecordTypesEnum.Register)
                              && permit.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            Id = permit.Id,
                            RegistrationNumber = permit.RegistrationNum,
                            TypeId = permit.PermitTypeId,
                            TypeName = type.ShortName,
                            TypeCode = type.Code,
                            TerritoryUnitId = territoryUnit.Id,
                            TerritoryUnitName = territoryUnit.Code + " - " + territoryUnit.Name,
                            ApplicationId = permit.ApplicationId,
                            DeliveryId = appl.DeliveryId,
                            IssueDate = permit.IssueDate.Value,
                            PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                            PermitValidFrom = permit.PermitValidFrom,
                            PermitValidTo = permit.PermitValidTo,
                            SubmittedForPersonId = submittedForPerson != null ? submittedForPerson.Id : default(int?),
                            SubmittedForIdentifier = submittedForPerson != null ? submittedForPerson.EgnLnc : submittedForLegal.Eik,
                            SubmittedForLegalId = submittedForLegal != null ? submittedForLegal.Id : default(int?),
                            SubmittedForName = submittedForPerson != null
                                               ? submittedForPerson.FirstName + " " + submittedForPerson.LastName
                                               : submittedForLegal.Name,
                            QualifiedFisherName = personFisher.FirstName + " " + personFisher.LastName,
                            ShipId = ship.Id,
                            ShipName = ship.Name,
                            ShipCfr = ship.Cfr,
                            ShipExternalMarking = ship.ExternalMark,
                            ShipRegistrationCertificateNumber = ship.RegLicenceNum,
                            PoundNetName = poundNet != null ? poundNet.Name : "",
                            PoundNetNumber = poundNet != null ? poundNet.PoundNetNum : "",
                            IsPermitSuspended = permit.IsSuspended,
                            IsActive = permit.IsActive,
                            IsSuspended = permit.IsSuspended,
                            SuspensionsInformation = (from suspensionHistory in Db.CommercialFishingPermitSuspensionChangeHistories
                                                      join suspensionReason in Db.NsuspensionReasons on suspensionHistory.ReasonId equals suspensionReason.Id
                                                      join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                      where suspensionHistory.PermitId == permit.Id && suspensionHistory.IsActive
                                                      orderby suspensionHistory.EnactmentDate descending
                                                      select suspensionHistory.EnactmentDate.ToString() + " - " + suspensionHistory.SuspensionValidTo.ToString() + " (" + suspensionType.Name + ", " + suspensionReason.Name + ")"
                                                      ).FirstOrDefault()
                        };


            if (filters.FishingGearTypeId != null || !string.IsNullOrEmpty(filters.FishingGearMarkNumber) || !string.IsNullOrEmpty(filters.FishingGearPingerNumber))
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          join fishingGear in Db.FishingGearRegisters on permitLicense.Id equals fishingGear.PermitLicenseId
                                          join fishingGearMark in Db.FishingGearMarks on fishingGear.Id equals fishingGearMark.FishingGearId into fgM
                                          from mark in fgM.DefaultIfEmpty()
                                          join fishingGearPinger in Db.FishingGearPingers on fishingGear.Id equals fishingGearPinger.FishingGearId into fgP
                                          from pinger in fgP.DefaultIfEmpty()
                                          where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                && (filters.FishingGearTypeId == null
                                                    || fishingGear.FishingGearTypeId == filters.FishingGearTypeId)
                                                && (string.IsNullOrEmpty(filters.FishingGearMarkNumber)
                                                    || (mark != null && mark.MarkNum.ToLower() == filters.FishingGearMarkNumber.ToLower()))
                                                && (string.IsNullOrEmpty(filters.FishingGearPingerNumber)
                                                    || (pinger != null && pinger.PingerNum.ToLower() == filters.FishingGearPingerNumber.ToLower()))
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permitIds.Contains(permit.Id)
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.LogbookNumber))
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          join logBookLicense in Db.LogBookPermitLicenses on permitLicense.Id equals logBookLicense.PermitLicenseRegisterId
                                          join logBook in Db.LogBooks on logBookLicense.LogBookId equals logBook.Id
                                          where logBook.LogNum.ToLower().Contains(filters.LogbookNumber.ToLower())
                                                && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permitIds.Contains(permit.Id)
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.Number))
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          where permitLicense.RegistrationNum.ToLower().Contains(filters.Number.ToLower())
                                                && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permit.RegistrationNumber.ToLower().Contains(filters.Number.ToLower()) || permitIds.Contains(permit.Id)
                        select permit;
            }

            if (filters.PermitIsSuspended.HasValue)
            {
                query = from permit in query
                        where permit.IsPermitSuspended == filters.PermitIsSuspended.Value
                        select permit;
            }

            if (filters.PermitLicenseIsSuspended.HasValue)
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                && permitLicense.IsSuspended == filters.PermitLicenseIsSuspended.Value
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permitIds.Contains(permit.Id)
                        select permit;
            }

            if (filters.PermitIsExpired.HasValue)
            {
                query = from permit in query
                        where permit.PermitValidFrom > now || permit.PermitValidTo <= now
                        select permit;
            }

            if (filters.PermitLicenseIsExpired.HasValue)
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                && (permitLicense.PermitLicenseValidFrom > now || permitLicense.PermitLicenseValidTo <= now)
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permitIds.Contains(permit.Id)
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.SubmittedForIdentifier))
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          join person in Db.Persons on permitLicense.SubmittedForPersonId equals person.Id into p
                                          from submittedForPerson in p.DefaultIfEmpty()
                                          join legal in Db.Legals on permitLicense.SubmittedForLegalId equals legal.Id into l
                                          from submittedForLegal in l.DefaultIfEmpty()
                                          where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                && ((submittedForPerson != null && submittedForPerson.EgnLnc == filters.SubmittedForIdentifier)
                                                     || (submittedForLegal != null && submittedForLegal.Eik == filters.SubmittedForIdentifier)
                                                   )
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permit.SubmittedForIdentifier == filters.SubmittedForIdentifier
                              || permitIds.Contains(permit.Id)
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.SubmittedForName))
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          join person in Db.Persons on permitLicense.SubmittedForPersonId equals person.Id into p
                                          from submittedForPerson in p.DefaultIfEmpty()
                                          join legal in Db.Legals on permitLicense.SubmittedForLegalId equals legal.Id into l
                                          from submittedForLegal in l.DefaultIfEmpty()
                                          where ((submittedForPerson != null
                                                    && (submittedForPerson.FirstName.ToLower().Contains(filters.SubmittedForName.ToLower())
                                                        || submittedForPerson.LastName.ToLower().Contains(filters.SubmittedForName.ToLower()))
                                                ) || (submittedForLegal != null
                                                      && submittedForLegal.Eik.Contains(filters.SubmittedForName)))
                                                && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permit.SubmittedForName.ToLower().Contains(filters.SubmittedForName.ToLower()) || permitIds.Contains(permit.Id)
                        select permit;
            }

            if (filters.PermitTypeId.HasValue)
            {
                query = from permit in query
                        where permit.TypeId == filters.PermitTypeId.Value
                        select permit;
            }

            if (filters.PermitLicenseTypeId.HasValue)
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          where permitLicense.PermitLicenseTypeId == filters.PermitLicenseTypeId.Value
                                                && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permitIds.Contains(permit.Id)
                        select permit;
            }

            if (filters.ShipId.HasValue)
            {
                int shipUid = (from ship in Db.ShipsRegister
                               where ship.Id == filters.ShipId.Value
                               select ship.ShipUid).First();

                List<int> shipIds = (from ship in Db.ShipsRegister
                                     where ship.ShipUid == shipUid
                                     select ship.Id).ToList();

                query = from permit in query
                        where shipIds.Contains(permit.ShipId)
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.ShipName))
            {
                query = from permit in query
                        where permit.ShipName.ToLower().Contains(filters.ShipName.ToLower())
                        select permit;
            }

            if (!string.IsNullOrWhiteSpace(filters.ShipCfr))
            {
                query = from permit in query
                        where permit.ShipCfr.ToLower().Contains(filters.ShipCfr.ToLower())
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.ShipExternalMarking))
            {
                query = from permit in query
                        where permit.ShipExternalMarking.ToLower().Contains(filters.ShipExternalMarking.ToLower())
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.ShipRegistrationCertificateNumber))
            {
                query = from permit in query
                        where permit.ShipRegistrationCertificateNumber.ToLower().Contains(filters.ShipRegistrationCertificateNumber.ToLower())
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.PoundNetName))
            {
                query = from permit in query
                        where permit.PoundNetName.ToLower().Contains(filters.PoundNetName.ToLower())
                        select permit;
            }

            if (!string.IsNullOrEmpty(filters.PoundNetNumber))
            {
                query = from permit in query
                        where permit.PoundNetNumber.ToLower().Contains(filters.PoundNetNumber.ToLower())
                        select permit;
            }

            if (filters.IssuedOnRangeStartDate.HasValue && filters.IssuedOnRangeEndDate.HasValue)
            {
                //HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                //                          where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                //                                && permitLicense.IssueDate >= filters.IssuedOnRangeStartDate.Value
                //                                && permitLicense.IssueDate <= filters.IssuedOnRangeEndDate.Value
                //                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where (permit.IssueDate >= filters.IssuedOnRangeStartDate.Value
                                && permit.IssueDate <= filters.IssuedOnRangeEndDate.Value)
                        //|| permitIds.Contains(permit.Id)
                        select permit;
            }

            if (filters.PermitTerritoryUnitId.HasValue)
            {
                query = from permit in query
                        where permit.TerritoryUnitId == filters.PermitTerritoryUnitId.Value
                        select permit;
            }

            if (filters.PermitLicenseTerritoryUnitId.HasValue)
            {
                HashSet<int> permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                          join appl in Db.Applications on permitLicense.ApplicationId equals appl.Id
                                          where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                && appl.TerritoryUnitId == filters.PermitLicenseTerritoryUnitId.Value
                                          select permitLicense.PermitId).ToHashSet();

                query = from permit in query
                        where permitIds.Contains(permit.Id)
                        select permit;
            }

            if (filters.PersonId.HasValue)
            {
                query = from permit in query
                        where permit.SubmittedForPersonId == filters.PersonId
                        select permit;
            }

            if (filters.LegalId.HasValue)
            {
                query = from permit in query
                        where permit.SubmittedForLegalId == filters.LegalId
                        select permit;
            }

            IQueryable<CommercialFishingPermitRegisterDTO> result = from permit in query
                                                                    orderby permit.IssueDate descending
                                                                    select new CommercialFishingPermitRegisterDTO
                                                                    {
                                                                        Id = permit.Id,
                                                                        RegistrationNumber = permit.RegistrationNumber,
                                                                        ApplicationId = permit.ApplicationId,
                                                                        TypeName = permit.TypeName,
                                                                        TypeCode = Enum.Parse<CommercialFishingTypesEnum>(permit.TypeCode),
                                                                        TerritoryUnitName = permit.TerritoryUnitName,
                                                                        DeliveryId = permit.DeliveryId,
                                                                        IssueDate = permit.IssueDate,
                                                                        PageCode = permit.PageCode,
                                                                        SubmittedForName = permit.SubmittedForName,
                                                                        ShipName = permit.ShipName,
                                                                        QualifiedFisherName = permit.QualifiedFisherName,
                                                                        IsActive = permit.IsActive,
                                                                        IsSuspended = permit.IsSuspended,
                                                                        SuspensionsInformation = permit.SuspensionsInformation
                                                                    };

            return result;
        }

        private IQueryable<CommercialFishingPermitRegisterDTO> GetFreeTextFilteredPermits(string text, bool showInactive, int? permitTerritoryUnitId, int? permitLicenseTerritoryUnitId)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            HashSet<int> suspensionPermitIds = (from suspensionHistory in Db.CommercialFishingPermitSuspensionChangeHistories
                                                join suspensionReason in Db.NsuspensionReasons on suspensionHistory.ReasonId equals suspensionReason.Id
                                                join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                where (searchDate.HasValue
                                                       && (suspensionHistory.EnactmentDate.Date == searchDate.Value.Date
                                                           || suspensionHistory.SuspensionValidTo.Date == searchDate.Value.Date)
                                                      )
                                                      || suspensionType.Name.ToLower().Contains(text)
                                                      || suspensionReason.Name.ToLower().Contains(text)
                                                select suspensionHistory.PermitId).ToHashSet();


            HashSet<int> permitIds = new HashSet<int>();
            if (permitLicenseTerritoryUnitId.HasValue)
            {
                permitIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                             join appl in Db.Applications on permitLicense.ApplicationId equals appl.Id
                             where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                   && appl.TerritoryUnitId == permitLicenseTerritoryUnitId.Value
                             select permitLicense.PermitId).ToHashSet();
            }

            IQueryable<CommercialFishingPermitRegisterDTO> result = from permit in Db.CommercialFishingPermitRegisters
                                                                    join appl in Db.Applications on permit.ApplicationId equals appl.Id
                                                                    join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                                    join territoryUnit in Db.NterritoryUnits on appl.TerritoryUnitId equals territoryUnit.Id
                                                                    join person in Db.Persons on permit.SubmittedForPersonId equals person.Id into p
                                                                    from submittedForPerson in p.DefaultIfEmpty()
                                                                    join legal in Db.Legals on permit.SubmittedForLegalId equals legal.Id into l
                                                                    from submittedForLegal in l.DefaultIfEmpty()
                                                                    join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                                                                    join fisher in Db.FishermenRegisters on permit.QualifiedFisherId equals fisher.Id
                                                                    join personFisher in Db.Persons on fisher.PersonId equals personFisher.Id
                                                                    join type in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals type.Id
                                                                    where permit.RecordType == nameof(RecordTypesEnum.Register)
                                                                          && permit.IsActive == !showInactive
                                                                          && (permit.Id.ToString().Contains(text)
                                                                              || (submittedForPerson != null
                                                                                  && (submittedForPerson.FirstName.ToLower().Contains(text)
                                                                                    || submittedForPerson.LastName.ToLower().Contains(text)))
                                                                              || (submittedForLegal != null
                                                                                  && submittedForLegal.Name.ToLower().Contains(text))
                                                                              || personFisher.FirstName.ToLower().Contains(text)
                                                                              || personFisher.LastName.ToLower().Contains(text)
                                                                              || ship.Name.ToLower().Contains(text)
                                                                              || (searchDate.HasValue && permit.IssueDate.Value.Date == searchDate.Value.Date)
                                                                              || suspensionPermitIds.Contains(permit.Id)
                                                                              || territoryUnit.Code.ToLower().Contains(text)
                                                                              || territoryUnit.Name.ToLower().Contains(text)
                                                                             )
                                                                           && (!permitTerritoryUnitId.HasValue || appl.TerritoryUnitId == permitTerritoryUnitId.Value)
                                                                           && (!permitLicenseTerritoryUnitId.HasValue || permitIds.Contains(permit.Id))
                                                                    orderby permit.IssueDate descending
                                                                    select new CommercialFishingPermitRegisterDTO
                                                                    {
                                                                        Id = permit.Id,
                                                                        RegistrationNumber = permit.RegistrationNum,
                                                                        ApplicationId = permit.ApplicationId,
                                                                        TypeName = type.ShortName,
                                                                        TypeId = type.Id,
                                                                        TypeCode = Enum.Parse<CommercialFishingTypesEnum>(type.Code),
                                                                        TerritoryUnitName = territoryUnit.Code + " - " + territoryUnit.Name,
                                                                        DeliveryId = appl.DeliveryId,
                                                                        IssueDate = permit.IssueDate.Value,
                                                                        PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                                                        ShipName = ship.Name,
                                                                        SubmittedForName = submittedForPerson != null
                                                                                           ? submittedForPerson.FirstName + " " + submittedForPerson.LastName
                                                                                           : submittedForLegal.Name,
                                                                        QualifiedFisherName = personFisher.FirstName + " " + personFisher.LastName,
                                                                        IsActive = permit.IsActive,
                                                                        IsSuspended = permit.IsSuspended,
                                                                        SuspensionsInformation = (from suspensionHistory in Db.CommercialFishingPermitSuspensionChangeHistories
                                                                                                  join suspensionReason in Db.NsuspensionReasons on suspensionHistory.ReasonId equals suspensionReason.Id
                                                                                                  join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                                                                  where suspensionHistory.PermitId == permit.Id && suspensionHistory.IsActive
                                                                                                  orderby suspensionHistory.EnactmentDate descending
                                                                                                  select suspensionHistory.EnactmentDate.ToString() + " - " + suspensionHistory.SuspensionValidTo.ToString() + " (" + suspensionType.Name + ", " + suspensionReason.Name + ")"
                                                                                                ).FirstOrDefault()
                                                                    };

            return result;
        }

        private IQueryable<CommercialFishingPermitRegisterDTO> GetAllPermits(bool showInactive)
        {
            IQueryable<CommercialFishingPermitRegisterDTO> result = from permit in Db.CommercialFishingPermitRegisters
                                                                    join appl in Db.Applications on permit.ApplicationId equals appl.Id
                                                                    join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                                    join territoryUnit in Db.NterritoryUnits on appl.TerritoryUnitId equals territoryUnit.Id
                                                                    join person in Db.Persons on permit.SubmittedForPersonId equals person.Id into p
                                                                    from submittedForPerson in p.DefaultIfEmpty()
                                                                    join legal in Db.Legals on permit.SubmittedForLegalId equals legal.Id into l
                                                                    from submittedForLegal in l.DefaultIfEmpty()
                                                                    join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                                                                    join fisher in Db.FishermenRegisters on permit.QualifiedFisherId equals fisher.Id
                                                                    join personFisher in Db.Persons on fisher.PersonId equals personFisher.Id
                                                                    join type in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals type.Id
                                                                    where permit.RecordType == nameof(RecordTypesEnum.Register)
                                                                          && permit.IsActive == !showInactive
                                                                    orderby permit.IssueDate descending
                                                                    select new CommercialFishingPermitRegisterDTO
                                                                    {
                                                                        Id = permit.Id,
                                                                        RegistrationNumber = permit.RegistrationNum,
                                                                        ApplicationId = permit.ApplicationId,
                                                                        DeliveryId = appl.DeliveryId,
                                                                        IssueDate = permit.IssueDate.Value,
                                                                        TypeId = permit.PermitTypeId,
                                                                        TypeCode = Enum.Parse<CommercialFishingTypesEnum>(type.Code),
                                                                        TypeName = type.ShortName,
                                                                        TerritoryUnitName = territoryUnit.Code + " - " + territoryUnit.Name,
                                                                        PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                                                        SubmittedForName = submittedForPerson != null
                                                                                           ? submittedForPerson.FirstName + " " + submittedForPerson.LastName
                                                                                           : submittedForLegal.Name,
                                                                        ShipName = ship.Name,
                                                                        QualifiedFisherName = personFisher.FirstName + " " + personFisher.LastName,
                                                                        IsActive = permit.IsActive,
                                                                        IsSuspended = permit.IsSuspended,
                                                                        SuspensionsInformation = (from suspensionHistory in Db.CommercialFishingPermitSuspensionChangeHistories
                                                                                                  join suspensionReason in Db.NsuspensionReasons on suspensionHistory.ReasonId equals suspensionReason.Id
                                                                                                  join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                                                                  where suspensionHistory.PermitId == permit.Id && suspensionHistory.IsActive
                                                                                                  orderby suspensionHistory.EnactmentDate descending
                                                                                                  select suspensionHistory.EnactmentDate.ToString() + " - " + suspensionHistory.SuspensionValidTo.ToString() + " (" + suspensionType.Name + ", " + suspensionReason.Name + ")"
                                                                                                ).FirstOrDefault()
                                                                    };
            return result;
        }

        private ILookup<int, CommercialFishingLogbookRegisterDTO> GetPermitLicensesLogBooksLookup(List<int> permitLicenseIds)
        {
            ILookup<int, CommercialFishingLogbookRegisterDTO> result = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                        join licenseLogBook in Db.LogBookPermitLicenses on permitLicense.Id equals licenseLogBook.PermitLicenseRegisterId
                                                                        join logBook in Db.LogBooks on licenseLogBook.LogBookId equals logBook.Id
                                                                        join logBookType in Db.NlogBookTypes on logBook.LogBookTypeId equals logBookType.Id
                                                                        join status in Db.NlogBookStatuses on logBook.StatusId equals status.Id
                                                                        where permitLicenseIds.Contains(permitLicense.Id)
                                                                        select new
                                                                        {
                                                                            PermitId = permitLicense.Id,
                                                                            PermitLicenseLogBookId = licenseLogBook.Id,
                                                                            LogbookId = licenseLogBook.LogBookId,
                                                                            LogBookTypeName = logBookType.Name,
                                                                            LogBookNumber = logBook.LogNum,
                                                                            LogBookIssueDate = logBook.IssueDate,
                                                                            LogBookFinishDate = logBook.FinishDate,
                                                                            LogBookStartPageNumber = licenseLogBook.StartPageNum,
                                                                            LogBookEndPageNumber = licenseLogBook.EndPageNum,
                                                                            LogBookStatusName = status.Name,
                                                                            IsActive = logBook.IsActive
                                                                        }).ToLookup(x => x.PermitId, y => new CommercialFishingLogbookRegisterDTO
                                                                        {
                                                                            Id = y.PermitLicenseLogBookId,
                                                                            LogbookId = y.LogbookId,
                                                                            Number = y.LogBookNumber,
                                                                            LogBookTypeName = y.LogBookTypeName,
                                                                            IssueDate = y.LogBookIssueDate,
                                                                            FinishDate = y.LogBookFinishDate,
                                                                            StatusName = y.LogBookStatusName,
                                                                            StartPageNumber = y.LogBookStartPageNumber,
                                                                            EndPageNumber = y.LogBookEndPageNumber,
                                                                            IsActive = y.IsActive
                                                                        });

            return result;
        }

        private List<CommercialFishingPermitLicenseRegisterDTO> GetAllPermitLicenses(IEnumerable<int> permitIds, bool showInactive)
        {
            List<CommercialFishingPermitLicenseRegisterDTO> result = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                      join appl in Db.Applications on permitLicense.ApplicationId equals appl.Id
                                                                      join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                                      join territoryUnit in Db.NterritoryUnits on appl.TerritoryUnitId equals territoryUnit.Id
                                                                      join person in Db.Persons on permitLicense.SubmittedForPersonId equals person.Id into p
                                                                      from submittedForPerson in p.DefaultIfEmpty()
                                                                      join legal in Db.Legals on permitLicense.SubmittedForLegalId equals legal.Id into l
                                                                      from submittedForLegal in l.DefaultIfEmpty()
                                                                      join fisher in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals fisher.Id
                                                                      join personFisher in Db.Persons on fisher.PersonId equals personFisher.Id
                                                                      join type in Db.NcommercialFishingPermitLicenseTypes on permitLicense.PermitLicenseTypeId equals type.Id
                                                                      where permitIds.Contains(permitLicense.PermitId)
                                                                            && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                            && permitLicense.IsActive == !showInactive
                                                                      orderby permitLicense.IssueDate.Value descending
                                                                      select new CommercialFishingPermitLicenseRegisterDTO
                                                                      {
                                                                          Id = permitLicense.Id,
                                                                          RegistrationNumber = permitLicense.RegistrationNum,
                                                                          PermitId = permitLicense.PermitId,
                                                                          IssueDate = permitLicense.IssueDate.Value,
                                                                          PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                                                          ApplicationId = permitLicense.ApplicationId,
                                                                          DeliveryId = appl.DeliveryId,
                                                                          TypeId = permitLicense.PermitLicenseTypeId,
                                                                          TypeCode = Enum.Parse<CommercialFishingTypesEnum>(type.Code),
                                                                          TypeName = type.ShortName,
                                                                          TerritoryUnitName = territoryUnit.Code + " - " + territoryUnit.Name,
                                                                          SubmittedForName = submittedForPerson != null
                                                                                           ? submittedForPerson.FirstName + " " + submittedForPerson.LastName
                                                                                           : submittedForLegal.Name,
                                                                          QualifiedFisherName = personFisher.FirstName + " " + personFisher.LastName,
                                                                          IsActive = permitLicense.IsActive,
                                                                          IsSuspended = permitLicense.IsSuspended,
                                                                          SuspensionsInformation = string.Join
                                                                                                (
                                                                                                    "; ",
                                                                                                    from suspensionHistory in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                                                    join suspensionReason in Db.NsuspensionReasons on suspensionHistory.ReasonId equals suspensionReason.Id
                                                                                                    join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                                                                    where suspensionHistory.PermitLicenseId == permitLicense.Id && suspensionHistory.IsActive
                                                                                                    orderby suspensionHistory.EnactmentDate
                                                                                                    select suspensionHistory.EnactmentDate.ToString("dd.MM.yyyy") + " - " + suspensionHistory.SuspensionValidTo.ToString("dd.MM.yyyy") + " (" + suspensionType.Name + ", " + suspensionReason.Name + ")"
                                                                                                )
                                                                      }).ToList();
            return result;
        }

        private List<CommercialFishingPermitLicenseRegisterDTO> GetParametersFilteredPermitLicenses(IEnumerable<int> permitIds, CommercialFishingRegisterFilters filters)
        {
            DateTime now = DateTime.Now;

            var result = from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                         join appl in Db.Applications on permitLicense.ApplicationId equals appl.Id
                         join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                         join territoryUnit in Db.NterritoryUnits on appl.TerritoryUnitId equals territoryUnit.Id
                         join person in Db.Persons on permitLicense.SubmittedForPersonId equals person.Id into p
                         from submittedForPerson in p.DefaultIfEmpty()
                         join legal in Db.Legals on permitLicense.SubmittedForLegalId equals legal.Id into l
                         from submittedForLegal in l.DefaultIfEmpty()
                         join fisher in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals fisher.Id
                         join personFisher in Db.Persons on fisher.PersonId equals personFisher.Id
                         join ship in Db.ShipsRegister on permitLicense.ShipId equals ship.Id
                         join poundnet in Db.PoundNetRegisters on permitLicense.PoundNetId equals poundnet.Id into pNet
                         from poundNet in pNet.DefaultIfEmpty()
                         join type in Db.NcommercialFishingPermitLicenseTypes on permitLicense.PermitLicenseTypeId equals type.Id
                         join permit in Db.CommercialFishingPermitRegisters on permitLicense.PermitId equals permit.Id
                         join permitPerson in Db.Persons on permit.SubmittedForPersonId equals permitPerson.Id into pPerson
                         from permitSubmittedForPerson in pPerson.DefaultIfEmpty()
                         join permitLegal in Db.Legals on permit.SubmittedForLegalId equals permitLegal.Id into pLegal
                         from permitSubmittedForLegal in pLegal.DefaultIfEmpty()
                         join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                         where permitIds.Contains(permitLicense.PermitId)
                               && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                               && permitLicense.IsActive == !filters.ShowInactiveRecords
                         orderby permitLicense.IssueDate.Value descending
                         select new
                         {
                             Id = permitLicense.Id,
                             RegistrationNumber = permitLicense.RegistrationNum,
                             PermitId = permitLicense.PermitId,
                             DeliveryId = appl.DeliveryId,
                             TypeId = permitLicense.PermitLicenseTypeId,
                             TypeName = type.ShortName,
                             PermitTypeId = permitType.Id,
                             IssueDate = permitLicense.IssueDate.Value,
                             PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                             ApplicationId = permitLicense.ApplicationId,
                             TypeCode = Enum.Parse<CommercialFishingTypesEnum>(type.Code),
                             TerritoryUnitId = appl.TerritoryUnitId,
                             TerritoryUnitName = territoryUnit.Code + " - " + territoryUnit.Name,
                             PermitLicenseValidFrom = permitLicense.PermitLicenseValidFrom,
                             PermitLicenseValidTo = permitLicense.PermitLicenseValidTo,
                             SubmittedForPersonId = submittedForPerson != null ? submittedForPerson.Id : default(int?),
                             SubmittedForIdentifier = submittedForPerson != null ? submittedForPerson.EgnLnc : submittedForLegal.Eik,
                             SubmittedForLegalId = submittedForLegal != null ? submittedForLegal.Id : default(int?),
                             SubmittedForName = submittedForPerson != null
                                                ? submittedForPerson.FirstName + " " + submittedForPerson.LastName
                                                : submittedForLegal.Name,
                             PermitSubmittedForIdentifier = permitSubmittedForPerson != null ? permitSubmittedForPerson.EgnLnc : permitSubmittedForLegal.Eik,
                             PermitSubmittedForName = permitSubmittedForPerson != null
                                                      ? permitSubmittedForPerson.FirstName + " " + permitSubmittedForPerson.LastName
                                                      : permitSubmittedForLegal.Name,
                             QualifiedFisherName = personFisher.FirstName + " " + personFisher.LastName,
                             ShipName = ship.Name,
                             ShipCfr = ship.Cfr,
                             ShipExternalMarking = ship.ExternalMark,
                             ShipRegistrationCertificateNumber = ship.RegLicenceNum,
                             PoundNetName = poundNet != null ? poundNet.Name : "",
                             PoundNetNumber = poundNet != null ? poundNet.PoundNetNum : "",
                             IsSuspended = permitLicense.IsSuspended,
                             IsActive = permitLicense.IsActive,
                             SuspensionsInformation = string.Join
                                                      (
                                                          "; ",
                                                          from suspensionHistory in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                          join suspensionReason in Db.NsuspensionReasons on suspensionHistory.ReasonId equals suspensionReason.Id
                                                          join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                          where suspensionHistory.PermitLicenseId == permitLicense.Id && suspensionHistory.IsActive
                                                          orderby suspensionHistory.EnactmentDate
                                                          select suspensionHistory.EnactmentDate.ToString("dd.MM.yyyy") + " - " + suspensionHistory.SuspensionValidTo.ToString("dd.MM.yyyy") + " (" + suspensionType.Name + ", " + suspensionReason.Name + ")"
                                                      )
                         };

            if (filters.FishingGearTypeId.HasValue)
            {

                HashSet<int> fishingGearPermitLicenseIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                            join fishingGearRegister in Db.FishingGearRegisters on permitLicense.Id equals fishingGearRegister.PermitLicenseId
                                                            where fishingGearRegister.FishingGearTypeId == filters.FishingGearTypeId.Value
                                                            select permitLicense.Id).ToHashSet();

                result = from permitLicense in result
                         where fishingGearPermitLicenseIds.Contains(permitLicense.Id)
                         select permitLicense;
            }

            if (!string.IsNullOrEmpty(filters.FishingGearMarkNumber))
            {
                result = from permitLicense in result
                         join fishingGearRegister in Db.FishingGearRegisters on permitLicense.Id equals fishingGearRegister.PermitLicenseId
                         join mark in Db.FishingGearMarks on fishingGearRegister.Id equals mark.FishingGearId
                         where mark.MarkNum.ToLower().Contains(filters.FishingGearMarkNumber.ToLower())
                         select permitLicense;
            }

            if (!string.IsNullOrEmpty(filters.FishingGearPingerNumber))
            {
                result = from permitLicense in result
                         join fishingGearRegister in Db.FishingGearRegisters on permitLicense.Id equals fishingGearRegister.PermitLicenseId
                         join pinger in Db.FishingGearPingers on fishingGearRegister.Id equals pinger.FishingGearId
                         where pinger.PingerNum.ToLower().Contains(filters.FishingGearPingerNumber.ToLower())
                         select permitLicense;
            }

            if (!string.IsNullOrEmpty(filters.LogbookNumber))
            {
                HashSet<int> permitLicenseIds = (from logBookPermitLicence in Db.LogBookPermitLicenses
                                                 join logBook in Db.LogBooks on logBookPermitLicence.LogBookId equals logBook.Id
                                                 where logBook.LogNum.ToLower().Contains(filters.LogbookNumber.ToLower())
                                                 select logBookPermitLicence.PermitLicenseRegisterId).ToHashSet();
                result = from permitLicense in result
                         where permitLicenseIds.Contains(permitLicense.Id)
                         select permitLicense;
            }

            if (!string.IsNullOrEmpty(filters.Number))
            {
                result = from permitLicense in result
                         where permitLicense.RegistrationNumber.ToLower().Contains(filters.Number.ToLower())
                         select permitLicense;
            }

            if (filters.PermitLicenseIsSuspended.HasValue)
            {
                result = from permitLicense in result
                         where permitLicense.IsSuspended == filters.PermitLicenseIsSuspended.Value
                         select permitLicense;
            }

            if (filters.PermitLicenseIsExpired.HasValue)
            {
                result = from permitLicense in result
                         where permitLicense.PermitLicenseValidFrom > now || permitLicense.PermitLicenseValidTo <= now
                         select permitLicense;
            }

            if (!string.IsNullOrEmpty(filters.SubmittedForIdentifier))
            {
                result = from permitLicense in result
                         where permitLicense.SubmittedForIdentifier == filters.SubmittedForIdentifier
                               || permitLicense.PermitSubmittedForIdentifier == filters.SubmittedForIdentifier
                         select permitLicense;
            }

            if (!string.IsNullOrEmpty(filters.SubmittedForName))
            {
                result = from permitLicense in result
                         where permitLicense.SubmittedForName.ToLower().Contains(filters.SubmittedForName.ToLower())
                               || permitLicense.PermitSubmittedForName.ToLower().Contains(filters.SubmittedForName.ToLower())
                         select permitLicense;
            }

            if (filters.PermitTypeId.HasValue)
            {
                result = from permitLicense in result
                         where permitLicense.PermitTypeId == filters.PermitTypeId.Value
                         select permitLicense;
            }

            if (filters.PermitLicenseTypeId.HasValue)
            {
                result = from permitLicense in result
                         where permitLicense.TypeId == filters.PermitLicenseTypeId.Value
                         select permitLicense;
            }

            if (filters.PermitLicenseTerritoryUnitId.HasValue)
            {
                result = from permitLicense in result
                         where permitLicense.TerritoryUnitId.HasValue
                               && permitLicense.TerritoryUnitId == filters.PermitLicenseTerritoryUnitId.Value
                         select permitLicense;
            }

            //if (filters.IssuedOnRangeStartDate.HasValue && filters.IssuedOnRangeEndDate.HasValue)
            //{
            //    result = from permitLicense in result
            //             where permitLicense.IssueDate >= filters.IssuedOnRangeStartDate.Value
            //                   && permitLicense.IssueDate <= filters.IssuedOnRangeEndDate.Value
            //             select permitLicense;
            //}

            if (filters.PersonId.HasValue)
            {
                result = from permitLicense in result
                         where permitLicense.SubmittedForPersonId == filters.PersonId
                         select permitLicense;
            }

            if (filters.LegalId.HasValue)
            {
                result = from permitLicense in result
                         where permitLicense.SubmittedForLegalId == filters.LegalId
                         select permitLicense;
            }

            List<CommercialFishingPermitLicenseRegisterDTO> permitLicenses = (from permitLicense in result
                                                                              select new CommercialFishingPermitLicenseRegisterDTO
                                                                              {
                                                                                  Id = permitLicense.Id,
                                                                                  RegistrationNumber = permitLicense.RegistrationNumber,
                                                                                  PermitId = permitLicense.PermitId,
                                                                                  DeliveryId = permitLicense.DeliveryId,
                                                                                  IssueDate = permitLicense.IssueDate,
                                                                                  PageCode = permitLicense.PageCode,
                                                                                  ApplicationId = permitLicense.ApplicationId,
                                                                                  TypeId = permitLicense.TypeId,
                                                                                  TypeCode = permitLicense.TypeCode,
                                                                                  TypeName = permitLicense.TypeName,
                                                                                  TerritoryUnitName = permitLicense.TerritoryUnitName,
                                                                                  SubmittedForName = permitLicense.SubmittedForName,
                                                                                  QualifiedFisherName = permitLicense.QualifiedFisherName,
                                                                                  IsActive = permitLicense.IsActive,
                                                                                  IsSuspended = permitLicense.IsSuspended,
                                                                                  SuspensionsInformation = permitLicense.SuspensionsInformation
                                                                              }).ToList();

            return permitLicenses;
        }

        private List<CommercialFishingPermitLicenseRegisterDTO> GetFreeTextFilteredPermitLicenses(IEnumerable<int> permitIds, string text, bool showInactive)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            List<CommercialFishingPermitLicenseRegisterDTO> result = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                      join appl in Db.Applications on permitLicense.ApplicationId equals appl.Id
                                                                      join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                                      join territoryUnit in Db.NterritoryUnits on appl.TerritoryUnitId equals territoryUnit.Id
                                                                      join person in Db.Persons on permitLicense.SubmittedForPersonId equals person.Id into p
                                                                      from submittedForPerson in p.DefaultIfEmpty()
                                                                      join legal in Db.Legals on permitLicense.SubmittedForLegalId equals legal.Id into l
                                                                      from submittedForLegal in l.DefaultIfEmpty()
                                                                      join fisher in Db.FishermenRegisters on permitLicense.QualifiedFisherId equals fisher.Id
                                                                      join personFisher in Db.Persons on fisher.PersonId equals personFisher.Id
                                                                      join type in Db.NcommercialFishingPermitLicenseTypes on permitLicense.PermitLicenseTypeId equals type.Id
                                                                      where permitIds.Contains(permitLicense.PermitId)
                                                                            && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                            && permitLicense.IsActive == !showInactive
                                                                            && (permitLicense.RegistrationNum.Contains(text)
                                                                                || (submittedForPerson != null
                                                                                    && (submittedForPerson.FirstName.ToLower().Contains(text)
                                                                                        || submittedForPerson.LastName.ToLower().Contains(text)))
                                                                                || (submittedForLegal != null
                                                                                    && submittedForLegal.Name.ToLower().Contains(text))
                                                                                || personFisher.FirstName.ToLower().Contains(text)
                                                                                || personFisher.LastName.ToLower().Contains(text)
                                                                                || (searchDate.HasValue && permitLicense.IssueDate.Value.Date == searchDate.Value.Date)
                                                                               )
                                                                      orderby permitLicense.IssueDate.Value descending
                                                                      select new CommercialFishingPermitLicenseRegisterDTO
                                                                      {
                                                                          Id = permitLicense.Id,
                                                                          RegistrationNumber = permitLicense.RegistrationNum,
                                                                          PermitId = permitLicense.PermitId,
                                                                          IssueDate = permitLicense.IssueDate.Value,
                                                                          PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                                                          ApplicationId = permitLicense.ApplicationId,
                                                                          DeliveryId = appl.DeliveryId,
                                                                          TypeId = permitLicense.PermitLicenseTypeId,
                                                                          TypeCode = Enum.Parse<CommercialFishingTypesEnum>(type.Code),
                                                                          TypeName = type.ShortName,
                                                                          TerritoryUnitName = territoryUnit.Code + " - " + territoryUnit.Name,
                                                                          SubmittedForName = submittedForPerson != null
                                                                                           ? submittedForPerson.FirstName + " " + submittedForPerson.LastName
                                                                                           : submittedForLegal.Name,
                                                                          QualifiedFisherName = personFisher.FirstName + " " + personFisher.LastName,
                                                                          IsActive = permitLicense.IsActive,
                                                                          IsSuspended = permitLicense.IsSuspended,
                                                                          SuspensionsInformation = string.Join
                                                                                                (
                                                                                                    "; ",
                                                                                                    from suspensionHistory in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                                                    join suspensionReason in Db.NsuspensionReasons on suspensionHistory.ReasonId equals suspensionReason.Id
                                                                                                    join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                                                                    where suspensionHistory.PermitLicenseId == permitLicense.Id && suspensionHistory.IsActive
                                                                                                    orderby suspensionHistory.EnactmentDate
                                                                                                    select suspensionHistory.EnactmentDate.ToString("dd.MM.yyyy") + " - " + suspensionHistory.SuspensionValidTo.ToString("dd.MM.yyyy") + " (" + suspensionType.Name + ", " + suspensionReason.Name + ")"
                                                                                                )
                                                                      }).ToList();

            return result;
        }

        private CommercialFishingEditDTO MapDbPermitToDTO(PermitRegister permit, bool isApplication = false)
        {
            CommercialFishingEditDTO model = new CommercialFishingEditDTO
            {
                PermitRegistrationNumber = permit.RegistrationNum,
                Type = Enum.Parse<CommercialFishingTypesEnum>(permit.PermitType.Code)
            };

            MapDbCommonCommercialFishingFieldsToDTO(model, permit, isApplication);

            if (model.PageCode == PageCodeEnum.PoundnetCommFish && permit.PoundNetGroundsForUse != null)
            {
                model.PoundNetGroundForUse = MapGroundForUseEntityToDto(permit.PoundNetGroundsForUse);

                if (isApplication)
                {
                    model.PoundNetGroundForUse.Id = default;
                }
            }

            if (!isApplication)
            {
                model.ValidFrom = permit.PermitValidFrom.Value;
                model.ValidTo = permit.PermitValidTo;
                model.IsPermitUnlimited = permit.IsPermitUnlimited.Value;

                model.Suspensions = GetPermitSuspensions(permit.Id);
                model.DuplicateEntries = duplicatesRegisterService.GetDuplicateEntries(permitId: permit.Id);
            }

            model.Files = Db.GetFiles(Db.CommercialFishingPermitRegisterFiles, permit.Id);

            return model;
        }

        private CommercialFishingEditDTO MapDbPermitLicenseToDTO(PermitLicensesRegister permitLicense, bool isApplication = false)
        {
            CommercialFishingEditDTO model = new CommercialFishingEditDTO
            {
                PermitLicenseRegistrationNumber = permitLicense.RegistrationNum,

                Type = Enum.Parse<CommercialFishingTypesEnum>(permitLicense.PermitLicenseType.Code),
                PermitLicensePermitId = permitLicense.PermitId
            };

            MapDbCommonCommercialFishingFieldsToDTO(model, permitLicense, isApplication);

            if (!isApplication)
            {
                model.ValidFrom = permitLicense.PermitLicenseValidFrom.Value;
                model.ValidTo = permitLicense.PermitLicenseValidTo.Value;

                model.Suspensions = GetPermitLicenseSuspensions(permitLicense.Id);
                model.DuplicateEntries = duplicatesRegisterService.GetDuplicateEntries(permitLicenceId: permitLicense.Id);
            }

            if (model.PageCode == PageCodeEnum.CatchQuataSpecies)
            {
                model.QuotaAquaticOrganisms = GetPermitLicenseAquaticOrganismTypes(permitLicense.Id);
                model.UnloaderPhoneNumber = permitLicense.UnloaderPhoneNumber;
            }
            else if (model.PageCode == PageCodeEnum.PoundnetCommFishLic)
            {
                model.PoundNetId = permitLicense.PoundNetId;
            }

            if (model.PageCode != PageCodeEnum.CatchQuataSpecies)
            {
                model.AquaticOrganismTypeIds = GetPermitLicenseAquaticOrganismTypeIds(permitLicense.Id);
            }

            model.FishingGears = fishingGearsService.GetCommercialFishingPermitLicenseFishingGears(permitLicense.Id, isApplication);

            model.LogBooks = logBooksService.GetPermitLicenseLogBooks(permitLicense.Id);

            model.Files = Db.GetFiles(Db.CommercialFishingPermitLicensesRegisterFile, permitLicense.Id);

            return model;
        }

        private int GetShipUId(int shipId)
        {
            return (from ship in Db.ShipsRegister
                    where ship.Id == shipId
                    select ship.ShipUid).First();
        }

        private List<int> GetShipIds(int shipUId)
        {
            return (from ship in Db.ShipsRegister
                    where ship.ShipUid == shipUId
                    select ship.Id).ToList();
        }

        private Tuple<int, string> AddApplicationForPermitLicense(Application permitApplication, int currentUserId, ApplicationHierarchyTypesEnum applicationHierarchyType)
        {
            DateTime now = DateTime.Now;

            int applicationTypeId = (from applType in Db.NapplicationTypes
                                     where applType.PageCode == nameof(PageCodeEnum.RightToFishResource)
                                           && applType.ValidFrom <= now
                                           && applType.ValidTo > now
                                     select applType.Id).Single();

            Tuple<int, string> applicationData = applicationsService.AddApplication(applicationTypeId, applicationHierarchyType, currentUserId);

            return applicationData;
        }

        private PermitRegister AddPermitApplicationNoTransaction(CommercialFishingApplicationEditDTO permit)
        {
            int permitTypeId = GetTypeIdByPageCode(permit.PageCode.Value);

            PermitRegister entry = new PermitRegister
            {
                ApplicationId = permit.ApplicationId.Value,
                RecordType = nameof(RecordTypesEnum.Application),
                PermitTypeId = permitTypeId,
                ShipId = permit.ShipId.Value,
                WaterTypeId = permit.WaterTypeId.Value
            };

            Application application = (from appl in Db.Applications
                                       where appl.Id == entry.ApplicationId
                                       select appl).First();

            FillAddApplicationRegisterCommonFields(entry, permit, application);

            if (permit.PageCode == PageCodeEnum.PoundnetCommFish)
            {
                entry.IsHolderShipOwner = permit.IsHolderShipOwner.Value;

                SetPermitPoudnetIdAndGroundsForUse(entry, permit.PoundNetId.Value, permit.PoundNetGroundForUse);

                AddOrEditHolderShipGroundsForUse(entry, permit.ShipGroundForUse);
            }
            else
            {
                entry.IsHolderShipOwner = true;
            }

            if (permit.Files != null)
            {
                foreach (FileInfoDTO file in permit.Files)
                {
                    Db.AddOrEditFile(entry, entry.PermitRegisterFiles, file);
                }
            }

            Db.CommercialFishingPermitRegisters.Add(entry);
            Db.SaveChanges();

            return entry;
        }

        private CommercialFishingApplicationEditDTO GetPermitLicenseApplicationFromPermit(int permitId, int applicationId)
        {
            PermitRegister dbPermit = (from permit in Db.CommercialFishingPermitRegisters
                                                        .AsSplitQuery()
                                                        .Include(x => x.ShipGroundsForUse)
                                       where permit.Id == permitId
                                       select permit).AsSplitQuery().First();

            PageCodeEnum applPageCode = (from appl in Db.Applications
                                         join type in Db.NapplicationTypes on appl.ApplicationTypeId equals type.Id
                                         where appl.Id == applicationId
                                         select Enum.Parse<PageCodeEnum>(type.PageCode)).First();

            CommercialFishingApplicationEditDTO result = new CommercialFishingApplicationEditDTO
            {
                ApplicationId = applicationId,
                QualifiedFisherId = dbPermit.QualifiedFisherId,
                ShipId = dbPermit.ShipId,
                PoundNetId = dbPermit.PoundNetId,
                WaterTypeId = dbPermit.WaterTypeId,
                PageCode = applPageCode,
                IsPaid = applicationService.IsApplicationPaid(applicationId),
                HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
            };

            MapQualifiedFisherFiledsToDto(dbPermit, result);

            result.SubmittedBy = applicationService.GetApplicationSubmittedBy(dbPermit.ApplicationId);
            result.SubmittedFor = applicationService.GetApplicationSubmittedFor(dbPermit.ApplicationId);

            if (result.SubmittedFor.Legal != null)
            {
                result.SubmittedFor.Legal.Id = null;
            }

            result.IsHolderShipOwner = dbPermit.IsHolderShipOwner;

            if (!result.IsHolderShipOwner.Value)
            {
                result.ShipGroundForUse = MapGroundForUseEntityToDto(dbPermit.ShipGroundsForUse);
                result.ShipGroundForUse.Id = default;
            }

            result.IsPaid = applicationService.IsApplicationPaid(applicationId);
            result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

            if (result.HasDelivery)
            {
                int? dbPermitDeliveryId = (from appl in Db.Applications
                                           join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                           where appl.Id == dbPermit.ApplicationId
                                           select appl.DeliveryId).First();

                result.DeliveryData = deliveryService.GetDeliveryData(dbPermitDeliveryId.Value);
            }

            if (result.IsPaid && result.PaymentInformation == null)
            {
                result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
            }

            return result;
        }

        private List<int> GetPermitLicenseAquaticOrganismTypeIds(int permitLicenseId)
        {
            return (from aquaticOrganism in Db.PermitLicenseRegisterFish
                    where aquaticOrganism.PermitLicenseRegisterId == permitLicenseId && aquaticOrganism.IsActive
                    select aquaticOrganism.FishId).ToList();
        }

        private List<QuotaAquaticOrganismDTO> GetPermitLicenseAquaticOrganismTypes(int permitLicenseId)
        {
            return (from aquaticOrganism in Db.PermitLicenseRegisterFish
                    where aquaticOrganism.PermitLicenseRegisterId == permitLicenseId && aquaticOrganism.IsActive
                    select new QuotaAquaticOrganismDTO
                    {
                        AquaticOrganismId = aquaticOrganism.FishId,
                        PortId = aquaticOrganism.PortOfUnloadingId.Value
                    }).ToList();
        }

        private void AddOrEditLogBook(CommercialFishingLogBookEditDTO licenseLogBook, PermitLicensesRegister dbPermitLicense, bool ignoreLogBookConflicts)
        {
            LogBook dbLogBook;

            if (!licenseLogBook.LogBookLicenseId.HasValue) // нова връзка на дневник към удостоверение
            {
                if (!licenseLogBook.LogBookId.HasValue) // нов дневник
                {
                    if (licenseLogBook.IsOnline.Value)
                    {
                        licenseLogBook.StartPageNumber = 1;
                        licenseLogBook.EndPageNumber = DefaultConstants.JS_MAX_SAFE_INTEGER;
                        licenseLogBook.PermitLicenseStartPageNumber = licenseLogBook.StartPageNumber;
                        licenseLogBook.PermitLicenseEndPageNumber = licenseLogBook.EndPageNumber;
                    }

                    int? shipId = null, personId = null, legalId = null;

                    if (licenseLogBook.OwnerType.HasValue)
                    {
                        if (licenseLogBook.OwnerType == LogBookPagePersonTypesEnum.Person)
                        {
                            personId = dbPermitLicense.SubmittedForPersonId;
                        }
                        else if (licenseLogBook.OwnerType == LogBookPagePersonTypesEnum.LegalPerson)
                        {
                            legalId = dbPermitLicense.SubmittedForLegalId;
                        }
                    }
                    else
                    {
                        shipId = dbPermitLicense.ShipId;
                    }

                    dbLogBook = Db.AddShipLogBook(licenseLogBook, ignoreLogBookConflicts, shipId, personId, legalId);
                }
                else
                {
                    dbLogBook = Db.EditShipLogBook(licenseLogBook, ignoreLogBookConflicts);
                    Db.SaveChanges();
                }

                dbLogBook.CurrentPermitLicense = dbPermitLicense;
                AddLogBooksPermitLicenseChangeHistory(dbLogBook, licenseLogBook, dbPermitLicense, ignoreLogBookConflicts);
            }
            else
            {
                dbLogBook = Db.EditShipLogBook(licenseLogBook, ignoreLogBookConflicts);
                dbLogBook.CurrentPermitLicense = dbPermitLicense;

                EditLogBookPermitLicensePages(licenseLogBook);
                Db.SaveChanges();
            }
        }

        private void AddLogBooksPermitLicenseChangeHistory(LogBook dbLogBook,
                                                           CommercialFishingLogBookEditDTO logBook,
                                                           PermitLicensesRegister dbPermitLicense,
                                                           bool ignoreLogBookConflicts)
        {
            DateTime now = DateTime.Now;

            LogBookStatusesEnum logBookStatus = (from lbStatus in Db.NlogBookStatuses
                                                 where lbStatus.Id == logBook.StatusId.Value
                                                 select Enum.Parse<LogBookStatusesEnum>(lbStatus.Code)).First();

            if (logBook.IsForRenewal)
            {
                if (logBookStatus != LogBookStatusesEnum.Finished)
                {
                    List<LogBookPermitLicense> overlappingLogBookPermitLicenseRanges = GetOverlappingLogBookPermitLicensePageRanges(logBook.LogBookId.Value,
                                                                                                                                logBook.PermitLicenseStartPageNumber.Value,
                                                                                                                                logBook.PermitLicenseEndPageNumber.Value);
                    if (overlappingLogBookPermitLicenseRanges.Count > 0)
                    {
                        LogBookPermitLicense firstLogBookLicense = overlappingLogBookPermitLicenseRanges.First();

                        if (firstLogBookLicense.StartPageNum.HasValue && firstLogBookLicense.EndPageNum.HasValue)
                        {
                            bool hasPagesInRange = HasLogBookPagesInRange(logBook.LogBookTypeId,
                                                                          logBook.LogBookId.Value,
                                                                          logBook.IsOnline.Value,
                                                                          logBook.PermitLicenseStartPageNumber.Value,
                                                                          logBook.PermitLicenseStartPageNumber.Value);

                            if (!ignoreLogBookConflicts && hasPagesInRange)
                            {
                                throw new InvalidLogBookLicensePagesRangeException();
                            }

                            firstLogBookLicense.EndPageNum = logBook.PermitLicenseStartPageNumber - 1;

                            if (overlappingLogBookPermitLicenseRanges.Count == 1 && firstLogBookLicense.EndPageNum <= firstLogBookLicense.StartPageNum)
                            {
                                firstLogBookLicense.StartPageNum = null;
                                firstLogBookLicense.EndPageNum = null;
                            }
                        }

                        foreach (LogBookPermitLicense range in overlappingLogBookPermitLicenseRanges.Where(x => x.Id != firstLogBookLicense.Id))
                        {
                            bool hasPagesInRange = HasLogBookPagesInRange(logBook.LogBookTypeId,
                                                                          logBook.LogBookId.Value,
                                                                          logBook.IsOnline.Value,
                                                                          range.StartPageNum.Value,
                                                                          range.EndPageNum.Value);

                            if (!ignoreLogBookConflicts && hasPagesInRange)
                            {
                                throw new InvalidLogBookLicensePagesRangeException();
                            }

                            range.StartPageNum = null;
                            range.EndPageNum = null;
                        }

                        LogBookPermitLicense lastLogBookLicense = overlappingLogBookPermitLicenseRanges.Last();
                        lastLogBookLicense.LogBookValidTo = now;
                    }
                    else
                    {
                        LogBookPermitLicense lastLogBookLicense = (from logBookLicense in Db.LogBookPermitLicenses
                                                                   where logBookLicense.Id == logBook.LastLogBookLicenseId
                                                                   select logBookLicense).First();
                        lastLogBookLicense.LogBookValidTo = now;
                    }
                }
            }

            if (!logBook.IsForRenewal || (logBook.IsForRenewal && logBookStatus != LogBookStatusesEnum.Finished))
            {
                Db.LogBookPermitLicenses.Add(new LogBookPermitLicense
                {
                    LogBook = dbLogBook,
                    PermitLicenseRegister = dbPermitLicense,
                    StartPageNum = logBook.PermitLicenseStartPageNumber,
                    EndPageNum = logBook.PermitLicenseEndPageNumber,
                    LogBookValidFrom = now,
                    LogBookValidTo = DefaultConstants.MAX_VALID_DATE
                });
            }
        }


        private int GetPermitIdByRegistrationNumber(string permitNumber)
        {
            int permitId = (from permit in Db.CommercialFishingPermitRegisters
                            where permit.RegistrationNum == permitNumber
                            select permit.Id).Single();

            return permitId;
        }

        private List<int> GetValidShipPermitIds(int shipId)
        {
            int shipUId = (from ship in Db.ShipsRegister
                           where ship.Id == shipId
                           select ship.ShipUid).First();

            List<int> result = (from permit in Db.CommercialFishingPermitRegisters
                                join ship in Db.ShipsRegister on permit.ShipId equals ship.Id
                                where permit.RecordType == nameof(RecordTypesEnum.Register)
                                      && ship.ShipUid == shipUId
                                      && !permit.IsSuspended
                                      && permit.IsActive
                                select permit.Id).ToList();
            return result;
        }

        private bool HasLogBookPagesInRange(int logBookTypeId, int logBookId, bool isOnline, long startPage, long endPage)
        {
            bool hasPagesInRange = true;
            LogBookTypesEnum logBookType = (from lbType in Db.NlogBookTypes
                                            where lbType.Id == logBookTypeId
                                            select Enum.Parse<LogBookTypesEnum>(lbType.Code)).First();

            switch (logBookType)
            {
                case LogBookTypesEnum.Ship:
                    hasPagesInRange = HasLogBookShipPagesInRange(logBookId, isOnline, startPage, endPage);
                    break;
                case LogBookTypesEnum.Admission:
                    hasPagesInRange = HasLogBookAdmissionPagesInRange(logBookId, startPage, endPage);
                    break;
                case LogBookTypesEnum.Transportation:
                    hasPagesInRange = HasLogBookTransportationPagesInRange(logBookId, startPage, endPage);
                    break;
            }

            return hasPagesInRange;
        }

        private bool HasLogBookShipPagesInRange(int logBookId, bool isOnline, long startPage, long endPage)
        {
            bool hasPages;
            List<long> shipPageNumbers;

            if (isOnline)
            {
                shipPageNumbers = (from logBook in Db.LogBooks
                                   join shipPage in Db.ShipLogBookPages on logBook.Id equals shipPage.LogBookId
                                   where logBook.Id == logBookId && shipPage.IsActive
                                   select shipPage.PageNum).ToList()
                                                           .Select(x => x.Split('-').LastOrDefault())
                                                           .Where(x => x != null)
                                                           .Select(x => long.Parse(x))
                                                           .ToList();
            }
            else
            {
                shipPageNumbers = (from shipPage in Db.ShipLogBookPages
                                   where shipPage.LogBookId == logBookId
                                         && shipPage.IsActive
                                   select shipPage.PageNum).ToList()
                                                           .Select(x => long.Parse(x))
                                                           .ToList();
            }

            hasPages = shipPageNumbers.Any(x => x >= startPage && x <= endPage);

            return hasPages;
        }

        private bool HasLogBookTransportationPagesInRange(int logBookId, decimal startPage, decimal endPage)
        {
            bool hasPages = (from transportationPage in Db.TransportationLogBookPages
                             where transportationPage.LogBookId == logBookId
                                   && transportationPage.IsActive
                                   && transportationPage.PageNum >= startPage
                                   && transportationPage.PageNum <= endPage
                             select transportationPage.Id).Any();

            return hasPages;
        }

        private bool HasLogBookAdmissionPagesInRange(int logBookId, decimal startPage, decimal endPage)
        {
            bool hasPages = (from admissionPage in Db.AdmissionLogBookPages
                             where admissionPage.LogBookId == logBookId
                                   && admissionPage.IsActive
                                   && admissionPage.PageNum >= startPage
                                   && admissionPage.PageNum <= endPage
                             select admissionPage.Id).Any();

            return hasPages;
        }

        private List<LogBookPermitLicense> GetOverlappingLogBookPermitLicensePageRanges(int logBookId, long startPage, long endPage)
        {
            List<LogBookPermitLicense> results = (from logBookPermitLicense in Db.LogBookPermitLicenses
                                                  where logBookPermitLicense.LogBookId == logBookId
                                                        && logBookPermitLicense.StartPageNum.HasValue
                                                        && logBookPermitLicense.EndPageNum.HasValue
                                                        && !(endPage < logBookPermitLicense.StartPageNum || logBookPermitLicense.EndPageNum < startPage)
                                                  orderby logBookPermitLicense.LogBookValidFrom
                                                  select logBookPermitLicense).ToList();

            return results;
        }

        private void EditLogBookPermitLicensePages(CommercialFishingLogBookEditDTO logBook)
        {
            LogBookPermitLicense dbLicenseLogBook = (from logBookPermitLicense in Db.LogBookPermitLicenses
                                                     where logBookPermitLicense.Id == logBook.LogBookLicenseId
                                                     select logBookPermitLicense).First();

            dbLicenseLogBook.StartPageNum = logBook.PermitLicenseStartPageNumber;
            dbLicenseLogBook.EndPageNum = logBook.PermitLicenseEndPageNumber;
            dbLicenseLogBook.IsActive = logBook.PermitLicenseIsActive;
        }

        private int? GetPermitIdByRegistraionNumber(string registrationNumber, List<int> shipIds)
        {
            int permitLicensePermitId = (from permit in Db.CommercialFishingPermitRegisters
                                         where permit.RegistrationNum == registrationNumber
                                               && shipIds.Contains(permit.ShipId)
                                               && permit.RecordType == nameof(RecordTypesEnum.Register)
                                         select permit.Id).SingleOrDefault();

            return permitLicensePermitId == default ? default(int?) : permitLicensePermitId;
        }

        private void SetPermitPoudnetIdAndGroundsForUse(PermitRegister entry, int poundNetId, HolderGroundForUseDTO groundForUse)
        {
            entry.PoundNetId = poundNetId;
            AddOrEditPoundNetGroundsForUse(entry, groundForUse);
        }

        private void SetHolderGroundsForUseEntityFields(HolderGroundsForUse entry, HolderGroundForUseDTO dto)
        {
            entry.GroundsForUseTypeId = dto.TypeId.Value;
            entry.Number = dto.Number;
            entry.GroundsForUseValidFrom = dto.GroundForUseValidFrom;
            entry.IsGroundsForUseUnlimited = dto.IsGroundForUseUnlimited.Value;

            if (!dto.IsGroundForUseUnlimited.Value)
            {
                entry.GroundsForUseValidTo = dto.GroundForUseValidTo.Value;
            }
            else
            {
                entry.GroundsForUseValidTo = null;
            }
        }

        private void AddOrEditPoundNetGroundsForUse(PermitRegister entity, HolderGroundForUseDTO groundForUse)
        {
            if (groundForUse == null)
            {
                HolderGroundsForUse holderGroundsForUseEntry = (from groundsForUse in Db.HolderGroundsForUses
                                                                where groundsForUse.Id == entity.PoundNetGroundsForUseId
                                                                select groundsForUse).FirstOrDefault();

                if (holderGroundsForUseEntry != null)
                {
                    holderGroundsForUseEntry.IsActive = false;
                }

                entity.PoundNetGroundsForUse = null;
            }
            else
            {
                HolderGroundsForUse holderGroundsForUseEntry = (from groundsForUse in Db.HolderGroundsForUses
                                                                where groundsForUse.Id == groundForUse.Id
                                                                select groundsForUse).FirstOrDefault();

                if (holderGroundsForUseEntry == null)
                {
                    holderGroundsForUseEntry = new HolderGroundsForUse();
                    SetHolderGroundsForUseEntityFields(holderGroundsForUseEntry, groundForUse);
                    entity.PoundNetGroundsForUse = holderGroundsForUseEntry;
                }
                else
                {
                    SetHolderGroundsForUseEntityFields(holderGroundsForUseEntry, groundForUse);
                }
            }
        }

        private void AddOrEditHolderShipGroundsForUse(ICommercialFishingRegister entity, HolderGroundForUseDTO groundForUse)
        {

            if (groundForUse == null)
            {
                HolderGroundsForUse holderGroundsForUseEntry = (from groundsForUse in Db.HolderGroundsForUses
                                                                where groundsForUse.Id == entity.ShipGroundsForUseId
                                                                select groundsForUse).FirstOrDefault();

                if (holderGroundsForUseEntry != null)
                {
                    holderGroundsForUseEntry.IsActive = false;
                }

                entity.ShipGroundsForUse = null;
            }
            else
            {
                HolderGroundsForUse holderGroundsForUseEntry = (from groundsForUse in Db.HolderGroundsForUses
                                                                where groundsForUse.Id == groundForUse.Id
                                                                select groundsForUse).FirstOrDefault();

                if (holderGroundsForUseEntry == null)
                {
                    holderGroundsForUseEntry = new HolderGroundsForUse();
                    SetHolderGroundsForUseEntityFields(holderGroundsForUseEntry, groundForUse);
                    entity.ShipGroundsForUse = holderGroundsForUseEntry;
                }
                else
                {
                    SetHolderGroundsForUseEntityFields(holderGroundsForUseEntry, groundForUse);
                }
            }
        }

        private void MapDbCommonCommercialFishingFieldsToDTO(CommercialFishingEditDTO dto, ICommercialFishingRegister dbRegister, bool isApplication)
        {
            if (!isApplication)
            {
                dto.Id = dbRegister.Id;
            }
            dto.ApplicationId = dbRegister.ApplicationId;
            dto.QualifiedFisherId = dbRegister.QualifiedFisherId;

            var qualifiedFisherData = (from fisher in Db.FishermenRegisters
                                       join person in Db.Persons on fisher.PersonId equals person.Id
                                       where fisher.Id == dbRegister.QualifiedFisherId
                                       select new
                                       {
                                           EngLnc = new EgnLncDTO
                                           {
                                               EgnLnc = person.EgnLnc,
                                               IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                           },
                                           person.FirstName,
                                           person.MiddleName,
                                           person.LastName
                                       }).First();

            dto.QualifiedFisherIdentifier = qualifiedFisherData.EngLnc;
            dto.QualifiedFisherFirstName = qualifiedFisherData.FirstName;
            dto.QualifiedFisherMiddleName = qualifiedFisherData.MiddleName;
            dto.QualifiedFisherLastName = qualifiedFisherData.LastName;

            dto.ShipId = dbRegister.ShipId;
            dto.WaterTypeId = dbRegister.WaterTypeId;
            dto.PoundNetId = dbRegister.PoundNetId;
            dto.QualifiedFisherSameAsSubmittedFor = dbRegister.IsQualifiedFisherSameAsSubmittedFor ?? false;

            if (!isApplication)
            {
                dto.IssueDate = dbRegister.IssueDate.Value;
            }

            dto.PageCode = (from appl in Db.Applications
                            join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                            where appl.Id == dbRegister.ApplicationId
                            select Enum.Parse<PageCodeEnum>(applType.PageCode)).First();

            if (dto.PageCode == PageCodeEnum.CommFish) // Когато имаме обикновено разрешително, винаги титулярът трябва да е собственик на кораба
            {
                dto.IsHolderShipOwner = true;
            }
            else
            {
                dto.IsHolderShipOwner = dbRegister.IsHolderShipOwner;
            }

            if (!dto.IsHolderShipOwner.Value)
            {
                dto.ShipGroundForUse = MapGroundForUseEntityToDto(dbRegister.ShipGroundsForUse);

                if (isApplication)
                {
                    dto.ShipGroundForUse.Id = default;
                }
            }

            if (isApplication)
            {
                dto.SubmittedFor = applicationService.GetApplicationSubmittedFor(dbRegister.ApplicationId);
            }
            else
            {
                dto.SubmittedFor = applicationService.GetRegisterSubmittedFor(dbRegister.ApplicationId, dbRegister.SubmittedForPersonId, dbRegister.SubmittedForLegalId);
            }

            dto.IsOnlineApplication = applicationService.IsApplicationHierarchyType(dbRegister.ApplicationId, ApplicationHierarchyTypesEnum.Online);
        }

        private CommercialFishingApplicationDataIds GetApplicationIds(int applicationId)
        {
            PageCodeEnum pageCode = (from appl in Db.Applications
                                     join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                     where appl.Id == applicationId
                                     select Enum.Parse<PageCodeEnum>(applType.PageCode)).First();

            CommercialFishingApplicationDataIds data = null;

            switch (pageCode)
            {
                case PageCodeEnum.CommFish:
                case PageCodeEnum.PoundnetCommFish:
                case PageCodeEnum.RightToFishThirdCountry:
                    {
                        data = (from permit in Db.CommercialFishingPermitRegisters
                                join appl in Db.Applications on permit.ApplicationId equals appl.Id
                                join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                where permit.ApplicationId == applicationId
                                      && permit.RecordType == nameof(RecordTypesEnum.Application)
                                select new CommercialFishingApplicationDataIds
                                {
                                    PermitId = permit.Id,
                                    ApplicationId = permit.ApplicationId,
                                    PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                    SubmittedByPersonId = appl.SubmittedByPersonId.Value,
                                    SubmittedByPersonRoleId = appl.SubmittedByPersonRoleId.Value,
                                    SubmittedForPersonId = appl.SubmittedForPersonId.HasValue ? appl.SubmittedForPersonId.Value : default,
                                    SubmittedForLegalId = appl.SubmittedForLegalId.HasValue ? appl.SubmittedForLegalId.Value : default
                                }).Single();
                    }
                    break;
                case PageCodeEnum.RightToFishResource:
                case PageCodeEnum.CatchQuataSpecies:
                case PageCodeEnum.PoundnetCommFishLic:
                    {
                        data = (from permit in Db.CommercialFishingPermitLicensesRegisters
                                join appl in Db.Applications on permit.ApplicationId equals appl.Id
                                join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                where permit.ApplicationId == applicationId
                                      && permit.RecordType == nameof(RecordTypesEnum.Application)
                                select new CommercialFishingApplicationDataIds
                                {
                                    PermitId = permit.Id,
                                    ApplicationId = permit.ApplicationId,
                                    PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                    SubmittedByPersonId = appl.SubmittedByPersonId.Value,
                                    SubmittedByPersonRoleId = appl.SubmittedByPersonRoleId.Value,
                                    SubmittedForPersonId = appl.SubmittedForPersonId.HasValue ? appl.SubmittedForPersonId.Value : default,
                                    SubmittedForLegalId = appl.SubmittedForLegalId.HasValue ? appl.SubmittedForLegalId.Value : default
                                }).Single();
                    }
                    break;
            }

            return data;
        }

        private RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> MapCommonRegixDataFieldsToDTO(int applicationId)
        {
            RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> regixData = new RegixChecksWrapperDTO<CommercialFishingRegixDataDTO>()
            {
                DialogDataModel = GetApplicationRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetCommercialFishingChecks(applicationId)
            };

            regixData.DialogDataModel.ApplicationRegiXChecks = regixData.RegiXDataModel.ApplicationRegiXChecks;

            return regixData;
        }

        private int GetTypeIdByPageCode(PageCodeEnum pageCode)
        {
            return pageCode switch
            {
                PageCodeEnum.CommFish => GetPermitTypeIdByCode(nameof(CommercialFishingTypesEnum.Permit)),
                PageCodeEnum.PoundnetCommFish => GetPermitTypeIdByCode(nameof(CommercialFishingTypesEnum.PoundNetPermit)),
                PageCodeEnum.RightToFishThirdCountry => GetPermitTypeIdByCode(nameof(CommercialFishingTypesEnum.ThirdCountryPermit)),
                PageCodeEnum.RightToFishResource => GetPermitLicenseTypeIdByCode(nameof(CommercialFishingTypesEnum.PermitLicense)),
                PageCodeEnum.PoundnetCommFishLic => GetPermitLicenseTypeIdByCode(nameof(CommercialFishingTypesEnum.PoundNetPermitLicense)),
                PageCodeEnum.CatchQuataSpecies => GetPermitLicenseTypeIdByCode(nameof(CommercialFishingTypesEnum.QuataSpeciesPermitLicense)),
                _ => throw new ArgumentException($"Unexpected application page code {pageCode}")
            };
        }

        private int GetPermitTypeIdByCode(string code)
        {
            DateTime now = DateTime.Now;

            int typeId = (from type in Db.NcommercialFishingPermitTypes
                          where type.Code == code && type.ValidFrom <= now && type.ValidTo > now
                          select type.Id).Single();

            return typeId;
        }

        private int GetPermitLicenseTypeIdByCode(string code)
        {
            DateTime now = DateTime.Now;

            int typeId = (from type in Db.NcommercialFishingPermitLicenseTypes
                          where type.Code == code && type.ValidFrom <= now && type.ValidTo > now
                          select type.Id).Single();

            return typeId;
        }

        private ApplicationPayment GetApplicationPayment(int applicationId)
        {
            ApplicationPayment applicationPayment = (from applPayment in Db.ApplicationPayments
                                                     where applPayment.ApplicationId == applicationId
                                                           && applPayment.IsActive
                                                     select applPayment).First();

            return applicationPayment;
        }

        private List<ApplicationPaymentTariff> GetApplicationPaymenTariffs(int applicationId)
        {
            List<ApplicationPaymentTariff> paymentTariffs = (from applPayment in Db.ApplicationPayments
                                                             join applPaymentTariff in Db.ApplicationPaymentTariffs on applPayment.Id equals applPaymentTariff.PaymentId
                                                             where applPayment.ApplicationId == applicationId
                                                                   && applPayment.IsActive
                                                                   && applPaymentTariff.IsActive
                                                             select applPaymentTariff).ToList();

            return paymentTariffs;
        }

        private string GeneratePermitRegisterNumber(int territoryUnitId, int permitTypeId, int waterTypeId)
        {
            NterritoryUnit territory = GetTerritoryUnitEntity(territoryUnitId);

            territory.PermitRegisterSequence++;

            string waterTypeCode = (from mapWaterTypePermit in Db.MapWaterTypePermitTypes
                                    where mapWaterTypePermit.WaterTypeId == waterTypeId
                                          && mapWaterTypePermit.PermitTypeId == permitTypeId
                                    select mapWaterTypePermit.Code).Single();

            return $"{territory.Code}{waterTypeCode}{territory.PermitRegisterSequence.ToString("D5")}";
        }

        private string GeneratePermitLicenseRegisterNumber(int territoryUnitId, int permitId, bool isQuotaLicense = false)
        {
            NterritoryUnit territory = GetTerritoryUnitEntity(territoryUnitId);

            territory.PermitLicenceRegisterSequence++;

            string permitRegisterNumber = (from permit in Db.CommercialFishingPermitRegisters
                                           where permit.Id == permitId
                                           select permit.RegistrationNum).First();

            string registrationNumber = $"{permitRegisterNumber}-{territory.PermitLicenceRegisterSequence.ToString("D3")}";

            if (isQuotaLicense)
            {
                registrationNumber += "K"; // Специално за "Калкан"
            }

            return registrationNumber;
        }

        private NterritoryUnit GetTerritoryUnitEntity(int territoryUnitId)
        {
            NterritoryUnit territoryUnit = (from terr in Db.NterritoryUnits
                                            where terr.Id == territoryUnitId
                                            select terr).First();

            return territoryUnit;
        }
    }
}
