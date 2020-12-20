using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SW.FluentOlap.AnalyticalNode;
using SW.FluentOlap.AnalyticalNodes;
using SW.FluentOlap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SW.FluentOlap.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddFluentOlap(this IServiceCollection serviceCollection, ServiceDefinitions serviceDefinitions ,params Assembly[] assemblies)
        {

            IDictionary<string, TypeMap> maps = new Dictionary<string, TypeMap>();
            IDictionary<string, string> serviceMaps = new Dictionary<string, string>();
            IList<MessageProperties> messageMaps = new List<MessageProperties>();
            serviceCollection.Scan(scan =>
            {
                var analyzerClasses = scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IAnalyticalNode))).AsImplementedInterfaces().WithTransientLifetime();
            });

            IEnumerable<ServiceDescriptor> services = serviceCollection.Where(
                a => a.ImplementationType != null && 
                a.ImplementationType.GetInterfaces() != null && 
                a.ImplementationType.GetInterfaces().Any(b => b == typeof(IAnalyticalNode))
            );

            foreach(ServiceDescriptor service in services){
                IAnalyticalNode instance = (IAnalyticalNode)Activator.CreateInstance(service.ImplementationType);

                foreach(var map in instance.TypeMap)
                    if(map.Value.ServiceName != null)
                        serviceMaps[map.Key] = map.Value.ServiceName;

                if (instance.MessageMap == null) continue;
                if(!maps.ContainsKey(instance.MessageMap.MessageName)) 
                    maps.Add(instance.MessageMap.MessageName, instance.TypeMap);

                if(!messageMaps.Any(m => m.MessageName == instance.MessageMap.MessageName)) 
                    messageMaps.Add(instance.MessageMap);

                if(!serviceMaps.ContainsKey(instance.MessageMap.MessageName)) 
                    serviceMaps.Add(instance.MessageMap.MessageName, instance.ServiceName);

            }

            //serviceCollection.AddSingleton(serviceDefinitions);
            FluentOlapConfiguration.ServiceDefinitions = serviceDefinitions;
            serviceCollection.AddSingleton(new MasterTypeMaps(maps, messageMaps));
            return serviceCollection;
        }
        public static IServiceCollection AddFluentOlap(this IServiceCollection serviceCollection, Action<ServiceDefinitions> configure,params Assembly[] assemblies)
        {
            if(assemblies.Length == 0)
            {
                Assembly assembly = Assembly.GetCallingAssembly();
                assemblies = new Assembly[] { assembly };
            }
            ServiceDefinitions serviceDefinitions = new ServiceDefinitions();
            configure.Invoke(serviceDefinitions);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceDefinitionsConfig = serviceProvider.GetRequiredService<IConfiguration>().GetSection("FluentOlap").GetSection("ServiceDefinitions");
            serviceDefinitionsConfig.Bind(serviceDefinitions);

            serviceCollection.AddFluentOlap(serviceDefinitions, assemblies);
            return serviceCollection;
        }

    }
}
