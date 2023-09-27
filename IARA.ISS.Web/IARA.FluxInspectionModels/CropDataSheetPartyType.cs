namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CropDataSheetParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CropDataSheetPartyType
    {

        private IDType idField;

        private TextType nameField;

        private TextType descriptionField;

        private PartyContactType definedPartyContactField;

        private StructuredAddressType specifiedStructuredAddressField;

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

        public TextType Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        public PartyContactType DefinedPartyContact
        {
            get
            {
                return this.definedPartyContactField;
            }
            set
            {
                this.definedPartyContactField = value;
            }
        }

        public StructuredAddressType SpecifiedStructuredAddress
        {
            get
            {
                return this.specifiedStructuredAddressField;
            }
            set
            {
                this.specifiedStructuredAddressField = value;
            }
        }
    }
}