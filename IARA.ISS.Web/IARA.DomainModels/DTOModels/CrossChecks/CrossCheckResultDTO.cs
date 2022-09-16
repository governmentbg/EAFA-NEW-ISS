using System;

namespace IARA.DomainModels.DTOModels.CrossChecks
{
    public class CrossCheckResultDTO : BaseCrossCheckResultDTO
    {
        public int Id { get; set; }

        public string CheckCode { get; set; }

        public string CheckName { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public int? AssignedUserId { get; set; }

        public string AssignedUser { get; set; }

        public string Resolution { get; set; }

        public bool IsActive { get; set; }
    }
}
