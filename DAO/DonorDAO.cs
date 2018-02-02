using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SQLite;
using DTO;

// TODO: Refactor common code in
// DonorDAO_xxx classes into a base class
//
namespace DAO
{
    public class DonorDAO_Excel : IDonorDAO
    {
        private string _database_name;
        private string _table_name;
        public DonorDAO_Excel(string database_name, string table_name="Sheet1$")
        {
            this._database_name = database_name;
            this._table_name = table_name;
        }
        public long AddSingleDonor(Donor d)
        {
            throw new NotImplementedException("DonorDAO_Excel class is Read-Only.");
        }

        public int AddMultipleDonors(List<Donor> donors)
        {
            throw new NotImplementedException("DonorDAO_Excel class is Read-Only.");
        }

        public List<Donor> GetDonors(long donorid = -1L, string name = null, string code = null)
        {
            String sql = String.Format("SELECT * FROM [{0}]", _table_name);
            Console.WriteLine(sql);
            List<Donor> retList = new List<Donor>();
            // If we are filtering by id, name, etc. then
            // we need a list of arguments to pass to the
            // actual database code:
            List<ArgumentInfo> arguments = new List<ArgumentInfo>();
            // Add WHERE clause if applicable:
            bool using_params = false;
            string where_clause = " WHERE";
            if (name != null)
            {
                using_params = true;
                where_clause += " ([name]=@name) AND";
                arguments.Add(new ArgumentInfo("@name", name));
            }
            if (code != null)
            {
                using_params = true;
                where_clause += " ([code]=@code) AND";
                arguments.Add(new ArgumentInfo("@code", code));
            }
            if (using_params)
            {
                where_clause = where_clause.Remove(where_clause.Length - (" AND".Length));
                sql += where_clause;
            }
            DataTable dt = null;
            using (DbConnection con = DBConnectionWrapper.CreateConnection(DAOBackendTypes.EXCEL, this._database_name))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DBCallWrapper wrapper = new DBCallWrapper(con);
                dt = wrapper.GetTable(sql, arguments);
            }
            // We can't assume that the column headers in the Excel file are exactly "name" and
            // "code." Therefore, iterate over the column names and pick the ones with
            // "name" and "code" contained in their names.
            DataColumn name_col = null;
            DataColumn code_col = null;
            foreach (DataColumn col in dt.Columns)
            {
                string cap = col.Caption.ToLower();
                if (cap.Contains("name"))
                {
                    name_col = col;
                }
                else if (cap.Contains("code"))
                {
                    code_col = col;
                }
            }
            // Iterate over rows and make a Donor object from each.
            foreach(DataRow row in dt.Rows)
            {
                Donor d = new Donor(
                    null,
                    (string)row[name_col],
                    (string)row[code_col]
                    );
                retList.Add(d);
            }
            return retList;
        }
    }

    public class DonorDAO_SQLite : IDonorDAO
    {
        private string _database_name;
        private string _table_name;
        public DonorDAO_SQLite(string database_name="", string table_name="donor")
        {
            this._database_name = database_name;
            this._table_name = table_name;
        }


        public long AddSingleDonor(Donor d)
        {
            // Does this Donor already exist in the database?
            if (d.Id != null) // If there's an id, the donor already exists.
                return -1L;
            // Check for name:
            if (GetDonors(name: d.Name).Count > 0)
                return -1L;
            string sql = string.Format("INSERT INTO {0} (name, code) VALUES (@name, @code)", this._table_name);
            List<ArgumentInfo> arguments = new List<ArgumentInfo>();
            arguments.Add(new ArgumentInfo("@name", d.Name));
            arguments.Add(new ArgumentInfo("@code", d.Code));
            long res = -1L;
            using (DbConnection con = DBConnectionWrapper.CreateConnection(DAOBackendTypes.SQLITE, this._database_name))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DBCallWrapper wrapper = new DBCallWrapper(con);
                res = wrapper.Insert(sql, arguments);
            }
            return res;
        }

        public int AddMultipleDonors(List<Donor> donors)
        {
            int res = 0;
            string sql = string.Format("INSERT INTO {0} (name, code) VALUES (@name, @code)", this._table_name);
            List<List<ArgumentInfo>> list_of_lists = new List<List<ArgumentInfo>>();
            foreach(Donor d in donors)
            {
                List<ArgumentInfo> arglist = new List<ArgumentInfo>();
                arglist.Add(new ArgumentInfo("@name", d.Name));
                arglist.Add(new ArgumentInfo("@code", d.Code));
                list_of_lists.Add(arglist);
            }
            using (DbConnection con = DBConnectionWrapper.CreateConnection(DAOBackendTypes.SQLITE, this._database_name))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DBCallWrapper wrapper = new DBCallWrapper(con);
                res = wrapper.InsertMultiple(sql, list_of_lists);
            }
            return res;
        }

        public List<Donor> GetDonors(long donorid=-1L, string name=null, string code=null)
        {
            // List to use to return results
            List<Donor> retList = new List<Donor>();
            // Basic select string (without any WHERE clause)
            string sql = string.Format("SELECT [id],[name],[code] FROM {0}", this._table_name);
            // If we are filtering by id, name, etc. then
            // we need a list of arguments to pass to the
            // actual database code:
            List<ArgumentInfo> arguments = new List<ArgumentInfo>();
            // Add WHERE clause if applicable:
            bool using_params = false;
            string where_clause = " WHERE";
            if (donorid != -1L)
            {
                using_params = true;
                where_clause += " ([id]=@id) AND";
                arguments.Add(new ArgumentInfo("@id", donorid));
            }
            if (name != null)
            {
                using_params = true;
                where_clause += " ([name]=@name) AND";
                arguments.Add(new ArgumentInfo("@name", name));
            }
            if (code != null)
            {
                using_params = true;
                where_clause += " ([code]=@code) AND";
                arguments.Add(new ArgumentInfo("@code", code));
            }
            if (using_params)
            {
                where_clause = where_clause.Remove(where_clause.Length - (" AND".Length));
                sql += where_clause;
            }
            // Call database code to get a table:
            DataTable dt = null;
            using (DbConnection con = DBConnectionWrapper.CreateConnection(DAOBackendTypes.SQLITE, this._database_name))
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DBCallWrapper wrapper = new DBCallWrapper(con);
                dt = wrapper.GetTable(sql, arguments);
            }
            // Make a Donor object from each returned row:
            foreach (DataRow row in dt.Rows)
            {
                Donor d = new Donor(
                    (long)row["id"],
                    (string)row["name"],
                    (string)row["code"]
                    );
                retList.Add(d);
            }
            return retList;
        }
    }
}
