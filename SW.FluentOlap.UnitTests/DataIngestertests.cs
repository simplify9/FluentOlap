using System;
using System.Collections.Generic;
using System.Data.Common;
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
    public class DataIngesterTests
    {
        private IConfiguration config;

        public DataIngesterTests()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("/home/shaheen/dev/FluentOlap/SW.FluentOlap.UnitTests/testsettings.json");
            config = builder.Build();
        }

        [TestMethod]
        public void Instantiation()
        {
            DataIngester ingester = new DataIngester(new MariaDbProvider(MariaDbTableEngine.InnoDB));
            ingester.InsertIntoDb(new PopulationResult(new Dictionary<string, object>(), new TypeMap()),
                new MySqlConnection(config.GetConnectionString("TestConnection")));
        }

        [TestMethod]
        public async Task Insertion()
        {
            DataIngester ingester = new DataIngester(new MariaDbProvider(MariaDbTableEngine.InnoDB));
            await ingester.InsertIntoDb(new PopulationResult(new Dictionary<string, object>()
                {
                    ["basicinsertclass_test1"] = 1,
                    ["basicinsertclass_test2"] = "hello"
                }, new AnalyticalObject<BasicInsertClass>().TypeMap),
                new MySqlConnection(config.GetConnectionString("TestConnection")));

            var connection = new MySqlConnection(config.GetConnectionString("TestConnection"));

            string test2Val = await connection.RunCommandGetString("select * from BasicInsertClass", "basicinsertclass_test2");

            await connection.RunCommandAsync("DROP TABLE BasicInsertClass");
            
            Assert.AreEqual(test2Val, "hello");
            
            


        }
        
    }
}