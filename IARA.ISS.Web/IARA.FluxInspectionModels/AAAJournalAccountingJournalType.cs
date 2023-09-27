namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalAccountingJournal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalAccountingJournalType
    {

        private IDType idField;

        private TextType nameField;

        private AccountingJournalCodeType typeCodeField;

        private AccountingJournalCategoryCodeType categoryCodeField;

        private DateTimeType openingDateTimeField;

        private DateTimeType closingDateTimeField;

        private AAAJournalContactType openingResponsibleAAAJournalContactField;

        private AAAJournalContactType closingResponsibleAAAJournalContactField;

        private AAAJournalAccountingAccountType offsettingAAAJournalAccountingAccountField;

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

        public DateTimeType OpeningDateTime
        {
            get
            {
                return this.openingDateTimeField;
            }
            set
            {
                this.openingDateTimeField = value;
            }
        }

        public DateTimeType ClosingDateTime
        {
            get
            {
                return this.closingDateTimeField;
            }
            set
            {
                this.closingDateTimeField = value;
            }
        }

        public AAAJournalContactType OpeningResponsibleAAAJournalContact
        {
            get
            {
                return this.openingResponsibleAAAJournalContactField;
            }
            set
            {
                this.openingResponsibleAAAJournalContactField = value;
            }
        }

        public AAAJournalContactType ClosingResponsibleAAAJournalContact
        {
            get
            {
                return this.closingResponsibleAAAJournalContactField;
            }
            set
            {
                this.closingResponsibleAAAJournalContactField = value;
            }
        }

        public AAAJournalAccountingAccountType OffsettingAAAJournalAccountingAccount
        {
            get
            {
                return this.offsettingAAAJournalAccountingAccountField;
            }
            set
            {
                this.offsettingAAAJournalAccountingAccountField = value;
            }
        }
    }
}