using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SW.FluentOlap.Utilities;

namespace SW.FluentOlap.Models
{
    public enum HttpVerb
    {
        Get,
        Post
    }

    /// <summary>
    /// Response model that defines the content to be a string
    /// </summary>
    public class HttpResponse : IServiceOutput
    {
        public string Content { get; set; }
        public string FormattedUrlCalled { get; set; }
        public string ContentType { get; set; }
        public string KeyPrefix { get; set; }
        public PopulationResult PopulationResult { get; set; }
        public string RawOutput => Content;
    }
    public class HttpServiceOptions : IServiceInput
    {
        private object _templateParameters;
        private Type _templateParametersType;
        
        public HttpVerb Verb { get; set; }

        /// <summary>
        /// In a POST request, this is sent as a body as is.
        /// In a GET Request, the Json Paths of the properties are used
        /// To fill in the curly braces {} in the template.
        /// </summary>
        public object Parameters
        {
            get => _templateParameters;
            set
            {
                _templateParameters = value;
                _templateParametersType = value.GetType();
            }
        }
        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
        private Type TemplateParametersType
        {
            get => _templateParametersType;
        }

        public string PrefixKey { get; set; }
    }
    /// <summary>
    /// Service that retrieves information using Http calls.
    /// </summary>
    public class HttpService : Service<HttpServiceOptions, HttpResponse >
    {
        
        private readonly IHttpClientFactory factory;
        private readonly IEnumerable<KeyValuePair<string, string>> headers;

        /// <summary>
        /// All strings between curly braces {} will be treated as Json Paths
        /// And filled in during the Data collection.
        /// This Url will override all other Url related properties if provided.
        /// Example: https://someUrl.com/{Id}/comments/{comment.Id}
        /// </summary>
        private readonly string templatedUrl;

        /// <summary>
        /// </summary>
        /// <param name="name">Will be set automatically if adding through ServiceDefinitions</param>
        /// <param name="templatedUrl">Url with Json Paths in curly braces {}
        /// that will be filled in by incoming parameters from the HttpServiceOptions
        /// </param>
        /// <param name="factory"></param>
        public HttpService(string templatedUrl, string name = null, IHttpClientFactory factory = null) : base(ServiceType.HttpCall, name)
        {
            this.templatedUrl = templatedUrl;
            this.factory = factory;
            
        }
        

        /// <summary>
        /// Convert parameters into a Microsoft HttpRequestMessage
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="verb"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private HttpRequestMessage GetRequestMessage(Uri uri, HttpVerb verb, object body)
        {
            HttpRequestMessage message;
            switch (verb)
            {
                case HttpVerb.Post:
                    message= new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = uri,
                        Content = new StringContent(
                            JsonConvert.SerializeObject(body),
                            Encoding.UTF8,
                            "application/json"
                        )
                    };
                    break;
                case HttpVerb.Get:
                default:
                    message = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = uri
                    };
                    break;
            }

            foreach(KeyValuePair<string, string> header in headers)
                message.Headers.Add(header.Key, header.Value);
            
            return message;
        }

        /// <summary>
        /// Removes any extra markings around parameter
        /// </summary>
        /// <param name="parameter">Parameter in template url.</param>
        /// <returns></returns>
        private string FormatParameter(string parameter) => new Regex("[\\{\\}]").Replace(parameter, "");

        /// <summary>
        /// Retrieves parameters from templated url.
        /// </summary>
        public IEnumerable<string> GetRequiredParameters(bool format = true)
        {
            IEnumerable<string> parameters = 
            Regex.Matches(
                templatedUrl,
                "" + "{\\w*\\}")
                .Select(c => 
                    c.Value
                );
            return !format ? parameters : parameters.Select(FormatParameter);
        }

        /// <summary>
        /// Fills in the JSON paths in a Uri using Parameters from HttpRequestOptions
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private Uri FormatUri(object parameters)
        {
            string formattedUrl = null;
            
            //All values between curly braces are treated as variables

            var requiredParameters = GetRequiredParameters(false);
            if (requiredParameters.Count() == 0) return new Uri(templatedUrl);
                
            foreach (string capture in requiredParameters)
            {
                JToken token = JToken.FromObject(parameters);

                // Optimization to avoid unnecessary split
                // If a dot is contained, it will be treated as a multi-level json retrieval.
                if (!capture.Contains('.'))
                {
                    string val = token[FormatParameter(capture)]?.Value<string>();
                    formattedUrl = templatedUrl.Replace(capture, val);

                }
                else
                {
                    JToken val = null;
                    foreach (string depthKey in capture.Split('.'))
                        val = token[depthKey];
                    formattedUrl = templatedUrl.Replace(capture, val!.Value<string>());
                }
            }
            return new Uri(formattedUrl);
        }

        /// <summary>
        /// Returns a delegate to make an HTTP call,
        /// Creates an HttpRequestMessage using parameters,
        /// Saves raw response as string and flattened dictionary
        /// </summary>
        public override Func<HttpServiceOptions, Task<HttpResponse>> InvokeAsync =>
            async options =>
            {
                using HttpClient client = factory != null? factory.CreateClient() : new HttpClient();

                Uri uri = FormatUri(options.Parameters);
                HttpRequestMessage request =
                    GetRequestMessage(uri, options.Verb, options.Parameters);
                
                HttpResponseMessage response = await client.SendAsync(request);

                string raw = await response.Content.ReadAsStringAsync();

                return new HttpResponse
                {
                    Content = raw,
                    
                    //TODO implement dynamic typing
                    ContentType = "application/json",
                    
                    FormattedUrlCalled = uri.OriginalString,
                    
                    PopulationResult = new PopulationResult(raw, options.PrefixKey?? "")
                };
            };
    }
}