namespace IARA.Mobile.Pub.Infrastructure.Persistence.Migrations.Interfaces
{
    /// <summary>
    /// Check the comment at the top of the AppDbMigration class for more info on how to add versions.
    /// </summary>
    public interface IVersion
    {
        void Migrate(AppDbContext context);
    }
}
