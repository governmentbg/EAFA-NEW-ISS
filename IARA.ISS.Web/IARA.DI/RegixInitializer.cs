using System;
using System.Collections.Generic;
using IARA.Common.ConfigModels;
using IARA.RegixAbstractions.Enums;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixIntegration;
using IARA.RegixIntegration.CheckServices;
using Microsoft.Extensions.DependencyInjection;
using TL.RegiXClient.Base;
using TL.RegiXClient.Base.Services;

namespace IARA.DI
{
    internal static class RegixInitializer
    {
        public static IServiceCollection AddRegixSingletonServices(this IServiceCollection services)
        {
            services.AddSingleton<IRegixConclusionsService, RegixConclusionsService>();
            services.AddSingleton<RegiXClientV1Service>();
            services.AddSingleton<RegiXClientV2Service>();

            services.AddSingleton<IRegiXClientService>((serviceProvider) =>
            {
                ExternalSystemSettings settings = serviceProvider.GetService<ExternalSystemSettings>();

                switch (settings.RegixVersion)
                {
                    case RegixVersions.V1:
                        return serviceProvider.GetService<RegiXClientV1Service>();
                    case RegixVersions.V2:
                        return serviceProvider.GetService<RegiXClientV2Service>();
                    default:
                        throw new NotImplementedException($"Can not create instance of {nameof(IRegiXClientService)}");
                }
            });

            services.AddSingleton<ActualStateCheckService>();
            services.AddSingleton<MockupActualStateCheckService>();
            services.AddSingleton<BaseActualStateCheckService>((serviceProvider) =>
            {
                bool useRegixMockChecks = serviceProvider.GetService<ExternalSystemSettings>().UseRegixMockChecks;

                if (useRegixMockChecks)
                {
                    return serviceProvider.GetRequiredService<MockupActualStateCheckService>();
                }
                else
                {
                    return serviceProvider.GetRequiredService<ActualStateCheckService>();
                }
            });

            services.AddSingleton<ForeignPersonCheckService>();
            services.AddSingleton<MockupForeignPersonCheckService>();
            services.AddSingleton<BaseForeignPersonCheckService>((serviceProvider) =>
            {
                bool useRegixMockChecks = serviceProvider.GetService<ExternalSystemSettings>().UseRegixMockChecks;

                if (useRegixMockChecks)
                {
                    return serviceProvider.GetRequiredService<MockupForeignPersonCheckService>();
                }
                else
                {
                    return serviceProvider.GetRequiredService<ForeignPersonCheckService>();
                }
            });

            services.AddSingleton<LastExpertDecisionCheckService>();
            services.AddSingleton<MockupLastExpertDecisionCheckService>();
            services.AddSingleton<BaseLastExpertDecisionCheckService>((serviceProvider) =>
            {
                bool useRegixMockChecks = serviceProvider.GetService<ExternalSystemSettings>().UseRegixMockChecks;

                if (useRegixMockChecks)
                {
                    return serviceProvider.GetRequiredService<MockupLastExpertDecisionCheckService>();
                }
                else
                {
                    return serviceProvider.GetRequiredService<LastExpertDecisionCheckService>();
                }
            });

            services.AddSingleton<PermanentAddressCheckService>();
            services.AddSingleton<MockupPermanentAddressCheckService>();
            services.AddSingleton<BasePermanentAddressCheckService>((serviceProvider) =>
            {
                bool useRegixMockChecks = serviceProvider.GetService<ExternalSystemSettings>().UseRegixMockChecks;

                if (useRegixMockChecks)
                {
                    return serviceProvider.GetRequiredService<MockupPermanentAddressCheckService>();
                }
                else
                {
                    return serviceProvider.GetRequiredService<PermanentAddressCheckService>();
                }
            });

            services.AddSingleton<PersonDataCheckService>();
            services.AddSingleton<MockupPersonDataCheckService>();
            services.AddSingleton<BasePersonDataCheckService>((serviceProvider) =>
            {
                bool useRegixMockChecks = serviceProvider.GetService<ExternalSystemSettings>().UseRegixMockChecks;

                if (useRegixMockChecks)
                {
                    return serviceProvider.GetRequiredService<MockupPersonDataCheckService>();
                }
                else
                {
                    return serviceProvider.GetRequiredService<PersonDataCheckService>();
                }
            });

            services.AddSingleton<PersonIdentityCheckService>();
            services.AddSingleton<MockupPersonIdentityCheckService>();
            services.AddSingleton<BasePersonIdentityCheckService>((serviceProvider) =>
            {
                bool useRegixMockChecks = serviceProvider.GetService<ExternalSystemSettings>().UseRegixMockChecks;

                if (useRegixMockChecks)
                {
                    return serviceProvider.GetRequiredService<MockupPersonIdentityCheckService>();
                }
                else
                {
                    return serviceProvider.GetRequiredService<PersonIdentityCheckService>();
                }
            });

            services.AddSingleton<RelationsCheckService>();
            services.AddSingleton<MockupRelationsCheckService>();
            services.AddSingleton<BaseRelationsCheckService>((serviceProvider) =>
            {
                bool useRegixMockChecks = serviceProvider.GetService<ExternalSystemSettings>().UseRegixMockChecks;

                if (useRegixMockChecks)
                {
                    return serviceProvider.GetRequiredService<MockupRelationsCheckService>();
                }
                else
                {
                    return serviceProvider.GetRequiredService<RelationsCheckService>();
                }
            });

            services.AddSingleton<VesselCheckService>();
            services.AddSingleton<MockupVesselCheckService>();
            services.AddSingleton<BaseVesselCheckService>((serviceProvider) =>
            {
                bool useRegixMockChecks = serviceProvider.GetService<ExternalSystemSettings>().UseRegixMockChecks;

                if (useRegixMockChecks)
                {
                    return serviceProvider.GetRequiredService<MockupVesselCheckService>();
                }
                else
                {
                    return serviceProvider.GetRequiredService<VesselCheckService>();
                }
            });


            services.AddSingleton<IRegixAdapterService>((serviceProvider) =>
            {
                Dictionary<RegixCheckTypes, object> checkServices = new Dictionary<RegixCheckTypes, object>
                {
                    { RegixCheckTypes.PersonIdentity, serviceProvider.GetService<BasePersonIdentityCheckService>() },
                    { RegixCheckTypes.ForeignPerson, serviceProvider.GetService<BaseForeignPersonCheckService>() },
                    { RegixCheckTypes.LastExpertDecision, serviceProvider.GetService<BaseLastExpertDecisionCheckService>() },
                    { RegixCheckTypes.PermanentAddress, serviceProvider.GetService<BasePermanentAddressCheckService>() },
                    { RegixCheckTypes.PersonData, serviceProvider.GetService<BasePersonDataCheckService>() },
                    { RegixCheckTypes.ActualState, serviceProvider.GetService<BaseActualStateCheckService>() },
                    { RegixCheckTypes.Relations, serviceProvider.GetService<BaseRelationsCheckService>() },
                    { RegixCheckTypes.Vessel, serviceProvider.GetService<BaseVesselCheckService>() }
                };

                IRegiXClientService regiXClientService = serviceProvider.GetRequiredService<IRegiXClientService>();

                return new RegixAdapterService(checkServices, regiXClientService);
            });

            return services;
        }
    }
}
