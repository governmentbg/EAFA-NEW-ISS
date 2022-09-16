using IARA.DataAccess;
using IARA.DataAccess.Abstractions;
using IARA.Security;
using IARA.Security.AuthContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TLTTS.Common.ConfigModels;

namespace IARA.DI
{
    public static class DatabaseInitializer
    {
        public static void AddDbContext(this IServiceCollection services, ConnectionStrings connectionString)
        {
            //services.AddDbContextFactory<BaseIARADbContext>(options =>
            //{
            //    options.UseNpgsql(connectionString.Value, npgSqlOptions =>
            //    {
            //        npgSqlOptions.UseNetTopologySuite();
            //    }).ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            //});

            services.AddDbContext<BaseIARADbContext>(options =>
            {
                options.UseNpgsql(connectionString.Connection, npgSqlOptions =>
                {
                    npgSqlOptions.UseNetTopologySuite();
                }).ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            }, ServiceLifetime.Scoped, ServiceLifetime.Singleton);


            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql(connectionString.Connection, npgSqlOptions =>
                {
                    npgSqlOptions.EnableRetryOnFailure(2);
                    npgSqlOptions.UseNetTopologySuite();
                });
            }, ServiceLifetime.Scoped, ServiceLifetime.Singleton);

            services.AddScoped<IARADbContext>();

            services.AddScoped<ILoggingDbContext>((serviceProvider) =>
            {
                return serviceProvider.GetService<IARADbContext>();
            });

            services.AddScoped<IUsersDbContext>((serviceProvider) =>
            {
                return serviceProvider.GetService<IARADbContext>();
            });

        }
    }
}
