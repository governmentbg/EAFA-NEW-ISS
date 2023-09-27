namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedGeographicalCoordinate", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedGeographicalCoordinateType
    {

        private MeasureType altitudeMeasureField;

        private MeasureType latitudeMeasureField;

        private MeasureType longitudeMeasureField;

        private IndicatorType latitudeDirectionIndicatorField;

        private IndicatorType longitudeDirectionIndicatorField;

        private IDType systemIDField;

        public MeasureType AltitudeMeasure
        {
            get
            {
                return this.altitudeMeasureField;
            }
            set
            {
                this.altitudeMeasureField = value;
            }
        }

        public MeasureType LatitudeMeasure
        {
            get
            {
                return this.latitudeMeasureField;
            }
            set
            {
                this.latitudeMeasureField = value;
            }
        }

        public MeasureType LongitudeMeasure
        {
            get
            {
                return this.longitudeMeasureField;
            }
            set
            {
                this.longitudeMeasureField = value;
            }
        }

        public IndicatorType LatitudeDirectionIndicator
        {
            get
            {
                return this.latitudeDirectionIndicatorField;
            }
            set
            {
                this.latitudeDirectionIndicatorField = value;
            }
        }

        public IndicatorType LongitudeDirectionIndicator
        {
            get
            {
                return this.longitudeDirectionIndicatorField;
            }
            set
            {
                this.longitudeDirectionIndicatorField = value;
            }
        }

        public IDType SystemID
        {
            get
            {
                return this.systemIDField;
            }
            set
            {
                this.systemIDField = value;
            }
        }
    }
}