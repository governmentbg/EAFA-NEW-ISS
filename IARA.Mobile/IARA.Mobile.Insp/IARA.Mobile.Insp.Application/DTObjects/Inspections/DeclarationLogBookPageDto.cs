using IARA.Mobile.Insp.Domain.Enums;
using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class DeclarationLogBookPageDto
    {
        public int Id { get; set; }
        public string Num { get; set; }
        public DateTime Date { get; set; }
        public LogBookPageStatusesEnum Status { get; set; }
        public int LogBookId { get; set; }
        public string LogBookNumber { get; set; }

        public string LogBookPageOrigin { get; set; }
        public DateTime? LogBookPageOriginDate { get; set; }

        public string DisplayValue => Num + $" ({Status})";
    }
}
