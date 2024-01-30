using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using IARA.Mobile.Application.Interfaces.Mappings;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Mobile.Pub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IReportsTransaction, ReportsTransaction>();
            services.AddTransient<IProfileTransaction, ProfileTransaction>();
            services.AddTransient<IStartupTransaction, StartupTransaction>();
            services.AddTransient<ITranslationTransaction, TranslationTransaction>();
            services.AddTransient<ICatchRecordsTransaction, CatchRecordsTransaction>();
            services.AddTransient<INomenclatureTransaction, NomenclatureTransaction>();
            services.AddTransient<IFishingTicketsTransaction, FishingTicketsTransaction>();
            services.AddTransient<IReportViolationTransaction, ReportViolationTransaction>();
            services.AddTransient<IScientificFishingTransaction, ScientificFishingTransaction>();
            services.AddTransient<IApplicationTransaction, ApplicationTransaction>();
            services.AddTransient<IUserTransaction, UserTransaction>();
            services.AddTransient<IPaymentTransaction, PaymentTransaction>();
            services.AddTransient<INewsTransaction, NewsTransaction>();
            services.AddTransient<IAuthenticationTransaction, AuthenticationTransaction>();
            services.AddTransient<IAddressTransaction, AddressTransaction>();
            services.AddTransient<IPasswordTransaction, ProfileTransaction>();

            IMapper mapper = CreateMapper();
            services.AddSingleton((_) => mapper);

            services.AddSingleton<BaseTransactionProvider>();

            return services;
        }

        private static IMapper CreateMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                Type[] exportedTypes = Assembly.GetExecutingAssembly().GetExportedTypes();

                cfg.CreateProfile("IMapFrom", profile =>
                {
                    IEnumerable<Type> types = exportedTypes
                        .Where(t => t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>))
                        );

                    foreach (Type type in types)
                    {
                        object instance = Activator.CreateInstance(type);
                        MethodInfo methodInfo = type.GetMethod("Mapping");
                        methodInfo?.Invoke(instance, new object[] { profile });
                    }
                });

                cfg.CreateProfile("IMapTo", profile =>
                {
                    IEnumerable<Type> types = exportedTypes
                        .Where(t => t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>))
                        );

                    foreach (Type type in types)
                    {
                        object instance = Activator.CreateInstance(type);
                        MethodInfo methodInfo = type.GetMethod("Mapping");
                        methodInfo?.Invoke(instance, new object[] { profile });
                    }
                });
            });

            return config.CreateMapper();
        }
    }
}
