using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class PenalDecreeDTO
    {
        public int Id { get; set; }

        public int AuanId { get; set; }

        public int TypeId { get; set; }

        public PenalDecreeTypeEnum DecreeType { get; set; }

        public string DecreeName { get; set; }

        public string DecreeNum { get; set; }

        public DateTime IssueDate { get; set; }

        public string InspectedEntity { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }
    }
}
