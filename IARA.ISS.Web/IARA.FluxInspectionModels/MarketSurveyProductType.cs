namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MarketSurveyProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MarketSurveyProductType
    {

        private IDType[] idField;

        private TextType[] descriptionField;

        private CodeType typeCodeField;

        private SubjectClassificationType[] designatedSubjectClassificationField;

        private MSIAvailabilityType[] supplyMSIAvailabilityField;

        private CIReferencedDocumentType[] resultingReportReferenceCIReferencedDocumentField;

        private MarketSurveyDataSetType includedMarketSurveyDataSetField;

        private MarketSurveyQueryType[] includedMarketSurveyQueryField;

        private MarketSurveyQuestionnaireType includedMarketSurveyQuestionnaireField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("DesignatedSubjectClassification")]
        public SubjectClassificationType[] DesignatedSubjectClassification
        {
            get => this.designatedSubjectClassificationField;
            set => this.designatedSubjectClassificationField = value;
        }

        [XmlElement("SupplyMSIAvailability")]
        public MSIAvailabilityType[] SupplyMSIAvailability
        {
            get => this.supplyMSIAvailabilityField;
            set => this.supplyMSIAvailabilityField = value;
        }

        [XmlElement("ResultingReportReferenceCIReferencedDocument")]
        public CIReferencedDocumentType[] ResultingReportReferenceCIReferencedDocument
        {
            get => this.resultingReportReferenceCIReferencedDocumentField;
            set => this.resultingReportReferenceCIReferencedDocumentField = value;
        }

        public MarketSurveyDataSetType IncludedMarketSurveyDataSet
        {
            get => this.includedMarketSurveyDataSetField;
            set => this.includedMarketSurveyDataSetField = value;
        }

        [XmlElement("IncludedMarketSurveyQuery")]
        public MarketSurveyQueryType[] IncludedMarketSurveyQuery
        {
            get => this.includedMarketSurveyQueryField;
            set => this.includedMarketSurveyQueryField = value;
        }

        public MarketSurveyQuestionnaireType IncludedMarketSurveyQuestionnaire
        {
            get => this.includedMarketSurveyQuestionnaireField;
            set => this.includedMarketSurveyQuestionnaireField = value;
        }
    }
}