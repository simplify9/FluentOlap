using Microsoft.EntityFrameworkCore;
using SW.FluentOlap.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SW.FluentOlap.Ingester.Interfaces
{
    /// <summary>
    /// Database providers like MariaDB or Clickhouse needs to implement this interface to be accepted.
    /// </summary>
    public interface IDbProvider
    {
        public IReadOnlyDictionary<string, string> TypeDictionary { get; }
        public Task EnsureModelTableExists(DbConnection ctx);
        public Task CreateTableFromTypeMap(DbConnection ctx, string tableName, TypeMap map);
        public Task AddOrUpdateSchemeRecord(DbConnection ctx, string messageName, string hash);
        public Task<bool> HashesMatch(DbConnection ctx, string messageName, TypeMap map);
        public Task Write(DbConnection ctx, string tableName, PopulationResult populationResult);
        public Task Write(DbConnection ctx, string tableName, PopulationResultCollection populationResult);
    }
}
