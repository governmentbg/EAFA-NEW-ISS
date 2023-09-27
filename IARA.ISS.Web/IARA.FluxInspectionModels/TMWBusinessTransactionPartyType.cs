namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWBusinessTransactionParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWBusinessTransactionPartyType
    {

        private IDType[] idField;

        private CodeType roleCodeField;

        private TMWContactType definedTMWContactField;

        [XmlElement("ID")]
        public IDType[] ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public CodeType RoleCode
        {
            get => this.roleCodeField;
            set => this.roleCodeField = value;
        }

        public TMWContactType DefinedTMWContact
        {
            get => this.definedTMWContactField;
            set => this.definedTMWContactField = value;
        }
    }
}