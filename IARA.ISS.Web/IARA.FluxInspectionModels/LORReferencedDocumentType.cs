namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LORReferencedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LORReferencedDocumentType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private DateTimeType issueDateTimeField;

        private IndicatorType controlRequirementIndicatorField;

        private DateTimeType creationDateTimeField;

        private CodeType statusCodeField;

        private IndicatorType copyIndicatorField;

        private NumericType lineCountNumericField;

        private CodeType purposeCodeField;

        private TextType informationField;

        private IDType sequenceIDField;

        private NumericType reportCountNumericField;

        private LaboratoryObservationPartyType senderLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType recipientLaboratoryObservationPartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public IndicatorType ControlRequirementIndicator
        {
            get => this.controlRequirementIndicatorField;
            set => this.controlRequirementIndicatorField = value;
        }

        public DateTimeType CreationDateTime
        {
            get => this.creationDateTimeField;
            set => this.creationDateTimeField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public IndicatorType CopyIndicator
        {
            get => this.copyIndicatorField;
            set => this.copyIndicatorField = value;
        }

        public NumericType LineCountNumeric
        {
            get => this.lineCountNumericField;
            set => this.lineCountNumericField = value;
        }

        public CodeType PurposeCode
        {
            get => this.purposeCodeField;
            set => this.purposeCodeField = value;
        }

        public TextType Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public IDType SequenceID
        {
            get => this.sequenceIDField;
            set => this.sequenceIDField = value;
        }

        public NumericType ReportCountNumeric
        {
            get => this.reportCountNumericField;
            set => this.reportCountNumericField = value;
        }

        public LaboratoryObservationPartyType SenderLaboratoryObservationParty
        {
            get => this.senderLaboratoryObservationPartyField;
            set => this.senderLaboratoryObservationPartyField = value;
        }

        public LaboratoryObservationPartyType RecipientLaboratoryObservationParty
        {
            get => this.recipientLaboratoryObservationPartyField;
            set => this.recipientLaboratoryObservationPartyField = value;
        }
    }
}