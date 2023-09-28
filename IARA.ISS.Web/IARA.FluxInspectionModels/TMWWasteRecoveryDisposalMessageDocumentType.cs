namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWWasteRecoveryDisposalMessageDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWWasteRecoveryDisposalMessageDocumentType
    {

        private IDType preliminaryIDField;

        private CodeType typeCodeField;

        private BinaryObjectType[] attachmentBinaryObjectField;

        private TMWBusinessTransactionPartyType senderTMWBusinessTransactionPartyField;

        private TMWBusinessTransactionPartyType recipientTMWBusinessTransactionPartyField;

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

        [XmlElement("AttachmentBinaryObject")]
        public BinaryObjectType[] AttachmentBinaryObject
        {
            get => this.attachmentBinaryObjectField;
            set => this.attachmentBinaryObjectField = value;
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