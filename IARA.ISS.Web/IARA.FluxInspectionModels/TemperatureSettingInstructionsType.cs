namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TemperatureSettingInstructions", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TemperatureSettingInstructionsType
    {

        private TextType[] procedureField;

        private TextType[] descriptionField;

        private CodeType descriptionCodeField;

        [XmlElement("Procedure")]
        public TextType[] Procedure
        {
            get
            {
                return this.procedureField;
            }
            set
            {
                this.procedureField = value;
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