using System;
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ScientificFishing
{
    public class ScientificFishingPermitDTO
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public int RequestNumber { get; set; }

        public string RequesterName { get; set; }

        public string ScientificOrganizationName { get; set; }

        public string PermitReasons { get; set; }

        public DateTime ValidTo { get; set; }

        public int OutingsCount { get; set; }

        public ScientificPermitStatusEnum PermitStatus { get; set; }

        public string PermitStatusName { get; set; }

        public int? DeliveryId { get; set; }

        // Not returned from the backend but necessary in the frontend models
        public List<ScientificFishingPermitHolderDTO> Holders { get; set; }

        public bool IsActive { get; set; }
    }
}
