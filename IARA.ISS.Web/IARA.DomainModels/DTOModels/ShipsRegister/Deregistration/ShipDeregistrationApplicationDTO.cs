using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipDeregistrationApplicationDTO : ShipDeregistrationBaseRegixDataDTO, IDeliverableApplication
    {
        public bool IsOnlineApplication { get; set; }

        public bool IsDraft { get; set; }

        public bool IsPaid { get; set; }

        public bool HasDelivery { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string DeregistrationReason { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        public FishingCapacityFreedActionsDTO FreedCapacityAction { get; set; }

        public ApplicationPaymentInformationDTO PaymentInformation { get; set; }

        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
