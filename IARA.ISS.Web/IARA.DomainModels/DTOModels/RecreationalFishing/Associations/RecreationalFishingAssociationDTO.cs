namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingAssociationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TerritoryUnit { get; set; }
        public string EIK { get; set; }
        public string Phone { get; set; }
        public int MembersCount { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsActive { get; set; }
    }
}
