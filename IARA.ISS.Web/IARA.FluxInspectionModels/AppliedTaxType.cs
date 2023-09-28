namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AppliedTax", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AppliedTaxType
    {

        private AmountType calculatedAmountField;

        private CodeType typeCodeField;

        private RateType calculatedRateField;

        private AmountType basisAmountField;

        private DateType taxPointDateField;

        public AmountType CalculatedAmount
        {
            get
            {
                return this.calculatedAmountField;
            }
            set
            {
                this.calculatedAmountField = value;
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

        public RateType CalculatedRate
        {
            get
            {
                return this.calculatedRateField;
            }
            set
            {
                this.calculatedRateField = value;
            }
        }

        public AmountType BasisAmount
        {
            get
            {
                return this.basisAmountField;
            }
            set
            {
                this.basisAmountField = value;
            }
        }

        public DateType TaxPointDate
        {
            get
            {
                return this.taxPointDateField;
            }
            set
            {
                this.taxPointDateField = value;
            }
        }
    }
}