using Microsoft.EntityFrameworkCore;
using SW.FluentOlap.Ingester.Interfaces;
using SW.FluentOlap.Models;
using SW.FluentOlap.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace SW.FluentOlap.Ingester
{

    /// <summary>
    /// Syntax sugar for data ingester construction
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataIngester<T> : DataIngester where T : IDbProvider
    {
        public DataIngester(params object[] constructorArgs) : base((T) Activator.CreateInstance(typeof(T), constructorArgs)) { }
        protected new async Task EnsureConsistency(PopulationResultCollection rs, DbConnection con)
        {
            await base.EnsureConsistency(rs.Sample, con);
        }
        /// <summary>
        /// Insuring that the existing schemas in the DB are ready to be inserted intp
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="con"></param>
        /// <param name="sqlProvider"></param>
        /// <returns></returns>
        protected new async Task EnsureConsistency(PopulationResult rs, DbConnection con)
        {
            await base.EnsureConsistency(rs, con);

        }
        public new async Task InsertIntoDb(PopulationResult rs, DbConnection con)
        {
            await base.InsertIntoDb(rs, con);
        }

        public new async Task InsertIntoDb(PopulationResultCollection rs, DbConnection con)
        {
            await base.InsertIntoDb(rs, con);

        }

    }
    
    /// <summary>
    /// Coordinator class that will use the different db providers to insert data
    /// </summary>
    public class DataIngester
    {

        private readonly IDbProvider provider;
        public DataIngester(IDbProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Taking a sample from the collection and ensuring consistency using it.
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="con"></param>
        /// <param name="sqlProvider"></param>
        /// <returns></returns>
        protected async Task EnsureConsistency(PopulationResultCollection rs, DbConnection con)
        {
            await EnsureConsistency(rs.Sample, con);
        }
        /// <summary>
        /// Insuring that the existing schemas in the DB are ready to be inserted intp
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="con"></param>
        /// <param name="sqlProvider"></param>
        /// <returns></returns>
        protected async Task EnsureConsistency(PopulationResult rs, DbConnection con)
        {
            await provider.EnsureConsistencyTableExists(con);

            string hash = rs.OriginTypeMap.EncodeToBase64();

            if (!await provider.HashesMatch(con, rs.OriginTypeMap.Name, rs.OriginTypeMap))
                await provider.AddOrUpdateConsistencyRecord(con, rs.OriginTypeMap.Name, hash);

        }
        public async Task InsertIntoDb(PopulationResult rs, DbConnection con)
        {
            await EnsureConsistency(rs, con);
            await provider.InsertData(con, rs.OriginTypeMap.Name, rs);
        }

        public async Task InsertIntoDb(PopulationResultCollection rs, DbConnection con)
        {
            await EnsureConsistency(rs, con);
            await provider.InsertData(con, rs.OriginTypeMap.Name, rs);

        }

    }
}
