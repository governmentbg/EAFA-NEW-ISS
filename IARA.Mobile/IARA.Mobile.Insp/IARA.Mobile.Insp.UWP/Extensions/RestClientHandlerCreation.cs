using IARA.Mobile.Application.Interfaces.Factories;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.UWP.Extensions
{
    public class RestClientHandlerCreation : IRestClientHandlerBuilder
    {
        public HttpMessageHandler GetHandler()
        {
            return new UWPHandler();
        }
    }

    internal class UWPHandler : HttpClientHandler
    {
        public UWPHandler()
        {
            ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException ex) when (ex.Message == "An error occurred while sending the request.")
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.ConnectFailure, null);
            }
            catch (TaskCanceledException ex)
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.Timeout, null);
            }
        }
    }
}
