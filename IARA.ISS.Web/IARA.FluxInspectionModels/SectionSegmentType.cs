namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SectionSegment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SectionSegmentType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType informationField;

        private BinaryObjectType imageBinaryObjectField;

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

        public TextType Information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }

        public BinaryObjectType ImageBinaryObject
        {
            get
            {
                return this.imageBinaryObjectField;
            }
            set
            {
                this.imageBinaryObjectField = value;
            }
        }
    }
}