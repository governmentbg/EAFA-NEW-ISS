using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;

namespace IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.API
{
    public class MunicipalityApiDto : NomenclatureDto, IMapTo<NMunicipality>
    {
        public int DistrictId { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<MunicipalityApiDto, NMunicipality>()
                .ForMember(f => f.Id, f => f.MapFrom(s => s.Value))
                .ForMember(f => f.Name, f => f.MapFrom(s => s.DisplayName));
        }
    }
}
