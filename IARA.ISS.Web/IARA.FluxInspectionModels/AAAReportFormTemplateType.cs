namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportFormTemplate", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportFormTemplateType
    {

        private IDType idField;

        private TextType nameField;

        private AAAReportType[] includedAAAReportField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        [XmlElement("IncludedAAAReport")]
        public AAAReportType[] IncludedAAAReport
        {
            get
            {
                return this.includedAAAReportField;
            }
            set
            {
                this.includedAAAReportField = value;
            }
        }
    }
}