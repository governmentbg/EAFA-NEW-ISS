namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIReferencedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIReferencedDocumentType
    {

        private IDType issuerAssignedIDField;

        private IDType uRIIDField;

        private string issueDateTimeField;

        private DocumentStatusCodeType statusCodeField;

        private IndicatorType copyIndicatorField;

        private IDType lineIDField;

        private ReferenceCodeType referenceTypeCodeField;

        private IDType globalIDField;

        private IDType revisionIDField;

        private TextType[] nameField;

        private TextType[] informationField;

        private IDType[] previousRevisionIDField;

        private TextType[] sectionNameField;

        private DocumentCodeType typeCodeField;

        private BinaryObjectType[] attachmentBinaryObjectField;

        private IDType pageIDField;

        private DateTimeType receiptDateTimeField;

        private IDType subordinateLineIDField;

        private MessageFunctionCodeType purposeCodeField;

        private CodeType categoryCodeField;

        private IDType requisitionerAssignedIDField;

        private CodeType[] subtypeCodeField;

        private CITradePartyType issuerCITradePartyField;

        private SpecifiedBinaryFileType[] attachedSpecifiedBinaryFileField;

        private CISpecifiedPeriodType effectiveCISpecifiedPeriodField;

        private CINoteType[] includedCINoteField;

        public IDType IssuerAssignedID
        {
            get
            {
                return this.issuerAssignedIDField;
            }
            set
            {
                this.issuerAssignedIDField = value;
            }
        }

        public IDType URIID
        {
            get
            {
                return this.uRIIDField;
            }
            set
            {
                this.uRIIDField = value;
            }
        }

        public string IssueDateTime
        {
            get
            {
                return this.issueDateTimeField;
            }
            set
            {
                this.issueDateTimeField = value;
            }
        }

        public DocumentStatusCodeType StatusCode
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

        public IndicatorType CopyIndicator
        {
            get
            {
                return this.copyIndicatorField;
            }
            set
            {
                this.copyIndicatorField = value;
            }
        }

        public IDType LineID
        {
            get
            {
                return this.lineIDField;
            }
            set
            {
                this.lineIDField = value;
            }
        }

        public ReferenceCodeType ReferenceTypeCode
        {
            get
            {
                return this.referenceTypeCodeField;
            }
            set
            {
                this.referenceTypeCodeField = value;
            }
        }

        public IDType GlobalID
        {
            get
            {
                return this.globalIDField;
            }
            set
            {
                this.globalIDField = value;
            }
        }

        public IDType RevisionID
        {
            get
            {
                return this.revisionIDField;
            }
            set
            {
                this.revisionIDField = value;
            }
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlElement("Information")]
        public TextType[] Information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }

        [XmlElement("PreviousRevisionID")]
        public IDType[] PreviousRevisionID
        {
            get
            {
                return this.previousRevisionIDField;
            }
            set
            {
                this.previousRevisionIDField = value;
            }
        }

        [XmlElement("SectionName")]
        public TextType[] SectionName
        {
            get
            {
                return this.sectionNameField;
            }
            set
            {
                this.sectionNameField = value;
            }
        }

        public DocumentCodeType TypeCode
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

        [XmlElement("AttachmentBinaryObject")]
        public BinaryObjectType[] AttachmentBinaryObject
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

        public IDType PageID
        {
            get
            {
                return this.pageIDField;
            }
            set
            {
                this.pageIDField = value;
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

        public IDType SubordinateLineID
        {
            get
            {
                return this.subordinateLineIDField;
            }
            set
            {
                this.subordinateLineIDField = value;
            }
        }

        public MessageFunctionCodeType PurposeCode
        {
            get
            {
                return this.purposeCodeField;
            }
            set
            {
                this.purposeCodeField = value;
            }
        }

        public CodeType CategoryCode
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

        public IDType RequisitionerAssignedID
        {
            get
            {
                return this.requisitionerAssignedIDField;
            }
            set
            {
                this.requisitionerAssignedIDField = value;
            }
        }

        [XmlElement("SubtypeCode")]
        public CodeType[] SubtypeCode
        {
            get
            {
                return this.subtypeCodeField;
            }
            set
            {
                this.subtypeCodeField = value;
            }
        }

        public CITradePartyType IssuerCITradeParty
        {
            get
            {
                return this.issuerCITradePartyField;
            }
            set
            {
                this.issuerCITradePartyField = value;
            }
        }

        [XmlElement("AttachedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] AttachedSpecifiedBinaryFile
        {
            get
            {
                return this.attachedSpecifiedBinaryFileField;
            }
            set
            {
                this.attachedSpecifiedBinaryFileField = value;
            }
        }

        public CISpecifiedPeriodType EffectiveCISpecifiedPeriod
        {
            get
            {
                return this.effectiveCISpecifiedPeriodField;
            }
            set
            {
                this.effectiveCISpecifiedPeriodField = value;
            }
        }

        [XmlElement("IncludedCINote")]
        public CINoteType[] IncludedCINote
        {
            get
            {
                return this.includedCINoteField;
            }
            set
            {
                this.includedCINoteField = value;
            }
        }
    }
}