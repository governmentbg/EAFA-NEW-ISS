namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIProductCharacteristic", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIProductCharacteristicType
    {

        private IDType[] idField;

        private CodeType typeCodeField;

        private TextType[] descriptionField;

        private MeasureType valueMeasureField;

        private CodeType measurementMethodCodeField;

        private CodeType contentTypeCodeField;

        private CodeType valueCodeField;

        private DateTimeType valueDateTimeField;

        private IndicatorType valueIndicatorField;

        private TextType[] valueField;

        private ReferencedStandardType applicableReferencedStandardField;

        private ProductCharacteristicConditionType[] applicableProductCharacteristicConditionField;

        private SpecifiedBinaryFileType valueSpecifiedBinaryFileField;

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

        public CodeType MeasurementMethodCode
        {
            get
            {
                return this.measurementMethodCodeField;
            }
            set
            {
                this.measurementMethodCodeField = value;
            }
        }

        public CodeType ContentTypeCode
        {
            get
            {
                return this.contentTypeCodeField;
            }
            set
            {
                this.contentTypeCodeField = value;
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

        [XmlElement("Value")]
        public TextType[] Value
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

        public ReferencedStandardType ApplicableReferencedStandard
        {
            get
            {
                return this.applicableReferencedStandardField;
            }
            set
            {
                this.applicableReferencedStandardField = value;
            }
        }

        [XmlElement("ApplicableProductCharacteristicCondition")]
        public ProductCharacteristicConditionType[] ApplicableProductCharacteristicCondition
        {
            get
            {
                return this.applicableProductCharacteristicConditionField;
            }
            set
            {
                this.applicableProductCharacteristicConditionField = value;
            }
        }

        public SpecifiedBinaryFileType ValueSpecifiedBinaryFile
        {
            get
            {
                return this.valueSpecifiedBinaryFileField;
            }
            set
            {
                this.valueSpecifiedBinaryFileField = value;
            }
        }
    }
}