using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity
{
    public class ReduceFishingCapacityApplicationDTO : ReduceFishingCapacityBaseRegixDataDTO, IDeliverableApplication
    {
        public bool IsOnlineApplication { get; set; }

        public bool IsDraft { get; set; }

        public bool IsPaid { get; set; }

        public bool HasDelivery { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ShipId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? ReduceGrossTonnageBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? ReducePowerBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public FishingCapacityFreedActionsDTO FreedCapacityAction { get; set; }

        public ApplicationPaymentInformationDTO PaymentInformation { get; set; }

        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
