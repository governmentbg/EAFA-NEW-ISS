namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportDocumentType
    {

        private BinaryObjectType attachmentBinaryObjectField;

        private AAAReportSoftwareType productionAAAReportSoftwareField;

        public BinaryObjectType AttachmentBinaryObject
        {
            get
            {
                return this.attachmentBinaryObjectField;
            }
            set
            {
                this.attachmentBinaryObjectField = value;
            }
        }

        public AAAReportSoftwareType ProductionAAAReportSoftware
        {
            get
            {
                return this.productionAAAReportSoftwareField;
            }
            set
            {
                this.productionAAAReportSoftwareField = value;
            }
        }
    }
}