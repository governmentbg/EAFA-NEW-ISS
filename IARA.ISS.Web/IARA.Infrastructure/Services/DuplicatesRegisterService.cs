using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Reports;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class DuplicatesRegisterService : Service, IDuplicatesRegisterService
    {
        private readonly IApplicationService applicationService;
        private readonly IDeliveryService deliveryService;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        private readonly IApplicationStateMachine stateMachine;
        private readonly IJasperReportExecutionService jasperReportExecutionService;

        private static readonly PageCodeEnum[] BuyerPageCodes = new PageCodeEnum[] {
            PageCodeEnum.DupFirstSaleBuyer,
            PageCodeEnum.DupFirstSaleCenter
        };

        private static readonly PageCodeEnum[] PermitPageCodes = new PageCodeEnum[] {
            PageCodeEnum.DupCommFish,
            PageCodeEnum.DupRightToFishThirdCountry,
            PageCodeEnum.DupPoundnetCommFish
        };

        private static readonly PageCodeEnum[] PermitLicencePageCodes = new PageCodeEnum[] {
            PageCodeEnum.DupRightToFishResource,
            PageCodeEnum.DupPoundnetCommFishLic,
            PageCodeEnum.DupCatchQuataSpecies
        };

        private static readonly PageCodeEnum[] FisherPageCodes = new PageCodeEnum[] {
            PageCodeEnum.CompetencyDup
        };

        public DuplicatesRegisterService(IARADbContext db,
                                         IApplicationService applicationService,
                                         IDeliveryService deliveryService,
                                         IRegixApplicationInterfaceService regixApplicationService,
                                         IApplicationStateMachine stateMachine,
                                         IJasperReportExecutionService jasperReportExecutionService)
            : base(db)
        {
            this.applicationService = applicationService;
            this.deliveryService = deliveryService;
            this.regixApplicationService = regixApplicationService;
            this.stateMachine = stateMachine;
            this.jasperReportExecutionService = jasperReportExecutionService;
        }

        public List<DuplicatesEntryDTO> GetDuplicateEntries(int? buyerId, int? fisherId, int? permitId, int? permitLicenceId)
        {
            var query = from dup in Db.DuplicatesRegister
                        join appl in Db.Applications on dup.ApplicationId equals appl.Id
                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                        join applPayment in Db.ApplicationPayments on appl.Id equals applPayment.ApplicationId into applPay
                        from applPayment in applPay.DefaultIfEmpty()
                        join person in Db.Persons on appl.SubmittedByPersonId equals person.Id
                        where dup.RecordType == nameof(RecordTypesEnum.Register)
                        select new
                        {
                            dup.Id,
                            dup.ApplicationId,
                            dup.FishermanId,
                            dup.BuyerId,
                            dup.PermitId,
                            dup.PermitLicenceId,
                            Price = applPayment != null ? (decimal?)applPayment.TotalAmountBgn : default,
                            Date = appl.SubmitDateTime,
                            SubmittedBy = $"{person.FirstName} {person.LastName}",
                            PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                            appl.DeliveryId
                        };

            if (buyerId.HasValue)
            {
                query = query.Where(x => x.BuyerId == buyerId.Value);
            }
            else if (fisherId.HasValue)
            {
                query = query.Where(x => x.FishermanId == fisherId.Value);
            }
            else if (permitId.HasValue)
            {
                query = query.Where(x => x.PermitId == permitId.Value);
            }
            else if (permitLicenceId.HasValue)
            {
                query = query.Where(x => x.PermitLicenceId == permitLicenceId.Value);
            }

            List<DuplicatesEntryDTO> entries = (from entry in query
                                                orderby entry.Date descending
                                                select new DuplicatesEntryDTO
                                                {
                                                    Id = entry.Id,
                                                    ApplicationId = entry.ApplicationId,
                                                    Date = entry.Date,
                                                    Price = entry.Price,
                                                    SubmittedBy = entry.SubmittedBy,
                                                    PageCode = entry.PageCode,
                                                    DeliveryId = entry.DeliveryId
                                                }).ToList();
            return entries;
        }

        public DuplicatesApplicationDTO GetDuplicatesApplication(int applicationId)
        {
            DuplicatesRegister duplicate = (from dup in Db.DuplicatesRegister
                                            where dup.ApplicationId == applicationId
                                                  && dup.RecordType == nameof(RecordTypesEnum.Application)
                                            select dup).FirstOrDefault();

            DuplicatesApplicationDTO result = null;

            if (duplicate == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<DuplicatesApplicationDTO>(draft);
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
                    result = new DuplicatesApplicationDTO
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
                result = new DuplicatesApplicationDTO
                {
                    Id = duplicate.Id,
                    ApplicationId = duplicate.ApplicationId,
                    Reason = duplicate.Reason
                };

                if (duplicate.BuyerId.HasValue)
                {
                    result.Buyer = new BuyerDuplicateDataDTO
                    {
                        BuyerId = duplicate.BuyerId.Value,
                        BuyerUrorrNumber = (from buyer in Db.BuyerRegisters
                                            where buyer.Id == duplicate.BuyerId.Value
                                            select buyer.UrrorNum).First()
                    };
                }
                else if (duplicate.PermitId.HasValue)
                {
                    result.Permit = new PermitDuplicateDataDTO
                    {
                        PermitId = duplicate.PermitId.Value,
                        PermitRegistrationNumber = (from permit in Db.CommercialFishingPermitRegisters
                                                    where permit.Id == duplicate.PermitId.Value
                                                    select permit.RegistrationNum).First()
                    };
                }
                else if (duplicate.PermitLicenceId.HasValue)
                {
                    result.PermitLicence = new PermitLicenseDuplicateDataDTO
                    {
                        PermitLicenceId = duplicate.PermitLicenceId.Value,
                        PermitLicenceRegistrationNumber = (from permitLicence in Db.CommercialFishingPermitLicensesRegisters
                                                           where permitLicence.Id == duplicate.PermitLicenceId.Value
                                                           select permitLicence.RegistrationNum).First()
                    };
                }
                else if (duplicate.FishermanId.HasValue)
                {
                    result.QualifiedFisher = new QualifiedFisherDuplicateDataDTO
                    {
                        QualifiedFisherId = duplicate.FishermanId.Value,
                        QualifiedFisher = (from fisher in Db.FishermenRegisters
                                           join person in Db.Persons on fisher.PersonId equals person.Id
                                           where fisher.Id == duplicate.FishermanId.Value
                                           select new RegixPersonDataDTO
                                           {
                                               EgnLnc = new EgnLncDTO
                                               {
                                                   IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType),
                                                   EgnLnc = person.EgnLnc
                                               },
                                               FirstName = person.FirstName,
                                               MiddleName = person.MiddleName,
                                               LastName = person.LastName
                                           }).First()
                    };
                }

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(duplicate.ApplicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(duplicate.ApplicationId);
                result.Files = Db.GetFiles(Db.DuplicatesRegisterFiles, duplicate.Id);

                result.IsPaid = applicationService.IsApplicationPaid(duplicate.ApplicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(duplicate.ApplicationId);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(duplicate.ApplicationId, ApplicationHierarchyTypesEnum.Online);

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(duplicate.ApplicationId);
                }

                if (result.HasDelivery)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(duplicate.ApplicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<DuplicatesApplicationRegixDataDTO> GetDuplicatesRegixData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetDuplicatesApplicationDataIds(applicationId);

            RegixChecksWrapperDTO<DuplicatesApplicationRegixDataDTO> result = new RegixChecksWrapperDTO<DuplicatesApplicationRegixDataDTO>
            {
                DialogDataModel = GetApplicationDuplicatesRegixData(regixDataIds.ApplicationId),
                RegiXDataModel = regixApplicationService.GetDuplicateApplicationChecks(regixDataIds.ApplicationId, regixDataIds.PageCode)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;
            return result;
        }

        public DuplicatesApplicationRegixDataDTO GetApplicationDuplicatesRegixData(int applicationId)
        {
            DuplicatesApplicationRegixDataDTO regixData = (from appl in Db.Applications
                                                           join type in Db.NapplicationTypes on appl.ApplicationTypeId equals type.Id
                                                           where appl.Id == applicationId
                                                           select new DuplicatesApplicationRegixDataDTO
                                                           {
                                                               ApplicationId = appl.Id,
                                                               PageCode = Enum.Parse<PageCodeEnum>(type.PageCode)
                                                           }).First();

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);

            return regixData;
        }

        public DuplicatesRegisterEditDTO GetApplicationDataForRegister(int applicationId)
        {
            int id = (from dup in Db.DuplicatesRegister
                      where dup.ApplicationId == applicationId
                        && dup.RecordType == nameof(RecordTypesEnum.Application)
                      select dup.Id).First();

            return GetDuplicateRegister(id);
        }

        public DuplicatesRegisterEditDTO GetRegisterByApplicationId(int applicationId)
        {
            int id = (from dup in Db.DuplicatesRegister
                      where dup.ApplicationId == applicationId
                        && dup.RecordType == nameof(RecordTypesEnum.Register)
                      select dup.Id).First();

            return GetDuplicateRegister(id);
        }

        public int AddDuplicatesApplication(DuplicatesApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null)
        {
            DuplicatesRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                entry = new DuplicatesRegister
                {
                    ApplicationId = application.ApplicationId.Value,
                    RecordType = nameof(RecordTypesEnum.Application),
                    Reason = application.Reason
                };

                SetDuplicateRegisterDataFromApplication(entry, application);

                Application dbAppl = (from appl in Db.Applications
                                      where appl.Id == entry.ApplicationId
                                      select appl).First();

                Db.AddOrEditApplicationSubmittedBy(dbAppl, application.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(dbAppl, application.SubmittedFor);
                entry.SubmittedForPerson = dbAppl.SubmittedForPerson;
                entry.SubmittedForLegal = dbAppl.SubmittedForLegal;
                Db.SaveChanges();

                if (application.Files != null)
                {
                    foreach (FileInfoDTO file in application.Files)
                    {
                        Db.AddOrEditFile(entry, entry.DuplicatesRegisterFiles, file);
                    }
                }

                Db.AddDeliveryData(dbAppl, application);

                Db.DuplicatesRegister.Add(entry);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> aquacultureFiles = application.Files;
            application.Files = null;
            stateMachine.Act(entry.ApplicationId, CommonUtils.Serialize(application), aquacultureFiles, nextManualStatus);

            return entry.Id;
        }

        public void EditDuplicatesApplication(DuplicatesApplicationDTO application, ApplicationStatusesEnum? manualStatus = null)
        {
            Application dbApplication;

            using (TransactionScope scope = new TransactionScope())
            {
                DuplicatesRegister dbDuplicate = (from dup in Db.DuplicatesRegister
                                                    .AsSplitQuery()
                                                    .Include(x => x.DuplicatesRegisterFiles)
                                                  where dup.Id == application.Id
                                                  select dup).First();

                dbDuplicate.Reason = application.Reason;

                SetDuplicateRegisterDataFromApplication(dbDuplicate, application);

                dbApplication = (from appl in Db.Applications
                                 where appl.Id == dbDuplicate.ApplicationId
                                 select appl).First();

                Db.AddOrEditApplicationSubmittedBy(dbApplication, application.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(dbApplication, application.SubmittedFor);
                Db.SaveChanges();

                if (application.Files != null)
                {
                    foreach (FileInfoDTO file in application.Files)
                    {
                        Db.AddOrEditFile(dbDuplicate, dbDuplicate.DuplicatesRegisterFiles, file);
                    }
                }

                Db.EditDeliveryData(dbApplication, application);

                scope.Complete();
            }

            List<FileInfoDTO> aquacultureFiles = application.Files;
            application.Files = null;
            stateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), aquacultureFiles, manualStatus, application.StatusReason);
        }

        public void EditDuplicatesRegixData(DuplicatesApplicationRegixDataDTO application)
        {
            Application dbApplication;

            using (TransactionScope scope = new TransactionScope())
            {
                dbApplication = (from appl in Db.Applications
                                 where appl.Id == application.ApplicationId.Value
                                 select appl).First();

                Db.AddOrEditApplicationSubmittedByRegixData(dbApplication, application.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedForRegixData(dbApplication, application.SubmittedFor);
                Db.SaveChanges();

                scope.Complete();
            }

            stateMachine.Act(id: dbApplication.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.DuplicatesRegister, id);
            return audit;
        }

        public DuplicatesRegisterEditDTO GetDuplicateRegister(int id)
        {
            var data = (from dup in Db.DuplicatesRegister
                        join appl in Db.Applications on dup.ApplicationId equals appl.Id
                        where dup.Id == id
                        select new
                        {
                            Ids = new
                            {
                                appl.SubmittedForPersonId,
                                appl.SubmittedForLegalId
                            },
                            Duplicate = new DuplicatesRegisterEditDTO
                            {
                                Id = dup.Id,
                                ApplicationId = dup.ApplicationId,
                                Reason = dup.Reason,
                                BuyerId = dup.BuyerId,
                                PermitId = dup.PermitId,
                                PermitLicenceId = dup.PermitLicenceId,
                                QualifiedFisherId = dup.FishermanId
                            }
                        }).First();

            DuplicatesRegisterEditDTO duplicate = data.Duplicate;

            duplicate.IsOnlineApplication = applicationService.IsApplicationHierarchyType(duplicate.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);
            duplicate.SubmittedFor = applicationService.GetRegisterSubmittedFor(duplicate.ApplicationId.Value, data.Ids.SubmittedForPersonId, data.Ids.SubmittedForLegalId);
            duplicate.Files = Db.GetFiles(Db.DuplicatesRegisterFiles, duplicate.ApplicationId.Value);

            return duplicate;
        }

        public int AddDuplicateRegister(DuplicatesRegisterEditDTO duplicate)
        {
            DuplicatesRegister entry;

            using (TransactionScope scope = new TransactionScope())
            {
                int registerApplicationId = (from aq in Db.DuplicatesRegister
                                             where aq.ApplicationId == duplicate.ApplicationId
                                                   && aq.RecordType == nameof(RecordTypesEnum.Application)
                                             select aq.Id).First();

                entry = new DuplicatesRegister
                {
                    ApplicationId = duplicate.ApplicationId.Value,
                    RegisterApplicationId = registerApplicationId,
                    RecordType = nameof(RecordTypesEnum.Register),
                    Reason = duplicate.Reason,
                    BuyerId = duplicate.BuyerId,
                    PermitId = duplicate.PermitId,
                    PermitLicenceId = duplicate.PermitLicenceId,
                    FishermanId = duplicate.QualifiedFisherId
                };

                Db.DuplicatesRegister.Add(entry);

                Db.AddOrEditRegisterSubmittedFor(entry, duplicate.SubmittedFor);
                Db.SaveChanges();

                if (duplicate.Files != null)
                {
                    foreach (FileInfoDTO file in duplicate.Files)
                    {
                        Db.AddOrEditFile(entry, entry.DuplicatesRegisterFiles, file);
                    }
                }

                stateMachine.Act(entry.ApplicationId);

                scope.Complete();
            }

            return entry.Id;
        }

        public Task<byte[]> DownloadDuplicateRegister(int id)
        {
            var ids = (from dup in Db.DuplicatesRegister
                       where dup.Id == id
                       select new
                       {
                           dup.BuyerId,
                           dup.FishermanId,
                           dup.PermitId,
                           dup.PermitLicenceId
                       }).First();

            if (ids.BuyerId.HasValue)
            {
                BuyerTypesEnum buyerType = (from buyer in Db.BuyerRegisters
                                            join type in Db.NbuyerTypes on buyer.BuyerTypeId equals type.Id
                                            where buyer.Id == ids.BuyerId.Value
                                            select Enum.Parse<BuyerTypesEnum>(type.Code)).First();

                return buyerType switch
                {
                    BuyerTypesEnum.Buyer => jasperReportExecutionService.GetFirstSaleBuyerRegister(ids.BuyerId.Value, duplicate: true),
                    BuyerTypesEnum.CPP => jasperReportExecutionService.GetFirstSaleCenterRegister(ids.BuyerId.Value, duplicate: true),
                    _ => throw new ArgumentException("Invalid BuyerTypesEnum value")
                };
            }
            else if (ids.FishermanId.HasValue)
            {
                return jasperReportExecutionService.GetFishermanRegister(ids.FishermanId.Value, duplicate: true);
            }
            else if (ids.PermitId.HasValue)
            {
                CommercialFishingTypesEnum permitType = (from permit in Db.CommercialFishingPermitRegisters
                                                         join type in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals type.Id
                                                         where permit.Id == ids.PermitId.Value
                                                         select Enum.Parse<CommercialFishingTypesEnum>(type.Code)).First();

                WaterTypesEnum? waterType = null;

                if (permitType == CommercialFishingTypesEnum.Permit || permitType == CommercialFishingTypesEnum.ThirdCountryPermit)
                {
                    waterType = (from permit in Db.CommercialFishingPermitRegisters
                                 join type in Db.NwaterTypes on permit.WaterTypeId equals type.Id
                                 where permit.Id == ids.PermitId.Value
                                 select Enum.Parse<WaterTypesEnum>(type.Code)).First();
                }

                switch (permitType)
                {
                    case CommercialFishingTypesEnum.Permit:
                        return waterType == WaterTypesEnum.DANUBE
                            ? jasperReportExecutionService.GetDanubePermitRegister(ids.PermitId.Value, duplicate: true)
                            : jasperReportExecutionService.GetBlackSeaPermitRegister(ids.PermitId.Value, duplicate: true);
                    case CommercialFishingTypesEnum.PoundNetPermit:
                        return jasperReportExecutionService.GetPoundNetPermitRegister(ids.PermitId.Value, duplicate: true);
                    case CommercialFishingTypesEnum.ThirdCountryPermit:
                        return waterType == WaterTypesEnum.DANUBE
                            ? jasperReportExecutionService.GetDanubeThirdCountryPermitRegister(ids.PermitId.Value, duplicate: true)
                            : jasperReportExecutionService.GetBlackSeaThirdCountryPermitRegister(ids.PermitId.Value, duplicate: true);
                };
            }
            else if (ids.PermitLicenceId.HasValue)
            {
                CommercialFishingTypesEnum permitLicType = (from permit in Db.CommercialFishingPermitRegisters
                                                            join type in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals type.Id
                                                            where permit.Id == ids.PermitId.Value
                                                            select Enum.Parse<CommercialFishingTypesEnum>(type.Code)).First();

                switch (permitLicType)
                {
                    case CommercialFishingTypesEnum.PermitLicense:
                        return jasperReportExecutionService.GetDanubePermitRegister(ids.PermitId.Value, duplicate: true);
                    case CommercialFishingTypesEnum.PoundNetPermitLicense:
                        return jasperReportExecutionService.GetPoundNetPermitLicenseRegister(ids.PermitId.Value, duplicate: true);
                    case CommercialFishingTypesEnum.QuataSpeciesPermitLicense:
                        return jasperReportExecutionService.GetQuotaSpeciesPermitLicenseRegister(ids.PermitId.Value, duplicate: true);
                };
            }

            throw new ArgumentException("One of BuyerID, FishermanID, PermitID, PermitLicenceID must be not null from DuplicatesRegister");
        }

        private BaseRegixApplicationDataIds GetDuplicatesApplicationDataIds(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = (from appl in Db.Applications
                                                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                        where appl.Id == applicationId
                                                        select new BaseRegixApplicationDataIds
                                                        {
                                                            ApplicationId = appl.Id,
                                                            PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                                                        }).First();

            return regixDataIds;
        }

        private void SetDuplicateRegisterDataFromApplication(DuplicatesRegister entry, DuplicatesApplicationDTO application)
        {
            if (BuyerPageCodes.Contains(application.PageCode.Value))
            {
                if (application.Buyer.IsOnline.Value)
                {
                    entry.BuyerId = (from buyer in Db.BuyerRegisters
                                     where buyer.UrrorNum == application.Buyer.BuyerUrorrNumber
                                     select buyer.Id).FirstOrDefault();

                    if (!entry.BuyerId.HasValue)
                    {
                        throw new BuyerDoesNotExistsException();
                    }
                }
                else
                {
                    entry.BuyerId = application.Buyer.BuyerId;
                }

                entry.PermitId = null;
                entry.PermitLicenceId = null;
                entry.FishermanId = null;
            }
            else if (PermitPageCodes.Contains(application.PageCode.Value))
            {
                if (application.Permit.IsOnline.Value)
                {
                    entry.PermitId = (from permit in Db.CommercialFishingPermitRegisters
                                      where permit.RegistrationNum == application.Permit.PermitRegistrationNumber
                                      select permit.Id).FirstOrDefault();

                    if (!entry.PermitId.HasValue)
                    {
                        throw new PermitDoesNotExistException();
                    }
                }
                else
                {
                    entry.PermitId = application.Permit.PermitId;
                }

                entry.BuyerId = null;
                entry.PermitLicenceId = null;
                entry.FishermanId = null;
            }
            else if (PermitLicencePageCodes.Contains(application.PageCode.Value))
            {
                if (application.PermitLicence.IsOnline.Value)
                {
                    entry.PermitLicenceId = (from permitLicence in Db.CommercialFishingPermitLicensesRegisters
                                             where permitLicence.RegistrationNum == application.PermitLicence.PermitLicenceRegistrationNumber
                                             select permitLicence.Id).FirstOrDefault();

                    if (!entry.PermitLicenceId.HasValue)
                    {
                        throw new PermitLicenceDoesNotExistException();
                    }
                }
                else
                {
                    entry.PermitLicenceId = application.PermitLicence.PermitLicenceId;
                }

                entry.BuyerId = null;
                entry.PermitId = null;
                entry.FishermanId = null;
            }
            else if (FisherPageCodes.Contains(application.PageCode.Value))
            {
                if (application.QualifiedFisher.IsOnline.Value)
                {
                    entry.FishermanId = (from fisher in Db.FishermenRegisters
                                         join person in Db.Persons on fisher.PersonId equals person.Id
                                         where person.IdentifierType == application.QualifiedFisher.QualifiedFisher.EgnLnc.IdentifierType.ToString()
                                            && person.EgnLnc == application.QualifiedFisher.QualifiedFisher.EgnLnc.EgnLnc.Trim()
                                            && person.FirstName.ToLower() == application.QualifiedFisher.QualifiedFisher.FirstName.Trim().ToLower()
                                            && person.LastName.ToLower() == application.QualifiedFisher.QualifiedFisher.LastName.Trim().ToLower()
                                         select fisher.Id).FirstOrDefault();

                    if (!entry.FishermanId.HasValue)
                    {
                        throw new QualifiedFisherDoesNotExistException();
                    }
                }
                else
                {
                    entry.FishermanId = application.QualifiedFisher.QualifiedFisherId;
                }

                entry.BuyerId = null;
                entry.PermitId = null;
                entry.PermitLicenceId = null;
            }
        }
    }
}
