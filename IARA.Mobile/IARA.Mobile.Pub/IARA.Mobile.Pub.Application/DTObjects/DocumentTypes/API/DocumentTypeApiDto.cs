using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;

namespace IARA.Mobile.Pub.Application.DTObjects.DocumentTypes.API
{
    public class DocumentTypeApiDto : NomenclatureDto, IMapTo<NDocumentType>
    {
        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<DocumentTypeApiDto, NDocumentType>()
                .ForMember(f => f.Id, f => f.MapFrom(s => s.Value))
                .ForMember(f => f.Name, f => f.MapFrom(s => s.DisplayName));
        }
    }
}
