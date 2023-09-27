namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WasteRecoveryDisposalEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WasteRecoveryDisposalEventType
    {

        private TMWPartyType relatedTMWPartyField;

        private TMWLocationType occurrenceTMWLocationField;

        private WasteRecoveryDisposalProcessType appliedWasteRecoveryDisposalProcessField;

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

        public WasteRecoveryDisposalProcessType AppliedWasteRecoveryDisposalProcess
        {
            get => this.appliedWasteRecoveryDisposalProcessField;
            set => this.appliedWasteRecoveryDisposalProcessField = value;
        }
    }
}