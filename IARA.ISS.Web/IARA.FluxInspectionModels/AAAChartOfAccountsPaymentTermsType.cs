namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAChartOfAccountsPaymentTerms", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAChartOfAccountsPaymentTermsType
    {

        private MeasureType durationMeasureField;

        private PaymentTermsTypeCodeType typeCodeField;

        private RateType settlementDiscountRateField;

        private PaymentTermsEventTimeReferenceCodeType fromEventCodeField;

        private AdditionalPostponementCodeType additionalPostponementCodeField;

        private AAAChartOfAccountsFinancialAccountType relatedAAAChartOfAccountsFinancialAccountField;

        public MeasureType DurationMeasure
        {
            get
            {
                return this.durationMeasureField;
            }
            set
            {
                this.durationMeasureField = value;
            }
        }

        public PaymentTermsTypeCodeType TypeCode
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

        public RateType SettlementDiscountRate
        {
            get
            {
                return this.settlementDiscountRateField;
            }
            set
            {
                this.settlementDiscountRateField = value;
            }
        }

        public PaymentTermsEventTimeReferenceCodeType FromEventCode
        {
            get
            {
                return this.fromEventCodeField;
            }
            set
            {
                this.fromEventCodeField = value;
            }
        }

        public AdditionalPostponementCodeType AdditionalPostponementCode
        {
            get
            {
                return this.additionalPostponementCodeField;
            }
            set
            {
                this.additionalPostponementCodeField = value;
            }
        }

        public AAAChartOfAccountsFinancialAccountType RelatedAAAChartOfAccountsFinancialAccount
        {
            get
            {
                return this.relatedAAAChartOfAccountsFinancialAccountField;
            }
            set
            {
                this.relatedAAAChartOfAccountsFinancialAccountField = value;
            }
        }
    }
}