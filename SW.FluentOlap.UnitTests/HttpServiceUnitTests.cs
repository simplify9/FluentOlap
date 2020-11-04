using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SW.FluentOlap.Models;

namespace UtilityUnitTests
{
    [TestClass]
    public class HttpServiceUnitTests
    {
        [TestMethod]
        public void CreateServiceTest()
        {
            HttpService service = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}", "PostsService");
            Assert.AreEqual("PostsService", service.ServiceName);
        }

        [TestMethod]
        public void GetRequiredParameters()
        {
            HttpService service1 = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}", "PostsService");
            
            
            Assert.IsTrue(service1.GetRequiredParameters().Contains("PostId"));
            Assert.IsTrue(service1.GetRequiredParameters().Count() == 1);
            
            HttpService service2 = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}/comments/{CommentId}", "CommentsService");
            Assert.IsTrue(service2.GetRequiredParameters().Contains("PostId") && service2.GetRequiredParameters().Contains("CommentId") );
            Assert.IsTrue(service2.GetRequiredParameters().Count() == 2);
        }

        [TestMethod]
        public async Task FormatUriTest()
        {
            
            HttpService service1 = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}", "PostsService");

            HttpResponse invokation = await service1.InvokeAsync(new HttpServiceOptions
            {
                Parameters = new
                {
                    PostId = 1
                }
            }) as HttpResponse;
            
            
            Assert.AreEqual(invokation.FormattedUrlCalled, "https://jsonplaceholder.typicode.com/posts/1" );
        }

        [TestMethod]
        public async Task CallApiTest()
        {
            HttpService service1 = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}", "PostsService");

            HttpResponse invocation = await service1.InvokeAsync(new HttpServiceOptions
            {
                Parameters = new
                {
                    PostId = 1
                }
            }) as HttpResponse;

            JToken rs = JToken.Parse(invocation.Content);
            Assert.AreEqual(rs["id"]?.ToString(), "1" );
            
        }
    }
}