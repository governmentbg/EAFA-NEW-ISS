namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalProduce", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalProduceType
    {

        private CodeType typeCodeField;

        private CodeType subordinateTypeCodeField;

        private TextType nameField;

        private MeasureType weightMeasureField;

        private CodeType useCodeField;

        private MeasureType calculatedYieldMeasureField;

        private MeasureType estimatedYieldMeasureField;

        private TechnicalCharacteristicType[] applicableTechnicalCharacteristicField;

        private ProduceBatchType[] specifiedProduceBatchField;

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

        public CodeType SubordinateTypeCode
        {
            get
            {
                return this.subordinateTypeCodeField;
            }
            set
            {
                this.subordinateTypeCodeField = value;
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

        public MeasureType WeightMeasure
        {
            get
            {
                return this.weightMeasureField;
            }
            set
            {
                this.weightMeasureField = value;
            }
        }

        public CodeType UseCode
        {
            get
            {
                return this.useCodeField;
            }
            set
            {
                this.useCodeField = value;
            }
        }

        public MeasureType CalculatedYieldMeasure
        {
            get
            {
                return this.calculatedYieldMeasureField;
            }
            set
            {
                this.calculatedYieldMeasureField = value;
            }
        }

        public MeasureType EstimatedYieldMeasure
        {
            get
            {
                return this.estimatedYieldMeasureField;
            }
            set
            {
                this.estimatedYieldMeasureField = value;
            }
        }

        [XmlElement("ApplicableTechnicalCharacteristic")]
        public TechnicalCharacteristicType[] ApplicableTechnicalCharacteristic
        {
            get
            {
                return this.applicableTechnicalCharacteristicField;
            }
            set
            {
                this.applicableTechnicalCharacteristicField = value;
            }
        }

        [XmlElement("SpecifiedProduceBatch")]
        public ProduceBatchType[] SpecifiedProduceBatch
        {
            get
            {
                return this.specifiedProduceBatchField;
            }
            set
            {
                this.specifiedProduceBatchField = value;
            }
        }
    }
}