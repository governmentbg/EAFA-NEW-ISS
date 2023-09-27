namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TMWHandlingInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TMWHandlingInstructionsType
    {

        private TextType handlingField;

        public TextType Handling
        {
            get => this.handlingField;
            set => this.handlingField = value;
        }
    }
}