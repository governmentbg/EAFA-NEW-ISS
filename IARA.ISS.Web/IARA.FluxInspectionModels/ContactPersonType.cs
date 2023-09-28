namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ContactPerson", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ContactPersonType
    {

        private TextType titleField;

        private TextType givenNameField;

        private TextType middleNameField;

        private TextType familyNamePrefixField;

        private TextType[] familyNameField;

        private TextType nameSuffixField;

        private CodeType genderCodeField;

        private TextType aliasField;

        private DateTimeType birthDateTimeField;

        private TextType birthplaceNameField;

        private TelecommunicationCommunicationType telephoneTelecommunicationCommunicationField;

        private TelecommunicationCommunicationType faxTelecommunicationCommunicationField;

        private EmailCommunicationType emailURIEmailCommunicationField;

        private WebsiteCommunicationType websiteURIWebsiteCommunicationField;

        private UniversalCommunicationType[] specifiedUniversalCommunicationField;

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

        public TextType GivenName
        {
            get
            {
                return this.givenNameField;
            }
            set
            {
                this.givenNameField = value;
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

        [XmlElement("FamilyName")]
        public TextType[] FamilyName
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

        public TextType NameSuffix
        {
            get
            {
                return this.nameSuffixField;
            }
            set
            {
                this.nameSuffixField = value;
            }
        }

        public CodeType GenderCode
        {
            get
            {
                return this.genderCodeField;
            }
            set
            {
                this.genderCodeField = value;
            }
        }

        public TextType Alias
        {
            get
            {
                return this.aliasField;
            }
            set
            {
                this.aliasField = value;
            }
        }

        public DateTimeType BirthDateTime
        {
            get
            {
                return this.birthDateTimeField;
            }
            set
            {
                this.birthDateTimeField = value;
            }
        }

        public TextType BirthplaceName
        {
            get
            {
                return this.birthplaceNameField;
            }
            set
            {
                this.birthplaceNameField = value;
            }
        }

        public TelecommunicationCommunicationType TelephoneTelecommunicationCommunication
        {
            get
            {
                return this.telephoneTelecommunicationCommunicationField;
            }
            set
            {
                this.telephoneTelecommunicationCommunicationField = value;
            }
        }

        public TelecommunicationCommunicationType FaxTelecommunicationCommunication
        {
            get
            {
                return this.faxTelecommunicationCommunicationField;
            }
            set
            {
                this.faxTelecommunicationCommunicationField = value;
            }
        }

        public EmailCommunicationType EmailURIEmailCommunication
        {
            get
            {
                return this.emailURIEmailCommunicationField;
            }
            set
            {
                this.emailURIEmailCommunicationField = value;
            }
        }

        public WebsiteCommunicationType WebsiteURIWebsiteCommunication
        {
            get
            {
                return this.websiteURIWebsiteCommunicationField;
            }
            set
            {
                this.websiteURIWebsiteCommunicationField = value;
            }
        }

        [XmlElement("SpecifiedUniversalCommunication")]
        public UniversalCommunicationType[] SpecifiedUniversalCommunication
        {
            get
            {
                return this.specifiedUniversalCommunicationField;
            }
            set
            {
                this.specifiedUniversalCommunicationField = value;
            }
        }
    }
}