namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ValuationBreakdownStatement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ValuationBreakdownStatementType
    {

        private IDType idField;

        private TextType nameField;

        private TextType[] descriptionField;

        private IDType[] measurementMethodIDField;

        private DateTimeType creationDateTimeField;

        private CurrencyCodeType defaultCurrencyCodeField;

        private CodeType defaultLanguageCodeField;

        private TextType[] commentField;

        private CodeType[] typeCodeField;

        private CodeType[] requestedActionCodeField;

        private IDType priceListIDField;

        private CodeType contractualLanguageCodeField;

        private GroupedWorkItemType[] itemGroupedWorkItemField;

        private BasicWorkItemType[] itemBasicWorkItemField;

        private CalculatedPriceType[] totalCalculatedPriceField;

        private IdentifiedBinaryFileType[] creationIdentifiedBinaryFileField;

        private IdentifiedBinaryFileType[] readerIdentifiedBinaryFileField;

        private RecordedStatusType[] changedRecordedStatusField;

        private IdentifiedBinaryFileType[] referencedIdentifiedBinaryFileField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("MeasurementMethodID")]
        public IDType[] MeasurementMethodID
        {
            get => this.measurementMethodIDField;
            set => this.measurementMethodIDField = value;
        }

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public CurrencyCodeType DefaultCurrencyCode
        {
            get => this.defaultCurrencyCodeField;
            set => this.defaultCurrencyCodeField = value;
        }

        public CodeType DefaultLanguageCode
        {
            get => this.defaultLanguageCodeField;
            set => this.defaultLanguageCodeField = value;
        }

        [XmlElement("Comment")]
        public TextType[] Comment
        {
            get => this.commentField;
            set => this.commentField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("RequestedActionCode")]
        public CodeType[] RequestedActionCode
        {
            get => this.requestedActionCodeField;
            set => this.requestedActionCodeField = value;
        }

        public IDType PriceListID
        {
            get => this.priceListIDField;
            set => this.priceListIDField = value;
        }

        public CodeType ContractualLanguageCode
        {
            get => this.contractualLanguageCodeField;
            set => this.contractualLanguageCodeField = value;
        }

        [XmlElement("ItemGroupedWorkItem")]
        public GroupedWorkItemType[] ItemGroupedWorkItem
        {
            get => this.itemGroupedWorkItemField;
            set => this.itemGroupedWorkItemField = value;
        }

        [XmlElement("ItemBasicWorkItem")]
        public BasicWorkItemType[] ItemBasicWorkItem
        {
            get => this.itemBasicWorkItemField;
            set => this.itemBasicWorkItemField = value;
        }

        [XmlElement("TotalCalculatedPrice")]
        public CalculatedPriceType[] TotalCalculatedPrice
        {
            get => this.totalCalculatedPriceField;
            set => this.totalCalculatedPriceField = value;
        }

        [XmlElement("CreationIdentifiedBinaryFile")]
        public IdentifiedBinaryFileType[] CreationIdentifiedBinaryFile
        {
            get => this.creationIdentifiedBinaryFileField;
            set => this.creationIdentifiedBinaryFileField = value;
        }

        [XmlElement("ReaderIdentifiedBinaryFile")]
        public IdentifiedBinaryFileType[] ReaderIdentifiedBinaryFile
        {
            get => this.readerIdentifiedBinaryFileField;
            set => this.readerIdentifiedBinaryFileField = value;
        }

        [XmlElement("ChangedRecordedStatus")]
        public RecordedStatusType[] ChangedRecordedStatus
        {
            get => this.changedRecordedStatusField;
            set => this.changedRecordedStatusField = value;
        }

        [XmlElement("ReferencedIdentifiedBinaryFile")]
        public IdentifiedBinaryFileType[] ReferencedIdentifiedBinaryFile
        {
            get => this.referencedIdentifiedBinaryFileField;
            set => this.referencedIdentifiedBinaryFileField = value;
        }
    }
}