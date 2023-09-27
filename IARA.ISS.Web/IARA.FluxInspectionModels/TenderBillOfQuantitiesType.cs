namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderBillOfQuantities", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderBillOfQuantitiesType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private IDType[] measurementMethodIDField;

        private DateTimeType creationDateTimeField;

        private CodeType exchangePhaseStatusCodeField;

        private CodeType defaultCurrencyCodeField;

        private CodeType defaultLanguageCodeField;

        private TextType[] commentField;

        private BinaryObjectType[] referenceFileBinaryObjectField;

        private GroupedWorkItemType[] itemGroupedWorkItemField;

        private BasicWorkItemType[] itemBasicWorkItemField;

        private CalculatedPriceType[] totalCalculatedPriceField;

        private ProductionSoftwareType[] creationProductionSoftwareField;

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

        public TextType Description
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

        public CodeType ExchangePhaseStatusCode
        {
            get => this.exchangePhaseStatusCodeField;
            set => this.exchangePhaseStatusCodeField = value;
        }

        public CodeType DefaultCurrencyCode
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

        [XmlElement("ReferenceFileBinaryObject")]
        public BinaryObjectType[] ReferenceFileBinaryObject
        {
            get => this.referenceFileBinaryObjectField;
            set => this.referenceFileBinaryObjectField = value;
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

        [XmlElement("CreationProductionSoftware")]
        public ProductionSoftwareType[] CreationProductionSoftware
        {
            get => this.creationProductionSoftwareField;
            set => this.creationProductionSoftwareField = value;
        }
    }
}