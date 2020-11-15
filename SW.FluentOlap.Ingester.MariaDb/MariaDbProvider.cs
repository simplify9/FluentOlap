using Microsoft.EntityFrameworkCore;
using SW.FluentOlap.Ingester.Interfaces;
using SW.FluentOlap.Models;
using SW.FluentOlap.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SW.FluentOlap.Ingester.MariaDb
{
    public enum MariaDbTableEngine
    {
        InnoDB,
        ColumnStore,
    }
    public class MariaDbProvider : IDbProvider
    {
        private readonly MariaDbTableEngine engine;
        public MariaDbProvider(MariaDbTableEngine engine)
        {
            this.engine = engine;
        }
        private readonly Dictionary<string, string> translatedTypes = new Dictionary<string, string>()
        {
            ["STRING"] = "TEXT",
            ["INTEGER"] = "INT",
            ["FLOAT"] = "FLOAT",
            ["BOOLEAN"] = "BOOLEAN",
            ["DATETIME"] = "DATETIME"
        };

        public IReadOnlyDictionary<string, string> TypeDictionary => translatedTypes;
        
        public async Task AddOrUpdateConsistencyRecord(DbConnection con, string tableName, string hash)
        {
            string command = $"INSERT INTO AnalyzedModelHashes (TableName, Hash) VALUES ('{tableName}', '{hash}');";
            await con.RunCommandAsync(command);
        }

        public async Task EnsureConsistencyTableExists(DbConnection con)
        {
            string sqlCreate = "CREATE TABLE if not exists AnalyzedModelHashes(\n" +
                               "TableName VARCHAR(40) primary key,\n" +
                               "Hash text\n" +
                               ") ENGINE=InnoDb";

            await con.RunCommandAsync(sqlCreate);
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

            string sqlCreate = $"CREATE TABLE {tableName} (\n";

            foreach (var map in sqlMaps)
                sqlCreate += Column(map);

            sqlCreate = sqlCreate.Substring(0, sqlCreate.Length - 2) + "\n)";
            sqlCreate += $" ENGINE={engine}";

            await con.RunCommandAsync(sqlCreate);
        }

        public async Task<bool> HashesMatch(DbConnection con, string tableName, TypeMap typeMap)
        {
            const string HASH_COLUMN = "Hash";

            string getHashRecord = $"select {HASH_COLUMN} from AnalyzedModelHashes where TableName='{tableName}'";

            string rs = await con.RunCommandGetString(getHashRecord, HASH_COLUMN);

            string hash = Hashing.HashTypeMaps(typeMap);
            if (rs == hash)
            {
                return true;
            }
            else if (rs == string.Empty)
            {
                await CreateTableFromTypeMap(con, tableName, typeMap);
                return false;
            }
            else
            {
                //ALTER TABLE IF ADD COLUMN
                //EXCEPTION IF CHANGE COLUMN TYPE
                return false;
            }


        }

        public async Task InsertData(DbConnection con, string tableName, PopulationResult populationResult)
        {
            string columns = string.Join(',', populationResult.Keys);

            IList<string> tmpValues = new List<string>();
            foreach (object value in populationResult.Values)
                tmpValues.Add($"'{value.ToString()}'");
            string values = string.Join(',', tmpValues);

            string insert = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            await con.RunCommandAsync(insert);
        }

        //TODO: Support true bulk
        public async Task InsertData(DbConnection ctx, string tableName, PopulationResultCollection populationResult)
        {
            foreach(PopulationResult rs in populationResult)
            {
                await InsertData(ctx, tableName, populationResult);
            }
        }
    }

}
