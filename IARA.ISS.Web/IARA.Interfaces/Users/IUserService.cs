using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.User;

namespace IARA.Interfaces
{
    public interface IUserService : IService
    {
        bool ConfirmEmailToken(string token);

        UserAuthDTO GetUserAuthInfo(int userId);

        UserAuthDTO GetUserAuthInfo(string identifier);

        string GetUserPhoto(int userId);

        bool ConfirmUserEmailAndPassword(int userID, UserLoginDTO user);

        void UpdateUserEAuthData(UserRegistrationDTO user);

        int GetUserPersonId(int userId);

        int? GetPersonIdByEgnLnc(string egnLnc, IdentifierTypeEnum idType);

        List<int> GetPersonIdsByUserId(int userId);

        List<int> GetApprovedLegalIdsByUserId(int userId);

        int AddExternalUser(UserRegistrationDTO user);

        int UpdateUserRegistrationInfo(UserRegistrationDTO user);

        void DeactivateUserPasswordAccount(string egnLnch);

        string SendConfirmationEmail(string email);

        void UpdateUsersActivity(IDictionary<string, DateTime> users);

        void ChangeUserPassword(UserChangePasswordDTO data);

        List<int> GetUserIdsInSameTerritoryUnitAsUser(int userId);

        int? GetUserTerritoryUnitId(int userId);

        AccountActivationStatusesEnum ActivateUserAccount(UserTokenDTO userTokenInfo);

        string GetUserEmailByConfirmationKey(string token);
        int TryUnlockUsers();
        bool UpdateUserInfo(ChangeUserDataDTO userData);
        ChangeUserDataDTO GetAllUserData(int userID);
    }
}
