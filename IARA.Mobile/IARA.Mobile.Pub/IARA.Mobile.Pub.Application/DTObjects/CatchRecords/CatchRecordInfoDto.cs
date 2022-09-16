using System;
using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords
{
    public class CatchRecordInfoDto
    {
        public int Id { get; set; }
        public bool IsOfflineOnly { get; set; }
        public UserTicketShortDto Ticket { get; set; }
        public string WaterArea { get; set; }
        public DateTime CatchDate { get; set; }
        public string Description { get; set; }
        public LocationDto Location { get; set; }
        public List<CatchRecordFishDto> Fishes { get; set; }
        public List<CatchRecordFileDto> Files { get; set; }
    }
}
