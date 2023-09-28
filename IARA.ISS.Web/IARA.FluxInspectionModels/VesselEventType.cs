namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselEventType
    {

        private IDType vesselIDField;

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private DateTimeType occurrenceDateTimeField;

        private IDType[] idField;

        private DelimitedPeriodType discreteDelimitedPeriodField;

        private ValidationResultDocumentType relatedValidationResultDocumentField;

        private VesselHistoricalCharacteristicType[] relatedVesselHistoricalCharacteristicField;

        private VesselTransportMeansType relatedVesselTransportMeansField;

        private ValidationQualityAnalysisType[] relatedValidationQualityAnalysisField;

        public IDType VesselID
        {
            get => this.vesselIDField;
            set => this.vesselIDField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public DateTimeType OccurrenceDateTime
        {
            get => this.occurrenceDateTimeField;
            set => this.occurrenceDateTimeField = value;
        }

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public DelimitedPeriodType DiscreteDelimitedPeriod
        {
            get => this.discreteDelimitedPeriodField;
            set => this.discreteDelimitedPeriodField = value;
        }

        public ValidationResultDocumentType RelatedValidationResultDocument
        {
            get => this.relatedValidationResultDocumentField;
            set => this.relatedValidationResultDocumentField = value;
        }

        [XmlElement("RelatedVesselHistoricalCharacteristic")]
        public VesselHistoricalCharacteristicType[] RelatedVesselHistoricalCharacteristic
        {
            get => this.relatedVesselHistoricalCharacteristicField;
            set => this.relatedVesselHistoricalCharacteristicField = value;
        }

        public VesselTransportMeansType RelatedVesselTransportMeans
        {
            get => this.relatedVesselTransportMeansField;
            set => this.relatedVesselTransportMeansField = value;
        }

        [XmlElement("RelatedValidationQualityAnalysis")]
        public ValidationQualityAnalysisType[] RelatedValidationQualityAnalysis
        {
            get => this.relatedValidationQualityAnalysisField;
            set => this.relatedValidationQualityAnalysisField = value;
        }
    }
}