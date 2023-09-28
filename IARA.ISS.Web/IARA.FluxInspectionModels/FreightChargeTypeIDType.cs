namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class FreightChargeTypeIDType
    {

        private string schemeIDField;

        private FreightChargeTypeIDSchemeAgencyIDContentType schemeAgencyIDField;

        private bool schemeAgencyIDFieldSpecified;

        private string schemeVersionIDField;

        private FreightCostCodeContentType valueField;

        [XmlAttribute(DataType = "token")]
        public string schemeID
        {
            get => this.schemeIDField;
            set => this.schemeIDField = value;
        }

        [XmlAttribute]
        public FreightChargeTypeIDSchemeAgencyIDContentType schemeAgencyID
        {
            get => this.schemeAgencyIDField;
            set => this.schemeAgencyIDField = value;
        }

        [XmlIgnore()]
        public bool schemeAgencyIDSpecified
        {
            get => this.schemeAgencyIDFieldSpecified;
            set => this.schemeAgencyIDFieldSpecified = value;
        }

        [XmlAttribute(DataType = "token")]
        public string schemeVersionID
        {
            get => this.schemeVersionIDField;
            set => this.schemeVersionIDField = value;
        }

        [XmlText]
        public FreightCostCodeContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}