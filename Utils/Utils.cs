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
    }
    public static class TextUtils
    {
        private static string non_alphanum = @"[^0-9a-zA-Z\s]+"; // one or more members of given class
        private static Regex rna = new Regex(non_alphanum);
        private static string multiple_spaces = @"[\s]+";
        private static Regex rms = new Regex(multiple_spaces);

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
    }
}
