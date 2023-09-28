namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ConformanceCertificate", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ConformanceCertificateType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType softwareOperatingSystemField;

        private DateTimeType issueDateTimeField;

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

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public TextType SoftwareOperatingSystem
        {
            get
            {
                return this.softwareOperatingSystemField;
            }
            set
            {
                this.softwareOperatingSystemField = value;
            }
        }

        public DateTimeType IssueDateTime
        {
            get
            {
                return this.issueDateTimeField;
            }
            set
            {
                this.issueDateTimeField = value;
            }
        }
    }
}