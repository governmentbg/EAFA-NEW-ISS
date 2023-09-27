namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AggregatedCatchReportDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AggregatedCatchReportDocumentType
    {

        private DelimitedPeriodType effectiveDelimitedPeriodField;

        private FLUXPartyType ownerFLUXPartyField;

        public DelimitedPeriodType EffectiveDelimitedPeriod
        {
            get => this.effectiveDelimitedPeriodField;
            set => this.effectiveDelimitedPeriodField = value;
        }

        public FLUXPartyType OwnerFLUXParty
        {
            get => this.ownerFLUXPartyField;
            set => this.ownerFLUXPartyField = value;
        }
    }
}