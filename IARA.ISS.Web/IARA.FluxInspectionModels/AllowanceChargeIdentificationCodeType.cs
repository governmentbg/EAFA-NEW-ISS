namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class AllowanceChargeIdentificationCodeType
    {

        private string listIDField;

        private string listAgencyIDField;

        private string listVersionIDField;

        private string listURIField;

        private AllowanceChargeIdentificationCodeContentType valueField;

        public AllowanceChargeIdentificationCodeType()
        {
            this.listIDField = "5189_AllowanceChargeID";
            this.listAgencyIDField = "6";
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

        [XmlAttribute(DataType = "token")]
        public string listAgencyID
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

        [XmlAttribute(DataType = "anyURI")]
        public string listURI
        {
            get
            {
                return this.listURIField;
            }
            set
            {
                this.listURIField = value;
            }
        }

        [XmlText]
        public AllowanceChargeIdentificationCodeContentType Value
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