using System.Collections.Generic;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords.API
{
    public class CatchRecordGroupApiDto
    {
        public List<CatchRecordApiDto> CatchRecords { get; set; }
        public List<CatchRecordFishApiDto> Fishes { get; set; }
        public List<CatchRecordTicketApiDto> Tickets { get; set; }
        public List<CatchRecordFileApiDto> Files { get; set; }
    }
}
