namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AutoignitionRangeMeasurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AutoignitionRangeMeasurementType
    {

        private CodeType lowerLimitComparisonOperatorCodeField;

        private CodeType upperLimitComparisonOperatorCodeField;

        private MeasureType lowerLimitActualMeasureField;

        private MeasureType upperLimitActualMeasureField;

        private MeasureType[] lowerLimitConditionMeasureField;

        private MeasureType[] upperLimitConditionMeasureField;

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

        [XmlElement("LowerLimitConditionMeasure")]
        public MeasureType[] LowerLimitConditionMeasure
        {
            get
            {
                return this.lowerLimitConditionMeasureField;
            }
            set
            {
                this.lowerLimitConditionMeasureField = value;
            }
        }

        [XmlElement("UpperLimitConditionMeasure")]
        public MeasureType[] UpperLimitConditionMeasure
        {
            get
            {
                return this.upperLimitConditionMeasureField;
            }
            set
            {
                this.upperLimitConditionMeasureField = value;
            }
        }
    }
}