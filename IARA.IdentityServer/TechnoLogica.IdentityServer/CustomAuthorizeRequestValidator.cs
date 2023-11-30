using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace TechnoLogica.IdentityServer
{
    public class CustomAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
    {
        public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
        {
            var clientId = context.Result.ValidatedRequest.Client?.ClientId;
            var currentClientId = context.Result.ValidatedRequest?.Subject?.Claims.Where(c => c.Type == "ClientID").Select(c => c.Value).FirstOrDefault();

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(currentClientId) &&
                clientId != currentClientId)
            {
                context.Result.IsError = true;
                context.Result.ErrorDescription = $"Logged in user is for different client. Expeccted {currentClientId} got {clientId}";
                context.Result.Error = "ClientId mismatch";
            }

            return Task.CompletedTask;
        }
    }
}
