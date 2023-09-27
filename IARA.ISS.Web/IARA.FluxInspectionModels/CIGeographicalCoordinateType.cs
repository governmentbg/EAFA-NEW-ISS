namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIGeographicalCoordinate", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIGeographicalCoordinateType
    {

        private MeasureType altitudeMeasureField;

        private MeasureType latitudeMeasureField;

        private MeasureType longitudeMeasureField;

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