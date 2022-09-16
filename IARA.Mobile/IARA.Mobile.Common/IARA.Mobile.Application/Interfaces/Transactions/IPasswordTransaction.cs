using IARA.Mobile.Application.DTObjects.Users;
using System.Threading.Tasks;

namespace IARA.Mobile.Application.Interfaces.Transactions
{
    public interface IPasswordTransaction
    {
        Task<bool> ChangePassword(UserPasswordDto userPassword);
    }
}
