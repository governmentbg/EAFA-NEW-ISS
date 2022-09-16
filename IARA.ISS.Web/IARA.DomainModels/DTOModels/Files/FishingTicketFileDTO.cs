using System;
using Microsoft.AspNetCore.Http;

namespace IARA.DomainModels.DTOModels.Files
{
    public class FishingTicketFileDTO
    {
        public IFormFile File { get; set; }
        public DateTime UploadedOn { get; set; }
    }
}
