using IARA.FluxInspectionModels;
using IARA.FluxModels;
using IARA.Infrastructure.FluxIntegrations.Utils;

namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    internal abstract class BaseInspectionData
    {
        public string InspectionType;

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

        public IDType CreateEuIsrId()
        {
            IDType result = new IDType
            {
                schemeID = ListIDTypes.EU_ISR_ID,
                Value = FluxIdentifierGenerator.GenerateInspectionIdentifier()
            };

            return result;
        }

        protected CodeType CreateReportType()
        {
            CodeType result = new CodeType
            {
                listID = ListIDTypes.ISR_REPORT_TYPE,
                Value = InspectionType //TODO is this correct?
            };

            return result;
        }
    }
}
