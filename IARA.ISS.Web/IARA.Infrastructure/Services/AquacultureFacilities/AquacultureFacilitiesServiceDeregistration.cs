using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.AquacultureFacilities.Deregistration;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class AquacultureFacilitiesService : Service, IAquacultureFacilitiesService
    {
        public AquacultureDeregistrationApplicationDTO GetAquacultureDeregistrationApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            AquacultureDeregistrationApplicationDTO result = null;

            if (changes.Count == 0)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<AquacultureDeregistrationApplicationDTO>(draft);
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
                    result = new AquacultureDeregistrationApplicationDTO
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
                result = new AquacultureDeregistrationApplicationDTO
                {
                    ApplicationId = applicationId,
                    AquacultureFacilityId = changes[0].AquacultureFacilityId.Value
                };

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);
                result.IsDraft = false;
                result.DeregistrationReason = changes[0].Description;
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

        public RegixChecksWrapperDTO<AquacultureDeregistrationRegixDataDTO> GetAquacultureDeregistrationRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<AquacultureDeregistrationRegixDataDTO> result = new RegixChecksWrapperDTO<AquacultureDeregistrationRegixDataDTO>
            {
                DialogDataModel = GetApplicationDeregistrationRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetAquacultureDeregistrationChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public AquacultureDeregistrationRegixDataDTO GetApplicationDeregistrationRegixData(int applicationId)
        {
            PageCodeEnum pageCode = (from appl in Db.Applications
                                     join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                     where appl.Id == applicationId
                                     select Enum.Parse<PageCodeEnum>(applType.PageCode)).First();

            AquacultureDeregistrationRegixDataDTO regixData = new AquacultureDeregistrationRegixDataDTO
            {
                ApplicationId = applicationId,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId),
                PageCode = pageCode
            };

            return regixData;
        }

        public int AddAquacultureDeregistrationApplication(AquacultureDeregistrationApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null)
        {
            DateTime now = DateTime.Now;

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

                List<ChangeOfCircumstancesDTO> changes = GetDeregAquacultureChangeOfCircumstances(application.AquacultureFacilityId.Value, application.DeregistrationReason);
                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, changes, null, application.AquacultureFacilityId);

                Db.AddDeliveryData(dbApplication, application);
                Db.SaveChanges();
                scope.Complete();
            }


            List<FileInfoDTO> aquacultureFiles = application.Files;
            application.Files = null;
            stateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), aquacultureFiles, nextManualStatus);

            return dbApplication.Id;
        }

        public void EditAquacultureDeregistrationApplication(AquacultureDeregistrationApplicationDTO application,
                                                             ApplicationStatusesEnum? manualStatus = null)
        {
            DateTime now = DateTime.Now;

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

                List<ChangeOfCircumstancesDTO> changes = GetDeregAquacultureChangeOfCircumstances(application.AquacultureFacilityId.Value, application.DeregistrationReason);
                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, changes, null, application.AquacultureFacilityId);

                Db.EditDeliveryData(dbApplication, application);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditAquacultureDeregistrationRegixData(AquacultureDeregistrationRegixDataDTO application)
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

        public AquacultureFacilityEditDTO GetAquacultureFromDeregistrationApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            AquacultureFacilityEditDTO result = GetAquaculture(changes[0].AquacultureFacilityId.Value);
            return result;
        }

        private List<ChangeOfCircumstancesDTO> GetDeregAquacultureChangeOfCircumstances(int aquacultureId, string description)
        {
            DateTime now = DateTime.Now;

            int typeId = (from type in Db.NchangeOfCircumstancesTypes
                          where type.Code == nameof(PageCodeEnum.AquaFarmDereg)
                                && type.ValidFrom <= now
                                && type.ValidTo > now
                          select type.Id).Single();

            ChangeOfCircumstancesDTO change = new ChangeOfCircumstancesDTO
            {
                TypeId = typeId,
                DataType = ChangeOfCircumstancesDataTypeEnum.FreeText,
                AquacultureFacilityId = aquacultureId,
                Description = description
            };

            return new List<ChangeOfCircumstancesDTO> { change };
        }
    }
}
