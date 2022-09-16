using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.DomainModels.DTOModels.Buyers.ChangeOfCircumstances
{
    public class BuyerChangeOfCircumstancesRegixDataDTO : BuyerChangeOfCircumstancesBaseRegixDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByRegixDataDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForRegixDataDTO SubmittedFor { get; set; }

        public List<ChangeOfCircumstancesDTO> Changes { get; set; }
    }
}
