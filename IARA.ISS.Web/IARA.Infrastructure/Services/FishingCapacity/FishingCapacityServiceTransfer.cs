using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.TransferCapacity;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class FishingCapacityService : Service, IFishingCapacityService
    {
        public TransferFishingCapacityApplicationDTO GetTransferFishingCapacityApplication(int applicationId)
        {
            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            TransferFishingCapacityApplicationDTO result = null;

            if (change == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<TransferFishingCapacityApplicationDTO>(draft);
                    result.IsDraft = true;
                    result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                    result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                    if (result.IsPaid && result.PaymentInformation == null)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
                else
                {
                    result = new TransferFishingCapacityApplicationDTO
                    {
                        IsDraft = true,
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
                result = new TransferFishingCapacityApplicationDTO
                {
                    ApplicationId = applicationId,
                    CapacityCertificateId = change.CapacityCertificateTransferId
                };

                result.Holders = GetCertificateHoldersFromChangeHistory(change);

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);

                result.IsDraft = false;
                result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                if (result.HasDelivery)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<TransferFishingCapacityRegixDataDTO> GetTransferFishingCapacityRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<TransferFishingCapacityRegixDataDTO> result = new RegixChecksWrapperDTO<TransferFishingCapacityRegixDataDTO>
            {
                DialogDataModel = GetApplicationTransferRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetFishingCapacityTransferChecks(applicationId),
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public TransferFishingCapacityRegixDataDTO GetApplicationTransferRegixData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetApplicationDataIds(applicationId);

            TransferFishingCapacityRegixDataDTO regixData = new TransferFishingCapacityRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId)
            };

            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);
            regixData.Holders = GetCertificateHoldersRegixDataFromChangeHistory(change);

            return regixData;
        }

        public int AddTransferFishingCapacityApplication(TransferFishingCapacityApplicationDTO application, ApplicationStatusesEnum? nextManualStatus)
        {
            return AddTransferFishingCapacityApplication(application, RecordTypesEnum.Application, nextManualStatus);
        }

        public void EditTransferFishingCapacityApplication(TransferFishingCapacityApplicationDTO application,
                                                           ApplicationStatusesEnum? manualStatus = null)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Application dbApplication = (from appl in Db.Applications
                                                .AsSplitQuery()
                                                .Include(x => x.ApplicationFiles)
                                             where appl.Id == application.ApplicationId.Value
                                             select appl).First();

                Db.AddOrEditApplicationSubmittedBy(dbApplication, application.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(dbApplication, application.SubmittedFor);
                Db.SaveChanges();

                if (application.Files != null)
                {
                    foreach (FileInfoDTO file in application.Files)
                    {
                        Db.AddOrEditFile(dbApplication, dbApplication.ApplicationFiles, file);
                    }
                }

                EditTransferCapacityChangeHistory(application.ApplicationId.Value, application.Holders);

                Db.EditDeliveryData(dbApplication, application);
                Db.SaveChanges();

                scope.Complete();
            }


            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditTransferFishingCapacityRegixData(TransferFishingCapacityRegixDataDTO application)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Application dbApplication = (from appl in Db.Applications
                                             where appl.Id == application.ApplicationId.Value
                                             select appl).First();

                Db.AddOrEditApplicationSubmittedByRegixData(dbApplication, application.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedForRegixData(dbApplication, application.SubmittedFor);
                Db.SaveChanges();

                EditTransferCapacityChangeHistory(application.ApplicationId.Value, application.Holders);
                Db.SaveChanges();

                scope.Complete();
            }

            stateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public void CompleteTransferFishingCapacityApplication(TransferFishingCapacityApplicationDTO application)
        {
            AddTransferFishingCapacityApplication(application, RecordTypesEnum.Register);
        }

        private List<FishingCapacityHolderDTO> GetCertificateHoldersFromChangeHistory(CapacityChangeHistoryDTO change)
        {
            List<CapacityHolderHelper> holders = GetCapacityHolderHelpers(change.CapacityCertificateIds);

            List<FishingCapacityHolderDTO> result = (from holder in holders
                                                     select new FishingCapacityHolderDTO
                                                     {
                                                         Id = holder.Id,
                                                         IsHolderPerson = holder.IsHolderPerson,
                                                         TransferredTonnage = holder.TransferredTonnage,
                                                         TransferredPower = holder.TransferredPower,
                                                         IsActive = holder.IsActive
                                                     }).ToList();

            SetCapacityHoldersPersonLegal(result, holders);

            return result;
        }

        private List<FishingCapacityHolderRegixDataDTO> GetCertificateHoldersRegixDataFromChangeHistory(CapacityChangeHistoryDTO change)
        {
            List<CapacityHolderRegixHelper> holders = GetCapacityHolderRegixHelpers(change.CapacityCertificateIds);

            List<FishingCapacityHolderRegixDataDTO> result = (from holder in holders
                                                              select new FishingCapacityHolderRegixDataDTO
                                                              {
                                                                  Id = holder.Id,
                                                                  IsHolderPerson = holder.IsHolderPerson,
                                                                  IsActive = holder.IsActive
                                                              }).ToList();

            SetCapacityHoldersPersonLegal(result, holders);

            return result;
        }

        /// <summary>
        /// Adds transfer fishing capacity application/register entry
        /// </summary>
        /// <param name="application">DTO with data for transfering</param>
        /// <param name="recordType">Type of record: Application/Register</param>
        /// <param name="nextManualStatus">Next manual status for the state machine - needed for record types Application</param>
        /// <returns>The id of the application</returns>
        private int AddTransferFishingCapacityApplication(TransferFishingCapacityApplicationDTO application,
                                                          RecordTypesEnum recordType,
                                                          ApplicationStatusesEnum? nextManualStatus = null)
        {
            Application dbApplication;

            using (TransactionScope scope = new TransactionScope())
            {
                dbApplication = (from appl in Db.Applications
                                    .AsSplitQuery()
                                    .Include(x => x.ApplicationFiles)
                                 where appl.Id == application.ApplicationId.Value
                                 select appl).First();

                Db.AddOrEditApplicationSubmittedBy(dbApplication, application.SubmittedBy);
                Db.SaveChanges();

                Db.AddOrEditApplicationSubmittedFor(dbApplication, application.SubmittedFor);
                Db.SaveChanges();

                if (application.Files != null)
                {
                    foreach (FileInfoDTO file in application.Files)
                    {
                        Db.AddOrEditFile(dbApplication, dbApplication.ApplicationFiles, file);
                    }
                }

                AddTransferCapacityChangeHistory(recordType, application.ApplicationId.Value, application.CapacityCertificateId.Value, application.Holders);

                Db.AddDeliveryData(dbApplication, application);
                Db.SaveChanges();

                if (recordType == RecordTypesEnum.Register)
                {
                    stateMachine.Act(application.ApplicationId.Value);
                }

                scope.Complete();
            }

            if (recordType == RecordTypesEnum.Application)
            {
                List<FileInfoDTO> aquacultureFiles = application.Files;
                application.Files = null;
                stateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), aquacultureFiles, nextManualStatus);
            }

            return dbApplication.Id;
        }

        /// <summary>
        /// Добавяне на CapacityChangeHistory при прехвърляне на капацитет 
        /// </summary>
        /// <param name="recordType">RecordType на записа</param>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="capacityCertificateId">Удостоверение за свободен капацитет, от което се прехвърля</param>
        /// <param name="holders">Лица, на които се прехвърля капацитет</param>
        private void AddTransferCapacityChangeHistory(RecordTypesEnum recordType,
                                                      int applicationId,
                                                      int capacityCertificateId,
                                                      List<FishingCapacityHolderDTO> holders)
        {
            using TransactionScope scope = new TransactionScope();

            DateTime now = DateTime.Now;

            int? registerApplicationId = null;

            if (recordType == RecordTypesEnum.Register)
            {
                registerApplicationId = (from capChange in Db.CapacityChangeHistory
                                         where capChange.ApplicationId == applicationId
                                            && capChange.RecordType == nameof(RecordTypesEnum.Application)
                                         select capChange.Id).First();
            }

            CapacityChangeHistory entry = new CapacityChangeHistory
            {
                ApplicationId = applicationId,
                RecordType = recordType.ToString(),
                RegisterApplicationId = registerApplicationId,
                TypeOfChange = nameof(FishingCapacityChangeTypeEnum.Transfer),
                CapacityCertificateTransferId = capacityCertificateId,
                DateOfChange = now
            };

            Db.CapacityChangeHistory.Add(entry);

            CapacityCertificatesRegister certificate = (from cert in Db.CapacityCertificatesRegister
                                                        where cert.Id == entry.CapacityCertificateTransferId.Value
                                                        select cert).First();

            AddCapacityCertificateForHolders(recordType, entry, holders, now, certificate.CertificateValidTo);

            if (recordType == RecordTypesEnum.Register)
            {
                certificate.CertificateValidTo = now;
            }

            Db.SaveChanges();
            scope.Complete();
        }

        /// <summary>
        /// Редактиране на CapacityChangeHistory при прехвърляне на капацитет
        /// </summary>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="holders">Лица, на които се прехвърля капацитет</param>
        private void EditTransferCapacityChangeHistory(int applicationId, List<FishingCapacityHolderDTO> holders)
        {
            DateTime now = DateTime.Now;

            CapacityChangeHistory change = (from ch in Db.CapacityChangeHistory
                                            where ch.ApplicationId == applicationId
                                            select ch).First();

            DateTime certificateValidTo = (from cert in Db.CapacityCertificatesRegister
                                           where cert.Id == change.CapacityCertificateTransferId
                                           select cert.CertificateValidTo).First();

            EditCapacityCertificatesForHolders(applicationId, change, holders, now, certificateValidTo);
            Db.SaveChanges();
        }

        /// <summary>
        /// Редактиране на RegiX данни от CapacityChangeHistory при прехвърляне на капацитет
        /// </summary>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="holders">Лица, на които се прехвърля капацитет</param>
        private void EditTransferCapacityChangeHistory(int applicationId, List<FishingCapacityHolderRegixDataDTO> holders)
        {
            EditCapacityCertificatesForHoldersRegix(applicationId, holders);
            Db.SaveChanges();
        }
    }
}
