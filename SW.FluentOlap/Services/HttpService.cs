using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SW.FluentOlap.Models
{
    public enum HttpVerb
    {
        GET,
        POST
    }

    public class HttpResponse 
    {
        public string Content { get; set; }
        public string ContentType { get; set; }
    }
    public class HttpServiceOptions
    {
        private object _templateParameters;
        private Type _templateParametersType;
        
        public HttpVerb Verb { get; set; }

        public IEnumerable<string> ParameterArray { get; set; }
        public object TemplateParameters
        {
            get => _templateParameters;
            set
            {
                _templateParameters = value;
                _templateParametersType = value.GetType();
            }
        }
        public Type TemplateParametersType
        {
            get => _templateParametersType;
        }
    }
    public class HttpService : Service<HttpServiceOptions, HttpResponse >
    {
        
        private readonly IHttpClientFactory factory;
        
        /// <summary>
        /// All strings between curly braces {} will be treated as Json Paths
        /// And filled in during the Data collection.
        /// This Url will override all other Url related properties if provided.
        /// Example: https://someUrl.com/{Id}/comments/{comment.Id}
        /// </summary>
        public string TemplatedUrl { get; set; }
        
        public HttpService(string templatedUrl, IHttpClientFactory factory = null) : base(ServiceType.HttpCall)
        {
            TemplatedUrl = templatedUrl;
            this.factory = factory;
            
        }

        private HttpRequestMessage GetRequestMessage(Uri uri, HttpVerb verb, object body)
        {
            switch (verb)
            {
                case HttpVerb.POST:
                    return new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = uri,
                        Content = new StringContent(
                            JsonConvert.SerializeObject(body),
                            Encoding.UTF8,
                            "application/json"
                        )
                    };
                case HttpVerb.GET:
                default:
                    return new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = uri
                    };
            }
        }
        
        public new Func<HttpServiceOptions, Task<HttpResponse>> InvokeAsync =>
            async options =>
            {
                using HttpClient client = factory.CreateClient();
                
                
                var uri = new Uri(string.Format(TemplatedUrl, options.ParameterArray));
                HttpRequestMessage request =
                    GetRequestMessage(uri, options.Verb, options.TemplateParameters);
                
                var response = await client.SendAsync(request);

                return new HttpResponse
                {
                    Content = await response.Content.ReadAsStringAsync(),
                    ContentType = response.Headers.GetValues("Content-Type").FirstOrDefault()
                };
            };
    }
}