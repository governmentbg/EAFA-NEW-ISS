namespace IARA.FluxInspectionModels
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32")]
    [XmlRoot("AnalysisReferencedDocument", Namespace = "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:32", IsNullable = false)]
    public partial class AnalysisReferencedDocumentType
    {

        private IDType issuerAssignedIDField;

        private string issueDateTimeField;

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

        public string IssueDateTime
        {
            get
            {
                return this.issueDateTimeField;
            }
            set
            {
                this.issueDateTimeField = value;
            }
        }
    }
}