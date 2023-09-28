namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class DateType
    {

        private object itemField;

        [XmlElement("Date", typeof(System.DateTime), DataType = "date")]
        [XmlElement("DateString", typeof(DateTypeDateString))]
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