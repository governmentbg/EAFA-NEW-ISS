namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("VesselPositionEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class VesselPositionEventType
    {

        private DateTimeType obtainedOccurrenceDateTimeField;

        private CodeType typeCodeField;

        private MeasureType speedValueMeasureField;

        private MeasureType courseValueMeasureField;

        private CodeType activityTypeCodeField;

        private VesselGeographicalCoordinateType specifiedVesselGeographicalCoordinateField;

        public DateTimeType ObtainedOccurrenceDateTime
        {
            get
            {
                return this.obtainedOccurrenceDateTimeField;
            }
            set
            {
                this.obtainedOccurrenceDateTimeField = value;
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

        public MeasureType SpeedValueMeasure
        {
            get
            {
                return this.speedValueMeasureField;
            }
            set
            {
                this.speedValueMeasureField = value;
            }
        }

        public MeasureType CourseValueMeasure
        {
            get
            {
                return this.courseValueMeasureField;
            }
            set
            {
                this.courseValueMeasureField = value;
            }
        }

        public CodeType ActivityTypeCode
        {
            get
            {
                return this.activityTypeCodeField;
            }
            set
            {
                this.activityTypeCodeField = value;
            }
        }

        public VesselGeographicalCoordinateType SpecifiedVesselGeographicalCoordinate
        {
            get
            {
                return this.specifiedVesselGeographicalCoordinateField;
            }
            set
            {
                this.specifiedVesselGeographicalCoordinateField = value;
            }
        }
    }
}