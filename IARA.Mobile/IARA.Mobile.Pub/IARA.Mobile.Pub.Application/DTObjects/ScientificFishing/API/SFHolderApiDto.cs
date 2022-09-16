using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.API
{
    public class SFHolderApiDto : IMapTo<SFHolder>
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int RequestNumber { get; set; }
        public string Name { get; set; }
        public string ScientificPosition { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<SFHolderApiDto, SFHolder>();
        }
    }
}
