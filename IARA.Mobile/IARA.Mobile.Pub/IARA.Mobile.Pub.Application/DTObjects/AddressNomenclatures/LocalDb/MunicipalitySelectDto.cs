using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;

namespace IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.LocalDb
{
    public class MunicipalitySelectDto : IMapFrom<NMunicipality>, ISelectProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DistrictId { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<NMunicipality, MunicipalitySelectDto>();
        }
    }
}
