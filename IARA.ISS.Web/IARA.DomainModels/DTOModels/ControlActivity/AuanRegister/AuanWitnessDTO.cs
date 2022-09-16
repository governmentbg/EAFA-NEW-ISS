using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanWitnessDTO
    {
        public int? Id { get; set; }

        public int? AuanId { get; set; }

        public int? InspDeliveryId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string WitnessNames { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? AddressId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public AddressRegistrationDTO Address { get; set; }

        public short? OrderNum { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
