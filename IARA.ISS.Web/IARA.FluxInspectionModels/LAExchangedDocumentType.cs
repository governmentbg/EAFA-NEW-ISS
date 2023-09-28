namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LAExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LAExchangedDocumentType
    {

        private IDType idField;

        private DateTimeType issueDateTimeField;

        private CodeType typeCodeField;

        private CodeType statusCodeField;

        private QuantityType itemQuantityField;

        private SpecifiedPartyType senderSpecifiedPartyField;

        private SpecifiedPartyType recipientSpecifiedPartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
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

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public QuantityType ItemQuantity
        {
            get => this.itemQuantityField;
            set => this.itemQuantityField = value;
        }

        public SpecifiedPartyType SenderSpecifiedParty
        {
            get => this.senderSpecifiedPartyField;
            set => this.senderSpecifiedPartyField = value;
        }

        public SpecifiedPartyType RecipientSpecifiedParty
        {
            get => this.recipientSpecifiedPartyField;
            set => this.recipientSpecifiedPartyField = value;
        }
    }
}