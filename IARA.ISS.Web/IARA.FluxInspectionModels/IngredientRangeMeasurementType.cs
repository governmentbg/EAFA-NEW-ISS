namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("IngredientRangeMeasurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class IngredientRangeMeasurementType
    {

        private CodeType lowerLimitComparisonOperatorCodeField;

        private CodeType upperLimitComparisonOperatorCodeField;

        private MeasureType lowerLimitActualMeasureField;

        private MeasureType upperLimitActualMeasureField;

        private MeasureType lowerLimitPressureConditionMeasureField;

        private MeasureType upperLimitPressureConditionMeasureField;

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

        public MeasureType LowerLimitPressureConditionMeasure
        {
            get => this.lowerLimitPressureConditionMeasureField;
            set => this.lowerLimitPressureConditionMeasureField = value;
        }

        public MeasureType UpperLimitPressureConditionMeasure
        {
            get => this.upperLimitPressureConditionMeasureField;
            set => this.upperLimitPressureConditionMeasureField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }
    }
}