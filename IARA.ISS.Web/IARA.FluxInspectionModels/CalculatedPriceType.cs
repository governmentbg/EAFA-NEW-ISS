namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CalculatedPrice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CalculatedPriceType
    {

        private CodeType[] typeCodeField;

        private AmountType[] chargeAmountField;

        private AppliedAllowanceChargeType[] relatedAppliedAllowanceChargeField;

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        [XmlElement("ChargeAmount")]
        public AmountType[] ChargeAmount
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

        [XmlElement("RelatedAppliedAllowanceCharge")]
        public AppliedAllowanceChargeType[] RelatedAppliedAllowanceCharge
        {
            get
            {
                return this.relatedAppliedAllowanceChargeField;
            }
            set
            {
                this.relatedAppliedAllowanceChargeField = value;
            }
        }
    }
}