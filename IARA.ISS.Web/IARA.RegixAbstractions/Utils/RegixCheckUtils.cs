using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.AquacultureFacilities.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.AquacultureFacilities.Deregistration;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Buyers.Termination;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.LegalEntities;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.DTOModels.Buyers.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.DTOModels.StatisticalForms.FishVessels;
using IARA.DomainModels.DTOModels.StatisticalForms.Reworks;
using IARA.Interfaces;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.Legals;
using IARA.RegixAbstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using IARA.DomainModels.DTOModels.Duplicates;

namespace IARA.RegixIntegration.Utils
{
    public static class RegixCheckUtils
    {
        public static Task<bool> EnqueueApplicationChecks(IServiceProvider serviceProvider, int id, PageCodeEnum applicationPageCode)
        {
            IRegiXChecksQueueService regiXService = serviceProvider.GetRequiredService<IRegiXChecksQueueService>();

            switch (applicationPageCode)
            {
                case PageCodeEnum.SciFi:
                    {
                        var scientificFishingService = serviceProvider.GetRequiredService<IScientificFishingService>();
                        ScientificFishingPermitRegixDataDTO applicationData = scientificFishingService.GetApplicationRegixData(id);
                        return regiXService.EnqueueScientificPermitChecks(id, applicationData);
                    }
                case PageCodeEnum.LE:
                    {
                        var legalService = serviceProvider.GetRequiredService<ILegalEntitiesService>();
                        LegalEntityRegixDataDTO applicationData = legalService.GetApplicationRegixData(id);
                        return regiXService.EnqueueLegalEntitiesChecks(id, applicationData);
                    }
                case PageCodeEnum.RecFish:
                case PageCodeEnum.RecFishStd:
                case PageCodeEnum.RecFishUnd14:
                case PageCodeEnum.RecFish14And18:
                case PageCodeEnum.RecFishElder:
                case PageCodeEnum.RecFishDisbl:
                case PageCodeEnum.RecFishAssoc:
                    {
                        var fishingTicketService = serviceProvider.GetRequiredService<IRecreationalFishingService>();
                        var applicationData = fishingTicketService.GetApplicationRegixData(id);
                        return regiXService.EnqueueTicketChecks(id, applicationData);
                    }
                case PageCodeEnum.QualiFi:
                case PageCodeEnum.CommFishLicense:
                    {
                        var qualifiedFishersService = serviceProvider.GetRequiredService<IQualifiedFishersService>();
                        QualifiedFisherRegixDataDTO applicationData = qualifiedFishersService.GetApplicationData(id);
                        return regiXService.EnqueueQualifiedFisherChecks(id, applicationData);
                    }
                case PageCodeEnum.Buyers:
                case PageCodeEnum.RegFirstSaleCenter:
                case PageCodeEnum.RegFirstSaleBuyer:
                    {
                        var buyersService = serviceProvider.GetRequiredService<IBuyersService>();
                        BuyerRegixDataDTO applicationData = buyersService.GetApplicationRegixData(id);
                        return regiXService.EnqueueBuyersChecks(id, applicationData);
                    }
                case PageCodeEnum.ChangeFirstSaleBuyer:
                    {
                        var buyersService = serviceProvider.GetRequiredService<IBuyersService>();
                        BuyerChangeOfCircumstancesRegixDataDTO applicationData = buyersService.GetBuyerApplicationChangeOfCircumstancesRegixData(id);
                        return regiXService.EnqueueBuyersChangeChecks(id, applicationData);
                    }
                case PageCodeEnum.ChangeFirstSaleCenter:
                    {
                        var buyersService = serviceProvider.GetRequiredService<IBuyersService>();
                        BuyerChangeOfCircumstancesRegixDataDTO applicationData = buyersService.GetFirstSaleCenterApplicationChangeOfCircumstancesRegixData(id);
                        return regiXService.EnqueueBuyersChangeChecks(id, applicationData);
                    }
                case PageCodeEnum.TermFirstSaleBuyer:
                    {
                        var buyersService = serviceProvider.GetRequiredService<IBuyersService>();
                        BuyerTerminationRegixDataDTO applicationData = buyersService.GetBuyerApplicationTerminationRegixData(id);
                        return regiXService.EnqueueBuyersTerminationChecks(id, applicationData);
                    }
                case PageCodeEnum.TermFirstSaleCenter:
                    {
                        var buyersService = serviceProvider.GetRequiredService<IBuyersService>();
                        BuyerTerminationRegixDataDTO applicationData = buyersService.GetFirstSaleCenterApplicationTerminationRegixData(id);
                        return regiXService.EnqueueBuyersTerminationChecks(id, applicationData);
                    }
                case PageCodeEnum.CommFish:
                case PageCodeEnum.RightToFishResource:
                case PageCodeEnum.RightToFishThirdCountry:
                case PageCodeEnum.PoundnetCommFish:
                case PageCodeEnum.PoundnetCommFishLic:
                case PageCodeEnum.CatchQuataSpecies:
                    {
                        var commercialFishingService = serviceProvider.GetRequiredService<ICommercialFishingService>();
                        CommercialFishingRegixDataDTO applicationData = commercialFishingService.GetApplicationRegixData(id);
                        return regiXService.EnqueueCommercialFishingCheck(id, applicationData);
                    }
                case PageCodeEnum.AquaFarmReg:
                    {
                        var aquacultureFacilitiesService = serviceProvider.GetRequiredService<IAquacultureFacilitiesService>();
                        var applicationData = aquacultureFacilitiesService.GetApplicationRegistrationRegixData(id);
                        return regiXService.EnqueueAquacultureRegistrationChecks(id, applicationData);
                    }
                case PageCodeEnum.AquaFarmChange:
                    {
                        var aquacultureFacilitiesService = serviceProvider.GetRequiredService<IAquacultureFacilitiesService>();
                        AquacultureChangeOfCircumstancesRegixDataDTO applicationData = aquacultureFacilitiesService.GetApplicationChangeOfCircumstancesRegixData(id);
                        return regiXService.EnqueueAquacultureChangeOfCircumstancesChecks(id, applicationData);
                    }
                case PageCodeEnum.AquaFarmDereg:
                    {
                        var aquacultureFacilitiesService = serviceProvider.GetRequiredService<IAquacultureFacilitiesService>();
                        AquacultureDeregistrationRegixDataDTO applicationData = aquacultureFacilitiesService.GetApplicationDeregistrationRegixData(id);
                        return regiXService.EnqueueAquacultureDeregistrationChecks(id, applicationData);
                    }
                case PageCodeEnum.ShipRegChange:
                    {
                        var shipsRegisterService = serviceProvider.GetRequiredService<IShipsRegisterService>();
                        ShipChangeOfCircumstancesRegixDataDTO applicationData = shipsRegisterService.GetApplicationChangeOfCircumstancesRegixData(id);
                        return regiXService.EnqueueShipChangeOfCircumstancesChecks(id, applicationData);
                    }
                case PageCodeEnum.TransferFishCap:
                    {
                        var fishingCapacityService = serviceProvider.GetRequiredService<IFishingCapacityService>();
                        var applicationData = fishingCapacityService.GetApplicationTransferRegixData(id);
                        return regiXService.EnqueueFishingCapacityTransferChecks(id, applicationData);
                    }
                case PageCodeEnum.CapacityCertDup:
                    {
                        var fishingCapacityService = serviceProvider.GetRequiredService<IFishingCapacityService>();
                        var applicationData = fishingCapacityService.GetApplicationCapacityCertificateDuplicateRegixData(id);
                        return regiXService.EnqueueFishingCapacityDuplicateChecks(id, applicationData);
                    }
                case PageCodeEnum.ReduceFishCap:
                    {
                        var fishingCapacityService = serviceProvider.GetRequiredService<IFishingCapacityService>();
                        var applicationData = fishingCapacityService.GetApplicationReduceRegixData(id);
                        return regiXService.EnqueueFishingCapacityReduceChecks(id, applicationData);
                    }
                case PageCodeEnum.RegVessel:
                    {
                        var shipsRegisterService = serviceProvider.GetRequiredService<IShipsRegisterService>();
                        ShipRegisterRegixDataDTO applicationData = shipsRegisterService.GetApplicationRegixData(id);
                        return regiXService.EnqueueRegisterShipChecks(id, applicationData);
                    }
                case PageCodeEnum.IncreaseFishCap:
                    {
                        var fishingCapacityService = serviceProvider.GetRequiredService<IFishingCapacityService>();
                        var applicationData = fishingCapacityService.GetApplicationIncreaseFishingCapacityRegixData(id);
                        return regiXService.EnqueueFishingCapacityIncreseChecks(id, applicationData);
                    }
                case PageCodeEnum.DeregShip:
                    {
                        var shipsRegisterService = serviceProvider.GetRequiredService<IShipsRegisterService>();
                        ShipDeregistrationRegixDataDTO applicationData = shipsRegisterService.GetApplicationDeregistrationData(id);
                        return regiXService.EnqueueShipDeregistrationChecks(id, applicationData);
                    }
                case PageCodeEnum.StatFormAquaFarm:
                    {
                        var statisticalFormsService = serviceProvider.GetRequiredService<IStatisticalFormsService>();
                        StatisticalFormAquaFarmRegixDataDTO applicationData = statisticalFormsService.GetApplicationAquaFarmRegixData(id);
                        return regiXService.EnqueueStatisticalFormAquaFarmChecks(id, applicationData);
                    }
                case PageCodeEnum.StatFormFishVessel:
                    {
                        var statisticalFormsService = serviceProvider.GetRequiredService<IStatisticalFormsService>();
                        StatisticalFormFishVesselRegixDataDTO applicationData = statisticalFormsService.GetApplicationFishVesselRegixData(id);
                        return regiXService.EnqueueStatisticalFormFishVesselChecks(id, applicationData);
                    }
                case PageCodeEnum.StatFormRework:
                    {
                        var statisticalFormsService = serviceProvider.GetRequiredService<IStatisticalFormsService>();
                        StatisticalFormReworkRegixDataDTO applicationData = statisticalFormsService.GetApplicationReworkRegixData(id);
                        return regiXService.EnqueueStatisticalFormReworkChecks(id, applicationData);
                    }
                case PageCodeEnum.DupFirstSaleBuyer:
                case PageCodeEnum.DupFirstSaleCenter:
                case PageCodeEnum.DupCommFish:
                case PageCodeEnum.DupRightToFishThirdCountry:
                case PageCodeEnum.DupPoundnetCommFish:
                case PageCodeEnum.DupRightToFishResource:
                case PageCodeEnum.DupPoundnetCommFishLic:
                case PageCodeEnum.DupCatchQuataSpecies:
                case PageCodeEnum.CompetencyDup:
                    {
                        var duplicatesService = serviceProvider.GetRequiredService<IDuplicatesRegisterService>();
                        DuplicatesApplicationRegixDataDTO applicationData = duplicatesService.GetApplicationDuplicatesRegixData(id);
                        return regiXService.EnqueueDuplicateApplicationChecks(id, applicationData);
                    }
                default:
                    return Task.FromException<bool>(new NotImplementedException());
            }
        }

