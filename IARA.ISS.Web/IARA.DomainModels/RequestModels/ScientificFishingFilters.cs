using System;
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.RequestModels
{
    public class ScientificFishingFilters : BaseRequestModel
    {
        public string RequestNumber { get; set; }
        public string PermitNumber { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public List<int> PermitReasonIds { get; set; }
        public List<int> PermitLegalReasonIds { get; set; }

        public string PermitRequesterName { get; set; }
        public string PermitOwnerName { get; set; }
        public string PermitOwnerEgn { get; set; }

        public string ScientificOrganizationName { get; set; }
        public string ResearchWaterArea { get; set; }
        public string AquaticOrganismType { get; set; }

        public string GearType { get; set; }
        public List<ScientificPermitStatusEnum> Statuses { get; set; }

        public int? TerritoryUnitId { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }
    }
}
