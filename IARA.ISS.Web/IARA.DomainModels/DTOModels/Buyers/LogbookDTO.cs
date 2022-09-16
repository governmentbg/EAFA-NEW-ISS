using System;

namespace IARA.DomainModels.DTOModels.Buyers
{
    public class LogbookDTO
    {
        public int? Id { get; set; }
        public int BuyerId { get; set; }
        public string LogbookNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public int StartPageNum { get; set; }
        public int EndPageNum { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public string Comment { get; set; }
    }
}
