namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRHTradeSettlementPayment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRHTradeSettlementPaymentType
    {

        private IDType endToEndIDField;

        private IDType instructionIDField;

        private DateTimeType requestedExecutionDateTimeField;

        private DateTimeType closingBookDueDateTimeField;

        private CIRHSupplyChainTradeSettlementType[] specifiedCIRHSupplyChainTradeSettlementField;

        public IDType EndToEndID
        {
            get => this.endToEndIDField;
            set => this.endToEndIDField = value;
        }

        public IDType InstructionID
        {
            get => this.instructionIDField;
            set => this.instructionIDField = value;
        }

        public DateTimeType RequestedExecutionDateTime
        {
            get => this.requestedExecutionDateTimeField;
            set => this.requestedExecutionDateTimeField = value;
        }

        public DateTimeType ClosingBookDueDateTime
        {
            get => this.closingBookDueDateTimeField;
            set => this.closingBookDueDateTimeField = value;
        }

        [XmlElement("SpecifiedCIRHSupplyChainTradeSettlement")]
        public CIRHSupplyChainTradeSettlementType[] SpecifiedCIRHSupplyChainTradeSettlement
        {
            get => this.specifiedCIRHSupplyChainTradeSettlementField;
            set => this.specifiedCIRHSupplyChainTradeSettlementField = value;
        }
    }
}