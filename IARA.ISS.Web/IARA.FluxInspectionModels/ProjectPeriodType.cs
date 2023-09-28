namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProjectPeriod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProjectPeriodType
    {

        private DurationUnitMeasureType durationMeasureField;

        private IndicatorType inclusiveIndicatorField;

        private TextType descriptionField;

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        private DateTimeType completeDateTimeField;

        private NumericType sequenceNumericField;

        private TextType nameField;

        public DurationUnitMeasureType DurationMeasure
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

        public NumericType SequenceNumeric
        {
            get
            {
                return this.sequenceNumericField;
            }
            set
            {
                this.sequenceNumericField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }
}