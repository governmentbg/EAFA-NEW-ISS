namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapPerson", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapPersonType
    {

        private IDType idField;

        private TextType nameField;

        private TextType middleNameField;

        private TextType familyNameField;

        private TextType titleField;

        private TextType salutationField;

        private TextType familyNamePrefixField;

        private IDType languageIDField;

        private AAACommunicationType[] uRIAAACommunicationField;

        private AAACommunicationType[] telephoneAAACommunicationField;

        private AAACommunicationType[] faxAAACommunicationField;

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

        public TextType MiddleName
        {
            get
            {
                return this.middleNameField;
            }
            set
            {
                this.middleNameField = value;
            }
        }

        public TextType FamilyName
        {
            get
            {
                return this.familyNameField;
            }
            set
            {
                this.familyNameField = value;
            }
        }

        public TextType Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        public TextType Salutation
        {
            get
            {
                return this.salutationField;
            }
            set
            {
                this.salutationField = value;
            }
        }

        public TextType FamilyNamePrefix
        {
            get
            {
                return this.familyNamePrefixField;
            }
            set
            {
                this.familyNamePrefixField = value;
            }
        }

        public IDType LanguageID
        {
            get
            {
                return this.languageIDField;
            }
            set
            {
                this.languageIDField = value;
            }
        }

        [XmlElement("URIAAACommunication")]
        public AAACommunicationType[] URIAAACommunication
        {
            get
            {
                return this.uRIAAACommunicationField;
            }
            set
            {
                this.uRIAAACommunicationField = value;
            }
        }

        [XmlElement("TelephoneAAACommunication")]
        public AAACommunicationType[] TelephoneAAACommunication
        {
            get
            {
                return this.telephoneAAACommunicationField;
            }
            set
            {
                this.telephoneAAACommunicationField = value;
            }
        }

        [XmlElement("FaxAAACommunication")]
        public AAACommunicationType[] FaxAAACommunication
        {
            get
            {
                return this.faxAAACommunicationField;
            }
            set
            {
                this.faxAAACommunicationField = value;
            }
        }
    }
}