namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WasteGenerationEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WasteGenerationEventType
    {

        private TMWPartyType relatedTMWPartyField;

        private TMWLocationType occurrenceTMWLocationField;

        private WasteOriginProcessType appliedWasteOriginProcessField;

        public TMWPartyType RelatedTMWParty
        {
            get => this.relatedTMWPartyField;
            set => this.relatedTMWPartyField = value;
        }

        public TMWLocationType OccurrenceTMWLocation
        {
            get => this.occurrenceTMWLocationField;
            set => this.occurrenceTMWLocationField = value;
        }

        public WasteOriginProcessType AppliedWasteOriginProcess
        {
            get => this.appliedWasteOriginProcessField;
            set => this.appliedWasteOriginProcessField = value;
        }
    }
}