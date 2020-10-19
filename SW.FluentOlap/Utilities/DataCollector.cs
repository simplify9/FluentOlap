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
    internal class DataCollector
    {
        private async Task<PopulationResult> CallService(IService focusedService, IServiceInput serviceInput)
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

                    break;
            }

            return new PopulationResult(output.RawOutput);
        }

        public async Task<PopulationResultCollection> CollectData<T>(AnalyticalObject<T> focusedObject,
            IServiceInput serviceInput)
        {
            IService focusedService = FluentOlapConfiguration.ServiceDefinitions[focusedObject.ServiceName];
            PopulationResultCollection results = new PopulationResultCollection();

            PopulationResult rootResult = await CallService(focusedService, serviceInput);
            results.Add(rootResult);

            foreach (var expandable in focusedObject.ExpandableChildren)
            {
                string expandableKey = rootResult[expandable.Key.Split('_').Last().ToLower()]?.ToString();
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
                            Parameters = parameters
                        };

                        break;
                }

                results.Add(await CallService(expandableService, input));
            }

            return results;
        }
    }
}