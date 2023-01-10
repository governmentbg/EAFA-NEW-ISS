using System.Threading.Tasks;
using IARA.Mobile.Application.DTObjects.Users;

namespace IARA.Mobile.Application.Interfaces.Transactions
{
    public interface IPasswordTransaction
    {
        Task<bool> ChangePassword(UserPasswordDto userPassword);
    }
}
