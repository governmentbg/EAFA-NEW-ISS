namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISpecifiedPeriod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISpecifiedPeriodType
    {

        private MeasureType[] durationMeasureField;

        private IndicatorType inclusiveIndicatorField;

        private TextType[] descriptionField;

        private DateTimeType startDateTimeField;

        private DateTimeType endDateTimeField;

        private DateTimeType completeDateTimeField;

        private CodeType seasonCodeField;

        private IDType idField;

        private TextType[] nameField;

        private NumericType sequenceNumericField;

        private CodeType startDateFlexibilityCodeField;

        private IndicatorType continuousIndicatorField;

        [XmlElement("DurationMeasure")]
        public MeasureType[] DurationMeasure
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

        [XmlElement("Description")]
        public TextType[] Description
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

        public CodeType SeasonCode
        {
            get
            {
                return this.seasonCodeField;
            }
            set
            {
                this.seasonCodeField = value;
            }
        }

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        [XmlElement("Name")]
        public TextType[] Name
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

        public CodeType StartDateFlexibilityCode
        {
            get
            {
                return this.startDateFlexibilityCodeField;
            }
            set
            {
                this.startDateFlexibilityCodeField = value;
            }
        }

        public IndicatorType ContinuousIndicator
        {
            get
            {
                return this.continuousIndicatorField;
            }
            set
            {
                this.continuousIndicatorField = value;
            }
        }
    }
}