namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MDRElementDataNode", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MDRElementDataNodeType
    {

        private TextType nameField;

        private TextType valueField;

        private CodeType typeCodeField;

        private FLUXBinaryFileType valueFLUXBinaryFileField;

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public TextType Value
        {
            get => this.valueField;
            set => this.valueField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public FLUXBinaryFileType ValueFLUXBinaryFile
        {
            get => this.valueFLUXBinaryFileField;
            set => this.valueFLUXBinaryFileField = value;
        }
    }
}