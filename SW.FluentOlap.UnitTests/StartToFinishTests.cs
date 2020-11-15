using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using SW.FluentOlap.AnalyticalNode;
using SW.FluentOlap.Ingester;
using SW.FluentOlap.Ingester.MariaDb;
using SW.FluentOlap.Models;
using UtilityUnitTests.Models;

namespace UtilityUnitTests
{
    [TestClass]
    public class StartToFinishTests
    {
        
        
        private IConfiguration config;

        public StartToFinishTests()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("/home/shaheen/dev/FluentOlap/SW.FluentOlap.UnitTests/testsettings.json");
            config = builder.Build();
        }
        
        [TestMethod]
        public async Task Test1()
        {
            
            MySqlConnection connection = new MySqlConnection(config.GetConnectionString("TestConnection"));
            await connection.RunCommandAsync("DROP TABLE IF EXISTS Post");
            await connection.RunCommandAsync("DROP TABLE IF EXISTS AnalyzedModelHashes");
            
            FluentOlapConfiguration.ServiceDefinitions = new ServiceDefinitions
            {
                ["PostsService"] = new HttpService("https://jsonplaceholder.typicode.com/posts/{PostId}"),
                ["UsersService"] = new HttpService("https://jsonplaceholder.typicode.com/users/{userId}"),
            };
            
            PostAnalyzer analyzer = new PostAnalyzer();

            analyzer.GetFromService("PostsService");
            analyzer.Property(p => p.userId).GetFromService("UsersService", new AnalyticalObject<User>());

            PopulationResult result = await analyzer.PopulateAsync(new HttpServiceOptions
            {
                Parameters = new
                {
                    PostId = 1
                }
            });
            
            DataIngester ingester = new DataIngester(new MariaDbProvider(MariaDbTableEngine.InnoDB));

            await ingester.InsertIntoDb(result, connection);
            
            
            string username = await connection.RunCommandGetString("select * from Post", "user_username");
            
            Assert.AreEqual(username, "Bret");

        }
        
    }
}