using System.ComponentModel.Composition.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TechnoLogica.Authentication.Common;
using TechnoLogica.Common;
using TechnoLogica.Mail;

namespace TechnoLogica.IdentityServer
{

    public abstract class BaseStartup
    {
        private const string CORSPolicy = "CORSPolicy";
        public static string CSP_FRAME_ANCESTORS = "";
        public static string CSP_JS_HASH = "";

        public IConfiguration Configuration { get; set; }

        public IWebHostEnvironment Environment { get; }
        public ILoggerFactory LoggerFactory { get; set; }

        protected IdentityServer IdentityServerSettings { get; private set; }

        protected BaseStartup(IWebHostEnvironment environment, IConfiguration config, ILoggerFactory loggerFactory)
        {
            Environment = environment;
            Configuration = config;
            LoggerFactory = loggerFactory;
            IdentityServerSettings = Configuration.GetSettings<IdentityServer>();
        }

        protected abstract IIdentityServerBuilder AddAdditionalConfiguration(IIdentityServerBuilder builder, OperationalStore operationalStore, string connectionString);
        protected abstract void AddAuthenticationProfiles(IServiceCollection services, IConfiguration configuration);

        public virtual void BaseConfigureServices(IServiceCollection services, string connectionString)
        {
            var part = new AssemblyPart(this.GetType().Assembly);

            services.AddSingleton(IdentityServerSettings);

            services.AddControllersWithViews()
                    .ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part))
                    .AddNewtonsoftJson();

            AddDataProtection(services, IdentityServerSettings, connectionString, LoggerFactory);

            //AddDistributedCache(services, compositionContainer);

            AddAuthenticationProviders(services);

            ConfigureCORS(services, IdentityServerSettings.CORS);

            services.AddMvc(options => options.EnableEndpointRouting = false);

            IIdentityServerBuilder builder = services.AddIdentityServer(options =>
            {
                if (IdentityServerSettings.CookieLifetime.HasValue)
                {
                    options.Authentication.CookieLifetime = IdentityServerSettings.CookieLifetime.Value;
                }
                if (IdentityServerSettings.CookieSlidingExpiration.HasValue)
                {
                    options.Authentication.CookieSlidingExpiration = IdentityServerSettings.CookieSlidingExpiration.Value;
                }
            })
             .AddCustomAuthorizeRequestValidator<CustomAuthorizeRequestValidator>()
             .AddProfileService<ProfileService>()
             .AddInMemoryIdentityResources(Configuration.GetSection("IdentityServer:IdentityResources"))
             .AddInMemoryApiScopes(Configuration.GetSection("IdentityServer:ApiScopes"))
             .AddInMemoryApiResources(Configuration.GetSection("IdentityServer:ApiResources"))
             .AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"));

            AddAdditionalConfiguration(builder, Configuration.GetSettings<OperationalStore>(), connectionString);

            AddAuthenticationProfiles(services, Configuration);

            services.AddTransient<UserValidator>();
            services.AddTransient<IMailService, MailService>();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
                CSP_FRAME_ANCESTORS = "http://localhost:4200";
            }
            else
            {
                builder.AddSigningCredential(IdentityServerSettings.SigningCredential.GetX509Certificate2());
                CSP_FRAME_ANCESTORS = IdentityServerSettings.CspFrameAncestors;
            }

            CSP_JS_HASH = IdentityServerSettings.CspJSHash;
        }

        private void AddDistributedCache(IServiceCollection services, CompositionContainer compositionContainer)
        {
            var distributedCacheProvider = compositionContainer.GetExportedValueOrDefault<IDistributionCacheProvider>();
            if (distributedCacheProvider != null)
            {
                distributedCacheProvider.AddDistributedCache(services, Configuration);
            }
        }

        protected virtual void AddDataProtection(IServiceCollection services, IdentityServer identityServerSettings, string connectionString, ILoggerFactory loggerFactory)
        {
            var dataProtectionBuilder = services.AddDataProtection()
                                                .ProtectKeysWithCertificate(identityServerSettings.SigningCredential.GetX509Certificate2());


            AddPersistance(dataProtectionBuilder, connectionString, loggerFactory);
        }

        protected abstract void AddPersistance(IDataProtectionBuilder dataProtectionBuilder, string connectionString, ILoggerFactory loggerFactory);

        protected abstract void AddAuthenticationProviders(IServiceCollection services);

        private void ConfigureCORS(IServiceCollection services, CORSSettings cors)
        {
            if (cors != null && cors.Enabled)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(CORSPolicy, builder =>
                    {
                        builder.WithOrigins(cors.Origins);
                        if (cors.Headers.Length == 1 && cors.Headers[0].Equals("*"))
                        {
                            builder.AllowAnyHeader();
                        }
                        else
                        {
                            builder.WithHeaders(cors.Headers);
                        }
                        if (cors.Methods.Length == 1 && cors.Methods[0].Equals("*"))
                        {
                            builder.AllowAnyMethod();
                        }
                        else
                        {
                            builder.WithHeaders(cors.Methods);
                        }
                    });
                });
            }
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            if (this.Environment.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }

            if (!string.IsNullOrEmpty(this.IdentityServerSettings.MappingPath))
            {
                _ = app.UsePathBase(this.IdentityServerSettings.MappingPath);
            }

            app.UseStaticFiles();

            if (this.IdentityServerSettings.SSLOffloaded)
            {
                app.Use((context, next) =>
                {
                    context.Request.Scheme = "https";
                    return next();
                });
            }
            else
            {
                app.UseHttpsRedirection();
            }

            if (this.IdentityServerSettings.UseForwardHeaders)
            {
                _ = app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            if (this.IdentityServerSettings.CORS != null && this.IdentityServerSettings.CORS.Enabled)
            {
                _ = app.UseCors(CORSPolicy);
            }

            app.UseIdentityServer(new IdentityServerMiddlewareOptions
            {
                AuthenticationMiddleware = (s) => { s. }
            });
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseRouting();
            app.UseEndpoints(configure =>
            {
                _ = configure.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
                _ = configure.MapControllers();
                _ = configure.MapRazorPages();
            });
        }

        private void ConfigureApplication(IApplicationBuilder app)
        {
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseStaticFiles();

            if (IdentityServerSettings.CORS != null && IdentityServerSettings.CORS.Enabled)
            {
                app.UseCors(CORSPolicy);
            }

            app.UseMvcWithDefaultRoute();
        }
    }
}
