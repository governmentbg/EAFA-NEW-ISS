using System.Reflection;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.Validations.Common
{
    public class FluxValidations<T>
    {
        protected IARADbContext Db { get; private set; }

        protected IScopedServiceProvider ServiceProvider { get; private set; }

        protected FluxValidations(IARADbContext db, IScopedServiceProviderFactory serviceProvider)
        {
            Db = db;
            ServiceProvider = serviceProvider.GetServiceProvider();
        }

        public List<FluxValidationResult> Run(T obj)
        {
            List<FluxValidationResult> validations = new();

            List<MethodInfo> methods = GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(FluxValidationAttribute), false).Length > 0)
                .ToList();

            foreach (MethodInfo method in methods)
            {
                object result = method.Invoke(this, new object[] { obj });

                if (result is bool valid)
                {
                    if (!valid)
                    {
                        FluxValidationAttribute attribute = method.GetCustomAttribute<FluxValidationAttribute>();

                        validations.Add(new FluxValidationResult
                        {
                            Code = attribute.Code,
                            Level = attribute.Level,
                            Severity = attribute.Severity,
                            Object = attribute.Object,
                            ErrorMessage = attribute.ErrorMessage
                        });
                    }
                }
                else
                {
                    throw new ArgumentException("FluxValidation method should return bool.");
                }
            }

            return validations;
        }
    }
}
