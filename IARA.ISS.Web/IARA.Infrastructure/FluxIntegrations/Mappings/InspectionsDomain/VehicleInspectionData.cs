namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    internal class VehicleInspectionData : BaseInspectionData
    {
        public string TractorLicensePlateNum;

        public string TrailerLicensePlateNum;

        public string TransporterComment;

        public string Latitude;

        public string Longitude;

        public static VehicleInspectionData GetVehicleInspectionData(int id, IARADbContext db)
        {
            VehicleInspectionData result = (from vehicle in db.TransportVehicleInspections
                                            join inspection in db.InspectionsRegister on vehicle.InspectionId equals inspection.Id
                                            join inspectionType in db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                                            where vehicle.InspectionId == id
                                            select new VehicleInspectionData
                                            {
                                                InspectionStartDate = inspection.InspectionStart,
                                                InspectionEndDate = inspection.InspectionEnd,
                                                ReportDate = inspection.CreatedOn,
                                                ActoionsTakenText = inspection.ActionsTakenText,
                                                InspectorComment = inspection.InspectorCommentText,
                                                IsAdministrativeViolation = inspection.HasAdministrativeViolation,
                                                TractorLicensePlateNum = vehicle.TractorLicensePlateNum,
                                                TrailerLicensePlateNum = vehicle.TrailerLicensePlateNum,
                                                TransporterComment = vehicle.TransporterComment,
                                                InspectionType = inspectionType.Code,
                                                Longitude = vehicle.InpectionLocationCoordinates != null ? new DMSType(vehicle.InpectionLocationCoordinates.Y).ToString() : null,
                                                Latitude = vehicle.InpectionLocationCoordinates != null ? new DMSType(vehicle.InpectionLocationCoordinates.X).ToString() : null
                                            }).First();

            result.Inspectors = InspectorData.GetInspectors(id, db);
            result.InspectedEntities = InspectedEntityData.GetInspectedEntities(id, db);
            result.CatchMeasures = CatchMeasureData.GetCatchMeasures(id, db);
            result.ObservationTextCategories = GetObservationTextCategories(id, db);

            return result;
        }
    }
}
