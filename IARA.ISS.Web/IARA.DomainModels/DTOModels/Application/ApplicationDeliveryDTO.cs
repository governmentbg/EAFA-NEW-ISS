using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationDeliveryDTO : ApplicationBaseDeliveryDTO
    {
        [RequiredIf(nameof(IsDelivered), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? DeliveryDate { get; set; }

        public DateTime? SentDate { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ReferenceNumber { get; set; }

        public bool IsDelivered { get; set; }
    }
}
