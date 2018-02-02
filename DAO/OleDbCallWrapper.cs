using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace DAO
{
    public static class OleDbCallWrapper
    {
        /// <summary>
        /// Open an OleDb connection and use it to execute
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
        public static DataTable GetDataTable(string sql, string connection_string,
            List<Tuple<string, object>> arguments)
        {
            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connection_string))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                foreach (Tuple<string, object> t in arguments)
                {
                    cmd.Parameters.AddWithValue(t.Item1, t.Item2);
                }
                conn.Open();
                using (OleDbDataAdapter da = new OleDbDataAdapter())
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
            long retValue;
            using (OleDbConnection conn = new OleDbConnection(connection_string))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                foreach (Tuple<string, object> t in arguments)
                {
                    cmd.Parameters.AddWithValue(t.Item1, t.Item2);
                }
                conn.Open();
                // ExecuteNonQuery() returns number of rows affected
                retValue = (long)cmd.ExecuteNonQuery();
            }
            return retValue;
        }
    }
}
