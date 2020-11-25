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
        public async Task StartToFinish()
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

            analyzer.Ignore(p => p.Body);

            PopulationResult result = await analyzer.PopulateAsync(new HttpServiceOptions
            {
                Parameters = new
                {
                    PostId = 1
                }
            });
            
            DataIngester ingester = new DataIngester(new MariaDbProvider(MariaDbTableEngine.InnoDB));

            await ingester.InsertIntoDb(result, connection);
            
            
            string username = await connection.RunCommandGetString("select * from Post", "userid_user_username");
            Assert.AreEqual(username, "Bret");
            
            
            analyzer.Property(p => p.Body);
            PopulationResult result2 = await analyzer.PopulateAsync(new HttpServiceOptions
            {
                Parameters = new
                {
                    PostId = 1
                }
            });
            
                
            await ingester.InsertIntoDb(result2, connection);
            
            string body = await connection.RunCommandGetString("select * from Post where Id=2", "post_body");
            
            
            
            string bodyFromApi =
                "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto";
            Assert.AreEqual(body, bodyFromApi);
            
            
            
            await connection.RunCommandAsync("DROP TABLE Post");
            await connection.RunCommandAsync("DROP TABLE AnalyzedModelHashes");

        }
        
    }
}