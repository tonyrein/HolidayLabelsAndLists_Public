using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DAO
{
    public class ArgumentInfo
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ArgumentInfo(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public class DBCallWrapper
    {
        private DbConnection _con = null;
        private DbCommand _cmd = null;

        public DBCallWrapper(DbConnection con)
        {
            this._con = con;
        }
        
        private void _LoadArguments(List<ArgumentInfo> arguments)
        {
            this._cmd.Parameters.Clear();
            foreach (ArgumentInfo a in arguments)
            {
                DbParameter p = (DbParameter)this._cmd.CreateParameter();
                p.ParameterName = a.Name;
                p.Value = a.Value;
                this._cmd.Parameters.Add(p);
            }
        }

        public DataTable GetTable(string sql, List<ArgumentInfo> arguments)
        {
            DataTable dt = new DataTable();
            using (this._cmd = (DbCommand)this._con.CreateCommand())
            {
                this._cmd.CommandText = sql;
                this._LoadArguments(arguments);
                using (DbDataAdapter da = (DbDataAdapter)DBConnectionWrapper.CreateDataAdapter(this._con))
                {
                    da.SelectCommand = this._cmd;
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public long Insert(string sql, List<ArgumentInfo> arguments)
        {
            long retValue = -1L;
            using (this._cmd = (DbCommand)this._con.CreateCommand())
            {
                this._cmd.CommandText = sql;
                this._LoadArguments(arguments);
                // execute call returns # of rows affected
                if (this._cmd.ExecuteNonQuery() > 0)
                {
                    // use method appropriate to connection type
                    retValue = DBConnectionWrapper.LastInsertId(this._con);
                }
            }
            return retValue;
        }

        /// <summary>
        /// Insert multiple records into the backend database
        /// within one transaction. If insertion of any records
        /// fails, the transaction is rolled back and no 
        /// records are inserted.
        /// </summary>
        /// <param name="sql">
        ///    sql string
        /// </param>
        /// <param name="list_of_lists">
        ///     A List of Lists of ArgumentInfo instances. Each item
        ///     in the list is itself a list of ArgumentInfo that will
        ///     be applied to one record.
        /// </param>
        /// <returns>
        ///     Number of records inserted
        /// </returns>
        public int InsertMultiple(string sql,
            List<List<ArgumentInfo>> list_of_lists)
        {
            int retValue = 0;
            using (this._cmd = (DbCommand)this._con.CreateCommand())
            {

                this._cmd.CommandText = sql;
                DbTransaction transaction = this._con.BeginTransaction();
                try
                {
                    foreach (List<ArgumentInfo> args in list_of_lists)
                    {
                        this._LoadArguments(args);
                        this._cmd.ExecuteNonQuery();
                        retValue++;
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    retValue = 0;
                }
            }
            return retValue;
        }
    }
}
