using System;
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ApplicationsRegister
{
    public class ApplicationRegisterDTO
    {
        public int ID { get; set; }
        public string AccessCode { get; set; }
        public string EventisNum { get; set; }
        public DateTime SubmitDateTime { get; set; }
        public string SubmittedFor { get; set; }
        public string Type { get; set; }
        public string SourceName { get; set; }
        public ApplicationHierarchyTypesEnum SourceCode { get; set; }
        public string StatusName { get; set; }
        public ApplicationStatusesEnum StatusCode { get; set; }
        public string StatusReason { get; set; }
        public string AssignedUser { get; set; }
        public int? AssignedUserId { get; set; }
        public bool IsActive { get; set; }
        public PaymentStatusesEnum PaymentStatus { get; set; }
        public PageCodeEnum PageCode { get; set; }
        public List<ApplicationsChangeHistoryDTO> ChangeHistoryRecords { get; set; }
    }
}
