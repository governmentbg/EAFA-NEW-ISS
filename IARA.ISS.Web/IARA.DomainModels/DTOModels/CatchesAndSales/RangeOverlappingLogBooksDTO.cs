using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class RangeOverlappingLogBooksDTO
    {
        public long StartPage { get; set; }

        public long EndPage { get; set; }

        public string LogBookNumber { get; set; }

        public List<OverlappingLogBookDTO> OverlappingLogBooks { get; set; }
    }
}
