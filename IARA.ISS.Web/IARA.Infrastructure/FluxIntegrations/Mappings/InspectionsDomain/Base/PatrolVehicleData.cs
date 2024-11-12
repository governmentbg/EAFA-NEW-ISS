namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain.Base
{
    public class PatrolVehicleData : VesselData
    {
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public static List<PatrolVehicleData> GetPatrolVehicles(int inspectionId, IARADbContext db)
        {
            List<PatrolVehicleData> result = (from inspectionPatrolVehicle in db.InspectionPatrolVehicles
                                              join unregisteredVessel in db.UnregisteredVessels on inspectionPatrolVehicle.PatrolUnregisteredVesselId equals unregisteredVessel.Id
                                              join type in db.NpatrolVehicleTypes on unregisteredVessel.PatrolVehicleTypeId equals type.Id into types
                                              from type in types.DefaultIfEmpty()
                                              join flagCountry in db.Ncountries on unregisteredVessel.FlagCountryId equals flagCountry.Id into flagCountries
                                              from flagCountry in flagCountries.DefaultIfEmpty()
                                              where inspectionPatrolVehicle.InspectionId == inspectionId
                                                 && inspectionPatrolVehicle.IsActive
                                              orderby inspectionPatrolVehicle.Id
                                              select new PatrolVehicleData
                                              {
                                                  ShipName = unregisteredVessel.Name,
                                                  ShipCfr = unregisteredVessel.Cfr,
                                                  ExternalMark = unregisteredVessel.ExternalMark,
                                                  IrsCallSign = unregisteredVessel.IrcscallSign,
                                                  FlagCountryCode = flagCountry.Code,
                                                  Longitude = inspectionPatrolVehicle.PatrolVesselCoordinates != null
                                                              ? new DMSType(inspectionPatrolVehicle.PatrolVesselCoordinates.Y).ToString()
                                                              : null,
                                                  Latitude = inspectionPatrolVehicle.PatrolVesselCoordinates != null
                                                             ? new DMSType(inspectionPatrolVehicle.PatrolVesselCoordinates.X).ToString()
                                                             : null
                                              }).ToList();

            return result;
        }
    }
}
