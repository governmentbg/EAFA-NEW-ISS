namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISupplyChainInventory", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISupplyChainInventoryType
    {

        private QuantityType stockQuantityField;

        private DateTimeType calculationDateTimeField;

        private MeasureType minimumStockLevelMeasureField;

        private MeasureType maximumStockLevelMeasureField;

        private DateTimeType plannedStockCalculationDateTimeField;

        private QuantityType plannedStockQuantityField;

        private QuantityType averageDemandQuantityField;

        private DateTimeType averageDurationDateTimeField;

        private QuantityType maximumStockQuantityField;

        private QuantityType minimumStockQuantityField;

        private CodeType statusCodeField;

        private CILogisticsLocationType specifiedCILogisticsLocationField;

        private CINoteType[] remarkCINoteField;

        private CIReferencedDocumentType dispositionCIReferencedDocumentField;

        public QuantityType StockQuantity
        {
            get => this.stockQuantityField;
            set => this.stockQuantityField = value;
        }

        public DateTimeType CalculationDateTime
        {
            get => this.calculationDateTimeField;
            set => this.calculationDateTimeField = value;
        }

        public MeasureType MinimumStockLevelMeasure
        {
            get => this.minimumStockLevelMeasureField;
            set => this.minimumStockLevelMeasureField = value;
        }

        public MeasureType MaximumStockLevelMeasure
        {
            get => this.maximumStockLevelMeasureField;
            set => this.maximumStockLevelMeasureField = value;
        }

        public DateTimeType PlannedStockCalculationDateTime
        {
            get => this.plannedStockCalculationDateTimeField;
            set => this.plannedStockCalculationDateTimeField = value;
        }

        public QuantityType PlannedStockQuantity
        {
            get => this.plannedStockQuantityField;
            set => this.plannedStockQuantityField = value;
        }

        public QuantityType AverageDemandQuantity
        {
            get => this.averageDemandQuantityField;
            set => this.averageDemandQuantityField = value;
        }

        public DateTimeType AverageDurationDateTime
        {
            get => this.averageDurationDateTimeField;
            set => this.averageDurationDateTimeField = value;
        }

        public QuantityType MaximumStockQuantity
        {
            get => this.maximumStockQuantityField;
            set => this.maximumStockQuantityField = value;
        }

        public QuantityType MinimumStockQuantity
        {
            get => this.minimumStockQuantityField;
            set => this.minimumStockQuantityField = value;
        }

        public CodeType StatusCode
        {
            get => this.statusCodeField;
            set => this.statusCodeField = value;
        }

        public CILogisticsLocationType SpecifiedCILogisticsLocation
        {
            get => this.specifiedCILogisticsLocationField;
            set => this.specifiedCILogisticsLocationField = value;
        }

        [XmlElement("RemarkCINote")]
        public CINoteType[] RemarkCINote
        {
            get => this.remarkCINoteField;
            set => this.remarkCINoteField = value;
        }

        public CIReferencedDocumentType DispositionCIReferencedDocument
        {
            get => this.dispositionCIReferencedDocumentField;
            set => this.dispositionCIReferencedDocumentField = value;
        }
    }
}