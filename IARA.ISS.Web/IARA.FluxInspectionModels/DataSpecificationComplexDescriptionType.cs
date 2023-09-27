namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DataSpecificationComplexDescription", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DataSpecificationComplexDescriptionType
    {

        private TextType[] contentField;

        private DataSpecificationIdentityType registrationDataSpecificationIdentityField;

        private DataSpecificationDocumentType[] responseDataSpecificationDocumentField;

        [XmlElement("Content")]
        public TextType[] Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public DataSpecificationIdentityType RegistrationDataSpecificationIdentity
        {
            get => this.registrationDataSpecificationIdentityField;
            set => this.registrationDataSpecificationIdentityField = value;
        }

        [XmlElement("ResponseDataSpecificationDocument")]
        public DataSpecificationDocumentType[] ResponseDataSpecificationDocument
        {
            get => this.responseDataSpecificationDocumentField;
            set => this.responseDataSpecificationDocumentField = value;
        }
    }
}