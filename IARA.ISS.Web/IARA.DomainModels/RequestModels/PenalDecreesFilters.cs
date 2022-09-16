using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class PenalDecreesFilters : BaseRequestModel
    {
        public string PenalDecreeNum { get; set; }

        public int? DrafterId { get; set; }

        public string DrafterName { get; set; }

        public int? TerritoryUnitId { get; set; }

        public DateTime? IssueDateFrom { get; set; }

        public DateTime? IssueDateTo { get; set; }

        public List<int> SanctionTypeIds { get; set; }

        public List<int> StatusTypeIds { get; set; }

        public int? FishingGearId { get; set; }

        public int? FishId { get; set; }

        public int? ApplianceId { get; set; }

        public string LocationDescription { get; set; }

        public string InspectedEntityFirstName { get; set; }

        public string InspectedEntityMiddleName { get; set; }

        public string InspectedEntityLastName { get; set; }

        public string Identifier { get; set; }

        public decimal? FineAmountFrom { get; set; }

        public decimal? FineAmountTo { get; set; }

        public bool? IsDelivered { get; set; }

        public int? AuanId { get; set; }
    }
}
