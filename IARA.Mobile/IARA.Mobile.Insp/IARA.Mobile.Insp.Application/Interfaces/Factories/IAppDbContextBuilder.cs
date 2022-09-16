using IARA.Mobile.Insp.Application.Interfaces.Database;

namespace IARA.Mobile.Insp.Application.Interfaces.Factories
{
    public interface IAppDbContextBuilder
    {
        /// <summary>
        /// Creates a new <see cref="IAppDbContext"/>.
        /// </summary>
        /// <returns>The <see cref="IAppDbContext"/>.</returns>
        IAppDbContext CreateContext();

        bool DatabaseExists { get; }
    }
}
