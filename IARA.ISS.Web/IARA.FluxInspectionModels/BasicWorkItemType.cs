namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BasicWorkItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BasicWorkItemType
    {

        private IDType idField;

        private IDType referenceIDField;

        private CodeType[] primaryClassificationCodeField;

        private CodeType[] alternativeClassificationCodeField;

        private CodeType[] typeCodeField;

        private TextType[] commentField;

        private QuantityType totalQuantityField;

        private CodeType totalQuantityClassificationCodeField;

        private ValueType indexValueField;

        private CodeType[] statusCodeField;

        private BinaryObjectType[] referenceFileBinaryObjectField;

        private TextType indexField;

        private CodeType[] requestedActionCodeField;

        private IDType priceListItemIDField;

        private CodeType contractualLanguageCodeField;

        private WorkItemComplexDescriptionType[] actualWorkItemComplexDescriptionField;

        private WorkItemQuantityAnalysisType[] totalQuantityWorkItemQuantityAnalysisField;

        private CalculatedPriceType[] unitCalculatedPriceField;

        private CalculatedPriceType[] totalCalculatedPriceField;

        private BasicWorkItemType[] subordinateBasicWorkItemField;

        private RecordedStatusType[] changedRecordedStatusField;

        private IdentifiedBinaryFileType[] referencedIdentifiedBinaryFileField;

        private BasicWorkItemType[] itemBasicWorkItemField;

        private SpecifiedBinaryFileType[] referencedSpecifiedBinaryFileField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public IDType ReferenceID
        {
            get
            {
                return this.referenceIDField;
            }
            set
            {
                this.referenceIDField = value;
            }
        }

        [XmlElement("PrimaryClassificationCode")]
        public CodeType[] PrimaryClassificationCode
        {
            get
            {
                return this.primaryClassificationCodeField;
            }
            set
            {
                this.primaryClassificationCodeField = value;
            }
        }

        [XmlElement("AlternativeClassificationCode")]
        public CodeType[] AlternativeClassificationCode
        {
            get
            {
                return this.alternativeClassificationCodeField;
            }
            set
            {
                this.alternativeClassificationCodeField = value;
            }
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        [XmlElement("Comment")]
        public TextType[] Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        public QuantityType TotalQuantity
        {
            get
            {
                return this.totalQuantityField;
            }
            set
            {
                this.totalQuantityField = value;
            }
        }

        public CodeType TotalQuantityClassificationCode
        {
            get
            {
                return this.totalQuantityClassificationCodeField;
            }
            set
            {
                this.totalQuantityClassificationCodeField = value;
            }
        }

        public ValueType IndexValue
        {
            get
            {
                return this.indexValueField;
            }
            set
            {
                this.indexValueField = value;
            }
        }

        [XmlElement("StatusCode")]
        public CodeType[] StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        [XmlElement("ReferenceFileBinaryObject")]
        public BinaryObjectType[] ReferenceFileBinaryObject
        {
            get
            {
                return this.referenceFileBinaryObjectField;
            }
            set
            {
                this.referenceFileBinaryObjectField = value;
            }
        }

        public TextType Index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        [XmlElement("RequestedActionCode")]
        public CodeType[] RequestedActionCode
        {
            get
            {
                return this.requestedActionCodeField;
            }
            set
            {
                this.requestedActionCodeField = value;
            }
        }

        public IDType PriceListItemID
        {
            get
            {
                return this.priceListItemIDField;
            }
            set
            {
                this.priceListItemIDField = value;
            }
        }

        public CodeType ContractualLanguageCode
        {
            get
            {
                return this.contractualLanguageCodeField;
            }
            set
            {
                this.contractualLanguageCodeField = value;
            }
        }

        [XmlElement("ActualWorkItemComplexDescription")]
        public WorkItemComplexDescriptionType[] ActualWorkItemComplexDescription
        {
            get
            {
                return this.actualWorkItemComplexDescriptionField;
            }
            set
            {
                this.actualWorkItemComplexDescriptionField = value;
            }
        }

        [XmlElement("TotalQuantityWorkItemQuantityAnalysis")]
        public WorkItemQuantityAnalysisType[] TotalQuantityWorkItemQuantityAnalysis
        {
            get
            {
                return this.totalQuantityWorkItemQuantityAnalysisField;
            }
            set
            {
                this.totalQuantityWorkItemQuantityAnalysisField = value;
            }
        }

        [XmlElement("UnitCalculatedPrice")]
        public CalculatedPriceType[] UnitCalculatedPrice
        {
            get
            {
                return this.unitCalculatedPriceField;
            }
            set
            {
                this.unitCalculatedPriceField = value;
            }
        }

        [XmlElement("TotalCalculatedPrice")]
        public CalculatedPriceType[] TotalCalculatedPrice
        {
            get
            {
                return this.totalCalculatedPriceField;
            }
            set
            {
                this.totalCalculatedPriceField = value;
            }
        }

        [XmlElement("SubordinateBasicWorkItem")]
        public BasicWorkItemType[] SubordinateBasicWorkItem
        {
            get
            {
                return this.subordinateBasicWorkItemField;
            }
            set
            {
                this.subordinateBasicWorkItemField = value;
            }
        }

        [XmlElement("ChangedRecordedStatus")]
        public RecordedStatusType[] ChangedRecordedStatus
        {
            get
            {
                return this.changedRecordedStatusField;
            }
            set
            {
                this.changedRecordedStatusField = value;
            }
        }

        [XmlElement("ReferencedIdentifiedBinaryFile")]
        public IdentifiedBinaryFileType[] ReferencedIdentifiedBinaryFile
        {
            get
            {
                return this.referencedIdentifiedBinaryFileField;
            }
            set
            {
                this.referencedIdentifiedBinaryFileField = value;
            }
        }

        [XmlElement("ItemBasicWorkItem")]
        public BasicWorkItemType[] ItemBasicWorkItem
        {
            get
            {
                return this.itemBasicWorkItemField;
            }
            set
            {
                this.itemBasicWorkItemField = value;
            }
        }

        [XmlElement("ReferencedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] ReferencedSpecifiedBinaryFile
        {
            get
            {
                return this.referencedSpecifiedBinaryFileField;
            }
            set
            {
                this.referencedSpecifiedBinaryFileField = value;
            }
        }
    }
}