using IARA.EntityModels.Entities;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class RelatedPageData
    {
        public ShipLogBookPage Page { get; set; }

        public bool IsPrimaryRelated { get; set; }
    }
}
