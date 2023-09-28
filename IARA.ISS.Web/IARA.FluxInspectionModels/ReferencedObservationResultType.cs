namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("ReferencedObservationResult", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class ReferencedObservationResultType
    {

        private IDType[] lORExchangedDocumentIDField;

        private IDType[] sampleIDField;

        [XmlElement("LORExchangedDocumentID")]
        public IDType[] LORExchangedDocumentID
        {
            get
            {
                return this.lORExchangedDocumentIDField;
            }
            set
            {
                this.lORExchangedDocumentIDField = value;
            }
        }

        [XmlElement("SampleID")]
        public IDType[] SampleID
        {
            get
            {
                return this.sampleIDField;
            }
            set
            {
                this.sampleIDField = value;
            }
        }
    }
}