using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.API
{
    public class SFPermitReasonApiDto : IMapTo<SFPermitReason>
    {
        public int PermitId { get; set; }
        public int PermitReasonId { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<SFPermitReasonApiDto, SFPermitReason>()
                .ForMember(f => f.NPermitReasonId, f => f.MapFrom(s => s.PermitReasonId));
        }
    }
}
