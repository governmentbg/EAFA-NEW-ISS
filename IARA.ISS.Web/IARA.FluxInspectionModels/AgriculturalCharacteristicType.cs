namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalCharacteristicType
    {

        private IDType idField;

        private CodeType typeCodeField;

        private TextType descriptionField;

        private MeasureType valueMeasureField;

        private TextType valueField;

        private CodeType valueCodeField;

        private DateTimeType valueDateTimeField;

        private IndicatorType valueIndicatorField;

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

        public DateTimeType ValueDateTime
        {
            get
            {
                return this.valueDateTimeField;
            }
            set
            {
                this.valueDateTimeField = value;
            }
        }

        public IndicatorType ValueIndicator
        {
            get
            {
                return this.valueIndicatorField;
            }
            set
            {
                this.valueIndicatorField = value;
            }
        }
    }
}