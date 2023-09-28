namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:32")]
    public partial class DateTimeType
    {

        private object itemField;

        [XmlElement("DateTime", typeof(System.DateTime))]
        [XmlElement("DateTimeString", typeof(DateTimeTypeDateTimeString))]
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