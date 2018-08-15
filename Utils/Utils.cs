using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class FileUtils
    {
        /// <summary>
        /// Construct a backup file name. If a file by that name already
        /// exists, construct another backup file name. Keep going until
        /// we find the name of a file that doesn't already exist.
        /// 
        /// Implicity assume there won't be more than 9999 backups needed!
        /// 
        /// </summary>
        /// <param name="orig_name"></param>
        /// <returns></returns>
        public static string NextAvailableBackupName(string orig_name)
        {
            string ext = Path.GetExtension(orig_name);
            string basename = Path.Combine(Path.GetDirectoryName(orig_name),
                Path.GetFileNameWithoutExtension(orig_name));
            string retName;
            int i = 1;
            do
            {
                // construct string such as "e:\original_path\original_filename_bak0003.docx"
                retName = $"{basename}.bak{i,0:0000}{ext}";
                i++;
            }
            while (File.Exists(retName));
            return retName;
        }

        /// <summary>
        /// Derives a backup filename based on the original name
        /// of the file and renames the file with the backup name.
        /// </summary>
        /// <param name="filespec"></param>
        public static void MoveToBackup(string filespec)
        {
            string backup_name = NextAvailableBackupName(filespec);
            File.Move(filespec, backup_name);
        }

        /// <summary>
        /// Copies the files in the given list to the destination..
        /// 
        /// The destination folder must already exist. The UI
        /// layer should take care of such issues as creating
        /// the destination if needed, or permissions errors.
        /// 
        /// Returns the count of files copied.
        /// 
        /// If replace==false and the destination filespec already exists,
        /// an exception will be thrown.
        /// 
        /// If performance becomes an issue, consider replacing the foreach
        /// loop with Parrallel.ForEach logic.
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="dest_folder"></param>
        /// <returns></returns>
        public static int CopyListOfFiles(List<string> filespecs, string dest_folder, bool replace = false)
        {
            int retInt = 0;
            //Dictionary<string, HllFileInfo> d = FilesMatchingFilter(fs);
            foreach (string full_path in filespecs)
            {
                if (File.Exists(full_path))
                {
                    string name_plus_ext = Path.GetFileName(full_path);
                    string dest = Path.Combine(dest_folder, name_plus_ext);
                    File.Copy(full_path, dest, replace);
                    retInt++;
                }
            }
            return retInt;
        }
    }

    public static class TextUtils
    {
        private static string non_alphanum = @"[^0-9a-zA-Z\s]+"; // one or more members of given class
        private static Regex rna = new Regex(non_alphanum);
        private static string multiple_spaces = @"[\s]+";
        private static Regex rms = new Regex(multiple_spaces);
        private static string us_zip_start = @"^[0-9]{5}.*"; // string starting with 5 digits
        private static Regex rzip = new Regex(us_zip_start);
        /// <summary>
        /// 
        /// Transform strings into format suitable for file names
        /// 1. Trim leading and trailing whitespace
        /// 2. Remove all periods
        /// 3. Replace each non-alphanumeric character or sequence of
        ///       non-alphanumeric characters with a single space.
        /// 4. Apply "proper" case (ie, capitalize first letter of each word)
        /// 5. Replace sequences of one or more spaces with one underscore
        /// (We need do do step #2 before step #3, because "proper" case depends
        ///   on spaces to find word boundaries. Another option would be to use a
        ///   slightly more complex regex.)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CleanString(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                s = s.Trim();
                s = s.Replace(".", " ");
                s = rna.Replace(s, " ");
                s = ProperCase(s);
                return rms.Replace(s, "_");
            }
            else
                return "";
        }

        /// <summary>
        /// Convert string to Title Case, also known
        /// as Proper Case. For example:
        ///    "alice in wonderland" => "Alice In Wonderland"
        ///    "war and peace" => "War And Peace"
        ///    
        /// TODO: The words "And," "In," "The" and a few others
        /// should not be in upper case in English titles, unless they
        /// are the first word.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lang_name"></param>
        /// <returns></returns>
        public static string ProperCase(string text, string lang_name = "en-US")
        {
            TextInfo ti = new CultureInfo(lang_name, false).TextInfo;
            return ti.ToTitleCase(text.ToLower());
        }

        /// <summary>
        /// Given a postal code, reformat it as follows:
        /// 1. Strip leading and trailing whitespace
        /// 2. If it starts with five consecutive digits, assume it's a US
        ///     ZIP code. In this case, return the first five digits.
        ///     (IE, ignore any ZIP+4 trailing stuff.)
        /// 3. If it doesn't start with five consecutive digits,
        ///     assume it's not a US ZIP code and return it
        ///     unchanged.
        /// </summary>
        /// <param name="orig_code"></param>
        /// <returns></returns>
        public static string CanonicalPostalCode(string orig_code)
        {
            if (string.IsNullOrEmpty(orig_code))
            {
                return "";
            }
            else
            {
                orig_code = orig_code.Trim();
                if (rzip.IsMatch(orig_code))
                {
                    orig_code = orig_code.Substring(0, 5);
                }
                return orig_code;
            }
        }
    }
}
