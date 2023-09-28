namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookAccountingEntry", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookAccountingEntryType
    {

        private IDType idField;

        private AccountingEntryProcessingCodeType processingStatusCodeField;

        private DateTimeType valueDateDateTimeField;

        private IndicatorType removalIndicatorField;

        private IndicatorType unbalancedIndicatorField;

        private IDType relatedEntryIDField;

        private AccountingEntryCategoryCodeType categoryCodeField;

        private TextType purposeField;

        private DateTimeType captureDateTimeField;

        private DateTimeType reversalDateTimeField;

        private DateTimeType validationDateTimeField;

        private AAAJournalBookDocumentType[] justificationAAAJournalBookDocumentField;

        private AAAJournalBookAccountingEntryLineType[] detailedAAAJournalBookAccountingEntryLineField;

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

        public AccountingEntryProcessingCodeType ProcessingStatusCode
        {
            get
            {
                return this.processingStatusCodeField;
            }
            set
            {
                this.processingStatusCodeField = value;
            }
        }

        public DateTimeType ValueDateDateTime
        {
            get
            {
                return this.valueDateDateTimeField;
            }
            set
            {
                this.valueDateDateTimeField = value;
            }
        }

        public IndicatorType RemovalIndicator
        {
            get
            {
                return this.removalIndicatorField;
            }
            set
            {
                this.removalIndicatorField = value;
            }
        }

        public IndicatorType UnbalancedIndicator
        {
            get
            {
                return this.unbalancedIndicatorField;
            }
            set
            {
                this.unbalancedIndicatorField = value;
            }
        }

        public IDType RelatedEntryID
        {
            get
            {
                return this.relatedEntryIDField;
            }
            set
            {
                this.relatedEntryIDField = value;
            }
        }

        public AccountingEntryCategoryCodeType CategoryCode
        {
            get
            {
                return this.categoryCodeField;
            }
            set
            {
                this.categoryCodeField = value;
            }
        }

        public TextType Purpose
        {
            get
            {
                return this.purposeField;
            }
            set
            {
                this.purposeField = value;
            }
        }

        public DateTimeType CaptureDateTime
        {
            get
            {
                return this.captureDateTimeField;
            }
            set
            {
                this.captureDateTimeField = value;
            }
        }

        public DateTimeType ReversalDateTime
        {
            get
            {
                return this.reversalDateTimeField;
            }
            set
            {
                this.reversalDateTimeField = value;
            }
        }

        public DateTimeType ValidationDateTime
        {
            get
            {
                return this.validationDateTimeField;
            }
            set
            {
                this.validationDateTimeField = value;
            }
        }

        [XmlElement("JustificationAAAJournalBookDocument")]
        public AAAJournalBookDocumentType[] JustificationAAAJournalBookDocument
        {
            get
            {
                return this.justificationAAAJournalBookDocumentField;
            }
            set
            {
                this.justificationAAAJournalBookDocumentField = value;
            }
        }

        [XmlElement("DetailedAAAJournalBookAccountingEntryLine")]
        public AAAJournalBookAccountingEntryLineType[] DetailedAAAJournalBookAccountingEntryLine
        {
            get
            {
                return this.detailedAAAJournalBookAccountingEntryLineField;
            }
            set
            {
                this.detailedAAAJournalBookAccountingEntryLineField = value;
            }
        }
    }
}