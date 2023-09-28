namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("TTBatchHistoryRecord", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class TTBatchHistoryRecordType
    {

        private IDType initialBatchIDField;

        private IDType issuerAssignedIDField;

        private IDType originBatchCountryIDField;

        private CodeType statusCodeField;

        private AnimalBatchType[] disassembledRelatedAnimalBatchField;

        private AnimalBatchType[] relatedAnimalBatchField;

        private TTProductBatchType[] disassembledRelatedTTProductBatchField;

        private TTProductBatchType[] relatedTTProductBatchField;

        public IDType InitialBatchID
        {
            get
            {
                return this.initialBatchIDField;
            }
            set
            {
                this.initialBatchIDField = value;
            }
        }

        public IDType IssuerAssignedID
        {
            get
            {
                return this.issuerAssignedIDField;
            }
            set
            {
                this.issuerAssignedIDField = value;
            }
        }

        public IDType OriginBatchCountryID
        {
            get
            {
                return this.originBatchCountryIDField;
            }
            set
            {
                this.originBatchCountryIDField = value;
            }
        }

        public CodeType StatusCode
        {
            get
            {
                return this.statusCodeField;
            }
            set
            {
                this.statusCodeField = value;
            }
        }

        [XmlElement("DisassembledRelatedAnimalBatch")]
        public AnimalBatchType[] DisassembledRelatedAnimalBatch
        {
            get
            {
                return this.disassembledRelatedAnimalBatchField;
            }
            set
            {
                this.disassembledRelatedAnimalBatchField = value;
            }
        }

        [XmlElement("RelatedAnimalBatch")]
        public AnimalBatchType[] RelatedAnimalBatch
        {
            get
            {
                return this.relatedAnimalBatchField;
            }
            set
            {
                this.relatedAnimalBatchField = value;
            }
        }

        [XmlElement("DisassembledRelatedTTProductBatch")]
        public TTProductBatchType[] DisassembledRelatedTTProductBatch
        {
            get
            {
                return this.disassembledRelatedTTProductBatchField;
            }
            set
            {
                this.disassembledRelatedTTProductBatchField = value;
            }
        }

        [XmlElement("RelatedTTProductBatch")]
        public TTProductBatchType[] RelatedTTProductBatch
        {
            get
            {
                return this.relatedTTProductBatchField;
            }
            set
            {
                this.relatedTTProductBatchField = value;
            }
        }
    }
}