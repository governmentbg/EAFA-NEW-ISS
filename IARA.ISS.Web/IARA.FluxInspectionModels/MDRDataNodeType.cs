namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MDRDataNode", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MDRDataNodeType
    {

        private IDType idField;

        private IDType parentIDField;

        private NumericType hierarchicalLevelNumericField;

        private DelimitedPeriodType effectiveDelimitedPeriodField;

        private MDRElementDataNodeType[] subordinateMDRElementDataNodeField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType ParentID
        {
            get => this.parentIDField;
            set => this.parentIDField = value;
        }

        public NumericType HierarchicalLevelNumeric
        {
            get => this.hierarchicalLevelNumericField;
            set => this.hierarchicalLevelNumericField = value;
        }

        public DelimitedPeriodType EffectiveDelimitedPeriod
        {
            get => this.effectiveDelimitedPeriodField;
            set => this.effectiveDelimitedPeriodField = value;
        }

        [XmlElement("SubordinateMDRElementDataNode")]
        public MDRElementDataNodeType[] SubordinateMDRElementDataNode
        {
            get => this.subordinateMDRElementDataNodeField;
            set => this.subordinateMDRElementDataNodeField = value;
        }
    }
}