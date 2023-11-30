using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnoLogica.Authentication.Common;

namespace TechnoLogica.IdentityServer
{
    public class ProfileService : IProfileService
    {
        private ILogger<ProfileService> Logger { get; set; }
        IEnumerable<IProfileClientService> ProfileClientServices { get; set; }

        public ProfileService(IEnumerable<IProfileClientService> profileClientServices,
            ILogger<ProfileService> logger)
        {
            Logger = logger;
            ProfileClientServices = profileClientServices;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            Logger.LogInformation($"Getting claims for user: {context?.Subject?.Identity?.Name} in client: {context?.Client?.ClientId}");
            var profileService = ProfileClientServices.Where(cs => cs.ClientId.Equals(context?.Client?.ClientId)).FirstOrDefault();
            profileService?.GetProfileDataAsync(context);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var profileService = ProfileClientServices.Where(cs => cs.ClientId.Equals(context?.Client?.ClientId)).FirstOrDefault();
            profileService?.IsActiveAsync(context);
        }
    }
}
