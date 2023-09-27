namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFExchangedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFExchangedDocumentType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private CodeType statusCodeField;

        private DateTimeType issueDateTimeField;

        private RASFFPartyType senderRASFFPartyField;

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

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public DateTimeType IssueDateTime
        {
            get => this.issueDateTimeField;
            set => this.issueDateTimeField = value;
        }

        public RASFFPartyType SenderRASFFParty
        {
            get => this.senderRASFFPartyField;
            set => this.senderRASFFPartyField = value;
        }
    }
}