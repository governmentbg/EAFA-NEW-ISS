namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BidPrice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BidPriceType
    {

        private AmountType chargeAmountField;

        public AmountType ChargeAmount
        {
            get
            {
                return this.chargeAmountField;
            }
            set
            {
                this.chargeAmountField = value;
            }
        }
    }
}