using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class FishingCapacityService : Service, IFishingCapacityService
    {
        public ReduceFishingCapacityApplicationDTO GetReduceFishingCapacityApplication(int applicationId)
        {
            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            ReduceFishingCapacityApplicationDTO result = null;

            if (change == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<ReduceFishingCapacityApplicationDTO>(draft);
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
                    result = new ReduceFishingCapacityApplicationDTO
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
                result = new ReduceFishingCapacityApplicationDTO
                {
                    ApplicationId = applicationId,
                    ReduceGrossTonnageBy = change.GrossTonnageChange,
                    ReducePowerBy = change.PowerChange
                };

                result.FreedCapacityAction = GetCapacityFreeActionsFromChangeHistory(change);

                result.ShipId = (from cap in Db.ShipCapacityRegister
                                 where cap.Id == change.ShipCapacityId.Value
                                 select cap.ShipId).First();

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

                if (result.HasDelivery && result.FreedCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<ReduceFishingCapacityRegixDataDTO> GetReduceFishingCapacityRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<ReduceFishingCapacityRegixDataDTO> result = new RegixChecksWrapperDTO<ReduceFishingCapacityRegixDataDTO>
            {
                DialogDataModel = GetApplicationReduceRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetFishingCapacityReduceChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public ReduceFishingCapacityDataDTO GetCapacityDataFromReduceCapacityApplication(int applicationId)
        {
            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            int shipUid = (from cap in Db.ShipCapacityRegister
                           join ship in Db.ShipsRegister on cap.ShipId equals ship.Id
                           where cap.Id == change.ShipCapacityId.Value
                           select ship.ShipUid).First();

            DateTime now = DateTime.Now;

            var currentCapacity = (from ship in Db.ShipsRegister
                                   where ship.ShipUid == shipUid
                                        && ship.ValidFrom <= now
                                        && ship.ValidTo > now
                                   select new
                                   {
                                       ship.GrossTonnage,
                                       ship.MainEnginePower
                                   }).First();

            ReduceFishingCapacityDataDTO data = new ReduceFishingCapacityDataDTO
            {
                NewTonnage = currentCapacity.GrossTonnage - change.GrossTonnageChange.Value,
                NewPower = currentCapacity.MainEnginePower - change.PowerChange.Value
            };

            return data;
        }

        public ReduceFishingCapacityRegixDataDTO GetApplicationReduceRegixData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetApplicationDataIds(applicationId);

            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            ReduceFishingCapacityRegixDataDTO regixData = new ReduceFishingCapacityRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId),
                FreedCapacityAction = GetCapacityFreeActionsRegixFromChangeHistory(change)
            };

            return regixData;
        }

        public int AddReduceFishingCapacityApplication(ReduceFishingCapacityApplicationDTO application, ApplicationStatusesEnum? nextManualStatus)
        {
            return AddReduceFishingCapacityApplication(application, RecordTypesEnum.Application, nextManualStatus);
        }

        public void EditReduceFishingCapacityApplication(ReduceFishingCapacityApplicationDTO application, ApplicationStatusesEnum? manualStatus = null)
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

                EditReduceCapacityChangeHistory(application.ApplicationId.Value,
                                                application.ReduceGrossTonnageBy.Value,
                                                application.ReducePowerBy.Value,
                                                application.FreedCapacityAction);

                Db.EditDeliveryData(dbApplication, application);

                Db.SaveChanges();
                scope.Complete();
            }


            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditReduceFishingCapacityRegixData(ReduceFishingCapacityRegixDataDTO application)
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

                EditReduceCapacityChangeHistory(application.ApplicationId.Value, application.FreedCapacityAction);
                Db.SaveChanges();

                scope.Complete();
            }

            stateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public void CompleteReduceFishingCapacityApplication(ReduceFishingCapacityApplicationDTO application)
        {
            AddReduceFishingCapacityApplication(application, RecordTypesEnum.Register);
        }

        public void AddReduceCapacityChangeHistory(RecordTypesEnum recordType,
                                                   int? applicationId,
                                                   int latestShipCapacityId,
                                                   decimal tonnageDecrease,
                                                   decimal powerDecrease,
                                                   FishingCapacityFreedActionsDTO remainingCapacityActions)
        {
            ShipCapacityRegister latestShipCapacity = (from cap in Db.ShipCapacityRegister
                                                       where cap.Id == latestShipCapacityId
                                                       select cap).First();

            AddReduceCapacityChangeHistory(recordType, applicationId, latestShipCapacity, tonnageDecrease, powerDecrease, remainingCapacityActions);
        }

        public void EditReduceCapacityChangeHistory(int applicationId,
                                                    decimal tonnageDecrease,
                                                    decimal powerDecrease,
                                                    FishingCapacityFreedActionsDTO remainingCapacityActions)
        {
            using TransactionScope scope = new TransactionScope();

            DateTime now = DateTime.Now;

            CapacityChangeHistory change = (from ch in Db.CapacityChangeHistory
                                            where ch.ApplicationId == applicationId
                                            select ch).First();

            change.GrossTonnageChange = tonnageDecrease;
            change.PowerChange = powerDecrease;

            // издаваме удостоверение на получателя на заявлението
            if (remainingCapacityActions.Action == FishingCapacityRemainderActionEnum.Certificate)
            {
                EditCapacityCertificateForSubmittedFor(change, tonnageDecrease, powerDecrease, now, now.AddYears(CERTIFICATE_VALIDITY_YEARS));
            }
            // издаваме удостоверения на трети лица
            else if (remainingCapacityActions.Action == FishingCapacityRemainderActionEnum.Transfer)
            {
                EditCapacityCertificatesForHolders(applicationId, change, remainingCapacityActions.Holders, now, now.AddYears(CERTIFICATE_VALIDITY_YEARS));
            }

            Db.SaveChanges();
            scope.Complete();
        }

        public void EditReduceCapacityChangeHistory(int applicationId, FishingCapacityFreedActionsRegixDataDTO remainingCapacityActions)
        {
            if (remainingCapacityActions.Action == FishingCapacityRemainderActionEnum.Transfer)
            {
                EditCapacityCertificatesForHoldersRegix(applicationId, remainingCapacityActions.Holders);
                Db.SaveChanges();
            }
        }

        /// <summary>
        /// Adds reduce fishing capacity application/register entry
        /// </summary>
        /// <param name="application">DTO with the data for reducing</param>
        /// <param name="recordType">Record type Application/Register</param>
        /// <param name="nextManualStatus">Next manual status for the state machine - needed for record types Application</param>
        /// <returns></returns>
        private int AddReduceFishingCapacityApplication(ReduceFishingCapacityApplicationDTO application,
                                                        RecordTypesEnum recordType,
                                                        ApplicationStatusesEnum? nextManualStatus = null)
        {
            Application dbApplication;

            using (TransactionScope scope = new TransactionScope())
            {
                DateTime now = DateTime.Now;

                if (recordType != RecordTypesEnum.Register)
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

                    Db.AddDeliveryData(dbApplication, application);
                    Db.SaveChanges();
                }

                int shipUid = (from ship in Db.ShipsRegister
                               where ship.Id == application.ShipId
                               select ship.ShipUid).First();

                List<int> shipIds = (from ship in Db.ShipsRegister
                                     where ship.ShipUid == shipUid
                                     select ship.Id).ToList();

                ShipCapacityRegister latestShipCapacity = (from cap in Db.ShipCapacityRegister
                                                           where shipIds.Contains(cap.ShipId)
                                                              && cap.RecordType == nameof(RecordTypesEnum.Register)
                                                              && cap.ValidFrom <= now
                                                              && cap.ValidTo > now
                                                           select cap).First();

                AddReduceCapacityChangeHistory(recordType,
                                               application.ApplicationId.Value,
                                               latestShipCapacity,
                                               application.ReduceGrossTonnageBy.Value,
                                               application.ReducePowerBy.Value,
                                               application.FreedCapacityAction);
                Db.SaveChanges();

                scope.Complete();
            }

            if (recordType == RecordTypesEnum.Application)
            {
                List<FileInfoDTO> aquacultureFiles = application.Files;
                application.Files = null;
                stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), aquacultureFiles, nextManualStatus);
            }

            return application.ApplicationId.Value;
        }

        private void AddReduceCapacityChangeHistory(RecordTypesEnum recordType,
                                                    int? applicationId,
                                                    ShipCapacityRegister latestShipCapacity,
                                                    decimal tonnageDecrease,
                                                    decimal powerDecrease,
                                                    FishingCapacityFreedActionsDTO remainingCapacityActions)
        {
            switch (remainingCapacityActions.Action.Value)
            {
                case FishingCapacityRemainderActionEnum.NoCertificate:
                    AddReduceCapacityChangeHistoryNoCertificate(recordType, applicationId, latestShipCapacity, tonnageDecrease, powerDecrease);
                    break;
                case FishingCapacityRemainderActionEnum.Certificate:
                    AddReduceCapacityChangeHistoryCertificate(recordType, applicationId.Value, latestShipCapacity, tonnageDecrease, powerDecrease);
                    break;
                case FishingCapacityRemainderActionEnum.Transfer:
                    AddReduceCapacityChangeHistoryTransfer(recordType, applicationId.Value, latestShipCapacity, tonnageDecrease, powerDecrease, remainingCapacityActions.Holders);
                    break;
            }
        }

        // Добавяне на CapacityChangeHistory при отписване или намаляване на капацитет без издаване на удостоверение
        private CapacityChangeHistory AddReduceCapacityChangeHistoryNoCertificate(RecordTypesEnum recordType,
                                                                                  int? applicationId,
                                                                                  ShipCapacityRegister latestShipCapacity,
                                                                                  decimal tonnageDecrease,
                                                                                  decimal powerDecrease)
        {
            DateTime now = DateTime.Now;

            int? registerApplicationId = null;

            if (recordType == RecordTypesEnum.Register && applicationId.HasValue)
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
                TypeOfChange = nameof(FishingCapacityChangeTypeEnum.Reduce),
                DateOfChange = now,
                GrossTonnageChange = tonnageDecrease,
                PowerChange = powerDecrease
            };

            if (recordType == RecordTypesEnum.Application)
            {
                entry.ShipCapacity = new ShipCapacityRegister
                {
                    RecordType = recordType.ToString(),
                    ShipId = latestShipCapacity.ShipId,
                    GrossTonnage = latestShipCapacity.GrossTonnage - tonnageDecrease,
                    EnginePower = latestShipCapacity.EnginePower - powerDecrease,
                    ValidFrom = now,
                    ValidTo = now
                };
            }
            else if (recordType == RecordTypesEnum.Register)
            {
                latestShipCapacity.ValidTo = now;

                entry.ShipCapacity = new ShipCapacityRegister
                {
                    RecordType = recordType.ToString(),
                    ShipId = latestShipCapacity.ShipId,
                    GrossTonnage = latestShipCapacity.GrossTonnage - tonnageDecrease,
                    EnginePower = latestShipCapacity.EnginePower - powerDecrease,
                    ValidFrom = now,
                    ValidTo = DefaultConstants.MAX_VALID_DATE
                };
            }

            Db.CapacityChangeHistory.Add(entry);

            return entry;
        }

        // Добавяне на CapacityChangeHistory при отписване или намаляване на капацитет с издаване на удостоверение
        private void AddReduceCapacityChangeHistoryCertificate(RecordTypesEnum recordType,
                                                               int applicationId,
                                                               ShipCapacityRegister latestShipCapacity,
                                                               decimal tonnageDecrease,
                                                               decimal powerDecrease)
        {
            DateTime now = DateTime.Now;

            CapacityChangeHistory change = AddReduceCapacityChangeHistoryNoCertificate(recordType,
                                                                                       applicationId,
                                                                                       latestShipCapacity,
                                                                                       tonnageDecrease,
                                                                                       powerDecrease);

            AddCapacityCertificateForSubmittedFor(applicationId, change, tonnageDecrease, powerDecrease, now, now.AddYears(CERTIFICATE_VALIDITY_YEARS));
        }

        // Добавяне на CapacityChangeHistory при отписване или намаляване на капацитет с издаване на удостоверения към нови лица
        private void AddReduceCapacityChangeHistoryTransfer(RecordTypesEnum recordType,
                                                            int applicationId,
                                                            ShipCapacityRegister latestShipCapacity,
                                                            decimal tonnageDecrease,
                                                            decimal powerDecrease,
                                                            List<FishingCapacityHolderDTO> holders)
        {
            DateTime now = DateTime.Now;

            CapacityChangeHistory change = AddReduceCapacityChangeHistoryNoCertificate(recordType,
                                                                                       applicationId,
                                                                                       latestShipCapacity,
                                                                                       tonnageDecrease,
                                                                                       powerDecrease);

            AddCapacityCertificateForHolders(recordType, change, holders, now, now.AddYears(CERTIFICATE_VALIDITY_YEARS));
        }
    }
}
