using Android.Net;
using IARA.Mobile.Application.Interfaces.Factories;
using Javax.Net.Ssl;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Android.Net;

namespace IARA.Mobile.Insp.Droid.Extensions
{
    public class RestClientHandlerCreation : IRestClientHandlerBuilder
    {
        public HttpMessageHandler GetHandler()
        {
            return new AndroidHandler();
        }
    }

    internal class AndroidHandler : AndroidClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Java.Net.ConnectException ex)
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.ConnectFailure, null);
            }
            catch (Java.Net.SocketTimeoutException ex)
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.Timeout, null);
            }
            catch (Java.Net.UnknownServiceException ex)
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.ProtocolError, null);
            }
            catch (Java.Lang.SecurityException ex)
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.SecureChannelFailure, null);
            }
            catch (Java.Net.SocketException ex)
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.Timeout, null);
            }
            catch (Java.IO.IOException ex)
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.UnknownError, null);
            }
        }

        protected override SSLSocketFactory ConfigureCustomSSLSocketFactory(HttpsURLConnection connection)
        {
            return SSLCertificateSocketFactory.GetInsecure(1000, null);
        }

        protected override IHostnameVerifier GetSSLHostnameVerifier(HttpsURLConnection connection)
        {
            return new IgnoreSSLHostnameVerifier();
        }

        internal class IgnoreSSLHostnameVerifier : Java.Lang.Object, IHostnameVerifier
        {
            public bool Verify(string hostname, ISSLSession session)
            {
                return true;
            }
        }
    }
}
