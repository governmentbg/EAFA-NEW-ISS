namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLUXResponseDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLUXResponseDocumentType
    {

        private IDType[] idField;

        private IDType referencedIDField;

        private DateTimeType creationDateTimeField;

        private CodeType responseCodeField;

        private TextType remarksField;

        private TextType rejectionReasonField;

        private CodeType typeCodeField;

        private ValidationResultDocumentType[] relatedValidationResultDocumentField;

        private FLUXPartyType respondentFLUXPartyField;

        [XmlElement("ID")]
        public IDType[] ID
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

        public IDType ReferencedID
        {
            get
            {
                return this.referencedIDField;
            }
            set
            {
                this.referencedIDField = value;
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

        public CodeType ResponseCode
        {
            get
            {
                return this.responseCodeField;
            }
            set
            {
                this.responseCodeField = value;
            }
        }

        public TextType Remarks
        {
            get
            {
                return this.remarksField;
            }
            set
            {
                this.remarksField = value;
            }
        }

        public TextType RejectionReason
        {
            get
            {
                return this.rejectionReasonField;
            }
            set
            {
                this.rejectionReasonField = value;
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

        [XmlElement("RelatedValidationResultDocument")]
        public ValidationResultDocumentType[] RelatedValidationResultDocument
        {
            get
            {
                return this.relatedValidationResultDocumentField;
            }
            set
            {
                this.relatedValidationResultDocumentField = value;
            }
        }

        public FLUXPartyType RespondentFLUXParty
        {
            get
            {
                return this.respondentFLUXPartyField;
            }
            set
            {
                this.respondentFLUXPartyField = value;
            }
        }
    }
}