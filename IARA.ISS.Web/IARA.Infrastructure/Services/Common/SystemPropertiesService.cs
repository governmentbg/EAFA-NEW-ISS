using System;
using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.EntityModels.Entities;
using IARA.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace IARA.Infrastructure.Services
{
    public class SystemPropertiesService
    {
        private readonly ScopedServiceProviderFactory scopedServiceProviderFactory;
        private static readonly object padlock = new object();

        private static SystemPropertiesDTO systemProperties = null;

        public SystemPropertiesDTO SystemProperties
        {
            get
            {
                if (systemProperties == null)
                {
                    lock (padlock)
                    {
                        if (systemProperties == null)
                        {
                            systemProperties = GetSystemProperties();
                        }
                    }
                }

                return systemProperties;
            }
        }

        public SystemPropertiesService(ScopedServiceProviderFactory scopedServiceProviderFactory)
        {
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
        }

        private SystemPropertiesDTO GetSystemProperties()
        {
            DateTime now = DateTime.Now;
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            using IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();

            List<NsystemParameter> parameters = (from prm in db.NsystemParameters
                                                 where prm.ValidFrom <= now && prm.ValidTo >= now
                                                 select prm).ToList();

            return BuildSystemPropertiesModel<SystemPropertiesDTO>(parameters);
        }

        private T BuildSystemPropertiesModel<T>(List<NsystemParameter> parameters)
            where T : class, new()
        {
            T settings = new T();

            foreach (var property in settings.GetType().GetProperties())
            {
                string parameterName = property.Name;

                var columnAttribute = property.GetCustomAttributes(typeof(ColumnAttribute), true).Cast<ColumnAttribute>().FirstOrDefault();
                if (columnAttribute != null)
                {
                    parameterName = columnAttribute.Name;
                }

                NsystemParameter param = parameters.Where(x => x.Code == parameterName).FirstOrDefault();

                if (param != null)
                {
                    object value = typeof(SystemPropertiesService)
                           .GetMethod(nameof(GetParamValue), BindingFlags.NonPublic | BindingFlags.Instance)
                           .MakeGenericMethod(property.PropertyType)
                           .Invoke(this, new object[] { param });

                    property.SetValue(settings, value);
                }
            }

            return settings;
        }

        private T GetParamValue<T>(NsystemParameter param)
        {
            switch (param.DataType)
            {
                case "STRING":
                    return (T)(object)param.ParamValue;
                case "DATE":
                case "DATETIME":
                    return (T)(object)DateTime.Parse(param.ParamValue);
                case "INT":
                    return (T)(object)int.Parse(param.ParamValue);
                case "NUMERIC":
                    return (T)(object)decimal.Parse(param.ParamValue);
                default:
                    throw new ArgumentException("Invalid data type for system property: " + param.DataType);
            }
        }
    }
}
