using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.FluentOlap.AnalyticalNode;
using SW.FluentOlap.Models;
using UtilityUnitTests.Models;

namespace UtilityUnitTests
{
    [TestClass]
    public class TransformationTests
    {
        [TestMethod]
        public async Task TransformConcatenationTest()
        {

            var analyzer = new AnalyticalObject<User> {ServiceName = "UsersService"};

            analyzer.Property(u => u.Username).HasTransformation(
                name =>
                {
                    string converted = name.ToString();
                    return converted + "ta";
                }
            );

            FluentOlapConfiguration.ServiceDefinitions = new ServiceDefinitions
            {
                ["UsersService"] = new HttpService("https://jsonplaceholder.typicode.com/users/1")
            };

            PopulationResult rs = await analyzer.PopulateAsync(new HttpServiceOptions());
            
            Assert.AreEqual(rs["user_username"], "Bretta");


        }
    }
}