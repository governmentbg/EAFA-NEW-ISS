namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("QueryResponse", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class QueryResponseType
    {

        private IDType[] idField;

        private TextType[] contentField;

        private CodeType[] typeCodeField;

        private FormattedDateTimeType actualDateTimeField;

        private CodeType statusCodeField;

        private TextType respondingPersonNameField;

        private CITradePartyType respondentCITradePartyField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public FormattedDateTimeType ActualDateTime
        {
            get => this.actualDateTimeField;
            set => this.actualDateTimeField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public TextType RespondingPersonName
        {
            get => this.respondingPersonNameField;
            set => this.respondingPersonNameField = value;
        }

        public CITradePartyType RespondentCITradeParty
        {
            get => this.respondentCITradePartyField;
            set => this.respondentCITradePartyField = value;
        }
    }
}