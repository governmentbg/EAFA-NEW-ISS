namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXISRQueryMessage:9")]
    [XmlRoot("FLUXISRQueryMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXISRQueryMessage:9", IsNullable = false)]
    public partial class FLUXISRQueryMessageType
    {

        private ISRQueryType iSRQueryField;

        public ISRQueryType ISRQuery
        {
            get
            {
                return this.iSRQueryField;
            }
            set
            {
                this.iSRQueryField = value;
            }
        }
    }
}