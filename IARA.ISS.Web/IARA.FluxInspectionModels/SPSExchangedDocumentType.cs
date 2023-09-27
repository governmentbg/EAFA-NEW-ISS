namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSExchangedDocumentType
    {

        private TextType[] nameField;

        private IDType idField;

        private TextType[] descriptionField;

        private DocumentCodeType typeCodeField;

        private StatusCodeType statusCodeField;

        private DateTimeType issueDateTimeField;

        private IndicatorType copyIndicatorField;

        private SPSPartyType issuerSPSPartyField;

        private SPSPartyType[] recipientSPSPartyField;

        private SPSNoteType[] includedSPSNoteField;

        private SPSReferencedDocumentType[] referenceSPSReferencedDocumentField;

        private SPSAuthenticationType[] signatorySPSAuthenticationField;

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public DocumentCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public StatusCodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public IndicatorType CopyIndicator
        {
            get => this.copyIndicatorField;
            set => this.copyIndicatorField = value;
        }

        public SPSPartyType IssuerSPSParty
        {
            get => this.issuerSPSPartyField;
            set => this.issuerSPSPartyField = value;
        }

        [XmlElement("RecipientSPSParty")]
        public SPSPartyType[] RecipientSPSParty
        {
            get => this.recipientSPSPartyField;
            set => this.recipientSPSPartyField = value;
        }

        [XmlElement("IncludedSPSNote")]
        public SPSNoteType[] IncludedSPSNote
        {
            get => this.includedSPSNoteField;
            set => this.includedSPSNoteField = value;
        }

        [XmlElement("ReferenceSPSReferencedDocument")]
        public SPSReferencedDocumentType[] ReferenceSPSReferencedDocument
        {
            get => this.referenceSPSReferencedDocumentField;
            set => this.referenceSPSReferencedDocumentField = value;
        }

        [XmlElement("SignatorySPSAuthentication")]
        public SPSAuthenticationType[] SignatorySPSAuthentication
        {
            get => this.signatorySPSAuthenticationField;
            set => this.signatorySPSAuthenticationField = value;
        }
    }
}