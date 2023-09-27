namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAJournalJournalList", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAJournalJournalListType
    {

        private IDType idField;

        private TextType commentField;

        private AAAJournalContactType responsibleAAAJournalContactField;

        private AAAJournalAccountingJournalType[] includedAAAJournalAccountingJournalField;

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

        public AAAJournalContactType ResponsibleAAAJournalContact
        {
            get
            {
                return this.responsibleAAAJournalContactField;
            }
            set
            {
                this.responsibleAAAJournalContactField = value;
            }
        }

        [XmlElement("IncludedAAAJournalAccountingJournal")]
        public AAAJournalAccountingJournalType[] IncludedAAAJournalAccountingJournal
        {
            get
            {
                return this.includedAAAJournalAccountingJournalField;
            }
            set
            {
                this.includedAAAJournalAccountingJournalField = value;
            }
        }
    }
}