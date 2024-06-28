using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfFishingGearEqualsAttribute : ValidationAttribute
    {
        public RequiredIfFishingGearEqualsAttribute(string fishingGearProperty, FishGearInputs fishGearType)
        {
            FishingGearProperty = fishingGearProperty;
            FishGearType = fishGearType;
        }

        public string FishingGearProperty { get; }
        public FishGearInputs FishGearType { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object fishingGearPropertyValue = validationContext.Items[FishingGearProperty];
            if (fishingGearPropertyValue == null)
            {
                return ValidationResult.Success;
            }
            FishingGearSelectNomenclatureDto fishingGear = fishingGearPropertyValue as FishingGearSelectNomenclatureDto;

            if (RequiredInputs[FishGearType].Contains(fishingGear.Code) && string.IsNullOrEmpty(value.ToString()))
            {
                return new RequiredAttribute
                {
                    ErrorMessage = ErrorMessage
                }.GetValidationResult(value, validationContext);
            }

            return ValidationResult.Success;
        }

        private readonly Dictionary<FishGearInputs, List<string>> RequiredInputs = new Dictionary<FishGearInputs, List<string>>
        {
            { FishGearInputs.NetEyeSize, new List<string> { "TBB", "OTB", "OT", "OTT", "OTP", "PTB", "TB", "TBN", "TBS", "PUK",
                                                            "PUL", "OTM", "PTM", "TM", "TMS", "TSP", "TX", "SDN", "SSC", "SPR",
                                                            "SX", "SV", "SB", "PS", "PS1", "PS2", "LA", "SUX", "DRB", "GN",
                                                            "GNS", "GND", "GNC", "GTN", "GTR", "GNF", "GEN" } },
            { FishGearInputs.Length, new List<string> { "TBB", "PUK", "SDN", "SSC", "SPR", "SX", "SV", "SB", "PS", "PS1",
                                                        "PS2", "LA", "SUX", "DRB", "GN", "GNS", "GND", "GNC", "GTN", "GTR",
                                                        "GNF", "GEN" }},
            { FishGearInputs.Height, new List<string> { "PS", "PS1", "PS2", "LA", "SUX", "GN", "GNS", "GND", "GNC", "GTN",
                                                        "GTR", "GNF", "GEN" }},
            { FishGearInputs.NetNominalLength, new List<string> { "GN", "GNS", "GND", "GNC", "GTN", "GTR", "GNF", "GEN" }},
            { FishGearInputs.LineCount, new List<string> { "LHP", "LHM", "LLS", "LLD", "LL" }},
            { FishGearInputs.NetsInFleetCount, new List<string> { "GN", "GNS", "GND", "GNC", "GTN", "GTR", "GNF", "GEN" }},
            { FishGearInputs.TrawlModel, new List<string> { "OTM", "PTM", "TM", "TMS", "TSP" }},
        };
    }
}
