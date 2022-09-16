using System;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Mobile.CatchRecords
{
    public class CatchRecordDTO
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string WaterArea { get; set; }
        public DateTime CatchDate { get; set; }
        public LocationDTO Location { get; set; }
        public string Description { get; set; }
    }
}
