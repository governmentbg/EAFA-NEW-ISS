namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedGeographicalMultiSurface", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedGeographicalMultiSurfaceType
    {

        private SpecifiedGeographicalObjectCharacteristicType associatedSpecifiedGeographicalObjectCharacteristicField;

        private SpecifiedPolygonType[] includedSpecifiedPolygonField;

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

        [XmlElement("IncludedSpecifiedPolygon")]
        public SpecifiedPolygonType[] IncludedSpecifiedPolygon
        {
            get
            {
                return this.includedSpecifiedPolygonField;
            }
            set
            {
                this.includedSpecifiedPolygonField = value;
            }
        }
    }
}