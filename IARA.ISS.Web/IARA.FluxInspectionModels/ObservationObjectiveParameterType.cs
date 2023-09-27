namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ObservationObjectiveParameter", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ObservationObjectiveParameterType
    {

        private IDType idField;

        private TextType valueField;

        private MeasureType valueMeasureField;

        private CodeType typeCodeField;

        private TextType descriptionField;

        private TextType nameField;

        private IndicatorType valueAllowedIndicatorField;

        private CodeType statusCodeField;

        private MeasureType statusValueMeasureField;

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

        public TextType Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        public MeasureType ValueMeasure
        {
            get
            {
                return this.valueMeasureField;
            }
            set
            {
                this.valueMeasureField = value;
            }
        }

        public CodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
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

        public IndicatorType ValueAllowedIndicator
        {
            get
            {
                return this.valueAllowedIndicatorField;
            }
            set
            {
                this.valueAllowedIndicatorField = value;
            }
        }

        public CodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        public MeasureType StatusValueMeasure
        {
            get
            {
                return this.statusValueMeasureField;
            }
            set
            {
                this.statusValueMeasureField = value;
            }
        }
    }
}