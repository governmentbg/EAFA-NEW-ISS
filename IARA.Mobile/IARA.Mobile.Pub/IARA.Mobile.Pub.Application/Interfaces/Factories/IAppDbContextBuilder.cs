using IARA.Mobile.Pub.Application.Interfaces.Database;

namespace IARA.Mobile.Pub.Application.Interfaces.Factories
{
    public interface IAppDbContextBuilder
    {
        bool DatabaseExists { get; }

        /// <summary>
        /// Creates a new DbContext
        /// </summary>
        /// <returns>The DbContext</returns>
        IAppDbContext CreateContext();
    }
}
