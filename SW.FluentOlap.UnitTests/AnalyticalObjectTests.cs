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
    public class AnalyticalObjectTests
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
            
            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.P2TypeMap, new DifferenceType[]
            {
                DifferenceType.ChangedColumnOrder
            });
            
            Assert.AreEqual(analyzedHash, analyzedCurrentHash);

        }

        [TestMethod]
        public void ExternalAnalyticalObjectTest()
        {
            // merge dictionaries (like GetFromService)
            foreach(var entry in TestTypeMaps.P2TypeMap)
                TestTypeMaps.P3TypeMap.Add(new KeyValuePair<string, NodeProperties>("referencetoparcel2level" + "_" + entry.Key, entry.Value));

            var analyzed = new Parcel3LevelAnalyzer();
            analyzed.Property(p => p.ReferenceToParcel2Level).GetFromService("SomeService", new Parcel2LevelAnalyzer());
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.P3TypeMap);
            DictionaryAssert.KeysMatch(analyzed.TypeMap, TestTypeMaps.P3TypeMap);

            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.P3TypeMap);

            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
        }
        
        [TestMethod]
        public void ExternalAnalyticalObjectTestNoReference()
        {
            // merge dictionaries (like GetFromService)
            foreach(var entry in TestTypeMaps.P2TypeMap)
                TestTypeMaps.P3TypeMapNoRef.Add(new KeyValuePair<string, NodeProperties>("referencetoparcel2level" + "_" + entry.Key, entry.Value));

            var analyzed = new Parcel3LevelAnalyzer();
            analyzed.Property("referencetoparcel2level", new AnalyticalObject<Parcel2Level>()).GetFromService("SomeService", new Parcel2LevelAnalyzer());
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.P3TypeMapNoRef);

            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.P3TypeMapNoRef, new List<DifferenceType>()
                {
                    DifferenceType.ChangedColumnOrder
                });

            DictionaryAssert.KeysMatch(analyzed.TypeMap, TestTypeMaps.P3TypeMapNoRef);
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
        public void DeepIgnoreTest()
        {
            var analyzed = new IgnoreMapAnalyzer();
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.IgnoreTestMap);
            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.IgnoreTestMap, new List<DifferenceType>
            {
                DifferenceType.ChangedColumnOrder
            });
            Assert.IsTrue(!differences.Any());
            
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
            var analyzed = new WideParcelSelfReferenceDeep();
            var analyzedHash = Hashing.HashTypeMaps(analyzed.TypeMap);
            var analyzedCurrentHash = Hashing.HashTypeMaps(TestTypeMaps.SelfReferenceTestDeep);
            TypeMapDifferences differences = new TypeMapDifferences(analyzed.TypeMap, TestTypeMaps.SelfReferenceTest, new List<DifferenceType>
            {
                DifferenceType.ChangedColumnOrder
            });

            var test = string.Empty;
            test += analyzed.TypeMap.ToString() + "\n\n\n\n\n\n\n\n\n";
            test += TestTypeMaps.SelfReferenceTest.ToString();
            
            Assert.AreEqual(analyzedHash, analyzedCurrentHash);
        }


    }
}
