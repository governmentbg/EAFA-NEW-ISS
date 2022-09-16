namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IAppDbMigration
    {
        void CheckForMigrations(bool exceptionCaught = false);

        void DropDatabase();
    }
}
