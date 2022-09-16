using System;
using AutoMapper;
using IARA.Common.ConfigModels;
using IARA.DataAccess;
using IARA.DI;
using IARA.Fakes.InfrastructureStubs;
using IARA.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Infrastructure.UnitTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDomainServices();
            ServicesInitialization.Initialize(services);
            services.AddSingleton<ISecurityEmailSender, MockupSecurityEmailSender>();
            services.AddSingleton<EmailClientSettings>();

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
        }
    }
}
