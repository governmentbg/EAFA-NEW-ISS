using System;
using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords
{
    public class CreateCatchRecordDto
    {
        public int? Id { get; set; }
        public string Identifier { get; set; }
        public string WaterArea { get; set; }
        public int? TicketId { get; set; }
        public DateTime? CatchDate { get; set; }
        public LocationDto Location { get; set; }
        public string Description { get; set; }
        public List<CreateCatchRecordFishDto> Fishes { get; set; }
        public List<FileModel> Files { get; set; }
    }
}
