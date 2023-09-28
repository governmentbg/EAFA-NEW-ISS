namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AgriculturalAnalysis", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AgriculturalAnalysisType
    {

        private DateTimeType samplingDateTimeField;

        private AnalysisReferencedDocumentType resultAnalysisReferencedDocumentField;

        private CropDataSheetPartyType executionCropDataSheetPartyField;

        public DateTimeType SamplingDateTime
        {
            get
            {
                return this.samplingDateTimeField;
            }
            set
            {
                this.samplingDateTimeField = value;
            }
        }

        public AnalysisReferencedDocumentType ResultAnalysisReferencedDocument
        {
            get
            {
                return this.resultAnalysisReferencedDocumentField;
            }
            set
            {
                this.resultAnalysisReferencedDocumentField = value;
            }
        }

        public CropDataSheetPartyType ExecutionCropDataSheetParty
        {
            get
            {
                return this.executionCropDataSheetPartyField;
            }
            set
            {
                this.executionCropDataSheetPartyField = value;
            }
        }
    }
}