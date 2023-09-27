namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapLedger", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapLedgerType
    {

        private IDType idField;

        private TextType commentField;

        private AAAWrapAccountingPeriodType[] specifiedAAAWrapAccountingPeriodField;

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

        [XmlElement("SpecifiedAAAWrapAccountingPeriod")]
        public AAAWrapAccountingPeriodType[] SpecifiedAAAWrapAccountingPeriod
        {
            get
            {
                return this.specifiedAAAWrapAccountingPeriodField;
            }
            set
            {
                this.specifiedAAAWrapAccountingPeriodField = value;
            }
        }
    }
}