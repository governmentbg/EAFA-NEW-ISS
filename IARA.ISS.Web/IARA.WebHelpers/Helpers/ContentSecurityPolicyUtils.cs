using System;
using System.IO;
using System.Text.Encodings.Web;
using IARA.Common.ConfigModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using TL.AspNet.Security.Abstractions.Services;

namespace IARA.WebHelpers.Helpers
{
    public static class ContentSecurityPolicyUtils
    {
        private const string DEFAULT_CONTENT_SECURITY_POLICY = "default-src 'self'; style-src 'self'; script-src 'self';";

        public static IApplicationBuilder UseSecureContentFileServer(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var fileServerOptions = new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(GetContentDirectory(env)),
                RequestPath = "/content",
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = false,
            };

            fileServerOptions.StaticFileOptions.OnPrepareResponse = (context) =>
            {
                if (context.Context.Request.Path.Value.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                {
                    INonceGenerator generator = context.Context.RequestServices.GetService<INonceGenerator>();
                    context.Context.Response.Headers.Add("Content-Security-Policy", GetSecurityPolicyValue(generator.GetNonce()));

                    //TODO - Add nonce to index html file
                }
                else
                {
                    context.Context.Response.Headers.Add("Content-Security-Policy", DEFAULT_CONTENT_SECURITY_POLICY);
                }
            };

            return app.UseFileServer(fileServerOptions);
        }

        private static string GetSecurityPolicyValue(string nonce)
        {
            nonce = JavaScriptEncoder.Default.Encode(nonce);

            return $"default-src 'self'; style-src 'self' '{nonce}'; script-src 'self' '{nonce}';";
        }

        private static string GetContentDirectory(IWebHostEnvironment env)
        {
            var dir = new DirectoryInfo($"{env.ContentRootPath}{StartupSettings.Default.ContentRootPath}");

            if (dir.Exists)
            {
                return dir.FullName;
            }
            else
            {
                return env.ContentRootPath;
            }
        }
    }
}
