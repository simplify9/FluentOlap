using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using SW.FluentOlap.Ingester.Interfaces;
using SW.FluentOlap.Models;

namespace SW.FluentOlap.Ingester.Postgres
{
    public class PostgresProvider : IDbProvider
    {
        private readonly Dictionary<InternalType, string> translatedTypes;
        public IReadOnlyDictionary<InternalType, string> TypeDictionary => translatedTypes;

        private readonly string schemaName;

        public PostgresProvider(string schemaName)
        {
            this.schemaName = schemaName;
        }
        
        public async Task EnsureModelTableExists(DbConnection con)
        {
            string sqlCreate = $"CREATE TABLE if not exists {schemaName}.analyzed_model_hashes(\n" +
                               "table_name VARCHAR (40) PRIMARY KEY,\n" +
                               "hash text\n" +
                               ");";

            await con.RunCommandAsync(sqlCreate);
        }

        public async Task AddOrUpdateSchemeRecord(DbConnection con, string tableName, string hash)
        {
            string command = $"INSERT INTO {schemaName}.analyzed_model_hashes (table_name, hash) VALUES ('{tableName}', '{hash}') " +
                             $"ON CONFLICT (table_name) DO UPDATE " +
                             $"SET hash='{hash}';";
            await con.RunCommandAsync(command);
        }

        //TODO: Support true bulk
        public async Task Write(DbConnection ctx, string tableName, PopulationResultCollection populationResult)
        {
            foreach (PopulationResult rs in populationResult)
            {
                await Write(ctx, tableName, populationResult);
            }
        }

        public async Task Write(DbConnection con, string tableName, PopulationResult populationResult)
        {
            string columns = string.Join(',', populationResult.Keys);

            IList<string> tmpValues = new List<string>();
            foreach (object value in populationResult.Values)
                tmpValues.Add(value != null? $"'{value}'" : "NULL");
            string values = string.Join(',', tmpValues);

            string insert = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            await con.RunCommandAsync(insert);
        }

        public async Task<bool> HashesMatch(DbConnection con, string tableName, TypeMap typeMap)
        {
            const string HASH_COLUMN = "hash";
            const string TABLE_NAME_COLUMN = "table_name";

            string getHashRecord = $"select {HASH_COLUMN} from AnalyzedModelHashes where {TABLE_NAME_COLUMN}='{tableName}'";

            string existingHash = await con.RunCommandGetString(getHashRecord, HASH_COLUMN);

            string newHash = typeMap.EncodeToBase64();
            if (existingHash == newHash)  return true;
            
            if (existingHash == string.Empty)
            {
                await CreateTableFromTypeMap(con, tableName, typeMap);
                return false;
            }
            
            TypeMap existing = TypeMap.DecodeFromBase64(existingHash);
            
            TypeMapDifferences differences = existing.GetDifferences(typeMap);
            foreach (TypeMapDifference difference in differences)
            {
                if (difference.DifferenceType == DifferenceType.DataTypeChange)
                {
                    throw new Exception($"Difference types {nameof(DifferenceType.DataTypeChange)}, " +
                                        $" not supported. \n" +
                                        $"Column:  {difference.ColumnKey}");
                }

                if (difference.DifferenceType == DifferenceType.ChangedColumnOrder)
                {
                    //TODO: Better handling.
                    //ignore for now
                }

                if (difference.DifferenceType == DifferenceType.AddedColumn)
                {
                    NodeProperties node = typeMap[difference.ColumnKey];
                    
                    string alterState = $"ALTER TABLE {typeMap.Name} ADD COLUMN {difference.ColumnKey} {SqlTranslator.SqlTypeFromInternalType(node.InternalType, this).SqlType};";

                    await con.RunCommandAsync(alterState);

                }
                
            }
            return false;
        }

        private string Column(KeyValuePair<string, SqlTypeInformation> map)
        {
            string tmp = string.Empty;
            //TODO: add CONSTRAINTS
            if (map.Value.IsUnique)
                tmp += string.Empty;
            tmp += $"\t{map.Key}\t{map.Value.SqlType},\n";
            return tmp;
        }

        public async Task CreateTableFromTypeMap(DbConnection con, string tableName, TypeMap typeMap)
        {
            var sqlMaps = SqlTranslator.SqlMapsFromTypeMaps(typeMap, this);

            if (typeMap == null || typeMap.Count == 0) throw new Exception("Type Map null or empty");

            string sqlCreate = $"CREATE TABLE {tableName} (\n";

            sqlCreate += "Id BIGINT AUTO_INCREMENT PRIMARY KEY,\n";
                

            foreach (var map in sqlMaps)
                sqlCreate += Column(map);

            sqlCreate = sqlCreate.Substring(0, sqlCreate.Length - 2) + "\n);";

            await con.RunCommandAsync(sqlCreate);
        }
    }
}