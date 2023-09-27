namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("RASFFConsignment", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class RASFFConsignmentType
    {

        private IDType lotIDField;

        private IDType commonEntryDocumentIDField;

        private MeasureType grossWeightMeasureField;

        private MeasureType netWeightMeasureField;

        private MeasureType grossVolumeMeasureField;

        private MeasureType netVolumeMeasureField;

        private QuantityType consignmentItemQuantityField;

        private RASFFConsignmentItemType includedRASFFConsignmentItemField;

        private RASFFCountryType originRASFFCountryField;

        private RASFFDistributionPartyType relatedRASFFDistributionPartyField;

        public IDType LotID
        {
            get => this.lotIDField;
            set => this.lotIDField = value;
        }

        public IDType CommonEntryDocumentID
        {
            get => this.commonEntryDocumentIDField;
            set => this.commonEntryDocumentIDField = value;
        }

        public MeasureType GrossWeightMeasure
        {
            get => this.grossWeightMeasureField;
            set => this.grossWeightMeasureField = value;
        }

        public MeasureType NetWeightMeasure
        {
            get => this.netWeightMeasureField;
            set => this.netWeightMeasureField = value;
        }

        public MeasureType GrossVolumeMeasure
        {
            get => this.grossVolumeMeasureField;
            set => this.grossVolumeMeasureField = value;
        }

        public MeasureType NetVolumeMeasure
        {
            get => this.netVolumeMeasureField;
            set => this.netVolumeMeasureField = value;
        }

        public QuantityType ConsignmentItemQuantity
        {
            get => this.consignmentItemQuantityField;
            set => this.consignmentItemQuantityField = value;
        }

        public RASFFConsignmentItemType IncludedRASFFConsignmentItem
        {
            get => this.includedRASFFConsignmentItemField;
            set => this.includedRASFFConsignmentItemField = value;
        }

        public RASFFCountryType OriginRASFFCountry
        {
            get => this.originRASFFCountryField;
            set => this.originRASFFCountryField = value;
        }

        public RASFFDistributionPartyType RelatedRASFFDistributionParty
        {
            get => this.relatedRASFFDistributionPartyField;
            set => this.relatedRASFFDistributionPartyField = value;
        }
    }
}