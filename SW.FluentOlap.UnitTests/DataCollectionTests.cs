using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.FluentOlap.AnalyticalNode;
using SW.FluentOlap.Models;
using SW.FluentOlap.Utilities;
using UtilityUnitTests.Models;

namespace UtilityUnitTests
{
    [TestClass]
    public class DataCollectionTests
    {
        [TestMethod]
        public async Task BasicCollection()
        {
            FluentOlapConfiguration.ServiceDefinitions = new ServiceDefinitions
            {
                ["PostsService"] = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}")
            };

            PostAnalyzer analyzer = new PostAnalyzer();
            analyzer.ServiceName = "PostsService";

            PopulationResultCollection rs = await DataCollector.CollectData(analyzer, new HttpServiceOptions
            {
                ChildKey = "post",
                Parameters = new
                {
                    PostId = 1
                }
            });

            PopulationResult data = rs.Dequeue();
            foreach (string key in data.Keys)
            {
                Assert.IsTrue(analyzer.TypeMap.Keys.Contains(key));
            }
            
        }

        [TestMethod]
        public async Task BasicPopulationTest()
        {
            FluentOlapConfiguration.ServiceDefinitions = new ServiceDefinitions
            {
                ["PostsService"] = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}")
            };

            PostAnalyzer analyzer = new PostAnalyzer();
            analyzer.ServiceName = "PostsService";
            PopulationResult rs = await analyzer.PopulateAsync(new PopulationContext<HttpServiceOptions>(
                new HttpServiceOptions
                {
                    Parameters = new
                    {
                        PostId = 1
                    }
                }));
            
            foreach (string key in rs.Keys)
                Assert.IsTrue(analyzer.TypeMap.Keys.Contains(key));
            
        }

        [TestMethod]
        public async Task WithChildPopulationTest()
        {
            PostAnalyzer analyzer = new PostAnalyzer();
            analyzer.ServiceName = "PostsService";
            
            FluentOlapConfiguration.ServiceDefinitions = new ServiceDefinitions
            {
                ["PostsService"] = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}"),
                ["UsersService"] = new HttpService("https://jsonplaceholder.typicode.com/users/{userId}"),
            };
            analyzer.Property(p => p.userId).GetFromService("UsersService", new AnalyticalObject<User>());
            
            
            PopulationResult rs = await analyzer.PopulateAsync(new PopulationContext<HttpServiceOptions>(
                new HttpServiceOptions
                {
                    Parameters = new
                    {
                        PostId = 1
                    }
                }));
            
            foreach (string key in analyzer.TypeMap.Keys)
                Assert.IsTrue(rs.Keys.Contains(key));
        }
    }
}