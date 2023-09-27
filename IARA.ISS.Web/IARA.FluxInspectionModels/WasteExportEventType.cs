namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WasteExportEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WasteExportEventType
    {

        private TMWPartyType relatedTMWPartyField;

        public TMWPartyType RelatedTMWParty
        {
            get => this.relatedTMWPartyField;
            set => this.relatedTMWPartyField = value;
        }
    }
}