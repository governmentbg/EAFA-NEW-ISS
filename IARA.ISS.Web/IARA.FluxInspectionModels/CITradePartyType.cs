namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CITradeParty", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CITradePartyType
    {

        private IDType[] idField;

        private IDType[] globalIDField;

        private TextType nameField;

        private PartyRoleCodeType[] roleCodeField;

        private TextType[] descriptionField;

        private IDType registeredIDField;

        private IDType dUNSIDField;

        private CodeType[] typeCodeField;

        private CILegalOrganizationType specifiedCILegalOrganizationField;

        private CITradeContactType[] definedCITradeContactField;

        private CITradeAddressType postalCITradeAddressField;

        private CIUniversalCommunicationType[] uRICIUniversalCommunicationField;

        private CITaxRegistrationType[] specifiedCITaxRegistrationField;

        private CIUniversalCommunicationType endPointURICIUniversalCommunicationField;

        private SpecifiedBinaryFileType[] logoAssociatedSpecifiedBinaryFileField;

        private CILogisticsLocationType specifiedCILogisticsLocationField;

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

        [XmlElement("GlobalID")]
        public IDType[] GlobalID
        {
            get
            {
                return this.globalIDField;
            }
            set
            {
                this.globalIDField = value;
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

        [XmlElement("RoleCode")]
        public PartyRoleCodeType[] RoleCode
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

        public IDType RegisteredID
        {
            get
            {
                return this.registeredIDField;
            }
            set
            {
                this.registeredIDField = value;
            }
        }

        public IDType DUNSID
        {
            get
            {
                return this.dUNSIDField;
            }
            set
            {
                this.dUNSIDField = value;
            }
        }

        [XmlElement("TypeCode")]
        public CodeType[] TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        public CILegalOrganizationType SpecifiedCILegalOrganization
        {
            get
            {
                return this.specifiedCILegalOrganizationField;
            }
            set
            {
                this.specifiedCILegalOrganizationField = value;
            }
        }

        [XmlElement("DefinedCITradeContact")]
        public CITradeContactType[] DefinedCITradeContact
        {
            get
            {
                return this.definedCITradeContactField;
            }
            set
            {
                this.definedCITradeContactField = value;
            }
        }

        public CITradeAddressType PostalCITradeAddress
        {
            get
            {
                return this.postalCITradeAddressField;
            }
            set
            {
                this.postalCITradeAddressField = value;
            }
        }

        [XmlElement("URICIUniversalCommunication")]
        public CIUniversalCommunicationType[] URICIUniversalCommunication
        {
            get
            {
                return this.uRICIUniversalCommunicationField;
            }
            set
            {
                this.uRICIUniversalCommunicationField = value;
            }
        }

        [XmlElement("SpecifiedCITaxRegistration")]
        public CITaxRegistrationType[] SpecifiedCITaxRegistration
        {
            get
            {
                return this.specifiedCITaxRegistrationField;
            }
            set
            {
                this.specifiedCITaxRegistrationField = value;
            }
        }

        public CIUniversalCommunicationType EndPointURICIUniversalCommunication
        {
            get
            {
                return this.endPointURICIUniversalCommunicationField;
            }
            set
            {
                this.endPointURICIUniversalCommunicationField = value;
            }
        }

        [XmlElement("LogoAssociatedSpecifiedBinaryFile")]
        public SpecifiedBinaryFileType[] LogoAssociatedSpecifiedBinaryFile
        {
            get
            {
                return this.logoAssociatedSpecifiedBinaryFileField;
            }
            set
            {
                this.logoAssociatedSpecifiedBinaryFileField = value;
            }
        }

        public CILogisticsLocationType SpecifiedCILogisticsLocation
        {
            get
            {
                return this.specifiedCILogisticsLocationField;
            }
            set
            {
                this.specifiedCILogisticsLocationField = value;
            }
        }
    }
}