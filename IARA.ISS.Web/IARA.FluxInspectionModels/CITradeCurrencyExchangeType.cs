namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeCurrencyExchange", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradeCurrencyExchangeType
    {

        private CurrencyCodeType sourceCurrencyCodeField;

        private NumericType sourceUnitBasisNumericField;

        private CurrencyCodeType targetCurrencyCodeField;

        private NumericType targetUnitBaseNumericField;

        private IDType marketIDField;

        private RateType conversionRateField;

        private DateTimeType conversionRateDateTimeField;

        private CIReferencedDocumentType referencedCIReferencedDocumentField;

        public CurrencyCodeType SourceCurrencyCode
        {
            get
            {
                return this.sourceCurrencyCodeField;
            }
            set
            {
                this.sourceCurrencyCodeField = value;
            }
        }

        public NumericType SourceUnitBasisNumeric
        {
            get
            {
                return this.sourceUnitBasisNumericField;
            }
            set
            {
                this.sourceUnitBasisNumericField = value;
            }
        }

        public CurrencyCodeType TargetCurrencyCode
        {
            get
            {
                return this.targetCurrencyCodeField;
            }
            set
            {
                this.targetCurrencyCodeField = value;
            }
        }

        public NumericType TargetUnitBaseNumeric
        {
            get
            {
                return this.targetUnitBaseNumericField;
            }
            set
            {
                this.targetUnitBaseNumericField = value;
            }
        }

        public IDType MarketID
        {
            get
            {
                return this.marketIDField;
            }
            set
            {
                this.marketIDField = value;
            }
        }

        public RateType ConversionRate
        {
            get
            {
                return this.conversionRateField;
            }
            set
            {
                this.conversionRateField = value;
            }
        }

        public DateTimeType ConversionRateDateTime
        {
            get
            {
                return this.conversionRateDateTimeField;
            }
            set
            {
                this.conversionRateDateTimeField = value;
            }
        }

        public CIReferencedDocumentType ReferencedCIReferencedDocument
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
    }
}