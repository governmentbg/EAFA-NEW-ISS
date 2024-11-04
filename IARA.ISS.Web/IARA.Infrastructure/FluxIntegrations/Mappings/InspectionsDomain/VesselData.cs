namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    public class VesselData
    {
        public string ShipName { get; set; }

        public string ShipCfr { get; set; }

        public string ExternalMark { get; set; }

        public string IrsCallSign { get; set; }

        public string RegistrationNumber { get; set; }

        public string Uvi { get; set; }

        public string Mmsi { get; set; }

        public int? VesselTypeId { get; set; }

        public string VesselTypeCode { get; set; }

        public int? FlagCountryId { get; set; }

        public string FlagCountryCode { get; set; }

        public static VesselData GetShipRegisterVesselData(int? shipId, IARADbContext db)
        {
            VesselData result = (from ship in db.ShipsRegister
                                 join flagCountry in db.Ncountries on ship.FlagCountryId equals flagCountry.Id
                                 join vesselType in db.NvesselTypes on ship.VesselTypeId equals vesselType.Id into vesselTypes
                                 from vesselType in vesselTypes.DefaultIfEmpty() //TODO mdr?
                                 where ship.Id == shipId
                                 select new VesselData
                                 {
                                     ShipName = ship.Name,
                                     ExternalMark = ship.ExternalMark,
                                     ShipCfr = ship.Cfr,
                                     Uvi = ship.Uvi,
                                     IrsCallSign = ship.IrcscallSign,
                                     Mmsi = ship.Mmsi,
                                     FlagCountryId = ship.FlagCountryId,
                                     FlagCountryCode = flagCountry.Code,
                                     VesselTypeId = ship.VesselTypeId,
                                     VesselTypeCode = vesselType != null ? vesselType.Code : null
                                 }).FirstOrDefault();

            return result;
        }

        public static VesselData GetUnregisteredVesselData(int? unregisteredVesselId, IARADbContext db)
        {
            VesselData result = (from ship in db.UnregisteredVessels
                                 join flagCountry in db.Ncountries on ship.FlagCountryId equals flagCountry.Id into flagCountries
                                 from flagCountry in flagCountries.DefaultIfEmpty()
                                 join vesselType in db.NvesselTypes on ship.VesselTypeId equals vesselType.Id into vesselTypes
                                 from vesselType in vesselTypes.DefaultIfEmpty()
                                 where ship.Id == unregisteredVesselId
                                 select new VesselData
                                 {
                                     ShipName = ship.Name,
                                     ExternalMark = ship.ExternalMark,
                                     ShipCfr = ship.Cfr,
                                     Uvi = ship.Uvi,
                                     IrsCallSign = ship.IrcscallSign,
                                     Mmsi = ship.Mmsi,
                                     FlagCountryId = ship.FlagCountryId,
                                     FlagCountryCode = flagCountry != null ? flagCountry.Code : null,
                                     VesselTypeId = ship.VesselTypeId,
                                     VesselTypeCode = vesselType != null ? vesselType.Code : null
                                 }).FirstOrDefault();

            return result;
        }
    }
}
