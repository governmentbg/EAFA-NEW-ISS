namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIDeliveryInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIDeliveryInstructionsType
    {

        private CodeType[] handlingCodeField;

        private TextType[] handlingField;

        private TextType[] descriptionField;

        private CodeType descriptionCodeField;

        [XmlElement("HandlingCode")]
        public CodeType[] HandlingCode
        {
            get
            {
                return this.handlingCodeField;
            }
            set
            {
                this.handlingCodeField = value;
            }
        }

        [XmlElement("Handling")]
        public TextType[] Handling
        {
            get
            {
                return this.handlingField;
            }
            set
            {
                this.handlingField = value;
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

        public CodeType DescriptionCode
        {
            get
            {
                return this.descriptionCodeField;
            }
            set
            {
                this.descriptionCodeField = value;
            }
        }
    }
}