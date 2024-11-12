namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain.Base
{
    internal abstract class BaseInspectionData
    {
        public string InspectionTypeCode;

        public InspectionTypesEnum InspectionType;

        public DateTime ReportDate;

        public DateTime InspectionStartDate;

        public DateTime? InspectionEndDate;

        public bool? IsAdministrativeViolation;

        public string ActoionsTakenText;

        public string InspectorComment;

        public Dictionary<string, string> ObservationTextCategories;

        public List<InspectorData> Inspectors;

        public List<InspectedEntityData> InspectedEntities;

        public List<CatchMeasureData> CatchMeasures;

        public List<InspectionCheckData> InspectionChecks;

        public List<string> FileTypeCodes;

        public static Dictionary<string, string> GetObservationTextCategories(int inspectionId, IARADbContext db)
        {
            Dictionary<string, string> result = (from obsText in db.InspectionObservationTexts
                                                 join textCategory in db.NinspectionObservationTextCategories on obsText.InspectionTextCategoryId equals textCategory.Id
                                                 where obsText.InspectionId == inspectionId
                                                     && obsText.IsActive
                                                 select new
                                                 {
                                                     Category = textCategory.Code,
                                                     Text = obsText.ObservationsOrViolationsText
                                                 }).ToDictionary(x => x.Category, x => x.Text);

            return result;
        }

        public static List<string> GetInspectionFileTypeCodes(int inspectionId, IARADbContext db)
        {
            List<string> result = (from recordFile in db.InspectionRegisterFiles
                                   join file in db.Files on recordFile.FileId equals file.Id
                                   join fileType in db.NfileTypes on recordFile.FileTypeId equals fileType.Id
                                   where recordFile.RecordId == inspectionId
                                        && recordFile.IsActive
                                        && file.IsActive
                                   select fileType.Code).ToList();

            return result;
        }
    }
}
