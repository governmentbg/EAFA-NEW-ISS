namespace IARA.Mobile.Insp.Infrastructure.Persistence.Migrations.Interfaces
{
    /// <summary>
    /// Check the comment at the top of the AppDbMigration class for more info on how to add versions.
    /// </summary>
    internal interface IVersion
    {
        void Migrate(AppDbContext context);
    }
}
