using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class FishingCapacityCertificateEditDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? Id { get; set; }

        public string DuplicateOfCertificateNum { get; set; }

        public string CertificateNum { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public decimal GrossTonnage { get; set; }

        public decimal Power { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsHolderPerson { get; set; }

        [RequiredIf(nameof(IsHolderPerson), "msgRequired", typeof(ErrorResources), true)]
        public RegixPersonDataDTO Person { get; set; }

        [RequiredIf(nameof(IsHolderPerson), "msgRequired", typeof(ErrorResources), false)]
        public RegixLegalDataDTO Legal { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> Addresses { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }
    }
}
