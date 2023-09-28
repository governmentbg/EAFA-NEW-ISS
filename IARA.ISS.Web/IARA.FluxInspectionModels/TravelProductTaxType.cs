namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TravelProductTax", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TravelProductTaxType
    {

        private CodeType typeCodeField;

        private CodeType categoryCodeField;

        private CodeType basisCodeField;

        private AmountType calculatedAmountField;

        private RateType calculatedRateField;

        private AmountType taxFreeAmountField;

        private TravelProductPeriodType[] validityTravelProductPeriodField;

        private TravelProductNoteType[] additionalInformationTravelProductNoteField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType CategoryCode
        {
            get => this.categoryCodeField;
            set => this.categoryCodeField = value;
        }

        public CodeType BasisCode
        {
            get => this.basisCodeField;
            set => this.basisCodeField = value;
        }

        public AmountType CalculatedAmount
        {
            get => this.calculatedAmountField;
            set => this.calculatedAmountField = value;
        }

        public RateType CalculatedRate
        {
            get => this.calculatedRateField;
            set => this.calculatedRateField = value;
        }

        public AmountType TaxFreeAmount
        {
            get => this.taxFreeAmountField;
            set => this.taxFreeAmountField = value;
        }

        [XmlElement("ValidityTravelProductPeriod")]
        public TravelProductPeriodType[] ValidityTravelProductPeriod
        {
            get => this.validityTravelProductPeriodField;
            set => this.validityTravelProductPeriodField = value;
        }

        [XmlElement("AdditionalInformationTravelProductNote")]
        public TravelProductNoteType[] AdditionalInformationTravelProductNote
        {
            get => this.additionalInformationTravelProductNoteField;
            set => this.additionalInformationTravelProductNoteField = value;
        }
    }
}