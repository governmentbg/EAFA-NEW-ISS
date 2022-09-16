using System;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IServerUrl
    {
        string Environment { get; set; }

        Uri BuildUri(string url = null, object parameters = null, string extension = null, string environment = null);

        string BuildUrl(string url = null, object parameters = null, string extension = null, string environment = null);

        Uri GetEnvironmentUri(string name, string environment = null);

        string GetEnvironmentUrl(string name, string environment = null);

        string GetEnvironmentBaseUrl(string environment = null);
    }
}
