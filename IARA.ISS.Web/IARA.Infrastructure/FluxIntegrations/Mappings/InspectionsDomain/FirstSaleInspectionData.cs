namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    internal class FirstSaleInspectionData : BaseInspectionData
    {
        public string SubjectName;

        public string SubjectAddress;

        public string RepresentativeComment;

        public static FirstSaleInspectionData GetFirstSaleInspectionData(int id, IARADbContext db)
        {
            FirstSaleInspectionData result = (from firstSale in db.FirstSaleInspections
                                              join inspection in db.InspectionsRegister on firstSale.InspectionId equals inspection.Id
                                              join inspectionType in db.NinspectionTypes on inspection.InspectionTypeId equals inspectionType.Id
                                              where inspection.Id == id
                                              select new FirstSaleInspectionData
                                              {
                                                  InspectionStartDate = inspection.InspectionStart,
                                                  InspectionEndDate = inspection.InspectionEnd,
                                                  ReportDate = inspection.CreatedOn, //TODO?
                                                  ActoionsTakenText = inspection.ActionsTakenText,
                                                  InspectorComment = inspection.InspectorCommentText,
                                                  IsAdministrativeViolation = inspection.HasAdministrativeViolation,
                                                  SubjectName = firstSale.Name,
                                                  SubjectAddress = firstSale.Address,
                                                  InspectionType = inspectionType.Code, //TODO
                                                  RepresentativeComment = firstSale.RepresentativeComment
                                              }).First();

            result.Inspectors = InspectorData.GetInspectors(id, db);
            result.InspectedEntities = InspectedEntityData.GetInspectedEntities(id, db);
            result.CatchMeasures = CatchMeasureData.GetCatchMeasures(id, db);
            result.ObservationTextCategories = GetObservationTextCategories(id, db);

            return result;
        }
    }
}
