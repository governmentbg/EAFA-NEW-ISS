using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Tests
{
    internal static class TestExtensions
    {
        public static T CreateTestService<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            IServiceScope scope = serviceProvider.CreateScope();
            IServiceProvider provider = scope.ServiceProvider;

            ConstructorInfo constructor = typeof(T).GetConstructor(new Type[] { typeof(IServiceProvider) });
            return constructor.Invoke(new object[] { provider }) as T;
        }
    }
}
