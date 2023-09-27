namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSCountrySubDivision", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSCountrySubDivisionType
    {

        private IDType idField;

        private TextType[] nameField;

        private CodeType hierarchicalLevelCodeField;

        private LocationFunctionCodeType functionTypeCodeField;

        private SPSCountrySubDivisionType[] superordinateSPSCountrySubDivisionField;

        private SPSCountrySubDivisionType[] subordinateSPSCountrySubDivisionField;

        private SPSPartyType[] activityAuthorizedSPSPartyField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        [XmlElement("Name")]
        public TextType[] Name
        {
            get => this.nameField;
            set => this.nameField = value;
        }

        public CodeType HierarchicalLevelCode
        {
            get => this.hierarchicalLevelCodeField;
            set => this.hierarchicalLevelCodeField = value;
        }

        public LocationFunctionCodeType FunctionTypeCode
        {
            get => this.functionTypeCodeField;
            set => this.functionTypeCodeField = value;
        }

        [XmlElement("SuperordinateSPSCountrySubDivision")]
        public SPSCountrySubDivisionType[] SuperordinateSPSCountrySubDivision
        {
            get => this.superordinateSPSCountrySubDivisionField;
            set => this.superordinateSPSCountrySubDivisionField = value;
        }

        [XmlElement("SubordinateSPSCountrySubDivision")]
        public SPSCountrySubDivisionType[] SubordinateSPSCountrySubDivision
        {
            get => this.subordinateSPSCountrySubDivisionField;
            set => this.subordinateSPSCountrySubDivisionField = value;
        }

        [XmlElement("ActivityAuthorizedSPSParty")]
        public SPSPartyType[] ActivityAuthorizedSPSParty
        {
            get => this.activityAuthorizedSPSPartyField;
            set => this.activityAuthorizedSPSPartyField = value;
        }
    }
}