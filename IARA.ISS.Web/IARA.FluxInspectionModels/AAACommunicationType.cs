namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAACommunication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAACommunicationType
    {

        private IDType uRIIDField;

        private TextType localNumberField;

        private TextType completeNumberField;

        private CodeType countryNumberCodeField;

        private TextType extensionNumberField;

        public IDType URIID
        {
            get
            {
                return this.uRIIDField;
            }
            set
            {
                this.uRIIDField = value;
            }
        }

        public TextType LocalNumber
        {
            get
            {
                return this.localNumberField;
            }
            set
            {
                this.localNumberField = value;
            }
        }

        public TextType CompleteNumber
        {
            get
            {
                return this.completeNumberField;
            }
            set
            {
                this.completeNumberField = value;
            }
        }

        public CodeType CountryNumberCode
        {
            get
            {
                return this.countryNumberCodeField;
            }
            set
            {
                this.countryNumberCodeField = value;
            }
        }

        public TextType ExtensionNumber
        {
            get
            {
                return this.extensionNumberField;
            }
            set
            {
                this.extensionNumberField = value;
            }
        }
    }
}