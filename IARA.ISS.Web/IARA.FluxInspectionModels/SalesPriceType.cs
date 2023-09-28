namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SalesPrice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SalesPriceType
    {

        private AmountType[] chargeAmountField;

        [XmlElement("ChargeAmount")]
        public AmountType[] ChargeAmount
        {
            get => this.chargeAmountField;
            set => this.chargeAmountField = value;
        }
    }
}