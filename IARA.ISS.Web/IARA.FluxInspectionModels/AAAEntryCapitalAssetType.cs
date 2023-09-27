namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAEntryCapitalAsset", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAEntryCapitalAssetType
    {

        private DateTimeType acquisitionDateTimeField;

        private NumericType valueSplitNumericField;

        private AAAEntryCapitalAssetAmortizationType[] specifiedAAAEntryCapitalAssetAmortizationField;

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

        [XmlElement("SpecifiedAAAEntryCapitalAssetAmortization")]
        public AAAEntryCapitalAssetAmortizationType[] SpecifiedAAAEntryCapitalAssetAmortization
        {
            get
            {
                return this.specifiedAAAEntryCapitalAssetAmortizationField;
            }
            set
            {
                this.specifiedAAAEntryCapitalAssetAmortizationField = value;
            }
        }
    }
}