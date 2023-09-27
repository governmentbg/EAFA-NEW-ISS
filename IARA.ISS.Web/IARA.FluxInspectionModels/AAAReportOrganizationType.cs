using IARA.FluxInspectionModels;

namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AAAReportOrganization", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AAAReportOrganizationType
    {

        private IDType taxRegistrationIDField;

        private CodeType businessTypeCodeField;

        private CodeType legalClassificationCodeField;

        private TextType[] nameField;

        private IDType[] idField;

        private TextType abbreviatedNameField;

        private OrganizationFunctionTypeCodeType functionCodeField;

        private IDType socialIDField;

        private IDType fiscalIDField;

        private IDType[] otherIDField;

        private AAAAddressType postalAAAAddressField;

        private AAAReportPersonType primaryAAAReportPersonField;

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

        public AAAAddressType PostalAAAAddress
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

        public AAAReportPersonType PrimaryAAAReportPerson
        {
            get
            {
                return this.primaryAAAReportPersonField;
            }
            set
            {
                this.primaryAAAReportPersonField = value;
            }
        }
    }
}