using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.FluentOlap.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UtilityUnitTests.Data;

namespace UtilityUnitTests.Utilities
{
    public static class DictionaryAssert
    {
        public static void KeysMatch(TypeMap analyzed, TypeMap test)
        {
            foreach(string key in analyzed.Keys)
                Assert.IsTrue(TestTypeMaps.P3TypeMap.ContainsKey(key));
            foreach(string key in TestTypeMaps.P3TypeMap.Keys)
                Assert.IsTrue(analyzed.ContainsKey(key));
        }

    }
}
