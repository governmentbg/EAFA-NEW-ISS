using System.Threading.Tasks;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IProfileTransaction
    {
        Task<ProfileApiDto> GetInfo();

        Task<bool> SaveInfo(EditProfileApiDto profile);

        Task<byte[]> GetPhoto();

        Task<FileResponse> GetUserPhoto();
    }
}
