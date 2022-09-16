using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords
{
    public class CreateCatchRecordFishDto : IDtoBaseResult
    {
        public int? Id { get; set; }
        public int? CatchRecordId { get; set; }
        public int FishTypeId { get; set; }
        public int Count { get; set; }
        public double Quantity { get; set; }
    }
}
