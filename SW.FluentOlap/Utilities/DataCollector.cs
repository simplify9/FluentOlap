using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using SW.FluentOlap.Models;
using System.Threading;
using SW.FluentOlap.AnalyticalNode;

namespace SW.FluentOlap.Utilities
{
    public static class DataCollector
    {
        private static async Task<PopulationResult> CallService(IService focusedService, IServiceInput serviceInput, string expandableKey = null)
        {
            IServiceOutput output = null;

            switch (focusedService.Type)
            {
                case ServiceType.DatabaseCall:
                    break;
                case ServiceType.HttpCall:

                    if (!(focusedService is HttpService service))
                        throw new Exception($"Invalid HttpService defined for service {focusedService.ServiceName}");

                    output = await service.InvokeAsync(serviceInput as HttpServiceOptions);

                    if (expandableKey != null) output.ChildKey = expandableKey;

                    break;
            }

            return output.PopulationResult;
        }

        public static async Task<PopulationResultCollection> CollectData<T>(AnalyticalObject<T> focusedObject,
            IServiceInput serviceInput)
        {
            if (focusedObject.ServiceName == null)
                throw new Exception("Root analyzer object must have a service defined.");
            
            if(!FluentOlapConfiguration.ServiceDefinitions.ContainsKey(focusedObject.ServiceName))
                throw new Exception($"Service {focusedObject.ServiceName} not defined.");
            
            IService focusedService = FluentOlapConfiguration.ServiceDefinitions[focusedObject.ServiceName];
            PopulationResultCollection results = new PopulationResultCollection();

            PopulationResult rootResult = await CallService(focusedService, serviceInput);
            results.Add(rootResult);

            foreach (var expandable in focusedObject.ExpandableChildren)
            {
                string expandableKey = rootResult[expandable.Key]?.ToString();
                
                if (expandable.Value.ServiceName == null)
                    throw new Exception("Child analyzer object must have a service defined.");
                
                if(!FluentOlapConfiguration.ServiceDefinitions.ContainsKey(expandable.Value.ServiceName))
                    throw new Exception($"Service {expandable.Value.ServiceName} not defined.");
                
                IService expandableService = FluentOlapConfiguration.ServiceDefinitions[expandable.Value.ServiceName];

                IServiceInput input = null;

                switch (expandableService.Type)
                {
                    case ServiceType.HttpCall:
                        HttpService expandableHttpService = expandableService as HttpService;
                        IDictionary<string, string> parameters = new Dictionary<string, string>();
                        foreach (string innerParam in expandableHttpService.GetRequiredParameters())
                        {
                            JToken parsedRoot = JToken.Parse(rootResult.Raw);
                            parameters.Add(innerParam, parsedRoot[innerParam].Value<string>());
                        }

                        input = new HttpServiceOptions
                        {
                            ChildKey = expandable.Value.NodeName,
                            Parameters = parameters
                        };

                        await expandableHttpService.InvokeAsync((HttpServiceOptions) input);

                        break;
                }

                results.Add(await CallService(expandableService, input));
            }

            return results;
        }
    }
}