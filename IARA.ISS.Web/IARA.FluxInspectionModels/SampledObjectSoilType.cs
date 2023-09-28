namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SampledObjectSoil", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SampledObjectSoilType
    {

        private TextType typeField;

        private MeasureType upperBoundaryDepthMeasureField;

        private MeasureType lowerBoundaryDepthMeasureField;

        private MeasureType depthMeasureField;

        private TextType previousCropField;

        public TextType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        public MeasureType UpperBoundaryDepthMeasure
        {
            get
            {
                return this.upperBoundaryDepthMeasureField;
            }
            set
            {
                this.upperBoundaryDepthMeasureField = value;
            }
        }

        public MeasureType LowerBoundaryDepthMeasure
        {
            get
            {
                return this.lowerBoundaryDepthMeasureField;
            }
            set
            {
                this.lowerBoundaryDepthMeasureField = value;
            }
        }

        public MeasureType DepthMeasure
        {
            get
            {
                return this.depthMeasureField;
            }
            set
            {
                this.depthMeasureField = value;
            }
        }

        public TextType PreviousCrop
        {
            get
            {
                return this.previousCropField;
            }
            set
            {
                this.previousCropField = value;
            }
        }
    }
}