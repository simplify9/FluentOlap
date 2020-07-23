using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SW.FluentOlap.Models
{
    public class PopulationContext<T>
    {
        public IHttpClientFactory HttpClientFactory { get; set; }
        public T Message { get; set; }
        public PopulationContext(T message, IHttpClientFactory httpClientFactory = null)
        {
            HttpClientFactory = httpClientFactory;
            Message = message;
        }
        
    }
}
