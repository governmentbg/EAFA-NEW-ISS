namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookDocumentType
    {

        private IDType idField;

        private AccountingDocumentCodeType typeCodeField;

        private TextType purposeField;

        private DateTimeType receiptDateTimeField;

        private DateTimeType creationDateTimeField;

        private AAAJournalBookAccountingVoucherType[] referencedAAAJournalBookAccountingVoucherField;

        private AAAJournalBookBinaryFileType containingAAAJournalBookBinaryFileField;

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

        public AccountingDocumentCodeType TypeCode
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

        [XmlElement("ReferencedAAAJournalBookAccountingVoucher")]
        public AAAJournalBookAccountingVoucherType[] ReferencedAAAJournalBookAccountingVoucher
        {
            get
            {
                return this.referencedAAAJournalBookAccountingVoucherField;
            }
            set
            {
                this.referencedAAAJournalBookAccountingVoucherField = value;
            }
        }

        public AAAJournalBookBinaryFileType ContainingAAAJournalBookBinaryFile
        {
            get
            {
                return this.containingAAAJournalBookBinaryFileField;
            }
            set
            {
                this.containingAAAJournalBookBinaryFileField = value;
            }
        }
    }
}