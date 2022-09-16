namespace IARA.DomainModels.DTOModels.Mobile.Ships
{
    public class BuyerMobileDTO
    {
        public int Id { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public bool HasUtility { get; set; }
        public bool HasVehicle { get; set; }
        public string UtilityName { get; set; }
        public ShipPersonnelAddressMobileDTO UtilityAddress { get; set; }
        public string VehicleNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
