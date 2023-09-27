namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapBundleCollection", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapBundleCollectionType
    {

        private IDType idField;

        private TextType commentField;

        private AAAWrapAccountingPeriodType[] specifiedAAAWrapAccountingPeriodField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public TextType Comment
        {
            get => this.commentField;
            set => this.commentField = value;
        }

        [XmlElement("SpecifiedAAAWrapAccountingPeriod")]
        public AAAWrapAccountingPeriodType[] SpecifiedAAAWrapAccountingPeriod
        {
            get => this.specifiedAAAWrapAccountingPeriodField;
            set => this.specifiedAAAWrapAccountingPeriodField = value;
        }
    }
}