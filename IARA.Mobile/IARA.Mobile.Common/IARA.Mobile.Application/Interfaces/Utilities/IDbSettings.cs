namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IDbSettings
    {
        int LastVersion { get; set; }

        void Clear();
    }
}
