namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ChemicalReactivityInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ChemicalReactivityInstructionsType
    {

        private TextType materialToAvoidItemNameField;

        private TextType incompatibilityDescriptionField;

        public TextType MaterialToAvoidItemName
        {
            get
            {
                return this.materialToAvoidItemNameField;
            }
            set
            {
                this.materialToAvoidItemNameField = value;
            }
        }

        public TextType IncompatibilityDescription
        {
            get
            {
                return this.incompatibilityDescriptionField;
            }
            set
            {
                this.incompatibilityDescriptionField = value;
            }
        }
    }
}