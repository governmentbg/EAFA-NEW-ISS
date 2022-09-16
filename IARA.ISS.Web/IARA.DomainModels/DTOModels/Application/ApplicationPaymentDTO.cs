using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationPaymentDTO
    {
        public int Id { get; set; }

        public PaymentTypesEnum? PaymentType { get; set; }

        [RequiredIf(nameof(PaymentStatus), "msgRequired", typeof(ErrorResources), PaymentStatusesEnum.PaidOK)]
        public DateTime? PaymentDateTime { get; set; }

        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PaymentRefNumber { get; set; }

        public PaymentStatusesEnum? PaymentStatus { get; set; }
    }
}
