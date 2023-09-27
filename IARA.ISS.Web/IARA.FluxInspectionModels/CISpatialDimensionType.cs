namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISpatialDimension", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISpatialDimensionType
    {

        private MeasureType valueMeasureField;

        private DimensionTypeCodeType typeCodeField;

        private TextType[] descriptionField;

        private MeasureType widthMeasureField;

        private MeasureType lengthMeasureField;

        private MeasureType heightMeasureField;

        private IDType idField;

        private LinearUnitMeasureType diameterMeasureField;

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

        public DimensionTypeCodeType TypeCode
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

        public MeasureType WidthMeasure
        {
            get
            {
                return this.widthMeasureField;
            }
            set
            {
                this.widthMeasureField = value;
            }
        }

        public MeasureType LengthMeasure
        {
            get
            {
                return this.lengthMeasureField;
            }
            set
            {
                this.lengthMeasureField = value;
            }
        }

        public MeasureType HeightMeasure
        {
            get
            {
                return this.heightMeasureField;
            }
            set
            {
                this.heightMeasureField = value;
            }
        }

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

        public LinearUnitMeasureType DiameterMeasure
        {
            get
            {
                return this.diameterMeasureField;
            }
            set
            {
                this.diameterMeasureField = value;
            }
        }
    }
}