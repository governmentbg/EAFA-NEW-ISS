using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IARA.Mobile.Infrastructure.Utilities
{
    public class RestClientUtility : IRestClient
    {
        private const string AuthenticationScheme = "Bearer";

        private readonly HttpClient _client;

        private readonly IAuthTokenProvider _authTokenProvider;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IFormDataFactory _formDataFactory;
        private readonly IConnectivity _connectivity;
        private readonly IServerUrl _serverUrl;
        private readonly IAuthenticationProvider _authentication;
        private readonly IPopUp _popup;

        public RestClientUtility(IRestClientHandlerBuilder clientHandlerBuilder, IAuthTokenProvider authTokenProvider, IExceptionHandler exceptionHandler, IFormDataFactory formDataFactory, IConnectivity connectivity, IServerUrl serverUrl, IAuthenticationProvider authentication, IPopUp popup)
        {
            _authTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            _formDataFactory = formDataFactory ?? throw new ArgumentNullException(nameof(formDataFactory));
            _connectivity = connectivity ?? throw new ArgumentNullException(nameof(connectivity));
            _serverUrl = serverUrl ?? throw new ArgumentNullException(nameof(serverUrl));
            _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            _popup = popup ?? throw new ArgumentNullException(nameof(popup));

            _client = new HttpClient(clientHandlerBuilder.GetHandler())
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        public Task<HttpResult<TResult>> GetAsync<TResult>(string url, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<TResult>(HttpMethod.Get, url, parameters, alertOnException: alertOnException);
        }

        public Task<HttpResult<TResult>> GetAsync<TResult>(string url, string urlExtension, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<TResult>(HttpMethod.Get, url, parameters, urlExtension: urlExtension, alertOnException: alertOnException);
        }

        public Task<HttpResult> PostAsync(string url, object content, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<Unit>(HttpMethod.Post, url, parameters, true, content, alertOnException: alertOnException)
                .ContinueWith(task => task.Result as HttpResult);
        }

        public Task<HttpResult> PostAsync(string url, object content, string urlExtension, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<Unit>(HttpMethod.Post, url, parameters, true, content, urlExtension: urlExtension, alertOnException: alertOnException)
                .ContinueWith(task => task.Result as HttpResult);
        }

        public Task<HttpResult> PostAsFormDataAsync(string url, object content, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<Unit>(HttpMethod.Post, url, parameters, true, content, asFormData: true, alertOnException: alertOnException)
                .ContinueWith(task => task.Result as HttpResult);
        }

        public Task<HttpResult<TResult>> PostAsFormDataAsync<TResult>(string url, object content, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<TResult>(HttpMethod.Post, url, parameters, true, content, asFormData: true, alertOnException: alertOnException);
        }

        public Task<HttpResult> PutAsFormDataAsync(string url, object content, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<Unit>(HttpMethod.Put, url, parameters, true, content, asFormData: true, alertOnException: alertOnException)
                .ContinueWith(task => task.Result as HttpResult);
        }

        public Task<HttpResult<TResult>> PutAsFormDataAsync<TResult>(string url, object content, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<TResult>(HttpMethod.Put, url, parameters, true, content, asFormData: true, alertOnException: alertOnException);
        }

        public Task<HttpResult<TResult>> PostAsync<TResult>(string url, object content, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<TResult>(HttpMethod.Post, url, parameters, true, content, alertOnException: alertOnException);
        }

        public Task<HttpResult<TResult>> PostAsync<TResult>(string url, object content, string urlExtension, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<TResult>(HttpMethod.Post, url, parameters, true, content, urlExtension: urlExtension, alertOnException: alertOnException);
        }

        public Task<HttpResult> PutAsync(string url, object content, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<Unit>(HttpMethod.Put, url, parameters, true, content, alertOnException: alertOnException)
                .ContinueWith(task => task.Result as HttpResult);
        }

        public Task<HttpResult<TResult>> PutAsync<TResult>(string url, object content, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<TResult>(HttpMethod.Put, url, parameters, true, content, alertOnException: alertOnException);
        }

        public Task<HttpResult> PutAsync(string url, object content, string urlExtension, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<Unit>(HttpMethod.Put, url, parameters, true, content, urlExtension: urlExtension, alertOnException: alertOnException)
                .ContinueWith(task => task.Result as HttpResult);
        }

        public Task<HttpResult> DeleteAsync(string url, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<Unit>(HttpMethod.Delete, url, parameters, alertOnException: alertOnException)
                .ContinueWith(task => task.Result as HttpResult);
        }

        public Task<HttpResult<TResult>> PatchAsync<TResult>(string url, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<TResult>(new HttpMethod("PATCH"), url, parameters, alertOnException: alertOnException);
        }

        public Task<HttpResult> PatchAsync(string url, object parameters = null, bool alertOnException = true)
        {
            return HandleRequest<Unit>(new HttpMethod("PATCH"), url, parameters, alertOnException: alertOnException)
                .ContinueWith(task => task.Result as HttpResult);
        }

        public async Task<HttpResult> HealthCheckAsync()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                Uri requestUri = _serverUrl.BuildUri("health");
                HttpResponseMessage response = await _client.GetAsync(requestUri, source.Token);

                if (response.IsSuccessStatusCode)
                {
                    string text = await response.Content.ReadAsStringAsync();

                    if (text == "Healthy")
                    {
                        return new HttpResult(HttpStatusCode.OK);
                    }
                }

                _connectivity.RunServerHealthChecker();
                return new HttpResult(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _connectivity.RunServerHealthChecker();
                return new HttpResult(HttpStatusCode.BadRequest);
            }
        }

        private async Task<HttpResult<TResult>> HandleRequest<TResult>(HttpMethod method, string url, object parameters, bool hasContent = false, object content = null, bool asFormData = false, string urlExtension = "Services", bool alertOnException = true)
        {
            try
            {
                if (await BeforeSendChecks())
                {
                    return new HttpResult<TResult>(HttpStatusCode.BadRequest);
                }

                HttpContent httpContent;

                if (!hasContent)
                {
                    httpContent = null;
                }
                else if (content == null)
                {
                    httpContent = new StringContent(string.Empty);
                }
                else if (asFormData)
                {
                    httpContent = _formDataFactory.BuildFormData(content);
                }
                else
                {
                    string jsonResult = JsonConvert.SerializeObject(content);
                    Debug.Write($"====JSON=====\n{jsonResult}");
                    httpContent = new StringContent(jsonResult, Encoding.UTF8, "application/json");
                }

                Uri requestUri = _serverUrl.BuildUri(url, parameters, urlExtension);

                HttpResponseMessage response = await _client.SendAsync(new HttpRequestMessage(method, requestUri)
                {
                    Content = httpContent,
                    Headers =
                    {
                        Authorization = new AuthenticationHeaderValue(AuthenticationScheme, _authTokenProvider.Token)
                    }
                });

                httpContent?.Dispose();

                Debug.WriteLine($"{requestUri} | Status Code={response.StatusCode}");

                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                    {
                        response.Dispose();
                        throw new UnauthorizedAccessException();
                    }
                    case (HttpStatusCode)422:
                    {
                        string errorJson = await response.Content.ReadAsStringAsync();
                        response.Dispose();
                        ErrorModel errorResponseContent = JsonConvert.DeserializeObject<ErrorModel>(errorJson);

                        if (alertOnException)
                        {
                            Exception ex = new Exception(errorJson + $"\n{requestUri} Status={response.StatusCode}, in {nameof(RestClientUtility)}.{nameof(HandleRequest)}")
                            {
                                Source = "MOBILE",
                            };
                            await _exceptionHandler.HandleException(ex);
                            _popup.AlertUnsuccessfulRequest();
                        }

                        return new HttpResult<TResult>(response.StatusCode, errorResponseContent);
                    }
                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.BadRequest:
                    {
                        string errorJson = await response.Content.ReadAsStringAsync();
                        response.Dispose();
                        Debug.WriteLine($"ERROR JSON: {errorJson}");
                        if (!string.IsNullOrEmpty(errorJson))
                        {
                            ErrorModel errorResponseContent = JsonConvert.DeserializeObject<ErrorModel>(errorJson);
                            return new HttpResult<TResult>(response.StatusCode, errorResponseContent);
                        }

                        return new HttpResult<TResult>(response.StatusCode);
                    }
                    case HttpStatusCode.RequestEntityTooLarge:
                    {
                        _popup.AlertRequestEntityTooLarge();
                        response.Dispose();
                        return new HttpResult<TResult>(response.StatusCode);
                    }
                    case HttpStatusCode.NoContent:
                    {
                        response.Dispose();
                        return new HttpResult<TResult>(response.StatusCode);
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    response.Dispose();
                    return new HttpResult<TResult>(response.StatusCode);
                }

                Type type = typeof(TResult);

                if (type == typeof(Unit))
                {
                    response.Dispose();
                    return new HttpResult<TResult>(response.StatusCode);
                }
                else if (type == typeof(string))
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    response.Dispose();
                    return new HttpResult<TResult>(response.StatusCode, jsonResponse);
                }
                else if (type == typeof(byte[]))
                {
                    byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                    response.Dispose();
                    return new HttpResult<TResult>(response.StatusCode, (TResult)(bytes as object));
                }
                else if (type == typeof(FileResponse))
                {
                    FileResponse fileResponse = new FileResponse
                    {
                        File = await response.Content.ReadAsByteArrayAsync(),
                        Name = response.Content.Headers.ContentDisposition?.FileName,
                        ContentType = response.Content.Headers.ContentType?.MediaType
                    };
                    response.Dispose();
                    return new HttpResult<TResult>(response.StatusCode, (TResult)(fileResponse as object));
                }

                string json = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"{requestUri} json={json}");
                response.Dispose();
                TResult responseContent = JsonConvert.DeserializeObject<TResult>(json);
                return new HttpResult<TResult>(response.StatusCode, responseContent);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("RestClient - UnauthorizedAccessException");
                _authentication.Dispose(navigateToLogin: true);
                return new HttpResult<TResult>(HttpStatusCode.Unauthorized);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ConnectFailure || ex.Status == WebExceptionStatus.Timeout)
                {
                    _connectivity.RunServerHealthChecker();

                    if (alertOnException)
                    {
                        _popup.AlertException();
                    }

                    return new HttpResult<TResult>(HttpStatusCode.RequestTimeout);
                }
                else
                {
                    await _exceptionHandler.HandleException(ex);

                    if (alertOnException)
                    {
                        _popup.AlertException();
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Type type = ex.GetType();
#endif
                await _exceptionHandler.HandleException(ex);

                if (alertOnException)
                {
                    _popup.AlertException();
                }
            }

            return new HttpResult<TResult>(HttpStatusCode.BadRequest);
        }

        private async Task<bool> BeforeSendChecks()
        {
            if (CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                return true;
            }

            if (_authentication.ShouldRefreshToken())
            {
                await _authentication.RefreshToken();
            }

            return false;
        }
    }
}
