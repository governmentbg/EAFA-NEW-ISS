namespace IARA.DomainModels.DTOModels.ScientificFishing
{
    public class ScientificFishingPermitHolderMobileDTO
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int RequestNumber { get; set; }
        public string Name { get; set; }
        public string ScientificPosition { get; set; }
    }
}
