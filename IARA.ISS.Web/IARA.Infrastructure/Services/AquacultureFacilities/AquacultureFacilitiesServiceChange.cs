using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.AquacultureFacilities.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class AquacultureFacilitiesService : Service, IAquacultureFacilitiesService
    {
        public AquacultureChangeOfCircumstancesApplicationDTO GetAquacultureChangeOfCircumstancesApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            AquacultureChangeOfCircumstancesApplicationDTO result = null;

            if (changes.Count == 0)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<AquacultureChangeOfCircumstancesApplicationDTO>(draft);
                    result.IsDraft = true;
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
                    result = new AquacultureChangeOfCircumstancesApplicationDTO
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
                result = new AquacultureChangeOfCircumstancesApplicationDTO
                {
                    ApplicationId = applicationId,
                    Changes = changes,
                    AquacultureFacilityId = changes[0].AquacultureFacilityId.Value
                };

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);
                result.IsDraft = false;
                result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);

                result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                if (result.HasDelivery)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<AquacultureChangeOfCircumstancesRegixDataDTO> GetAquacultureChangeOfCircumstancesRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<AquacultureChangeOfCircumstancesRegixDataDTO> result = new RegixChecksWrapperDTO<AquacultureChangeOfCircumstancesRegixDataDTO>
            {
                DialogDataModel = GetApplicationChangeOfCircumstancesRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetAquacultureChangeOfCircumstancesChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public AquacultureChangeOfCircumstancesRegixDataDTO GetApplicationChangeOfCircumstancesRegixData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetApplicationChangeOfCircumstancesDataIds(applicationId);

            AquacultureChangeOfCircumstancesRegixDataDTO regixData = new AquacultureChangeOfCircumstancesRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);
            regixData.Changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);
            regixData.AquacultureFacilityId = regixData.Changes[0].AquacultureFacilityId.Value;

            return regixData;
        }

        public int AddAquacultureChangeOfCircumstancesApplication(AquacultureChangeOfCircumstancesApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null)
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

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, null, application.AquacultureFacilityId);

                Db.AddDeliveryData(dbApplication, application);

                Db.SaveChanges();
                scope.Complete();
            }

            List<FileInfoDTO> aquacultureFiles = application.Files;
            application.Files = null;
            stateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), aquacultureFiles, nextManualStatus);

            return dbApplication.Id;
        }

        public void EditAquacultureChangeOfCircumstancesApplication(AquacultureChangeOfCircumstancesApplicationDTO application,
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

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, null, application.AquacultureFacilityId);

                Db.EditDeliveryData(dbApplication, application);

                if (application.Files != null)
                {
                    foreach (FileInfoDTO file in application.Files)
                    {
                        Db.AddOrEditFile(dbApplication, dbApplication.ApplicationFiles, file);
                    }
                }

                Db.SaveChanges();
                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditAquacultureChangeOfCircumstancesRegixData(AquacultureChangeOfCircumstancesRegixDataDTO application)
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

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, null, application.AquacultureFacilityId);

                scope.Complete();
            }

            stateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public AquacultureFacilityEditDTO GetAquacultureFromChangeOfCircumstancesApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            AquacultureFacilityEditDTO result = GetAquaculture(changes[0].AquacultureFacilityId.Value);
            return result;
        }

        public void CompleteChangeOfCircumstancesApplication(AquacultureFacilityEditDTO aquaculture, bool ignoreLogBookConflicts)
        {
            EditAquaculture(aquaculture, ignoreLogBookConflicts);
            stateMachine.Act(aquaculture.ApplicationId.Value);
        }

        private BaseRegixApplicationDataIds GetApplicationChangeOfCircumstancesDataIds(int applicationId)
        {
            var regixDataIds = (from appl in Db.Applications
                                join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                where appl.Id == applicationId
                                select new BaseRegixApplicationDataIds
                                {
                                    ApplicationId = applicationId,
                                    PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode),
                                }).First();

            return regixDataIds;
        }
    }
}
