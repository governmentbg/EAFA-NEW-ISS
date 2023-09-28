namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AwardedContractChange", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AwardedContractChangeType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private AmountType estimatedCostDifferenceAmountField;

        private AmountType actualCostDifferenceAmountField;

        private CodeType negotiatedStatusCodeField;

        private DateType effectiveDateField;

        private DateType completedNegotiationsEffectiveDateField;

        private RequestingPartyType identifiedRequestingPartyField;

        private ApprovingPartyType identifiedApprovingPartyField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public AmountType EstimatedCostDifferenceAmount
        {
            get
            {
                return this.estimatedCostDifferenceAmountField;
            }
            set
            {
                this.estimatedCostDifferenceAmountField = value;
            }
        }

        public AmountType ActualCostDifferenceAmount
        {
            get
            {
                return this.actualCostDifferenceAmountField;
            }
            set
            {
                this.actualCostDifferenceAmountField = value;
            }
        }

        public CodeType NegotiatedStatusCode
        {
            get
            {
                return this.negotiatedStatusCodeField;
            }
            set
            {
                this.negotiatedStatusCodeField = value;
            }
        }

        public DateType EffectiveDate
        {
            get
            {
                return this.effectiveDateField;
            }
            set
            {
                this.effectiveDateField = value;
            }
        }

        public DateType CompletedNegotiationsEffectiveDate
        {
            get
            {
                return this.completedNegotiationsEffectiveDateField;
            }
            set
            {
                this.completedNegotiationsEffectiveDateField = value;
            }
        }

        public RequestingPartyType IdentifiedRequestingParty
        {
            get
            {
                return this.identifiedRequestingPartyField;
            }
            set
            {
                this.identifiedRequestingPartyField = value;
            }
        }

        public ApprovingPartyType IdentifiedApprovingParty
        {
            get
            {
                return this.identifiedApprovingPartyField;
            }
            set
            {
                this.identifiedApprovingPartyField = value;
            }
        }
    }
}