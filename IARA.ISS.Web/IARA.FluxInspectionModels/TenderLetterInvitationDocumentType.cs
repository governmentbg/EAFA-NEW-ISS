namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderLetterInvitationDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderLetterInvitationDocumentType
    {

        private IDType idField;

        private TextType nameField;

        private TextType remarksField;

        private TextType nominationDescriptionField;

        private DateTimeType issueDateTimeField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Remarks
        {
            get => this.remarksField;
            set => this.remarksField = value;
        }

        public TextType NominationDescription
        {
            get => this.nominationDescriptionField;
            set => this.nominationDescriptionField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }
    }
}