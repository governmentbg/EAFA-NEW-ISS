using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class ReportHistoryDTO
    {
        public int Id { get; set; }

        public string EGN { get; set; }

        public string DocumentsName { get; set; }

        public PageCodeEnum PageCode { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool IsPerson { get; set; }
    }
}
