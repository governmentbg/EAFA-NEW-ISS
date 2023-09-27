namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PhotographicPicture", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PhotographicPictureType
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
            get
            {
                return this.areaIncludedField;
            }
            set
            {
                this.areaIncludedField = value;
            }
        }

        public TextType CopyrightOwnerName
        {
            get
            {
                return this.copyrightOwnerNameField;
            }
            set
            {
                this.copyrightOwnerNameField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public BinaryObjectType DigitalImageBinaryObject
        {
            get
            {
                return this.digitalImageBinaryObjectField;
            }
            set
            {
                this.digitalImageBinaryObjectField = value;
            }
        }

        [XmlElement("ID")]
        public IDType[] ID
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

        public TextType Subject
        {
            get
            {
                return this.subjectField;
            }
            set
            {
                this.subjectField = value;
            }
        }

        public DateTimeType TakenDateTime
        {
            get
            {
                return this.takenDateTimeField;
            }
            set
            {
                this.takenDateTimeField = value;
            }
        }

        public TextType TitleName
        {
            get
            {
                return this.titleNameField;
            }
            set
            {
                this.titleNameField = value;
            }
        }

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }
}