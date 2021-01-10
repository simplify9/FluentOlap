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
        public static IServiceCollection AddFluentOlap(
            this IServiceCollection serviceCollection, 
            ServiceDefinitions serviceDefinitions,
            Action<TransformationsMasterList> transformationsMasterList,
            params Assembly[] assemblies)
        {

            IDictionary<string, TypeMap> maps = new Dictionary<string, TypeMap>();
            IList<MessageProperties> messageMaps = new List<MessageProperties>();
            serviceCollection.Scan(scan =>
            {
                var analyzerClasses = scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IAnalyticalNode))).AsImplementedInterfaces().WithTransientLifetime();
            });

            IEnumerable<ServiceDescriptor> services = serviceCollection.Where(
                a => a.ImplementationType != null && 
                a.ImplementationType.GetInterfaces().Any(b => b == typeof(IAnalyticalNode))
            );

            foreach(ServiceDescriptor service in services){
                IAnalyticalNode instance = (IAnalyticalNode)Activator.CreateInstance(service.ImplementationType);

                if (instance.MessageMap == null) continue;
                if(!maps.ContainsKey(instance.MessageMap.MessageName)) 
                    maps.Add(instance.MessageMap.MessageName, instance.TypeMap);

                if(messageMaps.All(m => m.MessageName != instance.MessageMap.MessageName)) 
                    messageMaps.Add(instance.MessageMap);
            }

            //serviceCollection.AddSingleton(serviceDefinitions);
            FluentOlapConfiguration.ServiceDefinitions = serviceDefinitions;
            serviceCollection.AddSingleton(new MasterTypeMaps(maps, messageMaps));
            return serviceCollection;
        }

    }
}
