namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FLUXGeographicalCoordinate", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FLUXGeographicalCoordinateType
    {

        private MeasureType longitudeMeasureField;

        private MeasureType latitudeMeasureField;

        private MeasureType altitudeMeasureField;

        private IDType systemIDField;

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