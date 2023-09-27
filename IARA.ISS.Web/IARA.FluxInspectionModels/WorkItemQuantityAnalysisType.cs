namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WorkItemQuantityAnalysis", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WorkItemQuantityAnalysisType
    {

        private IDType idField;

        private QuantityType actualQuantityField;

        private TextType descriptionField;

        private PercentType actualQuantityPercentField;

        private CodeType statusCodeField;

        private CodeType typeCodeField;

        private CodeType[] primaryClassificationCodeField;

        private CodeType[] alternativeClassificationCodeField;

        private CodeType contractualLanguageCodeField;

        private WorkItemDimensionType[] actualQuantityWorkItemDimensionField;

        private WorkItemQuantityAnalysisType[] breakdownWorkItemQuantityAnalysisField;

        private RecordedStatusType[] changedRecordedStatusField;

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

        public QuantityType ActualQuantity
        {
            get
            {
                return this.actualQuantityField;
            }
            set
            {
                this.actualQuantityField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public PercentType ActualQuantityPercent
        {
            get
            {
                return this.actualQuantityPercentField;
            }
            set
            {
                this.actualQuantityPercentField = value;
            }
        }

        public CodeType StatusCode
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

        public CodeType TypeCode
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

        [XmlElement("ActualQuantityWorkItemDimension")]
        public WorkItemDimensionType[] ActualQuantityWorkItemDimension
        {
            get
            {
                return this.actualQuantityWorkItemDimensionField;
            }
            set
            {
                this.actualQuantityWorkItemDimensionField = value;
            }
        }

        [XmlElement("BreakdownWorkItemQuantityAnalysis")]
        public WorkItemQuantityAnalysisType[] BreakdownWorkItemQuantityAnalysis
        {
            get
            {
                return this.breakdownWorkItemQuantityAnalysisField;
            }
            set
            {
                this.breakdownWorkItemQuantityAnalysisField = value;
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
    }
}
