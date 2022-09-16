using System;
using IARA.Mobile.Application.DTObjects.Common;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords.API
{
    public class CatchRecordApiDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string WaterArea { get; set; }
        public DateTime CatchDate { get; set; }
        public string Description { get; set; }
        public LocationDto Location { get; set; }
    }
}
