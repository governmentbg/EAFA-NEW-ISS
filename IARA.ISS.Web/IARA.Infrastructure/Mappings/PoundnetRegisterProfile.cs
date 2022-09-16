using AutoMapper;
using IARA.DomainModels.DTOModels;
using IARA.EntityModels.Entities;

namespace IARA.Infrastructure.Mappings
{
    public class PoundnetRegisterProfile : Profile
    {
        public PoundnetRegisterProfile()
        {
            this.CreateMap<PoundNetRegister, PoundnetRegisterDTO>().ForMember(x => x.PoundnetCoordinates, m => m.Ignore()).ReverseMap();
            this.CreateMap<PoundNetRegister, PoundNetDTO>().ReverseMap();
        }
    }
}
