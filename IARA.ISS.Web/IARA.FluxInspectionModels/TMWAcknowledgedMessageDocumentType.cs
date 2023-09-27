namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWAcknowledgedMessageDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWAcknowledgedMessageDocumentType
    {

        private IDType idField;

        private IDType preliminaryIDField;

        private CodeType typeCodeField;

        private CodeType statusCodeField;

        private TextType rejectionReasonField;

        private TMWBusinessTransactionPartyType senderTMWBusinessTransactionPartyField;

        private TMWBusinessTransactionPartyType recipientTMWBusinessTransactionPartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType PreliminaryID
        {
            get => this.preliminaryIDField;
            set => this.preliminaryIDField = value;
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

        public TextType RejectionReason
        {
            get => this.rejectionReasonField;
            set => this.rejectionReasonField = value;
        }

        public TMWBusinessTransactionPartyType SenderTMWBusinessTransactionParty
        {
            get => this.senderTMWBusinessTransactionPartyField;
            set => this.senderTMWBusinessTransactionPartyField = value;
        }

        public TMWBusinessTransactionPartyType RecipientTMWBusinessTransactionParty
        {
            get => this.recipientTMWBusinessTransactionPartyField;
            set => this.recipientTMWBusinessTransactionPartyField = value;
        }
    }
}