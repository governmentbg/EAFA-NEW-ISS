namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class IndicatorType
    {

        private object itemField;

        [XmlElement("Indicator", typeof(bool))]
        [XmlElement("IndicatorString", typeof(IndicatorTypeIndicatorString))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }
}