using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.Buyers.ChangeOfCircumstances;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Buyers
{
    public partial class BuyersService : Service, IBuyersService
    {
        public BuyerChangeOfCircumstancesApplicationDTO GetBuyerChangeOfCircumstancesApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            BuyerChangeOfCircumstancesApplicationDTO result = null;

            if (changes.Count == 0)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<BuyerChangeOfCircumstancesApplicationDTO>(draft);
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
                    result = new BuyerChangeOfCircumstancesApplicationDTO
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
                result = new BuyerChangeOfCircumstancesApplicationDTO
                {
                    ApplicationId = applicationId,
                    Changes = changes,
                    BuyerId = changes[0].BuyerId.Value
                };

                result.BuyerUrorrNumber = (from buyer in Db.BuyerRegisters
                                           where buyer.Id == result.BuyerId
                                           select buyer.UrrorNum).First();

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);
                result.IsDraft = false;
                result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);

                result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                if (result.HasDelivery.Value)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }

                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
            }

            return result;
        }

        public RegixChecksWrapperDTO<BuyerChangeOfCircumstancesRegixDataDTO> GetBuyerChangeOfCircumstancesRegixData(int applicationId)
        {
            PageCodeEnum pageCode = (from appl in Db.Applications
                                     join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                     where appl.Id == applicationId
                                     select Enum.Parse<PageCodeEnum>(applType.PageCode)).First();

            RegixChecksWrapperDTO<BuyerChangeOfCircumstancesRegixDataDTO> result = new RegixChecksWrapperDTO<BuyerChangeOfCircumstancesRegixDataDTO>
            {
                DialogDataModel = pageCode == PageCodeEnum.ChangeFirstSaleBuyer
                                    ? GetBuyerApplicationChangeOfCircumstancesRegixData(applicationId)
                                    : GetFirstSaleCenterApplicationChangeOfCircumstancesRegixData(applicationId),
                RegiXDataModel = pageCode == PageCodeEnum.ChangeFirstSaleBuyer
                                    ? regixApplicationService.GetBuyerChangeOfCircumstancesChecks(applicationId)
                                    : regixApplicationService.GetFirstSaleCenterChangeOfCircumstancesChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public BuyerChangeOfCircumstancesRegixDataDTO GetBuyerApplicationChangeOfCircumstancesRegixData(int applicationId)
        {
            var regixDataIds = GetApplicationChangeOfCircumstancesDataIds(applicationId);

            var regixData = new BuyerChangeOfCircumstancesRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);
            regixData.Changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);
            regixData.BuyerId = regixData.Changes[0].BuyerId.Value;

            return regixData;
        }

        public BuyerChangeOfCircumstancesRegixDataDTO GetFirstSaleCenterApplicationChangeOfCircumstancesRegixData(int applicationId)
        {
            var regixDataIds = GetApplicationChangeOfCircumstancesDataIds(applicationId);

            var regixData = new BuyerChangeOfCircumstancesRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);
            regixData.Changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);
            regixData.BuyerId = regixData.Changes[0].BuyerId.Value;

            return regixData;
        }

        public int AddBuyerChangeOfCircumstancesApplication(BuyerChangeOfCircumstancesApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null)
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

                int buyerId = GetBuyerIdByUrorrOrThrow(application.IsOnline.Value, application.PageCode.Value, application.BuyerUrorrNumber, application.BuyerId);

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, buyerId: buyerId);

                AddBuyerChangeOfCircumstancesDeliveryData(dbApplication, application);

                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            applicationStateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), files, nextManualStatus);

            return dbApplication.Id;
        }

        public void EditBuyerChangeOfCircumstancesApplication(BuyerChangeOfCircumstancesApplicationDTO application,
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

                int buyerId = GetBuyerIdByUrorrOrThrow(application.IsOnline.Value, application.PageCode.Value, application.BuyerUrorrNumber, application.BuyerId);

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, buyerId: buyerId);

                EditBuyerChangeOfCircumstancesDeliveryData(dbApplication, application);

                if (application.Files != null)
                {
                    foreach (FileInfoDTO file in application.Files)
                    {
                        Db.AddOrEditFile(dbApplication, dbApplication.ApplicationFiles, file);
                    }
                }

                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            applicationStateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditBuyerChangeOfCircumstancesRegixData(BuyerChangeOfCircumstancesRegixDataDTO application)
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

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, buyerId: application.BuyerId.Value);

                scope.Complete();
            }

            applicationStateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public BuyerEditDTO GetBuyerFromChangeOfCircumstancesApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            BuyerEditDTO result = GetRegisterEntry(changes[0].BuyerId.Value);
            return result;
        }

        public void CompleteChangeOfCircumstancesApplication(BuyerEditDTO buyer, bool ignoreLogBookConflicts)
        {
            EditRegisterEntry(buyer, ignoreLogBookConflicts);
            applicationStateMachine.Act(buyer.ApplicationId);
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

        private void AddBuyerChangeOfCircumstancesDeliveryData(Application dbApplication, BuyerChangeOfCircumstancesApplicationDTO application)
        {
            if (application.HasDelivery.Value)
            {
                dbApplication.Delivery = Db.AddDeliveryData(application.DeliveryData);
            }
        }

        private void EditBuyerChangeOfCircumstancesDeliveryData(Application dbApplication, BuyerChangeOfCircumstancesApplicationDTO application)
        {
            if (application.HasDelivery.Value)
            {
                dbApplication.Delivery = Db.EditDeliveryData(application.DeliveryData, dbApplication.DeliveryId.Value);
            }
        }
    }
}
