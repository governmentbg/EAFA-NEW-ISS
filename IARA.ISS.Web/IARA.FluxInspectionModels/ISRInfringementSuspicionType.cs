namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ISRInfringementSuspicion", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ISRInfringementSuspicionType
    {

        private CodeType[] typeCodeField;

        private CodeType contextCodeField;

        private TextType legalReferenceField;

        private TextType observationField;

        private IndicatorType seriousViolationIndicatorField;

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

        public CodeType ContextCode
        {
            get
            {
                return this.contextCodeField;
            }
            set
            {
                this.contextCodeField = value;
            }
        }

        public TextType LegalReference
        {
            get
            {
                return this.legalReferenceField;
            }
            set
            {
                this.legalReferenceField = value;
            }
        }

        public TextType Observation
        {
            get
            {
                return this.observationField;
            }
            set
            {
                this.observationField = value;
            }
        }

        public IndicatorType SeriousViolationIndicator
        {
            get
            {
                return this.seriousViolationIndicatorField;
            }
            set
            {
                this.seriousViolationIndicatorField = value;
            }
        }
    }
}