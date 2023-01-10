using System;
using System.Collections.Generic;
using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb
{
    public class SFOutingDto : IMapTo<SFOuting>
    {
        public int Id { get; set; }
        public int PermitId { get; set; }
        public DateTime DateOfOuting { get; set; }
        public string WaterArea { get; set; }
        public List<SFCatchDto> Catches { get; set; }
        public bool IsActive { get; set; } = true;

        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<SFOutingDto, SFOuting>();
        }
    }
}
