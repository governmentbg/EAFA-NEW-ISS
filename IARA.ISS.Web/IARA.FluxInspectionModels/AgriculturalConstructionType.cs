namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalConstruction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalConstructionType
    {

        private IDType idField;

        private TextType nameField;

        private CodeType[] descriptionCodeField;

        private CodeType[] typeCodeField;

        private CodeType[] subordinateTypeCodeField;

        private CodeType coverageTypeCodeField;

        private CodeType[] certificationCodeField;

        private CodeType[] functionCodeField;

        private TextType licenceField;

        private DateTimeType completionDateTimeField;

        private DateTimeType[] renovationDateTimeField;

        private MeasureType[] capacityMeasureField;

        private MeasureType inputCapacityMeasureField;

        private MeasureType outputCapacityMeasureField;

        private MeasureType bufferCapacityMeasureField;

        private ReferencedLocationType specifiedReferencedLocationField;

        private CropPlotType[] specifiedCropPlotField;

        private AgriculturalConstructionType[] subordinateAgriculturalConstructionField;

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

        [XmlElement("DescriptionCode")]
        public CodeType[] DescriptionCode
        {
            get
            {
                return this.descriptionCodeField;
            }
            set
            {
                this.descriptionCodeField = value;
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

        [XmlElement("SubordinateTypeCode")]
        public CodeType[] SubordinateTypeCode
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

        public CodeType CoverageTypeCode
        {
            get
            {
                return this.coverageTypeCodeField;
            }
            set
            {
                this.coverageTypeCodeField = value;
            }
        }

        [XmlElement("CertificationCode")]
        public CodeType[] CertificationCode
        {
            get
            {
                return this.certificationCodeField;
            }
            set
            {
                this.certificationCodeField = value;
            }
        }

        [XmlElement("FunctionCode")]
        public CodeType[] FunctionCode
        {
            get
            {
                return this.functionCodeField;
            }
            set
            {
                this.functionCodeField = value;
            }
        }

        public TextType Licence
        {
            get
            {
                return this.licenceField;
            }
            set
            {
                this.licenceField = value;
            }
        }

        public DateTimeType CompletionDateTime
        {
            get
            {
                return this.completionDateTimeField;
            }
            set
            {
                this.completionDateTimeField = value;
            }
        }

        [XmlElement("RenovationDateTime")]
        public DateTimeType[] RenovationDateTime
        {
            get
            {
                return this.renovationDateTimeField;
            }
            set
            {
                this.renovationDateTimeField = value;
            }
        }

        [XmlElement("CapacityMeasure")]
        public MeasureType[] CapacityMeasure
        {
            get
            {
                return this.capacityMeasureField;
            }
            set
            {
                this.capacityMeasureField = value;
            }
        }

        public MeasureType InputCapacityMeasure
        {
            get
            {
                return this.inputCapacityMeasureField;
            }
            set
            {
                this.inputCapacityMeasureField = value;
            }
        }

        public MeasureType OutputCapacityMeasure
        {
            get
            {
                return this.outputCapacityMeasureField;
            }
            set
            {
                this.outputCapacityMeasureField = value;
            }
        }

        public MeasureType BufferCapacityMeasure
        {
            get
            {
                return this.bufferCapacityMeasureField;
            }
            set
            {
                this.bufferCapacityMeasureField = value;
            }
        }

        public ReferencedLocationType SpecifiedReferencedLocation
        {
            get
            {
                return this.specifiedReferencedLocationField;
            }
            set
            {
                this.specifiedReferencedLocationField = value;
            }
        }

        [XmlElement("SpecifiedCropPlot")]
        public CropPlotType[] SpecifiedCropPlot
        {
            get
            {
                return this.specifiedCropPlotField;
            }
            set
            {
                this.specifiedCropPlotField = value;
            }
        }

        [XmlElement("SubordinateAgriculturalConstruction")]
        public AgriculturalConstructionType[] SubordinateAgriculturalConstruction
        {
            get
            {
                return this.subordinateAgriculturalConstructionField;
            }
            set
            {
                this.subordinateAgriculturalConstructionField = value;
            }
        }
    }
}