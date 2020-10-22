using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            /*
            TypeMap:
            {
                "userId": "12312",
                "id": "12H"
            }
            Response:
             {
                "userId": "12312",
                "id": "12H"
                "body": "12321j",
             }
            */
            
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
        }
    }
}