using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.Duplicates;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class FishingCapacityService : Service, IFishingCapacityService
    {
        public CapacityCertificateDuplicateApplicationDTO GetCapacityCertificateDuplicateApplication(int applicationId)
        {
            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            CapacityCertificateDuplicateApplicationDTO result = null;

            if (change == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<CapacityCertificateDuplicateApplicationDTO>(draft);
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
                    result = new CapacityCertificateDuplicateApplicationDTO
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
                result = new CapacityCertificateDuplicateApplicationDTO
                {
                    ApplicationId = applicationId,
                    CapacityCertificateId = change.CapacityCertificateTransferId,
                    Reason = change.ReasonOfChange
                };

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

        public RegixChecksWrapperDTO<CapacityCertificateDuplicateRegixDataDTO> GetCapacityCertificateDuplicateRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<CapacityCertificateDuplicateRegixDataDTO> result = new RegixChecksWrapperDTO<CapacityCertificateDuplicateRegixDataDTO>
            {
                DialogDataModel = GetApplicationCapacityCertificateDuplicateRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetFishingCapacityDuplicateChecks(applicationId),
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public CapacityCertificateDuplicateRegixDataDTO GetApplicationCapacityCertificateDuplicateRegixData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetApplicationDataIds(applicationId);

            CapacityCertificateDuplicateRegixDataDTO regixData = new CapacityCertificateDuplicateRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId)
            };

            return regixData;
        }

        public int AddCapacityCertificateDuplicateApplication(CapacityCertificateDuplicateApplicationDTO application,
                                                              ApplicationStatusesEnum? nextManualStatus)
        {
            return AddCapacityCertificateDuplicateApplication(application, RecordTypesEnum.Application, nextManualStatus);
        }

        public void EditCapacityCertificateDuplicateApplication(CapacityCertificateDuplicateApplicationDTO application,
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

                Db.EditDeliveryData(dbApplication, application);

                EditDuplicateCapacityChangeHistory(application.ApplicationId.Value, application.CapacityCertificateId.Value, application.Reason);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditCapacityCertificateDuplicateRegixData(CapacityCertificateDuplicateRegixDataDTO application)
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

                scope.Complete();
            }

            stateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public void CompleteCapacityCertificateDuplicateApplication(CapacityCertificateDuplicateApplicationDTO application)
        {
            AddCapacityCertificateDuplicateApplication(application, RecordTypesEnum.Register);
        }

        /// <summary>
        /// Add capacity certificate duplicate application/register entry
        /// </summary>
        /// <param name="application">DTO with data for the duplicate</param>
        /// <param name="recordType">Type of record: Application/Register</param>
        /// <param name="nextManualStatus">Next manual status for the state machine - needed for record types Application</param>
        /// <returns>The id of the application</returns>
        private int AddCapacityCertificateDuplicateApplication(CapacityCertificateDuplicateApplicationDTO application,
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

                if (recordType == RecordTypesEnum.Application)
                {
                    Db.AddDeliveryData(dbApplication, application);
                }

                AddDuplicateCapacityChangeHistory(recordType, application.ApplicationId.Value, application.CapacityCertificateId.Value, application.Reason);
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
        /// Добавяне на CapacityChangeHistory при издаване на дубликат
        /// </summary>
        /// <param name="recordType">RecordType на записа</param>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="capacityCertificateId">Удостоверение за свободен капацитет, на което се издава дубликат</param>
        /// <param name="reason">Причина за подаване на заявлението</param>
        private void AddDuplicateCapacityChangeHistory(RecordTypesEnum recordType, int applicationId, int capacityCertificateId, string reason)
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
                TypeOfChange = nameof(FishingCapacityChangeTypeEnum.Duplicate),
                CapacityCertificateTransferId = capacityCertificateId,
                DateOfChange = now,
                ReasonOfChange = reason
            };

            Db.CapacityChangeHistory.Add(entry);

            CapacityCertificatesRegister certificate = (from cert in Db.CapacityCertificatesRegister
                                                        where cert.Id == entry.CapacityCertificateTransferId.Value
                                                        select cert).First();

            if (recordType == RecordTypesEnum.Register)
            {
                // добавяме ново удостоверение със същите данни и валидност
                AddCapacityCertificate(entry, certificate, now, certificate.CertificateValidTo);

                // инвалидираме старото удостоверение
                certificate.CertificateValidTo = now;
            }

            Db.SaveChanges();
            scope.Complete();
        }

        private void AddCapacityCertificate(CapacityChangeHistory change,
                                            CapacityCertificatesRegister certificate,
                                            DateTime validFrom,
                                            DateTime validTo)
        {
            CapacityCertificatesRegister newCertificate = new CapacityCertificatesRegister
            {
                RecordType = change.RecordType,
                PersonId = certificate.PersonId,
                LegalId = certificate.LegalId,
                GrossTonnage = certificate.GrossTonnage,
                MainEnginePower = certificate.MainEnginePower,
                CertificateValidFrom = validFrom,
                CertificateValidTo = validTo
            };

            CapacityChangeHistoryCertificate changeCertificate = new CapacityChangeHistoryCertificate
            {
                CapacityCertificate = newCertificate,
                CapacityChangeHistory = change
            };

            Db.CapacityChangeHistoryCertificates.Add(changeCertificate);
            Db.SaveChanges();
        }

        private void EditDuplicateCapacityChangeHistory(int applicationId, int certificateId, string reason)
        {
            CapacityChangeHistory change = (from ch in Db.CapacityChangeHistory
                                            where ch.ApplicationId == applicationId
                                            select ch).First();

            change.CapacityCertificateTransferId = certificateId;
            change.ReasonOfChange = reason;
            Db.SaveChanges();
        }
    }
}
