namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    public class PatrolVehicleData
    {
        public string ShipName;

        public string ShipCfr;

        public string ExternalMark;

        public string IrsCallSign;

        public string Uvi;

        public string Latitude;

        public string Longitude;

        public static List<PatrolVehicleData> GetPatrolVehicles(int inspectionId, IARADbContext db)
        {
            List<PatrolVehicleData> result = (from inspectionPatrolVehicle in db.InspectionPatrolVehicles
                                              join unregisteredVessel in db.UnregisteredVessels on inspectionPatrolVehicle.PatrolUnregisteredVesselId equals unregisteredVessel.Id
                                              where inspectionPatrolVehicle.InspectionId == inspectionId
                                                 && inspectionPatrolVehicle.IsActive
                                              orderby inspectionPatrolVehicle.Id
                                              select new PatrolVehicleData
                                              {
                                                  ShipName = unregisteredVessel.Name,
                                                  ShipCfr = unregisteredVessel.Cfr,
                                                  ExternalMark = unregisteredVessel.ExternalMark,
                                                  IrsCallSign = unregisteredVessel.IrcscallSign,
                                                  Uvi = unregisteredVessel.Uvi,
                                                  Longitude = inspectionPatrolVehicle.PatrolVesselCoordinates != null ? new DMSType(inspectionPatrolVehicle.PatrolVesselCoordinates.Y).ToString() : null,
                                                  Latitude = inspectionPatrolVehicle.PatrolVesselCoordinates != null ? new DMSType(inspectionPatrolVehicle.PatrolVesselCoordinates.X).ToString() : null
                                              }).ToList();

            return result;
        }
    }
}
