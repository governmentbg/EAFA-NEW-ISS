namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLUXBinaryFile", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLUXBinaryFileType
    {

        private TextType titleField;

        private BinaryObjectType includedBinaryObjectField;

        private TextType descriptionField;

        private MeasureType sizeMeasureField;

        private CodeType typeCodeField;

        private TextType nameField;

        public TextType Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        public BinaryObjectType IncludedBinaryObject
        {
            get
            {
                return this.includedBinaryObjectField;
            }
            set
            {
                this.includedBinaryObjectField = value;
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

        public MeasureType SizeMeasure
        {
            get
            {
                return this.sizeMeasureField;
            }
            set
            {
                this.sizeMeasureField = value;
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

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }
}