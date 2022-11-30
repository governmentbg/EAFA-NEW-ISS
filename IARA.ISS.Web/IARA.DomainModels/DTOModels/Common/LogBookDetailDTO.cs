using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Common
{
    public class LogBookDetailDTO
    {
        public int Id { get; set; }

        public int RegisterId { get; set; }

        public string Number { get; set; }

        public string LogBookTypeName { get; set; }

        public LogBookPagePersonTypesEnum? OwnerType { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string StatusName { get; set; }

        public long? StartPageNumber { get; set; }

        public long? EndPageNumber { get; set; }

        public bool IsOnline { get; set; }

        public bool IsActive { get; set; }
    }
}
