namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class VesselDto
    {
        public int? ShipId { get; set; }
        public int? UnregisteredVesselId { get; set; }
        public bool? IsRegistered { get; set; }
        public string Name { get; set; }
        public string ExternalMark { get; set; }
        public string CFR { get; set; }
        public string UVI { get; set; }
        public string RegularCallsign { get; set; }
        public string MMSI { get; set; }
        public int? FlagCountryId { get; set; }
        public int? PatrolVehicleTypeId { get; set; }
        public int? VesselTypeId { get; set; }
        public int? InstitutionId { get; set; }
    }
}
