namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFTransportMovement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFTransportMovementType
    {

        private RASFFPartyType carrierRASFFPartyField;

        private RASFFTransportMeansType usedRASFFTransportMeansField;

        public RASFFPartyType CarrierRASFFParty
        {
            get => this.carrierRASFFPartyField;
            set => this.carrierRASFFPartyField = value;
        }

        public RASFFTransportMeansType UsedRASFFTransportMeans
        {
            get => this.usedRASFFTransportMeansField;
            set => this.usedRASFFTransportMeansField = value;
        }
    }
}