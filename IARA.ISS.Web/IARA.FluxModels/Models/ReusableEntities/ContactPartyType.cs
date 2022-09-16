using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
[Serializable]


    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:20" +
        "")]
    public partial class ContactPartyType
    {

        private IDType[] idField;

        private TextType nameField;

        private TextType[] descriptionField;

        private IDType[] nationalityCountryIDField;

        private CodeType[] languageCodeField;

        private IDType residenceCountryIDField;

        private CodeType[] roleCodeField;

        private StructuredAddressType[] specifiedStructuredAddressField;

        private ContactPersonType[] specifiedContactPersonField;

        private TelecommunicationCommunicationType[] telephoneTelecommunicationCommunicationField;

        private TelecommunicationCommunicationType[] faxTelecommunicationCommunicationField;

        private EmailCommunicationType[] uRIEmailCommunicationField;

        private UniversalCommunicationType[] specifiedUniversalCommunicationField;


        [XmlElement("ID")]
        public IDType[] ID
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


        [XmlElement("NationalityCountryID")]
        public IDType[] NationalityCountryID
        {
            get
            {
                return this.nationalityCountryIDField;
            }
            set
            {
                this.nationalityCountryIDField = value;
            }
        }


        [XmlElement("LanguageCode")]
        public CodeType[] LanguageCode
        {
            get
            {
                return this.languageCodeField;
            }
            set
            {
                this.languageCodeField = value;
            }
        }


        public IDType ResidenceCountryID
        {
            get
            {
                return this.residenceCountryIDField;
            }
            set
            {
                this.residenceCountryIDField = value;
            }
        }


        [XmlElement("RoleCode")]
        public CodeType[] RoleCode
        {
            get
            {
                return this.roleCodeField;
            }
            set
            {
                this.roleCodeField = value;
            }
        }


        [XmlElement("SpecifiedStructuredAddress")]
        public StructuredAddressType[] SpecifiedStructuredAddress
        {
            get
            {
                return this.specifiedStructuredAddressField;
            }
            set
            {
                this.specifiedStructuredAddressField = value;
            }
        }


        [XmlElement("SpecifiedContactPerson")]
        public ContactPersonType[] SpecifiedContactPerson
        {
            get
            {
                return this.specifiedContactPersonField;
            }
            set
            {
                this.specifiedContactPersonField = value;
            }
        }


        [XmlElement("TelephoneTelecommunicationCommunication")]
        public TelecommunicationCommunicationType[] TelephoneTelecommunicationCommunication
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


        [XmlElement("FaxTelecommunicationCommunication")]
        public TelecommunicationCommunicationType[] FaxTelecommunicationCommunication
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


        [XmlElement("URIEmailCommunication")]
        public EmailCommunicationType[] URIEmailCommunication
        {
            get
            {
                return this.uRIEmailCommunicationField;
            }
            set
            {
                this.uRIEmailCommunicationField = value;
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