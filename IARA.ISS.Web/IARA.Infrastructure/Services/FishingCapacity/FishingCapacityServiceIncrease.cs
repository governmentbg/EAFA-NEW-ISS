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
using IARA.DomainModels.DTOModels.FishingCapacity.IncreaseCapacity;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public partial class FishingCapacityService : Service, IFishingCapacityService
    {
        private readonly static int CERTIFICATE_VALIDITY_YEARS = 3;

        public IncreaseFishingCapacityApplicationDTO GetIncreaseFishingCapacityApplication(int applicationId)
        {
            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            IncreaseFishingCapacityApplicationDTO result = null;

            if (change == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<IncreaseFishingCapacityApplicationDTO>(draft);
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
                    result = new IncreaseFishingCapacityApplicationDTO
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
                result = new IncreaseFishingCapacityApplicationDTO
                {
                    ApplicationId = applicationId,
                    IncreaseGrossTonnageBy = change.GrossTonnageChange,
                    IncreasePowerBy = change.PowerChange
                };

                result.ShipId = (from cap in Db.ShipCapacityRegister
                                 where cap.Id == change.ShipCapacityId.Value
                                 select cap.ShipId).First();

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);

                result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                result.IsDraft = false;
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                result.AcquiredCapacity = GetAcquiredFishingCapacity(change.AcquiredFishingCapacityId.Value);
                result.RemainingCapacityAction = GetCapacityFreeActionsFromChangeHistory(change);
                result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);

                if (result.IsPaid)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                if (result.HasDelivery && result.RemainingCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<IncreaseFishingCapacityRegixDataDTO> GetIncreaseFishingCapacityRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<IncreaseFishingCapacityRegixDataDTO> result = new RegixChecksWrapperDTO<IncreaseFishingCapacityRegixDataDTO>
            {
                DialogDataModel = GetApplicationIncreaseFishingCapacityRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetFishingCapacityIncreaseChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public IncreaseFishingCapacityDataDTO GetCapacityDataFromIncreaseCapacityApplication(int applicationId)
        {
            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);

            int shipUId = (from cap in Db.ShipCapacityRegister
                           join ship in Db.ShipsRegister on cap.ShipId equals ship.Id
                           where cap.Id == change.ShipCapacityId.Value
                           select ship.ShipUid).First();

            DateTime now = DateTime.Now;

            var currentCapacity = (from ship in Db.ShipsRegister
                                   where ship.ShipUid == shipUId
                                        && ship.ValidFrom <= now
                                        && ship.ValidTo > now
                                   select new
                                   {
                                       ship.GrossTonnage,
                                       ship.MainEnginePower
                                   }).First();

            IncreaseFishingCapacityDataDTO data = new IncreaseFishingCapacityDataDTO
            {
                NewTonnage = currentCapacity.GrossTonnage + change.GrossTonnageChange.Value,
                NewPower = currentCapacity.MainEnginePower + change.PowerChange.Value
            };

            return data;
        }

        public IncreaseFishingCapacityRegixDataDTO GetApplicationIncreaseFishingCapacityRegixData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetApplicationDataIds(applicationId);

            IncreaseFishingCapacityRegixDataDTO regixData = new IncreaseFishingCapacityRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode,
                SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId),
                SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId),
            };

            CapacityChangeHistoryDTO change = GetCapacityChangeHistory(applicationId, RecordTypesEnum.Application);
            regixData.RemainingCapacityAction = GetCapacityFreeActionsRegixFromChangeHistory(change);

            return regixData;
        }

        public int AddIncreaseFishingCapacityApplication(IncreaseFishingCapacityApplicationDTO application, ApplicationStatusesEnum? nextManualStatus)
        {
            return AddIncreaseFishingCapacityApplication(application, RecordTypesEnum.Application, nextManualStatus);
        }

        public void EditIncreaseFishingCapacityApplication(IncreaseFishingCapacityApplicationDTO application,
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

                EditIncreaseCapacityChangeHistory(application.ApplicationId.Value,
                                                  application.IncreaseGrossTonnageBy.Value,
                                                  application.IncreasePowerBy.Value,
                                                  application.AcquiredCapacity,
                                                  application.RemainingCapacityAction);

                Db.EditDeliveryData(dbApplication, application);

                Db.SaveChanges();
                scope.Complete();
            }

            List<FileInfoDTO> files = application.Files;
            application.Files = null;
            stateMachine.Act(application.ApplicationId.Value, CommonUtils.Serialize(application), files, manualStatus, application.StatusReason);
        }

        public void EditIncreaseFishingCapacityRegixData(IncreaseFishingCapacityRegixDataDTO application)
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

                EditIncreaseCapacityChangeHistory(application.ApplicationId.Value, application.RemainingCapacityAction);
                Db.SaveChanges();

                scope.Complete();
            }

            stateMachine.Act(application.ApplicationId.Value, ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public void CompleteIncreaseFishingCapacityApplication(IncreaseFishingCapacityApplicationDTO application)
        {
            AddIncreaseFishingCapacityApplication(application, RecordTypesEnum.Register);
        }

        public void AddIncreaseCapacityChangeHistory(RecordTypesEnum recordType,
                                                     int applicationId,
                                                     int shipId,
                                                     decimal tonnageIncrease,
                                                     decimal powerIncrease,
                                                     AcquiredFishingCapacityDTO acquiredCapacity,
                                                     FishingCapacityFreedActionsDTO remainingCapacityActions)
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

            var shipData = (from ship in Db.ShipsRegister
                            where ship.Id == shipId
                            select new
                            {
                                ship.ShipUid,
                                ship.GrossTonnage,
                                ship.MainEnginePower
                            }).First();

            AcquiredCapacityRegister acquired = AddAcquiredFishingCapacity(acquiredCapacity);

            CapacityChangeHistory entry = new CapacityChangeHistory
            {
                ApplicationId = applicationId,
                RecordType = recordType.ToString(),
                RegisterApplicationId = registerApplicationId,
                TypeOfChange = nameof(FishingCapacityChangeTypeEnum.Increase),
                GrossTonnageChange = tonnageIncrease,
                PowerChange = powerIncrease,
                DateOfChange = now,
                AcquiredFishingCapacity = acquired
            };

            if (recordType == RecordTypesEnum.Application)
            {
                entry.ShipCapacity = new ShipCapacityRegister
                {
                    RecordType = recordType.ToString(),
                    ShipId = shipId,
                    GrossTonnage = shipData.GrossTonnage + tonnageIncrease,
                    EnginePower = shipData.MainEnginePower + powerIncrease,
                    ValidFrom = now,
                    ValidTo = now
                };
            }
            else if (recordType == RecordTypesEnum.Register)
            {
                List<int> shipIds = (from sh in Db.ShipsRegister
                                     where sh.RecordType == nameof(RecordTypesEnum.Register)
                                        && sh.ShipUid == shipData.ShipUid
                                     select sh.Id).ToList();

                ShipCapacityRegister latest = (from cap in Db.ShipCapacityRegister
                                               where cap.RecordType == nameof(RecordTypesEnum.Register)
                                                    && shipIds.Contains(cap.ShipId)
                                                    && cap.ValidFrom <= now
                                                    && cap.ValidTo > now
                                               select cap).FirstOrDefault();

                if (latest != null)
                {
                    latest.ValidTo = now;
                }

                entry.ShipCapacity = new ShipCapacityRegister
                {
                    RecordType = recordType.ToString(),
                    ShipId = shipId,
                    GrossTonnage = shipData.GrossTonnage, // тонажът е преизчислен предварително при MOD на кораба
                    EnginePower = shipData.MainEnginePower, // мощността е преизчислена предварително при MOD на кораба
                    ValidFrom = now,
                    ValidTo = DefaultConstants.MAX_VALID_DATE
                };
            }

            Db.CapacityChangeHistory.Add(entry);
            Db.SaveChanges();

            // изчисляваме остатъка от капацитета и издаваме удостоверение, ако има нужда
            decimal tonnageRemaining = 0;
            decimal powerRemaining = 0;
            DateTime certificateValidTo = now.AddYears(CERTIFICATE_VALIDITY_YEARS);

            if (acquired.AcquiredType == nameof(AcquiredCapacityMannerEnum.Ranking))
            {
                tonnageRemaining = acquired.GrossTonnage.Value - tonnageIncrease;
                powerRemaining = acquired.EnginePower.Value - powerIncrease;
            }
            else if (acquired.AcquiredType == nameof(AcquiredCapacityMannerEnum.FreeCapLicence))
            {
                List<CapacityCertificatesRegister> certificates = (from cert in Db.CapacityCertificatesRegister
                                                                   where acquiredCapacity.CapacityLicenceIds.Contains(cert.Id)
                                                                   select cert).ToList();

                decimal sumTonnage = certificates.Sum(x => x.GrossTonnage);
                decimal sumPower = certificates.Sum(x => x.MainEnginePower);

                tonnageRemaining = sumTonnage - tonnageIncrease;
                powerRemaining = sumPower - powerIncrease;

                // при използвани удостоверения, ново удостоверение за остатъка от капацитета се издава със същата валидност
                // или най-благоприятната валидност, ако са използвани повече от две удостоверения
                certificateValidTo = certificates.Max(x => x.CertificateValidTo);

                // инвалидираме използваните удостоверения при регистров запис 
                if (recordType == RecordTypesEnum.Register)
                {
                    foreach (CapacityCertificatesRegister certificate in certificates)
                    {
                        certificate.CertificateValidTo = now;
                    }
                }
            }

            tonnageRemaining = tonnageRemaining < 0 ? 0 : tonnageRemaining;
            powerRemaining = powerRemaining < 0 ? 0 : powerRemaining;

            // издаване на ново удостоверение за остатъка от капацитета
            if (tonnageRemaining != 0 || powerRemaining != 0)
            {
                if (remainingCapacityActions == null)
                {
                    throw new ArgumentException("No actions provided for remaining fishing capacity (Increase)");
                }

                switch (remainingCapacityActions.Action)
                {
                    case FishingCapacityRemainderActionEnum.NoCertificate:
                        // не се издава удостоверение за остатъка от капацитета
                        break;
                    case FishingCapacityRemainderActionEnum.Certificate:
                        // издава се удостоверение за остатъка от капацитета на получателя на заявлението
                        AddCapacityCertificateForSubmittedFor(applicationId, entry, tonnageRemaining, powerRemaining, now, certificateValidTo);
                        break;
                    case FishingCapacityRemainderActionEnum.Transfer:
                        // издават се удостоверения за остатъка от капацитета на трети лица
                        AddCapacityCertificateForHolders(recordType, entry, remainingCapacityActions.Holders, now, certificateValidTo);
                        break;
                }
            }

            Db.SaveChanges();
            scope.Complete();
        }

        public bool EditIncreaseCapacityChangeHistory(int applicationId,
                                                      decimal tonnageIncrease,
                                                      decimal powerIncrease,
                                                      AcquiredFishingCapacityDTO acquiredCapacity,
                                                      FishingCapacityFreedActionsDTO remainingCapacityActions,
                                                      bool isActive = true)
        {
            DateTime now = DateTime.Now;

            CapacityChangeHistory change = (from ch in Db.CapacityChangeHistory
                                            where ch.ApplicationId == applicationId
                                            select ch).FirstOrDefault();

            if (change == null)
            {
                return false;
            }

            // промяната няма осигурен капацитет, няма какво да се редактира
            if (change.AcquiredFishingCapacityId == null)
            {
                return false;
            }

            using TransactionScope scope = new TransactionScope();

            change.GrossTonnageChange = tonnageIncrease;
            change.PowerChange = powerIncrease;

            // изчисляваме остатъка от капацитета и издаваме удостоверение, ако има нужда
            decimal tonnageRemaining = 0;
            decimal powerRemaining = 0;
            DateTime certificateValidTo = now.AddYears(CERTIFICATE_VALIDITY_YEARS);

            AcquiredCapacityRegister acquired = (from acq in Db.AcquiredCapacityRegister
                                                 where acq.Id == change.AcquiredFishingCapacityId.Value
                                                 select acq).First();

            // редактираме осигурения капацитет
            if (acquiredCapacity != null)
            {
                List<AcquiredCapacityCertificate> dbCerts = (from cert in Db.AcquiredCapacityCertificates
                                                             where cert.AcquiredCapacityId == acquired.Id
                                                             select cert).ToList();

                acquired.AcquiredType = acquiredCapacity.AcquiredManner.ToString();

                foreach (AcquiredCapacityCertificate dbCert in dbCerts)
                {
                    dbCert.IsActive = false;
                }

                if (acquiredCapacity.AcquiredManner.Value == AcquiredCapacityMannerEnum.Ranking)
                {
                    acquired.GrossTonnage = acquiredCapacity.GrossTonnage.Value;
                    acquired.EnginePower = acquiredCapacity.Power.Value;

                    tonnageRemaining = acquired.GrossTonnage.Value - change.GrossTonnageChange.Value;
                    powerRemaining = acquired.EnginePower.Value - change.PowerChange.Value;
                }
                else if (acquiredCapacity.AcquiredManner.Value == AcquiredCapacityMannerEnum.FreeCapLicence)
                {
                    acquired.GrossTonnage = null;
                    acquired.EnginePower = null;

                    foreach (int certificateId in acquiredCapacity.CapacityLicenceIds)
                    {
                        AcquiredCapacityCertificate dbCert = dbCerts.Where(x => x.CapacityCertificateId == certificateId).FirstOrDefault();

                        if (dbCert != null)
                        {
                            dbCert.IsActive = true;
                        }
                        else
                        {
                            AcquiredCapacityCertificate entry = new AcquiredCapacityCertificate
                            {
                                AcquiredCapacity = acquired,
                                CapacityCertificateId = certificateId
                            };

                            Db.AcquiredCapacityCertificates.Add(entry);
                        }
                    }

                    List<CapacityCertificatesRegister> certificates = (from cert in Db.CapacityCertificatesRegister
                                                                       where acquiredCapacity.CapacityLicenceIds.Contains(cert.Id)
                                                                       select cert).ToList();

                    decimal sumTonnage = certificates.Sum(x => x.GrossTonnage);
                    decimal sumPower = certificates.Sum(x => x.MainEnginePower);

                    tonnageRemaining = sumTonnage - change.GrossTonnageChange.Value;
                    powerRemaining = sumPower - change.PowerChange.Value;

                    // при използвани удостоверения, ново удостоверение за остатъка от капацитета се издава със същата валидност
                    // или най-благоприятната валидност, ако са използвани повече от две удостоверения
                    certificateValidTo = certificates.Max(x => x.CertificateValidTo);
                }
            }

            acquired.IsActive = isActive;
            change.IsActive = isActive;

            // редактираме необходимите действия с остатъка от капацитета
            if (remainingCapacityActions != null)
            {
                // издаваме удостоверение на получателя на заявлението
                if (remainingCapacityActions.Action == FishingCapacityRemainderActionEnum.Certificate)
                {
                    EditCapacityCertificateForSubmittedFor(change, tonnageRemaining, powerRemaining, now, certificateValidTo);
                }
                // издаваме удостоверения на трети лица
                else if (remainingCapacityActions.Action == FishingCapacityRemainderActionEnum.Transfer)
                {
                    EditCapacityCertificatesForHolders(applicationId, change, remainingCapacityActions.Holders, now, certificateValidTo);
                }
            }

            Db.SaveChanges();

            scope.Complete();
            return true;
        }

        public void EditIncreaseCapacityChangeHistory(int applicationId, FishingCapacityFreedActionsRegixDataDTO remainingCapacityActions)
        {
            if (remainingCapacityActions.Action == FishingCapacityRemainderActionEnum.Transfer)
            {
                EditCapacityCertificatesForHoldersRegix(applicationId, remainingCapacityActions.Holders);
                Db.SaveChanges();
            }
        }

        /// <summary>
        /// Adds increase fishing capacity application/register entry
        /// </summary>
        /// <param name="application">DTO with data for increasing</param>
        /// <param name="recordType">Record type Application/Register</param>
        /// <param name="nextManualStatus">Next manual status for the state machine - needed for recod types Application</param>
        /// <returns></returns>
        private int AddIncreaseFishingCapacityApplication(IncreaseFishingCapacityApplicationDTO application,
                                                          RecordTypesEnum recordType,
                                                          ApplicationStatusesEnum? nextManualStatus = null)
        {
            Application dbApplication;

            using (TransactionScope scope = new TransactionScope())
            {
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

                DateTime now = DateTime.Now;

                int shipUid = (from sh in Db.ShipsRegister
                               where sh.Id == application.ShipId.Value
                               select sh.ShipUid).First();

                int latestShipId = (from sh in Db.ShipsRegister
                                    where sh.ShipUid == shipUid
                                       && sh.ValidFrom <= now
                                       && sh.ValidTo > now
                                    select sh.Id).First();

                AddIncreaseCapacityChangeHistory(recordType,
                                                 application.ApplicationId.Value,
                                                 latestShipId,
                                                 application.IncreaseGrossTonnageBy.Value,
                                                 application.IncreasePowerBy.Value,
                                                 application.AcquiredCapacity,
                                                 application.RemainingCapacityAction);

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

        private BaseRegixApplicationDataIds GetApplicationDataIds(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = (from appl in Db.Applications
                                                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                                        where appl.Id == applicationId
                                                        select new BaseRegixApplicationDataIds
                                                        {
                                                            ApplicationId = applicationId,
                                                            PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                                                        }).First();

            return regixDataIds;
        }

        // Добавяне на AcquiredCapacityRegister запис
        private AcquiredCapacityRegister AddAcquiredFishingCapacity(AcquiredFishingCapacityDTO capacity)
        {
            AcquiredCapacityRegister entry = new AcquiredCapacityRegister
            {
                AcquiredType = capacity.AcquiredManner.ToString()
            };

            Db.AcquiredCapacityRegister.Add(entry);

            if (capacity.AcquiredManner == AcquiredCapacityMannerEnum.Ranking)
            {
                entry.GrossTonnage = capacity.GrossTonnage.Value;
                entry.EnginePower = capacity.Power.Value;
            }
            else if (capacity.AcquiredManner == AcquiredCapacityMannerEnum.FreeCapLicence)
            {
                AddAcquiredCapacityCertificates(entry, capacity.CapacityLicenceIds);
            }

            return entry;
        }

        // Добавяне на AcquiredCapacityCertificate записи към AcquiredCapacityRegister
        private void AddAcquiredCapacityCertificates(AcquiredCapacityRegister acquired, List<int> certificateIds)
        {
            foreach (int certificateId in certificateIds)
            {
                AcquiredCapacityCertificate acqCert = new AcquiredCapacityCertificate
                {
                    AcquiredCapacity = acquired,
                    CapacityCertificateId = certificateId
                };

                Db.AcquiredCapacityCertificates.Add(acqCert);
            }
        }
    }
}
