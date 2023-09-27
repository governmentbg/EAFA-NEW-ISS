namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProxyDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProxyDocumentType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private IDType messageIDField;

        private DateTimeType signedDateTimeField;

        private DateTimeType acceptanceDateTimeField;

        private TextType signatureLocationField;

        private DateTimeType cancellationDateTimeField;

        private ProxyPartyType issuerProxyPartyField;

        private ProxyPartyType[] recipientProxyPartyField;

        private ProxyPartyType[] businessOwnerNamedProxyPartyField;

        private DelimitedPeriodType effectiveDelimitedPeriodField;

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

        public IDType MessageID
        {
            get => this.messageIDField;
            set => this.messageIDField = value;
        }

        public DateTimeType SignedDateTime
        {
            get => this.signedDateTimeField;
            set => this.signedDateTimeField = value;
        }

        public DateTimeType AcceptanceDateTime
        {
            get => this.acceptanceDateTimeField;
            set => this.acceptanceDateTimeField = value;
        }

        public TextType SignatureLocation
        {
            get => this.signatureLocationField;
            set => this.signatureLocationField = value;
        }

        public DateTimeType CancellationDateTime
        {
            get => this.cancellationDateTimeField;
            set => this.cancellationDateTimeField = value;
        }

        public ProxyPartyType IssuerProxyParty
        {
            get => this.issuerProxyPartyField;
            set => this.issuerProxyPartyField = value;
        }

        [XmlElement("RecipientProxyParty")]
        public ProxyPartyType[] RecipientProxyParty
        {
            get => this.recipientProxyPartyField;
            set => this.recipientProxyPartyField = value;
        }

        [XmlElement("BusinessOwnerNamedProxyParty")]
        public ProxyPartyType[] BusinessOwnerNamedProxyParty
        {
            get => this.businessOwnerNamedProxyPartyField;
            set => this.businessOwnerNamedProxyPartyField = value;
        }

        public DelimitedPeriodType EffectiveDelimitedPeriod
        {
            get => this.effectiveDelimitedPeriodField;
            set => this.effectiveDelimitedPeriodField = value;
        }
    }
}