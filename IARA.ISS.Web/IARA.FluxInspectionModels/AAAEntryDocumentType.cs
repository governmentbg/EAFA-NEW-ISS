namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryDocumentType
    {

        private IDType idField;

        private AccountingDocumentCodeType typeCodeField;

        private TextType purposeField;

        private DateTimeType receiptDateTimeField;

        private DateTimeType creationDateTimeField;

        private AAAEntryAccountingEntryType[] justifiedAAAEntryAccountingEntryField;

        private AAAEntryBinaryFileType[] attachedAAAEntryBinaryFileField;

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

        [XmlElement("JustifiedAAAEntryAccountingEntry")]
        public AAAEntryAccountingEntryType[] JustifiedAAAEntryAccountingEntry
        {
            get
            {
                return this.justifiedAAAEntryAccountingEntryField;
            }
            set
            {
                this.justifiedAAAEntryAccountingEntryField = value;
            }
        }

        [XmlElement("AttachedAAAEntryBinaryFile")]
        public AAAEntryBinaryFileType[] AttachedAAAEntryBinaryFile
        {
            get
            {
                return this.attachedAAAEntryBinaryFileField;
            }
            set
            {
                this.attachedAAAEntryBinaryFileField = value;
            }
        }
    }
}