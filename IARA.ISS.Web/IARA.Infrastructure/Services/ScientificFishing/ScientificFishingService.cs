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
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Helpers;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Legals;
using IARA.Interfaces.Reports;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;
using TL.EDelivery;

namespace IARA.Infrastructure.Services
{
    public class ScientificFishingService : Service, IScientificFishingService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;
        private readonly IApplicationService applicationService;
        private readonly IUserService userService;
        private readonly ICancellationService cancellationService;
        private readonly IApplicationStateMachine applicationStateMachine;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IJasperReportExecutionService jasperReportsService;
        private readonly IDeliveryService deliveryService;

        public ScientificFishingService(IARADbContext db,
                                        IPersonService personService,
                                        ILegalService legalService,
                                        IApplicationService applicationService,
                                        IUserService userService,
                                        ICancellationService cancellationService,
                                        IApplicationStateMachine applicationStateMachine,
                                        IRegixApplicationInterfaceService regixApplicationService,
                                        IJasperReportExecutionService jasperReportsService,
                                        IDeliveryService deliveryService)
                : base(db)
        {
            this.personService = personService;
            this.legalService = legalService;
            this.applicationService = applicationService;
            this.userService = userService;
            this.cancellationService = cancellationService;
            this.applicationStateMachine = applicationStateMachine;
            this.regixApplicationService = regixApplicationService;
            this.jasperReportsService = jasperReportsService;
            this.deliveryService = deliveryService;
        }

        public IQueryable<ScientificFishingPermitDTO> GetAllPermits(ScientificFishingFilters filters)
        {
            IQueryable<ScientificFishingPermitDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPermits(showInactive);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredPermits(filters.FreeTextSearch, filters.ShowInactiveRecords, territoryUnitId: filters.TerritoryUnitId)
                    : GetParametersFilteredPermits(filters);
            }

