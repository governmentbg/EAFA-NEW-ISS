namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDocumentAuthentication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDocumentAuthenticationType
    {

        private CodeType categoryCodeField;

        private DateTimeType actualDateTimeField;

        private IDType idField;

        private TextType[] informationField;

        private TextType signatoryField;

        private BinaryObjectType signatoryImageBinaryObjectField;

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public DateTimeType ActualDateTime
        {
            get => this.actualDateTimeField;
            set => this.actualDateTimeField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Information")]
        public TextType[] Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public TextType Signatory
        {
            get => this.signatoryField;
            set => this.signatoryField = value;
        }

        public BinaryObjectType SignatoryImageBinaryObject
        {
            get => this.signatoryImageBinaryObjectField;
            set => this.signatoryImageBinaryObjectField = value;
        }
    }
}