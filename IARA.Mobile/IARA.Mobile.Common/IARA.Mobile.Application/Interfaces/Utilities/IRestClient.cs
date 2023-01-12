using System.Threading.Tasks;
using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IRestClient
    {
        /// <summary>
        /// Receive data from the IARA API using GET protocol
        /// </summary>
        /// <typeparam name="TResult">The class used for which the json will be serialized to</typeparam>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns the json serialized into <typeparamref name="TResult"/></returns>
        Task<HttpResult<TResult>> GetAsync<TResult>(string url, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Receive data from the IARA API using GET protocol
        /// </summary>
        /// <typeparam name="TResult">The class used for which the json will be serialized to</typeparam>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="urlExtension">Extension to be added after the base url</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns the json serialized into <typeparamref name="TResult"/></returns>
        Task<HttpResult<TResult>> GetAsync<TResult>(string url, string urlExtension, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send data to the IARA API using POST protocol
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns weather or not the request performed successfully</returns>
        Task<HttpResult> PostAsync(string url, object content, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send data to the IARA API using POST protocol
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns weather or not the request performed successfully</returns>
        Task<HttpResult> PostAsync(string url, object content, string urlExtension, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send and then receive data to the IARA API using POST protocol
        /// </summary>
        /// <typeparam name="TResult">The class used for which the json will be serialized to</typeparam>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns the json serialized into <typeparamref name="TResult"/></returns>
        Task<HttpResult<TResult>> PostAsync<TResult>(string url, object content, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send and then receive data to the IARA API using POST protocol
        /// </summary>
        /// <typeparam name="TResult">The class used for which the json will be serialized to</typeparam>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="urlExtension">Extension to be added after the base url</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns the json serialized into <typeparamref name="TResult"/></returns>
        Task<HttpResult<TResult>> PostAsync<TResult>(string url, object content, string urlExtension, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send and then receive data to the IARA API using POST protocol
        /// Mainly used to send files
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        Task<HttpResult> PostAsFormDataAsync(string url, object content, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send and then receive data to the IARA API using POST protocol
        /// Mainly used to send files
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        Task<HttpResult<TResult>> PostAsFormDataAsync<TResult>(string url, object content, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send and then receive data to the IARA API using PUT protocol
        /// Mainly used to send files
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        Task<HttpResult> PutAsFormDataAsync(string url, object content, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send and then receive data to the IARA API using PUT protocol
        /// Mainly used to send files
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        Task<HttpResult<TResult>> PutAsFormDataAsync<TResult>(string url, object content, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send data to the IARA API using PUT protocol
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns weather or not the request performed successfully</returns>
        Task<HttpResult> PutAsync(string url, object content, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send and then receive data to the IARA API using PUT protocol
        /// </summary>
        /// <typeparam name="TResult">The class used for which the json will be serialized to</typeparam>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns the json serialized into <typeparamref name="TResult"/></returns>
        Task<HttpResult<TResult>> PutAsync<TResult>(string url, object content, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send and then receive data to the IARA API using POST protocol
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="urlExtension">Extension to be added after the base url</param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns the json serialized into <typeparamref name="TResult"/></returns>
        Task<HttpResult> PutAsync(string url, object content, string urlExtension, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send a delete request to the IARA API using DELETE protocol
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns weather or not the request performed successfully</returns>
        Task<HttpResult> DeleteAsync(string url, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send a restore request to the IARA API using PATCH protocol
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns weather or not the request performed successfully</returns>
        Task<HttpResult> PatchAsync(string url, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Send a restore request to the IARA API using PATCH protocol
        /// </summary>
        /// <param name="url">The URL to the API using format '{Controller}/{Action}?{Parameters}'</param>
        /// <param name="parameters">a collection of parameters that are then URL encoded and formatted into the <paramref name="url"/></param>
        /// <param name="alertOnException">If <see langword="false"/> then it will not display alert dialog on exception.</param>
        /// <returns>Returns the json serialized into <typeparamref name="TResult"/></returns>
        Task<HttpResult<TResult>> PatchAsync<TResult>(string url, object parameters = null, bool alertOnException = true);

        /// <summary>
        /// Get health status from the IARA API
        /// </summary>
        /// <returns>Returns Okay status if the server is healthy</returns>
        Task<HttpResult> HealthCheckAsync();
    }
}
