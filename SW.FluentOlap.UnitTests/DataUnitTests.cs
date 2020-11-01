using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.FluentOlap.AnalyticalNode;
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
            
            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.P2TypeMap);
            
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

            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.P3TypeMap);

            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
        }

        [TestMethod]
        public void StringifyingTest()
        {
            string p3mapString = TestTypeMaps.P3TypeMap.ToString();
            TypeMap map = TypeMap.FromString(p3mapString);
            Assert.AreEqual(Hashing.HashTypeMaps(map), Hashing.HashTypeMaps(TestTypeMaps.P3TypeMap));
        }


        [TestMethod]
        public void Base64Compression()
        {
            string p3mapbase64 = TestTypeMaps.P3TypeMap.EncodeToBase64();
            TypeMap testmap = TypeMap.DecodeFromBase64(p3mapbase64);
            Assert.AreEqual(Hashing.HashTypeMaps(testmap), Hashing.HashTypeMaps(TestTypeMaps.P3TypeMap));

        }

        [TestMethod]
        public void IgnoreTest()
        {
            var analyzed = new Parcel2LevelAnalyzer();
            analyzed.Ignore(p => p.Shipper);
            analyzed.Ignore(p => p.Shipper2);
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);

            foreach(var pair in TestTypeMaps.P2TypeMap)
                if (pair.Key.Contains("shipper"))
                    TestTypeMaps.P2TypeMap.Remove(pair);
            TestTypeMaps.P2TypeMap.Remove("parcel2level_count");

            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.P2TypeMap);

            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
            
        }

        [TestMethod]
        public void DeepIgnoreTest()
        {
            var analyzed = new IgnoreMapAnalyzer();
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.IgnoreTestMap);
            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.IgnoreTestMap, new List<DifferenceType>
            {
                DifferenceType.ChangedColumnOrder
            });
            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
            
        }

        [TestMethod]
        public void SelfRefTest()
        {
            var analyzed = new WideParcelSelfReference();
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.SelfReferenceTest);
            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.SelfReferenceTest, new List<DifferenceType>
            {
                DifferenceType.ChangedColumnOrder
            });

            var test = string.Empty;
            test += analyzed.TypeMap.ToString() + "\n\n\n\n\n\n\n\n\n";
            test += TestTypeMaps.SelfReferenceTest.ToString();
            
            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
        }

        [TestMethod]
        public void DeepSelfRefTest()
        {
            
        }


    }
}
