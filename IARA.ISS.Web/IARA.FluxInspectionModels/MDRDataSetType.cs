namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MDRDataSet", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MDRDataSetType
    {

        private IDType[] idField;

        private TextType descriptionField;

        private TextType originField;

        private TextType nameField;

        private DataSetVersionType[] specifiedDataSetVersionField;

        private MDRDataNodeType[] containedMDRDataNodeField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public TextType Origin
        {
            get => this.originField;
            set => this.originField = value;
        }

        public TextType Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        [XmlElement("SpecifiedDataSetVersion")]
        public DataSetVersionType[] SpecifiedDataSetVersion
        {
            get => this.specifiedDataSetVersionField;
            set => this.specifiedDataSetVersionField = value;
        }

        [XmlElement("ContainedMDRDataNode")]
        public MDRDataNodeType[] ContainedMDRDataNode
        {
            get => this.containedMDRDataNodeField;
            set => this.containedMDRDataNodeField = value;
        }
    }
}