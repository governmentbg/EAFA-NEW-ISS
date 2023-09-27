namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("CIProductClassification", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class CIProductClassificationType
    {

        private IDType systemIDField;

        private TextType[] systemNameField;

        private CodeType classCodeField;

        private TextType[] classNameField;

        private CodeType subClassCodeField;

        private CIProductCharacteristicType[] classCIProductCharacteristicField;

        private ReferencedStandardType applicableReferencedStandardField;

        public IDType SystemID
        {
            get
            {
                return this.systemIDField;
            }
            set
            {
                this.systemIDField = value;
            }
        }

        [XmlElement("SystemName")]
        public TextType[] SystemName
        {
            get
            {
                return this.systemNameField;
            }
            set
            {
                this.systemNameField = value;
            }
        }

        public CodeType ClassCode
        {
            get
            {
                return this.classCodeField;
            }
            set
            {
                this.classCodeField = value;
            }
        }

        [XmlElement("ClassName")]
        public TextType[] ClassName
        {
            get
            {
                return this.classNameField;
            }
            set
            {
                this.classNameField = value;
            }
        }

        public CodeType SubClassCode
        {
            get
            {
                return this.subClassCodeField;
            }
            set
            {
                this.subClassCodeField = value;
            }
        }

        [XmlElement("ClassCIProductCharacteristic")]
        public CIProductCharacteristicType[] ClassCIProductCharacteristic
        {
            get
            {
                return this.classCIProductCharacteristicField;
            }
            set
            {
                this.classCIProductCharacteristicField = value;
            }
        }

        public ReferencedStandardType ApplicableReferencedStandard
        {
            get
            {
                return this.applicableReferencedStandardField;
            }
            set
            {
                this.applicableReferencedStandardField = value;
            }
        }
    }
}