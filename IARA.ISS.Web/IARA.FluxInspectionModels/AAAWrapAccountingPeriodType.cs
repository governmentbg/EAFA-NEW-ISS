namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapAccountingPeriod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapAccountingPeriodType
    {

        private AAAPeriodType specifiedAAAPeriodField;

        private AAAWrapAccountingCheckType specifiedAAAWrapAccountingCheckField;

        public AAAPeriodType SpecifiedAAAPeriod
        {
            get
            {
                return this.specifiedAAAPeriodField;
            }
            set
            {
                this.specifiedAAAPeriodField = value;
            }
        }

        public AAAWrapAccountingCheckType SpecifiedAAAWrapAccountingCheck
        {
            get
            {
                return this.specifiedAAAWrapAccountingCheckField;
            }
            set
            {
                this.specifiedAAAWrapAccountingCheckField = value;
            }
        }
    }
}