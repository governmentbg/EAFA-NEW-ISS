namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookAccountingJournal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookAccountingJournalType
    {

        private IDType idField;

        private TextType nameField;

        private AccountingJournalCodeType typeCodeField;

        private AccountingJournalCategoryCodeType categoryCodeField;

        private AAAJournalBookAccountingEntryType[] includedAAAJournalBookAccountingEntryField;

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

        public AccountingJournalCodeType TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public AccountingJournalCategoryCodeType CategoryCode
        {
            get
            {
                return this.categoryCodeField;
            }
            set
            {
                this.categoryCodeField = value;
            }
        }

        [XmlElement("IncludedAAAJournalBookAccountingEntry")]
        public AAAJournalBookAccountingEntryType[] IncludedAAAJournalBookAccountingEntry
        {
            get
            {
                return this.includedAAAJournalBookAccountingEntryField;
            }
            set
            {
                this.includedAAAJournalBookAccountingEntryField = value;
            }
        }
    }
}