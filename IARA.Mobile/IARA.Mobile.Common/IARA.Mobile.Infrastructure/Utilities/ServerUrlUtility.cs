using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;

namespace IARA.Mobile.Infrastructure.Utilities
{
    public class ServerUrlUtility : IServerUrl
    {
        private readonly IServerUrlFactory _urlFactory;

        public ServerUrlUtility(IServerUrlFactory urlFactory)
        {
            _urlFactory = urlFactory ?? throw new ArgumentNullException(nameof(urlFactory));
        }

        public string Environment { get; set; }

        public Uri BuildUri(string url = null, object parameters = null, string extension = null, string environment = null)
        {
            string requestUrl = _urlFactory.GetUrl(environment ?? Environment)
                + (string.IsNullOrEmpty(extension)
                    ? string.Empty
                    : _urlFactory.GetExtension(extension)
                )
                + (url ?? string.Empty);

            UriBuilder uriBuilder = new UriBuilder(requestUrl)
            {
                Query = FormatToURLParams(parameters)
            };

            return uriBuilder.Uri;
        }

        public string BuildUrl(string url = null, object parameters = null, string extension = null, string environment = null)
        {
            return BuildUri(url, parameters, extension, environment).ToString();
        }

        public Uri GetEnvironmentUri(string name, string environment = null)
        {
            return new Uri(_urlFactory.GetEnvironmentUrl(environment ?? Environment, name));
        }

        public string GetEnvironmentUrl(string name, string environment = null)
        {
            return GetEnvironmentUri(name, environment).ToString();
        }

        public string GetEnvironmentBaseUrl(string environment = null)
        {
            string requestUrl = _urlFactory.GetUrl(environment ?? Environment);
            UriBuilder uriBuilder = new UriBuilder(requestUrl);
            return uriBuilder.Uri.GetLeftPart(System.UriPartial.Authority);
        }

        private string FormatToURLParams(object parameters)
        {
            return string.Join("&", FormatProperties(string.Empty, parameters));
        }

        private IEnumerable<string> FormatProperties(string prefix, object instance)
        {
            if (instance != null)
            {
                foreach (PropertyInfo prop in instance.GetType().GetProperties())
                {
                    object value = prop.GetValue(instance);

                    if (value == null)
                    {
                        continue;
                    }

                    Type propertyType = prop.PropertyType;

                    if (propertyType.IsValueType || value is string)
                    {
                        yield return prefix + prop.Name + '=' + HttpUtility.UrlEncode(GetFormat(value));
                    }
                    else if (value is IEnumerable enumerable)
                    {
                        int i = 0;
                        foreach (object item in enumerable)
                        {
                            foreach (string urlProp in FormatProperties($"{prefix}{prop.Name}[{i}].", item))
                            {
                                yield return urlProp;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        yield return prefix + prop.Name + '=' + HttpUtility.UrlEncode(GetFormat(value));
                    }
                }
            }
        }

        private string GetFormat(object value)
        {
            if (value is DateTime date)
            {
                return date.ToString(CommonConstants.DateTimeFormat);
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
