using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Text;

namespace DAO
{
    /// <summary>
    /// A class to be used by DBWrapper in order to
    /// de-clutter DBWrapper
    /// </summary>
    public static class DBWrapperHelper
    {
        public static SQLiteConnection MakeConnection()
        {
            SQLiteConnection conn = new SQLiteConnection(_build_connection_string());
            InitConnection(conn);
            return conn;
        }
        private static string _build_connection_string()
        {
            SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder();
            csb.DataSource = ":memory:";
            csb.FailIfMissing = false;
            csb.Version = 3;
            return csb.ToString();
        }

        public static void InitConnection(SQLiteConnection conn)
        {
            conn.Open();
            SQLiteTransaction tr = conn.BeginTransaction();
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.Transaction = tr;
                _create_tables(cmd);
                _init_donors(cmd);
                tr.Commit();
            }
        }

        private static void _create_tables(SQLiteCommand cmd)
        {
            foreach (string table_name in table_names)
            {
                cmd.CommandText = Properties.Resources.ResourceManager.GetString(table_name);
                if (cmd.CommandText != null)
                    cmd.ExecuteNonQuery();
            }
        }

        private static void _init_donors(SQLiteCommand cmd)
        {
            foreach (string[] sa in donor_info)
            {
                cmd.CommandText = $"INSERT INTO donor (code, name) VALUES ('{sa[0]}','{sa[1]}')";
                cmd.ExecuteNonQuery();
            }
        }

        public static List<NameValueCollection> ListAll0(SQLiteConnection conn, string table_name, string order_by="")
        {
            List<NameValueCollection> retList = new List<NameValueCollection>();
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"SELECT * FROM '{table_name}'";
                if (order_by != "")
                    cmd.CommandText += $" ORDER BY '{order_by}'";
                SQLiteDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    retList.Add(reader.GetValues());
                }
            }
            return retList;
        }

        public static List<NameValueCollection> ListAll(SQLiteConnection conn, string table_name, string order_by="")
        {
            return ListWhere(conn, table_name, where_args: null, order_by: order_by);
        }

        public static List<NameValueCollection> ListWhere(SQLiteConnection conn, string table_name, 
            KeyValuePair<string, string>[] where_args=null, string order_by = "")
        {
            List<NameValueCollection> retList = new List<NameValueCollection>();
            using (SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"SELECT * FROM '{table_name}'";
                cmd.CommandText += _build_where_string(where_args);
                if (order_by != "")
                    cmd.CommandText += $" ORDER BY [{order_by}]";
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retList.Add(reader.GetValues());
                }
            }
            return retList;
        }

        private static string _build_where_string(KeyValuePair<string,string>[] where_args)
        {
            StringBuilder sb = new StringBuilder();
            if (where_args != null && where_args.Length > 0)
            {
                sb.Append(" WHERE ");
                string k = where_args[0].Key;
                string v = where_args[0].Value;
                sb.Append($"[{k}]='{v}'");
                for (int i = 1; i < where_args.Length; i++)
                {
                    k = where_args[i].Key;
                    v = where_args[i].Value;
                    sb.Append($" AND [{k}]='{v}'");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Table names used as keys in a resource file.
        /// The values from the resource file are the SQL
        /// statements to create the tables.
        /// </summary>
        private static string[] table_names = new string[]
        {
            "bag_label_info",
            "donor",
            "gift_label_info",
            "services_household_enrollment"
        };

        /// <summary>
        /// Initial list of donors
        /// </summary>
        public static string[][] donor_info = new string[][]
        {
            new string[] {"AHT","Church of Ascension and Holy Trinity"},
            new string[] {"Bethany Baar","Junior women club" },
            new string[] {"Bethany_Baar","Bethany_Baar"},
            new string[] {"CCG","Christ Church of Glendale" },
            new string[] {"ERP","ERP Suites"},
            new string[] {"ERP Suites","Eric P"},
            new string[] {"Erp_Suites","Erp_Suites"},
            new string[] {"FPG","First Presbyterian Church of Glendale" },
            new string[] {"FRUCC","Fleming Rd United Church of Christ" },
            new string[] {"FUMC","Friendship United Methodist Church" },
            new string[] {"GNH","The Gathering at Northern Hills" },
            new string[] {"HMC","Hartwell Methodist Church" },
            new string[] {"JK","Jessica Kaylor" },
            new string[] {"JWC","Junior Women Club" },
            new string[] {"KJ","Kathy Jurell" },
            new string[] {"LC","Landmark Church" },
            new string[] {"LCC","Lockland Christian Church" },
            new string[] {"LCN","Lockland Church of the Nazarene" },
            new string[] {"LKF","Laura K. Fidler" },
            new string[] {"LPF","Laura Fidler" },
            new string[] {"LY","Lauren Young" },
            new string[] {"MAL","Mike Albert Leasing" },
            new string[] {"ME","Margaret Eldredge" },
            new string[] {"MZB","Mt. Zion Baptist Church" },
            new string[] {"OLSH","Our Lady of Sacred Heart" },
            new string[] {"PCW","Presbyterian Church of Wyoming" },
            new string[] {"PP","Process Plus" },
            new string[] {"RB","Rivertown Brewery" },
            new string[] {"Robb_M","Robb M" },
            new string[] {"SJUCC","St. John United Church of Christ" },
            new string[] {"SJV","St. James of the Valley" },
            new string[] {"SMC","St. Michaels Church" },
            new string[] {"VB","Valarie Barrett" },
            new string[] {"VV","Valley Volunteers" },
            new string[] {"WN","Wyoming Newcomers" }
        };


    }
}
