namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencePrice", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencePriceType
    {

        private AmountType chargeAmountField;

        private QuantityType[] basisQuantityField;

        private IndicatorType[] netPriceIndicatorField;

        private CodeType comparisonMethodCodeField;

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

        [XmlElement("BasisQuantity")]
        public QuantityType[] BasisQuantity
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

        [XmlElement("NetPriceIndicator")]
        public IndicatorType[] NetPriceIndicator
        {
            get
            {
                return this.netPriceIndicatorField;
            }
            set
            {
                this.netPriceIndicatorField = value;
            }
        }

        public CodeType ComparisonMethodCode
        {
            get
            {
                return this.comparisonMethodCodeField;
            }
            set
            {
                this.comparisonMethodCodeField = value;
            }
        }
    }
}