            return result;
        }

        public IQueryable<ScientificFishingPermitDTO> GetAllPermits(ScientificFishingPublicFilters filters, int currentUserId)
        {
            List<int> personIds = userService.GetPersonIdsByUserId(currentUserId);

            IQueryable<ScientificFishingPermitDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPermits(showInactive, personIds);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredPermits(filters, personIds)
                    : GetFreeTextFilteredPermits(filters.FreeTextSearch, filters.ShowInactiveRecords, personIds);
            }
            return result;
        }

        public void SetPermitHoldersForTable(List<ScientificFishingPermitDTO> permits)
        {
            List<int> permitIds = permits.Select(x => x.Id).ToList();

            var holders = (from permitOwner in Db.ScientificPermitOwners
                           join person in Db.Persons on permitOwner.OwnerId equals person.Id
                           where permitIds.Contains(permitOwner.ScientificPermitId)
                              && permitOwner.IsActive
                           orderby permitOwner.Id
                           select new
                           {
                               RequestNumber = permitOwner.ScientificPermitId,
                               Data = new ScientificFishingPermitHolderDTO
                               {
                                   PermitNumber = permitOwner.Id,
                                   Name = $"{person.FirstName} {person.LastName}",
                                   ScientificPosition = permitOwner.RequestedByOrganizationPosition,
                                   IsActive = permitOwner.IsActive
                               }
                           }).ToLookup(x => x.RequestNumber, y => y.Data);

            foreach (ScientificFishingPermitDTO permit in permits)
            {
                permit.Holders = holders[permit.Id].ToList();
            }
        }

        public ScientificFishingPermitEditDTO GetPermit(int id, int? userId)
        {
            ScientificPermitRegister dbPermit = (from permit in Db.ScientificPermitRegisters
                                                 where permit.Id == id
                                                 select permit).First();

            ScientificFishingPermitEditDTO result;

            if (userId.HasValue)
            {
                result = IsUserPermitSubmittedBy(userId.Value, id)
                    ? MapDbPermitToDTO(dbPermit, isApplication: false)
                    : MapDbPermitToDTO(dbPermit, isApplication: false, userId);
            }
            else
            {
                result = MapDbPermitToDTO(dbPermit, isApplication: false);
            }

            return result;
        }

        public int AddPermit(ScientificFishingPermitEditDTO permit)
        {
            using TransactionScope scope = new TransactionScope();

            var data = (from p in Db.ScientificPermitRegisters
                        where p.ApplicationId == permit.ApplicationId
                           && p.RecordType == nameof(RecordTypesEnum.Application)
                        select new
                        {
                            p.Id,
                            p.ApplicationId,
                            RequesterPosition = p.SubmittedByPersonPosition
                        }).First();

            ScientificPermitRegister entry = new ScientificPermitRegister
            {
                ApplicationId = permit.ApplicationId.Value,
                RegisterApplicationId = data.Id,
                RecordType = nameof(RecordTypesEnum.Register),
                SubmittedByPersonPosition = data.RequesterPosition,
                PermitRegistrationDateTime = permit.RegistrationDate.Value,
                PermitStatusId = CalculateNewScientificPermitStatus(permit),
                PermitValidFrom = permit.ValidFrom.Value,
                PermitValidTo = permit.ValidTo.Value,
                ResearchPeriodFrom = permit.ResearchPeriodFrom.Value,
                ResearchPeriodTo = permit.ResearchPeriodTo.Value,
                ResearchWaterAreas = permit.ResearchWaterArea,
                ResearchGoalsDesc = permit.ResearchGoalsDescription,
                FishTypesDesc = permit.FishTypesDescription,
                FishTypesApp4Zbrdesc = permit.FishTypesApp4ZBRDesc,
                FishTypesCrayFish = permit.FishTypesCrayFish,
                FishingGearDescr = permit.FishingGearDescription,
                IsShipRegistered = permit.IsShipRegistered.Value
            };

            if (entry.IsShipRegistered)
            {
                entry.ShipId = permit.ShipId;
            }
            else
            {
                entry.ShipName = permit.ShipName;
                entry.ShipExternalMark = permit.ShipExternalMark;
                entry.ShipCaptainName = permit.ShipCaptainName;
            }

            // Add submitted for legal
            entry.SubmittedForLegal = Db.AddOrEditLegal(
                new ApplicationRegisterDataDTO
                {
                    RecordType = RecordTypesEnum.Register,
                    ApplicationId = permit.ApplicationId
                },
                permit.Receiver
            );

            Db.SaveChanges();

            // Add permit reasons
            AddPermitReasons(entry, permit.PermitReasonsIds.Concat(permit.PermitLegalReasonsIds).ToList());

            // Add permit holders
            AddPermitHolders(entry, permit.Holders);

            // Add files
            if (permit.Files != null)
            {
                foreach (FileInfoDTO file in permit.Files)
                {
                    Db.AddOrEditFile(entry, entry.ScientificPermitRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            applicationStateMachine.Act(entry.ApplicationId);

            scope.Complete();

            return entry.Id;
        }

        public void EditPermit(ScientificFishingPermitEditDTO permit)
        {
            using TransactionScope scope = new TransactionScope();

            ScientificPermitRegister dbPermit = (from scientificPermit in Db.ScientificPermitRegisters
                                                    .AsSplitQuery()
                                                    .Include(x => x.ScientificPermitRegisterFiles)
                                                 where scientificPermit.Id == permit.Id
                                                 select scientificPermit).First();

            dbPermit.PermitValidFrom = permit.ValidFrom.Value;
            dbPermit.PermitValidTo = permit.ValidTo.Value;
            dbPermit.ResearchPeriodFrom = permit.ResearchPeriodFrom.Value;
            dbPermit.ResearchPeriodTo = permit.ResearchPeriodTo.Value;
            dbPermit.ResearchWaterAreas = permit.ResearchWaterArea;
            dbPermit.ResearchGoalsDesc = permit.ResearchGoalsDescription;
            dbPermit.FishTypesDesc = permit.FishTypesDescription;
            dbPermit.FishTypesApp4Zbrdesc = permit.FishTypesApp4ZBRDesc;
            dbPermit.FishTypesCrayFish = permit.FishTypesCrayFish;
            dbPermit.FishingGearDescr = permit.FishingGearDescription;
            dbPermit.IsShipRegistered = permit.IsShipRegistered.Value;

            if (permit.IsShipRegistered.Value)
            {
                dbPermit.ShipId = permit.ShipId;
                dbPermit.ShipName = null;
                dbPermit.ShipExternalMark = null;
                dbPermit.ShipCaptainName = null;
            }
            else
            {
                dbPermit.ShipId = null;
                dbPermit.ShipName = permit.ShipName;
                dbPermit.ShipExternalMark = permit.ShipExternalMark;
                dbPermit.ShipCaptainName = permit.ShipCaptainName;
            }

            dbPermit.CoordinationCommittee = permit.CoordinationCommittee;
            dbPermit.CoordinationLetterNo = permit.CoordinationLetterNo;
            dbPermit.CoordinationDate = permit.CoordinationDate;
            dbPermit.CoordinationComments = permit.CoordinationComments;

            Db.AddOrEditCancellationDetails(dbPermit, permit.CancellationDetails);

            dbPermit.PermitStatusId = CalculateNewScientificPermitStatus(permit);

            // Edit submitted by legal
            dbPermit.SubmittedForLegal = Db.AddOrEditLegal(
                new ApplicationRegisterDataDTO
                {
                    RecordType = RecordTypesEnum.Register,
                    ApplicationId = permit.ApplicationId
                },
                permit.Receiver,
                null,
                dbPermit.SubmittedForLegalId
            );

            Db.SaveChanges();

            // Edit permit reasons
            EditPermitReasons(dbPermit.Id, permit.PermitReasonsIds.Concat(permit.PermitLegalReasonsIds).ToList());

            // Edit permit holders
            EditPermitHolders(dbPermit.Id, permit.Holders);

            // Edit permit outings
            EditPermitOutings(dbPermit.Id, permit.Outings);

            // add and remove permit files
            if (permit.Files != null)
            {
                foreach (FileInfoDTO file in permit.Files)
                {
                    Db.AddOrEditFile(dbPermit, dbPermit.ScientificPermitRegisterFiles, file);
                }
            }

            Db.SaveChanges();

            scope.Complete();
        }

        public void DeletePermit(int id)
        {
            DeleteRecordWithId(Db.ScientificPermitRegisters, id);
            Db.SaveChanges();
        }

        public void UndoDeletePermit(int id)
        {
            UndoDeleteRecordWithId(Db.ScientificPermitRegisters, id);
            Db.SaveChanges();
        }

        public async Task<DownloadableFileDTO> GetRegisterFileForDownload(int registerId, SciFiPrintTypesEnum printType)
        {
            DownloadableFileDTO downloadableFile = new DownloadableFileDTO
            {
                MimeType = "application/pdf"
            };

            string submittedForName = (from sciFi in Db.ScientificPermitRegisters
                                       join legal in Db.Legals on sciFi.SubmittedForLegalId equals legal.Id
                                       where sciFi.Id == registerId
                                       select $"{legal.Name}-{legal.Eik}").First();

            switch (printType)
            {
                case SciFiPrintTypesEnum.Register:
                    downloadableFile.FileName = $"Разрешително-за-научен-рибилов_{submittedForName}.pdf".Replace("  ", "");
                    downloadableFile.Bytes = await jasperReportsService.GetScientificFishingPermitRegister(registerId);
                    break;
                case SciFiPrintTypesEnum.RegisterProject:
                    downloadableFile.FileName = $"Проект-разрешително-за-научен-рибилов_{submittedForName}.pdf".Replace("  ", "");
                    downloadableFile.Bytes = await jasperReportsService.GetScientificFishingPermitProject(registerId);
                    break;
                case SciFiPrintTypesEnum.Gov:
                    downloadableFile.FileName = $"МЗХГ-разрешително-за-научен-рибилов_{submittedForName}.pdf".Replace("  ", "");
                    downloadableFile.Bytes = await jasperReportsService.GetScientificFishingPermitGovRegister(registerId);
                    break;
                case SciFiPrintTypesEnum.GovProject:
                    downloadableFile.FileName = $"Проект-МЗХГ-разрешително-за-научен-рибилов_{submittedForName}.pdf".Replace("  ", "");
                    downloadableFile.Bytes = await jasperReportsService.GetScientificFishingPermitGovProject(registerId);
                    break;
            }

            return downloadableFile;
        }

        public ApplicationSubmittedByDTO GetUserAsSubmittedBy(int userId)
        {
            int personId = (from user in Db.Users
                            where user.Id == userId
                            select user.PersonId).First();

            RegixPersonDataDTO person = personService.GetRegixPersonData(personId);

            ApplicationSubmittedByDTO result = new ApplicationSubmittedByDTO
            {
                Person = person,
                Addresses = new List<AddressRegistrationDTO>()
            };

            return result;
        }

        public string GetPermitHolderPhoto(int holderId)
        {
            int personId = (from holder in Db.ScientificPermitOwners
                            where holder.Id == holderId
                            select holder.OwnerId).First();

            return personService.GetPersonPhoto(personId);
        }

        public ScientificFishingApplicationEditDTO GetPermitApplication(int applicationId)
        {
            ScientificFishingApplicationEditDTO result = null;

            ScientificPermitRegister dbPermit = (from permit in Db.ScientificPermitRegisters
                                                 where permit.ApplicationId == applicationId
                                                        && permit.RecordType == nameof(RecordTypesEnum.Application)
                                                 select permit).FirstOrDefault();

            if (dbPermit == null)
            {
                string draftContent = (from appl in Db.Applications
                                       where appl.Id == applicationId
                                       select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draftContent))
                {
                    result = CommonUtils.Deserialize<ScientificFishingApplicationEditDTO>(draftContent);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
                }
                else
                {
                    result = new ScientificFishingApplicationEditDTO
                    {
                        HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };
                }
            }
            else
            {
                result = new ScientificFishingApplicationEditDTO
                {
                    Id = dbPermit.Id,
                    ApplicationId = dbPermit.ApplicationId,
                    RegistrationDate = dbPermit.PermitRegistrationDateTime,
                    RequesterPosition = dbPermit.SubmittedByPersonPosition,
                    ValidFrom = dbPermit.PermitValidFrom,
                    ValidTo = dbPermit.PermitValidTo,
                    ResearchPeriodFrom = dbPermit.ResearchPeriodFrom,
                    ResearchPeriodTo = dbPermit.ResearchPeriodTo,
                    ResearchWaterArea = dbPermit.ResearchWaterAreas,
                    ResearchGoalsDescription = dbPermit.ResearchGoalsDesc,
                    FishTypesDescription = dbPermit.FishTypesDesc,
                    FishTypesApp4ZBRDesc = dbPermit.FishTypesApp4Zbrdesc,
                    FishTypesCrayFish = dbPermit.FishTypesCrayFish,
                    FishingGearDescription = dbPermit.FishingGearDescr,
                    IsShipRegistered = dbPermit.IsShipRegistered,
                    ShipId = dbPermit.ShipId,
                    ShipName = dbPermit.ShipName,
                    ShipExternalMark = dbPermit.ShipExternalMark,
                    ShipCaptainName = dbPermit.ShipCaptainName
                };

                int submittedByPersonId = (from appl in Db.Applications
                                           where appl.Id == result.ApplicationId
                                           select appl.SubmittedByPersonId.Value).First();

                result.Requester = personService.GetRegixPersonData(submittedByPersonId);
                result.RequesterLetterOfAttorney = applicationService.GetLetterOfAttorney(result.ApplicationId.Value);
                result.Receiver = legalService.GetRegixLegalData(dbPermit.SubmittedForLegalId);

                result.PermitReasonsIds = GetPermitReasons(result.Id.Value);
                result.Holders = GetPermitHolders(result.Id.Value);

                result.Files = Db.GetFiles(Db.ScientificPermitRegisterFiles, result.Id.Value);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                if (result.HasDelivery.Value)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO> GetPermitRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO> result = new RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO>
            {
                DialogDataModel = GetApplicationRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetScientificFishingPermitChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public ScientificFishingPermitRegixDataDTO GetApplicationRegixData(int applicationId)
        {
            ScientificFishingApplicationDataIds regixDataIds = GetApplicationDataIds(applicationId);

            RegixPersonDataDTO requester = personService.GetRegixPersonData(regixDataIds.SubmittedByPersonId);
            RegixLegalDataDTO receiver = legalService.GetRegixLegalData(regixDataIds.SubmittedForLegalId);
            List<ScientificFishingPermitHolderRegixDataDTO> holders = GetHoldersRegix(regixDataIds.PermitId);

            ScientificFishingPermitRegixDataDTO result = new ScientificFishingPermitRegixDataDTO
            {
                Id = regixDataIds.PermitId,
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                Requester = requester,
                Receiver = receiver,
                Holders = holders
            };

            return result;
        }

        public ScientificFishingPermitEditDTO GetApplicationDataForRegister(int applicationId)
        {
            ScientificPermitRegister dbPermit = (from permit in Db.ScientificPermitRegisters
                                                 where permit.ApplicationId == applicationId
                                                       && permit.RecordType == nameof(RecordTypesEnum.Application)
                                                       && permit.IsActive
                                                 select permit).First();

            ScientificFishingPermitEditDTO result = MapDbPermitToDTO(dbPermit, isApplication: true);
            return result;
        }

        public ScientificFishingPermitEditDTO GetRegisterByApplicationId(int applicationId)
        {
            int id = (from permit in Db.ScientificPermitRegisters
                      where permit.ApplicationId == applicationId
                            && permit.RecordType == nameof(RecordTypesEnum.Register)
                      select permit.Id).First();

            return GetPermit(id, null);
        }

        public int AddPermitApplication(ScientificFishingApplicationEditDTO permit, ApplicationStatusesEnum? nextManualStatus)
        {
            ScientificPermitRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                entry = new ScientificPermitRegister
                {
                    ApplicationId = permit.ApplicationId.Value,
                    RecordType = nameof(RecordTypesEnum.Application),
                    PermitRegistrationDateTime = permit.RegistrationDate.Value,
                    PermitStatusId = GetPermitStatusId(ScientificPermitStatusEnum.Application),
                    PermitValidFrom = permit.ValidFrom.Value,
                    PermitValidTo = permit.ValidTo.Value,
                    SubmittedByPersonPosition = permit.RequesterPosition,
                    ResearchPeriodFrom = permit.ResearchPeriodFrom.Value,
                    ResearchPeriodTo = permit.ResearchPeriodTo.Value,
                    ResearchWaterAreas = permit.ResearchWaterArea,
                    ResearchGoalsDesc = permit.ResearchGoalsDescription,
                    FishTypesDesc = permit.FishTypesDescription,
                    FishTypesApp4Zbrdesc = permit.FishTypesApp4ZBRDesc,
                    FishTypesCrayFish = permit.FishTypesCrayFish,
                    FishingGearDescr = permit.FishingGearDescription,
                    IsShipRegistered = permit.IsShipRegistered.Value
                };

                if (entry.IsShipRegistered)
                {
                    entry.ShipId = permit.ShipId;
                }
                else
                {
                    entry.ShipName = permit.ShipName;
                    entry.ShipExternalMark = permit.ShipExternalMark;
                    entry.ShipCaptainName = permit.ShipCaptainName;
                }

                Application application = (from appl in Db.Applications
                                           where appl.Id == entry.ApplicationId
                                           select appl).First();

                application.SubmittedByPerson = Db.AddOrEditPerson(permit.Requester);

                Db.SaveChanges();

                Legal submittedForLegal = Db.AddOrEditLegal(
                    new ApplicationRegisterDataDTO
                    {
                        RecordType = RecordTypesEnum.Application,
                        ApplicationId = application.Id
                    },
                    permit.Receiver
                );

                Db.SaveChanges();

                entry.SubmittedForLegal = submittedForLegal;
                application.SubmittedForLegal = submittedForLegal;
                application.SubmittedByLetterOfAttorney = Db.AddOrEditLetterOfAttorney(permit.RequesterLetterOfAttorney);

                SetSubmittedByPersonRole(application, permit);

                Db.ScientificPermitRegisters.Add(entry);

                // Add permit reasons
                AddPermitReasons(entry, permit.PermitReasonsIds);

                // Add permit holders
                AddPermitHolders(entry, permit.Holders);

                // Add files
                if (permit.Files != null)
                {
                    foreach (FileInfoDTO file in permit.Files)
                    {
                        Db.AddOrEditFile(entry, entry.ScientificPermitRegisterFiles, file);
                    }
                }

                AddPermitApplicationDeliveryData(application, permit);

                Db.SaveChanges();
                scope.Complete();
            }

            List<FileInfoDTO> permitFiles = permit.Files;
            permit.Files = null;
            applicationStateMachine.Act(entry.ApplicationId, CommonUtils.Serialize(permit), permit.Files, nextManualStatus);

            return entry.Id;
        }

        public void EditPermitApplication(ScientificFishingApplicationEditDTO permit, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                ScientificPermitRegister dbPermit = (from scientificPermit in Db.ScientificPermitRegisters
                                                            .AsSplitQuery()
                                                            .Include(x => x.ScientificPermitRegisterFiles)
                                                     where scientificPermit.Id == permit.Id
                                                     select scientificPermit).First();

                dbPermit.PermitValidFrom = permit.ValidFrom.Value;
                dbPermit.PermitValidTo = permit.ValidTo.Value;
                dbPermit.SubmittedByPersonPosition = permit.RequesterPosition;
                dbPermit.ResearchPeriodFrom = permit.ResearchPeriodFrom.Value;
                dbPermit.ResearchPeriodTo = permit.ResearchPeriodTo.Value;
                dbPermit.ResearchWaterAreas = permit.ResearchWaterArea;
                dbPermit.ResearchGoalsDesc = permit.ResearchGoalsDescription;
                dbPermit.FishTypesDesc = permit.FishTypesDescription;
                dbPermit.FishTypesApp4Zbrdesc = permit.FishTypesApp4ZBRDesc;
                dbPermit.FishTypesCrayFish = permit.FishTypesCrayFish;
                dbPermit.FishingGearDescr = permit.FishingGearDescription;
                dbPermit.IsShipRegistered = permit.IsShipRegistered.Value;

                if (dbPermit.IsShipRegistered)
                {
                    dbPermit.ShipId = permit.ShipId;
                    dbPermit.ShipName = null;
                    dbPermit.ShipExternalMark = null;
                    dbPermit.ShipCaptainName = null;
                }
                else
                {
                    dbPermit.ShipId = null;
                    dbPermit.ShipName = permit.ShipName;
                    dbPermit.ShipExternalMark = permit.ShipExternalMark;
                    dbPermit.ShipCaptainName = permit.ShipCaptainName;
                }

                application = (from appl in Db.Applications
                               where appl.Id == dbPermit.ApplicationId
                               select appl).First();

                EditApplicationRegixFields(application, dbPermit, new ScientificFishingPermitRegixDataDTO
                {
                    Id = permit.Id,
                    ApplicationId = permit.ApplicationId,
                    Receiver = permit.Receiver,
                    Requester = permit.Requester
                });

                application.SubmittedByLetterOfAttorney = Db.AddOrEditLetterOfAttorney(permit.RequesterLetterOfAttorney, application.SubmittedByLetterOfAttorneyId);

                SetSubmittedByPersonRole(application, permit);

                // Edit permit reasons
                EditPermitReasons(dbPermit.Id, permit.PermitReasonsIds);

                // Edit permit holders
                EditPermitHolders(dbPermit.Id, permit.Holders);

                // Edit files
                if (permit.Files != null)
                {
                    foreach (FileInfoDTO file in permit.Files)
                    {
                        Db.AddOrEditFile(dbPermit, dbPermit.ScientificPermitRegisterFiles, file);
                    }
                }

                EditPermitApplicationDeliveryData(application, permit);

                Db.SaveChanges();
                scope.Complete();
            }

            List<FileInfoDTO> permitFiles = permit.Files;
            permit.Files = null;
            applicationStateMachine.Act(application.Id, CommonUtils.Serialize(permit), permitFiles, manualStatus, permit.StatusReason);
        }

        public void EditPermitApplicationRegixData(ScientificFishingPermitRegixDataDTO permit)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                ScientificPermitRegister dbPermit = GetPermitById(permit.Id.Value);

                application = (from appl in Db.Applications
                               where appl.Id == dbPermit.ApplicationId
                               select appl).First();

                EditApplicationRegixFields(application, dbPermit, permit);

                // Edit permit holders regix data
                foreach (ScientificFishingPermitHolderRegixDataDTO holder in permit.Holders)
                {
                    Db.AddOrEditPerson(holder.RegixPersonData, holder.AddressRegistrations, holder.OwnerId);
                    Db.SaveChanges();
                }

                scope.Complete();
            }

            applicationStateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public int AddOuting(ScientificFishingOutingDTO outing)
        {
            ScientificPermitRegister permit = GetPermitById(outing.PermitId.Value);
            int outingId = AddPermitOuting(permit, outing);

            Db.SaveChanges();
            return outingId;
        }

        public SimpleAuditDTO GetPermitHolderSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.ScientificPermitOwners, id);
            return audit;
        }

        public SimpleAuditDTO GetPermitOutingSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.ScientificPermitOutings, id);
            return audit;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.ScientificPermitRegisters, id);
            return audit;
        }

        public bool HasUserAccessToPermits(int userId, List<int> permitIds)
        {
            // проверяваме дали потребителят е титуляр на поне едно от съответните разрешителни
            ILookup<int, int> permitOwnerIds = GetPermitHoldersLookup(permitIds);

            if (permitOwnerIds.Count > 0)
            {
                List<int> personIds = userService.GetPersonIdsByUserId(userId);

                foreach (IGrouping<int, int> permit in permitOwnerIds)
                {
                    List<int> ownerIds = permit.ToList();
                    if (ownerIds.Any(x => personIds.Contains(x)))
                    {
                        return true;
                    }
                }
            }

            // проверяваме дали потребителят е заявител на съответните разрешителни
            Dictionary<int, int> permitRequesterIds = GetPermitRequestersLookup(permitIds);

            if (permitRequesterIds.Count > 0)
            {
                List<int> personIds = userService.GetPersonIdsByUserId(userId);

                foreach (KeyValuePair<int, int> permit in permitRequesterIds)
                {
                    if (personIds.Contains(permit.Value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasUserAccessToPermitHolder(int userId, int holderId)
        {
            List<int> personIds = userService.GetPersonIdsByUserId(userId);
            List<int> userPermitIds = GetPersonPermitIds(personIds);

            // проверяваме дали потребителят е титуляр
            ILookup<int, int> permitOwnerIds = GetPermitHoldersLookup(userPermitIds);
            foreach (IGrouping<int, int> permit in permitOwnerIds)
            {
                List<int> ownerIds = permit.ToList();
                if (ownerIds.Contains(holderId))
                {
                    return true;
                }
            }

            // проверяваме дали потребителят е заявител на съответните разрешителни
            Dictionary<int, int> permitRequesterIds = GetPermitRequestersLookup(userPermitIds);
            foreach (KeyValuePair<int, int> permit in permitRequesterIds)
            {
                if (personIds.Contains(permit.Value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasUserAccessToPermitOuting(int userId, int outingId)
        {
            int permitId = (from po in Db.ScientificPermitOutings
                            where po.Id == outingId
                            select po.ScientificPermitId).First();

            List<int> personIds = userService.GetPersonIdsByUserId(userId);
            List<int> permitIds = GetPersonPermitIds(personIds);

            return permitIds.Contains(permitId);
        }

        public bool HasUserAccessToPermitFile(int userId, int fileId)
        {
            List<int> filePermitIds = (from pf in Db.ScientificPermitRegisterFiles
                                       where pf.FileId == fileId
                                       select pf.RecordId).ToList();

            // file is in draft
            if (filePermitIds.Count == 0)
            {
                List<int> applicationIds = (from af in Db.ApplicationFiles
                                            where af.FileId == fileId
                                            select af.RecordId).ToList();

                return applicationService.AreApplicationsSubmittedByUser(userId, applicationIds);
            }

            List<int> personIds = userService.GetPersonIdsByUserId(userId);
            List<int> userPermitIds = GetPersonPermitIds(personIds);

            return filePermitIds.All(x => userPermitIds.Contains(x));
        }

        public async Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            if (pageCode == PageCodeEnum.SciFi)
            {
                var data = (from permit in Db.ScientificPermitRegisters
                            join appl in Db.Applications on permit.ApplicationId equals appl.Id
                            join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                            join legal in Db.Legals on permit.SubmittedForLegalId equals legal.Id
                            join user in Db.Users on permit.CreatedBy equals user.Username
                            join person in Db.Persons on user.PersonId equals person.Id
                            where permit.ApplicationId == applicationId
                                && permit.RecordType == nameof(RecordTypesEnum.Register)
                            select new
                            {
                                permit.Id,
                                ApplicationType = applType.Name,
                                ApplicationTypeCode = applType.Code,
                                SubmittedForLegalID = legal.Id,
                                CreatedByPersonEGN = person.EgnLnc
                            }).First();

                RegixLegalDataDTO submittedFor = legalService.GetRegixLegalData(data.SubmittedForLegalID);

                DownloadableFileDTO pdf = await GetRegisterFileForDownload(data.Id, SciFiPrintTypesEnum.Register);

                ApplicationEDeliveryInfo info = new ApplicationEDeliveryInfo
                {
                    Subject = data.ApplicationType,
                    DocBytes = pdf.Bytes,
                    DocNameWithExtension = pdf.FileName,
                    DocRegNumber = applicationId.ToString(),
                    ReceiverType = eProfileType.LegalPerson,
                    ReceiverUniqueIdentifier = submittedFor.EIK,
                    ReceiverPhone = submittedFor.Phone,
                    ReceiverEmail = submittedFor.Email,
                    ServiceOID = data.ApplicationTypeCode,
                    OperatorEGN = data.CreatedByPersonEGN
                };

                return info;
            }

            throw new ArgumentException("Nothing to deliver for page code: " + pageCode.ToString());
        }

        private IQueryable<ScientificFishingPermitDTO> GetAllPermits(bool showInactive, List<int> personIds = null)
        {
            var query = from permit in Db.ScientificPermitRegisters
                        join application in Db.Applications on permit.ApplicationId equals application.Id
                        join receiver in Db.Legals on application.SubmittedForLegalId equals receiver.Id
                        where permit.RecordType == nameof(RecordTypesEnum.Register)
                            && permit.IsActive == !showInactive
                        select new
                        {
                            permit.Id,
                            permit.ApplicationId,
                            permit.PermitStatus,
                            application.SubmittedByPerson,
                            application.SubmittedForLegal,
                            permit.ScientificPermitReasons,
                            permit.ScientificPermitOutings,
                            permit.CoordinationDate,
                            permit.IsActive,
                            permit.PermitRegistrationDateTime,
                            permit.PermitValidTo,
                            application.DeliveryId
                        };

            if (personIds != null)
            {
                List<int> permitIds = (from permit in query
                                       join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                                       where personIds.Contains(permit.SubmittedByPerson.Id) || personIds.Contains(owner.OwnerId)
                                       select permit.Id).ToList();

                query = query.Where(x => permitIds.Contains(x.Id));
            }

            var result = from permit in query
                         orderby permit.PermitRegistrationDateTime descending
                         select new ScientificFishingPermitDTO
                         {
                             Id = permit.Id,
                             ApplicationId = permit.ApplicationId,
                             PermitStatus = Enum.Parse<ScientificPermitStatusEnum>(permit.PermitStatus.Code),
                             PermitStatusName = permit.PermitStatus.Name,
                             RequestNumber = permit.Id,
                             RequesterName = permit.SubmittedByPerson.FirstName + " " + permit.SubmittedByPerson.LastName,
                             ScientificOrganizationName = permit.SubmittedForLegal.Name,
                             PermitReasons = string.Join("; ", permit.ScientificPermitReasons.Where(x => x.IsActive).Select(x => x.Reason).Select(x => x.Name)),
                             ValidTo = permit.PermitValidTo,
                             OutingsCount = permit.ScientificPermitOutings.Count,
                             IsActive = permit.IsActive,
                             DeliveryId = permit.DeliveryId
                         };

            return result;
        }

        private IQueryable<ScientificFishingPermitDTO> GetParametersFilteredPermits(ScientificFishingFilters filters)
        {
            var query = from permit in Db.ScientificPermitRegisters
                        join permitStatus in Db.NpermitStatuses on permit.PermitStatusId equals permitStatus.Id
                        join application in Db.Applications on permit.ApplicationId equals application.Id
                        join receiver in Db.Legals on application.SubmittedForLegalId equals receiver.Id
                        where permit.RecordType == nameof(RecordTypesEnum.Register)
                            && permit.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            permit.ScientificPermitReasons,
                            permit.ScientificPermitOutings,
                            permit.Id,
                            permit.ApplicationId,
                            PermitStatusCode = permitStatus.Code,
                            PermitStatusName = permitStatus.Name,
                            application.SubmittedByPerson,
                            permit.PermitRegistrationDateTime,
                            permit.PermitValidFrom,
                            permit.PermitValidTo,
                            application.SubmittedForLegal,
                            permit.ResearchWaterAreas,
                            permit.FishTypesDesc,
                            permit.FishTypesCrayFish,
                            permit.FishTypesApp4Zbrdesc,
                            permit.FishingGearDescr,
                            permit.IsActive,
                            application.DeliveryId,
                            application.TerritoryUnitId
                        };

            if (filters.TerritoryUnitId.HasValue)
            {
                query = query.Where(x => x.TerritoryUnitId.HasValue && x.TerritoryUnitId.Value == filters.TerritoryUnitId.Value);
            }

            if (!string.IsNullOrEmpty(filters.RequestNumber))
            {
                query = query.Where(x => x.Id.ToString().Contains(filters.RequestNumber));
            }

            if (!string.IsNullOrEmpty(filters.PermitNumber))
            {
                query = from permit in query
                        join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                        where owner.Id.ToString().Contains(filters.PermitNumber)
                        select permit;
            }

            if (filters.CreationDateFrom.HasValue)
            {
                query = query.Where(x => x.PermitRegistrationDateTime >= filters.CreationDateFrom);
            }

            if (filters.CreationDateTo.HasValue)
            {
                query = query.Where(x => x.PermitRegistrationDateTime <= filters.CreationDateTo);
            }

            if (filters.ValidFrom.HasValue)
            {
                query = query.Where(x => x.PermitValidFrom >= filters.ValidFrom);
            }

            if (filters.ValidTo.HasValue)
            {
                query = query.Where(x => x.PermitValidTo <= filters.ValidTo);
            }

            if (filters.PermitReasonIds != null && filters.PermitReasonIds.Count != 0)
            {
                HashSet<int> permitIds = (from permitReason in Db.ScientificPermitReasons
                                          where filters.PermitReasonIds.Contains(permitReason.ReasonId)
                                          select permitReason.ScientificPermitId).ToHashSet();

                query = query.Where(x => permitIds.Contains(x.Id));
            }

            if (filters.PermitLegalReasonIds != null && filters.PermitLegalReasonIds.Count != 0)
            {
                HashSet<int> permitIds = (from permitReason in Db.ScientificPermitReasons
                                          where filters.PermitLegalReasonIds.Contains(permitReason.ReasonId)
                                          select permitReason.ScientificPermitId).ToHashSet();

                query = query.Where(x => permitIds.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(filters.PermitRequesterName))
            {
                query = query.Where(x => (x.SubmittedByPerson.FirstName + " " + x.SubmittedByPerson.LastName).ToLower().Contains(filters.PermitRequesterName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.PermitOwnerName))
            {
                HashSet<int> permitIds = (from permit in query
                                          join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                                          join person in Db.Persons on owner.OwnerId equals person.Id
                                          where (person.FirstName + " " + person.LastName).ToLower().Contains(filters.PermitOwnerName.ToLower())
                                          select permit.Id).ToHashSet();

                query = query.Where(x => permitIds.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(filters.PermitOwnerEgn))
            {
                HashSet<int> permitIds = (from permit in query
                                          join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                                          join person in Db.Persons on owner.OwnerId equals person.Id
                                          where person.EgnLnc == filters.PermitOwnerEgn
                                          select permit.Id).ToHashSet();

                query = query.Where(x => permitIds.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(filters.ScientificOrganizationName))
            {
                query = query.Where(x => x.SubmittedForLegal.Name.ToLower().Contains(filters.ScientificOrganizationName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ResearchWaterArea))
            {
                query = query.Where(x => x.ResearchWaterAreas.ToLower().Contains(filters.ResearchWaterArea.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.AquaticOrganismType))
            {
                filters.AquaticOrganismType = filters.AquaticOrganismType.ToLower();
                query = query.Where(x => x.FishTypesDesc.ToLower().Contains(filters.AquaticOrganismType.ToLower())
                                      || x.FishTypesCrayFish.ToLower().Contains(filters.AquaticOrganismType.ToLower())
                                      || x.FishTypesApp4Zbrdesc.ToLower().Contains(filters.AquaticOrganismType.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.GearType))
            {
                query = query.Where(x => x.FishingGearDescr.ToLower().Contains(filters.GearType.ToLower()));
            }

            if (filters.Statuses != null)
            {
                List<string> statuses = filters.Statuses.Select(x => x.ToString()).ToList();
                query = query.Where(x => statuses.Contains(x.PermitStatusCode));
            }

            if (filters.PersonId.HasValue)
            {
                query = from permit in query
                        join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                        where owner.OwnerId == filters.PersonId
                        select permit;
            }

            if (filters.LegalId.HasValue)
            {
                query = query.Where(x => x.SubmittedForLegal.Id == filters.LegalId);
            }

            var result = from data in query
                         orderby data.Id descending
                         select new ScientificFishingPermitDTO
                         {
                             Id = data.Id,
                             ApplicationId = data.ApplicationId,
                             PermitStatus = Enum.Parse<ScientificPermitStatusEnum>(data.PermitStatusCode),
                             RequesterName = data.SubmittedByPerson.FirstName + " " + data.SubmittedByPerson.LastName,
                             PermitStatusName = data.PermitStatusName,
                             RequestNumber = data.Id,
                             ScientificOrganizationName = data.SubmittedForLegal.Name,
                             PermitReasons = string.Join("; ", data.ScientificPermitReasons.Where(x => x.IsActive).Select(x => x.Reason).Select(x => x.Name)),
                             ValidTo = data.PermitValidTo,
                             OutingsCount = data.ScientificPermitOutings.Count,
                             IsActive = data.IsActive,
                             DeliveryId = data.DeliveryId
                         };

            return result;
        }

        private IQueryable<ScientificFishingPermitDTO> GetParametersFilteredPermits(ScientificFishingPublicFilters filters, List<int> personIds)
        {
            var query = from permit in Db.ScientificPermitRegisters
                        join permitStatus in Db.NpermitStatuses on permit.PermitStatusId equals permitStatus.Id
                        join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                        join application in Db.Applications on permit.ApplicationId equals application.Id
                        where permit.RecordType == nameof(RecordTypesEnum.Register)
                           && permit.IsActive == !filters.ShowInactiveRecords
                           && (string.IsNullOrEmpty(filters.RequestNumber) || permit.Id.ToString().Contains(filters.RequestNumber))
                           && (string.IsNullOrEmpty(filters.PermitNumber) || owner.Id.ToString().Contains(filters.PermitNumber))
                           && (!filters.CreationDateFrom.HasValue || permit.PermitRegistrationDateTime >= filters.CreationDateFrom)
                           && (!filters.CreationDateTo.HasValue || permit.PermitRegistrationDateTime <= filters.CreationDateTo)
                           && (!filters.ValidFrom.HasValue || permit.CoordinationDate >= filters.ValidFrom)
                           && (!filters.ValidTo.HasValue || permit.PermitValidTo <= filters.ValidTo)
                        select new
                        {
                            permit.ScientificPermitReasons,
                            permit.ScientificPermitOutings,
                            permit.Id,
                            permit.ApplicationId,
                            PermitStatusCode = permitStatus.Code,
                            PermitStatusName = permitStatus.Name,
                            application.SubmittedByPerson,
                            application.SubmittedForLegal,
                            permit.PermitValidFrom,
                            permit.IsActive,
                            PermitNumber = owner.Id,
                            permit.PermitValidTo,
                            application.DeliveryId
                        };

            if (personIds != null)
            {
                List<int> permitIds = (from permit in query
                                       join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                                       where personIds.Contains(permit.SubmittedByPerson.Id) || personIds.Contains(owner.OwnerId)
                                       select permit.Id).ToList();

                query = query.Where(x => permitIds.Contains(x.Id));
            }

            var result = from data in query
                         orderby data.Id descending
                         select new ScientificFishingPermitDTO
                         {
                             Id = data.Id,
                             ApplicationId = data.ApplicationId,
                             PermitStatus = Enum.Parse<ScientificPermitStatusEnum>(data.PermitStatusCode),
                             PermitStatusName = data.PermitStatusName,
                             RequesterName = data.SubmittedByPerson.FirstName + " " + data.SubmittedByPerson.LastName,
                             RequestNumber = data.Id,
                             ScientificOrganizationName = data.SubmittedForLegal.Name,
                             PermitReasons = string.Join("; ", data.ScientificPermitReasons.Select(x => x.Reason).Select(x => x.Name)),
                             ValidTo = data.PermitValidTo,
                             OutingsCount = data.ScientificPermitOutings.Count,
                             IsActive = data.IsActive,
                             DeliveryId = data.DeliveryId
                         };
            return result;
        }

        private IQueryable<ScientificFishingPermitDTO> GetFreeTextFilteredPermits(string text, bool showInactive, List<int> personIds = null, int? territoryUnitId = null)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            HashSet<int> permitIds = (from owner in Db.ScientificPermitOwners
                                      join person in Db.Persons on owner.OwnerId equals person.Id
                                      where owner.Id.ToString().Contains(text)
                                           || person.FirstName.ToLower().Contains(text)
                                           || person.LastName.ToLower().Contains(text)
                                           || owner.RequestedByOrganizationPosition.ToLower().Contains(text)
                                      select owner.ScientificPermitId).ToHashSet();

            permitIds.UnionWith((from permitReason in Db.ScientificPermitReasons
                                 join reason in Db.NpermitReasons on permitReason.ReasonId equals reason.Id
                                 where reason.Name.ToLower().Contains(text)
                                 select permitReason.ScientificPermitId).ToHashSet());

            var query = from permit in Db.ScientificPermitRegisters
                        join application in Db.Applications on permit.ApplicationId equals application.Id
                        join organization in Db.Legals on application.SubmittedForLegalId equals organization.Id
                        where permit.RecordType == nameof(RecordTypesEnum.Register)
                           && permit.IsActive == !showInactive
                           && (permit.Id.ToString().Contains(text)
                               || (application.SubmittedByPerson.FirstName + " " + application.SubmittedByPerson.LastName).ToLower().Contains(text)
                               || organization.Name.ToLower().Contains(text)
                               || (searchDate.HasValue && permit.PermitValidFrom == searchDate.Value)
                               || permitIds.Contains(permit.Id))
                        select new
                        {
                            permit.Id,
                            permit.ApplicationId,
                            permit.PermitStatus,
                            application.SubmittedByPerson,
                            OrganizationName = organization.Name,
                            permit.ScientificPermitReasons,
                            permit.ScientificPermitOutings,
                            permit.PermitValidFrom,
                            permit.IsActive,
                            permit.PermitValidTo,
                            application.DeliveryId,
                            application.TerritoryUnitId
                        };

            if (personIds != null)
            {
                List<int> ids = (from permit in query
                                 join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                                 where personIds.Contains(permit.SubmittedByPerson.Id) || personIds.Contains(owner.OwnerId)
                                 select permit.Id).ToList();

                query = query.Where(x => ids.Contains(x.Id));
            }

            if (territoryUnitId.HasValue)
            {
                query = from permit in query
                        where permit.TerritoryUnitId == territoryUnitId.Value
                        select permit;
            }

            var result = from permit in query
                         orderby permit.Id descending
                         select new ScientificFishingPermitDTO
                         {
                             Id = permit.Id,
                             ApplicationId = permit.ApplicationId,
                             PermitStatus = Enum.Parse<ScientificPermitStatusEnum>(permit.PermitStatus.Code),
                             PermitStatusName = permit.PermitStatus.Name,
                             RequesterName = permit.SubmittedByPerson.FirstName + " " + permit.SubmittedByPerson.LastName,
                             RequestNumber = permit.Id,
                             ScientificOrganizationName = permit.OrganizationName,
                             PermitReasons = string.Join("; ", permit.ScientificPermitReasons.Where(x => x.IsActive).Select(x => x.Reason).Select(x => x.Name)),
                             ValidTo = permit.PermitValidTo,
                             OutingsCount = permit.ScientificPermitOutings.Count,
                             IsActive = permit.IsActive,
                             DeliveryId = permit.DeliveryId
                         };

            return result;
        }

        private ScientificFishingPermitEditDTO MapDbPermitToDTO(ScientificPermitRegister dbPermit, bool isApplication, int? userId = null)
        {
            ScientificFishingPermitEditDTO result = new ScientificFishingPermitEditDTO
            {
                Id = dbPermit.Id,
                ApplicationId = dbPermit.ApplicationId,
                RegistrationDate = dbPermit.PermitRegistrationDateTime,
                ValidFrom = dbPermit.PermitValidFrom,
                ValidTo = dbPermit.PermitValidTo,
                ResearchPeriodFrom = dbPermit.ResearchPeriodFrom,
                ResearchPeriodTo = dbPermit.ResearchPeriodTo,
                ResearchWaterArea = dbPermit.ResearchWaterAreas,
                ResearchGoalsDescription = dbPermit.ResearchGoalsDesc,
                FishTypesDescription = dbPermit.FishTypesDesc,
                FishTypesApp4ZBRDesc = dbPermit.FishTypesApp4Zbrdesc,
                FishTypesCrayFish = dbPermit.FishTypesCrayFish,
                FishingGearDescription = dbPermit.FishingGearDescr,
                IsShipRegistered = dbPermit.IsShipRegistered,
                ShipId = dbPermit.ShipId,
                ShipName = dbPermit.ShipName,
                ShipExternalMark = dbPermit.ShipExternalMark,
                ShipCaptainName = dbPermit.ShipCaptainName,
                CoordinationCommittee = dbPermit.CoordinationCommittee,
                CoordinationLetterNo = dbPermit.CoordinationLetterNo,
                CoordinationDate = dbPermit.CoordinationDate,
                CoordinationComments = dbPermit.CoordinationComments
            };

            result.PermitStatus = (from status in Db.NpermitStatuses
                                   where status.Id == dbPermit.PermitStatusId
                                   select Enum.Parse<ScientificPermitStatusEnum>(status.Code)).First();

            result.CancellationDetails = cancellationService.GetCancellationDetails(dbPermit.CancellationDetailsId);

            int submittedForLegalId = isApplication
                ? (from appl in Db.Applications
                   where appl.Id == result.ApplicationId
                   select appl.SubmittedForLegalId.Value).First()
                : dbPermit.SubmittedForLegalId;

            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(dbPermit.ApplicationId, ApplicationHierarchyTypesEnum.Online);
            result.Receiver = legalService.GetRegixLegalData(submittedForLegalId);

            result.PermitReasonsIds = GetPermitReasons(dbPermit.Id);
            result.PermitLegalReasonsIds = GetPermitLegalReasons(dbPermit.Id);
            result.Holders = GetPermitHolders(dbPermit.Id, userId);
            result.Outings = GetPermitOutings(dbPermit.Id);

            // не връщаме файлове на потребители в публичното, които не са заявители на разрешителното
            if (!userId.HasValue)
            {
                result.Files = Db.GetFiles(Db.ScientificPermitRegisterFiles, dbPermit.Id);
            }

            return result;
        }

        private ScientificFishingApplicationDataIds GetApplicationDataIds(int applicationId)
        {
            var regixDataIds = (from permit in Db.ScientificPermitRegisters
                                join appl in Db.Applications on permit.ApplicationId equals appl.Id
                                join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                where permit.ApplicationId == applicationId
                                    && permit.RecordType == nameof(RecordTypesEnum.Application)
                                select new ScientificFishingApplicationDataIds
                                {
                                    PermitId = permit.Id,
                                    PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                    SubmittedByPersonId = appl.SubmittedByPersonId.Value,
                                    SubmittedForLegalId = appl.SubmittedForLegalId.Value
                                }).First();

            return regixDataIds;
        }

        private ScientificPermitRegister GetPermitById(int id)
        {
            ScientificPermitRegister dbPermit = (from scientificPermit in Db.ScientificPermitRegisters
                                                 where scientificPermit.Id == id
                                                 select scientificPermit).First();

            return dbPermit;
        }

        private int GetPermitStatusId(ScientificPermitStatusEnum status)
        {
            DateTime now = DateTime.Now;

            int id = (from s in Db.NpermitStatuses
                      where s.Code == status.ToString()
                        && s.ValidFrom <= now
                        && s.ValidTo > now
                      select s.Id).First();

            return id;
        }

        // Permit reasons
        private List<int> GetPermitReasons(int permitId)
        {
            List<int> reasons = GetPermitReasons(permitId, false);
            return reasons;
        }

        private List<int> GetPermitLegalReasons(int permitId)
        {
            List<int> reasons = GetPermitReasons(permitId, true);
            return reasons;
        }

        private List<int> GetPermitReasons(int permitId, bool legal)
        {
            List<int> reasons = (from permitReason in Db.ScientificPermitReasons
                                 join reason in Db.NpermitReasons on permitReason.ReasonId equals reason.Id
                                 where reason.IsLegalReason == legal
                                      && permitReason.ScientificPermitId == permitId
                                      && permitReason.IsActive
                                 select reason.Id).ToList();

            return reasons;
        }

        private void AddPermitReasons(ScientificPermitRegister permit, List<int> reasonIds)
        {
            foreach (int reasonId in reasonIds)
            {
                ScientificPermitReason entry = new ScientificPermitReason
                {
                    ScientificPermit = permit,
                    ReasonId = reasonId
                };

                Db.ScientificPermitReasons.Add(entry);
            }
        }

        private void EditPermitReasons(int permitId, List<int> newReasonIds)
        {
            List<ScientificPermitReason> dbReasons = (from permitReason in Db.ScientificPermitReasons
                                                      where permitReason.ScientificPermitId == permitId
                                                      select permitReason).ToList();

            List<int> currentReasonIDs = dbReasons.Select(x => x.ReasonId).ToList();
            List<int> reasonIdsToAdd = newReasonIds.Where(x => !currentReasonIDs.Contains(x)).ToList();
            List<int> reasonIdsToRemove = currentReasonIDs.Where(x => !newReasonIds.Contains(x)).ToList();

            foreach (int reasonId in reasonIdsToAdd)
            {
                ScientificPermitReason reason = dbReasons.Where(x => x.ReasonId == reasonId).FirstOrDefault();

                if (reason != null)
                {
                    reason.IsActive = true;
                }
                else
                {
                    ScientificPermitReason entry = new ScientificPermitReason
                    {
                        ReasonId = reasonId,
                        ScientificPermitId = permitId
                    };

                    Db.ScientificPermitReasons.Add(entry);
                }
            }

            foreach (ScientificPermitReason reason in dbReasons.Where(x => reasonIdsToRemove.Contains(x.ReasonId)))
            {
                reason.IsActive = false;
            }
        }

        // Permit holders
        private List<ScientificFishingPermitHolderDTO> GetPermitHolders(int permitId, int? userId = null)
        {
            List<int> personIds;

            if (userId.HasValue)
            {
                List<int> userPersonIds = userService.GetPersonIdsByUserId(userId.Value);

                personIds = (from permitOwner in Db.ScientificPermitOwners
                             where permitOwner.ScientificPermitId == permitId
                                && userPersonIds.Contains(permitOwner.OwnerId)
                             select permitOwner.OwnerId).ToList();
            }
            else
            {
                personIds = (from permitOwner in Db.ScientificPermitOwners
                             where permitOwner.ScientificPermitId == permitId
                             select permitOwner.OwnerId).ToList();
            }

            Dictionary<int, RegixPersonDataDTO> regixData = personService.GetRegixPersonsData(personIds);
            ILookup<int, AddressRegistrationDTO> addressData = personService.GetAddressRegistrations(personIds);

            Dictionary<int, FileInfoDTO> photos = (from personFile in Db.PersonFiles
                                                   join photo in Db.Files on personFile.FileId equals photo.Id
                                                   join fileType in Db.NfileTypes on personFile.FileTypeId equals fileType.Id
                                                   where fileType.Code == nameof(FileTypeEnum.PHOTO)
                                                        && personFile.IsActive
                                                        && photo.IsActive
                                                   select new
                                                   {
                                                       personFile.RecordId,
                                                       Photo = new FileInfoDTO
                                                       {
                                                           Id = photo.Id
                                                       }
                                                   }).ToDictionary(x => x.RecordId, y => y.Photo);

            List<ScientificFishingPermitHolderDTO> result = (from permitOwner in Db.ScientificPermitOwners
                                                             join person in Db.Persons on permitOwner.OwnerId equals person.Id
                                                             where permitOwner.ScientificPermitId == permitId
                                                                && personIds.Contains(person.Id)
                                                             orderby permitOwner.Id
                                                             select new ScientificFishingPermitHolderDTO
                                                             {
                                                                 Id = permitOwner.Id,
                                                                 OwnerId = permitOwner.OwnerId,
                                                                 RequestNumber = permitOwner.ScientificPermitId,
                                                                 PermitNumber = permitOwner.Id,
                                                                 ScientificPosition = permitOwner.RequestedByOrganizationPosition,
                                                                 IsActive = permitOwner.IsActive
                                                             }).ToList();

            foreach (ScientificFishingPermitHolderDTO holder in result)
            {
                holder.RegixPersonData = regixData[holder.OwnerId.Value];
                holder.AddressRegistrations = addressData[holder.OwnerId.Value].ToList();

                if (photos.TryGetValue(holder.OwnerId.Value, out FileInfoDTO photo))
                {
                    holder.Photo = photo;
                }
            }

            return result;
        }

        private List<ScientificFishingPermitHolderRegixDataDTO> GetHoldersRegix(int permitId)
        {
            var permitOwnersData = (from owner in Db.ScientificPermitOwners
                                    where owner.ScientificPermitId == permitId
                                    select new
                                    {
                                        PermitOwnerId = owner.Id,
                                        owner.OwnerId,
                                        owner.IsActive
                                    }).ToList();

            List<int> holderIds = permitOwnersData.Select(x => x.OwnerId).ToList();
            Dictionary<int, RegixPersonDataDTO> regixData = personService.GetRegixPersonsData(holderIds);
            ILookup<int, AddressRegistrationDTO> addressData = personService.GetAddressRegistrations(holderIds);

            List<ScientificFishingPermitHolderRegixDataDTO> holders = new List<ScientificFishingPermitHolderRegixDataDTO>();

            foreach (var permitOwner in permitOwnersData)
            {
                ScientificFishingPermitHolderRegixDataDTO holder = new ScientificFishingPermitHolderRegixDataDTO
                {
                    Id = permitOwner.PermitOwnerId,
                    OwnerId = permitOwner.OwnerId,
                    AddressRegistrations = addressData[permitOwner.OwnerId].ToList(),
                    RegixPersonData = regixData[permitOwner.OwnerId],
                    IsActive = permitOwner.IsActive
                };

                holders.Add(holder);
            }

            return holders;
        }

        private void AddPermitHolders(ScientificPermitRegister permit, List<ScientificFishingPermitHolderDTO> holders)
        {
            foreach (ScientificFishingPermitHolderDTO holder in holders)
            {
                ScientificPermitOwner entry = new ScientificPermitOwner
                {
                    Owner = Db.AddOrEditPerson(holder.RegixPersonData, holder.AddressRegistrations, null, holder.Photo, holder.PhotoBase64),
                    RequestedByOrganizationPosition = holder.ScientificPosition,
                    ScientificPermit = permit
                };

                Db.ScientificPermitOwners.Add(entry);

                Db.SaveChanges();
            }
        }

        private void EditPermitHolders(int permitId, List<ScientificFishingPermitHolderDTO> holders)
        {
            List<ScientificPermitOwner> dbHolders = (from permitOwner in Db.ScientificPermitOwners
                                                     where permitOwner.ScientificPermitId == permitId
                                                     select permitOwner).ToList();

            if (holders != null)
            {
                foreach (ScientificFishingPermitHolderDTO holder in holders)
                {
                    if (holder.Id.HasValue)
                    {
                        ScientificPermitOwner dbHolder = dbHolders.Where(x => x.Id == holder.Id.Value).Single();

                        dbHolder.Owner = Db.AddOrEditPerson(holder.RegixPersonData, holder.AddressRegistrations, dbHolder.OwnerId, holder.Photo);
                        dbHolder.RequestedByOrganizationPosition = holder.ScientificPosition;
                        dbHolder.IsActive = holder.IsActive.Value;
                    }
                    else
                    {
                        ScientificPermitOwner entry = new ScientificPermitOwner
                        {
                            Owner = Db.AddOrEditPerson(holder.RegixPersonData, holder.AddressRegistrations, null, holder.Photo),
                            RequestedByOrganizationPosition = holder.ScientificPosition,
                            ScientificPermitId = permitId
                        };
                    }

                    Db.SaveChanges();
                }
            }
            else
            {
                foreach (ScientificPermitOwner holder in dbHolders)
                {
                    holder.IsActive = false;
                }
            }
        }

        private ILookup<int, int> GetPermitHoldersLookup(List<int> permitIds)
        {
            ILookup<int, int> result = (from permit in Db.ScientificPermitRegisters
                                        join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                                        where permitIds.Contains(permit.Id)
                                        select new
                                        {
                                            PermitId = permit.Id,
                                            owner.OwnerId
                                        }).ToLookup(x => x.PermitId, y => y.OwnerId);

            return result;
        }

        private Dictionary<int, int> GetPermitRequestersLookup(List<int> permitIds)
        {
            Dictionary<int, int> result = (from permit in Db.ScientificPermitRegisters
                                           join application in Db.Applications on permit.ApplicationId equals application.Id
                                           where permitIds.Contains(permit.Id)
                                           select new
                                           {
                                               permit.Id,
                                               SubmittedByPersonId = application.SubmittedByPersonId.Value
                                           }).ToDictionary(x => x.Id, y => y.SubmittedByPersonId);

            return result;
        }

        // Permit outings
        private List<ScientificFishingOutingDTO> GetPermitOutings(int permitId)
        {
            List<ScientificFishingOutingDTO> outings = (from outing in Db.ScientificPermitOutings
                                                        where outing.ScientificPermitId == permitId
                                                        orderby outing.OutingDate descending
                                                        select new ScientificFishingOutingDTO
                                                        {
                                                            Id = outing.Id,
                                                            PermitId = outing.ScientificPermitId,
                                                            DateOfOuting = outing.OutingDate,
                                                            WaterArea = outing.WaterAreaDesc,
                                                            IsActive = outing.IsActive
                                                        }).ToList();

            List<int> outingIds = outings.Select(x => x.Id.Value).ToList();

            ILookup<int, ScientificFishingOutingCatchDTO> catches = (from oCatch in Db.ScientificPermitOutingCatches
                                                                     join fishType in Db.Nfishes on oCatch.FishId equals fishType.Id
                                                                     where outingIds.Contains(oCatch.ScientificPermitOutingId)
                                                                     orderby oCatch.Id
                                                                     select new
                                                                     {
                                                                         OutingID = oCatch.ScientificPermitOutingId,
                                                                         Catch = new ScientificFishingOutingCatchDTO
                                                                         {
                                                                             Id = oCatch.Id,
                                                                             OutingId = oCatch.ScientificPermitOutingId,
                                                                             FishTypeId = fishType.Id,
                                                                             CatchUnder100 = oCatch.CatchUnder100,
                                                                             Catch100To500 = oCatch.Catch100To500,
                                                                             Catch500To1000 = oCatch.Catch500To1000,
                                                                             CatchOver1000 = oCatch.CatchOver1000,
                                                                             TotalKeptCount = oCatch.TotalKeptCount,
                                                                             IsActive = oCatch.IsActive
                                                                         }
                                                                     }).ToLookup(x => x.OutingID, y => y.Catch);

            foreach (ScientificFishingOutingDTO outing in outings)
            {
                outing.Catches = catches[outing.Id.Value].ToList();
            }

            return outings;
        }

        private int AddPermitOuting(ScientificPermitRegister permit, ScientificFishingOutingDTO outing)
        {
            ScientificPermitOuting entry = new ScientificPermitOuting
            {
                ScientificPermit = permit,
                OutingDate = outing.DateOfOuting.Value,
                WaterAreaDesc = outing.WaterArea
            };

            AddPermitOutingCatches(entry, outing.Catches);

            Db.ScientificPermitOutings.Add(entry);
            return entry.Id;
        }

        private void AddPermitOutingCatches(ScientificPermitOuting outing, List<ScientificFishingOutingCatchDTO> catches)
        {
            foreach (ScientificFishingOutingCatchDTO outingCatch in catches)
            {
                ScientificPermitOutingCatch outCatch = new ScientificPermitOutingCatch
                {
                    ScientificPermitOuting = outing,
                    FishId = outingCatch.FishTypeId.Value,
                    CatchUnder100 = outingCatch.CatchUnder100.Value,
                    Catch100To500 = outingCatch.Catch100To500.Value,
                    Catch500To1000 = outingCatch.Catch500To1000.Value,
                    CatchOver1000 = outingCatch.CatchOver1000.Value,
                    TotalKeptCount = outingCatch.TotalKeptCount.Value
                };

                Db.ScientificPermitOutingCatches.Add(outCatch);
            }
        }

        private void EditPermitOutings(int permitId, List<ScientificFishingOutingDTO> outings)
        {
            List<ScientificPermitOuting> dbOutings = (from sOuting in Db.ScientificPermitOutings
                                                        .AsSplitQuery()
                                                        .Include(x => x.ScientificPermitOutingCatches)
                                                      where sOuting.ScientificPermitId == permitId
                                                      select sOuting).ToList();

            if (outings != null)
            {
                foreach (ScientificFishingOutingDTO outing in outings)
                {
                    if (outing.Id.HasValue)
                    {
                        ScientificPermitOuting dbOuting = dbOutings.Where(x => x.Id == outing.Id.Value).Single();

                        dbOuting.OutingDate = outing.DateOfOuting.Value;
                        dbOuting.WaterAreaDesc = outing.WaterArea;
                        dbOuting.IsActive = outing.IsActive.Value;

                        EditPermitOutingCatches(dbOuting, outing.Catches);
                    }
                    else
                    {
                        ScientificPermitOuting entry = new ScientificPermitOuting
                        {
                            ScientificPermitId = permitId,
                            OutingDate = outing.DateOfOuting.Value,
                            WaterAreaDesc = outing.WaterArea
                        };

                        AddPermitOutingCatches(entry, outing.Catches);

                        Db.ScientificPermitOutings.Add(entry);
                    }
                }
            }
            else
            {
                foreach (ScientificPermitOuting sOuting in dbOutings)
                {
                    sOuting.IsActive = false;

                    foreach (ScientificPermitOutingCatch oCatch in sOuting.ScientificPermitOutingCatches)
                    {
                        oCatch.IsActive = false;
                    }
                }
            }
        }

        private void EditPermitOutingCatches(ScientificPermitOuting outing, List<ScientificFishingOutingCatchDTO> catches)
        {
            foreach (ScientificFishingOutingCatchDTO outingCatch in catches)
            {
                if (outingCatch.Id.HasValue)
                {
                    ScientificPermitOutingCatch existing = outing.ScientificPermitOutingCatches.Where(x => x.Id == outingCatch.Id.Value).Single();

                    existing.FishId = outingCatch.FishTypeId.Value;
                    existing.CatchUnder100 = outingCatch.CatchUnder100.Value;
                    existing.Catch100To500 = outingCatch.Catch100To500.Value;
                    existing.Catch500To1000 = outingCatch.Catch500To1000.Value;
                    existing.CatchOver1000 = outingCatch.CatchOver1000.Value;
                    existing.TotalKeptCount = outingCatch.TotalKeptCount.Value;
                    existing.IsActive = outingCatch.IsActive.Value;
                }
                else
                {
                    ScientificPermitOutingCatch entry = new ScientificPermitOutingCatch
                    {
                        ScientificPermitOuting = outing,
                        FishId = outingCatch.FishTypeId.Value,
                        CatchUnder100 = outingCatch.CatchUnder100.Value,
                        Catch100To500 = outingCatch.Catch100To500.Value,
                        Catch500To1000 = outingCatch.Catch500To1000.Value,
                        CatchOver1000 = outingCatch.CatchOver1000.Value,
                        TotalKeptCount = outingCatch.TotalKeptCount.Value
                    };

                    Db.ScientificPermitOutingCatches.Add(entry);
                }
            }
        }

        // Other
        private void SetSubmittedByPersonRole(Application application, ScientificFishingApplicationEditDTO permit)
        {
            DateTime now = DateTime.Now;

            if (permit.RequesterLetterOfAttorney != null)
            {
                application.SubmittedByPersonRoleId = (from role in Db.NsubmittedByRoles
                                                       where role.Code == nameof(SubmittedByRolesEnum.LegalRepresentative)
                                                            && role.ValidFrom <= now
                                                            && role.ValidTo > now
                                                       select role.Id).First();
            }
            else
            {
                application.SubmittedByPersonRoleId = (from role in Db.NsubmittedByRoles
                                                       where role.Code == nameof(SubmittedByRolesEnum.LegalOwner)
                                                            && role.ValidFrom <= now
                                                            && role.ValidTo > now
                                                       select role.Id).First();
            }
        }

        private void EditApplicationRegixFields(Application application,
                                                ScientificPermitRegister dbPermit,
                                                ScientificFishingPermitRegixDataDTO permitRegixData)
        {
            application.SubmittedByPerson = Db.AddOrEditPerson(permitRegixData.Requester, oldPersonId: application.SubmittedByPersonId);

            Db.SaveChanges();

            dbPermit.SubmittedForLegal = Db.AddOrEditLegal(
                new ApplicationRegisterDataDTO
                {
                    RecordType = RecordTypesEnum.Application,
                    ApplicationId = application.Id
                },
                permitRegixData.Receiver,
                null,
                dbPermit.SubmittedForLegalId
            );

            Db.SaveChanges();
        }

        private int CalculateNewScientificPermitStatus(ScientificFishingPermitEditDTO permit)
        {
            if (permit.CancellationDetails != null)
            {
                if (permit.CancellationDetails.IsActive)
                {
                    return GetPermitStatusId(ScientificPermitStatusEnum.Canceled);
                }
                else
                {
                    if (permit.ValidTo <= DateTime.Now)
                    {
                        return GetPermitStatusId(ScientificPermitStatusEnum.Expired);
                    }
                    else
                    {
                        return GetPermitStatusId(ScientificPermitStatusEnum.Approved);
                    }
                }
            }
            else if (permit.ValidTo <= DateTime.Now)
            {
                return GetPermitStatusId(ScientificPermitStatusEnum.Expired);
            }

            return Db.NpermitStatuses.Where(x => x.Code == permit.PermitStatus.ToString()).First().Id;
        }

        // Delivery
        private void AddPermitApplicationDeliveryData(Application application, ScientificFishingApplicationEditDTO permit)
        {
            if (permit.DeliveryData != null)
            {
                application.Delivery = Db.AddDeliveryData(permit.DeliveryData);
            }
        }

        private void EditPermitApplicationDeliveryData(Application application, ScientificFishingApplicationEditDTO permit)
        {
            if (permit.DeliveryData != null)
            {
                if (application.DeliveryId.HasValue)
                {
                    application.Delivery = Db.EditDeliveryData(permit.DeliveryData, application.DeliveryId.Value);
                }
                else
                {
                    application.Delivery = Db.AddDeliveryData(permit.DeliveryData);
                }
            }
            else
            {
                application.DeliveryId = null;
            }
        }

        // Utilities
        private List<int> GetPersonPermitIds(List<int> personIds)
        {
            // взимаме всички разрешителни, за които потребителят е заявител
            List<int> requesterPermits = (from permit in Db.ScientificPermitRegisters
                                          join application in Db.Applications on permit.ApplicationId equals application.Id
                                          where personIds.Contains(application.SubmittedByPersonId.Value)
                                          select permit.Id).ToList();

            // взимаме всички разрешителни, за които потребителят е титуляр
            List<int> ownerPermits = (from permit in Db.ScientificPermitRegisters
                                      join owner in Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                                      where personIds.Contains(owner.Id)
                                      select permit.Id).ToList();

            return requesterPermits.Union(ownerPermits).ToList();
        }

        private bool IsUserPermitSubmittedBy(int userId, int permitId)
        {
            int submittedByPersonId = (from permit in Db.ScientificPermitRegisters
                                       join application in Db.Applications on permit.ApplicationId equals application.Id
                                       where permit.Id == permitId
                                       select application.SubmittedByPersonId.Value).First();

            List<int> personIds = userService.GetPersonIdsByUserId(userId);
            return personIds.Contains(submittedByPersonId);
        }
    }
}
