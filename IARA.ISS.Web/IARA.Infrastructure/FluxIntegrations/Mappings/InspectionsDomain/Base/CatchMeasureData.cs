namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain.Base
{
    public class CatchMeasureData
    {

        public string FishCode { get; set; }

        public int? CatchCount { get; set; }

        public decimal? CatchQuantity { get; set; }

        public decimal? UnloadedQuantity { get; set; }

        public string CatchQuadrant { get; set; }

        public bool? Undersized { get; set; }

        public int? ShipId { get; set; }

        public int? UnregisteredVesselId { get; set; }

        public VesselData Vessel { get; set; }

        public static List<CatchMeasureData> GetCatchMeasures(int inspectionId, IARADbContext db)
        {
            List<CatchMeasureData> result = (from catchMeasure in db.InspectionCatchMeasures
                                             join catchType in db.NcatchInspectionTypes on catchMeasure.CatchInspectionTypeId equals catchType.Id into catchTypeGrp
                                             from catchType in catchTypeGrp.DefaultIfEmpty()
                                             join fish in db.Nfishes on catchMeasure.FishId equals fish.Id 
                                             join catchZone in db.NcatchZones on catchMeasure.CatchZoneId equals catchZone.Id into cz
                                             from catchZone in cz.DefaultIfEmpty()
                                             join page in db.InspectionLogBookPages on catchMeasure.InspectedLogBookPageId equals page.Id into pgrp
                                             from page in pgrp.DefaultIfEmpty()
                                             where catchMeasure.IsActive
                                                && catchMeasure.InspectionId == inspectionId
                                             orderby catchMeasure.Id descending
                                             select new CatchMeasureData
                                             {
                                                 FishCode = fish.Code,
                                                 CatchQuadrant = catchZone.Gfcmquadrant,
                                                 Undersized = catchType == null ? null : new bool?(catchType.Code == nameof(CatchSizeCodesEnum.BMS)),
                                                 CatchQuantity = catchMeasure.CatchQuantity,
                                                 UnloadedQuantity = catchMeasure.UnloadedQuantity,
                                                 CatchCount = catchMeasure.CatchCount,
                                                 ShipId = page != null ? page.ShipId : null,
                                                 UnregisteredVesselId = page != null ? page.UnregisteredShipId : null
                                             }).ToList();

            Dictionary<int, VesselData> ships = GetShips(result, db);
            Dictionary<int, VesselData> unregisteredVessels = GetShips(result, db);

            foreach (CatchMeasureData catchMeasure in result)
            {
                if (catchMeasure.ShipId.HasValue)
                {
                    catchMeasure.Vessel = ships[catchMeasure.ShipId.Value];
                }
                else if (catchMeasure.UnregisteredVesselId.HasValue)
                {
                    catchMeasure.Vessel = ships[catchMeasure.UnregisteredVesselId.Value];
                }
            }

            return result;
        }

        private static Dictionary<int, VesselData> GetShips(List<CatchMeasureData> catches, IARADbContext db)
        {
            List<int> shipIds = catches.Where(x => x.ShipId.HasValue).Select(x => x.ShipId.Value).ToList();

            Dictionary<int, VesselData> result = (from ship in db.ShipsRegister
                                                  join flagCountry in db.Ncountries on ship.FlagCountryId equals flagCountry.Id
                                                  join vesselType in db.NvesselTypes on ship.VesselTypeId equals vesselType.Id into vesselTypes
                                                  from vesselType in vesselTypes.DefaultIfEmpty() //TODO mdr?
                                                  where shipIds.Contains(ship.Id)
                                                  select new
                                                  {
                                                      ship.Id,
                                                      Vessel = new VesselData
                                                      {
                                                          ShipName = ship.Name,
                                                          ExternalMark = ship.ExternalMark,
                                                          ShipCfr = ship.Cfr,
                                                          Uvi = ship.Uvi,
                                                          IrsCallSign = ship.IrcscallSign,
                                                          Mmsi = ship.Mmsi,
                                                          FlagCountryCode = flagCountry.Code,
                                                          VesselTypeCode = vesselType != null ? vesselType.Code : null
                                                      }
                                                  }).ToDictionary(x => x.Id, y => y.Vessel);

            return result;
        }

        private static Dictionary<int, VesselData> GetUnregisteredVessels(List<CatchMeasureData> catches, IARADbContext db)
        {
            List<int> vesselIds = catches.Where(x => x.UnregisteredVesselId.HasValue).Select(x => x.UnregisteredVesselId.Value).ToList();

            Dictionary<int, VesselData> result = (from ship in db.UnregisteredVessels
                                                  join flagCountry in db.Ncountries on ship.FlagCountryId equals flagCountry.Id into flagCountries
                                                  from flagCountry in flagCountries.DefaultIfEmpty()
                                                  join vesselType in db.NvesselTypes on ship.VesselTypeId equals vesselType.Id into vesselTypes
                                                  from vesselType in vesselTypes.DefaultIfEmpty()
                                                  where vesselIds.Contains(ship.Id)
                                                  select new
                                                  {
                                                      ship.Id,
                                                      Vessel = new VesselData
                                                      {
                                                          ShipName = ship.Name,
                                                          ExternalMark = ship.ExternalMark,
                                                          ShipCfr = ship.Cfr,
                                                          Uvi = ship.Uvi,
                                                          IrsCallSign = ship.IrcscallSign,
                                                          Mmsi = ship.Mmsi,
                                                          FlagCountryCode = flagCountry != null ? flagCountry.Code : null,
                                                          VesselTypeCode = vesselType != null ? vesselType.Code : null
                                                      }
                                                  }).ToDictionary(x => x.Id, y => y.Vessel);

            return result;
        }
    }
}
