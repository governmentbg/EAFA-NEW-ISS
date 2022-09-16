using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using System;
using System.Collections.Generic;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.API
{
    public class SFOutingEditApiDto : IMapFrom<SFOutingDto>
    {
        public int Id { get; set; }
        public int PermitId { get; set; }
        public DateTime DateOfOuting { get; set; }
        public string WaterArea { get; set; }
        public List<SFCatchDto> Catches { get; set; }
        public bool IsActive { get; set; } = true;

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<SFOutingDto, SFOutingEditApiDto>();
        }
    }
}
