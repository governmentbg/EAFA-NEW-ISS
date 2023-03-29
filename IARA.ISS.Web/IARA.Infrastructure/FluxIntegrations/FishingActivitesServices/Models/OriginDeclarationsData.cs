namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class OriginDeclarationsData
    {
        public List<OriginDeclaration> OriginDeclarations { get; set; }

        public IDictionary<int, List<OriginDeclarationFish>> LogBookPageOriginDeclarationFishes { get; set; }
    }
}
