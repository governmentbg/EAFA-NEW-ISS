using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Mobile.CatchRecords
{
    public class CatchRecordFishDTO
    {
        public int Id { get; set; }
        public int CatchRecordId { get; set; }
        public NomenclatureDTO FishType { get; set; }
        public int Count { get; set; }
        public double Quantity { get; set; }
    }
}
