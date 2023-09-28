namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("SPSTradeLineItem", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class SPSTradeLineItemType
    {

        private NumericType sequenceNumericField;

        private TextType[] descriptionField;

        private TextType[] commonNameField;

        private TextType[] scientificNameField;

        private IDType[] productionBatchIDField;

        private TextType[] intendedUseField;

        private DateTimeType[] expiryDateTimeField;

        private MeasureType netWeightMeasureField;

        private MeasureType grossWeightMeasureField;

        private MeasureType netVolumeMeasureField;

        private MeasureType grossVolumeMeasureField;

        private SPSNoteType[] additionalInformationSPSNoteField;

        private SPSClassificationType[] applicableSPSClassificationField;

        private SPSPackageType[] physicalSPSPackageField;

        private SPSTransportEquipmentType[] associatedSPSTransportEquipmentField;

        private SPSCountryType[] originSPSCountryField;

        private SPSLocationType[] originSPSLocationField;

        private SPSProcessType[] appliedSPSProcessField;

        private SPSAuthenticationType[] assertedSPSAuthenticationField;

        private SPSReferencedDocumentType[] referenceSPSReferencedDocumentField;

        public NumericType SequenceNumeric
        {
            get => this.sequenceNumericField;
            set => this.sequenceNumericField = value;
        }

        [XmlElement("Description")]
        public TextType[] Description
        {
            get => this.descriptionField;
            set => this.descriptionField = value;
        }

        [XmlElement("CommonName")]
        public TextType[] CommonName
        {
            get => this.commonNameField;
            set => this.commonNameField = value;
        }

        [XmlElement("ScientificName")]
        public TextType[] ScientificName
        {
            get => this.scientificNameField;
            set => this.scientificNameField = value;
        }

        [XmlElement("ProductionBatchID")]
        public IDType[] ProductionBatchID
        {
            get => this.productionBatchIDField;
            set => this.productionBatchIDField = value;
        }

        [XmlElement("IntendedUse")]
        public TextType[] IntendedUse
        {
            get => this.intendedUseField;
            set => this.intendedUseField = value;
        }

        [XmlElement("ExpiryDateTime")]
        public DateTimeType[] ExpiryDateTime
        {
            get => this.expiryDateTimeField;
            set => this.expiryDateTimeField = value;
        }

        public MeasureType NetWeightMeasure
        {
            get => this.netWeightMeasureField;
            set => this.netWeightMeasureField = value;
        }

        public MeasureType GrossWeightMeasure
        {
            get => this.grossWeightMeasureField;
            set => this.grossWeightMeasureField = value;
        }

        public MeasureType NetVolumeMeasure
        {
            get => this.netVolumeMeasureField;
            set => this.netVolumeMeasureField = value;
        }

        public MeasureType GrossVolumeMeasure
        {
            get => this.grossVolumeMeasureField;
            set => this.grossVolumeMeasureField = value;
        }

        [XmlElement("AdditionalInformationSPSNote")]
        public SPSNoteType[] AdditionalInformationSPSNote
        {
            get => this.additionalInformationSPSNoteField;
            set => this.additionalInformationSPSNoteField = value;
        }

        [XmlElement("ApplicableSPSClassification")]
        public SPSClassificationType[] ApplicableSPSClassification
        {
            get => this.applicableSPSClassificationField;
            set => this.applicableSPSClassificationField = value;
        }

        [XmlElement("PhysicalSPSPackage")]
        public SPSPackageType[] PhysicalSPSPackage
        {
            get => this.physicalSPSPackageField;
            set => this.physicalSPSPackageField = value;
        }

        [XmlElement("AssociatedSPSTransportEquipment")]
        public SPSTransportEquipmentType[] AssociatedSPSTransportEquipment
        {
            get => this.associatedSPSTransportEquipmentField;
            set => this.associatedSPSTransportEquipmentField = value;
        }

        [XmlElement("OriginSPSCountry")]
        public SPSCountryType[] OriginSPSCountry
        {
            get => this.originSPSCountryField;
            set => this.originSPSCountryField = value;
        }

        [XmlElement("OriginSPSLocation")]
        public SPSLocationType[] OriginSPSLocation
        {
            get => this.originSPSLocationField;
            set => this.originSPSLocationField = value;
        }

        [XmlElement("AppliedSPSProcess")]
        public SPSProcessType[] AppliedSPSProcess
        {
            get => this.appliedSPSProcessField;
            set => this.appliedSPSProcessField = value;
        }

        [XmlElement("AssertedSPSAuthentication")]
        public SPSAuthenticationType[] AssertedSPSAuthentication
        {
            get => this.assertedSPSAuthenticationField;
            set => this.assertedSPSAuthenticationField = value;
        }

        [XmlElement("ReferenceSPSReferencedDocument")]
        public SPSReferencedDocumentType[] ReferenceSPSReferencedDocument
        {
            get => this.referenceSPSReferencedDocumentField;
            set => this.referenceSPSReferencedDocumentField = value;
        }
    }
}