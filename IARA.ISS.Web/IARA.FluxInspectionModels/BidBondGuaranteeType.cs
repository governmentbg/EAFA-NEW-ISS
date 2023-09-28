namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BidBondGuarantee", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BidBondGuaranteeType
    {

        private TextType descriptionField;

        private AmountType liabilityAmountField;

        private RateType amountRateField;

        private GuaranteeCreditorOrganizationType creditChargeGuaranteeCreditorOrganizationField;

        private TenderingPeriodType effectiveTenderingPeriodField;

        private GuaranteeRequestingOrganizationType subscriberGuaranteeRequestingOrganizationField;

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public AmountType LiabilityAmount
        {
            get
            {
                return this.liabilityAmountField;
            }
            set
            {
                this.liabilityAmountField = value;
            }
        }

        public RateType AmountRate
        {
            get
            {
                return this.amountRateField;
            }
            set
            {
                this.amountRateField = value;
            }
        }

        public GuaranteeCreditorOrganizationType CreditChargeGuaranteeCreditorOrganization
        {
            get
            {
                return this.creditChargeGuaranteeCreditorOrganizationField;
            }
            set
            {
                this.creditChargeGuaranteeCreditorOrganizationField = value;
            }
        }

        public TenderingPeriodType EffectiveTenderingPeriod
        {
            get
            {
                return this.effectiveTenderingPeriodField;
            }
            set
            {
                this.effectiveTenderingPeriodField = value;
            }
        }

        public GuaranteeRequestingOrganizationType SubscriberGuaranteeRequestingOrganization
        {
            get
            {
                return this.subscriberGuaranteeRequestingOrganizationField;
            }
            set
            {
                this.subscriberGuaranteeRequestingOrganizationField = value;
            }
        }
    }
}