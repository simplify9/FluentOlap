using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public string ContentType { get; set; }
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
        private Type TemplateParametersType
        {
            get => _templateParametersType;
        }
    }
    /// <summary>
    /// Service that retrieves information using Http calls.
    /// </summary>
    public class HttpService : Service<HttpServiceOptions, HttpResponse >
    {
        
        private readonly IHttpClientFactory factory;

        /// <summary>
        /// All strings between curly braces {} will be treated as Json Paths
        /// And filled in during the Data collection.
        /// This Url will override all other Url related properties if provided.
        /// Example: https://someUrl.com/{Id}/comments/{comment.Id}
        /// </summary>
        private readonly string templatedUrl;
        
        /// <summary>
        /// </summary>
        /// <param name="templatedUrl">Url with Json Paths in curly braces {}
        /// that will be filled in by incoming parameters from the HttpServiceOptions
        /// </param>
        /// <param name="factory"></param>
        public HttpService(string templatedUrl, IHttpClientFactory factory = null) : base(ServiceType.HttpCall)
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
            switch (verb)
            {
                case HttpVerb.Post:
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
                case HttpVerb.Get:
                default:
                    return new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = uri
                    };
            }
        }
        
        public IEnumerable<string> RequiredParameters => 
            Regex.Match(
                templatedUrl,
                "" + "{\\w*\\}")
                .Captures
                .Select(c => c.Value);

        /// <summary>
        /// Fills in the JSON paths in a Uri using Parameters from HttpRequestOptions
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private Uri FormattedUri(object parameters)
        {
            string formattedUrl = null;
            
            //All values between curly braces are treated as variables
            
            foreach (string capture in RequiredParameters)
            {
                JToken token = JToken.FromObject(parameters);

                // Optimization to avoid unnecessary split
                // If a dot is contained, it will be treated as a multi-level json retrieval.
                if (!capture.Contains('.'))
                {
                    string val = token[capture]?.Value<string>();
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

        public new Func<HttpServiceOptions, Task<HttpResponse>> InvokeAsync =>
            async options =>
            {
                using HttpClient client = factory.CreateClient();


                Uri uri = FormattedUri(options.Parameters);
                HttpRequestMessage request =
                    GetRequestMessage(uri, options.Verb, options.Parameters);
                
                HttpResponseMessage response = await client.SendAsync(request);

                return new HttpResponse
                {
                    Content = await response.Content.ReadAsStringAsync(),
                    ContentType = response.Headers.GetValues("Content-Type").FirstOrDefault()?? "application/json"
                };
            };
    }
}