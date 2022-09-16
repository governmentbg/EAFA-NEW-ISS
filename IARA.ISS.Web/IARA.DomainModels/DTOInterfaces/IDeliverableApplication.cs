using IARA.DomainModels.DTOModels.Application;

namespace IARA.DomainModels.DTOInterfaces
{
    public interface IDeliverableApplication
    {
        public int? ApplicationId { get; set; }

        public ApplicationSubmittedByDTO SubmittedBy { get; set; }

        public ApplicationSubmittedForDTO SubmittedFor { get; set; }

        public bool HasDelivery { get; set; }

        public ApplicationBaseDeliveryDTO DeliveryData { get; set; }
    }
}
