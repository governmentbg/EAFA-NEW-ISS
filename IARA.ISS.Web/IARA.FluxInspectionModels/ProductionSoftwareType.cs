namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProductionSoftware", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProductionSoftwareType
    {

        private IDType idField;

        private TextType nameField;

        private TextType authorNameField;

        private IDType versionIDField;

        private ConformanceCertificateType[] providedConformanceCertificateField;

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

        [XmlElement("ProvidedConformanceCertificate")]
        public ConformanceCertificateType[] ProvidedConformanceCertificate
        {
            get
            {
                return this.providedConformanceCertificateField;
            }
            set
            {
                this.providedConformanceCertificateField = value;
            }
        }
    }
}