using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.ConfigModels;
using IARA.Common.Constants;
using IARA.DataAccess;
using IARA.DI;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebAPI.Hubs;
using IARA.WebAPI.Hubs.Stats;
using IARA.WebFilters;
using IARA.WebHelpers;
using IARA.WebHelpers.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TL.RequestThrottling;
using TLTTS.Common.ConfigModels;
using SecurityToken = Microsoft.IdentityModel.Tokens.SecurityToken;

[assembly: ApiController()]

namespace IARA.Web
{
    public class Startup
    {
        private const string CORS_POLICY = "AllowSpecificCors";
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ScopedServiceProviderFactory serviceProviderFactory)
        {
            var loggerProvider = serviceProviderFactory.RootServiceProvider.GetService<IDatabaseLoggerProvider>();
            loggerFactory.AddProvider(loggerProvider);

            app.UsePathBase(new PathString(StartupSettings.Default.BasePath));

            app.UseStaticFiles();

            app.UseRateLimiting();
            //app.UseMiddleware<RequestTracingMiddleware>();

            if (env.IsDevelopment() || env.IsStaging())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger(options =>
                {
                    options.SerializeAsV2 = true;
                });

                //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                //// specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.DefaultModelExpandDepth(2);
                    c.EnableDeepLinking();
                    c.EnableFilter();
                    c.EnableTryItOutByDefault();
                    c.EnableValidator();
                    c.ShowCommonExtensions();
                });
            }

            app.UseRouting();

            app.UseRequestLocalization(options =>
            {
                var bgCulture = new CultureInfo("bg-BG");
                bgCulture.NumberFormat.NumberDecimalSeparator = DefaultConstants.DEFAULT_NUMBER_DECIMAL_SEPARATOR;
                bgCulture.NumberFormat.NumberGroupSeparator = DefaultConstants.DEFAULT_NUMBER_GROUP_SEPARATOR;

                var enCulture = new CultureInfo("en-US");
                enCulture.NumberFormat.NumberDecimalSeparator = DefaultConstants.DEFAULT_NUMBER_DECIMAL_SEPARATOR;
                enCulture.NumberFormat.NumberGroupSeparator = DefaultConstants.DEFAULT_NUMBER_GROUP_SEPARATOR;

                options.DefaultRequestCulture = new RequestCulture(bgCulture);
                options.SupportedCultures.Add(bgCulture);
                options.SupportedCultures.Add(enCulture);

                //options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(CORS_POLICY);

            app.UseWebSockets();

            app.UseEndpoints(builder =>
            {
                builder.MapHealthChecks("/health", new HealthCheckOptions
                {
                    AllowCachingResponses = false,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });

                builder.MapControllers();
                builder.MapHub<StatisticsHub>($"/statistics").RequireCors(CORS_POLICY);
                builder.MapHub<NotificationsHub>($"/notifications").AllowAnonymous().RequireAuthorization()
                .RequireCors(CORS_POLICY)
                .Add(builder =>
                {
                    var requestDelegate = builder.RequestDelegate;
                    builder.RequestDelegate = (httpContext) =>
                    {
                        var value = httpContext.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(value))
                        {
                            httpContext.Request.Headers.Add("Authorization", $"Bearer {value}");
                        }

                        return requestDelegate(httpContext);
                    };
                });

                //builder.MapDefaultControllerRoute();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true; //Add this line

            services.AddSettingsModels(configuration);

            services.AddCors(options =>
            {
                options.AddPolicy(CORS_POLICY, builder =>
                {
                    builder.AllowAnyHeader();
                    builder.WithOrigins(CorsSettings.Default.AllowedOrigins);
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                    builder.WithExposedHeaders("Content-Disposition");
                });
            });

            services.AddHealthChecks().AddDbContextCheck<BaseIARADbContext>();
            services.AddSignalR();

            services.AddControllers(configure =>
            {
                configure.AllowEmptyInputInBodyModelBinding = false;
                configure.Filters.Add<ResultExceptionFilter>();

                configure.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
                configure.ModelBinderProviders.Insert(0, new DateModelBinderProvider());
                configure.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "IARA Web API",
                    Description = "IARA ISS Web API",
                    TermsOfService = new Uri("/terms-of-service", UriKind.Relative),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact Person",
                        Email = string.Empty,
                        Url = new Uri("/contacts", UriKind.Relative),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Licence",
                        Url = new Uri("/licence", UriKind.Relative),
                    }
                });

                c.OrderActionsBy(x => x.GroupName);
                c.DocumentFilter<SwaggerFilterOutControllers>(new List<AreaType> { AreaType.Integration });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

#if DEBUG
            ServicePointManager.ServerCertificateValidationCallback += (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
#endif

            services.AddAuthentication("Bearer").AddJwtBearer("Bearer",
                options =>
                {
                    options.Audience = "";
                    options.Authority = JwtBearerSettings.Default.Authorities[0];
                    options.RequireHttpsMetadata = false;
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new CustomJwtSecurityTokenHandler(JwtBearerSettings.Default.Authorities, JwtBearerSettings.Default.Audiences));

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidAudiences = JwtBearerSettings.Default.Audiences,
                        AudienceValidator = new AudienceValidator(ValidateAudience)
                    };

                    if (!JwtBearerSettings.Default.ShouldValidateServerCert)
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = delegate { return true; }
                        };
                    }

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // If the request is for our hub...
                            PathString path = context.HttpContext.Request.Path;

                            if (path.StartsWithSegments("/notifications"))
                            {
                                StringValues accessToken = context.Request.Headers["Authorization"];

                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    string token = accessToken.ToString().Replace("Bearer ", "");

                                    // Read the token out of the query string
                                    context.Token = token;
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };
                });


            services.AddDbContext(ConnectionStrings.Default);
            services.AddHttpContextAccessor();
            services.AddDomainServices();
            services.AddManualDomainServices();
        }

        private static bool ValidateAudience(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            var jwtToken = (JwtSecurityToken)securityToken;
            string clientId = jwtToken.Claims.Where(x => x.Type == "client_id").Select(x => x.Value).FirstOrDefault();

            return JwtBearerSettings.Default.Audiences.Contains(clientId);
        }
    }
}
