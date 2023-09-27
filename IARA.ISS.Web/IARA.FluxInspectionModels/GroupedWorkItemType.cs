namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("GroupedWorkItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class GroupedWorkItemType
    {

        private IDType idField;

        private CodeType[] primaryClassificationCodeField;

        private CodeType[] alternativeClassificationCodeField;

        private CodeType[] typeCodeField;

        private TextType[] commentField;

        private QuantityType totalQuantityField;

        private TextType indexField;

        private CodeType[] requestedActionCodeField;

        private IDType priceListItemIDField;

        private CodeType contractualLanguageCodeField;

        private CalculatedPriceType[] totalCalculatedPriceField;

        private GroupedWorkItemType[] itemGroupedWorkItemField;

        private BasicWorkItemType[] itemBasicWorkItemField;

        private RecordedStatusType[] changedRecordedStatusField;

        private IdentifiedBinaryFileType[] referencedIdentifiedBinaryFileField;

        private WorkItemComplexDescriptionType[] actualWorkItemComplexDescriptionField;

        private SpecifiedBinaryFileType[] referencedSpecifiedBinaryFileField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("PrimaryClassificationCode")]
        public CodeType[] PrimaryClassificationCode
        {
            get => this.primaryClassificationCodeField;
            set => this.primaryClassificationCodeField = value;
        }

        [XmlElement("AlternativeClassificationCode")]
        public CodeType[] AlternativeClassificationCode
        {
            get => this.alternativeClassificationCodeField;
            set => this.alternativeClassificationCodeField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("Comment")]
        public TextType[] Comment
        {
            get => this.commentField;
            set => this.commentField = value;
        }

        public QuantityType TotalQuantity
        {
            get => this.totalQuantityField;
            set => this.totalQuantityField = value;
        }

        public TextType Index
        {
            get => this.indexField;
            set => this.indexField = value;
        }

        [XmlElement("RequestedActionCode")]
        public CodeType[] RequestedActionCode
        {
            get => this.requestedActionCodeField;
            set => this.requestedActionCodeField = value;
        }

        public IDType PriceListItemID
        {
            get => this.priceListItemIDField;
            set => this.priceListItemIDField = value;
        }

        public CodeType ContractualLanguageCode
        {
            get => this.contractualLanguageCodeField;
            set => this.contractualLanguageCodeField = value;
        }

        [XmlElement("TotalCalculatedPrice")]
        public CalculatedPriceType[] TotalCalculatedPrice
        {
            get => this.totalCalculatedPriceField;
            set => this.totalCalculatedPriceField = value;
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

        [XmlElement("ActualWorkItemComplexDescription")]
        public WorkItemComplexDescriptionType[] ActualWorkItemComplexDescription
        {
            get => this.actualWorkItemComplexDescriptionField;
            set => this.actualWorkItemComplexDescriptionField = value;
        }

        [XmlElement("ReferencedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] ReferencedSpecifiedBinaryFile
        {
            get => this.referencedSpecifiedBinaryFileField;
            set => this.referencedSpecifiedBinaryFileField = value;
        }
    }
}