using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class TableNodeDTO
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<TableNodeDTO> Children { get; set; }
    }
}
