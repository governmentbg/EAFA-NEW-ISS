namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookAccountingEntryLine", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookAccountingEntryLineType
    {

        private TextType commentField;

        private AccountingEntryLineCategoryCodeType categoryCodeField;

        private AccountingEntryLineSourceCodeType sourceCodeField;

        private DateTimeType lastChangeDateTimeField;

        private TextType lastChangeResponsiblePersonNameField;

        private QuantityType actualQuantityField;

        private TextType natureField;

        private AAAJournalBookAccountingLineIndexType[] specifiedAAAJournalBookAccountingLineIndexField;

        private AAAJournalBookMonetaryAllocationType[] repeatedAAAJournalBookMonetaryAllocationField;

        private AAAJournalBookMonetaryInstalmentType[] repeatedAAAJournalBookMonetaryInstalmentField;

        private AAAJournalBookAccountingLineMonetaryValueType[] relatedAAAJournalBookAccountingLineMonetaryValueField;

        private AAAJournalBookTaxType relatedAAAJournalBookTaxField;

        private AAAJournalBookPaymentType[] repeatedAAAJournalBookPaymentField;

        public TextType Comment
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

        public AccountingEntryLineCategoryCodeType CategoryCode
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

        public AccountingEntryLineSourceCodeType SourceCode
        {
            get
            {
                return this.sourceCodeField;
            }
            set
            {
                this.sourceCodeField = value;
            }
        }

        public DateTimeType LastChangeDateTime
        {
            get
            {
                return this.lastChangeDateTimeField;
            }
            set
            {
                this.lastChangeDateTimeField = value;
            }
        }

        public TextType LastChangeResponsiblePersonName
        {
            get
            {
                return this.lastChangeResponsiblePersonNameField;
            }
            set
            {
                this.lastChangeResponsiblePersonNameField = value;
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

        public TextType Nature
        {
            get
            {
                return this.natureField;
            }
            set
            {
                this.natureField = value;
            }
        }

        [XmlElement("SpecifiedAAAJournalBookAccountingLineIndex")]
        public AAAJournalBookAccountingLineIndexType[] SpecifiedAAAJournalBookAccountingLineIndex
        {
            get
            {
                return this.specifiedAAAJournalBookAccountingLineIndexField;
            }
            set
            {
                this.specifiedAAAJournalBookAccountingLineIndexField = value;
            }
        }

        [XmlElement("RepeatedAAAJournalBookMonetaryAllocation")]
        public AAAJournalBookMonetaryAllocationType[] RepeatedAAAJournalBookMonetaryAllocation
        {
            get
            {
                return this.repeatedAAAJournalBookMonetaryAllocationField;
            }
            set
            {
                this.repeatedAAAJournalBookMonetaryAllocationField = value;
            }
        }

        [XmlElement("RepeatedAAAJournalBookMonetaryInstalment")]
        public AAAJournalBookMonetaryInstalmentType[] RepeatedAAAJournalBookMonetaryInstalment
        {
            get
            {
                return this.repeatedAAAJournalBookMonetaryInstalmentField;
            }
            set
            {
                this.repeatedAAAJournalBookMonetaryInstalmentField = value;
            }
        }

        [XmlElement("RelatedAAAJournalBookAccountingLineMonetaryValue")]
        public AAAJournalBookAccountingLineMonetaryValueType[] RelatedAAAJournalBookAccountingLineMonetaryValue
        {
            get
            {
                return this.relatedAAAJournalBookAccountingLineMonetaryValueField;
            }
            set
            {
                this.relatedAAAJournalBookAccountingLineMonetaryValueField = value;
            }
        }

        public AAAJournalBookTaxType RelatedAAAJournalBookTax
        {
            get
            {
                return this.relatedAAAJournalBookTaxField;
            }
            set
            {
                this.relatedAAAJournalBookTaxField = value;
            }
        }

        [XmlElement("RepeatedAAAJournalBookPayment")]
        public AAAJournalBookPaymentType[] RepeatedAAAJournalBookPayment
        {
            get
            {
                return this.repeatedAAAJournalBookPaymentField;
            }
            set
            {
                this.repeatedAAAJournalBookPaymentField = value;
            }
        }
    }
}