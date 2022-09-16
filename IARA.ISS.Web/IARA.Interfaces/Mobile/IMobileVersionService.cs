using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Mobile.Versions;

namespace IARA.Interfaces
{
    public interface IMobileVersionService : IService
    {
        MobileVersionResponseDTO IsAppOutdated(int version, MobileTypeEnum mobileType, string platform);
    }
}
