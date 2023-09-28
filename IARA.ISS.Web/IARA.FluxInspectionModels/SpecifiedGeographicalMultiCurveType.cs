namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SpecifiedGeographicalMultiCurve", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SpecifiedGeographicalMultiCurveType
    {

        private SpecifiedDirectPositionListType associatedSpecifiedDirectPositionListField;

        private SpecifiedGeographicalObjectCharacteristicType associatedSpecifiedGeographicalObjectCharacteristicField;

        private SpecifiedGeographicalLineType[] memberSpecifiedGeographicalLineField;

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

        [XmlElement("MemberSpecifiedGeographicalLine")]
        public SpecifiedGeographicalLineType[] MemberSpecifiedGeographicalLine
        {
            get
            {
                return this.memberSpecifiedGeographicalLineField;
            }
            set
            {
                this.memberSpecifiedGeographicalLineField = value;
            }
        }
    }
}