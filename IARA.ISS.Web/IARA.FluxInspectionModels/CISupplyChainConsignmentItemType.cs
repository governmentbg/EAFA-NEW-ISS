namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISupplyChainConsignmentItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISupplyChainConsignmentItemType
    {

        private GoodsTypeCodeType typeCodeField;

        private GoodsTypeExtensionCodeType typeExtensionCodeField;

        private AmountType declaredValueForCustomsAmountField;

        private AmountType declaredValueForStatisticsAmountField;

        private AmountType[] invoiceAmountField;

        private WeightUnitMeasureType grossWeightMeasureField;

        private WeightUnitMeasureType netWeightMeasureField;

        private QuantityType tariffQuantityField;

        private TransportCargoType natureIdentificationTransportCargoField;

        public GoodsTypeCodeType TypeCode
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

        public GoodsTypeExtensionCodeType TypeExtensionCode
        {
            get
            {
                return this.typeExtensionCodeField;
            }
            set
            {
                this.typeExtensionCodeField = value;
            }
        }

        public AmountType DeclaredValueForCustomsAmount
        {
            get
            {
                return this.declaredValueForCustomsAmountField;
            }
            set
            {
                this.declaredValueForCustomsAmountField = value;
            }
        }

        public AmountType DeclaredValueForStatisticsAmount
        {
            get
            {
                return this.declaredValueForStatisticsAmountField;
            }
            set
            {
                this.declaredValueForStatisticsAmountField = value;
            }
        }

        [XmlElement("InvoiceAmount")]
        public AmountType[] InvoiceAmount
        {
            get
            {
                return this.invoiceAmountField;
            }
            set
            {
                this.invoiceAmountField = value;
            }
        }

        public WeightUnitMeasureType GrossWeightMeasure
        {
            get
            {
                return this.grossWeightMeasureField;
            }
            set
            {
                this.grossWeightMeasureField = value;
            }
        }

        public WeightUnitMeasureType NetWeightMeasure
        {
            get
            {
                return this.netWeightMeasureField;
            }
            set
            {
                this.netWeightMeasureField = value;
            }
        }

        public QuantityType TariffQuantity
        {
            get
            {
                return this.tariffQuantityField;
            }
            set
            {
                this.tariffQuantityField = value;
            }
        }

        public TransportCargoType NatureIdentificationTransportCargo
        {
            get
            {
                return this.natureIdentificationTransportCargoField;
            }
            set
            {
                this.natureIdentificationTransportCargoField = value;
            }
        }
    }
}