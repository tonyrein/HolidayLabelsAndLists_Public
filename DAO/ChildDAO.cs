using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;

namespace DAO
{
    class ChildDAO_SQLite : IChildDAO
    {
        private string _database_name;
        private string _table_name;
        public ChildDAO_SQLite(string database_name = "", string table_name = "child")
        {
            this._database_name = database_name;
            this._table_name = table_name;
        }
        public int AddMultipleChildren(List<Child> children)
        {
            throw new NotImplementedException();
        }

        public long AddSingleChild(Child c)
        {
            throw new NotImplementedException();
        }

        public List<Child> GetChildren(long id = -1, string first_name = "", string vesta_id = "", Gender gender = Gender.NotSpecified)
        {
            // List to use to return results
            List<Child> retList = new List<Child>();
            // Basic select string (without any WHERE clause)
            string sql = string.Format("SELECT [id],[name],[vesta_id],[gender] FROM {0}", this._table_name);
            // If we are filtering by id, name, etc. then
            // we need a list of arguments to pass to the
            // actual database code:
            List<ArgumentInfo> arguments = new List<ArgumentInfo>();
            // Add WHERE clause if applicable:
            bool using_params = false;
            string where_clause = " WHERE";
            if (id != -1L)
            {
                using_params = true;
                where_clause += " ([id]=@id) AND";
                arguments.Add(new ArgumentInfo("@id", id));
            }
            if (first_name != null)
            {
                using_params = true;
                where_clause += " ([name]=@name) AND";
                arguments.Add(new ArgumentInfo("@name", first_name));
            }
            if (vesta_id != null)
            {
                using_params = true;
                where_clause += " ([vesta_id]=@vesta_id) AND";
                arguments.Add(new ArgumentInfo("@vesta_id", vesta_id));
            }
            if (gender != Gender.NotSpecified)
            {
                using_params = true;
                where_clause += " ([gender]=@gender) AND";
                arguments.Add(new ArgumentInfo("@gender", gender.ToString()));
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
            // Make a Child object from each returned row:
            foreach (DataRow row in dt.Rows)
            {
                Gender g = (Gender)Enum.Parse(typeof(Gender), (string)row["gender"]);
                Child d = new Child(
                    (long)row["id"],
                    (string)row["name"],
                    (string)row["vesta_id"],
                    g
                    );
                retList.Add(d);
            }
            return retList;
        }
    }
}
