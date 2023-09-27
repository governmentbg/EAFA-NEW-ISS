namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:QualifiedDataType:32")]
    public partial class LogisticsChargeCalculationBasisCodeType
    {

        private string listIDField;

        private string listAgencyIDField;

        private string listVersionIDField;

        private string nameField;

        private FreightChargeQuantityUnitBasisCodeContentType valueField;

        public LogisticsChargeCalculationBasisCodeType()
        {
            this.listIDField = "6131";
            this.listAgencyIDField = "6";
            this.listVersionIDField = "D22A";
        }

        [XmlAttribute(DataType = "token")]
        public string listID
        {
            get => this.listIDField;
            set => this.listIDField = value;
        }

        [XmlAttribute(DataType = "token")]
        public string listAgencyID
        {
            get => this.listAgencyIDField;
            set => this.listAgencyIDField = value;
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
        public FreightChargeQuantityUnitBasisCodeContentType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }
    }
}