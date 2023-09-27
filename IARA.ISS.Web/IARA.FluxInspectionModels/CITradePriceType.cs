namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradePrice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradePriceType
    {

        private PriceTypeCodeType typeCodeField;

        private AmountType[] chargeAmountField;

        private QuantityType basisQuantityField;

        private NumericType orderUnitConversionFactorNumericField;

        private TextType[] changeReasonField;

        private QuantityType minimumQuantityField;

        private QuantityType maximumQuantityField;

        private TextType[] typeField;

        private DateTimeType basisDateTimeField;

        private CITradeAllowanceChargeType[] appliedCITradeAllowanceChargeField;

        private CISpecifiedPeriodType validityCISpecifiedPeriodField;

        private CIReferencedDocumentType[] referencedCIReferencedDocumentField;

        private CITradeLocationType[] deliveryCITradeLocationField;

        private ReferencePriceType[] tradeComparisonReferencePriceField;

        public PriceTypeCodeType TypeCode
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

        public QuantityType BasisQuantity
        {
            get
            {
                return this.basisQuantityField;
            }
            set
            {
                this.basisQuantityField = value;
            }
        }

        public NumericType OrderUnitConversionFactorNumeric
        {
            get
            {
                return this.orderUnitConversionFactorNumericField;
            }
            set
            {
                this.orderUnitConversionFactorNumericField = value;
            }
        }

        [XmlElement("ChangeReason")]
        public TextType[] ChangeReason
        {
            get
            {
                return this.changeReasonField;
            }
            set
            {
                this.changeReasonField = value;
            }
        }

        public QuantityType MinimumQuantity
        {
            get
            {
                return this.minimumQuantityField;
            }
            set
            {
                this.minimumQuantityField = value;
            }
        }

        public QuantityType MaximumQuantity
        {
            get
            {
                return this.maximumQuantityField;
            }
            set
            {
                this.maximumQuantityField = value;
            }
        }

        [XmlElement("Type")]
        public TextType[] Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        public DateTimeType BasisDateTime
        {
            get
            {
                return this.basisDateTimeField;
            }
            set
            {
                this.basisDateTimeField = value;
            }
        }

        [XmlElement("AppliedCITradeAllowanceCharge")]
        public CITradeAllowanceChargeType[] AppliedCITradeAllowanceCharge
        {
            get
            {
                return this.appliedCITradeAllowanceChargeField;
            }
            set
            {
                this.appliedCITradeAllowanceChargeField = value;
            }
        }

        public CISpecifiedPeriodType ValidityCISpecifiedPeriod
        {
            get
            {
                return this.validityCISpecifiedPeriodField;
            }
            set
            {
                this.validityCISpecifiedPeriodField = value;
            }
        }

        [XmlElement("ReferencedCIReferencedDocument")]
        public CIReferencedDocumentType[] ReferencedCIReferencedDocument
        {
            get
            {
                return this.referencedCIReferencedDocumentField;
            }
            set
            {
                this.referencedCIReferencedDocumentField = value;
            }
        }

        [XmlElement("DeliveryCITradeLocation")]
        public CITradeLocationType[] DeliveryCITradeLocation
        {
            get
            {
                return this.deliveryCITradeLocationField;
            }
            set
            {
                this.deliveryCITradeLocationField = value;
            }
        }

        [XmlElement("TradeComparisonReferencePrice")]
        public ReferencePriceType[] TradeComparisonReferencePrice
        {
            get
            {
                return this.tradeComparisonReferencePriceField;
            }
            set
            {
                this.tradeComparisonReferencePriceField = value;
            }
        }
    }
}