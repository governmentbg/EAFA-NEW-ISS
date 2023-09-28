namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTConsignment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTConsignmentType
    {

        private TTTransportMovementType[] specifiedTTTransportMovementField;

        [XmlElement("SpecifiedTTTransportMovement")]
        public TTTransportMovementType[] SpecifiedTTTransportMovement
        {
            get => this.specifiedTTTransportMovementField;
            set => this.specifiedTTTransportMovementField = value;
        }
    }
}