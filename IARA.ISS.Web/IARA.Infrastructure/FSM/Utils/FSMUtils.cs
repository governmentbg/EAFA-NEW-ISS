using System;
using IARA.Common.Enums;
using IARA.EntityModels.Entities;

namespace IARA.Infrastructure.FSM.Utils
{
    public static class FSMUtils
    {
        public static Type GetApplicationEntityType(PageCodeEnum applPageCode)
        {
            return applPageCode switch
            {
                PageCodeEnum.SciFi => typeof(ScientificPermitRegister),
                PageCodeEnum.RecFish => typeof(FishingTicket),
                PageCodeEnum.RegVessel => typeof(ShipRegister),
                PageCodeEnum.ShipRegChange => typeof(ApplicationChangeOfCircumstance),
                PageCodeEnum.DeregShip => typeof(ApplicationChangeOfCircumstance),
                PageCodeEnum.IncreaseFishCap => typeof(CapacityChangeHistory),
                PageCodeEnum.ReduceFishCap => typeof(CapacityChangeHistory),
                PageCodeEnum.TransferFishCap => typeof(CapacityChangeHistory),
                PageCodeEnum.CapacityCertDup => typeof(CapacityChangeHistory),
                PageCodeEnum.AquaFarmReg => typeof(AquacultureFacilityRegister),
                PageCodeEnum.AquaFarmChange => typeof(ApplicationChangeOfCircumstance),
                PageCodeEnum.AquaFarmDereg => typeof(ApplicationChangeOfCircumstance),
                PageCodeEnum.LE => typeof(Legal),
                PageCodeEnum.Buyers => typeof(BuyerRegister),
                PageCodeEnum.QualiFi => typeof(FishermenRegister),
                PageCodeEnum.CommFish => typeof(PermitRegister),
                PageCodeEnum.PoundnetCommFish => typeof(PermitRegister),
                PageCodeEnum.RightToFishThirdCountry => typeof(PermitRegister),
                PageCodeEnum.DupCommFish => typeof(DuplicatesRegister),
                PageCodeEnum.DupRightToFishThirdCountry => typeof(DuplicatesRegister),
                PageCodeEnum.DupPoundnetCommFish => typeof(DuplicatesRegister),
                PageCodeEnum.RightToFishResource => typeof(PermitLicensesRegister),
                PageCodeEnum.PoundnetCommFishLic => typeof(PermitLicensesRegister),
                PageCodeEnum.CatchQuataSpecies => typeof(PermitLicensesRegister),
                PageCodeEnum.DupRightToFishResource => typeof(DuplicatesRegister),
                PageCodeEnum.DupPoundnetCommFishLic => typeof(DuplicatesRegister),
                PageCodeEnum.DupCatchQuataSpecies => typeof(DuplicatesRegister),
                PageCodeEnum.CommFishLicense => typeof(FishermenRegister),
                PageCodeEnum.CompetencyDup => typeof(DuplicatesRegister),
                PageCodeEnum.RegFirstSaleCenter => typeof(BuyerRegister),
                PageCodeEnum.RegFirstSaleBuyer => typeof(BuyerRegister),
                PageCodeEnum.ChangeFirstSaleBuyer => typeof(ApplicationChangeOfCircumstance),
                PageCodeEnum.ChangeFirstSaleCenter => typeof(ApplicationChangeOfCircumstance),
                PageCodeEnum.TermFirstSaleBuyer => typeof(ApplicationChangeOfCircumstance),
                PageCodeEnum.TermFirstSaleCenter => typeof(ApplicationChangeOfCircumstance),
                PageCodeEnum.DupFirstSaleBuyer => typeof(DuplicatesRegister),
                PageCodeEnum.DupFirstSaleCenter => typeof(DuplicatesRegister),
                PageCodeEnum.StatFormAquaFarm => typeof(StatisticalFormsRegister),
                PageCodeEnum.StatFormFishVessel => typeof(StatisticalFormsRegister),
                PageCodeEnum.StatFormRework => typeof(StatisticalFormsRegister),
                _ => throw new ArgumentException($"Unexpected application type {applPageCode}"),
            };
        }

