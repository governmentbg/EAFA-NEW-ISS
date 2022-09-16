namespace IARA.DomainModels.DTOModels.Mobile.Ships
{
    public class ShipOwnerMobileDTO
    {
        public int Id { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public int ShipUid { get; set; }
        public bool IsActive { get; set; }
    }
}
