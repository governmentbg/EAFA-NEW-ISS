namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalBookDayBook", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalBookDayBookType
    {

        private IDType idField;

        private TextType commentField;

        private AAAJournalBookAccountingJournalType[] includedAAAJournalBookAccountingJournalField;

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

        [XmlElement("IncludedAAAJournalBookAccountingJournal")]
        public AAAJournalBookAccountingJournalType[] IncludedAAAJournalBookAccountingJournal
        {
            get
            {
                return this.includedAAAJournalBookAccountingJournalField;
            }
            set
            {
                this.includedAAAJournalBookAccountingJournalField = value;
            }
        }
    }
}