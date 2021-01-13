using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace SW.FluentOlap.Ingester
{
    /// <summary>
    /// Basic wrappers for sql statements
    /// </summary>
    public static class SqlCommands
    {
        public static async Task<int> RunCommandAsync(this DbConnection con, string commandTxt)
        {
            await con.OpenAsync();
            DbCommand command = con.CreateCommand();
            command.CommandText = commandTxt;
            int rs = await command.ExecuteNonQueryAsync();
            con.Close();
            return rs;
        }

        public static async Task<string> RunCommandGetString(this DbConnection con, string commandTxt, string targetColumn)
        {
            await con.OpenAsync();
            DbCommand command = con.CreateCommand();
            command.CommandText = commandTxt;
            var reader = await command.ExecuteReaderAsync();
            if(reader.RecordsAffected > 1)
            {
                Console.Error.WriteLine("NOTE: More than 1 record gotten from RunCommandGetString");
            }
            string rs = string.Empty;
            while (reader.Read())
            {
                int ordinal = reader.GetOrdinal(targetColumn);
                rs = reader.GetString(ordinal);
            }

            con.Close();
            return rs;

        }
    }
}
