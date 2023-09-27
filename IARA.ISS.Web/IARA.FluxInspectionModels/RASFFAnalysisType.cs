namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFAnalysis", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFAnalysisType
    {

        private TextType[] analyticalMethodField;

        private TextType[] appliedTreatmentField;

        private QuantityType sampleQuantityField;

        private CodeType corroborativeResultCodeField;

        private RASFFPartyType executionRASFFPartyField;

        private RASFFHealthHazardType[] detectedRASFFHealthHazardField;

        private RASFFObservationResultType specifiedRASFFObservationResultField;

        [XmlElement("AnalyticalMethod")]
        public TextType[] AnalyticalMethod
        {
            get => this.analyticalMethodField;
            set => this.analyticalMethodField = value;
        }

        [XmlElement("AppliedTreatment")]
        public TextType[] AppliedTreatment
        {
            get => this.appliedTreatmentField;
            set => this.appliedTreatmentField = value;
        }

        public QuantityType SampleQuantity
        {
            get => this.sampleQuantityField;
            set => this.sampleQuantityField = value;
        }

        public CodeType CorroborativeResultCode
        {
            get => this.corroborativeResultCodeField;
            set => this.corroborativeResultCodeField = value;
        }

        public RASFFPartyType ExecutionRASFFParty
        {
            get => this.executionRASFFPartyField;
            set => this.executionRASFFPartyField = value;
        }

        [XmlElement("DetectedRASFFHealthHazard")]
        public RASFFHealthHazardType[] DetectedRASFFHealthHazard
        {
            get => this.detectedRASFFHealthHazardField;
            set => this.detectedRASFFHealthHazardField = value;
        }

        public RASFFObservationResultType SpecifiedRASFFObservationResult
        {
            get => this.specifiedRASFFObservationResultField;
            set => this.specifiedRASFFObservationResultField = value;
        }
    }
}