namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("InternetCommunication", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class InternetCommunicationType
    {

        private IDType uRIIDField;

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