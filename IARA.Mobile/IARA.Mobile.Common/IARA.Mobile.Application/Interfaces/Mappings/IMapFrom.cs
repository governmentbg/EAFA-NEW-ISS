using AutoMapper;

namespace IARA.Mobile.Application.Interfaces.Mappings
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile);
    }
}
