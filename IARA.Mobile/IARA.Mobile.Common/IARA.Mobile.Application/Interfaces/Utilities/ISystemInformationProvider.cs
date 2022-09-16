using System.Collections.Generic;
using System.Threading.Tasks;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface ISystemInformationProvider
    {
        Task<List<string>> Get();
    }
}
