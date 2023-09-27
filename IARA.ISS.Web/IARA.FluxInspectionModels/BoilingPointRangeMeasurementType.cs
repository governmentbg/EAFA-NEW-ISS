namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BoilingPointRangeMeasurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BoilingPointRangeMeasurementType
    {

        private CodeType lowerLimitComparisonOperatorCodeField;

        private CodeType upperLimitComparisonOperatorCodeField;

        private MeasureType lowerLimitActualMeasureField;

        private MeasureType upperLimitActualMeasureField;

        private TextType testMethodField;

        private TextType descriptionField;

        public CodeType LowerLimitComparisonOperatorCode
        {
            get
            {
                return this.lowerLimitComparisonOperatorCodeField;
            }
            set
            {
                this.lowerLimitComparisonOperatorCodeField = value;
            }
        }

        public CodeType UpperLimitComparisonOperatorCode
        {
            get
            {
                return this.upperLimitComparisonOperatorCodeField;
            }
            set
            {
                this.upperLimitComparisonOperatorCodeField = value;
            }
        }

        public MeasureType LowerLimitActualMeasure
        {
            get
            {
                return this.lowerLimitActualMeasureField;
            }
            set
            {
                this.lowerLimitActualMeasureField = value;
            }
        }

        public MeasureType UpperLimitActualMeasure
        {
            get
            {
                return this.upperLimitActualMeasureField;
            }
            set
            {
                this.upperLimitActualMeasureField = value;
            }
        }

        public TextType TestMethod
        {
            get
            {
                return this.testMethodField;
            }
            set
            {
                this.testMethodField = value;
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
    }
}