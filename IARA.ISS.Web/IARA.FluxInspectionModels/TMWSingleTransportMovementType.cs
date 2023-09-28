namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWSingleTransportMovement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWSingleTransportMovementType
    {

        private CodeType[] modeCodeField;

        private TMWPartyType carrierTMWPartyField;

        [XmlElement("ModeCode")]
        public CodeType[] ModeCode
        {
            get => this.modeCodeField;
            set => this.modeCodeField = value;
        }

        public TMWPartyType CarrierTMWParty
        {
            get => this.carrierTMWPartyField;
            set => this.carrierTMWPartyField = value;
        }
    }
}