using System;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalPoints
{
    public class PenalPointsOrderDTO
    {
        public int Id { get; set; }

        public bool IsIncreasePoints { get; set; }

        public string Type { get; set; }

        public string DecreeNum { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? EffectiveDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int PointsAmount { get; set; }
    }
}
