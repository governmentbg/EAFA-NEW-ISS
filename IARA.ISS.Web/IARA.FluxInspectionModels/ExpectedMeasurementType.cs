namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ExpectedMeasurement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ExpectedMeasurementType
    {

        private MeasureType valueMeasureField;

        private CodeType valueCodeField;

        private CodeType basisCodeField;

        private MeasureType minimumValueMeasureField;

        private MeasureType maximumValueMeasureField;

        private TextType descriptionField;

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

        public CodeType ValueCode
        {
            get
            {
                return this.valueCodeField;
            }
            set
            {
                this.valueCodeField = value;
            }
        }

        public CodeType BasisCode
        {
            get
            {
                return this.basisCodeField;
            }
            set
            {
                this.basisCodeField = value;
            }
        }

        public MeasureType MinimumValueMeasure
        {
            get
            {
                return this.minimumValueMeasureField;
            }
            set
            {
                this.minimumValueMeasureField = value;
            }
        }

        public MeasureType MaximumValueMeasure
        {
            get
            {
                return this.maximumValueMeasureField;
            }
            set
            {
                this.maximumValueMeasureField = value;
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