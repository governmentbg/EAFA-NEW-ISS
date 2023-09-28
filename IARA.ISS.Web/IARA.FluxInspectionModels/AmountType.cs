namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class AmountType
    {

        private string currencyIDField;

        private string currencyCodeListVersionIDField;

        private decimal valueField;

        [XmlAttribute(DataType = "token")]
        public string currencyID
        {
            get
            {
                return this.currencyIDField;
            }
            set
            {
                this.currencyIDField = value;
            }
        }

        [XmlAttribute(DataType = "token")]
        public string currencyCodeListVersionID
        {
            get
            {
                return this.currencyCodeListVersionIDField;
            }
            set
            {
                this.currencyCodeListVersionIDField = value;
            }
        }

        [XmlText]
        public decimal Value
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