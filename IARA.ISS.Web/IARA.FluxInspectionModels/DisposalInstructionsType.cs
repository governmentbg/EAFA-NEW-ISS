namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("DisposalInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class DisposalInstructionsType
    {

        private TextType handlingField;

        private TextType rCRAHandlingField;

        private IDType materialIDField;

        private CodeType[] recyclingDescriptionCodeField;

        private TextType[] descriptionField;

        private TextType[] recyclingProcedureField;

        public TextType Handling
        {
            get => this.handlingField;
            set => this.handlingField = value;
        }

        public TextType RCRAHandling
        {
            get => this.rCRAHandlingField;
            set => this.rCRAHandlingField = value;
        }

        public IDType MaterialID
        {
            get => this.materialIDField;
            set => this.materialIDField = value;
        }

        [XmlElement("RecyclingDescriptionCode")]
        public CodeType[] RecyclingDescriptionCode
        {
            get => this.recyclingDescriptionCodeField;
            set => this.recyclingDescriptionCodeField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("RecyclingProcedure")]
        public TextType[] RecyclingProcedure
        {
            get => this.recyclingProcedureField;
            set => this.recyclingProcedureField = value;
        }
    }
}