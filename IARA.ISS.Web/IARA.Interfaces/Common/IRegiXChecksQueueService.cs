using System;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.AquacultureFacilities.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.AquacultureFacilities.Deregistration;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Buyers.Termination;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.FishingCapacity.IncreaseCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.TransferCapacity;
using IARA.DomainModels.DTOModels.LegalEntities;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.DTOModels.Buyers.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.DTOModels.StatisticalForms.FishVessels;
using IARA.DomainModels.DTOModels.StatisticalForms.Reworks;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.DomainModels.DTOModels.FishingCapacity.Duplicates;

namespace IARA.Interfaces
{
    public interface IRegiXChecksQueueService
    {
        /// <summary>
        /// Enqueue Regix register checks for Aquaculture change of circumstances
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueAquacultureChangeOfCircumstancesChecks(int applicationId, AquacultureChangeOfCircumstancesRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Aquaculture degistration
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueAquacultureDeregistrationChecks(int applicationId, AquacultureDeregistrationRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Buyers register
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueBuyersChecks(int applicationId, BuyerRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Buyer change of circumstances
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueBuyersChangeChecks(int applicationId, BuyerChangeOfCircumstancesRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Buyer termination
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueBuyersTerminationChecks(int applicationId, BuyerTerminationRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for commersial fishing register
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueCommercialFishingCheck(int applicationId, CommercialFishingBaseRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Increase in fishing capacity application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueFishingCapacityIncreseChecks(int applicationId, IncreaseFishingCapacityRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Reduce in fishing capacity application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueFishingCapacityReduceChecks(int applicationId, ReduceFishingCapacityRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Transfer of fishing capacity application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueFishingCapacityTransferChecks(int applicationId, TransferFishingCapacityRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Transfer of capacity certificate duplicate application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueFishingCapacityDuplicateChecks(int applicationId, CapacityCertificateDuplicateRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Legal entities application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueLegalEntitiesChecks(int applicationId, LegalEntityRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Qualified fisher application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueQualifiedFisherChecks(int applicationId, QualifiedFisherRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Scientific fishing permit application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueScientificPermitChecks(int applicationId, ScientificFishingPermitRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Ship change of circumstances application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueShipChangeOfCircumstancesChecks(int applicationId, ShipChangeOfCircumstancesRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Register ship application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueRegisterShipChecks(int applicationId, ShipRegisterRegixDataDTO applicationData);

        /// <summary>
        /// Enqueue Regix register checks for Ship deregistration application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueShipDeregistrationChecks(int applicationId, ShipDeregistrationRegixDataDTO applicationData);
        Task<bool> EnqueueAquacultureRegistrationChecks(int applicationId, AquacultureRegixDataDTO applicationData);
        Task<bool> EnqueueTicketChecks(int applicationId, RecreationalFishingTicketBaseRegixDataDTO applicationData);
        /// <summary>
        /// Enqueue Regix register checks for statistical form for aquaculture farm facility
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueStatisticalFormAquaFarmChecks(int applicationId, StatisticalFormAquaFarmRegixDataDTO applicationData);
        /// <summary>
        /// Enqueue Regix register checks for statistical form for fishing vessel
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueStatisticalFormFishVesselChecks(int applicationId, StatisticalFormFishVesselRegixDataDTO applicationData);
        /// <summary>
        /// Enqueue Regix register checks for statistical form for rework
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueStatisticalFormReworkChecks(int applicationId, StatisticalFormReworkRegixDataDTO applicationData);
        /// <summary>
        /// Enqueue Regix register checks for duplicate application
        /// </summary>
        /// <param name="applicationId">ID of the Application</param>
        /// <param name="applicationData">Application Data to be checked in Regix</param>
        Task<bool> EnqueueDuplicateApplicationChecks(int applicationId, DuplicatesApplicationRegixDataDTO applicationData);
    }
}
