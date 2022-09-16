using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Mobile.Reports
{
    public class MobileReportNodeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<MobileReportNodeDTO> Children { get; set; }
    }
}
