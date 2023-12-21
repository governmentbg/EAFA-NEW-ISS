using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.RequestModels;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces.CommercialFishing;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.CommercialFishing
{
    public partial class CommercialFishingService : Service, ICommercialFishingService
    {
        public PermitLicenseFishingGearsApplicationDTO GetPermitLicenseFishingGearsApplication(int applicationId, bool getRegiXData)
        {
            List<FishingGearDTO> fishingGears = fishingGearsService.GetFishingGearsByApplicationId(applicationId);

            PermitLicenseFishingGearsApplicationDTO result = null;

            if (fishingGears.Count == 0)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<PermitLicenseFishingGearsApplicationDTO>(draft);
                    result.IsDraft = true;
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                    result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                    result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                    if (result.IsPaid == true && result.PaymentInformation == null)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
                else
                {
                    result = new PermitLicenseFishingGearsApplicationDTO
                    {
                        IsDraft = true,
                        IsPaid = applicationService.IsApplicationPaid(applicationId),
                        HasDelivery = deliveryService.HasApplicationDelivery(applicationId),
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };

                    if (result.IsPaid == true)
                    {
                        result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                    }
                }
            }
            else
            {
                result = new PermitLicenseFishingGearsApplicationDTO
                {
                    ApplicationId = applicationId,
                    FishingGears = fishingGears,
                    PermitLicenseId = fishingGears[0].PermitId,
                    PermitLicenseNumber = (from permLic in Db.CommercialFishingPermitLicensesRegisters
                                           where permLic.Id == fishingGears[0].PermitId
                                           select permLic.RegistrationNum).First(),
                    ShipId = (from permLic in Db.CommercialFishingPermitLicensesRegisters
                              where permLic.Id == fishingGears[0].PermitId
                              select permLic.ShipId).First()
                };

                result.SubmittedBy = applicationService.GetApplicationSubmittedBy(applicationId);
                result.SubmittedFor = applicationService.GetApplicationSubmittedFor(applicationId);
                result.IsDraft = false;
                result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);

                result.IsPaid = applicationService.IsApplicationPaid(applicationId);
                result.HasDelivery = deliveryService.HasApplicationDelivery(applicationId);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);

                if (getRegiXData)
                {
                    result.RegiXDataModel = regixApplicationService.GetPermitLicenseFishingGearsChecks(applicationId);
                    result.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;
                }

                if (result.IsPaid == true)
                {
                    result.PaymentInformation = applicationService.GetApplicationPaymentInformation(applicationId);
                }

                if (result.HasDelivery == true)
                {
                    result.DeliveryData = deliveryService.GetApplicationDeliveryData(applicationId);
                }
            }

            return result;
        }

        public RegixChecksWrapperDTO<PermitLicenseFishingGearsRegixDataDTO> GetPermitLicenseFishingGearsRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<PermitLicenseFishingGearsRegixDataDTO> result = new()
            {
                DialogDataModel = GetApplicationPermitLicenseFishingGearsRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetPermitLicenseFishingGearsChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public PermitLicenseFishingGearsRegixDataDTO GetApplicationPermitLicenseFishingGearsRegixData(int applicationId)
        {
            BaseRegixApplicationDataIds regixDataIds = GetApplicationPermitLicenseFishingGearsDataIds(applicationId);

            PermitLicenseFishingGearsRegixDataDTO regixData = new PermitLicenseFishingGearsRegixDataDTO
            {
                ApplicationId = applicationId,
                PageCode = regixDataIds.PageCode
            };

            regixData.SubmittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);
            regixData.SubmittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);

            return regixData;
        }

        public int AddPermitLicenseFishingGearsApplication(PermitLicenseFishingGearsApplicationDTO application, bool isFromPublicApp = false, ApplicationStatusesEnum? nextManualStatus = null)
        {
            Application dbApplication;

            using (TransactionScope scope = Db.CreateTransaction())
            {
                dbApplication = (from appl in Db.Applications
                                    .AsSingleQuery()
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

                int? permitLicenseId = application.PermitLicenseId;

                if (isFromPublicApp)
                {
                    permitLicenseId = GetPermitLicenseByRegistrationNumber(application.PermitLicenseNumber, application.ShipId.Value);
                }

                PermitLicensesRegister dbPermitLicense = (from permLic in Db.CommercialFishingPermitLicensesRegisters
                                                          where permLic.Id == permitLicenseId
                                                          select permLic).First();

                List<FishingGearDTO> fishingGears = application.FishingGears.Where(x => x.IsActive == true).ToList();
                AddOrEditApplicationFishingGears(fishingGears, dbPermitLicense, application.ApplicationId);

                Db.AddDeliveryData(dbApplication, application);

                bool isPaid = applicationService.IsApplicationPaid(application.ApplicationId.Value);

                if (isPaid)
                {
                    UpdateAppliedishingGearTariffs(application, dbApplication, true);
                }

                Db.SaveChanges();
                scope.Complete();
            }

            List<FileInfoDTO> fishingGearsFiles = application.Files;
            application.Files = null;
            stateMachine.Act(dbApplication.Id, CommonUtils.Serialize(application), fishingGearsFiles, nextManualStatus);

            return dbApplication.Id;
        }

        public void EditPermitLicenseFishingGearsApplication(PermitLicenseFishingGearsApplicationDTO application,
                                                             bool isFromPublicApp = false,
                                                             ApplicationStatusesEnum? manualStatus = null)
        {
            using (TransactionScope scope = Db.CreateTransaction())
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

                int? permitLicenseId = application.PermitLicenseId;

                if (isFromPublicApp)
                {
                    permitLicenseId = GetPermitLicenseByRegistrationNumber(application.PermitLicenseNumber, application.ShipId.Value);
                }

                PermitLicensesRegister dbPermitLicense = (from permLic in Db.CommercialFishingPermitLicensesRegisters
                                                          where permLic.Id == permitLicenseId
                                                          select permLic).First();

                List<FishingGearDTO> fishingGears = application.FishingGears.Where(x => x.IsActive == true).ToList();
                AddOrEditApplicationFishingGears(fishingGears, dbPermitLicense, application.ApplicationId);

                Db.EditDeliveryData(dbApplication, application);

                bool isPaid = applicationService.IsApplicationPaid(application.ApplicationId.Value);

                if (isPaid)
                {
                    UpdateAppliedishingGearTariffs(application, dbApplication, false);
                }

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

        public void EditPermitLicenseFishingGearsRegixData(PermitLicenseFishingGearsRegixDataDTO application)
        {
            using (TransactionScope scope = Db.CreateTransaction())
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

        public CommercialFishingEditDTO GetPermitLicenseFromFishingGearsApplication(int applicationId)
        {
            List<FishingGearDTO> gears = fishingGearsService.GetFishingGearsByApplicationId(applicationId);

            int permitId = (from permLic in Db.CommercialFishingPermitLicensesRegisters
                            where permLic.Id == gears[0].PermitId
                            select permLic.Id).First();

            CommercialFishingEditDTO result = GetPermitLicense(permitId, false);
            result.FishingGears = gears;

            return result;
        }

        public void CompletePermitLicenseFishingGearsApplication(CommercialFishingEditDTO permitLicense)
        {
            PermitLicensesRegister dbPermitLicense = (from permLic in Db.CommercialFishingPermitLicensesRegisters
                                                      where permLic.Id == permitLicense.Id
                                                      select permLic).First();

            List<FishingGearDTO> fishingGears = permitLicense.FishingGears.Where(x => x.IsActive == true).ToList();

            if (permitLicense.FishingGears != null && permitLicense.FishingGears.Any(x => x.Marks != null && x.Marks.Count > 0))
            {
                UpdateFishingGearMarkStatuses(fishingGears);
            }

            AddOrEditApplicationFishingGears(fishingGears, dbPermitLicense, null, true);

            stateMachine.Act(permitLicense.ApplicationId);
        }

        public async Task<List<CommercialFishingValidationErrorsEnum>> ValidateFishingGearsApplicationData(ApplicationSubmittedForRegixDataDTO submittedFor,
                                                                                                           ApplicationSubmittedByRegixDataDTO submittedBy,
                                                                                                           int shipId,
                                                                                                           int deliveryTypeId,
                                                                                                           string permitLicenseRegistartionNumber = null)
        {
            List<CommercialFishingValidationErrorsEnum> validationErrors = new List<CommercialFishingValidationErrorsEnum>();

            if (await deliveryService.HasSubmittedForEDelivery(deliveryTypeId, submittedFor, submittedBy) == false)
            {
                validationErrors.Add(CommercialFishingValidationErrorsEnum.NoEDeliveryRegistration);
            }

            if (!string.IsNullOrWhiteSpace(permitLicenseRegistartionNumber))
            {
                int? permitId = GetPermitLicenseByRegistrationNumber(permitLicenseRegistartionNumber, shipId);
                if (!permitId.HasValue)
                {
                    validationErrors.Add(CommercialFishingValidationErrorsEnum.InvalidPermitLisenseRegistrationNumber);
                }
            }

            return validationErrors;
        }

        public List<FishingGearDTO> GetFishingGearsByPermitLicenseRegistrationNumber(string permitLicenseNumber, int shipId)
        {
            int? permitLicenseId = GetPermitLicenseByRegistrationNumber(permitLicenseNumber, shipId);

            if (!permitLicenseId.HasValue)
            {
                throw new InvalidPermitLicenseNumberException();
            }

            List<FishingGearDTO> result = fishingGearsService.GetCommercialFishingPermitLicenseFishingGears(permitLicenseId.Value, nameof(RecordTypesEnum.Register));

            return result;
        }

        private BaseRegixApplicationDataIds GetApplicationPermitLicenseFishingGearsDataIds(int applicationId)
        {
            var regixDataIds = (from appl in Db.Applications
                                join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                where appl.Id == applicationId
                                select new BaseRegixApplicationDataIds
                                {
                                    ApplicationId = appl.Id,
                                    PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                                }).First();

            return regixDataIds;
        }

        private void AddOrEditApplicationFishingGears(List<FishingGearDTO> fishingGears, PermitLicensesRegister dbPermitLicense, int? applicationId = null, bool addRegister = false)
        {
            if (fishingGears != null)
            {
                CheckAndThrowIfDuplicatedMarks(dbPermitLicense, fishingGears);
                CheckAndThrowIfDuplicatedPingers(dbPermitLicense, fishingGears);

                SetFishingGearMarksPrefix(fishingGears, dbPermitLicense.IssueDate);

                if (addRegister)
                {
                    List<FishingGearRegister> dbFishingGears = (from gear in Db.FishingGearRegisters
                                                                where gear.PermitLicenseId == dbPermitLicense.Id
                                                                   && gear.RecordType == nameof(RecordTypesEnum.Register)
                                                                select gear).ToList();

                    foreach (FishingGearRegister dbFishingGear in dbFishingGears)
                    {
                        dbFishingGear.IsActive = false;
                    }
                }
                else
                {
                    List<FishingGearRegister> dbFishingGears = (from gear in Db.FishingGearRegisters
                                                                where gear.ApplicationId == applicationId
                                                                   && gear.RecordType == nameof(RecordTypesEnum.Application)
                                                                select gear).ToList();

                    foreach (FishingGearRegister dbFishingGear in dbFishingGears)
                    {
                        dbFishingGear.IsActive = false;
                    }
                }

                foreach (FishingGearDTO fishingGear in fishingGears)
                {
                    HandleFishingGearMarksAndPingersIds(fishingGear, addRegister);

                    fishingGearsService.AddOrEditFishingGear(fishingGear,
                                                             new ApplicationRegisterDataDTO
                                                             {
                                                                 ApplicationId = applicationId,
                                                                 RecordType = addRegister
                                                                              ? RecordTypesEnum.Register
                                                                              : RecordTypesEnum.Application
                                                             },
                                                             dbPermitLicense.Id,
                                                             null);
                }
            }
        }

        private void UpdateAppliedishingGearTariffs(PermitLicenseFishingGearsApplicationDTO application, Application dbApplication, bool isAdd)
        {
            Db.AddOrEditApplicationPayment(
                dbApplication,
                Db.MapPaymentInformationToApplicationPaymentDTO(application.PaymentInformation),
                null,
                application.PaymentInformation.Id);

            ApplicationPayment applicationPayment = GetApplicationPayment(application.ApplicationId.Value);

            PaymentStatusesEnum paymentStatus = (from ps in Db.NPaymentStatuses
                                                 where ps.Id == applicationPayment.PaymentStatusId
                                                 select Enum.Parse<PaymentStatusesEnum>(ps.Code)).First();

            if ((paymentStatus != PaymentStatusesEnum.PaidOK && paymentStatus != PaymentStatusesEnum.NotNeeded) || isAdd)
            {
                PermitLicenseTariffCalculationParameters tariffsParameters = GetFishingGearApplicationTariffCalculationParameters(application);

                List<PaymentTariffDTO> appliedTariffs = CalculatePermitLicenseAppliedTariffs(tariffsParameters);
                List<ApplicationPaymentTariff> oldAppliedTariffs = GetApplicationPaymenTariffs(application.ApplicationId.Value);

                List<PaymentTariffDTO> appliedCalculatedTariffs = appliedTariffs.Where(x => x.IsCalculated).ToList();

                foreach (PaymentTariffDTO paymentTariff in appliedCalculatedTariffs)
                {
                    int? oldAppliedTariffId = oldAppliedTariffs.Where(x => x.TariffId == paymentTariff.TariffId).Select(x => x.Id).FirstOrDefault();

                    Db.AddOrEditApplicationPaymentTariff(applicationPayment,
                                                         paymentTariff,
                                                         oldAppliedTariffId == default(int) ? null : oldAppliedTariffId);
                }

                List<ApplicationPaymentTariff> oldAppliedTariffsToDelete = oldAppliedTariffs.Where(x => !appliedCalculatedTariffs.Any(y => y.TariffId == x.TariffId)).ToList();

                foreach (ApplicationPaymentTariff oldAppliedTariff in oldAppliedTariffsToDelete)
                {
                    Db.AddOrEditApplicationPaymentTariff(applicationPayment, null, oldAppliedTariff.Id);
                }
            }
        }

        private PermitLicenseTariffCalculationParameters GetFishingGearApplicationTariffCalculationParameters(PermitLicenseFishingGearsApplicationDTO application)
        {
            PermitLicenseTariffCalculationParameters tariffsParameters = new PermitLicenseTariffCalculationParameters
            {
                ApplicationId = application.ApplicationId.Value,
                PageCode = PageCodeEnum.FishingGearsCommFish,
                ShipId = null,
                WaterTypeCode = (from permit in Db.CommercialFishingPermitLicensesRegisters
                                 join waterType in Db.NwaterTypes on permit.WaterTypeId equals waterType.Id
                                 where permit.Id == application.PermitLicenseId
                                 select waterType.Code).First(),
                AquaticOrganismTypeIds = null,
                FishingGears = application.FishingGears,
                PoundNetId = null
            };

            if (application.PaymentInformation != null
                && application.PaymentInformation.PaymentSummary != null
                && application.PaymentInformation.PaymentSummary.Tariffs != null)
            {
                tariffsParameters.ExcludedTariffsIds = application.PaymentInformation.PaymentSummary.Tariffs
                                                                                                      .Where(x => !x.IsChecked)
                                                                                                      .Select(x => x.TariffId).ToList();
            }

            return tariffsParameters;
        }

        private void HandleFishingGearMarksAndPingersIds(FishingGearDTO fishingGear, bool isRegister = false)
        {
            bool noId = (from gear in Db.FishingGearRegisters
                         where isRegister
                               || !fishingGear.Id.HasValue
                               || (gear.Id == fishingGear.Id.Value
                                   && gear.RecordType == nameof(RecordTypesEnum.Register))
                         select 1).Any();

            if (noId)
            {
                fishingGear.Id = null;

                if (fishingGear.Marks != null)
                {
                    foreach (FishingGearMarkDTO mark in fishingGear.Marks)
                    {
                        mark.Id = null;
                    }
                }

                if (fishingGear.Pingers != null)
                {
                    foreach (FishingGearPingerDTO pinger in fishingGear.Pingers)
                    {
                        pinger.Id = null;
                    }
                }
            }
        }

        private void UpdateFishingGearMarkStatuses(List<FishingGearDTO> fishingGears)
        {
            List<FishingGearMarkDTO> marksToUpdate = fishingGears.Where(x => x.Marks != null)
                                                                 .SelectMany(x => x.Marks)
                                                                 .Where(x => x.IsActive && x.SelectedStatus == FishingGearMarkStatusesEnum.NEW)
                                                                 .ToList();

            if (marksToUpdate.Count > 0)
            {
                int registeredMarkId = GetRegisteredMarkStatusId();

                foreach (FishingGearMarkDTO mark in marksToUpdate)
                {
                    mark.StatusId = registeredMarkId;
                    mark.SelectedStatus = FishingGearMarkStatusesEnum.REGISTERED;
                }
            }
        }

        private int? GetPermitLicenseByRegistrationNumber(string registrationNumber, int shipId)
        {
            int shipUid = GetShipUId(shipId);
            List<int> shipIds = GetShipIds(shipUid);

            int? permitLicenseId = (from permLic in Db.CommercialFishingPermitLicensesRegisters
                                    where permLic.RegistrationNum == registrationNumber
                                       && shipIds.Contains(permLic.ShipId)
                                       && permLic.RecordType == nameof(RecordTypesEnum.Register)
                                    select permLic.Id).FirstOrDefault();

            return permitLicenseId == default(int) ? null : permitLicenseId;
        }
    }
}
