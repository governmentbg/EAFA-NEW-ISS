using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;
using IARA.Mobile.Pub.Domain.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class AuthenticationTransaction : BaseTransaction, IAuthenticationTransaction
    {
        public AuthenticationTransaction(BaseTransactionProvider provider) : base(provider)
        {
        }

        public async Task<JwtToken> LogIn(AuthCredentials authCredentials)
        {
            HttpResult<JwtToken> result = await RestClient.PostAsync<JwtToken>("Security/SignIn","Common", false, authCredentials);
            if(result.IsSuccessful)
            {
                return result.Content;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> LogOut()
        {
            HttpResult result = await RestClient.PostAsync("Security/Logout", null, urlExtension: "Common");

            return result.IsSuccessful;
        }
    }
}
