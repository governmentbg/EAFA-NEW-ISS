namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("BranchFinancialInstitution", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class BranchFinancialInstitutionType
    {

        private IDType idField;

        private TextType nameField;

        private FinancialInstitutionAddressType locationFinancialInstitutionAddressField;

        public IDType ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        public TextType Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        public FinancialInstitutionAddressType LocationFinancialInstitutionAddress
        {
            get
            {
                return this.locationFinancialInstitutionAddressField;
            }
            set
            {
                this.locationFinancialInstitutionAddressField = value;
            }
        }
    }
}