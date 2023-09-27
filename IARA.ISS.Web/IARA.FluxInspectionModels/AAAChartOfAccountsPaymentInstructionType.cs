namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAChartOfAccountsPaymentInstruction", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAChartOfAccountsPaymentInstructionType
    {

        private IndicatorType priorApprovalRequirementIndicatorField;

        private AAAChartOfAccountsPaymentTermsType identifiedAAAChartOfAccountsPaymentTermsField;

        private AAAChartOfAccountsPartyType payerAgentAAAChartOfAccountsPartyField;

        public IndicatorType PriorApprovalRequirementIndicator
        {
            get
            {
                return this.priorApprovalRequirementIndicatorField;
            }
            set
            {
                this.priorApprovalRequirementIndicatorField = value;
            }
        }

        public AAAChartOfAccountsPaymentTermsType IdentifiedAAAChartOfAccountsPaymentTerms
        {
            get
            {
                return this.identifiedAAAChartOfAccountsPaymentTermsField;
            }
            set
            {
                this.identifiedAAAChartOfAccountsPaymentTermsField = value;
            }
        }

        public AAAChartOfAccountsPartyType PayerAgentAAAChartOfAccountsParty
        {
            get
            {
                return this.payerAgentAAAChartOfAccountsPartyField;
            }
            set
            {
                this.payerAgentAAAChartOfAccountsPartyField = value;
            }
        }
    }
}