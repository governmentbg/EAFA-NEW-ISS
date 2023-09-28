namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ValidationResultDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ValidationResultDocumentType
    {

        private IDType validatorIDField;

        private DateTimeType creationDateTimeField;

        private ValidationQualityAnalysisType[] relatedValidationQualityAnalysisField;

        public IDType ValidatorID
        {
            get => this.validatorIDField;
            set => this.validatorIDField = value;
        }

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        [XmlElement("RelatedValidationQualityAnalysis")]
        public ValidationQualityAnalysisType[] RelatedValidationQualityAnalysis
        {
            get => this.relatedValidationQualityAnalysisField;
            set => this.relatedValidationQualityAnalysisField = value;
        }
    }
}