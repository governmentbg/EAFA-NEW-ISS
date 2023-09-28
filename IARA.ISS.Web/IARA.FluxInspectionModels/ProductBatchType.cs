namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ProductBatch", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ProductBatchType
    {

        private IDType idField;

        private DateTimeType creationDateTimeField;

        private TextType productNameField;

        private CodeType typeCodeField;

        private MeasureType sizeMeasureField;

        private MeasureType weightMeasureField;

        private QuantityType unitQuantityField;

        private TextType[] appliedTreatmentField;

        private AgriculturalCharacteristicType[] specifiedAgriculturalCharacteristicField;

        private AgriculturalCertificateType[] specifiedAgriculturalCertificateField;

        private AgriculturalInputProductType[] specifiedAgriculturalInputProductField;

        private SpecifiedAgriculturalApplicationType[] appliedSpecifiedAgriculturalApplicationField;

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

        [XmlElement("SpecifiedAgriculturalInputProduct")]
        public AgriculturalInputProductType[] SpecifiedAgriculturalInputProduct
        {
            get
            {
                return this.specifiedAgriculturalInputProductField;
            }
            set
            {
                this.specifiedAgriculturalInputProductField = value;
            }
        }

        [XmlElement("AppliedSpecifiedAgriculturalApplication")]
        public SpecifiedAgriculturalApplicationType[] AppliedSpecifiedAgriculturalApplication
        {
            get
            {
                return this.appliedSpecifiedAgriculturalApplicationField;
            }
            set
            {
                this.appliedSpecifiedAgriculturalApplicationField = value;
            }
        }
    }
}