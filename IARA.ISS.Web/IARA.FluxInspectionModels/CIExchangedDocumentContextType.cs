namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIExchangedDocumentContext", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIExchangedDocumentContextType
    {

        private IDType specifiedTransactionIDField;

        private DateTimeType processingTransactionDateTimeField;

        private IndicatorType testIndicatorField;

        private CIDocumentContextParameterType[] businessProcessSpecifiedCIDocumentContextParameterField;

        private CIDocumentContextParameterType[] bIMSpecifiedCIDocumentContextParameterField;

        private CIDocumentContextParameterType[] scenarioSpecifiedCIDocumentContextParameterField;

        private CIDocumentContextParameterType[] applicationSpecifiedCIDocumentContextParameterField;

        private CIDocumentContextParameterType[] guidelineSpecifiedCIDocumentContextParameterField;

        private CIDocumentContextParameterType[] subsetSpecifiedCIDocumentContextParameterField;

        private CIDocumentContextParameterType messageStandardSpecifiedCIDocumentContextParameterField;

        private CIDocumentContextParameterType[] userSpecifiedCIDocumentContextParameterField;

        public IDType SpecifiedTransactionID
        {
            get => this.specifiedTransactionIDField;
            set => this.specifiedTransactionIDField = value;
        }

        public DateTimeType ProcessingTransactionDateTime
        {
            get => this.processingTransactionDateTimeField;
            set => this.processingTransactionDateTimeField = value;
        }

        public IndicatorType TestIndicator
        {
            get => this.testIndicatorField;
            set => this.testIndicatorField = value;
        }

        [XmlElement("BusinessProcessSpecifiedCIDocumentContextParameter")]
        public CIDocumentContextParameterType[] BusinessProcessSpecifiedCIDocumentContextParameter
        {
            get => this.businessProcessSpecifiedCIDocumentContextParameterField;
            set => this.businessProcessSpecifiedCIDocumentContextParameterField = value;
        }

        [XmlElement("BIMSpecifiedCIDocumentContextParameter")]
        public CIDocumentContextParameterType[] BIMSpecifiedCIDocumentContextParameter
        {
            get => this.bIMSpecifiedCIDocumentContextParameterField;
            set => this.bIMSpecifiedCIDocumentContextParameterField = value;
        }

        [XmlElement("ScenarioSpecifiedCIDocumentContextParameter")]
        public CIDocumentContextParameterType[] ScenarioSpecifiedCIDocumentContextParameter
        {
            get => this.scenarioSpecifiedCIDocumentContextParameterField;
            set => this.scenarioSpecifiedCIDocumentContextParameterField = value;
        }

        [XmlElement("ApplicationSpecifiedCIDocumentContextParameter")]
        public CIDocumentContextParameterType[] ApplicationSpecifiedCIDocumentContextParameter
        {
            get => this.applicationSpecifiedCIDocumentContextParameterField;
            set => this.applicationSpecifiedCIDocumentContextParameterField = value;
        }

        [XmlElement("GuidelineSpecifiedCIDocumentContextParameter")]
        public CIDocumentContextParameterType[] GuidelineSpecifiedCIDocumentContextParameter
        {
            get => this.guidelineSpecifiedCIDocumentContextParameterField;
            set => this.guidelineSpecifiedCIDocumentContextParameterField = value;
        }

        [XmlElement("SubsetSpecifiedCIDocumentContextParameter")]
        public CIDocumentContextParameterType[] SubsetSpecifiedCIDocumentContextParameter
        {
            get => this.subsetSpecifiedCIDocumentContextParameterField;
            set => this.subsetSpecifiedCIDocumentContextParameterField = value;
        }

        public CIDocumentContextParameterType MessageStandardSpecifiedCIDocumentContextParameter
        {
            get => this.messageStandardSpecifiedCIDocumentContextParameterField;
            set => this.messageStandardSpecifiedCIDocumentContextParameterField = value;
        }

        [XmlElement("UserSpecifiedCIDocumentContextParameter")]
        public CIDocumentContextParameterType[] UserSpecifiedCIDocumentContextParameter
        {
            get => this.userSpecifiedCIDocumentContextParameterField;
            set => this.userSpecifiedCIDocumentContextParameterField = value;
        }
    }
}