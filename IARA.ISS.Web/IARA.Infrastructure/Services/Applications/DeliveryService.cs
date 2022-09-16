using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Interfaces;
using TL.EDelivery;
using IARA.Infrastructure.Services.Internal;
using System.Threading.Tasks;
using IARA.Interfaces.Applications;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.Services.Applications
{
    public class DeliveryService : Service, IDeliveryService
    {
        private readonly IEDeliveryService eDeliveryService;
        private readonly IAddressService addressService;
        private readonly IApplicationService applicationService;
        private readonly IServiceProvider serviceProvider;

        public DeliveryService(IARADbContext db,
                               IEDeliveryService eDeliveryService,
                               IAddressService addressService,
                               IApplicationService applicationService,
                               IServiceProvider serviceProvider)
            : base(db)
        {
            this.eDeliveryService = eDeliveryService;
            this.addressService = addressService;
            this.applicationService = applicationService;
            this.serviceProvider = serviceProvider;
        }

        public bool HasApplicationDelivery(int applicationId)
        {
            bool result = (from appl in Db.Applications
                           join applTypeDel in Db.MapApplicationTypeDeliveryTypes on appl.ApplicationTypeId equals applTypeDel.ApplicationTypeId
                           where appl.Id == applicationId
                                && applTypeDel.IsActive
                           select applTypeDel).Any();

            return result;
        }

        public ApplicationDeliveryDTO GetApplicationDeliveryData(int applicationId)
        {
            int deliveryId = (from appl in Db.Applications
                              where appl.Id == applicationId
                              select appl.DeliveryId.Value).First();

            return GetDeliveryData(deliveryId);
        }

        public ApplicationDeliveryDTO GetDeliveryData(int deliveryId)
        {
            ApplicationDelivery applicationDelivery = (from delivery in Db.ApplicationDeliveries
                                                       where delivery.Id == deliveryId
                                                       select delivery).First();

            ApplicationDeliveryDTO result = new ApplicationDeliveryDTO
            {
                Id = applicationDelivery.Id,
                DeliveryTypeId = applicationDelivery.DeliveryTypeId,
                IsDelivered = applicationDelivery.IsDelivered,
                DeliveryDate = applicationDelivery.DeliveryDate,
                SentDate = applicationDelivery.SentDate,
                ReferenceNumber = applicationDelivery.ReferenceNum,
                DeliveryTeritorryUnitId = applicationDelivery.TerritoryUnitId
            };

            if (applicationDelivery.AddressId != null)
            {
                result.DeliveryAddress = addressService.GetAddressRegistration(applicationDelivery.AddressId.Value);
            }

            return result;
        }

        public async Task<int> UpdateDeliveryData(ApplicationDeliveryDTO deliveryData, int deliveryId, bool sendEDelivery = false)
        {
            Db.EditDeliveryData(deliveryData, deliveryId);

            int result = 200;

            if (sendEDelivery)
            {
                result = await SendViaEDelivery(deliveryId);
            }

            Db.SaveChanges();
            return result;
        }

        public async Task<bool> HasSubmittedForEDelivery(int deliveryTypeId,
                                                         ApplicationSubmittedForRegixDataDTO submittedFor,
                                                         ApplicationSubmittedByRegixDataDTO submittedBy)
        {
            string chosenDeliveryTypeCode = (from deliveryType in Db.NdeliveryTypes
                                             where deliveryType.Id == deliveryTypeId
                                             select deliveryType.Code).First();

            if (chosenDeliveryTypeCode == nameof(DeliveryTypesEnum.eDelivery))
            {
                if (submittedFor.SubmittedByRole == SubmittedByRolesEnum.Personal)
                {
                    if (await SubmittedForHasAccessToEDeliveryCheck(submittedBy.Person.EgnLnc.EgnLnc, true) == false) // валидационна грешка
                    {
                        return false;
                    }
                }
                else
                {
                    if (submittedFor.Person != null)
                    {
                        if (await SubmittedForHasAccessToEDeliveryCheck(submittedFor.Person.EgnLnc.EgnLnc, true) == false) // валидационна грешка
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (await SubmittedForHasAccessToEDeliveryCheck(submittedFor.Legal.EIK, false) == false) // валидационна грешка
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public Task<bool> HasSubmittedForEDelivery(int deliveryId, int deliveryTypeId)
        {
            int applicationId = (from appl in Db.Applications
                                 where appl.DeliveryId == deliveryId
                                 select appl.Id).First();

            ApplicationSubmittedForRegixDataDTO submittedFor = applicationService.GetApplicationSubmittedForRegixData(applicationId);
            ApplicationSubmittedByRegixDataDTO submittedBy = applicationService.GetApplicationSubmittedByRegixData(applicationId);

            return HasSubmittedForEDelivery(deliveryTypeId, submittedFor, submittedBy);
        }

        public async Task<bool> HasPersonAccessToEDeliveryAsync(string identifier)
        {
            IEDeliveryIntegrationService eDeliveryIntegrationService = this.eDeliveryService.CreateChannel();
            DcPersonRegistrationInfo info = await eDeliveryIntegrationService.CheckPersonHasRegistrationAsync(identifier);

            return info.HasRegistration;
        }

        public async Task<bool> HasLegalAccessToEDeliveryAsync(string identifier)
        {
            IEDeliveryIntegrationService eDeliveryIntegrationService = this.eDeliveryService.CreateChannel();
            DcLegalPersonRegistrationInfo info = await eDeliveryIntegrationService.CheckLegalPersonHasRegistrationAsync(identifier);

            return info.HasRegistration;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(Db.ApplicationDeliveries, id);
        }

        private async Task<bool> SubmittedForHasAccessToEDeliveryCheck(string identifier, bool isPerson)
        {
            if (isPerson)
            {
                return await HasPersonAccessToEDeliveryAsync(identifier);
            }
            else
            {
                return await HasLegalAccessToEDeliveryAsync(identifier);
            }
        }

        private async Task<int> SendViaEDelivery(int deliveryId)
        {
            var data = (from appl in Db.Applications
                        join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                        where appl.DeliveryId == deliveryId
                        select new
                        {
                            ApplicationId = appl.Id,
                            PageCode = Enum.Parse<PageCodeEnum>(applType.PageCode)
                        }).First();

            IRegisterDeliveryService service = GetRegisterDeliveryServiceByPageCode(data.PageCode);

            ApplicationEDeliveryInfo info = await service.GetEDeliveryInfo(data.ApplicationId, data.PageCode);

            IEDeliveryIntegrationService delivery = this.eDeliveryService.CreateChannel();

            return await delivery.SendElectronicDocumentAsync(info.Subject,
                                                              info.DocBytes,
                                                              info.DocNameWithExtension,
                                                              info.DocRegNumber,
                                                              info.ReceiverType,
                                                              info.ReceiverUniqueIdentifier,
                                                              info.ReceiverPhone,
                                                              info.ReceiverEmail,
                                                              info.ServiceOID,
                                                              info.OperatorEGN);
        }

        private IRegisterDeliveryService GetRegisterDeliveryServiceByPageCode(PageCodeEnum pageCode)
        {
            switch (pageCode)
            {
                case PageCodeEnum.LE:
                    return serviceProvider.GetService(typeof(ILegalEntitiesService)) as IRegisterDeliveryService;
                case PageCodeEnum.RegVessel:
                case PageCodeEnum.DeregShip:
                case PageCodeEnum.ShipRegChange:
                    return serviceProvider.GetService(typeof(IShipsRegisterService)) as IRegisterDeliveryService;
                case PageCodeEnum.ReduceFishCap:
                case PageCodeEnum.IncreaseFishCap:
                case PageCodeEnum.TransferFishCap:
                case PageCodeEnum.CapacityCertDup:
                    return serviceProvider.GetService(typeof(IFishingCapacityService)) as IRegisterDeliveryService;
                // Купувачи
                case PageCodeEnum.RegFirstSaleBuyer:
                case PageCodeEnum.ChangeFirstSaleBuyer:
                case PageCodeEnum.TermFirstSaleBuyer:
                case PageCodeEnum.DupFirstSaleBuyer:
                // ЦПП
                case PageCodeEnum.RegFirstSaleCenter:
                case PageCodeEnum.ChangeFirstSaleCenter:
                case PageCodeEnum.TermFirstSaleCenter:
                case PageCodeEnum.DupFirstSaleCenter:
                    return serviceProvider.GetService(typeof(IBuyersService)) as IRegisterDeliveryService;
                case PageCodeEnum.AquaFarmReg:
                case PageCodeEnum.AquaFarmDereg:
                case PageCodeEnum.AquaFarmChange:
                    return serviceProvider.GetService(typeof(IAquacultureFacilitiesService)) as IRegisterDeliveryService;
                // Разрешителни
                case PageCodeEnum.PoundnetCommFish:
                case PageCodeEnum.DupPoundnetCommFish:
                case PageCodeEnum.CommFish:
                case PageCodeEnum.DupCommFish:
                case PageCodeEnum.RightToFishThirdCountry:
                case PageCodeEnum.DupRightToFishThirdCountry:
                // Удостоверения
                case PageCodeEnum.PoundnetCommFishLic:
                case PageCodeEnum.DupPoundnetCommFishLic:
                case PageCodeEnum.CatchQuataSpecies:
                case PageCodeEnum.DupCatchQuataSpecies:
                case PageCodeEnum.RightToFishResource:
                case PageCodeEnum.DupRightToFishResource:
                    return serviceProvider.GetService(typeof(ICommercialFishingService)) as IRegisterDeliveryService;
                case PageCodeEnum.StatFormFishVessel:
                case PageCodeEnum.StatFormAquaFarm:
                case PageCodeEnum.StatFormRework:
                    return serviceProvider.GetService(typeof(IStatisticalFormsService)) as IRegisterDeliveryService;
                case PageCodeEnum.CommFishLicense:
                case PageCodeEnum.CompetencyDup:
                    return serviceProvider.GetService(typeof(IQualifiedFishersService)) as IRegisterDeliveryService;
                case PageCodeEnum.SciFi:
                    return serviceProvider.GetService(typeof(IScientificFishingService)) as IRegisterDeliveryService;
                default:
                    throw new ArgumentException("Failed to send via EDelivery. Unknown page code: " + pageCode.ToString());
            };
        }
    }
}
