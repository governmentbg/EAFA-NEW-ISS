namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFSpecificProduct", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFSpecificProductType
    {

        private IDType idField;

        private IDType barcodeIDField;

        private IDType labelIDField;

        private CodeType categoryTypeCodeField;

        private TextType tradeNameField;

        private TextType labelNameField;

        private TextType[] descriptionField;

        private CodeType temperatureCodeField;

        private CodeType distributionStatusCodeField;

        private CodeType propertyCodeField;

        private RASFFConsignmentType[] specifiedRASFFConsignmentField;

        private RASFFRegulatoryControlType appliedRASFFRegulatoryControlField;

        private RASFFSampledObjectType specifiedRASFFSampledObjectField;

        private RASFFProcessType[] appliedRASFFProcessField;

        private RASFFCharacteristicType volumeApplicableRASFFCharacteristicField;

        private RASFFCharacteristicType weightApplicableRASFFCharacteristicField;

        private RASFFSpecificProductType[] relatedRASFFSpecificProductField;

        private RASFFSpecificProductType[] variantRASFFSpecificProductField;

        public IDType ID
        {
            get => this.idField;
            set => this.idField = value;
        }

        public IDType BarcodeID
        {
            get => this.barcodeIDField;
            set => this.barcodeIDField = value;
        }

        public IDType LabelID
        {
            get => this.labelIDField;
            set => this.labelIDField = value;
        }

        public CodeType CategoryTypeCode
        {
            get => this.categoryTypeCodeField;
            set => this.categoryTypeCodeField = value;
        }

        public TextType TradeName
        {
            get => this.tradeNameField;
            set => this.tradeNameField = value;
        }

        public TextType LabelName
        {
            get => this.labelNameField;
            set => this.labelNameField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        public CodeType TemperatureCode
        {
            get => this.temperatureCodeField;
            set => this.temperatureCodeField = value;
        }

        public CodeType DistributionStatusCode
        {
            get => this.distributionStatusCodeField;
            set => this.distributionStatusCodeField = value;
        }

        public CodeType PropertyCode
        {
            get => this.propertyCodeField;
            set => this.propertyCodeField = value;
        }

        [XmlElement("SpecifiedRASFFConsignment")]
        public RASFFConsignmentType[] SpecifiedRASFFConsignment
        {
            get => this.specifiedRASFFConsignmentField;
            set => this.specifiedRASFFConsignmentField = value;
        }

        public RASFFRegulatoryControlType AppliedRASFFRegulatoryControl
        {
            get => this.appliedRASFFRegulatoryControlField;
            set => this.appliedRASFFRegulatoryControlField = value;
        }

        public RASFFSampledObjectType SpecifiedRASFFSampledObject
        {
            get => this.specifiedRASFFSampledObjectField;
            set => this.specifiedRASFFSampledObjectField = value;
        }

        [XmlElement("AppliedRASFFProcess")]
        public RASFFProcessType[] AppliedRASFFProcess
        {
            get => this.appliedRASFFProcessField;
            set => this.appliedRASFFProcessField = value;
        }

        public RASFFCharacteristicType VolumeApplicableRASFFCharacteristic
        {
            get => this.volumeApplicableRASFFCharacteristicField;
            set => this.volumeApplicableRASFFCharacteristicField = value;
        }

        public RASFFCharacteristicType WeightApplicableRASFFCharacteristic
        {
            get => this.weightApplicableRASFFCharacteristicField;
            set => this.weightApplicableRASFFCharacteristicField = value;
        }

        [XmlElement("RelatedRASFFSpecificProduct")]
        public RASFFSpecificProductType[] RelatedRASFFSpecificProduct
        {
            get => this.relatedRASFFSpecificProductField;
            set => this.relatedRASFFSpecificProductField = value;
        }

        [XmlElement("VariantRASFFSpecificProduct")]
        public RASFFSpecificProductType[] VariantRASFFSpecificProduct
        {
            get => this.variantRASFFSpecificProductField;
            set => this.variantRASFFSpecificProductField = value;
        }
    }
}