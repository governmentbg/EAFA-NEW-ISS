namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DailyPrice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DailyPriceType
    {

        private CodeType typeCodeField;

        private TextType typeField;

        private FormattedDateTimeType chargeDateTimeField;

        private AmountType basicChargeAmountField;

        private AmountType extraChargeAmountField;

        private AmountType discountChargeAmountField;

        private QuantityType customerServicePointQuantityField;

        private AmountType serviceChargeAmountField;

        private AmountType travelProductTaxChargeAmountField;

        private AmountType commissionChargeAmountField;

        private AmountType taxCommissionChargeAmountField;

        private AmountType cancellationChargeAmountField;

        private TextType informationField;

        private BasicPriceType chargedBasicPriceField;

        private ExtraPriceType[] chargedExtraPriceField;

        private DiscountPriceType[] chargedDiscountPriceField;

        private ServicePriceType chargedServicePriceField;

        private TravelProductTaxType[] chargedTravelProductTaxField;

        private CancellationPriceType[] chargedCancellationPriceField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType Type
        {
            get => this.typeField;
            set => this.typeField = value;
        }

        public FormattedDateTimeType ChargeDateTime
        {
            get => this.chargeDateTimeField;
            set => this.chargeDateTimeField = value;
        }

        public AmountType BasicChargeAmount
        {
            get => this.basicChargeAmountField;
            set => this.basicChargeAmountField = value;
        }

        public AmountType ExtraChargeAmount
        {
            get => this.extraChargeAmountField;
            set => this.extraChargeAmountField = value;
        }

        public AmountType DiscountChargeAmount
        {
            get => this.discountChargeAmountField;
            set => this.discountChargeAmountField = value;
        }

        public QuantityType CustomerServicePointQuantity
        {
            get => this.customerServicePointQuantityField;
            set => this.customerServicePointQuantityField = value;
        }

        public AmountType ServiceChargeAmount
        {
            get => this.serviceChargeAmountField;
            set => this.serviceChargeAmountField = value;
        }

        public AmountType TravelProductTaxChargeAmount
        {
            get => this.travelProductTaxChargeAmountField;
            set => this.travelProductTaxChargeAmountField = value;
        }

        public AmountType CommissionChargeAmount
        {
            get => this.commissionChargeAmountField;
            set => this.commissionChargeAmountField = value;
        }

        public AmountType TaxCommissionChargeAmount
        {
            get => this.taxCommissionChargeAmountField;
            set => this.taxCommissionChargeAmountField = value;
        }

        public AmountType CancellationChargeAmount
        {
            get => this.cancellationChargeAmountField;
            set => this.cancellationChargeAmountField = value;
        }

        public TextType Information
        {
            get => this.informationField;
            set => this.informationField = value;
        }

        public BasicPriceType ChargedBasicPrice
        {
            get => this.chargedBasicPriceField;
            set => this.chargedBasicPriceField = value;
        }

        [XmlElement("ChargedExtraPrice")]
        public ExtraPriceType[] ChargedExtraPrice
        {
            get => this.chargedExtraPriceField;
            set => this.chargedExtraPriceField = value;
        }

        [XmlElement("ChargedDiscountPrice")]
        public DiscountPriceType[] ChargedDiscountPrice
        {
            get => this.chargedDiscountPriceField;
            set => this.chargedDiscountPriceField = value;
        }

        public ServicePriceType ChargedServicePrice
        {
            get => this.chargedServicePriceField;
            set => this.chargedServicePriceField = value;
        }

        [XmlElement("ChargedTravelProductTax")]
        public TravelProductTaxType[] ChargedTravelProductTax
        {
            get => this.chargedTravelProductTaxField;
            set => this.chargedTravelProductTaxField = value;
        }

        [XmlElement("ChargedCancellationPrice")]
        public CancellationPriceType[] ChargedCancellationPrice
        {
            get => this.chargedCancellationPriceField;
            set => this.chargedCancellationPriceField = value;
        }
    }
}