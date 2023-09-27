namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedGeographicalLine", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedGeographicalLineType
    {

        private SpecifiedDirectPositionListType associatedSpecifiedDirectPositionListField;

        private SpecifiedGeographicalObjectCharacteristicType associatedSpecifiedGeographicalObjectCharacteristicField;

        public SpecifiedDirectPositionListType AssociatedSpecifiedDirectPositionList
        {
            get => this.associatedSpecifiedDirectPositionListField;
            set => this.associatedSpecifiedDirectPositionListField = value;
        }

        public SpecifiedGeographicalObjectCharacteristicType AssociatedSpecifiedGeographicalObjectCharacteristic
        {
            get => this.associatedSpecifiedGeographicalObjectCharacteristicField;
            set => this.associatedSpecifiedGeographicalObjectCharacteristicField = value;
        }
    }
}