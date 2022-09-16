using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Mobile.CatchRecords
{
    public class MobileCatchRecordGroupDTO
    {
        public List<CatchRecordDTO> CatchRecords { get; set; }
        public List<CatchRecordFishDTO> Fishes { get; set; }
        public List<CatchRecordTicketDTO> Tickets { get; set; }
        public List<CatchRecordFileDTO> Files { get; set; }
    }
}
