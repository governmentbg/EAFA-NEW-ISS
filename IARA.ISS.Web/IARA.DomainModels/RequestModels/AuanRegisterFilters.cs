using System;

namespace IARA.DomainModels.RequestModels
{
    public class AuanRegisterFilters : BaseRequestModel
    {
        public string AuanNum { get; set; }

        public int? DrafterId { get; set; }

        public string DrafterName { get; set; }

        public int? TerritoryUnitId { get; set; }

        public DateTime? DraftDateFrom { get; set; }

        public DateTime? DraftDateTo { get; set; }

        public int? InspectionTypeId { get; set; }

        public int? FishingGearId { get; set; }

        public int? FishId { get; set; }

        public int? ApplianceId { get; set; }

        public string LocationDescription { get; set; } 

        public string InspectedEntityFirstName { get; set; }

        public string InspectedEntityMiddleName { get; set; }

        public string InspectedEntityLastName { get; set; }

        public string Identifier { get; set; }

        public bool? IsDelivered { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }
    }
}
