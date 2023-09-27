namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class PaymentMethodCodeType
    {

        private string listIDField;

        private PaymentMethodCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private PaymentMethodCodeContentType valueField;

        public PaymentMethodCodeType()
        {
            this.listIDField = "4439";
            this.listAgencyIDField = PaymentMethodCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute]
        public PaymentMethodCodeListAgencyIDContentType listAgencyID
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

        [XmlText]
        public PaymentMethodCodeContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}