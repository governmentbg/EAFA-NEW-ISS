namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("PartitionCoefficientRangeMeasurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class PartitionCoefficientRangeMeasurementType
    {

        private CodeType lowerLimitComparisonOperatorCodeField;

        private CodeType upperLimitComparisonOperatorCodeField;

        private MeasureType lowerLimitActualMeasureField;

        private MeasureType upperLimitActualMeasureField;

        private TextType testMethodField;

        private TextType descriptionField;

        public CodeType LowerLimitComparisonOperatorCode
        {
            get => this.lowerLimitComparisonOperatorCodeField;
            set => this.lowerLimitComparisonOperatorCodeField = value;
        }

        public CodeType UpperLimitComparisonOperatorCode
        {
            get => this.upperLimitComparisonOperatorCodeField;
            set => this.upperLimitComparisonOperatorCodeField = value;
        }

        public MeasureType LowerLimitActualMeasure
        {
            get => this.lowerLimitActualMeasureField;
            set => this.lowerLimitActualMeasureField = value;
        }

        public MeasureType UpperLimitActualMeasure
        {
            get => this.upperLimitActualMeasureField;
            set => this.upperLimitActualMeasureField = value;
        }

        public TextType TestMethod
        {
            get => this.testMethodField;
            set => this.testMethodField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}