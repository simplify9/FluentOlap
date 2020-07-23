using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using SW.FluentOlap.Models;
using System.Threading;

namespace SW.FluentOlap.Utilities
{
    public class DataCollectionRequest
    {
        public string RootValue { get; set; }
        public string RootName { get; set; }
        public AnalyticalMetadata Metadata { get; set; }
        public ServiceDefinitions Services { get; set; }
        public TypeMap TypeMap { get; set; }
    }
    internal class DataCollector
    {
        public DataCollector()
        {
        }


        /// <summary>
        /// Gets data of root object using baseUrl, rootValue and rootName, then uses retrieved
        /// data to expand from other services, if appropiate.
        /// </summary>
        /// <param name="baseUrl">Base URL for the root object</param>
        /// <param name="rootValue">ID of root object</param>
        /// <param name="rootName">Endpoint for root object</param>
        /// <param name="metadata">AnalyticalMetaData from MasterTypeMaps</param>
        /// <param name="services">ServiceDefinitions from DI</param>
        /// <param name="typeMap">TypeMap of the root object</param>
        /// <returns>Flatted and denormalized Dictionary of the root object and its children</returns>
        public async Task<PopulationResult> GetDataFromEndpoints(string rootValue, string rootName, ServiceDefinitions services, TypeMap typeMap, AnalyticalMetadata metadata = null, IHttpClientFactory httpClientFactory = null)
        {
            string baseUrl = metadata == null? services[rootName].BaseUrl : metadata.BaseUrl;
            string endpoint = services[rootName].Endpoint;
            string fullUrl = (baseUrl.EndsWith("/") ? baseUrl : baseUrl + '/') + (rootName.EndsWith("/") ? endpoint : endpoint + '/') + rootValue;
            IDictionary<string, object> objects = new Dictionary<string, object>();

            using(HttpClient httpClient = httpClientFactory != null? httpClientFactory.CreateClient() : new HttpClient())
            {
                string rootObjectRs = await httpClient.GetStringAsync(fullUrl);
                IDictionary<string, object> rootObject = JsonHelper.DeserializeAndFlatten(rootObjectRs);;

                foreach(KeyValuePair<string, object> childEntry in rootObject)
                    objects.Add(endpoint.Replace("/", "") + "_" + childEntry.Key, childEntry.Value);

                foreach (KeyValuePair<string, NodeProperties> map in typeMap) {
                    string serviceName = map.Value.ServiceName;
                    if (serviceName == null) continue;
                    string key = map.Key.Split('_').Last().ToLower();
                    string nodeName = map.Value.NodeName;

                    string childBaseUrl = services[serviceName].BaseUrl ?? metadata.BaseUrl;
                    string childFullUrl = childBaseUrl + services[serviceName].Endpoint + '/' +rootObject[key];
                    string childRs = await httpClient.GetStringAsync(childFullUrl);

                    IDictionary<string, object> childObject = JsonHelper.DeserializeAndFlatten(childRs);
                    foreach(KeyValuePair<string, object> childEntry in childObject)
                        if(!objects.ContainsKey(nodeName + '_' + childEntry.Key))
                            objects.Add(nodeName + '_' + childEntry.Key, childEntry.Value);
                }
            }
            return new PopulationResult(typeMap, objects);

        }
    }
}
