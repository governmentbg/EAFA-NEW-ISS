namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DebtorFinancialAccount", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DebtorFinancialAccountType
    {

        private IDType iBANIDField;

        private IDType bBANIDField;

        private IDType uPICIDField;

        private CodeType typeCodeField;

        private TextType proprietaryTypeField;

        private CodeType currencyCodeField;

        private TextType accountNameField;

        private IDType proprietaryIDField;

        public IDType IBANID
        {
            get => this.iBANIDField;
            set => this.iBANIDField = value;
        }

        public IDType BBANID
        {
            get => this.bBANIDField;
            set => this.bBANIDField = value;
        }

        public IDType UPICID
        {
            get => this.uPICIDField;
            set => this.uPICIDField = value;
        }

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public TextType ProprietaryType
        {
            get => this.proprietaryTypeField;
            set => this.proprietaryTypeField = value;
        }

        public CodeType CurrencyCode
        {
            get => this.currencyCodeField;
            set => this.currencyCodeField = value;
        }

        public TextType AccountName
        {
            get => this.accountNameField;
            set => this.accountNameField = value;
        }

        public IDType ProprietaryID
        {
            get => this.proprietaryIDField;
            set => this.proprietaryIDField = value;
        }
    }
}