namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAALedgerAccountingEntryLine", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAALedgerAccountingEntryLineType
    {

        private TextType commentField;

        private AccountingEntryLineCategoryCodeType categoryCodeField;

        private AccountingEntryLineSourceCodeType sourceCodeField;

        private DateTimeType lastChangeDateTimeField;

        private TextType lastChangeResponsiblePersonNameField;

        private QuantityType actualQuantityField;

        private TextType natureField;

        private AAALedgerAccountingEntryType connectedAAALedgerAccountingEntryField;

        private AAALedgerAccountingLineIndexType specifiedAAALedgerAccountingLineIndexField;

        private AAALedgerTaxType[] relatedAAALedgerTaxField;

        private AAALedgerMonetaryAllocationType repeatedAAALedgerMonetaryAllocationField;

        private AAALedgerMonetaryInstalmentType repeatedAAALedgerMonetaryInstalmentField;

        private AAALedgerAccountingLineMonetaryValueType relatedAAALedgerAccountingLineMonetaryValueField;

        private AAALedgerPaymentType[] repeatedAAALedgerPaymentField;

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

        public AAALedgerAccountingEntryType ConnectedAAALedgerAccountingEntry
        {
            get
            {
                return this.connectedAAALedgerAccountingEntryField;
            }
            set
            {
                this.connectedAAALedgerAccountingEntryField = value;
            }
        }

        public AAALedgerAccountingLineIndexType SpecifiedAAALedgerAccountingLineIndex
        {
            get
            {
                return this.specifiedAAALedgerAccountingLineIndexField;
            }
            set
            {
                this.specifiedAAALedgerAccountingLineIndexField = value;
            }
        }

        [XmlElement("RelatedAAALedgerTax")]
        public AAALedgerTaxType[] RelatedAAALedgerTax
        {
            get
            {
                return this.relatedAAALedgerTaxField;
            }
            set
            {
                this.relatedAAALedgerTaxField = value;
            }
        }

        public AAALedgerMonetaryAllocationType RepeatedAAALedgerMonetaryAllocation
        {
            get
            {
                return this.repeatedAAALedgerMonetaryAllocationField;
            }
            set
            {
                this.repeatedAAALedgerMonetaryAllocationField = value;
            }
        }

        public AAALedgerMonetaryInstalmentType RepeatedAAALedgerMonetaryInstalment
        {
            get
            {
                return this.repeatedAAALedgerMonetaryInstalmentField;
            }
            set
            {
                this.repeatedAAALedgerMonetaryInstalmentField = value;
            }
        }

        public AAALedgerAccountingLineMonetaryValueType RelatedAAALedgerAccountingLineMonetaryValue
        {
            get
            {
                return this.relatedAAALedgerAccountingLineMonetaryValueField;
            }
            set
            {
                this.relatedAAALedgerAccountingLineMonetaryValueField = value;
            }
        }

        [XmlElement("RepeatedAAALedgerPayment")]
        public AAALedgerPaymentType[] RepeatedAAALedgerPayment
        {
            get
            {
                return this.repeatedAAALedgerPaymentField;
            }
            set
            {
                this.repeatedAAALedgerPaymentField = value;
            }
        }
    }
}