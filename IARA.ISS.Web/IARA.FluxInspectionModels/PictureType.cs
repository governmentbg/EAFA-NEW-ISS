namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("Picture", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PictureType
    {

        private TextType areaIncludedField;

        private TextType copyrightOwnerNameField;

        private TextType descriptionField;

        private BinaryObjectType digitalImageBinaryObjectField;

        private IDType[] idField;

        private TextType subjectField;

        private DateTimeType takenDateTimeField;

        private TextType titleNameField;

        private TextType typeField;

        public TextType AreaIncluded
        {
            get => this.areaIncludedField;
            set => this.areaIncludedField = value;
        }

        public TextType CopyrightOwnerName
        {
            get => this.copyrightOwnerNameField;
            set => this.copyrightOwnerNameField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public BinaryObjectType DigitalImageBinaryObject
        {
            get => this.digitalImageBinaryObjectField;
            set => this.digitalImageBinaryObjectField = value;
        }

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Subject
        {
            get => this.subjectField;
            set => this.subjectField = value;
        }

        public DateTimeType TakenDateTime
        {
            get => this.takenDateTimeField;
            set => this.takenDateTimeField = value;
        }

        public TextType TitleName
        {
            get => this.titleNameField;
            set => this.titleNameField = value;
        }

        public TextType Type
        {
            get => this.typeField;
            set => this.typeField = value;
        }
    }
}