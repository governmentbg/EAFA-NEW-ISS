namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportSoftware", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportSoftwareType
    {

        private TextType authorNameField;

        private TextType nameField;

        private IDType versionIDField;

        private SoftwareUserTypeCodeType userTypeCodeField;

        private IDType revisionIDField;

        private DateTimeType latestUpdateDateTimeField;

        public TextType AuthorName
        {
            get
            {
                return this.authorNameField;
            }
            set
            {
                this.authorNameField = value;
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

        public IDType VersionID
        {
            get
            {
                return this.versionIDField;
            }
            set
            {
                this.versionIDField = value;
            }
        }

        public SoftwareUserTypeCodeType UserTypeCode
        {
            get
            {
                return this.userTypeCodeField;
            }
            set
            {
                this.userTypeCodeField = value;
            }
        }

        public IDType RevisionID
        {
            get
            {
                return this.revisionIDField;
            }
            set
            {
                this.revisionIDField = value;
            }
        }

        public DateTimeType LatestUpdateDateTime
        {
            get
            {
                return this.latestUpdateDateTimeField;
            }
            set
            {
                this.latestUpdateDateTimeField = value;
            }
        }
    }
}