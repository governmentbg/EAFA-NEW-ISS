namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAATrialBalanceTrialBalance", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAATrialBalanceTrialBalanceType
    {

        private IDType idField;

        private TextType commentField;

        private AAATrialBalanceAccountingCharacteristicType[] definedAAATrialBalanceAccountingCharacteristicField;

        private AAATrialBalanceAccountingAccountType[] includedAAATrialBalanceAccountingAccountField;

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

        public TextType Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        [XmlElement("DefinedAAATrialBalanceAccountingCharacteristic")]
        public AAATrialBalanceAccountingCharacteristicType[] DefinedAAATrialBalanceAccountingCharacteristic
        {
            get
            {
                return this.definedAAATrialBalanceAccountingCharacteristicField;
            }
            set
            {
                this.definedAAATrialBalanceAccountingCharacteristicField = value;
            }
        }

        [XmlElement("IncludedAAATrialBalanceAccountingAccount")]
        public AAATrialBalanceAccountingAccountType[] IncludedAAATrialBalanceAccountingAccount
        {
            get
            {
                return this.includedAAATrialBalanceAccountingAccountField;
            }
            set
            {
                this.includedAAATrialBalanceAccountingAccountField = value;
            }
        }
    }
}