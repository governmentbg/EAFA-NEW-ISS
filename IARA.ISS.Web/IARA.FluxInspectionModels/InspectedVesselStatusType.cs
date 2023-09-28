namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("InspectedVesselStatus", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class InspectedVesselStatusType
    {

        private CodeType regimeManagementCodeField;

        private ISRItemCharacteristicType[] specifiedISRItemCharacteristicField;

        public CodeType RegimeManagementCode
        {
            get
            {
                return this.regimeManagementCodeField;
            }
            set
            {
                this.regimeManagementCodeField = value;
            }
        }

        [XmlElement("SpecifiedISRItemCharacteristic")]
        public ISRItemCharacteristicType[] SpecifiedISRItemCharacteristic
        {
            get
            {
                return this.specifiedISRItemCharacteristicField;
            }
            set
            {
                this.specifiedISRItemCharacteristicField = value;
            }
        }
    }
}