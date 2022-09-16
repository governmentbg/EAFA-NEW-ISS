using IARA.Infrastructure.FluxIntegration;
using IARA.Infrastructure.FluxIntegrations;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Infrastructure.FluxIntegrations.QueueServices;
using IARA.Infrastructure.FVMSIntegrations;
using IARA.Interfaces.Flux;
using IARA.Interfaces.Flux.AggregatedCatchReports;
using IARA.Interfaces.Flux.InspectionAndSurveillance;
using IARA.Interfaces.Flux.PermitsAndCertificates;
using IARA.Interfaces.FVMSIntegrations;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.DI
{
    internal static class FluxFVMSInitializer
    {
        public static IServiceCollection AddFVMSServices(this IServiceCollection services)
        {
            services.AddSingleton<FVMSQueueService>();
            services.AddSingleton<IFVMSReceiverIntegrationService, FVMSQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FVMSQueueService>());
            services.AddSingleton<IFVMSInitiatorIntegrationService, FVMSQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FVMSQueueService>());
            services.AddScoped<IPermitsAndLicencesService, PermitsAndLicencesService>();

            return services;
        }

        public static IServiceCollection AddFluxServices(this IServiceCollection services)
        {
            //Vessel Domain
            services.AddSingleton<FluxVesselQueueService>();
            services.AddSingleton<IFluxVesselDomainReceiverService, FluxVesselQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FluxVesselQueueService>());
            services.AddSingleton<IFluxVesselDomainInitiatorService, FluxVesselQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FluxVesselQueueService>());
            services.AddScoped<IVesselsDomainService, VesselsDomainService>();

            //Fishing Activities Domain
            services.AddSingleton<FluxFAQueueService>();
            services.AddSingleton<IFluxFishingActivitiesInitiatorService, FluxFAQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FluxFAQueueService>());
            services.AddSingleton<IFluxFishingActivitiesReceiverService, FluxFAQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FluxFAQueueService>());
            services.AddScoped<IFishingActivitiesDomainService, FishingActivitiesDomainService>();

            //Aggregated Report Domain
            services.AddSingleton<ACDRQueueService>();
            services.AddSingleton<IFluxAggregatedCatchReportInitiatorService, ACDRQueueService>((serviceProvider) => serviceProvider.GetRequiredService<ACDRQueueService>());
            services.AddSingleton<IFluxAggregatedCatchReportReceiverService, ACDRQueueService>((serviceProvider) => serviceProvider.GetRequiredService<ACDRQueueService>());
            services.AddScoped<IAggregatedCatchReportService, AggregatedCatchReportService>();

            //Sales Domain
            services.AddSingleton<FluxSalesQueueService>();
            services.AddSingleton<IFluxSalesDomainInitiatorService, FluxSalesQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FluxSalesQueueService>());
            services.AddSingleton<IFluxSalesDomainReceiverService, FluxSalesQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FluxSalesQueueService>());
            services.AddScoped<ISalesDomainService, SalesDomainService>();

            //Permits Domain
            services.AddSingleton<FluxPermitsQueueService>();
            services.AddSingleton<IFluxPermitsDomainInitiatorService, FluxPermitsQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FluxPermitsQueueService>());
            services.AddSingleton<IFluxPermitsDomainReceiverService, FluxPermitsQueueService>((serviceProvider) => serviceProvider.GetRequiredService<FluxPermitsQueueService>());
            services.AddScoped<IFluxPermitsDomainService, FluxPermitsDomainService>();

            //Inspections Domain
            services.AddScoped<IFluxInspectionsDomainInitiatorService, FluxInspectionsDomainService>();
            services.AddScoped<IFluxInspectionsDomainReceiverService, FluxInspectionsDomainService>();

            //External domains
            services.AddScoped<IFluxFvmsRequestsService, FluxFvmsRequestsService>();

            return services;
        }
    }
}
