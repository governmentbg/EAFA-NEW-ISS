using System;

namespace IARA.Mobile.Application.DTObjects.Common.API
{
    public class PersonDocumentApiDto
    {
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime? DocumentIssuedOn { get; set; }
        public string DocumentIssuedBy { get; set; }
    }
}
