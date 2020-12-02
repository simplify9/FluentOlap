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
            builder.AddJsonFile("appsettings.json");
            config = builder.Build();
        }


        [TestMethod]
        public async Task Insertion()
        {
            var connection = new MySqlConnection(config.GetConnectionString("TestConnection"));
            
            await connection.RunCommandAsync("DROP TABLE IF EXISTS BasicInsertClass");
            await connection.RunCommandAsync("DROP TABLE IF EXISTS AnalyzedModelHashes");
            
            DataIngester ingester = new DataIngester(new MariaDbProvider(MariaDbTableEngine.InnoDB));
            
            await ingester.Insert(new PopulationResult(new Dictionary<string, object>()
                {
                    ["basicinsertclass_test1"] = 1,
                    ["basicinsertclass_test2"] = "hello"
                }, new AnalyticalObject<BasicInsertClass>().TypeMap),
                new MySqlConnection(config.GetConnectionString("TestConnection")));


            string test2Val = await connection.RunCommandGetString("select * from BasicInsertClass", "basicinsertclass_test2");

            
            Assert.AreEqual(test2Val, "hello");
            
            


        }
        
    }
}