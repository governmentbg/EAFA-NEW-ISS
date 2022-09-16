using System.Collections.Generic;
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
using IARA.EntityModels.Entities;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.FishingCapacity.Duplicates;

namespace IARA.RegixAbstractions.Interfaces
{
    public interface IRegixApplicationInterfaceService
    {
        BuyerRegixDataDTO GetBuyersChecks(int applicationId);
        BuyerChangeOfCircumstancesRegixDataDTO GetBuyerChangeOfCircumstancesChecks(int applicationId);
        BuyerChangeOfCircumstancesRegixDataDTO GetFirstSaleCenterChangeOfCircumstancesChecks(int applicationId);
        BuyerTerminationRegixDataDTO GetBuyerTerminationChecks(int applicationId);
        BuyerTerminationRegixDataDTO GetFirstSaleCenterTerminationChecks(int applicationId);

        AquacultureChangeOfCircumstancesRegixDataDTO GetAquacultureChangeOfCircumstancesChecks(int applicationId);
        AquacultureDeregistrationRegixDataDTO GetAquacultureDeregistrationChecks(int applicationId);
        AquacultureRegixDataDTO GetAquacultureRegistrationChecks(int applicationId);

        CommercialFishingRegixDataDTO GetCommercialFishingChecks(int applicationId);

        List<ApplicationRegiXcheck> GetCurrentApplicationChecks(int applicationId);

        IncreaseFishingCapacityRegixDataDTO GetFishingCapacityIncreaseChecks(int applicationId);
        ReduceFishingCapacityRegixDataDTO GetFishingCapacityReduceChecks(int applicationId);
        TransferFishingCapacityRegixDataDTO GetFishingCapacityTransferChecks(int applicationId);
        CapacityCertificateDuplicateRegixDataDTO GetFishingCapacityDuplicateChecks(int applicationId);

        DuplicatesApplicationRegixDataDTO GetDuplicateApplicationChecks(int applicationId, PageCodeEnum pageCode);

        LegalEntityRegixDataDTO GetLegalChecks(int applicationId);

        QualifiedFisherRegixDataDTO GetQualifiedFisherChecks(int applicationId);

        ScientificFishingPermitRegixDataDTO GetScientificFishingPermitChecks(int applicationId);

        ShipChangeOfCircumstancesRegixDataDTO GetShipChangeOfCircumstancesChecks(int applicationId);
        ShipDeregistrationRegixDataDTO GetShipDeregistrationChecks(int applicationId);
        ShipRegisterRegixDataDTO GetShipRegisterChecks(int applicationId);

        RecreationalFishingTicketBaseRegixDataDTO GetTicketChecks(int applicationId);

        StatisticalFormFishVesselRegixDataDTO GetStatisticalFormFishVesselChecks(int applicationId);
        StatisticalFormReworkRegixDataDTO GetStatisticalFormReworkChecks(int applicationId);
        StatisticalFormAquaFarmRegixDataDTO GetStatisticalFormAquaFarmChecks(int applicationId);
    }
}
