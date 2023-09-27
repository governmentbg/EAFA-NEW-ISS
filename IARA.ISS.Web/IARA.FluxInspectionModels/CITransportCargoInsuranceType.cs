namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITransportCargoInsurance", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITransportCargoInsuranceType
    {

        private CodeType coverageCodeField;

        private TextType coverageDescriptionField;

        private TextType contractGeneralConditionsField;

        private CITradePartyType coverageCITradePartyField;

        public CodeType CoverageCode
        {
            get
            {
                return this.coverageCodeField;
            }
            set
            {
                this.coverageCodeField = value;
            }
        }

        public TextType CoverageDescription
        {
            get
            {
                return this.coverageDescriptionField;
            }
            set
            {
                this.coverageDescriptionField = value;
            }
        }

        public TextType ContractGeneralConditions
        {
            get
            {
                return this.contractGeneralConditionsField;
            }
            set
            {
                this.contractGeneralConditionsField = value;
            }
        }

        public CITradePartyType CoverageCITradeParty
        {
            get
            {
                return this.coverageCITradePartyField;
            }
            set
            {
                this.coverageCITradePartyField = value;
            }
        }
    }
}