namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampleObservationRequestCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampleObservationRequestCharacteristicType
    {

        private IDType methodParameterIDField;

        private TextType parameterValueField;

        private CodeType comparisonOperatorCodeField;

        private MeasureType measuredValueMeasureField;

        private TextType measuredValueField;

        private TextType rangeField;

        private MeasureType measuredAccuracyMeasureField;

        private CodeType qualityResultCodeField;

        private TextType qualityResultDescriptionField;

        private TextType referenceLevelQualityResultDescriptionField;

        private NumericType appliedDilutionNumericField;

        private ObservationObjectiveParameterType[] applicableObservationObjectiveParameterField;

        public IDType MethodParameterID
        {
            get
            {
                return this.methodParameterIDField;
            }
            set
            {
                this.methodParameterIDField = value;
            }
        }

        public TextType ParameterValue
        {
            get
            {
                return this.parameterValueField;
            }
            set
            {
                this.parameterValueField = value;
            }
        }

        public CodeType ComparisonOperatorCode
        {
            get
            {
                return this.comparisonOperatorCodeField;
            }
            set
            {
                this.comparisonOperatorCodeField = value;
            }
        }

        public MeasureType MeasuredValueMeasure
        {
            get
            {
                return this.measuredValueMeasureField;
            }
            set
            {
                this.measuredValueMeasureField = value;
            }
        }

        public TextType MeasuredValue
        {
            get
            {
                return this.measuredValueField;
            }
            set
            {
                this.measuredValueField = value;
            }
        }

        public TextType Range
        {
            get
            {
                return this.rangeField;
            }
            set
            {
                this.rangeField = value;
            }
        }

        public MeasureType MeasuredAccuracyMeasure
        {
            get
            {
                return this.measuredAccuracyMeasureField;
            }
            set
            {
                this.measuredAccuracyMeasureField = value;
            }
        }

        public CodeType QualityResultCode
        {
            get
            {
                return this.qualityResultCodeField;
            }
            set
            {
                this.qualityResultCodeField = value;
            }
        }

        public TextType QualityResultDescription
        {
            get
            {
                return this.qualityResultDescriptionField;
            }
            set
            {
                this.qualityResultDescriptionField = value;
            }
        }

        public TextType ReferenceLevelQualityResultDescription
        {
            get
            {
                return this.referenceLevelQualityResultDescriptionField;
            }
            set
            {
                this.referenceLevelQualityResultDescriptionField = value;
            }
        }

        public NumericType AppliedDilutionNumeric
        {
            get
            {
                return this.appliedDilutionNumericField;
            }
            set
            {
                this.appliedDilutionNumericField = value;
            }
        }

        [XmlElement("ApplicableObservationObjectiveParameter")]
        public ObservationObjectiveParameterType[] ApplicableObservationObjectiveParameter
        {
            get
            {
                return this.applicableObservationObjectiveParameterField;
            }
            set
            {
                this.applicableObservationObjectiveParameterField = value;
            }
        }
    }
}