        public static Type GetApplicationFilesEntityType(PageCodeEnum applPageCode)
        {
            return applPageCode switch
            {
                PageCodeEnum.SciFi => typeof(ScientificPermitRegisterFile),
                PageCodeEnum.RecFish => typeof(FishingTicketFile),
                PageCodeEnum.RegVessel => typeof(ShipRegisterFile),
                PageCodeEnum.ShipRegChange => typeof(ApplicationFile),
                PageCodeEnum.DeregShip => typeof(ApplicationFile),
                PageCodeEnum.IncreaseFishCap => typeof(ApplicationFile),
                PageCodeEnum.ReduceFishCap => typeof(ApplicationFile),
                PageCodeEnum.TransferFishCap => typeof(ApplicationFile),
                PageCodeEnum.CapacityCertDup => typeof(ApplicationFile),
                PageCodeEnum.AquaFarmReg => typeof(AquacultureFacilityRegisterFile),
                PageCodeEnum.AquaFarmChange => typeof(ApplicationFile),
                PageCodeEnum.AquaFarmDereg => typeof(ApplicationFile),
                PageCodeEnum.LE => typeof(LegalFile),
                PageCodeEnum.Buyers => typeof(BuyerRegisterFile),
                PageCodeEnum.QualiFi => typeof(FishermenRegisterFile),
                PageCodeEnum.CommFish => typeof(PermitRegisterFile),
                PageCodeEnum.PoundnetCommFish => typeof(PermitRegisterFile),
                PageCodeEnum.RightToFishThirdCountry => typeof(PermitRegisterFile),
                PageCodeEnum.DupCommFish => typeof(DuplicatesRegisterFile),
                PageCodeEnum.DupRightToFishThirdCountry => typeof(DuplicatesRegisterFile),
                PageCodeEnum.DupPoundnetCommFish => typeof(DuplicatesRegisterFile),
                PageCodeEnum.RightToFishResource => typeof(PermitLicensesRegisterFile),
                PageCodeEnum.PoundnetCommFishLic => typeof(PermitLicensesRegisterFile),
                PageCodeEnum.CatchQuataSpecies => typeof(PermitLicensesRegisterFile),
                PageCodeEnum.DupRightToFishResource => typeof(DuplicatesRegisterFile),
                PageCodeEnum.DupPoundnetCommFishLic => typeof(DuplicatesRegisterFile),
                PageCodeEnum.DupCatchQuataSpecies => typeof(DuplicatesRegisterFile),
                PageCodeEnum.CommFishLicense => typeof(FishermenRegisterFile),
                PageCodeEnum.CompetencyDup => typeof(DuplicatesRegisterFile),
                PageCodeEnum.RegFirstSaleCenter => typeof(BuyerRegisterFile),
                PageCodeEnum.RegFirstSaleBuyer => typeof(BuyerRegisterFile),
                PageCodeEnum.ChangeFirstSaleBuyer => typeof(ApplicationFile),
                PageCodeEnum.ChangeFirstSaleCenter => typeof(ApplicationFile),
                PageCodeEnum.TermFirstSaleBuyer => typeof(ApplicationFile),
                PageCodeEnum.TermFirstSaleCenter => typeof(ApplicationFile),
                PageCodeEnum.DupFirstSaleBuyer => typeof(DuplicatesRegisterFile),
                PageCodeEnum.DupFirstSaleCenter => typeof(DuplicatesRegisterFile),
                PageCodeEnum.StatFormAquaFarm => typeof(StatisticalFormsRegisterFile),
                PageCodeEnum.StatFormFishVessel => typeof(StatisticalFormsRegisterFile),
                PageCodeEnum.StatFormRework => typeof(StatisticalFormsRegisterFile),
                _ => throw new ArgumentException($"Unexpected application type {applPageCode}"),
            };
        }
    }
}
