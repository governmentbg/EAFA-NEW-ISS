namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTAggregationEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTAggregationEventType
    {

        private DateTimeType occurrenceDateTimeField;

        private DateTimeType recordedDateTimeField;

        private IDType parentObjectIDField;

        private IDType[] childObjectInstanceIDField;

        private CodeType actionCodeField;

        private CodeType businessStepCodeField;

        private CodeType dispositionCodeField;

        private TTLocationType relatedTTLocationField;

        private TTTradeTransactionType[] specifiedTTTradeTransactionField;

        private TTEventElementType[] childQuantitySpecifiedTTEventElementField;

        private TTPartyType[] sourceRelatedTTPartyField;

        private TTPartyType[] destinationRelatedTTPartyField;

        private TTLocationType readPointRelatedTTLocationField;

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

        public IDType ParentObjectID
        {
            get => this.parentObjectIDField;
            set => this.parentObjectIDField = value;
        }

        [XmlElement("ChildObjectInstanceID")]
        public IDType[] ChildObjectInstanceID
        {
            get => this.childObjectInstanceIDField;
            set => this.childObjectInstanceIDField = value;
        }

        public CodeType ActionCode
        {
            get => this.actionCodeField;
            set => this.actionCodeField = value;
        }

        public CodeType BusinessStepCode
        {
            get => this.businessStepCodeField;
            set => this.businessStepCodeField = value;
        }

        public CodeType DispositionCode
        {
            get => this.dispositionCodeField;
            set => this.dispositionCodeField = value;
        }

        public TTLocationType RelatedTTLocation
        {
            get => this.relatedTTLocationField;
            set => this.relatedTTLocationField = value;
        }

        [XmlElement("SpecifiedTTTradeTransaction")]
        public TTTradeTransactionType[] SpecifiedTTTradeTransaction
        {
            get => this.specifiedTTTradeTransactionField;
            set => this.specifiedTTTradeTransactionField = value;
        }

        [XmlElement("ChildQuantitySpecifiedTTEventElement")]
        public TTEventElementType[] ChildQuantitySpecifiedTTEventElement
        {
            get => this.childQuantitySpecifiedTTEventElementField;
            set => this.childQuantitySpecifiedTTEventElementField = value;
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

        public TTLocationType ReadPointRelatedTTLocation
        {
            get => this.readPointRelatedTTLocationField;
            set => this.readPointRelatedTTLocationField = value;
        }
    }
}