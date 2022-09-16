namespace IARA.DomainModels.DTOModels.Mobile.Ships
{
    public class LegalMobileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Eik { get; set; }
        public ShipPersonnelAddressMobileDTO Address { get; set; }
    }
}
