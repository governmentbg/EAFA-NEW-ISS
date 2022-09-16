using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;
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
        public ShipDeregistrationApplicationDTO GetShipDeregistrationApplication(int applicationId)
        {
            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);

            ShipDeregistrationApplicationDTO result = null;

            if (changes.Count == 0)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<ShipDeregistrationApplicationDTO>(draft);
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
                    result = new ShipDeregistrationApplicationDTO
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
                result = new ShipDeregistrationApplicationDTO
                {
                    ApplicationId = applicationId,
                    ShipId = changes[0].ShipId.Value
                };

                FleetTypeNomenclatureDTO fleet = GetFleetTypeByShipId(result.ShipId.Value);

                if (fleet.HasFishingCapacity)
                {
                    CapacityChangeHistoryDTO change = fishingCapacityService.GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);
                    result.FreedCapacityAction = fishingCapacityService.GetCapacityFreeActionsFromChangeHistory(change);
                }

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);
                result.IsDraft = false;
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
                result.DeregistrationReason = changes[0].Description;
                result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);

                result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                if (result.HasDelivery && fleet.HasFishingCapacity && result.FreedCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<ShipDeregistrationRegixDataDTO> GetShipDeregistrationRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<ShipDeregistrationRegixDataDTO> result = new RegixChecksWrapperDTO<ShipDeregistrationRegixDataDTO>
            {
                DialogDataModel = GetApplicationDeregistrationData(applicationId),
                RegiXDataModel = regixApplicationService.GetShipDeregistrationChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public ShipDeregistrationRegixDataDTO GetApplicationDeregistrationData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetRegixDataIds(applicationId);

            ShipDeregistrationRegixDataDTO regixData = new ShipDeregistrationRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId)
            };

            List<ChangeOfCircumstancesDTO> changes = changeOfCircumstancesService.GetChangeOfCircumstances(applicationId);
            FleetTypeNomenclatureDTO fleet = GetFleetTypeByShipId(changes[0].ShipId.Value);

            if (fleet.HasFishingCapacity)
            {
                CapacityChangeHistoryDTO change = fishingCapacityService.GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);
                regixData.FreedCapacityAction = fishingCapacityService.GetCapacityFreeActionsRegixFromChangeHistory(change);
            }

            return regixData;
        }

        public int AddShipDeregistrationApplication(ShipDeregistrationApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null)
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

                FleetTypeNomenclatureDTO fleet = GetFleetTypeByShipId(application.ShipId.Value);
                if (fleet.HasFishingCapacity)
                {
                    ShipCapacityRegister latestShipCapacity = GetLatestShipCapacity(application.ShipId.Value);

                    fishingCapacityService.AddReduceCapacityChangeHistory(RecordTypesEnum.Application,
                                                                          application.ApplicationId.Value,
                                                                          latestShipCapacity.Id,
                                                                          latestShipCapacity.GrossTonnage,
                                                                          latestShipCapacity.EnginePower,
                                                                          application.FreedCapacityAction);
                    Db.SaveChanges();
                }

                List<ChangeOfCircumstancesDTO> changes = GetDeregShipChangeOfCircumstances(application.ShipId.Value, application.DeregistrationReason);
                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, changes, application.ShipId);

                Db.AddDeliveryData(dbApplication, application);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> shipFiles = application.Files;
            application.Files = null;
            stateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), shipFiles, nextManualStatus);

            return dbApplication.Id;
        }

        public void EditShipDeregistrationApplication(ShipDeregistrationApplicationDTO application, ApplicationStatusesEnum? manualStatus = null)
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

                FleetTypeNomenclatureDTO fleet = GetFleetTypeByShipId(application.ShipId.Value);
                if (fleet.HasFishingCapacity)
                {
                    ShipCapacityRegister latestShipCapacity = GetLatestShipCapacity(application.ShipId.Value);

                    fishingCapacityService.EditReduceCapacityChangeHistory(application.ApplicationId.Value,
                                                                           latestShipCapacity.GrossTonnage,
                                                                           latestShipCapacity.EnginePower,
                                                                           application.FreedCapacityAction);
                    Db.SaveChanges();
                }

                List<ChangeOfCircumstancesDTO> changes = GetDeregShipChangeOfCircumstances(application.ShipId.Value, application.DeregistrationReason);
                changeOfCircumstancesService.AddOrEditChangeOfCircumstances(application.ApplicationId.Value, changes, application.ShipId);

                Db.EditDeliveryData(dbApplication, application);
                Db.SaveChanges();

                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditShipDeregistrationRegixData(ShipDeregistrationRegixDataDTO application)
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

                if (application.FreedCapacityAction != null)
                {
                    fishingCapacityService.EditReduceCapacityChangeHistory(application.ApplicationId.Value, application.FreedCapacityAction);
                    Db.SaveChanges();
                }

                scope.Complete();
            }

            stateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);

        }

        public void CompleteShipDeregistrationApplication(ShipRegisterDeregistrationDTO ships)
        {
            DateTime now = DateTime.Now;

            using (TransactionScope scope = new TransactionScope())
            {
                foreach (ShipRegisterEditDTO ship in ships.Ships)
                {
                    EditShip(ship, ships.ApplicationId.Value, ship.ShipUID);
                }

                List<int> shipIds = (from ship in Db.ShipsRegister
                                     where ship.ShipUid == ships.Ships[0].ShipUID.Value
                                     select ship.Id).ToList();

                FleetTypeNomenclatureDTO fleet = GetFleetTypeByShipId(shipIds[0]);
                if (fleet.HasFishingCapacity)
                {
                    ShipCapacityRegister latestShipCapacity = (from cap in Db.ShipCapacityRegister
                                                               where shipIds.Contains(cap.ShipId)
                                                                  && cap.ValidFrom <= now
                                                                  && cap.ValidTo > now
                                                               select cap).First();

                    CapacityChangeHistoryDTO change = fishingCapacityService.GetCapacityChangeHistory(ships.ApplicationId.Value, RecordTypesEnum.Application);
                    FishingCapacityFreedActionsDTO action = fishingCapacityService.GetCapacityFreeActionsFromChangeHistory(change);

                    fishingCapacityService.AddReduceCapacityChangeHistory(RecordTypesEnum.Register,
                                                                          ships.ApplicationId.Value,
                                                                          latestShipCapacity.Id,
                                                                          latestShipCapacity.GrossTonnage,
                                                                          latestShipCapacity.EnginePower,
                                                                          action);
                    Db.SaveChanges();
                }

                scope.Complete();
            }

            stateMachine.Act(ships.ApplicationId.Value);
        }

        public ShipRegisterEditDTO GetShipFromDeregistrationApplication(int applicationId)
        {
            int shipId = (from appl in Db.Applications
                          join change in Db.ApplicationChangeOfCircumstances on appl.Id equals change.ApplicationId
                          where appl.Id == applicationId
                          select change.ShipId.Value).First();

            return GetShip(shipId);
        }

        private List<ChangeOfCircumstancesDTO> GetDeregShipChangeOfCircumstances(int shipId, string description)
        {
            DateTime now = DateTime.Now;

            ChangeOfCircumstancesDTO change = new ChangeOfCircumstancesDTO
            {
                TypeId = (from type in Db.NchangeOfCircumstancesTypes
                          where type.Code == nameof(PageCodeEnum.DeregShip)
                                && type.ValidFrom <= now
                                && type.ValidTo > now
                          select type.Id).First(),
                DataType = ChangeOfCircumstancesDataTypeEnum.Ship,
                ShipId = shipId,
                Description = description
            };

            return new List<ChangeOfCircumstancesDTO> { change };
        }

        private ShipCapacityRegister GetLatestShipCapacity(int shipId)
        {
            DateTime now = DateTime.Now;

            int shipUid = (from ship in Db.ShipsRegister
                           where ship.Id == shipId
                           select ship.ShipUid).First();

            List<int> shipIds = (from ship in Db.ShipsRegister
                                 where ship.ShipUid == shipUid
                                 select ship.Id).ToList();

            ShipCapacityRegister latestShipCapacity = (from cap in Db.ShipCapacityRegister
                                                       where shipIds.Contains(cap.ShipId)
                                                          && cap.ValidFrom <= now
                                                          && cap.ValidTo > now
                                                       select cap).First();

            return latestShipCapacity;
        }
    }
}
