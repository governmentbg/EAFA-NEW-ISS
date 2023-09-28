namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DataSpecificationProfileComplexDescription", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DataSpecificationProfileComplexDescriptionType
    {

        private TextType[] contentField;

        private DataSpecificationProfileComplexDescriptionType[] subsetDataSpecificationProfileComplexDescriptionField;

        private DataSpecificationIdentityType registrationDataSpecificationIdentityField;

        private SummaryDocumentType responseSummaryDocumentField;

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        [XmlElement("SubsetDataSpecificationProfileComplexDescription")]
        public DataSpecificationProfileComplexDescriptionType[] SubsetDataSpecificationProfileComplexDescription
        {
            get => this.subsetDataSpecificationProfileComplexDescriptionField;
            set => this.subsetDataSpecificationProfileComplexDescriptionField = value;
        }

        public DataSpecificationIdentityType RegistrationDataSpecificationIdentity
        {
            get => this.registrationDataSpecificationIdentityField;
            set => this.registrationDataSpecificationIdentityField = value;
        }

        public SummaryDocumentType ResponseSummaryDocument
        {
            get => this.responseSummaryDocumentField;
            set => this.responseSummaryDocumentField = value;
        }
    }
}