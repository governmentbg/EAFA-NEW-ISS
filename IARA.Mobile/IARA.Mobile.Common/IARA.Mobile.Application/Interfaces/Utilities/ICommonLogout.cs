namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface ICommonLogout
    {
        void DeleteLocalInfo(bool changePage = true);

        void SoftDeleteLocalInfo();
    }
}
