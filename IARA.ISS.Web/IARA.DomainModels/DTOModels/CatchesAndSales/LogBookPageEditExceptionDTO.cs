using IARA.Common.Resources;
using System.ComponentModel.DataAnnotations;
using System;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPageEditExceptionDTO
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? LogBookTypeId { get; set; }

        public int? LogBookId { get; set; }

        public DateTime ExceptionActiveFrom { get; set; }

        public DateTime ExceptionActiveTo { get; set; }

        public DateTime EditPageFrom { get; set; }

        public DateTime EditPageTo { get; set; }
    }
}
