using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.Duplicates
{
    public class DuplicatesRegisterEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ApplicationId { get; set; }

        public bool IsOnlineApplication { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Reason { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public PageCodeEnum? PageCode { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.DupFirstSaleBuyer, PageCodeEnum.DupFirstSaleCenter)]
        public int? BuyerId { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.DupCommFish, PageCodeEnum.DupRightToFishThirdCountry, PageCodeEnum.DupPoundnetCommFish)]
        public int? PermitId { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.DupRightToFishResource, PageCodeEnum.DupPoundnetCommFishLic, PageCodeEnum.DupCatchQuataSpecies)]
        public int? PermitLicenceId { get; set; }

        [RequiredIf(nameof(PageCode), "msgRequired", typeof(ErrorResources), PageCodeEnum.CompetencyDup)]
        public int? QualifiedFisherId { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
