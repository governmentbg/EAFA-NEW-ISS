namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LORExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LORExchangedDocumentType
    {

        private NumericType reportCountNumericField;

        private IDType idField;

        private TextType descriptionField;

        private DateTimeType issueDateTimeField;

        private CodeType typeCodeField;

        private IndicatorType copyIndicatorField;

        private IndicatorType controlRequirementIndicatorField;

        private CodeType purposeCodeField;

        private NumericType lineCountNumericField;

        private TextType informationField;

        private CodeType statusCodeField;

        private IDType sequenceIDField;

        private LaboratoryObservationPartyType senderLaboratoryObservationPartyField;

        private LaboratoryObservationPartyType recipientLaboratoryObservationPartyField;

        public NumericType ReportCountNumeric
        {
            get => this.reportCountNumericField;
            set => this.reportCountNumericField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public IndicatorType CopyIndicator
        {
            get => this.copyIndicatorField;
            set => this.copyIndicatorField = value;
        }

        public IndicatorType ControlRequirementIndicator
        {
            get => this.controlRequirementIndicatorField;
            set => this.controlRequirementIndicatorField = value;
        }

        public CodeType PurposeCode
        {
            get => this.purposeCodeField;
            set => this.purposeCodeField = value;
        }

        public NumericType LineCountNumeric
        {
            get => this.lineCountNumericField;
            set => this.lineCountNumericField = value;
        }

        public TextType Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public IDType SequenceID
        {
            get => this.sequenceIDField;
            set => this.sequenceIDField = value;
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