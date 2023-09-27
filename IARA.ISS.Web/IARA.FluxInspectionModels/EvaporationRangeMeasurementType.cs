namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("EvaporationRangeMeasurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class EvaporationRangeMeasurementType
    {

        private CodeType lowerLimitComparisonOperatorCodeField;

        private CodeType upperLimitComparisonOperatorCodeField;

        private MeasureType lowerLimitActualMeasureField;

        private MeasureType upperLimitActualMeasureField;

        private TextType evaporationRateMethodField;

        private TextType referenceMaterialDescriptionField;

        private TextType evaporationRateDescriptionField;

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

        public TextType EvaporationRateMethod
        {
            get => this.evaporationRateMethodField;
            set => this.evaporationRateMethodField = value;
        }

        public TextType ReferenceMaterialDescription
        {
            get => this.referenceMaterialDescriptionField;
            set => this.referenceMaterialDescriptionField = value;
        }

        public TextType EvaporationRateDescription
        {
            get => this.evaporationRateDescriptionField;
            set => this.evaporationRateDescriptionField = value;
        }
    }
}