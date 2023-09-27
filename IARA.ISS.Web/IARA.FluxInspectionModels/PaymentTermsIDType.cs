namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class PaymentTermsIDType
    {

        private string schemeIDField;

        private PaymentTermsIDSchemeAgencyIDContentType schemeAgencyIDField;

        private bool schemeAgencyIDFieldSpecified;

        private string schemeVersionIDField;

        private PaymentTermsDescriptionIdentifierContentType valueField;

        [XmlAttribute(DataType = "token")]
        public string schemeID
        {
            get
            {
                return this.schemeIDField;
            }
            set
            {
                this.schemeIDField = value;
            }
        }

        [XmlAttribute]
        public PaymentTermsIDSchemeAgencyIDContentType schemeAgencyID
        {
            get
            {
                return this.schemeAgencyIDField;
            }
            set
            {
                this.schemeAgencyIDField = value;
            }
        }

        [XmlIgnore]
        public bool schemeAgencyIDSpecified
        {
            get
            {
                return this.schemeAgencyIDFieldSpecified;
            }
            set
            {
                this.schemeAgencyIDFieldSpecified = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string schemeVersionID
        {
            get
            {
                return this.schemeVersionIDField;
            }
            set
            {
                this.schemeVersionIDField = value;
            }
        }

        [XmlText]
        public PaymentTermsDescriptionIdentifierContentType Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}