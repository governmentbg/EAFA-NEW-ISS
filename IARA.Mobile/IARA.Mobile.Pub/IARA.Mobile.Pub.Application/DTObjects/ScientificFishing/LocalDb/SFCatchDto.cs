using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb
{
    public class SFCatchDto : IMapTo<SFCatch>, IDtoResult
    {
        public int Id { get; set; }
        public int OutingId { get; set; }
        public int FishTypeId { get; set; }
        public NomenclatureDto FishType { get; set; }
        public int CatchUnder100 { get; set; }
        public int Catch100To500 { get; set; }
        public int Catch500To1000 { get; set; }
        public int CatchOver1000 { get; set; }
        public int TotalKeptCount { get; set; }
        public int TotalCatch { get; set; }
        public bool IsActive { get; set; } = true;

        public DtoResultEnum Result { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<SFCatchDto, SFCatch>()
                .ForMember(f => f.FishTypeId, f => f.MapFrom(s => s.FishTypeId));
        }
    }
}
