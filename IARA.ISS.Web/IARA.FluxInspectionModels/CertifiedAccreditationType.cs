namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CertifiedAccreditation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CertifiedAccreditationType
    {

        private CodeType categoryCodeField;

        private CodeType typeCodeField;

        private FormattedDateTimeType obtainedDateTimeField;

        private TextType accreditingBodyNameField;

        private TextType descriptionField;

        private IDType[] idField;

        private CodeType[] authenticationMethodCodeField;

        private FormattedDateTimeType expiryDateTimeField;

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public FormattedDateTimeType ObtainedDateTime
        {
            get => this.obtainedDateTimeField;
            set => this.obtainedDateTimeField = value;
        }

        public TextType AccreditingBodyName
        {
            get => this.accreditingBodyNameField;
            set => this.accreditingBodyNameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("AuthenticationMethodCode")]
        public CodeType[] AuthenticationMethodCode
        {
            get => this.authenticationMethodCodeField;
            set => this.authenticationMethodCodeField = value;
        }

        public FormattedDateTimeType ExpiryDateTime
        {
            get => this.expiryDateTimeField;
            set => this.expiryDateTimeField = value;
        }
    }
}