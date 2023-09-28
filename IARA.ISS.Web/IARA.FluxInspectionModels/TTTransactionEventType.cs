namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTTransactionEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTTransactionEventType
    {

        private DateTimeType occurrenceDateTimeField;

        private DateTimeType recordedDateTimeField;

        private IDType parentObjectIDField;

        private IDType[] objectInstanceIDField;

        private CodeType actionCodeField;

        private CodeType businessStepCodeField;

        private CodeType dispositionCodeField;

        private TTLocationType readPointRelatedTTLocationField;

        private TTLocationType businessRelatedTTLocationField;

        private TTTradeTransactionType[] specifiedTTTradeTransactionField;

        private TTEventElementType[] quantitySpecifiedTTEventElementField;

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

        public IDType ParentObjectID
        {
            get => this.parentObjectIDField;
            set => this.parentObjectIDField = value;
        }

        [XmlElement("ObjectInstanceID")]
        public IDType[] ObjectInstanceID
        {
            get => this.objectInstanceIDField;
            set => this.objectInstanceIDField = value;
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

        [XmlElement("QuantitySpecifiedTTEventElement")]
        public TTEventElementType[] QuantitySpecifiedTTEventElement
        {
            get => this.quantitySpecifiedTTEventElementField;
            set => this.quantitySpecifiedTTEventElementField = value;
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