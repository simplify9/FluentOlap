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

        public async Task<PopulationResult> GetDataFromEndpoints<T>(AnalyticalObject<T> analyticalObject, string rootValue,
            IHttpClientFactory httpClientFactory = null)
        {
            return await 
                GetDataFromEndpoints(analyticalObject.Name.ToLower(),
                rootValue, analyticalObject.ServiceName,
                FluentOlapConfiguration.ServiceDefinitions, 
                analyticalObject.TypeMap, 
                FluentOlapConfiguration.Metadata, 
                httpClientFactory);
        }


        /// <summary>
        /// Gets data using service and returns it in a flattened dictionary
        /// </summary>
        /// <param name="rootValue">ID of root object</param>
        /// <param name="metadata">AnalyticalMetaData from MasterTypeMaps</param>
        /// <param name="rootServiceName"></param>
        /// <param name="services">ServiceDefinitions from DI</param>
        /// <param name="typeMap">TypeMap of the root object</param>
        /// <returns>Flattened and denormalized Dictionary of the root object and its children</returns>
        public async Task<PopulationResult> GetDataFromEndpoints(string rootObjectName, string rootValue, string rootServiceName, ServiceDefinitions services, TypeMap typeMap, AnalyticalMetadata metadata = null, IHttpClientFactory httpClientFactory = null)
        {
            if (services == null)
                throw new Exception("No service definitions found.");

            if (rootServiceName == null)
                throw new Exception("No service declared for root object.");
            
            if(!services.ContainsKey(rootServiceName))
                throw new Exception($"Service with name {rootServiceName} not found.");
            
            string baseUrl = services[rootServiceName].BaseUrl;
            
            string endpoint = services[rootServiceName].Endpoint.StartsWith('/')? services[rootServiceName].Endpoint : '/' + services[rootServiceName].Endpoint ;
            string fullUrl = (baseUrl.EndsWith("/") ? baseUrl.Substring(0, baseUrl.Length - 1) : baseUrl) 
                             + (endpoint.EndsWith("/") ? endpoint : endpoint + '/') + rootValue;
            IDictionary<string, object> objects = new Dictionary<string, object>();

            using(HttpClient httpClient = httpClientFactory != null? httpClientFactory.CreateClient() : new HttpClient())
            {
                string rootObjectRs = await httpClient.GetStringAsync(fullUrl);
                IDictionary<string, object> rootObject = JsonHelper.DeserializeAndFlatten(rootObjectRs);;

                foreach(KeyValuePair<string, object> childEntry in rootObject)
                    objects.Add(rootObjectName.Replace("/", "") + "_" + childEntry.Key, childEntry.Value);

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
