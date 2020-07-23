using System;
using System.Collections.Generic;
using System.Text;

namespace SW.FluentOlap.Api.Services
{

    internal class SqlStatementObjects
    {
        public SqlStatementObjects() {}
        public SqlStatementObjects(string sql)
        {
            string[] sqlArray = sql.Split(' ');
            int conditionLocation = sql.IndexOf("where");

            this.Type = sqlArray[0];
            this.Target = sql.Substring(sql.IndexOf("from") + 1, sql.IndexOf(" ", sql.IndexOf("from") + 1));
            this.Condition = conditionLocation != -1 ? sql.Substring(conditionLocation) : "";
            this.Limit = 0; //implement different DB types
            this.Fields = new List<string>();
        }

        public string Type { get; }
        public string Target { get; }
        public IEnumerable<string> Fields { get; }
        public string Condition { get; }
        public int Limit { get; }
    }

    internal class SqlAnalyzer
    {
        readonly SqlStatementObjects sqlObject;
        public SqlAnalyzer(string sql)
        {
            this.sqlObject = new SqlStatementObjects(sql);
        }


    }
}
