using IARA.Mobile.Application.DTObjects.Profile.API;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.Application.Interfaces.Transactions
{
    public interface IProfileTransaction
    {
        Task<ProfileApiDto> GetInfo();

        Task<bool> SaveInfo(EditProfileApiDto profile);

        Task<byte[]> GetPhoto();
    }
}
