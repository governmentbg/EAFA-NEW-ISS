namespace IARA.DomainModels.DTOModels.Application
{
    public class PdfDocumentMetadata
    {
        public PdfDocumentMetadata(int applicationId, int applicationHistoryId, string personIdentifier)
        {
            this.PersonIdentifier = personIdentifier;
            this.ApplicationId = applicationId;
            this.ApplicationHistoryId = applicationHistoryId;
        }

        public string PersonIdentifier { get; set; }
        public int ApplicationId { get; set; }
        public int ApplicationHistoryId { get; set; }
    }
}
