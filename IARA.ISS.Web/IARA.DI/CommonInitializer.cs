using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using IARA.Common;
using IARA.Common.ConfigModels;
using IARA.Common.Utils;
using IARA.Infrastructure;
using IARA.Infrastructure.FSM;
using IARA.Infrastructure.Services;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers.Hubs.Stats;
using IARA.WebMiddlewares.RequestsTracing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.EDelivery;
using TL.EGovPayments;
using TL.EGovPayments.Interfaces;
using TL.JasperReports.Integration;
using TL.Signer;
using TL.SysToSysSecCom;
using TL.SysToSysSecCom.Interfaces;
using IARA.Infrastructure.RegiX;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixIntegration;
using IARA.Logging;
using IARA.Interfaces.FSM;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Common.TempFileUtils;
using IARA.Notifications;
using IARA.Infrastructure.Services.Payments;
using TL.EPayPayments.Interfaces;
using TL.Email.ExtendedClient;
using TL.Caching.Services;
using TL.Caching.Interfaces;
using TL.Caching.Helpers;
using IARA.Excel.Tools;

#if DEBUG
using IARA.Fakes.InfrastructureStubs;
#endif

namespace IARA.DI
{
    public static class CommonInitializer
    {
        private static object firebaseLock = new object();

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            IEnumerable<Type> assesmblyInterfaceTypes = Assembly
                    .GetAssembly(typeof(IService)).GetTypes().Where(x => x.IsInterface);

            var serviceInterfacePair = Assembly
                        .GetAssembly(typeof(Service))
                        .GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && assesmblyInterfaceTypes.Any(i => i.Name == $"I{t.Name}"))
                        .Select(t => new
                        {
                            Interface = assesmblyInterfaceTypes.Where(x => x.Name == $"I{t.Name}").First(),
                            Implementation = t
                        }).ToList();

            foreach (var item in serviceInterfacePair)
            {
                services.AddScoped(item.Interface, item.Implementation);
            }

            return services;
        }

        public static IServiceCollection AddManualDomainServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddCustomLogging();
            services.AddPayments();
            services.AddBackgroundWorkers();
            services.AddFirebaseMessaging();
            services.AddSingletonServices();
            services.AddFluxServices();
            services.AddFVMSServices();

            services.AddScoped<IPdfSigner, PdfSigner>();

#if DEBUG
            services.AddScoped<IPdfSignatureValidator, MockPdfSignatureValidator>();
#else
            services.AddScoped<IPdfSignatureValidator, PdfSignatureValidator>();
#endif

            services.AddScoped<IEmailClient, EmailClient>(serviceProvider =>
             {
                 EmailClientSettings settings = serviceProvider.GetRequiredService<EmailClientSettings>();
                 IExtendedLogger logger = serviceProvider.GetRequiredService<IExtendedLogger>();
                 return new EmailClient(settings.Host, settings.Port, logger);
             });

            services.AddScoped<ApplicationStateMachine>();
            services.AddScoped<IApplicationStateMachine, ApplicationStateMachine>();
            services.AddScoped<IPermissionsService, PermissionsService>();

            services.AddScoped<IEDeliveryService, EDeliveryService>();
            services.AddScoped<ICryptoHelper, CryptoHelper>();
            services.AddScoped<ISecureHttpClient, SecureHttpClient>();

            services.AddScoped<IRegiXChecksQueueService, RegiXChecksQueueService>();
            services.AddScoped<IRegixApplicationInterfaceService, RegixApplicationInterfaceService>();

            services.AddScoped<IJasperReportsClient, JasperReportsClient>();

            services.AddScoped<IScopedServiceProvider>(serviceProvider =>
            {
                return serviceProvider.GetRequiredService<ScopedServiceProviderFactory>().GetServiceProvider();
            });

            var mapperConfig = new MapperConfiguration(configuration =>
            {
                configuration.AddMaps(typeof(Service).Assembly);
            });

            services.AddScoped<IMapper>(serviceProvider =>
            {
                return mapperConfig.CreateMapper();
            });

            return services;
        }

        private static void AddFirebaseMessaging(this IServiceCollection services)
        {
            services.AddSingleton<FirebaseMessaging>(serviceProvider =>
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    lock (firebaseLock)
                    {
                        if (FirebaseApp.DefaultInstance == null)
                        {
                            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                            FirebaseApp.Create(new AppOptions
                            {
                                Credential = GoogleCredential.FromJson(configuration["FirebaseCredentials"])
                            });
                        }
                    }
                }

                return FirebaseMessaging.DefaultInstance;
            });
        }

        private static void AddPayments(this IServiceCollection services)
        {
            //e-Gov Payments
            services.AddScoped<IEGovPaymentService, EGovPaymentService>();
            services.AddScoped<IEGovIntegrationService, EGovPaymentsService>();

            //e-Pay Payments
            services.AddScoped<IEPayCallbackService, EPayPaymentsService>();
            services.AddScoped<IEPayPaymentDataService, EPayPaymentsService>();

            //Common Payments Service
            services.AddScoped<IPaymentsService, EPaymentsService>();
        }


        private static IServiceCollection AddSingletonServices(this IServiceCollection services)
        {
            services.AddSingleton<DatabaseActivityLogger>();
            services.AddSingleton<InMemoryStatisticsLogger>();

            //services.AddMemoryCaching();
            services.AddMultiServerMemoryCaching();

            services.AddSingleton<StastisticsWorker>();
            services.AddSingleton<PermissionsCache>();

            services.AddSingleton<SystemPropertiesService>();
            services.AddSingleton<ISecurityEmailSender, SecurityEmailSender>();

            services.AddExcelExporter();
            services.AddSingleton<ScopedServiceProviderFactory>();
            services.AddSingleton<IFilesSweeper, FilesSweeper>();

            services.AddRegixSingletonServices();
            services.AddSignalRNotifications();
            services.AddFirebaseMessaging();
            services.AddEmailQueueSender();

            return services;
        }


    }
}
