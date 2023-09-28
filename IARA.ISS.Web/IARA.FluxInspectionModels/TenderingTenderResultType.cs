namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TenderingTenderResult", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TenderingTenderResultType
    {

        private CodeType resultCodeField;

        private TextType descriptionField;

        private AmountType awardPriceAmountField;

        private TendererPartyType winningTendererPartyField;

        private IndividualTendererResultType[] detailedIndividualTendererResultField;

        public CodeType ResultCode
        {
            get => this.resultCodeField;
            set => this.resultCodeField = value;
        }

        public TextType Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public AmountType AwardPriceAmount
        {
            get => this.awardPriceAmountField;
            set => this.awardPriceAmountField = value;
        }

        public TendererPartyType WinningTendererParty
        {
            get => this.winningTendererPartyField;
            set => this.winningTendererPartyField = value;
        }

        [XmlElement("DetailedIndividualTendererResult")]
        public IndividualTendererResultType[] DetailedIndividualTendererResult
        {
            get => this.detailedIndividualTendererResultField;
            set => this.detailedIndividualTendererResultField = value;
        }
    }
}