using SW.FluentOlap.Ingester.Interfaces;
using SW.FluentOlap.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Ingester
{
    public static class SqlTranslator
    {

        public static SqlTypeInformation SqlTypeFromInternalType(InternalType internalType, IDbProvider provider)
        {
            return new SqlTypeInformation
               {
                   SqlType = provider.TypeDictionary[internalType.typeString],
                   IsUnique = false
               };
        }
        
        /// <summary>
        /// Translating Internal Typemaps into an sql type
        /// </summary>
        /// <param name="typeMaps"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IDictionary<string, SqlTypeInformation> SqlMapsFromTypeMaps(TypeMap typeMaps, IDbProvider provider)
        {
            IDictionary<string, SqlTypeInformation> sqlMaps = new Dictionary<string, SqlTypeInformation>();
            foreach (var map in typeMaps)
            {
                sqlMaps[map.Key] = SqlTypeFromInternalType(map.Value.InternalType, provider);
            }
            return sqlMaps;
        }

    }
}
