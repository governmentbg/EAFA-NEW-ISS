namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("FAReportDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class FAReportDocumentType
    {

        private CodeType typeCodeField;

        private CodeType fMCMarkerCodeField;

        private IDType[] relatedReportIDField;

        private DateTimeType acceptanceDateTimeField;

        private DateTimeType transmissionDateTimeField;

        private FLUXReportDocumentType relatedFLUXReportDocumentField;

        private FishingActivityType[] specifiedFishingActivityField;

        private VesselTransportMeansType specifiedVesselTransportMeansField;

        private FLUXCharacteristicType[] specifiedFLUXCharacteristicField;

        public CodeType TypeCode
        {
            get => this.typeCodeField;
            set => this.typeCodeField = value;
        }

        public CodeType FMCMarkerCode
        {
            get => this.fMCMarkerCodeField;
            set => this.fMCMarkerCodeField = value;
        }

        [XmlElement("RelatedReportID")]
        public IDType[] RelatedReportID
        {
            get => this.relatedReportIDField;
            set => this.relatedReportIDField = value;
        }

        public DateTimeType AcceptanceDateTime
        {
            get => this.acceptanceDateTimeField;
            set => this.acceptanceDateTimeField = value;
        }

        public DateTimeType TransmissionDateTime
        {
            get => this.transmissionDateTimeField;
            set => this.transmissionDateTimeField = value;
        }

        public FLUXReportDocumentType RelatedFLUXReportDocument
        {
            get => this.relatedFLUXReportDocumentField;
            set => this.relatedFLUXReportDocumentField = value;
        }

        [XmlElement("SpecifiedFishingActivity")]
        public FishingActivityType[] SpecifiedFishingActivity
        {
            get => this.specifiedFishingActivityField;
            set => this.specifiedFishingActivityField = value;
        }

        public VesselTransportMeansType SpecifiedVesselTransportMeans
        {
            get => this.specifiedVesselTransportMeansField;
            set => this.specifiedVesselTransportMeansField = value;
        }

        [XmlElement("SpecifiedFLUXCharacteristic")]
        public FLUXCharacteristicType[] SpecifiedFLUXCharacteristic
        {
            get => this.specifiedFLUXCharacteristicField;
            set => this.specifiedFLUXCharacteristicField = value;
        }
    }
}