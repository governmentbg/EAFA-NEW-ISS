namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedGeographicalMultiPoint", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedGeographicalMultiPointType
    {

        private SpecifiedDirectPositionListType associatedSpecifiedDirectPositionListField;

        private SpecifiedGeographicalObjectCharacteristicType associatedSpecifiedGeographicalObjectCharacteristicField;

        private SpecifiedGeographicalPointType[] memberSpecifiedGeographicalPointField;

        public SpecifiedDirectPositionListType AssociatedSpecifiedDirectPositionList
        {
            get
            {
                return this.associatedSpecifiedDirectPositionListField;
            }
            set
            {
                this.associatedSpecifiedDirectPositionListField = value;
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

        [XmlElement("MemberSpecifiedGeographicalPoint")]
        public SpecifiedGeographicalPointType[] MemberSpecifiedGeographicalPoint
        {
            get
            {
                return this.memberSpecifiedGeographicalPointField;
            }
            set
            {
                this.memberSpecifiedGeographicalPointField = value;
            }
        }
    }
}