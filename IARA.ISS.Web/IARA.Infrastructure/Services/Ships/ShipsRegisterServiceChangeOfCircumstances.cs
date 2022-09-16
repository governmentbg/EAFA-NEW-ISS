using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class ShipsRegisterService : Service, IShipsRegisterService
    {
        public ShipChangeOfCircumstancesApplicationDTO GetShipChangeOfCircumstancesApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            ShipChangeOfCircumstancesApplicationDTO result = null;

            if (changes.Count == 0)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<ShipChangeOfCircumstancesApplicationDTO>(draft);
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
                    result = new ShipChangeOfCircumstancesApplicationDTO
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
                result = new ShipChangeOfCircumstancesApplicationDTO
                {
                    ApplicationId = applicationId,
                    Changes = changes,
                    ShipId = changes[0].ShipId.Value
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
            }

            return result;
        }

        public RegixChecksWrapperDTO<ShipChangeOfCircumstancesRegixDataDTO> GetShipChangeOfCircumstancesRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<ShipChangeOfCircumstancesRegixDataDTO> result = new RegixChecksWrapperDTO<ShipChangeOfCircumstancesRegixDataDTO>
            {
                DialogDataModel = GetApplicationChangeOfCircumstancesRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetShipChangeOfCircumstancesChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public ShipChangeOfCircumstancesRegixDataDTO GetApplicationChangeOfCircumstancesRegixData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetRegixDataIds(applicationId);

            ShipChangeOfCircumstancesRegixDataDTO regixData = new ShipChangeOfCircumstancesRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId),
                Changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId)
            };

            regixData.ShipId = regixData.Changes[0].ShipId.Value;

            return regixData;
        }

        public int AddShipChangeOfCircumstancesApplication(ShipChangeOfCircumstancesApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null)
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

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, application.ShipId);

                Db.AddDeliveryData(dbApplication, application);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> shipFiles = application.Files;
            application.Files = null;
            stateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), shipFiles, nextManualStatus);

            return dbApplication.Id;
        }

        public void EditShipChangeOfCircumstancesApplication(ShipChangeOfCircumstancesApplicationDTO application,
                                                             ApplicationStatusesEnum? manualStatus = null)
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

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, application.ShipId);

                Db.EditDeliveryData(dbApplication, application);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditShipChangeOfCircumstancesRegixData(ShipChangeOfCircumstancesRegixDataDTO application)
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

                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, application.Changes, application.ShipId);

                Db.SaveChanges();
                scope.Complete();
            }

            stateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public void CompleteShipChangeOfCircumstancesApplication(ShipRegisterChangeOfCircumstancesDTO ships)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (ShipRegisterEditDTO ship in ships.Ships)
                {
                    EditShip(ship, ships.ApplicationId.Value, ship.ShipUID);
                }

                scope.Complete();
            }

            stateMachine.Act(ships.ApplicationId.Value);
        }

        public ShipRegisterEditDTO GetShipFromChangeOfCircumstancesApplication(int applicationId)
        {
            int shipId = (from appl in Db.Applications
                          join change in Db.ApplicationChangeOfCircumstances on appl.Id equals change.ApplicationId
                          where appl.Id == applicationId
                          select change.ShipId.Value).First();

            return GetShip(shipId);
        }
    }
}
