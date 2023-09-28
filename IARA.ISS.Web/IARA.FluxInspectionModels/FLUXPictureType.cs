namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLUXPicture", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLUXPictureType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private DateTimeType takenDateTimeField;

        private IDType areaIncludedIDField;

        private TextType descriptionField;

        private BinaryObjectType digitalImageBinaryObjectField;

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

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
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

        public IDType AreaIncludedID
        {
            get
            {
                return this.areaIncludedIDField;
            }
            set
            {
                this.areaIncludedIDField = value;
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
    }
}