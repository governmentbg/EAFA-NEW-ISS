using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.RecreationalFishing.Associations
{
    public class FishingAssociationApplicationEditDTO : FishingAssociationBaseRegixDataDTO
    {
        public bool IsOnlineApplication { get; set; }

        public bool IsPaid { get; set; }

        public bool HasDelivery { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixPersonDataDTO SubmittedBy { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> SubmittedByAddresses { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixLegalDataDTO SubmittedFor { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> SubmittedForAddresses { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<FishingAssociationPersonDTO> Persons { get; set; }

        public ApplicationPaymentInformationDTO PaymentInformation { get; set; }

        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }

        public List<FileInfoDTO> Files { get; set; }

        public FishingAssociationRegixDataDTO RegiXDataModel { get; set; }
    }
}
