namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFNotification", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFNotificationType
    {

        private IDType idField;

        private IndicatorType communicationIndicatorField;

        private TextType[] communicationReasonField;

        private CodeType statusCodeField;

        private CodeType typeCodeField;

        private CodeType classificationCodeField;

        private CodeType basisCodeField;

        private DateTimeType creationDateTimeField;

        private RASFFCountryType originRASFFCountryField;

        private ReferencedLocationType originReferencedLocationField;

        private RASFFDamageType concernedRASFFDamageField;

        private RASFFProductType concernedRASFFProductField;

        private RASFFNoteType[] includedRASFFNoteField;

        private RASFFDocumentType[] referenceRASFFDocumentField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IndicatorType CommunicationIndicator
        {
            get => this.communicationIndicatorField;
            set => this.communicationIndicatorField = value;
        }

        [XmlElement("CommunicationReason")]
        public TextType[] CommunicationReason
        {
            get => this.communicationReasonField;
            set => this.communicationReasonField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType ClassificationCode
        {
            get => this.classificationCodeField;
            set => this.classificationCodeField = value;
        }

        public CodeType BasisCode
        {
            get => this.basisCodeField;
            set => this.basisCodeField = value;
        }

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public RASFFCountryType OriginRASFFCountry
        {
            get => this.originRASFFCountryField;
            set => this.originRASFFCountryField = value;
        }

        public ReferencedLocationType OriginReferencedLocation
        {
            get => this.originReferencedLocationField;
            set => this.originReferencedLocationField = value;
        }

        public RASFFDamageType ConcernedRASFFDamage
        {
            get => this.concernedRASFFDamageField;
            set => this.concernedRASFFDamageField = value;
        }

        public RASFFProductType ConcernedRASFFProduct
        {
            get => this.concernedRASFFProductField;
            set => this.concernedRASFFProductField = value;
        }

        [XmlElement("IncludedRASFFNote")]
        public RASFFNoteType[] IncludedRASFFNote
        {
            get => this.includedRASFFNoteField;
            set => this.includedRASFFNoteField = value;
        }

        [XmlElement("ReferenceRASFFDocument")]
        public RASFFDocumentType[] ReferenceRASFFDocument
        {
            get => this.referenceRASFFDocumentField;
            set => this.referenceRASFFDocumentField = value;
        }
    }
}