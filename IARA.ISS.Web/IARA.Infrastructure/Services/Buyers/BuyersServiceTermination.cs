using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Buyers.Termination;
using IARA.DomainModels.DTOModels.Files;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Buyers
{
    public partial class BuyersService : Service, IBuyersService
    {
        public BuyerTerminationApplicationDTO GetBuyerTerminationApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            BuyerTerminationApplicationDTO result = null;

            if (changes.Count == 0)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<BuyerTerminationApplicationDTO>(draft);
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
                    result = new BuyerTerminationApplicationDTO
                    {
                        IsDraft = true,
                        IsPaid = applicationService.IsApplicationPaid(applicationId),
                        HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };

                    if (result.IsPaid && result.PaymentInformation == null)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
            }
            else
            {
                result = new BuyerTerminationApplicationDTO
                {
                    ApplicationId = applicationId,
                    BuyerId = changes[0].BuyerId.Value
                };

                result.BuyerUrorrNumber = (from buyer in Db.BuyerRegisters
                                           where buyer.Id == result.BuyerId
                                           select buyer.UrrorNum).First();

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

                if (result.HasDelivery.Value)
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

        public RegixChecksWrapperDTO<BuyerTerminationRegixDataDTO> GetBuyerTerminationRegixData(int applicationId)
        {
            PageCodeEnum pageCode = (from appl in Db.Applications
                                     join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                     where appl.Id == applicationId
                                     select Enum.Parse<PageCodeEnum>(applType.PageCode)).First();

            RegixChecksWrapperDTO<BuyerTerminationRegixDataDTO> result = new RegixChecksWrapperDTO<BuyerTerminationRegixDataDTO>
            {
                DialogDataModel = pageCode == PageCodeEnum.TermFirstSaleBuyer
                                    ? GetBuyerApplicationTerminationRegixData(applicationId)
                                    : GetFirstSaleCenterApplicationTerminationRegixData(applicationId),
                RegiXDataModel = pageCode == PageCodeEnum.TermFirstSaleBuyer
                                    ? regixApplicationService.GetBuyerTerminationChecks(applicationId)
                                    : regixApplicationService.GetFirstSaleCenterTerminationChecks(applicationId)
            };

            result.DialogDataModel.PageCode = pageCode;
            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public BuyerTerminationRegixDataDTO GetBuyerApplicationTerminationRegixData(int applicationId)
        {
            BuyerTerminationRegixDataDTO regixData = new BuyerTerminationRegixDataDTO
            {
                ApplicationId = applicationId,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId)
            };

            return regixData;
        }

        public BuyerTerminationRegixDataDTO GetFirstSaleCenterApplicationTerminationRegixData(int applicationId)
        {
            BuyerTerminationRegixDataDTO regixData = new BuyerTerminationRegixDataDTO
            {
                ApplicationId = applicationId,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId)
            };

            return regixData;
        }

        public int AddBuyerTerminationApplication(BuyerTerminationApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null)
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

                int buyerId = GetBuyerIdByUrorrOrThrow(application.IsOnline.Value, application.PageCode.Value, application.BuyerUrorrNumber, application.BuyerId);

                List<ChangeOfCircumstancesDTO> changes = GetTermBuyerChangeOfCircumstances(buyerId, application.PageCode.Value, application.DeregistrationReason);
                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, changes, null, buyerId: buyerId);

                AddBuyerTerminationDeliveryData(dbApplication, application);

                scope.Complete();
            }

            List<FileInfoDTO> aquacultureFiles = application.Files;
            application.Files = null;
            applicationStateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), aquacultureFiles, nextManualStatus);

            return dbApplication.Id;
        }

        public void EditBuyerTerminationApplication(BuyerTerminationApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null)
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

                int buyerId = GetBuyerIdByUrorrOrThrow(application.IsOnline.Value, application.PageCode.Value, application.BuyerUrorrNumber, application.BuyerId);

                List<ChangeOfCircumstancesDTO> changes = GetTermBuyerChangeOfCircumstances(buyerId, application.PageCode.Value, application.DeregistrationReason);
                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, changes, null, buyerId: buyerId);

                EditBuyerTerminationDeliveryData(dbApplication, application);

                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            applicationStateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, nextManualStatus, application.StatusReason);
        }

        public void EditBuyerTerminationRegixData(BuyerTerminationRegixDataDTO application)
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

            applicationStateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public BuyerEditDTO GetBuyerFromTerminationApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            BuyerEditDTO result = GetRegisterEntry(changes[0].BuyerId.Value);
            return result;
        }

        private List<ChangeOfCircumstancesDTO> GetTermBuyerChangeOfCircumstances(int buyerId, PageCodeEnum pageCode, string description)
        {
            DateTime now = DateTime.Now;

            int typeId = (from type in Db.NchangeOfCircumstancesTypes
                          where type.Code == pageCode.ToString()
                                && type.ValidFrom <= now
                                && type.ValidTo > now
                          select type.Id).Single();

            ChangeOfCircumstancesDTO change = new ChangeOfCircumstancesDTO
            {
                TypeId = typeId,
                DataType = ChangeOfCircumstancesDataTypeEnum.FreeText,
                BuyerId = buyerId,
                Description = description
            };

            return new List<ChangeOfCircumstancesDTO> { change };
        }

        private void AddBuyerTerminationDeliveryData(Application dbApplication, BuyerTerminationApplicationDTO application)
        {
            if (application.HasDelivery.Value)
            {
                dbApplication.Delivery = Db.AddDeliveryData(application.DeliveryData);
            }
        }

        private void EditBuyerTerminationDeliveryData(Application dbApplication, BuyerTerminationApplicationDTO application)
        {
            if (application.HasDelivery.Value)
            {
                dbApplication.Delivery = Db.EditDeliveryData(application.DeliveryData, dbApplication.DeliveryId.Value);
            }
        }
    }
}
