namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSAuthentication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSAuthenticationType
    {

        private GovernmentActionCodeType typeCodeField;

        private DateTimeType actualDateTimeField;

        private SPSLocationType issueSPSLocationField;

        private SPSPartyType providerSPSPartyField;

        private SPSPartyType locationProviderSPSPartyField;

        private SPSClauseType[] includedSPSClauseField;

        public GovernmentActionCodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public DateTimeType ActualDateTime
        {
            get => this.actualDateTimeField;
            set => this.actualDateTimeField = value;
        }

        public SPSLocationType IssueSPSLocation
        {
            get => this.issueSPSLocationField;
            set => this.issueSPSLocationField = value;
        }

        public SPSPartyType ProviderSPSParty
        {
            get => this.providerSPSPartyField;
            set => this.providerSPSPartyField = value;
        }

        public SPSPartyType LocationProviderSPSParty
        {
            get => this.locationProviderSPSPartyField;
            set => this.locationProviderSPSPartyField = value;
        }

        [XmlElement("IncludedSPSClause")]
        public SPSClauseType[] IncludedSPSClause
        {
            get => this.includedSPSClauseField;
            set => this.includedSPSClauseField = value;
        }
    }
}