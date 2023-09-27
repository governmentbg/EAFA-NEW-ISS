namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CISupplyChainForecastTerms", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CISupplyChainForecastTermsType
    {

        private CodeType forecastTypeCodeField;

        private CodeType frequencyCodeField;

        private CodeType dateTypeCodeField;

        private CodeType commitmentLevelCodeField;

        public CodeType ForecastTypeCode
        {
            get => this.forecastTypeCodeField;
            set => this.forecastTypeCodeField = value;
        }

        public CodeType FrequencyCode
        {
            get => this.frequencyCodeField;
            set => this.frequencyCodeField = value;
        }

        public CodeType DateTypeCode
        {
            get => this.dateTypeCodeField;
            set => this.dateTypeCodeField = value;
        }

        public CodeType CommitmentLevelCode
        {
            get => this.commitmentLevelCodeField;
            set => this.commitmentLevelCodeField = value;
        }
    }
}