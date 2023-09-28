namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("UniversalCommunication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class UniversalCommunicationType
    {

        private IDType uRIIDField;

        private CommunicationChannelCodeType channelCodeField;

        private TextType localNumberField;

        private TextType completeNumberField;

        private CodeType countryNumberCodeField;

        private TextType extensionNumberField;

        private CodeType areaNumberCodeField;

        private TextType[] accessField;

        private CodeType useCodeField;

        private IndicatorType hTMLPreferredIndicatorField;

        private SpecifiedPreferenceType usageSpecifiedPreferenceField;

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

        public CommunicationChannelCodeType ChannelCode
        {
            get
            {
                return this.channelCodeField;
            }
            set
            {
                this.channelCodeField = value;
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

        public CodeType AreaNumberCode
        {
            get
            {
                return this.areaNumberCodeField;
            }
            set
            {
                this.areaNumberCodeField = value;
            }
        }

        [XmlElement("Access")]
        public TextType[] Access
        {
            get
            {
                return this.accessField;
            }
            set
            {
                this.accessField = value;
            }
        }

        public CodeType UseCode
        {
            get
            {
                return this.useCodeField;
            }
            set
            {
                this.useCodeField = value;
            }
        }

        public IndicatorType HTMLPreferredIndicator
        {
            get
            {
                return this.hTMLPreferredIndicatorField;
            }
            set
            {
                this.hTMLPreferredIndicatorField = value;
            }
        }

        public SpecifiedPreferenceType UsageSpecifiedPreference
        {
            get
            {
                return this.usageSpecifiedPreferenceField;
            }
            set
            {
                this.usageSpecifiedPreferenceField = value;
            }
        }
    }
}