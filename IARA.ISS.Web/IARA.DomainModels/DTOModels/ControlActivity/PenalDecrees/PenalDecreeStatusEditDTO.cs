using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class PenalDecreeStatusEditDTO : PenalDecreeStatusDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public PenalDecreeStatusTypesEnum? StatusType { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.FirstInstAppealed, PenalDecreeStatusTypesEnum.SecondInstAppealed)]
        public DateTime? AppealDate { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.FirstInstAppealed, PenalDecreeStatusTypesEnum.SecondInstAppealed)]
        public int? CourtId { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.FirstInstAppealed, PenalDecreeStatusTypesEnum.SecondInstAppealed)]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string CaseNum { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.FirstInstDecision)]
        public DateTime? ComplaintDueDate { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.SecondInstDecision)]
        public decimal? RemunerationAmount { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.Valid, 
            PenalDecreeStatusTypesEnum.PartiallyChanged, PenalDecreeStatusTypesEnum.Compulsory)]
        public DateTime? EnactmentDate { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.Withdrawn)]
        public int? PenalAuthorityTypeId { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PenalAuthorityName { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.PartiallyChanged)]
        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Amendments { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.Compulsory)]
        public int? ConfiscationInstitutionId { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.Rescheduled)]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<PenalDecreePaymentScheduleDTO> PaymentSchedule { get; set; }

        [RequiredIf(nameof(StatusType), "msgRequired", typeof(ErrorResources), PenalDecreeStatusTypesEnum.PartiallyPaid)]
        public decimal? PaidAmount { get; set; }
    }
}
