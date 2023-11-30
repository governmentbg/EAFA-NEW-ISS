using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TechnoLogica.Authentication.Common.Models
{
    public class AuthenticationProviders : IEnumerable<IAuthenticationProvider>
    {
        internal IServiceProvider serviceProvider;
        internal IServiceCollection services;
        private ServiceLifetime serviceLifetime;
        private IConfiguration configuration;

        public AuthenticationProviders(IServiceCollection services, ServiceLifetime serviceLifetime, IConfiguration configuration)
        {
            this.services = services;
            this.configuration = configuration;
            this.serviceLifetime = serviceLifetime;
        }

        public IEnumerator<IAuthenticationProvider> GetEnumerator()
        {
            return AuthenticationProvidersExtensions.AuthenticationProviders
                                                    .Values
                                                    .Select(x => serviceProvider.GetServices(x) as IAuthenticationProvider)
                                                    .GetEnumerator();
        }

        public AuthenticationProviders AddProvider<T>()
            where T : class, IAuthenticationProvider, new()
        {
            AuthenticationProvidersExtensions.AddAuthenticationProvider<T>(this.services, serviceLifetime);
            new T().AddAuthentication(services, configuration);

            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public static class AuthenticationProvidersExtensions
    {
        internal static Dictionary<string, Type> AuthenticationProviders = new Dictionary<string, Type>();

        public static AuthenticationProviders AddAuthenticationProviders(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            var authenticationProviders = new AuthenticationProviders(services, serviceLifetime, configuration);

            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.AddSingleton<AuthenticationProviders>(serviceProvider =>
                        {
                            authenticationProviders.serviceProvider = serviceProvider;
                            return authenticationProviders;
                        });
                    }
                    break;
                case ServiceLifetime.Scoped:
                    {
                        services.AddScoped<AuthenticationProviders>(serviceProvider =>
                        {
                            var providers = new AuthenticationProviders(null, ServiceLifetime.Scoped, configuration);
                            authenticationProviders.serviceProvider = serviceProvider;

                            return providers;
                        });
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        services.AddScoped<AuthenticationProviders>(serviceProvider =>
                        {
                            var providers = new AuthenticationProviders(null, ServiceLifetime.Scoped, configuration);
                            authenticationProviders.serviceProvider = serviceProvider;

                            return providers;
                        });
                    }
                    break;
                default:
                    break;
            }

            return authenticationProviders;
        }

        public static void AddAuthenticationProvider<T>(this IServiceCollection services, ServiceLifetime serviceLifetime)
            where T : class, IAuthenticationProvider
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        services.AddSingleton<T>();
                        AuthenticationProviders.Add(typeof(T).Name, typeof(T));
                    }
                    break;
                case ServiceLifetime.Scoped:
                    {
                        services.AddScoped<T>();
                        AuthenticationProviders.Add(typeof(T).Name, typeof(T));
                    }
                    break;
                case ServiceLifetime.Transient:
                    {
                        services.AddTransient<T>();
                        AuthenticationProviders.Add(typeof(T).Name, typeof(T));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
