namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselEngine", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselEngineType
    {

        private IDType serialNumberIDField;

        private CodeType typeCodeField;

        private CodeType roleCodeField;

        private CodeType propulsionTypeCodeField;

        private MeasureType[] powerMeasureField;

        private CodeType powerMeasurementMethodCodeField;

        private CodeType manufacturerCodeField;

        private TextType modelField;

        private TextType manufacturerField;

        private FLUXPictureType[] illustrateFLUXPictureField;

        public IDType SerialNumberID
        {
            get
            {
                return this.serialNumberIDField;
            }
            set
            {
                this.serialNumberIDField = value;
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

        public CodeType RoleCode
        {
            get
            {
                return this.roleCodeField;
            }
            set
            {
                this.roleCodeField = value;
            }
        }

        public CodeType PropulsionTypeCode
        {
            get
            {
                return this.propulsionTypeCodeField;
            }
            set
            {
                this.propulsionTypeCodeField = value;
            }
        }

        [XmlElement("PowerMeasure")]
        public MeasureType[] PowerMeasure
        {
            get
            {
                return this.powerMeasureField;
            }
            set
            {
                this.powerMeasureField = value;
            }
        }

        public CodeType PowerMeasurementMethodCode
        {
            get
            {
                return this.powerMeasurementMethodCodeField;
            }
            set
            {
                this.powerMeasurementMethodCodeField = value;
            }
        }

        public CodeType ManufacturerCode
        {
            get
            {
                return this.manufacturerCodeField;
            }
            set
            {
                this.manufacturerCodeField = value;
            }
        }

        public TextType Model
        {
            get
            {
                return this.modelField;
            }
            set
            {
                this.modelField = value;
            }
        }

        public TextType Manufacturer
        {
            get
            {
                return this.manufacturerField;
            }
            set
            {
                this.manufacturerField = value;
            }
        }

        [XmlElement("IllustrateFLUXPicture")]
        public FLUXPictureType[] IllustrateFLUXPicture
        {
            get
            {
                return this.illustrateFLUXPictureField;
            }
            set
            {
                this.illustrateFLUXPictureField = value;
            }
        }
    }
}