using AutoMapper;

namespace IARA.Mobile.Application.Interfaces.Mappings
{
    public interface IMapTo<T>
    {
        void Mapping(Profile profile);
    }
}
