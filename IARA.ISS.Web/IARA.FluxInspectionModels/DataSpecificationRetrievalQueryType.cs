namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DataSpecificationRetrievalQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DataSpecificationRetrievalQueryType
    {

        private TextType contentField;

        private DataSpecificationIdentityType registrationDataSpecificationIdentityField;

        private ResponseIdentityType subjectResponseIdentityField;

        public TextType Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public DataSpecificationIdentityType RegistrationDataSpecificationIdentity
        {
            get => this.registrationDataSpecificationIdentityField;
            set => this.registrationDataSpecificationIdentityField = value;
        }

        public ResponseIdentityType SubjectResponseIdentity
        {
            get => this.subjectResponseIdentityField;
            set => this.subjectResponseIdentityField = value;
        }
    }
}