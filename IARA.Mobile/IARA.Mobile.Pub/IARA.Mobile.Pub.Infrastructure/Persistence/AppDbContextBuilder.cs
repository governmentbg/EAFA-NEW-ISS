using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Database;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Pub.Application.Interfaces.Database;
using IARA.Mobile.Pub.Application.Interfaces.Factories;

namespace IARA.Mobile.Pub.Infrastructure.Persistence
{
    public class AppDbContextBuilder : IAppDbContextBuilder, IDbContextBuilder
    {
        public bool DatabaseExists => AppDbMigration.DatabaseExists;

        public AppDbContext CreateContext()
        {
            return new AppDbContext(CommonGlobalVariables.DatabasePath);
        }

        IAppDbContext IAppDbContextBuilder.CreateContext()
        {
            return CreateContext();
        }

        IDbContext IDbContextBuilder.CreateContext()
        {
            return CreateContext();
        }
    }
}
