namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CropProduceBatch", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CropProduceBatchType
    {

        private IDType idField;

        private DateTimeType creationDateTimeField;

        private DateTimeType breakUpDateTimeField;

        private TextType productNameField;

        private CodeType typeCodeField;

        private MeasureType sizeMeasureField;

        private QuantityType unitQuantityField;

        private QuantityType specifiedQuantityField;

        private NumericType nominalSizeNumericField;

        private TextType[] appliedTreatmentField;

        private AgriculturalCharacteristicType[] specifiedAgriculturalCharacteristicField;

        private AgriculturalCertificateType[] specifiedAgriculturalCertificateField;

        private ReferencedObservationResultType[] specifiedReferencedObservationResultField;

        private CropProduceType[] specifiedCropProduceField;

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

        public DateTimeType CreationDateTime
        {
            get
            {
                return this.creationDateTimeField;
            }
            set
            {
                this.creationDateTimeField = value;
            }
        }

        public DateTimeType BreakUpDateTime
        {
            get
            {
                return this.breakUpDateTimeField;
            }
            set
            {
                this.breakUpDateTimeField = value;
            }
        }

        public TextType ProductName
        {
            get
            {
                return this.productNameField;
            }
            set
            {
                this.productNameField = value;
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

        public MeasureType SizeMeasure
        {
            get
            {
                return this.sizeMeasureField;
            }
            set
            {
                this.sizeMeasureField = value;
            }
        }

        public QuantityType UnitQuantity
        {
            get
            {
                return this.unitQuantityField;
            }
            set
            {
                this.unitQuantityField = value;
            }
        }

        public QuantityType SpecifiedQuantity
        {
            get
            {
                return this.specifiedQuantityField;
            }
            set
            {
                this.specifiedQuantityField = value;
            }
        }

        public NumericType NominalSizeNumeric
        {
            get
            {
                return this.nominalSizeNumericField;
            }
            set
            {
                this.nominalSizeNumericField = value;
            }
        }

        [XmlElement("AppliedTreatment")]
        public TextType[] AppliedTreatment
        {
            get
            {
                return this.appliedTreatmentField;
            }
            set
            {
                this.appliedTreatmentField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalCharacteristic")]
        public AgriculturalCharacteristicType[] SpecifiedAgriculturalCharacteristic
        {
            get
            {
                return this.specifiedAgriculturalCharacteristicField;
            }
            set
            {
                this.specifiedAgriculturalCharacteristicField = value;
            }
        }

        [XmlElement("SpecifiedAgriculturalCertificate")]
        public AgriculturalCertificateType[] SpecifiedAgriculturalCertificate
        {
            get
            {
                return this.specifiedAgriculturalCertificateField;
            }
            set
            {
                this.specifiedAgriculturalCertificateField = value;
            }
        }

        [XmlElement("SpecifiedReferencedObservationResult")]
        public ReferencedObservationResultType[] SpecifiedReferencedObservationResult
        {
            get
            {
                return this.specifiedReferencedObservationResultField;
            }
            set
            {
                this.specifiedReferencedObservationResultField = value;
            }
        }

        [XmlElement("SpecifiedCropProduce")]
        public CropProduceType[] SpecifiedCropProduce
        {
            get
            {
                return this.specifiedCropProduceField;
            }
            set
            {
                this.specifiedCropProduceField = value;
            }
        }
    }
}