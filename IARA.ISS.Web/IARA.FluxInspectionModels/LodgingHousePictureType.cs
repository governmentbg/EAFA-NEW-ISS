namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LodgingHousePicture", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LodgingHousePictureType
    {

        private IDType idField;

        private TextType titleNameField;

        private TextType copyrightOwnerNameField;

        private TextType typeField;

        private TextType subjectField;

        private IDType copyrightRegisteredIDField;

        private BinaryObjectType digitalImageBinaryObjectField;

        private DateTimeType takenDateTimeField;

        private TextType areaIncludedField;

        private TextType descriptionField;

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

        public IDType CopyrightRegisteredID
        {
            get
            {
                return this.copyrightRegisteredIDField;
            }
            set
            {
                this.copyrightRegisteredIDField = value;
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
    }
}