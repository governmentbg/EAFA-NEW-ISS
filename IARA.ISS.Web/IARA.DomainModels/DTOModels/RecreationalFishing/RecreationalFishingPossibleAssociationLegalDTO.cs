namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingPossibleAssociationLegalDTO
    {
        public int Id { get; set; }
        public string EIK { get; set; }
        public string Name { get; set; }
        public bool HasPermissions { get; set; }
        public bool IsChecked { get; set; }
    }
}
