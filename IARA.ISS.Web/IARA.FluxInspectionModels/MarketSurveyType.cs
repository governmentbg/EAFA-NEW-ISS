namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MarketSurvey", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MarketSurveyType
    {

        private IDType[] idField;

        private TextType[] descriptionField;

        private TextType[] targetGroupDescriptionField;

        private QuantityType sampleSizeQuantityField;

        private CodeType[] communicationChannelCodeField;

        private CodeType organizationProcessCodeField;

        private CodeType recordingProcessCodeField;

        private CodeType[] methodCodeField;

        private CodeType[] typeCodeField;

        private IndicatorType timeSeriesIndicatorField;

        private CITradePartyType[] ownerCITradePartyField;

        private CITradePartyType conductingCITradePartyField;

        private SubjectClassificationType[] specifiedSubjectClassificationField;

        private SpecifiedPeriodType fieldworkSpecifiedPeriodField;

        private CITradeCountryType[] targetCITradeCountryField;

        private MarketSurveyProductType[] derivedDeliverableMarketSurveyProductField;

        private CIReferencedDocumentType[] briefingCIReferencedDocumentField;

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

        [XmlElement("TargetGroupDescription")]
        public TextType[] TargetGroupDescription
        {
            get => this.targetGroupDescriptionField;
            set => this.targetGroupDescriptionField = value;
        }

        public QuantityType SampleSizeQuantity
        {
            get => this.sampleSizeQuantityField;
            set => this.sampleSizeQuantityField = value;
        }

        [XmlElement("CommunicationChannelCode")]
        public CodeType[] CommunicationChannelCode
        {
            get => this.communicationChannelCodeField;
            set => this.communicationChannelCodeField = value;
        }

        public CodeType OrganizationProcessCode
        {
            get => this.organizationProcessCodeField;
            set => this.organizationProcessCodeField = value;
        }

        public CodeType RecordingProcessCode
        {
            get => this.recordingProcessCodeField;
            set => this.recordingProcessCodeField = value;
        }

        [XmlElement("MethodCode")]
        public CodeType[] MethodCode
        {
            get => this.methodCodeField;
            set => this.methodCodeField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public IndicatorType TimeSeriesIndicator
        {
            get => this.timeSeriesIndicatorField;
            set => this.timeSeriesIndicatorField = value;
        }

        [XmlElement("OwnerCITradeParty")]
        public CITradePartyType[] OwnerCITradeParty
        {
            get => this.ownerCITradePartyField;
            set => this.ownerCITradePartyField = value;
        }

        public CITradePartyType ConductingCITradeParty
        {
            get => this.conductingCITradePartyField;
            set => this.conductingCITradePartyField = value;
        }

        [XmlElement("SpecifiedSubjectClassification")]
        public SubjectClassificationType[] SpecifiedSubjectClassification
        {
            get => this.specifiedSubjectClassificationField;
            set => this.specifiedSubjectClassificationField = value;
        }

        public SpecifiedPeriodType FieldworkSpecifiedPeriod
        {
            get => this.fieldworkSpecifiedPeriodField;
            set => this.fieldworkSpecifiedPeriodField = value;
        }

        [XmlElement("TargetCITradeCountry")]
        public CITradeCountryType[] TargetCITradeCountry
        {
            get => this.targetCITradeCountryField;
            set => this.targetCITradeCountryField = value;
        }

        [XmlElement("DerivedDeliverableMarketSurveyProduct")]
        public MarketSurveyProductType[] DerivedDeliverableMarketSurveyProduct
        {
            get => this.derivedDeliverableMarketSurveyProductField;
            set => this.derivedDeliverableMarketSurveyProductField = value;
        }

        [XmlElement("BriefingCIReferencedDocument")]
        public CIReferencedDocumentType[] BriefingCIReferencedDocument
        {
            get => this.briefingCIReferencedDocumentField;
            set => this.briefingCIReferencedDocumentField = value;
        }
    }
}