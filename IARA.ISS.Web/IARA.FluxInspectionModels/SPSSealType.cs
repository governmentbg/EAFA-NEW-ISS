namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSSeal", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSSealType
    {

        private IDType idField;

        private IDType maximumIDField;

        private SPSPartyType issuingSPSPartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType MaximumID
        {
            get => this.maximumIDField;
            set => this.maximumIDField = value;
        }

        public SPSPartyType IssuingSPSParty
        {
            get => this.issuingSPSPartyField;
            set => this.issuingSPSPartyField = value;
        }
    }
}