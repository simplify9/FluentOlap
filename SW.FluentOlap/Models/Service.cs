using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SW.FluentOlap.Models
{
    public enum ServiceType
    {
        HttpCall,
        DatabaseCall
    }
    public interface IService
    {
        
        public ServiceType ServiceType { get; }

    }

    public class HttpService : IService
    {
        /// <summary>
        /// All strings between double curlies {{}} will be treated as Json Paths
        /// And filled in during the Data collection.
        /// This Url will override all other Url related properties if provided.
        /// Example: https://someUrl.com/{Id}/comments/{comment.Id}
        /// </summary>
        public string TemplatedUrl { get; set; }

        public ServiceType ServiceType => ServiceType.HttpCall;
    }

    public class DatabaseService : IService
    {
        
        /// <summary>
        /// All strings between double curlies {{}} will be treated as Json Paths
        /// And filled in during the Data collection.
        /// This Url will override all other Url related properties if provided.
        /// Example: https://someUrl.com/{Id}/comments/{comment.Id}
        /// </summary>
        public string TemplatedSelect { get; set; }
        public ServiceType ServiceType => ServiceType.HttpCall;
    }
    
    
}
