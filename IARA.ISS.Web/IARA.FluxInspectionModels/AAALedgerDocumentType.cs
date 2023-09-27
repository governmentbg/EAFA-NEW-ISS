namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAALedgerDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAALedgerDocumentType
    {

        private IDType idField;

        private AccountingDocumentTypeCodeType typeCodeField;

        private TextType purposeField;

        private DateTimeType receiptDateTimeField;

        private DateTimeType creationDateTimeField;

        private AAALedgerAccountingVoucherType[] referencedAAALedgerAccountingVoucherField;

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

        public AccountingDocumentTypeCodeType TypeCode
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

        public DateTimeType ReceiptDateTime
        {
            get
            {
                return this.receiptDateTimeField;
            }
            set
            {
                this.receiptDateTimeField = value;
            }
        }

        public DateTimeType CreationDateTime
        {
            get
            {
                return this.creationDateTimeField;
            }
            set
            {
                this.creationDateTimeField = value;
            }
        }

        [XmlElement("ReferencedAAALedgerAccountingVoucher")]
        public AAALedgerAccountingVoucherType[] ReferencedAAALedgerAccountingVoucher
        {
            get
            {
                return this.referencedAAALedgerAccountingVoucherField;
            }
            set
            {
                this.referencedAAALedgerAccountingVoucherField = value;
            }
        }
    }
}