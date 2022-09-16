using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class StatisticalFormsFilters : BaseRequestModel
    {
        public string RegistryNumber { get; set; }

        public DateTime? SubmissionDateFrom { get; set; }

        public DateTime? SubmissionDateTo { get; set; }

        public int? ProcessUserId { get; set; }

        public int? SubmissionUserId { get; set; }

        public List<int> FormTypeIds { get; set; }

        public string FormObject { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }

        public int? TerritoryUnitId { get; set; }

        public bool? FilterAquaFarmTerritoryUnit { get; set; }

        public bool? FilterReworkTerritoryUnit { get; set; }

        public bool? FilterFishVesselTerritoryUnit { get; set; }
    }
}
