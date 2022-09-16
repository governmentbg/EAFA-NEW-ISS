using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.User;

namespace IARA.Interfaces
{
    public interface IMyProfileService : IService
    {
        MyProfileDTO GetUserProfile(int userId);

        void UpdateUserProfile(MyProfileDTO userProfile, int userId);

        void ChangeUserPassword(int userId, UserPasswordDTO userData);

        bool HasUserEgnLnc(EgnLncDTO egnLnc, int userId);
    }
}
