namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapSoftware", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapSoftwareType
    {

        private TextType authorNameField;

        private TextType nameField;

        private IDType versionIDField;

        private IDType revisionIDField;

        private DateTimeType latestUpdateDateTimeField;

        private AAAWrapCertificateType[] providedAAAWrapCertificateField;

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

        [XmlElement("ProvidedAAAWrapCertificate")]
        public AAAWrapCertificateType[] ProvidedAAAWrapCertificate
        {
            get
            {
                return this.providedAAAWrapCertificateField;
            }
            set
            {
                this.providedAAAWrapCertificateField = value;
            }
        }
    }
}