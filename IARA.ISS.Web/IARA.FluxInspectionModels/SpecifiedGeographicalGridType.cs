namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedGeographicalGrid", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedGeographicalGridType
    {

        private TextType cellField;

        private IDType[] idField;

        private NumericType offsetVectorNumericField;

        private TextType lowLimitField;

        private TextType highLimitField;

        private NumericType dimensionNumericField;

        private TextType[] axisNameField;

        private SpecifiedDirectPositionListType originAssociatedSpecifiedDirectPositionListField;

        private SpecifiedGeographicalObjectCharacteristicType associatedSpecifiedGeographicalObjectCharacteristicField;

        private CropPlotType[] specifiedCropPlotField;

        public TextType Cell
        {
            get
            {
                return this.cellField;
            }
            set
            {
                this.cellField = value;
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

        public NumericType OffsetVectorNumeric
        {
            get
            {
                return this.offsetVectorNumericField;
            }
            set
            {
                this.offsetVectorNumericField = value;
            }
        }

        public TextType LowLimit
        {
            get
            {
                return this.lowLimitField;
            }
            set
            {
                this.lowLimitField = value;
            }
        }

        public TextType HighLimit
        {
            get
            {
                return this.highLimitField;
            }
            set
            {
                this.highLimitField = value;
            }
        }

        public NumericType DimensionNumeric
        {
            get
            {
                return this.dimensionNumericField;
            }
            set
            {
                this.dimensionNumericField = value;
            }
        }

        [XmlElement("AxisName")]
        public TextType[] AxisName
        {
            get
            {
                return this.axisNameField;
            }
            set
            {
                this.axisNameField = value;
            }
        }

        public SpecifiedDirectPositionListType OriginAssociatedSpecifiedDirectPositionList
        {
            get
            {
                return this.originAssociatedSpecifiedDirectPositionListField;
            }
            set
            {
                this.originAssociatedSpecifiedDirectPositionListField = value;
            }
        }

        public SpecifiedGeographicalObjectCharacteristicType AssociatedSpecifiedGeographicalObjectCharacteristic
        {
            get
            {
                return this.associatedSpecifiedGeographicalObjectCharacteristicField;
            }
            set
            {
                this.associatedSpecifiedGeographicalObjectCharacteristicField = value;
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
    }
}