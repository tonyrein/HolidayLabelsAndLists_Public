using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SQLite;
using System.IO;
using System.Resources;
using System.Text;

namespace DAO
{
    /// <summary>
    /// Used to initialize the database on app startup.
    /// </summary>
    public static class DBUtils
    {
        private const string DB_FILENAME = "ValleyLabelsAndLists.sqlite";
        private const string DB_METADATA = @"res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl";

#if DEBUG
        private static string WorkingDirectory() {  return @"Q:\HolidayLabelsAndLists"; }
#else
        private static string WorkingDirectory() { return AppDomain.CurrentDomain.BaseDirectory; }
#endif
        /// <summary>
        /// Get SQL commands to create database tables by
        /// using table names as keys in resource file.
        /// </summary>
        private static string[] table_names = new string[]
        {
            "archive_bag_label_info",
            "archive_donor",
            "archive_gift_label_info",
            "archive_services_household_enrollment",
            "bag_label_info",
            "donor",
            "gift_label_info",
            "services_household_enrollment"
        };

        /// <summary>
        /// Initial list of donors
        /// </summary>
        private static string[][] donor_info = new string[][]
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


        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionStringWithoutMetadata(), false);
        }


        public static void MakeDB()
        {
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();
                SQLiteTransaction tr = conn.BeginTransaction();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.Transaction = tr;
                    DropTables(cmd);
                    CreateTables(cmd);
                    AddDonors(cmd);
                    tr.Commit();
                }
            }
        }

        private static void DropTables(SQLiteCommand cmd)
        {
            foreach (string table_name in table_names)
            {
                cmd.CommandText = $"DROP TABLE IF EXISTS {table_name}";
                cmd.ExecuteNonQuery();
            }
        }

        private static void CreateTables(SQLiteCommand cmd)
        {
            foreach (string table_name in table_names)
            {
                cmd.CommandText = Properties.Resources.ResourceManager.GetString(table_name);
                cmd.ExecuteNonQuery();
            }
        }

        private static void AddDonors(SQLiteCommand cmd)
        {
            foreach (string[] sa in donor_info)
            {
                cmd.CommandText = $"INSERT INTO donor (code, name) VALUES ('{sa[0]}','{sa[1]}')";
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Delete records from all tables
        /// </summary>
        public static void ClearDB(SQLiteCommand cmd)
        {
            foreach (string table_name in table_names)
            {
                cmd.CommandText = $"DELETE FROM [{table_name}]";
                cmd.ExecuteNonQuery();
            }
            // now reclaim disk space:
            cmd.CommandText = "VACUUM";
            cmd.ExecuteNonQuery();
        }

        public static void RefreshDB()
        {
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();
                SQLiteTransaction tr = conn.BeginTransaction();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.Transaction = tr;
                    ClearDB(cmd);
                    AddDonors(cmd);
                    tr.Commit();
                }
            }
        }
        public static string ConnectionStringWithoutMetadata()
        {
            SQLiteConnectionStringBuilder csb = new SQLiteConnectionStringBuilder();
            // Program must be run in a folder where user has write permission!
            csb.DataSource = Path.Combine(WorkingDirectory(), DBUtils.DB_FILENAME);
            csb.FailIfMissing = false;
            csb.Version = 3;
            return csb.ToString();
        }

        /// <summary>
        /// Add metadata to connection string for use by EntityFramework
        /// </summary>
        /// <returns></returns>
        public static string FullConnectionString()
        {
            var ecsBuilder = new EntityConnectionStringBuilder();
            ecsBuilder.Metadata = DBUtils.DB_METADATA;
            ecsBuilder.ProviderConnectionString = ConnectionStringWithoutMetadata();
            ecsBuilder.Provider = "System.Data.SQLite.EF6";
            return ecsBuilder.ToString();
        }
    }
}
