using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Domain.Entities.CatchRecords;

namespace IARA.Mobile.Pub.Application.DTObjects.CatchRecords.API
{
    public class CatchRecordFishApiDto : IMapTo<CatchRecordFish>
    {
        public int Id { get; set; }
        public int CatchRecordId { get; set; }
        public NomenclatureDto FishType { get; set; }
        public int Count { get; set; }
        public double Quantity { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<CatchRecordFishApiDto, CatchRecordFish>()
                .ForMember(f => f.FishTypeId, f => f.MapFrom(s => s.FishType.Value));
        }
    }
}
