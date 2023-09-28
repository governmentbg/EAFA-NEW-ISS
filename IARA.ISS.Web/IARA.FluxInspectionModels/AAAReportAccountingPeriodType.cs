namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportAccountingPeriod", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportAccountingPeriodType
    {

        private AAAPeriodType specifiedAAAPeriodField;

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
    }
}