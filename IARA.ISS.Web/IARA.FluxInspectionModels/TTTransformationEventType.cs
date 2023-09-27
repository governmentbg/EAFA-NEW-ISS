namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTTransformationEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTTransformationEventType
    {

        private DateTimeType occurrenceDateTimeField;

        private DateTimeType recordedDateTimeField;

        private IDType[] inputObjectInstanceIDField;

        private IDType[] outputObjectInstanceIDField;

        private CodeType dispositionCodeField;

        private CodeType businessStepCodeField;

        private IDType transformationIDField;

        private TTLocationType readPointRelatedTTLocationField;

        private TTLocationType businessRelatedTTLocationField;

        private TTTradeTransactionType[] specifiedTTTradeTransactionField;

        private TTEventElementType[] inputQuantitySpecifiedTTEventElementField;

        private TTEventElementType[] outputQuantitySpecifiedTTEventElementField;

        private TTPartyType[] sourceRelatedTTPartyField;

        private TTPartyType[] destinationRelatedTTPartyField;

        public DateTimeType OccurrenceDateTime
        {
            get => this.occurrenceDateTimeField;
            set => this.occurrenceDateTimeField = value;
        }

        public DateTimeType RecordedDateTime
        {
            get => this.recordedDateTimeField;
            set => this.recordedDateTimeField = value;
        }

        [XmlElement("InputObjectInstanceID")]
        public IDType[] InputObjectInstanceID
        {
            get => this.inputObjectInstanceIDField;
            set => this.inputObjectInstanceIDField = value;
        }

        [XmlElement("OutputObjectInstanceID")]
        public IDType[] OutputObjectInstanceID
        {
            get => this.outputObjectInstanceIDField;
            set => this.outputObjectInstanceIDField = value;
        }

        public CodeType DispositionCode
        {
            get => this.dispositionCodeField;
            set => this.dispositionCodeField = value;
        }

        public CodeType BusinessStepCode
        {
            get => this.businessStepCodeField;
            set => this.businessStepCodeField = value;
        }

        public IDType TransformationID
        {
            get => this.transformationIDField;
            set => this.transformationIDField = value;
        }

        public TTLocationType ReadPointRelatedTTLocation
        {
            get => this.readPointRelatedTTLocationField;
            set => this.readPointRelatedTTLocationField = value;
        }

        public TTLocationType BusinessRelatedTTLocation
        {
            get => this.businessRelatedTTLocationField;
            set => this.businessRelatedTTLocationField = value;
        }

        [XmlElement("SpecifiedTTTradeTransaction")]
        public TTTradeTransactionType[] SpecifiedTTTradeTransaction
        {
            get => this.specifiedTTTradeTransactionField;
            set => this.specifiedTTTradeTransactionField = value;
        }

        [XmlElement("InputQuantitySpecifiedTTEventElement")]
        public TTEventElementType[] InputQuantitySpecifiedTTEventElement
        {
            get => this.inputQuantitySpecifiedTTEventElementField;
            set => this.inputQuantitySpecifiedTTEventElementField = value;
        }

        [XmlElement("OutputQuantitySpecifiedTTEventElement")]
        public TTEventElementType[] OutputQuantitySpecifiedTTEventElement
        {
            get => this.outputQuantitySpecifiedTTEventElementField;
            set => this.outputQuantitySpecifiedTTEventElementField = value;
        }

        [XmlElement("SourceRelatedTTParty")]
        public TTPartyType[] SourceRelatedTTParty
        {
            get => this.sourceRelatedTTPartyField;
            set => this.sourceRelatedTTPartyField = value;
        }

        [XmlElement("DestinationRelatedTTParty")]
        public TTPartyType[] DestinationRelatedTTParty
        {
            get => this.destinationRelatedTTPartyField;
            set => this.destinationRelatedTTPartyField = value;
        }
    }
}