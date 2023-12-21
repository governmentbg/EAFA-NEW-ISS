using IARA.Common.Resources;
using System.ComponentModel.DataAnnotations;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class PermitLicenseFishingGearsApplicationDTO : BaseRegixChecksDTO, IDeliverableApplication
    {
        public bool IsOnlineApplication { get; set; }

        public bool HasDelivery { get; set; }

        public bool? IsPaid { get; set; }

        public bool? IsDraft { get; set; }

        public int? PermitLicenseId { get; set; }

        public string PermitLicenseNumber { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        public ApplicationPaymentInformationDTO PaymentInformation { get; set; }

        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }

        public List<FishingGearDTO> FishingGears { get; set; }

        public List<FileInfoDTO> Files { get; set; }

        public PermitLicenseFishingGearsRegixDataDTO RegiXDataModel { get; set; }
    }
}
