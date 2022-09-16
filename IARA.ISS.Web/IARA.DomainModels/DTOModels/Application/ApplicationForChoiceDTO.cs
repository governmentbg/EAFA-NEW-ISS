using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationForChoiceDTO
    {
        public int Id { get; set; }

        public PageCodeEnum PageCode { get; set; }

        public string EventisNumber { get; set; }

        public DateTime SubmitDateTime { get; set; }

        public string SubmittedBy { get; set; }

        public string SubmittedFor { get; set; }

        public string Type { get; set; }

        public bool IsChecked { get; set; }
    }
}
