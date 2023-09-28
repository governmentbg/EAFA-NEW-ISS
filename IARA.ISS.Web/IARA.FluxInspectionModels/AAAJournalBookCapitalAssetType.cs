namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookCapitalAsset", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookCapitalAssetType
    {

        private DateTimeType acquisitionDateTimeField;

        private NumericType valueSplitNumericField;

        private AAAJournalBookCapitalAssetAmortizationType[] specifiedAAAJournalBookCapitalAssetAmortizationField;

        public DateTimeType AcquisitionDateTime
        {
            get
            {
                return this.acquisitionDateTimeField;
            }
            set
            {
                this.acquisitionDateTimeField = value;
            }
        }

        public NumericType ValueSplitNumeric
        {
            get
            {
                return this.valueSplitNumericField;
            }
            set
            {
                this.valueSplitNumericField = value;
            }
        }

        [XmlElement("SpecifiedAAAJournalBookCapitalAssetAmortization")]
        public AAAJournalBookCapitalAssetAmortizationType[] SpecifiedAAAJournalBookCapitalAssetAmortization
        {
            get
            {
                return this.specifiedAAAJournalBookCapitalAssetAmortizationField;
            }
            set
            {
                this.specifiedAAAJournalBookCapitalAssetAmortizationField = value;
            }
        }
    }
}