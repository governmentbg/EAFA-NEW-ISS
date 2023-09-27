namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAPeriod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAPeriodType
    {

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        private MeasureType durationMeasureField;

        private IndicatorType inclusiveIndicatorField;

        private TextType descriptionField;

        private DateTimeType completeDateTimeField;

        private AccountingPeriodFunctionCodeType functionCodeField;

        public DateTimeType StartDateTime
        {
            get
            {
                return this.startDateTimeField;
            }
            set
            {
                this.startDateTimeField = value;
            }
        }

        public DateTimeType EndDateTime
        {
            get
            {
                return this.endDateTimeField;
            }
            set
            {
                this.endDateTimeField = value;
            }
        }

        public MeasureType DurationMeasure
        {
            get
            {
                return this.durationMeasureField;
            }
            set
            {
                this.durationMeasureField = value;
            }
        }

        public IndicatorType InclusiveIndicator
        {
            get
            {
                return this.inclusiveIndicatorField;
            }
            set
            {
                this.inclusiveIndicatorField = value;
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

        public DateTimeType CompleteDateTime
        {
            get
            {
                return this.completeDateTimeField;
            }
            set
            {
                this.completeDateTimeField = value;
            }
        }

        public AccountingPeriodFunctionCodeType FunctionCode
        {
            get
            {
                return this.functionCodeField;
            }
            set
            {
                this.functionCodeField = value;
            }
        }
    }
}