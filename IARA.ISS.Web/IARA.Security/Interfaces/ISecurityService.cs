using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Security.Enum;
using IARA.Security.SecurityModels;
using Microsoft.AspNetCore.Http;

namespace IARA.Security.Services
{
    public interface ISecurityService
    {
        List<int> GetPermissions(List<int> roleIds);
        SecurityUser GetUserByPersonIdentifier(string egnLnch);
        SecurityUser GetUserByVATNumber(string vatNumber);
        List<int> GetUserLegalRoles(int userId);
        List<int> GetUserRoles(int userId);
        bool IsActive(int userId);
        bool IsActive(SecurityUser user);
        void UpdateUser(SecurityUser user);
        Task<int> LogUserLoginAction(HttpContext context, bool isLogin, bool? isSuccessful = null, string tableId = null, string userName = null);
        SecurityUser GetInternalUser(string identifier, bool searchByPersonId);
        SecurityUser GetUser(int userID);
        SecurityUser GetUser(string userName, bool searchByPersonId);
        void UpdateUserRoles(int userId, LoginTypesEnum loginType, bool isFromLegal = false);
    }
}
