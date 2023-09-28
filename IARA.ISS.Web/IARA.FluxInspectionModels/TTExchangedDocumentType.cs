namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTExchangedDocumentType
    {

        private IDType idField;

        private TextType descriptionField;

        private CodeType typeCodeField;

        private DateTimeType issueDateTimeField;

        private CodeType statusCodeField;

        private CodeType purposeCodeField;

        private TTPartyType senderSpecifiedTTPartyField;

        private TTPartyType receiverSpecifiedTTPartyField;

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

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public CodeType PurposeCode
        {
            get => this.purposeCodeField;
            set => this.purposeCodeField = value;
        }

        public TTPartyType SenderSpecifiedTTParty
        {
            get => this.senderSpecifiedTTPartyField;
            set => this.senderSpecifiedTTPartyField = value;
        }

        public TTPartyType ReceiverSpecifiedTTParty
        {
            get => this.receiverSpecifiedTTPartyField;
            set => this.receiverSpecifiedTTPartyField = value;
        }
    }
}