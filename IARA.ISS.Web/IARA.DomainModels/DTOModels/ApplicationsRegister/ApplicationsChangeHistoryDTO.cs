using System;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.DomainModels.DTOModels.ApplicationsRegister
{
    public class ApplicationsChangeHistoryDTO
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string StatusName { get; set; }
        public string StatusReason { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ModifiedByUserId { get; set; }
        public string ModifiedByUserName { get; set; }
        public int? AssignedUserId { get; set; }
        public string AssingedUserName { get; set; }
        public string TerritoryUnitName { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentStatusCode { get; set; }
        public bool HasApplicationDraftContent { get; set; }
    }
}
