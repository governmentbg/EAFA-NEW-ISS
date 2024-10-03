namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    internal class VesselInspectionData : BaseInspectionData
    {
        public string ShipName;

        public string ShipCfr;

        public string ExternalMark;

        public string IrsCallSign;

        public string Uvi;

        public string Latitude;

        public string Longitude;

        public string LocationDescription;

        public DateTime? DateTimeOfPosition;

        public string CaptainComment;

        public int? VesselTypeId;

        public int? FlagCountryId;

        public int? PortId;

        public string UnregisteredPortName;

        public int? UnregisteredPortCountryId;

        public DateTime? PortVisitDate;

        public List<PatrolVehicleData> PatrolVehicles;

        public static VesselInspectionData GetShipInspectionData(int id, IARADbContext db, SubjectRoleEnum? role = null)
        {
            VesselInspectionData result = (from inspAtSea in db.ShipInspections
                                           join inspection in db.InspectionsRegister on inspAtSea.InspectionId equals inspection.Id
                                           join inspectionType in db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                                           join ship in db.ShipsRegister on inspAtSea.InspectiedShipId equals ship.Id into shipRegister
                                           from ship in shipRegister.DefaultIfEmpty()
                                           join unregVessel in db.UnregisteredVessels on inspAtSea.InspectedUnregisteredShipId equals unregVessel.Id into unregisteredVessel
                                           from unregVessel in unregisteredVessel.DefaultIfEmpty()
                                           join portVisit in db.InspectionLastPortVisits on inspection.Id equals portVisit.InspectionId into portVisits
                                           from portVisit in portVisits.DefaultIfEmpty()
                                           where inspection.Id == id
                                             && (role == null || inspAtSea.InspectedShipType == role.ToString())
                                           select new VesselInspectionData
                                           {
                                               InspectionStartDate = inspection.InspectionStart,
                                               InspectionEndDate = inspection.InspectionEnd,
                                               ReportDate = inspection.CreatedOn, //TODO?
                                               ActoionsTakenText = inspection.ActionsTakenText,
                                               InspectorComment = inspection.InspectorCommentText,
                                               CaptainComment = inspAtSea.CaptainComment,
                                               ShipName = ship != null ? ship.Name : unregVessel.Name,
                                               ShipCfr = ship != null ? ship.Cfr : unregVessel.Cfr,
                                               ExternalMark = ship != null ? ship.ExternalMark : unregVessel.ExternalMark,
                                               IrsCallSign = ship != null ? ship.IrcscallSign : unregVessel.IrcscallSign,
                                               Uvi = ship != null ? ship.Uvi : unregVessel.Uvi,
                                               VesselTypeId = ship != null ? ship.VesselTypeId : unregVessel.VesselTypeId,
                                               FlagCountryId = ship != null ? ship.FlagCountryId : unregVessel.FlagCountryId,
                                               Longitude = inspAtSea.InspectedShipCoordinates != null
                                                           ? new DMSType(inspAtSea.InspectedShipCoordinates.Y).ToString()
                                                           : null,
                                               Latitude = inspAtSea.InspectedShipCoordinates != null
                                                          ? new DMSType(inspAtSea.InspectedShipCoordinates.X).ToString()
                                                          : null,
                                               IsAdministrativeViolation = inspection.HasAdministrativeViolation,
                                               InspectionType = inspectionType.Code,
                                               LocationDescription = inspAtSea.InspectedShipLocation,
                                               PortId = portVisit != null ? portVisit.PortId : null,
                                               UnregisteredPortName = portVisit != null ? portVisit.UnregisteredPortName : null,
                                               UnregisteredPortCountryId = portVisit != null ? portVisit.UnregisteredPortCountryId : null,
                                               PortVisitDate = portVisit != null ? portVisit.VisitDate : null
                                           }).FirstOrDefault();

            if (result != null && role != SubjectRoleEnum.TransboardSender)
            {
                result.Inspectors = InspectorData.GetInspectors(id, db);
                result.InspectedEntities = InspectedEntityData.GetInspectedEntities(id, db);
                result.PatrolVehicles = PatrolVehicleData.GetPatrolVehicles(id, db);
                result.CatchMeasures = CatchMeasureData.GetCatchMeasures(id, db);
                result.ObservationTextCategories = GetObservationTextCategories(id, db);
            }

            return result;
        }

        public static VesselInspectionData GetObservationAtSeaShipData(int id, IARADbContext db)
        {
            VesselInspectionData result = (from observation in db.ObservationsAtSea
                                           join inspection in db.InspectionsRegister on observation.InspectionId equals inspection.Id
                                           join inspectionType in db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                                           join ship in db.ShipsRegister on observation.ObservedShipId equals ship.Id into shipRegister
                                           from ship in shipRegister.DefaultIfEmpty()
                                           join unregVessel in db.UnregisteredVessels on observation.ObservedUnregisteredShipId equals unregVessel.Id into unregisteredVessel
                                           from unregVessel in unregisteredVessel.DefaultIfEmpty()
                                           where inspection.Id == id
                                           select new VesselInspectionData
                                           {
                                               InspectionStartDate = inspection.InspectionStart,
                                               InspectionEndDate = inspection.InspectionEnd,
                                               ReportDate = inspection.CreatedOn,
                                               ActoionsTakenText = inspection.ActionsTakenText,
                                               InspectorComment = inspection.InspectorCommentText,
                                               ShipName = ship != null ? ship.Name : unregVessel.Name,
                                               ShipCfr = ship != null ? ship.Cfr : unregVessel.Cfr,
                                               ExternalMark = ship != null ? ship.ExternalMark : unregVessel.ExternalMark,
                                               IrsCallSign = ship != null ? ship.IrcscallSign : unregVessel.IrcscallSign,
                                               Uvi = ship != null ? ship.Uvi : unregVessel.Uvi,
                                               VesselTypeId = ship != null ? ship.VesselTypeId : unregVessel.VesselTypeId,
                                               FlagCountryId = ship != null ? ship.FlagCountryId : unregVessel.FlagCountryId,
                                               Longitude = observation.ObservedShipCoordinates != null
                                                           ? new DMSType(observation.ObservedShipCoordinates.Y).ToString()
                                                           : null,
                                               Latitude = observation.ObservedShipCoordinates != null
                                                          ? new DMSType(observation.ObservedShipCoordinates.X).ToString()
                                                          : null,
                                               IsAdministrativeViolation = inspection.HasAdministrativeViolation,
                                               InspectionType = inspectionType.Code,
                                               LocationDescription = observation.ObservedShipLocation
                                           }).First();

            result.Inspectors = InspectorData.GetInspectors(id, db);
            result.PatrolVehicles = PatrolVehicleData.GetPatrolVehicles(id, db);
            result.ObservationTextCategories = GetObservationTextCategories(id, db);

            return result;
        }
    }
}
