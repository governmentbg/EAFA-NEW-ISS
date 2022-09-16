namespace IARA.DomainModels.DTOModels.Mobile.Ships
{
    public class ShipPersonnelDTO
    {
        public int Id { get; set; }
        public int EntryId { get; set; }
        public int ShipId { get; set; }
        public string Name { get; set; }
        public string EgnEik { get; set; }
        public ShipPersonnelAddressMobileDTO Address { get; set; }
        public bool IsActive { get; set; }
    }
}
