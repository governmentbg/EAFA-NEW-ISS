namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FinancialGuarantee", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FinancialGuaranteeType
    {

        private TextType[] descriptionField;

        private AmountType[] liabilityAmountField;

        private TextType[] conditionField;

        private DelimitedPeriodType effectiveDelimitedPeriodField;

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("LiabilityAmount")]
        public AmountType[] LiabilityAmount
        {
            get => this.liabilityAmountField;
            set => this.liabilityAmountField = value;
        }

        [XmlElement("Condition")]
        public TextType[] Condition
        {
            get => this.conditionField;
            set => this.conditionField = value;
        }

        public DelimitedPeriodType EffectiveDelimitedPeriod
        {
            get => this.effectiveDelimitedPeriodField;
            set => this.effectiveDelimitedPeriodField = value;
        }
    }
}