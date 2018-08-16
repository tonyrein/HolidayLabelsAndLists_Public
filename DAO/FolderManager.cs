using System.IO;


namespace DAO
{
    /// <summary>
    /// Utility class used to supply destination folders for
    /// various report types.
    /// 
    /// The top level of the output folder hierarcy starts
    /// with the value taken from the application's settings.
    /// 
    /// </summary>
    public static class FolderManager
    {
        // Top level of folder structure used to hold
        // this app's output (the label and list files).
        public static string OutputFolder
        {
            get { return Properties.Settings.Default.OutputFolder; }
        }

        // Output files for each year are placed in
        // a subfolder under OutputFolder
        public static string YearFolder(int year)
        {
            return Path.Combine(OutputFolder, year.ToString());
        }

        // Files for Christmas-related programs are stored
        // in this subfolder under the applicable Year folder.
        public static string ChristmasProgramFolder(int year)
        {
            return Path.Combine(YearFolder(year), "Christmas");
        }

        // Files for Thanksgiving-related programs are stored
        // in this subfolder under the applicable Year folder.
        public static string ThanksgivingProgramFolder(int year)
        {
            return Path.Combine(YearFolder(year), "Thanksgiving");
        }

        // Files that don't belong with the Christmas- or Thanksgiving-
        // related files go in here.
        public static string MiscProgramFolder(int year)
        {
            return Path.Combine(YearFolder(year), "Other");
        }

        // Bag or Gift label files are Christmas-related.
        public static string BagAndGiftLabelFolder(int year)
        {
            return ChristmasProgramFolder(year);
        }

        // Sort these files into subfolders depending on what program they're
        // for.
        public static string PostcardsAndParticipantsFolder(int year, string service_type)
        {
            string retString = "";
            switch (Utils.TextUtils.CleanString(service_type))
            {
                case "Adopt_A_Family":
                case "Holiday_Food_Basket":
                    retString = ChristmasProgramFolder(year);
                    break;
                case "Thanksgiving_Basket":
                    retString = ThanksgivingProgramFolder(year);
                    break;
                default:
                    retString = "Other";
                    break;
            }
            return Path.Combine(YearFolder(year), retString);
        }

        public static string DbFolder()
        {
            return Path.Combine(OutputFolder, "Database");
        }
    }
}
