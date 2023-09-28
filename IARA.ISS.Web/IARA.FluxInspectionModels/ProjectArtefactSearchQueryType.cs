namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectArtefactSearchQuery", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectArtefactSearchQueryType
    {

        private TextType contentField;

        private IDType idField;

        private PrimitiveQueryParameterType simplePrimitiveQueryParameterField;

        private LogicalQueryParameterType compoundLogicalQueryParameterField;

        public TextType Content
        {
            get => this.contentField;
            set => this.contentField = value;
        }

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public PrimitiveQueryParameterType SimplePrimitiveQueryParameter
        {
            get => this.simplePrimitiveQueryParameterField;
            set => this.simplePrimitiveQueryParameterField = value;
        }

        public LogicalQueryParameterType CompoundLogicalQueryParameter
        {
            get => this.compoundLogicalQueryParameterField;
            set => this.compoundLogicalQueryParameterField = value;
        }
    }
}