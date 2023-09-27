namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class PriceTypeCodeType
    {

        private string listIDField;

        private PriceTypeCodeListAgencyIDContentType listAgencyIDField;

        private bool listAgencyIDFieldSpecified;

        private string listVersionIDField;

        private PriceTypeCodeContentType valueField;

        public PriceTypeCodeType()
        {
            this.listIDField = "5375";
            this.listAgencyIDField = PriceTypeCodeListAgencyIDContentType.Item6;
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get
            {
                return this.listIDField;
            }
            set
            {
                this.listIDField = value;
            }
        }

        [XmlAttribute]
        public PriceTypeCodeListAgencyIDContentType listAgencyID
        {
            get
            {
                return this.listAgencyIDField;
            }
            set
            {
                this.listAgencyIDField = value;
            }
        }

        [XmlIgnore]
        public bool listAgencyIDSpecified
        {
            get
            {
                return this.listAgencyIDFieldSpecified;
            }
            set
            {
                this.listAgencyIDFieldSpecified = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string listVersionID
        {
            get
            {
                return this.listVersionIDField;
            }
            set
            {
                this.listVersionIDField = value;
            }
        }

        [XmlText]
        public PriceTypeCodeContentType Value
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