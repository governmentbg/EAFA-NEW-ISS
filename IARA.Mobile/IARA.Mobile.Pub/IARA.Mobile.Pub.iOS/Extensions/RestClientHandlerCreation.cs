using Foundation;
using IARA.Mobile.Application.Interfaces.Factories;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.iOS.Extensions
{
    public class RestClientHandlerCreation : IRestClientHandlerBuilder
    {
        public HttpMessageHandler GetHandler()
        {
            return new NSUrlSessionHandler
            {
#if DEBUG
                BypassBackgroundSessionCheck = true,
                TrustOverrideForUrl = (_, __, ___) => true
#endif
            };
        }

        internal class IosHandler : NSUrlSessionHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                try
                {
                    return await base.SendAsync(request, cancellationToken);
                }
                catch (OperationCanceledException ex)
                {
                    throw new WebException(ex.Message, ex, WebExceptionStatus.RequestCanceled, null);
                }
                catch (HttpRequestException ex) when (ex?.InnerException is NSErrorException error)
                {
                    WebExceptionStatus status;

                    switch ((NSUrlError)(int)error.Code)
                    {
                        case NSUrlError.CannotConnectToHost:
                        case NSUrlError.NetworkConnectionLost:
                        case NSUrlError.NotConnectedToInternet:
                            status = WebExceptionStatus.ConnectFailure;
                            break;
                        case NSUrlError.Cancelled:
                            status = WebExceptionStatus.RequestCanceled;
                            break;
                        case NSUrlError.TimedOut:
                            status = WebExceptionStatus.Timeout;
                            break;
                        case NSUrlError.UnsupportedURL:
                            status = WebExceptionStatus.ProtocolError;
                            break;
                        case NSUrlError.SecureConnectionFailed:
                            status = WebExceptionStatus.SecureChannelFailure;
                            break;
                        default:
                            status = WebExceptionStatus.UnknownError;
                            break;
                    }

                    throw new WebException(ex.Message, ex, status, null);
                }
            }
        }
    }
}
