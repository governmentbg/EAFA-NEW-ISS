using System.Collections.Generic;

namespace IARA.Mobile.Application.DTObjects.Reports
{
    public class ReportNodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<ReportNodeDto> Children { get; set; }
    }
}
