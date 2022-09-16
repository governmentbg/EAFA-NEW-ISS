using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
namespace IARA.Mobile.Pub.Application.DTObjects.DocumentTypes.LocalDb
{
    public class DocumentTypeSelectDto : IMapFrom<NDocumentType>, ISelectProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<NDocumentType, DocumentTypeSelectDto>();
        }
    }
}
