namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("LogicalQueryParameter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class LogicalQueryParameterType
    {

        private TextType logicalOperatorField;

        private IndicatorType negationIndicatorField;

        private LogicalQueryParameterType subordinateLogicalQueryParameterField;

        private PrimitiveQueryParameterType firstSubordinatePrimitiveQueryParameterField;

        private PrimitiveQueryParameterType secondSubordinatePrimitiveQueryParameterField;

        public TextType LogicalOperator
        {
            get => this.logicalOperatorField;
            set => this.logicalOperatorField = value;
        }

        public IndicatorType NegationIndicator
        {
            get => this.negationIndicatorField;
            set => this.negationIndicatorField = value;
        }

        public LogicalQueryParameterType SubordinateLogicalQueryParameter
        {
            get => this.subordinateLogicalQueryParameterField;
            set => this.subordinateLogicalQueryParameterField = value;
        }

        public PrimitiveQueryParameterType FirstSubordinatePrimitiveQueryParameter
        {
            get => this.firstSubordinatePrimitiveQueryParameterField;
            set => this.firstSubordinatePrimitiveQueryParameterField = value;
        }

        public PrimitiveQueryParameterType SecondSubordinatePrimitiveQueryParameter
        {
            get => this.secondSubordinatePrimitiveQueryParameterField;
            set => this.secondSubordinatePrimitiveQueryParameterField = value;
        }
    }
}