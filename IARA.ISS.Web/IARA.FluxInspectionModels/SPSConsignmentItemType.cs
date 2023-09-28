namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSConsignmentItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSConsignmentItemType
    {

        private SPSCargoType[] natureIdentificationSPSCargoField;

        private SPSTradeLineItemType[] includedSPSTradeLineItemField;

        [XmlElement("NatureIdentificationSPSCargo")]
        public SPSCargoType[] NatureIdentificationSPSCargo
        {
            get => this.natureIdentificationSPSCargoField;
            set => this.natureIdentificationSPSCargoField = value;
        }

        [XmlElement("IncludedSPSTradeLineItem")]
        public SPSTradeLineItemType[] IncludedSPSTradeLineItem
        {
            get => this.includedSPSTradeLineItemField;
            set => this.includedSPSTradeLineItemField = value;
        }
    }
}