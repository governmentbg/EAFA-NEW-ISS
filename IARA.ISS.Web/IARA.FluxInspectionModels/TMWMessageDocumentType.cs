namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWMessageDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWMessageDocumentType
    {

        private IDType preliminaryIDField;

        private BinaryObjectType[] attachmentBinaryObjectField;

        private TMWBusinessTransactionPartyType senderTMWBusinessTransactionPartyField;

        private TMWBusinessTransactionPartyType recipientTMWBusinessTransactionPartyField;

        public IDType PreliminaryID
        {
            get => this.preliminaryIDField;
            set => this.preliminaryIDField = value;
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