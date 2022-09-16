using System.Net.Http;

namespace IARA.Mobile.Application.Interfaces.Factories
{
    public interface IRestClientHandlerBuilder
    {
        /// <summary>
        /// In production it allow you to get a faster <see cref="HttpClient"/>
        /// In debugging it allows you to access sites which don't have a certificate
        /// </summary>
        HttpMessageHandler GetHandler();
    }
}
