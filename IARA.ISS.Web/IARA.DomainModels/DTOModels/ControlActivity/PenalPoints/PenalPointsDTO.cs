using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalPoints
{
    public class PenalPointsDTO
    {
        public int? Id { get; set; }
        public string DecreeNum { get; set; }
        public string PenalDecreeNum { get; set; }
        public int PenalDecreeId { get; set; }
        public PointsTypeEnum? PointsType { get; set; }
        public string PointsTypeName { get; set; }
        public string OrderTypeName { get; set; }
        public string OrderNum { get; set; }
        public DateTime IssueDate { get; set; }
        public string Ship { get; set; }
        public string Name { get; set; }
        public int? PointsAmount { get; set; }
        public int? PointsTotalCount { get; set; }
        public bool? IsIncreasePoints { get; set; }
        public bool IsActive { get; set; }
    }
}
