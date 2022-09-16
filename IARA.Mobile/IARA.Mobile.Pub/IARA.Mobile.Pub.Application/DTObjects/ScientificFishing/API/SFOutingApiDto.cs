using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;
using System;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.API
{
    public class SFOutingApiDto : IMapTo<SFOuting>
    {
        public int Id { get; set; }
        public int PermitId { get; set; }
        public DateTime DateOfOuting { get; set; }
        public string WaterArea { get; set; }
        public bool IsActive { get; set; }

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<SFOutingApiDto, SFOuting>();
        }
    }
}
