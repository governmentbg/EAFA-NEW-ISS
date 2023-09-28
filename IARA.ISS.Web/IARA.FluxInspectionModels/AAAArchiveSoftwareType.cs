namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAArchiveSoftware", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAArchiveSoftwareType
    {

        private TextType authorNameField;

        private TextType nameField;

        private IDType versionIDField;

        private IDType revisionIDField;

        private DateTimeType latestUpdateDateTimeField;

        private AAAArchiveCertificateType[] providedAAAArchiveCertificateField;

        public TextType AuthorName
        {
            get => this.authorNameField;
            set => this.authorNameField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public IDType VersionID
        {
            get => this.versionIDField;
            set => this.versionIDField = value;
        }

        public IDType RevisionID
        {
            get => this.revisionIDField;
            set => this.revisionIDField = value;
        }

        public DateTimeType LatestUpdateDateTime
        {
            get => this.latestUpdateDateTimeField;
            set => this.latestUpdateDateTimeField = value;
        }

        [XmlElement("ProvidedAAAArchiveCertificate")]
        public AAAArchiveCertificateType[] ProvidedAAAArchiveCertificate
        {
            get => this.providedAAAArchiveCertificateField;
            set => this.providedAAAArchiveCertificateField = value;
        }
    }
}