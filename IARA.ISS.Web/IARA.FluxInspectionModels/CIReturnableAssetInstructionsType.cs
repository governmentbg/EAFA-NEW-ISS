namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIReturnableAssetInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIReturnableAssetInstructionsType
    {

        private IDType[] materialIDField;

        private TextType[] termsAndConditionsDescriptionField;

        private CodeType termsAndConditionsDescriptionCodeField;

        private AmountType[] depositValueSpecifiedAmountField;

        private CISpecifiedPeriodType depositValueValidityCISpecifiedPeriodField;

        [XmlElement("MaterialID")]
        public IDType[] MaterialID
        {
            get
            {
                return this.materialIDField;
            }
            set
            {
                this.materialIDField = value;
            }
        }

        [XmlElement("TermsAndConditionsDescription")]
        public TextType[] TermsAndConditionsDescription
        {
            get
            {
                return this.termsAndConditionsDescriptionField;
            }
            set
            {
                this.termsAndConditionsDescriptionField = value;
            }
        }

        public CodeType TermsAndConditionsDescriptionCode
        {
            get
            {
                return this.termsAndConditionsDescriptionCodeField;
            }
            set
            {
                this.termsAndConditionsDescriptionCodeField = value;
            }
        }

        [XmlElement("DepositValueSpecifiedAmount")]
        public AmountType[] DepositValueSpecifiedAmount
        {
            get
            {
                return this.depositValueSpecifiedAmountField;
            }
            set
            {
                this.depositValueSpecifiedAmountField = value;
            }
        }

        public CISpecifiedPeriodType DepositValueValidityCISpecifiedPeriod
        {
            get
            {
                return this.depositValueValidityCISpecifiedPeriodField;
            }
            set
            {
                this.depositValueValidityCISpecifiedPeriodField = value;
            }
        }
    }
}