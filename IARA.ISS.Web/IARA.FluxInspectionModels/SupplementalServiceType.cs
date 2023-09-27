namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SupplementalService", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SupplementalServiceType
    {

        private IDType idField;

        private TextType descriptionField;

        private TextType nameField;

        private SpecifiedServiceOptionType[] specificSpecifiedServiceOptionField;

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

        [XmlElement("SpecificSpecifiedServiceOption")]
        public SpecifiedServiceOptionType[] SpecificSpecifiedServiceOption
        {
            get
            {
                return this.specificSpecifiedServiceOptionField;
            }
            set
            {
                this.specificSpecifiedServiceOptionField = value;
            }
        }
    }
}