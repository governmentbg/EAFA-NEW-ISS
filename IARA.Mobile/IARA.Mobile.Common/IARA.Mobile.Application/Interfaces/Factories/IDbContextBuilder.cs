using IARA.Mobile.Application.Interfaces.Database;

namespace IARA.Mobile.Application.Interfaces.Factories
{
    public interface IDbContextBuilder
    {
        /// <summary>
        /// Creates a new DbContext
        /// </summary>
        /// <returns>The DbContext</returns>
        IDbContext CreateContext();

        bool DatabaseExists { get; }
    }
}
