using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SQLite;

/// <summary>
/// Abstracts logic of creating a database connection of any one
/// of the types known to this application.
/// </summary>
namespace DAO
{
    public enum DAOBackendTypes
    {
        EXCEL,
        SQLITE
    }
    public static class DBConnectionWrapper
    {
        private static Dictionary<string, string> ConnectionStrings = new Dictionary<string, string>
        {
            {
                DAOBackendTypes.EXCEL.ToString(),
                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;\""
            },
            {
                DAOBackendTypes.SQLITE.ToString(),
                @"data source=E:\devel\WinForms\ValleyLabelsAndLists\ValleyLabelsAndLists.sqlite;version=3;foreign_keys = ON;"
            }
        };

        public static DbConnection CreateConnection(DAOBackendTypes dbtype, string database_name = "")
        {
            DbConnection con = null;
            string connstring = ConnectionStrings[dbtype.ToString()];
            if (!string.IsNullOrWhiteSpace(database_name))
                connstring = string.Format(connstring, database_name);
            switch (dbtype)
            {
                case DAOBackendTypes.EXCEL:
                    con = new OleDbConnection(connstring);
                    break;
                case DAOBackendTypes.SQLITE:
                    con = new SQLiteConnection(connstring);
                    break;
                default:
                    throw new System.ArgumentException("Database type " + dbtype.ToString() + " is not suported.");
            }
            return con;
        }
        public static System.Data.Common.DataAdapter CreateDataAdapter(DbConnection con)
        {
            DataAdapter da = null;
            // GetFactory(con) returns null for OleDb connection, so first
            // check for that case:
            if (con is OleDbConnection)
            {
                da = new OleDbDataAdapter();
            }
            else
            {
                da = DbProviderFactories.GetFactory(con).CreateDataAdapter();
            }
            return da;
        }

        public static long LastInsertId(DbConnection con)
        {
            long res = -1L;
            if (con is SQLiteConnection)
            {
                res = ((SQLiteConnection)con).LastInsertRowId;
            }
            return res;
        }
    }

    
} // end namespace
