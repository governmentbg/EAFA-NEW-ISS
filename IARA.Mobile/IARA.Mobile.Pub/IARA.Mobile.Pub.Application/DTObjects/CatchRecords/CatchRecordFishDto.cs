using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords
{
    public class CatchRecordFishDto
    {
        public int Id { get; set; }
        public int RecordCatchId { get; set; }
        public NomenclatureDto FishType { get; set; }
        public int Count { get; set; }
        public double Quantity { get; set; }
    }
}
