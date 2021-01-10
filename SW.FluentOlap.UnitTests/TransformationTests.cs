using System;
using System.Collections.Generic;
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
        public void TransformationShadowPropertyTest()
        {
            
            var analyzer = new AnalyticalObject<ValueTypeTest>();
            var innerAnalyzer = new AnalyticalObject<ValueTypeTestInner>();
            innerAnalyzer.Property(x => x.z).HasTransformation(o => o + 2);
            analyzer.Property("shadow", innerAnalyzer);
            
            var popRs = new PopulationResult(new Dictionary<string, object>()
            {
                ["valuetypetestinner_z"] = 1
            }, analyzer.TypeMap);
            Assert.AreEqual(popRs["valuetypetestinner_z"], 3);
        }
        
        
        [TestMethod]
        public void TransformationShadowPropertyNullTest()
        {
            
            var analyzer = new AnalyticalObject<ValueTypeTest>();
            var innerAnalyzer = new AnalyticalObject<ValueTypeTestInner>();
            innerAnalyzer.Property(x => x.z).HasTransformation(o => o + 2);
            analyzer.Property("shadow", innerAnalyzer);

            try
            {
                var popRs = new PopulationResult(new Dictionary<string, object>()
                {
                }, analyzer.TypeMap);
            }
            catch(InvalidOperationException e)
            {
            }
            //Assert.AreEqual(popRs["valuetypetestinner_z"], 3);
        }
        
        [TestMethod]
        public void TransformationCastTest()
        {
            var analyzer = new AnalyticalObject<ValueTypeTest>();
            analyzer.Property(p => p.x).HasTransformation<string>(o =>  $"|{o}|");
            
            var popRs = new PopulationResult(new Dictionary<string, object>()
            {
                ["valuetypetest_x"] = 1
            }, analyzer.TypeMap);
            
            Assert.AreEqual(popRs["valuetypetest_x"], "|1|");
        }

        [TestMethod]
        public void TransformationFromMasterListTest()
        {
            FluentOlapConfiguration.TransformationsMasterList.AddTransformation<int, string>("Surround", o => $"|{o}|");
            
            var analyzer = new AnalyticalObject<ValueTypeTest>();
            analyzer.Property(p => p.x).HasTransformation("Surround");
            var popRs = new PopulationResult(new Dictionary<string, object>()
            {
                ["valuetypetest_x"] = 1
            }, analyzer.TypeMap);
            Assert.AreEqual(popRs["valuetypetest_x"], "|1|");
        }
        [TestMethod]
        public void TransformationFromMasterListInternalKeytest()
        {
            FluentOlapConfiguration.TransformationsMasterList
                .AddTransformation<int, string>(InternalType.INTEGER, o => $"|{o}|");
            
            var analyzer = new AnalyticalObject<ValueTypeTest>();
            analyzer.Property(p => p.x);
            var popRs = new PopulationResult(new Dictionary<string, object>()
            {
                ["valuetypetest_x"] = 1
            }, analyzer.TypeMap);
            Assert.AreEqual(popRs["valuetypetest_x"], "|1|");
        }
        
        
        [TestMethod]
        public void TransformationValueTypeNullTest()
        {
            var analyzer = new AnalyticalObject<ValueTypeTest>();
            analyzer.Property(p => p.x).HasTransformation(o => o + 1);
            try
            {
                var popRs = new PopulationResult(new Dictionary<string, object>()
                {
                    ["valuetypetest_x"] = null
                }, analyzer.TypeMap);
            }
            catch (InvalidOperationException e)
            {
            }
        }


        [TestMethod]
        public void TransformWithDefaultValueTest()
        {
            
            var analyzer = new AnalyticalObject<ValueTypeTest>();
            analyzer.Property(p => p.x).HasTransformation(o => o + 1, 45);
            var popRs2 = new PopulationResult(new Dictionary<string, object>()
            {
                ["valuetypetest_x"] = null
            }, analyzer.TypeMap);
            Assert.AreEqual((int)popRs2["valuetypetest_x"], 45);
        }

        [TestMethod]
        public void TransformValueTypeValidTest()
        {
            var analyzer = new AnalyticalObject<ValueTypeTest>();
            analyzer.Property(p => p.x).HasTransformation(o => o + 1);
            var popRs2 = new PopulationResult(new Dictionary<string, object>()
            {
                ["valuetypetest_x"] = 2
            }, analyzer.TypeMap);
            
            Assert.AreEqual((int)popRs2["valuetypetest_x"], 3);
            
        }
        
        [TestMethod]
        public async Task TransformConcatenationTest()
        {

            var analyzer = new AnalyticalObject<ValueTypeTest>();

            analyzer.Property(u => u.y).HasTransformation(o => o + "5");

            PopulationResult rs = new PopulationResult(new Dictionary<string, object>()
            {
                ["valuetypetest_y"] = "4"
            }, analyzer.TypeMap);
            
            Assert.AreEqual(rs["valuetypetest_y"], "45");

        }
    }
}