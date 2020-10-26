using System;
using System.Data;
using System.Threading.Tasks;

namespace SW.FluentOlap.Models
{
    public enum DatabaseEngine
    {
        MySql,
        SqlServer,
        Oracle
    }
    public class DatabaseServiceOptions : IServiceInput
    {
        public DatabaseEngine Database { get; set; }
        public string ChildKey { get; set; }
    }

    public class DatabaseServiceResult : IServiceOutput
    {
        public string KeyPrefix { get; set; }
        public PopulationResult PopulationResult { get; set; }
        public string RawOutput { get; }
    }
    
    public class DatabaseService : Service<DatabaseServiceOptions, DatabaseServiceResult>
    {
        private Func<IDbConnection> connection;
        
        public DatabaseService(string connectionString, DatabaseEngine engine, string name) : base(ServiceType.DatabaseCall, name)
        {
            connection = engine switch
            {
            };
        }

        public override Func<DatabaseServiceOptions, Task<DatabaseServiceResult>> InvokeAsync =>
            options =>
            {
                return null;
            };
    }
}