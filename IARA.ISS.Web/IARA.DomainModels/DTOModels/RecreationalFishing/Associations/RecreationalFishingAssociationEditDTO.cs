using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingAssociationEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? TerritoryUnitId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsAdding { get; set; }

        [RequiredIf(nameof(IsAdding), "msgRequired", typeof(ErrorResources), true)]
        public int? LegalId { get; set; }

        [RequiredIf(nameof(IsAdding), "msgRequired", typeof(ErrorResources), false)]
        public RegixLegalDataDTO Legal { get; set; }

        [RequiredIf(nameof(IsAdding), "msgRequired", typeof(ErrorResources), false)]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> LegalAddresses { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsCanceled { get; set; }

        [RequiredIf(nameof(IsCanceled), "msgRequired", typeof(ErrorResources), true)]
        public DateTime? CancellationDate { get; set; }

        [RequiredIf(nameof(IsCanceled), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string CancellationReason { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
