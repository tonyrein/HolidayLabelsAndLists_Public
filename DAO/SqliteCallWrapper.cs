using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace DAO
{
    public static class SqliteCallWrapper
    {
        /// <summary>
        /// Open a SQLite connection and use it to execute
        /// the desired query. Return the results in
        /// a System.Data.DataTable
        /// 
        /// </summary>
        /// <param name="sql">
        ///     SQL string describing query
        /// </param>
        /// <param name="connection_string">
        ///     string used to open the connection
        /// </param>
        /// <param name="arguments">
        ///     parameters to be passed to the query
        /// </param>
        /// <returns>
        ///     a DataTable containing the results
        /// </returns>
        public static System.Data.DataTable GetDataTable(string sql, string connection_string,
            List<Tuple<string,object>> arguments)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(connection_string))
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                foreach(Tuple<string,object> t in arguments)
                {
                    cmd.Parameters.AddWithValue(t.Item1, t.Item2);
                }
                conn.Open();
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }
        public static long Insert(string sql, string connection_string,
            List<Tuple<string, object>> arguments)
        {
            long retValue = -1L;
            using (SQLiteConnection conn = new SQLiteConnection(connection_string))
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                foreach (Tuple<string, object> t in arguments)
                {
                    cmd.Parameters.AddWithValue(t.Item1, t.Item2);
                }
                conn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retValue = conn.LastInsertRowId;
                }
            }
            return retValue;
        }
    }
}
