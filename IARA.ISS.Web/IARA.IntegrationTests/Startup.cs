using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using IARA.Common.Logging;
using IARA.DataAccess;
using IARA.DI;
using IARA.Fakes.InfrastructureStubs;
using IARA.Infrastructure;
using IARA.WebHelpers.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Web.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDomainServices();
            //services.AddSettingsModels(configuration, out _);
            services.AddScoped<IExtendedLogger, MockupLogger>();
            services.AddSingleton<IUserActionsAuditLogger, MockupUserActionsLogger>();

            var mapperConfig = new MapperConfiguration(configuration =>
            {
                configuration.AddMaps(typeof(Service).Assembly);
            });

            services.AddTransient<IMapper>(serviceProvider =>
            {
                return mapperConfig.CreateMapper();
            });

            services.AddDbContext<BaseIARADbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            }, ServiceLifetime.Scoped, ServiceLifetime.Transient);

            services.AddScoped<IARADbContext>();

            var controllerTypes = Assembly
                                  .GetAssembly(typeof(BaseController))
                                  .GetTypes()
                                  .Where(x => x.IsClass
                                         && !x.IsAbstract
                                         && x.BaseType != null
                                         && x.BaseType == typeof(BaseController))
                                  .ToList();

            foreach (var type in controllerTypes)
            {
                services.AddScoped(type);
            }
        }
    }
}
