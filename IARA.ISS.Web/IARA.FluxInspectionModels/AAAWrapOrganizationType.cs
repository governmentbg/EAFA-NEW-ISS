using IARA.FluxInspectionModels;

namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAWrapOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAWrapOrganizationType
    {

        private IDType taxRegistrationIDField;

        private CodeType businessTypeCodeField;

        private CodeType legalClassificationCodeField;

        private TextType[] nameField;

        private IDType[] idField;

        private TextType abbreviatedNameField;

        private IDType parentIDField;

        private TextType parentField;

        private CodeType officialLanguageCodeField;

        private OrganizationFunctionTypeCodeType functionCodeField;

        private IDType socialIDField;

        private IDType fiscalIDField;

        private IDType[] otherIDField;

        private AAAAddressType[] postalAAAAddressField;

        private AAAWrapContactType[] primaryAAAWrapContactField;

        private AAAWrapPartyType[] designatedAAAWrapPartyField;

        public IDType TaxRegistrationID
        {
            get
            {
                return this.taxRegistrationIDField;
            }
            set
            {
                this.taxRegistrationIDField = value;
            }
        }

        public CodeType BusinessTypeCode
        {
            get
            {
                return this.businessTypeCodeField;
            }
            set
            {
                this.businessTypeCodeField = value;
            }
        }

        public CodeType LegalClassificationCode
        {
            get
            {
                return this.legalClassificationCodeField;
            }
            set
            {
                this.legalClassificationCodeField = value;
            }
        }

        [XmlElement("Name")]
        public TextType[] Name
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

        public TextType AbbreviatedName
        {
            get
            {
                return this.abbreviatedNameField;
            }
            set
            {
                this.abbreviatedNameField = value;
            }
        }

        public IDType ParentID
        {
            get
            {
                return this.parentIDField;
            }
            set
            {
                this.parentIDField = value;
            }
        }

        public TextType Parent
        {
            get
            {
                return this.parentField;
            }
            set
            {
                this.parentField = value;
            }
        }

        public CodeType OfficialLanguageCode
        {
            get
            {
                return this.officialLanguageCodeField;
            }
            set
            {
                this.officialLanguageCodeField = value;
            }
        }

        public OrganizationFunctionTypeCodeType FunctionCode
        {
            get
            {
                return this.functionCodeField;
            }
            set
            {
                this.functionCodeField = value;
            }
        }

        public IDType SocialID
        {
            get
            {
                return this.socialIDField;
            }
            set
            {
                this.socialIDField = value;
            }
        }

        public IDType FiscalID
        {
            get
            {
                return this.fiscalIDField;
            }
            set
            {
                this.fiscalIDField = value;
            }
        }

        [XmlElement("OtherID")]
        public IDType[] OtherID
        {
            get
            {
                return this.otherIDField;
            }
            set
            {
                this.otherIDField = value;
            }
        }

        [XmlElement("PostalAAAAddress")]
        public AAAAddressType[] PostalAAAAddress
        {
            get
            {
                return this.postalAAAAddressField;
            }
            set
            {
                this.postalAAAAddressField = value;
            }
        }

        [XmlElement("PrimaryAAAWrapContact")]
        public AAAWrapContactType[] PrimaryAAAWrapContact
        {
            get
            {
                return this.primaryAAAWrapContactField;
            }
            set
            {
                this.primaryAAAWrapContactField = value;
            }
        }

        [XmlElement("DesignatedAAAWrapParty")]
        public AAAWrapPartyType[] DesignatedAAAWrapParty
        {
            get
            {
                return this.designatedAAAWrapPartyField;
            }
            set
            {
                this.designatedAAAWrapPartyField = value;
            }
        }
    }
}