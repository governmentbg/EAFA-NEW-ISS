namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIRTSpecifiedBalanceOut", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIRTSpecifiedBalanceOutType
    {

        private IDType idField;

        private TextType[] descriptionField;

        private CodeType reasonCodeField;

        private TextType[] reasonDescriptionField;

        private DateTimeType occurrenceDateTimeField;

        private AmountType[] calculatedAmountField;

        private CIRTSpecifiedBalanceOutType[] breakdownCIRTSpecifiedBalanceOutField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public CodeType ReasonCode
        {
            get => this.reasonCodeField;
            set => this.reasonCodeField = value;
        }

        [XmlElement("ReasonDescription")]
        public TextType[] ReasonDescription
        {
            get => this.reasonDescriptionField;
            set => this.reasonDescriptionField = value;
        }

        public DateTimeType OccurrenceDateTime
        {
            get => this.occurrenceDateTimeField;
            set => this.occurrenceDateTimeField = value;
        }

        [XmlElement("CalculatedAmount")]
        public AmountType[] CalculatedAmount
        {
            get => this.calculatedAmountField;
            set => this.calculatedAmountField = value;
        }

        [XmlElement("BreakdownCIRTSpecifiedBalanceOut")]
        public CIRTSpecifiedBalanceOutType[] BreakdownCIRTSpecifiedBalanceOut
        {
            get => this.breakdownCIRTSpecifiedBalanceOutField;
            set => this.breakdownCIRTSpecifiedBalanceOutField = value;
        }
    }
}