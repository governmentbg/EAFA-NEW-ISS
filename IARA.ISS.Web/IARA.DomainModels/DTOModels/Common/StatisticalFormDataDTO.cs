using System;

namespace IARA.DomainModels.DTOModels.Common
{
    public class StatisticalFormDataDTO
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public DateTime SubmissionDate { get; set; }

        public bool IsActive { get; set; }
    }
}
