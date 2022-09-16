using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class ReportNodeDTO
    {
        public string Name { get; set; }
        public string IconCode { get; set; }
        public string IconName { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public List<ReportNodeDTO> Children { get; set; }
    }
}
