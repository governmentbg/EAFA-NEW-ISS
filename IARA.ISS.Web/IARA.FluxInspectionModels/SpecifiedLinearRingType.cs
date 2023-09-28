namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedLinearRing", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedLinearRingType
    {

        private TextType[] coordinateField;

        private SpecifiedDirectPositionType coordinateSpecifiedDirectPositionField;

        private FLUXGeographicalCoordinateType[] specifiedFLUXGeographicalCoordinateField;

        private SpecifiedGeographicalObjectCharacteristicType associatedSpecifiedGeographicalObjectCharacteristicField;

        [XmlElement("Coordinate")]
        public TextType[] Coordinate
        {
            get
            {
                return this.coordinateField;
            }
            set
            {
                this.coordinateField = value;
            }
        }

        public SpecifiedDirectPositionType CoordinateSpecifiedDirectPosition
        {
            get
            {
                return this.coordinateSpecifiedDirectPositionField;
            }
            set
            {
                this.coordinateSpecifiedDirectPositionField = value;
            }
        }

        [XmlElement("SpecifiedFLUXGeographicalCoordinate")]
        public FLUXGeographicalCoordinateType[] SpecifiedFLUXGeographicalCoordinate
        {
            get
            {
                return this.specifiedFLUXGeographicalCoordinateField;
            }
            set
            {
                this.specifiedFLUXGeographicalCoordinateField = value;
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
    }
}