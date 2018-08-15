using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public static class DbBackupManager
    {


        /// <summary>
        /// Returns a list of currently-existing
        /// backup files, sorted in alphabetical
        /// order -- this should be the same as
        /// numerical order for these files, and
        /// should also be oldest to most recent.
        /// </summary>
        /// <returns></returns>
        public static List<string> ListBackupFiles()
        {
            List<string> retList = new List<string>();
            retList.Sort();
            return retList;
        }

        /// <summary>
        /// Frees up a space by deleting old backup(s) if
        /// neccessary and renaming remaining backups.
        /// Returns available backup name
        /// </summary>
        /// <returns></returns>
        public static string MakeSlot()
        {
            string retString = "";
            return retString;
        }

        /// <summary>
        /// Deletes zero or more old backup files as
        /// needed to reduce number of backups to
        /// Max - 1.
        /// 
        /// Returns number of files deleted.
        /// </summary>
        /// <returns></returns>
        public static int DeleteOldBackups()
        {
            int retInt = 0;
            return retInt;
        }


        /// <summary>
        /// 
        /// Deletes files as needed to free up
        /// the first slot in the backup file name space.
        /// 
        /// If a file with the first slot name exists,
        /// it will be deleted.
        /// 
        /// If that file does not exist, BUT the number
        /// of backups is >= to max allowed, the oldest
        /// file(s) will be deleted to bring the number
        /// down to max allowed - 1.

        /// </summary>
        /// <returns></returns>
        public static int DeleteOldest()
        {
            int retInt = 0;
            //retName = $"{basename}.bak{i,0:0000}{ext}";
            List<string> list_of_backups = ListBackupFiles();
            //int max_allowed = Properties.Settings
            return retInt;
        }

        /// <summary>
        /// Get list of backup files, sorted in order
        /// of age, from oldest to youngest.
        /// 
        /// Then, start at the oldest and rename each
        /// one to the next backup name, starting
        /// with dbfilename.litedb.bak001.
        /// 
        /// To ensure dbfilename.litedb.bak001 is free,
        /// call DeleteOldBackups().
        /// </summary>
        public static void DownshiftBackupNames()
        {
            DeleteOldBackups();

        }


    }

}
