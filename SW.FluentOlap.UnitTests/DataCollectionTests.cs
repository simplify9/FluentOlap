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
            
            PopulationResultCollection rs =  await DataCollector.CollectData(analyzer, new HttpServiceOptions
            {
                Parameters = new
                {
                    PostId = 1
                }
            });

            PopulationResult data = rs.FirstOrDefault();
            return;

        }
    }
}