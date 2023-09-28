namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSClassification", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSClassificationType
    {

        private IDType systemIDField;

        private TextType[] systemNameField;

        private CodeType classCodeField;

        private TextType[] classNameField;

        public IDType SystemID
        {
            get => this.systemIDField;
            set => this.systemIDField = value;
        }

        [XmlElement("SystemName")]
        public TextType[] SystemName
        {
            get => this.systemNameField;
            set => this.systemNameField = value;
        }

        public CodeType ClassCode
        {
            get => this.classCodeField;
            set => this.classCodeField = value;
        }

        [XmlElement("ClassName")]
        public TextType[] ClassName
        {
            get => this.classNameField;
            set => this.classNameField = value;
        }
    }
}