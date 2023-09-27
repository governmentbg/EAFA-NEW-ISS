namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookMonetaryAllocation", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookMonetaryAllocationType
    {

        private AmountType localCurrencyAmountField;

        private NumericType periodNumericField;

        private NumericType rankingNumericField;

        private AAAPeriodType relatedAAAPeriodField;

        public AmountType LocalCurrencyAmount
        {
            get
            {
                return this.localCurrencyAmountField;
            }
            set
            {
                this.localCurrencyAmountField = value;
            }
        }

        public NumericType PeriodNumeric
        {
            get
            {
                return this.periodNumericField;
            }
            set
            {
                this.periodNumericField = value;
            }
        }

        public NumericType RankingNumeric
        {
            get
            {
                return this.rankingNumericField;
            }
            set
            {
                this.rankingNumericField = value;
            }
        }

        public AAAPeriodType RelatedAAAPeriod
        {
            get
            {
                return this.relatedAAAPeriodField;
            }
            set
            {
                this.relatedAAAPeriodField = value;
            }
        }
    }
}