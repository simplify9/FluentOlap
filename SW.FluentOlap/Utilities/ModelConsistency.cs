using SW.FluentOlap.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using SW.DeeBee;
using SW.FluentOlap.Domain;
using SW.PrimitiveTypes;
using System.Threading.Tasks;
using SW.FluentOlap.Models;

namespace SW.FluentOlap.Utilities
{
    internal static class ModelConsistency
    {
        public static async Task<bool> CheckModelConsistency(DbConnection con, string messageName, TypeMap typeMap)
        {
            con.Open();
            //check if model consistency table exists, if not create it and return false

            var rs = await con.One<AnalyzedModelHashes>("AnalyzedModelHashes", messageName);
            if(rs.Hash == Hashing.HashTypeMaps(typeMap)) return true;
            return false;

        }
    }
}
