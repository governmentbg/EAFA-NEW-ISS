namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXISRMessage:9")]
    [XmlRoot("FLUXISRMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXISRMessage:9", IsNullable = false)]
    public partial class FLUXISRMessageType
    {

        private FLUXReportDocumentType fLUXReportDocumentField;

        private ISReportType[] iSReportField;

        public FLUXReportDocumentType FLUXReportDocument
        {
            get
            {
                return this.fLUXReportDocumentField;
            }
            set
            {
                this.fLUXReportDocumentField = value;
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