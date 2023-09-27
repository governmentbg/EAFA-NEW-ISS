namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TelecommunicationCommunication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TelecommunicationCommunicationType
    {

        private TextType localNumberField;

        private TextType completeNumberField;

        private CodeType countryNumberCodeField;

        private TextType extensionNumberField;

        private CodeType areaNumberCodeField;

        private TextType internalAccessField;

        private CodeType useCodeField;

        private TextType specialDeviceTypeField;

        private SpecifiedPreferenceType usageSpecifiedPreferenceField;

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

        public TextType InternalAccess
        {
            get
            {
                return this.internalAccessField;
            }
            set
            {
                this.internalAccessField = value;
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

        public TextType SpecialDeviceType
        {
            get
            {
                return this.specialDeviceTypeField;
            }
            set
            {
                this.specialDeviceTypeField = value;
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