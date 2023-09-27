namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDisposalInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDisposalInstructionsType
    {

        private IDType materialIDField;

        private CodeType[] recyclingDescriptionCodeField;

        private TextType[] descriptionField;

        private TextType[] recyclingProcedureField;

        public IDType MaterialID
        {
            get
            {
                return this.materialIDField;
            }
            set
            {
                this.materialIDField = value;
            }
        }

        [XmlElement("RecyclingDescriptionCode")]
        public CodeType[] RecyclingDescriptionCode
        {
            get
            {
                return this.recyclingDescriptionCodeField;
            }
            set
            {
                this.recyclingDescriptionCodeField = value;
            }
        }

        [XmlElement("Description")]
        public TextType[] Description
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

        [XmlElement("RecyclingProcedure")]
        public TextType[] RecyclingProcedure
        {
            get
            {
                return this.recyclingProcedureField;
            }
            set
            {
                this.recyclingProcedureField = value;
            }
        }
    }
}