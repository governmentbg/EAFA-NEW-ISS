namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class PaymentMeansChannelCodeType
    {

        private string listIDField;

        private PaymentMeansChannelCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private string nameField;

        private PaymentMeansChannelCodeContentType valueField;

        public PaymentMeansChannelCodeType()
        {
            this.listIDField = "4435";
            this.listAgencyIDField = PaymentMeansChannelCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute]
        public PaymentMeansChannelCodeListAgencyIDContentType listAgencyID
        {
            get => this.listAgencyIDField;
            set => this.listAgencyIDField = value;
        }

        [XmlIgnore()]
        public bool listAgencyIDSpecified
        {
            get => this.listAgencyIDFieldSpecified;
            set => this.listAgencyIDFieldSpecified = value;
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get => this.listVersionIDField;
            set => this.listVersionIDField = value;
        }

        [XmlAttribute]
        public string name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlText]
        public PaymentMeansChannelCodeContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}