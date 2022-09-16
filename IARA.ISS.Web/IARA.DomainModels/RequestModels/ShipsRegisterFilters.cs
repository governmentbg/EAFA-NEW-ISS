using System;

namespace IARA.DomainModels.RequestModels
{
    public class ShipsRegisterFilters : BaseRequestModel
    {
        public int? EventTypeId { get; set; }

        public DateTime? EventDateFrom { get; set; }

        public DateTime? EventDateTo { get; set; }

        public int? CancellationReasonId { get; set; }

        public bool? IsCancelled { get; set; }

        public bool? IsThirdPartyShip { get; set; }

        public string ShipOwnerName { get; set; }

        public string ShipOwnerEgnLnc { get; set; }

        public int? FleetId { get; set; }

        public int? VesselTypeId { get; set; }

        public bool? HasCommercialFishingLicence { get; set; }

        public bool? HasVMS { get; set; }

        public string CFR { get; set; }

        public string Name { get; set; }

        public string ExternalMark { get; set; }

        public string IRCSCallSign { get; set; }

        public int? PublicAidTypeId { get; set; }

        public decimal? TotalLengthFrom { get; set; }

        public decimal? TotalLengthTo { get; set; }

        public decimal? GrossTonnageFrom { get; set; }

        public decimal? GrossTonnageTo { get; set; }

        public decimal? NetTonnageFrom { get; set; }

        public decimal? NetTonnageTo { get; set; }

        public decimal? MainEnginePowerFrom { get; set; }

        public decimal? MainEnginePowerTo { get; set; }

        public int? MainFishingGearId { get; set; }

        public int? AdditionalFishingGearId { get; set; }

        public DateTime? FoodLawLicenceDateFrom { get; set; }

        public DateTime? FoodLawLicenceDateTo { get; set; }

        public int? AllowedForQuotaFishId { get; set; }

        public int? ShipAssociationId { get; set; }

        public int? TerritoryUnitId { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }
    }
}
