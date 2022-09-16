using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class CommercialFishingLogbookRegisterDTO
    {
        public int Id { get; set; }

        public int LogbookId { get; set; }

        public string Number { get; set; }

        public string LogBookTypeName { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string StatusName { get; set; }

        public long? StartPageNumber { get; set; }

        public long? EndPageNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
