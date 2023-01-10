using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Application.DTObjects.Reports
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ReportType ReportType { get; set; }
        public List<ReportParameterDto> Parameters { get; set; }
    }
}
