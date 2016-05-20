using System;
using gen.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace gen
{
    internal class ServiceLocator
    {
        private static readonly IServiceProvider ServiceProvider = GetServiceProvider();

        public static IApplicationEngine CreateApplicationEngine()
        {
            return ServiceProvider.GetService<IApplicationEngine>();
        }

        private static IServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IApplicationEngine, ApplicationEngine>();
            serviceCollection.AddSingleton<IAssemblyFinder, AssemblyFinder>();
            serviceCollection.AddSingleton<IAssemblyListWriterFactory, AssemblyListWriterFactory>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
