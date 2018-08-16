using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using LiteDB;

namespace DAO
{
    public static class DbUtils
    {
        private static string GetDatabaseFilename()
        {
            string dbdir = FolderManager.DbFolder();
            if (!Directory.Exists(dbdir))
                Directory.CreateDirectory(dbdir);
            return Path.Combine(dbdir, Properties.Resources.db_filename);
        }

        /// <summary>
        /// Create a connection to the database
        /// used for persistent storage.
        /// </summary>
        /// <returns></returns>
        public static LiteDatabase GetDatabase()
        {
            return new LiteDatabase(DbUtils.GetDatabaseFilename());
        }

        /// <summary>
        /// Returns a Queue of the backup files
        /// with the oldest first in line.
        /// </summary>
        /// <returns></returns>
        private static Queue<FileInfo> GetBackupQueue()
        {
            List<FileInfo> unsorted = new List<FileInfo>();
            string glob = Properties.Resources.db_backup_glob;
            string[] sa = Directory.GetFiles(FolderManager.DbFolder(), glob).ToArray();
            foreach(string s in sa)
            {
                unsorted.Add(new FileInfo(s));
            }
            List<FileInfo> sorted = unsorted.OrderBy(f => f.CreationTimeUtc).ToList();
            return new Queue<FileInfo>(sorted);
        }

        private static int NumberFromBackupFilename(string fn)
        {
            // filename is, for example:
            //    P:\HLL_INTERNAL\Database\hll_data.bak0012.litedb
            // We want the 0012 portion, parsed to 12.
            string basename = Path.Combine(Path.GetDirectoryName(fn),
                Path.GetFileNameWithoutExtension(fn));
            int retInt;
            if (!int.TryParse(basename.Substring(basename.Length - 4), out retInt))
                retInt = -1;
            return retInt;
        }
            /// <summary>
            /// Derive the next available backup file number.
            /// 
            /// Backup files are named <db base filename>.bak0000.ext, <db base filename>.back0001.ext, and
            /// so on. We retain up to (Properties.Settings.Default.MaxDBBackups).
            /// 
            /// Return a number that's one more than the max of the numerical portions of our
            /// backup filenames. If we get to 999, start over at 0. (This assumes that
            /// the max allowable db backups is < 999).
            /// </summary>
            /// <param name="Q"></param>
            /// <returns></returns>
            private static int NextFileNum(Queue<FileInfo> Q)
            {
            int retInt = 0;
            if (Q.Count() > 0)
            {
                retInt = Q.Max(fi => NumberFromBackupFilename(fi.FullName)) + 1;
            }
            if (retInt > 999)
                retInt = 0;
            return retInt;
        }

        /// <summary>
        /// Dequeue files from the bottom of the queue (oldest end) until
        /// we are down to the max allowable file number.
        /// </summary>
        /// <param name="Q"></param>
        private static void ShrinkBackupFileQueue(Queue<FileInfo> Q)
        {
            int max_files = Properties.Settings.Default.MaxDBBackups;
            while (Q.Count() >= max_files)
            {
                FileInfo fi = Q.Dequeue();
                fi.Delete();
            }
        }

        public static void BackupDatabase()
        {
            Queue<FileInfo> Q = GetBackupQueue();
            ShrinkBackupFileQueue(Q);
            int next_num = NextFileNum(Q);
            Utils.FileUtils.MoveToBackup(GetDatabaseFilename(), start_at: next_num);
        }

    }

}
