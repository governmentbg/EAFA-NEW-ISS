namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedPolygon", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedPolygonType
    {

        private SpecifiedLinearRingType[] interiorSpecifiedLinearRingField;

        private SpecifiedLinearRingType exteriorSpecifiedLinearRingField;

        private SpecifiedGeographicalObjectCharacteristicType associatedSpecifiedGeographicalObjectCharacteristicField;

        [XmlElement("InteriorSpecifiedLinearRing")]
        public SpecifiedLinearRingType[] InteriorSpecifiedLinearRing
        {
            get
            {
                return this.interiorSpecifiedLinearRingField;
            }
            set
            {
                this.interiorSpecifiedLinearRingField = value;
            }
        }

        public SpecifiedLinearRingType ExteriorSpecifiedLinearRing
        {
            get
            {
                return this.exteriorSpecifiedLinearRingField;
            }
            set
            {
                this.exteriorSpecifiedLinearRingField = value;
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