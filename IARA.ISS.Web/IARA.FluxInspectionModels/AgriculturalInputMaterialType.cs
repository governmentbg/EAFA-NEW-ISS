namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalInputMaterial", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalInputMaterialType
    {

        private TextType nameField;

        private CodeType[] typeCodeField;

        private TextType compositionDescriptionField;

        private MeasureType massMeasureField;

        private MeasureType massRatioMeasureField;

        private MeasureType volumeMeasureField;

        private MeasureType volumeRatioMeasureField;

        private IDType[] classificationIDField;

        private IDType[] idField;

        private TextType[] informationField;

        private NumericType indexNumericField;

        private IndicatorType activeIngredientIndicatorField;

        private TextType descriptionField;

        private AgriculturalCharacteristicType[] applicableAgriculturalCharacteristicField;

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

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
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

        public TextType CompositionDescription
        {
            get
            {
                return this.compositionDescriptionField;
            }
            set
            {
                this.compositionDescriptionField = value;
            }
        }

        public MeasureType MassMeasure
        {
            get
            {
                return this.massMeasureField;
            }
            set
            {
                this.massMeasureField = value;
            }
        }

        public MeasureType MassRatioMeasure
        {
            get
            {
                return this.massRatioMeasureField;
            }
            set
            {
                this.massRatioMeasureField = value;
            }
        }

        public MeasureType VolumeMeasure
        {
            get
            {
                return this.volumeMeasureField;
            }
            set
            {
                this.volumeMeasureField = value;
            }
        }

        public MeasureType VolumeRatioMeasure
        {
            get
            {
                return this.volumeRatioMeasureField;
            }
            set
            {
                this.volumeRatioMeasureField = value;
            }
        }

        [XmlElement("ClassificationID")]
        public IDType[] ClassificationID
        {
            get
            {
                return this.classificationIDField;
            }
            set
            {
                this.classificationIDField = value;
            }
        }

        [XmlElement("ID")]
        public IDType[] ID
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

        [XmlElement("Information")]
        public TextType[] Information
        {
            get
            {
                return this.informationField;
            }
            set
            {
                this.informationField = value;
            }
        }

        public NumericType IndexNumeric
        {
            get
            {
                return this.indexNumericField;
            }
            set
            {
                this.indexNumericField = value;
            }
        }

        public IndicatorType ActiveIngredientIndicator
        {
            get
            {
                return this.activeIngredientIndicatorField;
            }
            set
            {
                this.activeIngredientIndicatorField = value;
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

        [XmlElement("ApplicableAgriculturalCharacteristic")]
        public AgriculturalCharacteristicType[] ApplicableAgriculturalCharacteristic
        {
            get
            {
                return this.applicableAgriculturalCharacteristicField;
            }
            set
            {
                this.applicableAgriculturalCharacteristicField = value;
            }
        }
    }
}