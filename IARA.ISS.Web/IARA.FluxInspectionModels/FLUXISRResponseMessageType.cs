namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXISRResponseMessage:9")]
    [XmlRoot("FLUXISRResponseMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXISRResponseMessage:9", IsNullable = false)]
    public partial class FLUXISRResponseMessageType
    {

        private FLUXResponseDocumentType fLUXResponseDocumentField;

        private ISReportType[] iSReportField;

        public FLUXResponseDocumentType FLUXResponseDocument
        {
            get
            {
                return this.fLUXResponseDocumentField;
            }
            set
            {
                this.fLUXResponseDocumentField = value;
            }
        }

        [XmlElement("ISReport")]
        public ISReportType[] ISReport
        {
            get
            {
                return this.iSReportField;
            }
            set
            {
                this.iSReportField = value;
            }
        }
    }
}