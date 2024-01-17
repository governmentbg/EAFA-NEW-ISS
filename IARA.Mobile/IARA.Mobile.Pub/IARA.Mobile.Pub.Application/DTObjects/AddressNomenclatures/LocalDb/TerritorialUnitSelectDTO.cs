using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.LocalDb
{
    public class TerritorialUnitSelectDto : IMapFrom<NTerritorialUnit>, ISelectProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public void Mapping(AutoMapper.Profile profile)
        {
            profile.CreateMap<NTerritorialUnit, TerritorialUnitSelectDto>();
        }
    }
}
