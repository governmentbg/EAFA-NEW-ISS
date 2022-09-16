using System.Net.Http;

namespace IARA.Mobile.Application.Interfaces.Factories
{
    public interface IFormDataFactory
    {
        HttpContent BuildFormData(object obj);
    }
}
