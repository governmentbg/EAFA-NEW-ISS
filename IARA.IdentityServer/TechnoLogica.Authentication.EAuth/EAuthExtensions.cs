using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace TechnoLogica.Authentication.EAuth
{
    public static class EAuthExtensions
    {
        public static AuthenticationBuilder AddEAuth(this AuthenticationBuilder builder)
            => builder.AddEAuth(EAuthDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddEAuth(this AuthenticationBuilder builder, Action<EAuthOptions> configureOptions)
            => builder.AddEAuth(EAuthDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddEAuth(this AuthenticationBuilder builder, string authenticationScheme, Action<EAuthOptions> configureOptions)
            => builder.AddEAuth(authenticationScheme, EAuthDefaults.DisplayName, configureOptions);
        
        public static AuthenticationBuilder AddEAuth(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<EAuthOptions> configureOptions)
        {
            return builder.AddRemoteScheme<EAuthOptions, EAuthHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
