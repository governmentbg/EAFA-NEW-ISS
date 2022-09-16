using IARA.Mobile.Domain.Enums;
using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionLogBookDto
    {
        public int? Id { get; set; }
        public CheckTypeEnum? CheckValue { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public string PageNum { get; set; }
        public int? LogBookId { get; set; }
        public int? PageId { get; set; }
        public DateTime? From { get; set; }
        public long? StartPage { get; set; }
        public long? EndPage { get; set; }
    }
}
