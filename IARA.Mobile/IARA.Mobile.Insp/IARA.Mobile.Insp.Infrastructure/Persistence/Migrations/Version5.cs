using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Infrastructure.Persistence.Migrations.Interfaces;

namespace IARA.Mobile.Insp.Infrastructure.Persistence.Migrations
{
    public class Version5 : IVersion
    {
        private IAuthenticationProvider _authenticationProvider;
        private ICommonLogout _commonLogout;
        public Version5(IAuthenticationProvider authenticationProvider, ICommonLogout commonLogout)
        {
            _authenticationProvider = authenticationProvider;
            _commonLogout = commonLogout;
        }
        public void Migrate(AppDbContext context)
        {
            context.DropTable<PermitLicense>();
            context.CreateTable<PermitLicense>();
            _commonLogout.DeleteLocalInfo(false);
            _authenticationProvider.Logout();
        }
    }
}
