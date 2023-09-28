namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CICrossBorderCustomsValuation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CICrossBorderCustomsValuationType
    {

        private AmountType addedAdjustmentAmountField;

        private AmountType deductedAdjustmentAmountField;

        private PercentType addedAdjustmentPercentField;

        private PercentType deductedAdjustmentPercentField;

        private CodeType methodCodeField;

        private CodeType wTOAdditionCodeField;

        private CodeType chargeApportionMethodCodeField;

        private AmountType[] otherChargeAmountField;

        private IndicatorType buyerSellerRelationshipIndicatorField;

        private IndicatorType buyerSellerRelationshipPriceInfluenceIndicatorField;

        private IndicatorType saleRestrictionIndicatorField;

        private IndicatorType salePriceConditionIndicatorField;

        private IndicatorType royaltyLicenseFeeIndicatorField;

        private CodeType typeCodeField;

        private TextType[] saleRestrictionField;

        private CITradeCurrencyExchangeType applicableCITradeCurrencyExchangeField;

        public AmountType AddedAdjustmentAmount
        {
            get
            {
                return this.addedAdjustmentAmountField;
            }
            set
            {
                this.addedAdjustmentAmountField = value;
            }
        }

        public AmountType DeductedAdjustmentAmount
        {
            get
            {
                return this.deductedAdjustmentAmountField;
            }
            set
            {
                this.deductedAdjustmentAmountField = value;
            }
        }

        public PercentType AddedAdjustmentPercent
        {
            get
            {
                return this.addedAdjustmentPercentField;
            }
            set
            {
                this.addedAdjustmentPercentField = value;
            }
        }

        public PercentType DeductedAdjustmentPercent
        {
            get
            {
                return this.deductedAdjustmentPercentField;
            }
            set
            {
                this.deductedAdjustmentPercentField = value;
            }
        }

        public CodeType MethodCode
        {
            get
            {
                return this.methodCodeField;
            }
            set
            {
                this.methodCodeField = value;
            }
        }

        public CodeType WTOAdditionCode
        {
            get
            {
                return this.wTOAdditionCodeField;
            }
            set
            {
                this.wTOAdditionCodeField = value;
            }
        }

        public CodeType ChargeApportionMethodCode
        {
            get
            {
                return this.chargeApportionMethodCodeField;
            }
            set
            {
                this.chargeApportionMethodCodeField = value;
            }
        }

        [XmlElement("OtherChargeAmount")]
        public AmountType[] OtherChargeAmount
        {
            get
            {
                return this.otherChargeAmountField;
            }
            set
            {
                this.otherChargeAmountField = value;
            }
        }

        public IndicatorType BuyerSellerRelationshipIndicator
        {
            get
            {
                return this.buyerSellerRelationshipIndicatorField;
            }
            set
            {
                this.buyerSellerRelationshipIndicatorField = value;
            }
        }

        public IndicatorType BuyerSellerRelationshipPriceInfluenceIndicator
        {
            get
            {
                return this.buyerSellerRelationshipPriceInfluenceIndicatorField;
            }
            set
            {
                this.buyerSellerRelationshipPriceInfluenceIndicatorField = value;
            }
        }

        public IndicatorType SaleRestrictionIndicator
        {
            get
            {
                return this.saleRestrictionIndicatorField;
            }
            set
            {
                this.saleRestrictionIndicatorField = value;
            }
        }

        public IndicatorType SalePriceConditionIndicator
        {
            get
            {
                return this.salePriceConditionIndicatorField;
            }
            set
            {
                this.salePriceConditionIndicatorField = value;
            }
        }

        public IndicatorType RoyaltyLicenseFeeIndicator
        {
            get
            {
                return this.royaltyLicenseFeeIndicatorField;
            }
            set
            {
                this.royaltyLicenseFeeIndicatorField = value;
            }
        }

        public CodeType TypeCode
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

        [XmlElement("SaleRestriction")]
        public TextType[] SaleRestriction
        {
            get
            {
                return this.saleRestrictionField;
            }
            set
            {
                this.saleRestrictionField = value;
            }
        }

        public CITradeCurrencyExchangeType ApplicableCITradeCurrencyExchange
        {
            get
            {
                return this.applicableCITradeCurrencyExchangeField;
            }
            set
            {
                this.applicableCITradeCurrencyExchangeField = value;
            }
        }
    }
}