namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAALedgerLedger", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAALedgerLedgerType
    {

        private IDType idField;

        private TextType commentField;

        private AAALedgerAccountingAccountType[] includedAAALedgerAccountingAccountField;

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

        [XmlElement("IncludedAAALedgerAccountingAccount")]
        public AAALedgerAccountingAccountType[] IncludedAAALedgerAccountingAccount
        {
            get
            {
                return this.includedAAALedgerAccountingAccountField;
            }
            set
            {
                this.includedAAALedgerAccountingAccountField = value;
            }
        }
    }
}