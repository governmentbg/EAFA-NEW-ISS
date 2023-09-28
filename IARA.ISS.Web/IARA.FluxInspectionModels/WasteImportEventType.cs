namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("WasteImportEvent", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class WasteImportEventType
    {

        private TMWPartyType relatedTMWPartyField;

        public TMWPartyType RelatedTMWParty
        {
            get => this.relatedTMWPartyField;
            set => this.relatedTMWPartyField = value;
        }
    }
}