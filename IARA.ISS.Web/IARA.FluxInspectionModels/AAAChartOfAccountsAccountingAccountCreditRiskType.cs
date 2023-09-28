namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAChartOfAccountsAccountingAccountCreditRisk", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAChartOfAccountsAccountingAccountCreditRiskType
    {

        private AmountType capLimitationAmountField;

        private AmountType currentAmountField;

        public AmountType CapLimitationAmount
        {
            get
            {
                return this.capLimitationAmountField;
            }
            set
            {
                this.capLimitationAmountField = value;
            }
        }

        public AmountType CurrentAmount
        {
            get
            {
                return this.currentAmountField;
            }
            set
            {
                this.currentAmountField = value;
            }
        }
    }
}