namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryAccountingEntryLine", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryAccountingEntryLineType
    {

        private TextType commentField;

        private AccountingEntryLineCategoryCodeType categoryCodeField;

        private AccountingEntryLineSourceCodeType sourceCodeField;

        private DateTimeType lastChangeDateTimeField;

        private TextType lastChangeResponsiblePersonNameField;

        private QuantityType actualQuantityField;

        private TextType natureField;

        private AAAEntryAccountingLineIndexType[] specifiedAAAEntryAccountingLineIndexField;

        private AAAEntryMonetaryAllocationType[] repeatedAAAEntryMonetaryAllocationField;

        private AAAEntryMonetaryInstalmentType[] repeatedAAAEntryMonetaryInstalmentField;

        private AAAEntryAccountingLineMonetaryValueType[] relatedAAAEntryAccountingLineMonetaryValueField;

        private AAAEntryTaxType relatedAAAEntryTaxField;

        private AAAEntryPaymentType[] repeatedAAAEntryPaymentField;

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

        [XmlElement("SpecifiedAAAEntryAccountingLineIndex")]
        public AAAEntryAccountingLineIndexType[] SpecifiedAAAEntryAccountingLineIndex
        {
            get
            {
                return this.specifiedAAAEntryAccountingLineIndexField;
            }
            set
            {
                this.specifiedAAAEntryAccountingLineIndexField = value;
            }
        }

        [XmlElement("RepeatedAAAEntryMonetaryAllocation")]
        public AAAEntryMonetaryAllocationType[] RepeatedAAAEntryMonetaryAllocation
        {
            get
            {
                return this.repeatedAAAEntryMonetaryAllocationField;
            }
            set
            {
                this.repeatedAAAEntryMonetaryAllocationField = value;
            }
        }

        [XmlElement("RepeatedAAAEntryMonetaryInstalment")]
        public AAAEntryMonetaryInstalmentType[] RepeatedAAAEntryMonetaryInstalment
        {
            get
            {
                return this.repeatedAAAEntryMonetaryInstalmentField;
            }
            set
            {
                this.repeatedAAAEntryMonetaryInstalmentField = value;
            }
        }

        [XmlElement("RelatedAAAEntryAccountingLineMonetaryValue")]
        public AAAEntryAccountingLineMonetaryValueType[] RelatedAAAEntryAccountingLineMonetaryValue
        {
            get
            {
                return this.relatedAAAEntryAccountingLineMonetaryValueField;
            }
            set
            {
                this.relatedAAAEntryAccountingLineMonetaryValueField = value;
            }
        }

        public AAAEntryTaxType RelatedAAAEntryTax
        {
            get
            {
                return this.relatedAAAEntryTaxField;
            }
            set
            {
                this.relatedAAAEntryTaxField = value;
            }
        }

        [XmlElement("RepeatedAAAEntryPayment")]
        public AAAEntryPaymentType[] RepeatedAAAEntryPayment
        {
            get
            {
                return this.repeatedAAAEntryPaymentField;
            }
            set
            {
                this.repeatedAAAEntryPaymentField = value;
            }
        }
    }
}