namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryAccountingVoucher", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryAccountingVoucherType
    {

        private TextType storageLocationField;

        private TextType receivingDepartmentField;

        private IDType idField;

        private AccountingVoucherMediumCodeType mediumCodeField;

        private DateTimeType taxPointDateTimeField;

        private IDType importedFileIDField;

        private BinaryObjectType attachmentBinaryObjectField;

        private AAAEntryDocumentType[] relatedAAAEntryDocumentField;

        public TextType StorageLocation
        {
            get
            {
                return this.storageLocationField;
            }
            set
            {
                this.storageLocationField = value;
            }
        }

        public TextType ReceivingDepartment
        {
            get
            {
                return this.receivingDepartmentField;
            }
            set
            {
                this.receivingDepartmentField = value;
            }
        }

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

        public AccountingVoucherMediumCodeType MediumCode
        {
            get
            {
                return this.mediumCodeField;
            }
            set
            {
                this.mediumCodeField = value;
            }
        }

        public DateTimeType TaxPointDateTime
        {
            get
            {
                return this.taxPointDateTimeField;
            }
            set
            {
                this.taxPointDateTimeField = value;
            }
        }

        public IDType ImportedFileID
        {
            get
            {
                return this.importedFileIDField;
            }
            set
            {
                this.importedFileIDField = value;
            }
        }

        public BinaryObjectType AttachmentBinaryObject
        {
            get
            {
                return this.attachmentBinaryObjectField;
            }
            set
            {
                this.attachmentBinaryObjectField = value;
            }
        }

        [XmlElement("RelatedAAAEntryDocument")]
        public AAAEntryDocumentType[] RelatedAAAEntryDocument
        {
            get
            {
                return this.relatedAAAEntryDocumentField;
            }
            set
            {
                this.relatedAAAEntryDocumentField = value;
            }
        }
    }
}