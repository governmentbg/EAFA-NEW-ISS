namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSProcess", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSProcessType
    {

        private ProcessTypeCodeType typeCodeField;

        private SPSPeriodType completionSPSPeriodField;

        private SPSProcessCharacteristicType[] applicableSPSProcessCharacteristicField;

        private SPSCountryType operationSPSCountryField;

        private SPSPartyType operatorSPSPartyField;

        public ProcessTypeCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public SPSPeriodType CompletionSPSPeriod
        {
            get => this.completionSPSPeriodField;
            set => this.completionSPSPeriodField = value;
        }

        [XmlElement("ApplicableSPSProcessCharacteristic")]
        public SPSProcessCharacteristicType[] ApplicableSPSProcessCharacteristic
        {
            get => this.applicableSPSProcessCharacteristicField;
            set => this.applicableSPSProcessCharacteristicField = value;
        }

        public SPSCountryType OperationSPSCountry
        {
            get => this.operationSPSCountryField;
            set => this.operationSPSCountryField = value;
        }

        public SPSPartyType OperatorSPSParty
        {
            get => this.operatorSPSPartyField;
            set => this.operatorSPSPartyField = value;
        }
    }
}