using IARA.DomainModels.DTOModels.Common;

namespace IARA.Interfaces
{
    public interface IAddressService
    {
        AddressRegistrationDTO GetAddressRegistration(int id);
    }
}