        public static RegixCheckStatus GetPersonFullChecks(RegixPersonContext response, RegixPersonContext compare)
        {
            var personChecksStatus = GetPersonChecksStatus(response, compare);
            var permanentChecksStatus = GetPermanentAddressChecksStatus(response, compare);

            if (personChecksStatus.Status == RegixCheckStatusesEnum.ERROR | permanentChecksStatus.Status == RegixCheckStatusesEnum.ERROR)
            {
                string errorDescription = GetCommonErrorDescription(personChecksStatus, permanentChecksStatus);
                return new RegixCheckStatus(RegixCheckStatusesEnum.ERROR, errorDescription);
            }
            else if (personChecksStatus.Status == RegixCheckStatusesEnum.WARN | permanentChecksStatus.Status == RegixCheckStatusesEnum.WARN)
            {
                string errorDescription = GetCommonErrorDescription(personChecksStatus, permanentChecksStatus);
                return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, errorDescription);
            }
            else
            {
                return new RegixCheckStatus(RegixCheckStatusesEnum.NONE);
            }
        }

        public static RegixCheckStatus GetPersonChecksStatus(RegixPersonContext response, RegixPersonContext compare)
        {
            if (response != null && compare != null)
            {
                if (!Equals(response, compare, out List<string> errors, true, x => x.Person.FirstName, x => x.Person.LastName))
                {
                    string errorDescription = string.Format(ErrorResources.msgPropertiesDiscrepancy, string.Join(',', errors));

                    if (Equals(response, compare, out errors, false, x => x.Person.FirstName, x => x.Person.LastName))
                    {
                        return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, errorDescription);
                    }
                    else
                    {
                        return new RegixCheckStatus(RegixCheckStatusesEnum.ERROR, errorDescription);
                    }
                }
                else if (!Equals(response,
                                  compare,
                                  out List<string> errorProperties,
                                  true,
                                  x => x.Person.Document.DocumentNumber,
                                  x => x.Person.Document.DocumentIssuedOn,
                                  x => x.Person.Document.DocumentIssuedBy))
                {
                    string errorDescription = string.Format(ErrorResources.msgPropertiesDiscrepancy, string.Join(',', errors));
                    return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, errorDescription);
                }
                else
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.NONE);
                }
            }
            else
            {
                return new RegixCheckStatus(RegixCheckStatusesEnum.ERROR, ErrorResources.msgMissingPerson);
            }
        }

        public static RegixCheckStatus GetPermanentAddressChecksStatus(RegixPersonContext response, RegixPersonContext compare)
        {
            if (response == null
             || response.Addresses == null
             || response.Addresses.Count == 0
             || compare == null
             || compare.Addresses == null
             || compare.Addresses.Count == 0
             || !response.Addresses.Any(x => x.AddressType == AddressTypesEnum.PERMANENT)
             || !compare.Addresses.Any(x => x.AddressType == AddressTypesEnum.PERMANENT))
            {
                return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, ErrorResources.msgPermanentAddressesError);
            }
            else
            {
                var address = response.Addresses.Where(x => x.AddressType == AddressTypesEnum.PERMANENT).FirstOrDefault();
                var otherAddress = compare.Addresses.Where(x => x.AddressType == AddressTypesEnum.PERMANENT).FirstOrDefault();
                return GetAddressCheckStatus(address, otherAddress);
            }
        }

        public static RegixCheckStatus GetAddressCheckStatus(AddressRegistrationDTO address, AddressRegistrationDTO otherAddress)
        {
            if (address == null || otherAddress == null)
            {
                return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, ErrorResources.msgMissingData);
            }
            else if (!Equals(address,
                         otherAddress,
                         out List<string> errorProperties,
                         true,
                         x => x.PopulatedAreaName,
                         x => x.FloorNum,
                         x => x.EntranceNum,
                         x => x.BlockNum,
                         x => x.DistrictName,
                         x => x.MunicipalityName,
                         x => x.Region))
            {
                string errorDescription = string.Format(ErrorResources.msgPropertiesDiscrepancy, string.Join(',', errorProperties));
                return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, errorDescription);
            }
            else
            {
                return new RegixCheckStatus(RegixCheckStatusesEnum.NONE);
            }
        }

        public static bool Equals<T>(T first, T second, out List<string> errorProperties, bool caseSensitive = true, params Expression<Func<T, object>>[] expressions)
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();

            foreach (var expression in expressions)
            {
                object firstValue, secondValue;

                string name = (expression.Body as MemberExpression).Member.Name;

                try
                {
                    firstValue = expression.Compile().Invoke(first);
                }
                catch (NullReferenceException)
                {
                    firstValue = GetDefaultValue(expression.Body.Type);
                }

                try
                {
                    secondValue = expression.Compile().Invoke(second);
                }
                catch (NullReferenceException)
                {
                    secondValue = GetDefaultValue(expression.Body.Type);
                }

                if (!caseSensitive && firstValue != null && secondValue != null && firstValue.GetType() == typeof(string))
                {
                    firstValue = firstValue.ToString().ToLower();
                    secondValue = secondValue.ToString().ToLower();
                }

                if (firstValue != secondValue)
                {
                    results.Add(name, false);
                }
                else
                {
                    results.Add(name, true);
                }
            }

            errorProperties = results.Where(x => !x.Value).Select(x => x.Key).ToList();

            return !results.Any(x => !x.Value);
        }

        public static string GetCommonErrorDescription(params RegixCheckStatus[] regixCheckStatuses)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var regixStatus in regixCheckStatuses)
            {
                builder.Append(regixStatus.ErrorDescription);
                builder.Append(";");
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static object GetDefaultValue(Type t)
        {
            return typeof(RegixCheckUtils)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(x => x.Name == nameof(GetDefaultValue) && x.IsGenericMethod)
                .First()
                .MakeGenericMethod(t)
                .Invoke(null, new object[0]);
        }

        private static T GetDefaultValue<T>()
        {
            return default;
        }
    }
}
