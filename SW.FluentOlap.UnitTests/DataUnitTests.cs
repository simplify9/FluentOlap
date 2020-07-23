using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.FluentOlap.Models;
using SW.FluentOlap.Utilities;
using UtilityUnitTests.Data;
using UtilityUnitTests.Models;
using UtilityUnitTests.Utilities;

namespace UtilityUnitTests
{
    [TestClass]
    public class DataUnitTests
    {
        [TestMethod]
        public void HashTest()
        {
            string hash = Hashing.HashTypeMaps(TestTypeMaps.P1TypeMap);
            Assert.AreEqual(hash, Hashing.HashTypeMaps(TestTypeMaps.P1TypeMap));
        }

        [TestMethod]
        public void AnalyticalObjectSimpleTest()
        {
            var analyzed = new Parcel1LevelAnalyzer();
            var defaultInitAnalyzed = new DefaultInitParcel1LevelAnalyzer();
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.P1TypeMap);
            var defaultInitHash = Hashing.HashTypeMaps(defaultInitAnalyzed.TypeMap);

            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
            Assert.AreEqual(analyzedHash, defaultInitHash);
        }

        [TestMethod]
        public void AnalyticalObjectComplexText()
        {
            var analyzed = new Parcel2LevelAnalyzer();
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.P2TypeMap);
            foreach(string key in analyzed.TypeMap.Keys)
                Assert.IsTrue(TestTypeMaps.P2TypeMap.ContainsKey(key));
            Assert.AreEqual(analyzedHash, analyzedCurrentHash);

        }

        [TestMethod]
        public void ExternalAnalyticalObjectTest()
        {
            // merge dictionaries (like GetFromService)
            foreach(var entry in TestTypeMaps.P2TypeMap)
                TestTypeMaps.P3TypeMap.Add(entry);

            var analyzed = new Parcel3LevelAnalyzer();
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.P3TypeMap);
            DictionaryAssert.KeysMatch(analyzed.TypeMap, TestTypeMaps.P3TypeMap);

            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
        }

        [TestMethod]
        public void IgnoreTest()
        {
            var analyzed = new Parcel2LevelAnalyzer();
            analyzed.Ignore(p => p.Count);
            analyzed.Ignore(p => p.Shipper);
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);

            foreach(var pair in TestTypeMaps.P2TypeMap)
                if (pair.Key.Contains("shipper"))
                    TestTypeMaps.P2TypeMap.Remove(pair);
            TestTypeMaps.P2TypeMap.Remove("parcel2level_count");

            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.P2TypeMap);

            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
            
        }


    }
}
