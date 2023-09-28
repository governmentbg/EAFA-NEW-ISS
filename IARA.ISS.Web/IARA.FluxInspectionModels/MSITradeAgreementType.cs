namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("MSITradeAgreement", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class MSITradeAgreementType
    {

        private CITradePartyType informationProviderCITradePartyField;

        private CITradePartyType informationReceiverCITradePartyField;

        public CITradePartyType InformationProviderCITradeParty
        {
            get => this.informationProviderCITradePartyField;
            set => this.informationProviderCITradePartyField = value;
        }

        public CITradePartyType InformationReceiverCITradeParty
        {
            get => this.informationReceiverCITradePartyField;
            set => this.informationReceiverCITradePartyField = value;
        }
    }
}