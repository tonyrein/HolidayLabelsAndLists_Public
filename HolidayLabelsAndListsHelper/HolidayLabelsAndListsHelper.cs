﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DAO;
using Utils;

//using GlobRes = AppWideResources.Properties.Resources;

namespace HolidayLabelsAndListsHelper
{
    public static class HllUtils
    {
        private static string[] VALID_HLL_TYPES = new string[]
            { "BAG", "GIFT", "DONOR", "MASTER", "PARTICIPANT", "PARTICIPANTSUMMARY", "POSTCARD", "THANKSGIVINGDELIVERY" };
        private static string[] FILE_TYPES_WITH_NO_DONOR = new string[] { "PARTICIPANT", "PARTICIPANTSUMMARY", "POSTCARD", "THANKSGIVINGDELIVERY" };
        public static string[] FILE_TYPES_WITH_DONOR = new string[] { "ALL", "BAG", "GIFT", "DONOR", "MASTER" };
        public static string[] LIST_AND_LABEL_EXTENSIONS = new string[] { ".docx", ".xlsx" };
        public const string BACKUP_FILE_REGEX = @".*\.bak\d{4,}$";
        public const string VALID_YEAR_REGEX = @"^20\d{2}$";
        public static bool TypeHasDonor(string doctype) { return FILE_TYPES_WITH_DONOR.Contains(doctype); }
        public static bool IsValidHllType(string doctype) { return VALID_HLL_TYPES.Contains(doctype); }

        /// <summary>
        /// Get list of Gift Label and Bag Label request output_doc_types ("Clothing,"
        /// "Toys," "Other") contained in the data read in from the VESTA
        /// reports.
        /// </summary>
        /// <param name="context" (DBWrapper)></param>
        /// <returns>array of strings representing gift label info and bag label info request output_doc_types</returns>
        //public static string[] RequestTypesInDb(DBWrapper context)
        //{
        //    return (from g in context.GliList select g.request_type).Concat
        //            (from b in context.BliList select b.request_type).Distinct().ToArray();
        //}

        /// <summary>
        /// Get list of the service enrollment output_doc_types contained in the data
        /// read in from the VESTA reports.
        /// </summary>
        /// <param name="context" (DBWrapper)></param>
        /// <returns>array of string</returns>
        public static string[] ServiceTypesInDb(DBWrapper context)
        {
            return (from s in context.HoEnrList
                    select s.service_type).Distinct().ToArray();
        }

        /// <summary>
        /// Ask OS to open the given file with the
        /// default program.
        /// </summary>
        /// <param name="filespec" (string)></param>
        public static void OpenFile(string filespec)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = filespec
            };
            process.Start();
        }

        /// <summary>
        /// For each year contained in the data read in from the VESTA
        /// reports, generate the donor-specific output files (labels,
        /// master lists, and donor lists) and then generate the donor-neutral
        /// output files (postcard labels and participant lists).
        /// 
        /// Return count of files generated
        /// </summary>
        /// <param name="wk" (BackgroundWorker)></param>
        /// <param name="context" (DBWrapper)></param>
        /// <returns>int</returns>
        public static int MakeOutputFiles(BackgroundWorker wk, int[] years, DBWrapper context)
        {
            int retInt = 0;
            //string[] request_types = RequestTypesInDb(context);
            //int[] years = YearsInDb(context);
            foreach (int year in years)
            {
                foreach (Donor d in context.DonorList)
                {
                    retInt += MakeOutputForDonor(wk, context, d, year);
                }
                retInt += MakeDonorNeutralDocs(wk, context, year);
            }
            return retInt;
        }

        /// <summary>
        /// Create all label and list files for a given donor
        /// and a given year.
        /// 
        /// Return the number of files written.
        /// </summary>
        /// <param name="wk" (BackgroundWorker)></param>
        /// <param name="ctx" (DBWrapper)></param>
        /// <param name="d" (Donor)></param>
        /// <param name="request_types" (array of string></param>
        /// <param name="year" (int)></param>
        /// <returns>int</returns>
        private static int MakeOutputForDonor(BackgroundWorker wk,
            DBWrapper ctx, Donor d, int year)
        {
            int retInt;
            retInt = MakeListsForDonor(wk, ctx, d, year);
            retInt += MakeLabelsForDonor(wk, ctx, d, year);
            return retInt;
        }

        /// <summary>
        /// Make donor and master lists for a given donor and
        /// a given year.
        /// 
        /// Return the number of files written.
        /// </summary>
        /// <param name="wk" (BackgroundWorker)></param>
        /// <param name="ctx" (DBWrapper)></param>
        /// <param name="d" (Donor)></param>
        /// <param name="request_types" (array of string></param>
        /// <param name="year" (int)></param>
        /// <returns>int</returns>
        private static int MakeListsForDonor(BackgroundWorker wk,
            DBWrapper ctx, Donor d, int year)
        {
            int retInt = 0;
            ListWriter w;
            foreach (string s in ctx.RequestTypesInDb ())
            {
                w = new MasterListWriter(wk, ctx, d, s, year);
                retInt += w.TypeReport();
                w = new DonorListWriter(wk, ctx, d, s, year);
                retInt += w.TypeReport();
            }
            return retInt;
        }

        /// <summary>
        /// Make bag and gift label files for a givendonor and a given
        /// year.
        /// 
        /// Return the number of files written.
        /// </summary>
        /// <param name="wk" (BackgroundWorker)></param>
        /// <param name="ctx" (DBWrapper)></param>
        /// <param name="d" (Donor)></param>
        /// <param name="year" (int)></param>
        /// <returns>int</returns>
        private static int MakeLabelsForDonor(BackgroundWorker wk,
            DBWrapper ctx, Donor d, int year)
        {
            int retInt = 0;
            LabelWriter w;
            foreach (string s in ctx.RequestTypesInDb())
            {
                w = new BagLabelWriter(wk, ctx, d, s, year);
                retInt += w.TypeAllRecords();
                w = new GiftLabelWriter(wk, ctx, d, s, year);
                retInt += w.TypeAllRecords();
            }
            return retInt;
        }

        /// <summary>
        /// Make those files for a given year which do not depend on
        /// a donor - that is the participant lists and the postcard
        /// labels.
        /// 
        /// Return the number of files written.
        /// </summary>
        /// <param name="wk" (BackgroundWorker)></param>
        /// <param name="ctx" (DBWrapper)></param>
        /// <param name="year" (int)></param>
        /// <returns>int</returns>
        private static int MakeDonorNeutralDocs(BackgroundWorker wk,
            DBWrapper ctx, int year)
        {
            int retInt = 0;
            retInt += MakeParticipantLists(wk, ctx, year);
            retInt += MakePostcardLabels(wk, ctx, year);
            retInt += MakeParticipantSummaryLabels(wk, ctx, year);
            retInt += MakeThanksgivingParticipantLists(wk, ctx, year);
            return retInt;
        }

        /// <summary>
        /// Make participant lists for a given year.
        /// Make one list for each service type.
        /// Return number of files written.
        /// </summary>
        /// <param name="wk" (BackgroundWorker)></param>
        /// <param name="ctx" (DBWrapper)></param>
        /// <param name="year" (int)></param>
        /// <returns>int</returns>
        private static int MakeParticipantLists(BackgroundWorker wk,
            DBWrapper ctx, int year)
        {
            ListWriter w;
            int retInt = 0;
            string[] service_types = ServiceTypesInDb(ctx);
            foreach (string s in service_types)
            {
                w = new ParticipantListWriter(wk, ctx, s, year);
                retInt += w.TypeReport();
            }
            return retInt;
        }

        private static int MakeThanksgivingParticipantLists(BackgroundWorker wk,
            DBWrapper ctx, int year)
        {
            ListWriter w = new ThanksgivingDeliveryList(wk, ctx, year);
            return w.TypeReport();
        }

        /// <summary>
        /// Create postcard label files for the given year,
        /// one for each service type.
        /// 
        /// Return number of files written
        /// </summary>
        /// <param name="wk" (BackgroundWorker)></param>
        /// <param name="ctx" (DBWrapper)></param>
        /// <param name="year" (int)></param>
        /// <returns>int</returns>
        static int MakePostcardLabels(BackgroundWorker wk,
            DBWrapper ctx, int year)
        {
            LabelWriter w;
            int retInt = 0;
            string[] service_types = ServiceTypesInDb(ctx);
            foreach (string s in service_types)
            {
                w = new PostcardLabelWriter(wk, ctx, s, year);
                retInt += w.TypeAllRecords();
            }
            return retInt;
        }

        static int MakeParticipantSummaryLabels(BackgroundWorker wk,
            DBWrapper ctx, int year)
        {
            LabelWriter w;
            int retInt = 0;
            w = new ParticipantSummaryLabelWriter(wk, ctx, year);
            retInt += w.TypeAllRecords();
            return retInt;
        }

        /// <summary>
        /// Create a BackgroundWorker and assign its
        /// event handlers
        /// </summary>
        /// <param name="work_event_handler"></param>
        /// <param name="work_completed_handler"></param>
        /// <param name="progress_handler"></param>
        /// <returns></returns>
        public static BackgroundWorker MakeWorker(DoWorkEventHandler work_event_handler,
            RunWorkerCompletedEventHandler work_completed_handler,
            ProgressChangedEventHandler progress_handler)
        {
            BackgroundWorker wk = new BackgroundWorker();
            wk.DoWork += work_event_handler;
            wk.RunWorkerCompleted += work_completed_handler;
            if (progress_handler != null)
            {
                wk.ProgressChanged += progress_handler;
                wk.WorkerReportsProgress = true;
            }
            wk.WorkerSupportsCancellation = true;
            return wk;
        }

        /// <summary>
        /// Does the filename (without path or extension) contain
        /// something that matches the below regex,
        /// such as "Participant_List_2017_Aht.bak0003"
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsBackupFile(string filename)
        {
            return Regex.Match(filename, HllUtils.BACKUP_FILE_REGEX).Success;
        }

        /// <summary>
        /// Does the string denote a valid year?
        /// </summary>
        /// <param name="yearstring"></param>
        /// <returns></returns>
        public static bool IsValidYear(string yearstring)
        {
            return Regex.Match(yearstring, HllUtils.VALID_YEAR_REGEX).Success;
        }
        /// <summary>
        /// Allow the user to select one or more VESTA report
        /// spreadsheets to use as input for our label and list files.
        /// </summary>
        /// <returns></returns>
        public static string[] GetVestaReportNames()
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Multiselect = true;
            d.Filter = Properties.Resources.FileSpecFilterExcel;
            d.FilterIndex = 1;
            d.CheckFileExists = true;
            d.CheckPathExists = true;
            d.Title = Properties.Resources.VestaReportSelectTitle;
            d.InitialDirectory = Properties.Settings.Default.InitialVestaFolder;
            DialogResult dr = d.ShowDialog();
            if (dr == DialogResult.OK)
                return d.FileNames;
            else
                return null;
        }
    }


    // Internal class to hold info about label and list files:
    internal class HllFileInfo
    {
        internal string FullPath { get; set; }
        internal string BareName { get; set; } // without folder or extension
        internal FilterSetTypeFilters Type { get; set; }      // ie, Master List, Bag Label, Postcard Label, etc.

        internal string Year { get; set; }
        internal string DonorCode { get; set; }
        internal bool IsBackupFile { get; set; }
        internal bool IsValidHLL { get; set; }
        internal bool HasNoDonor { get; set; }
        internal bool HasDonor { get { return !HasNoDonor; } }

        internal HllFileInfo(string fullpath)
        {
            FullPath = fullpath;
            BareName = Path.GetFileNameWithoutExtension(fullpath);
            string without_bu_portion = BareName.Split('.')[0];
            this.Type = new FilterSetTypeFilters(without_bu_portion);
            // Parse the file name (without the backup portion). The
            // elements denoting file type, year, and donor code are
            // separated by underscores.
            string[] sarray = without_bu_portion.ToUpper().Split('_');
            // Year is given in first element of sarray which starts with
            // appropriate century.
            string y = sarray.FirstOrDefault(s => s.StartsWith("20"));
            Year = (y == null ? "" : y);
            //Year = (sarray.Length >= 3) ? sarray[2] : "";
            IsValidHLL = ( Type.IsValid && HllUtils.IsValidYear(Year) );
            if (!IsValidHLL) // if not, don't bother with donor code, etc.
            {
                IsBackupFile = false;
                HasNoDonor = true;
            }
            else
            {
                IsBackupFile = HllUtils.IsBackupFile(BareName);
                if (!Type.HasDonor())
                    HasNoDonor = true;
                else
                    DonorCode = DonorCodeFromFileName(sarray);
            }
        }
        /// <summary>
        /// Given a string array with the donor code
        /// denoted by the fifth through the end element,
        /// return the donor code. For example, if the
        /// array is [ "DONOR", "LIST", "2016", "TOYS", "HB", "COL" ]
        /// the donor code is HB_COL.
        /// 
        /// </summary>
        /// <returns></returns>
        private string DonorCodeFromFileName(string[] sarray)
        {
            // donor code starts at offset 4. 
            if (sarray.Length < 5)
                return "";
            else
                return String.Join("_", sarray.Skip(4).ToArray());
        }
    }

    public enum output_doc_types
    {
        ALL,
        BAG,
        GIFT,
        DONORANDMASTERLIST,
        PARTICIPANTLIST,
        PARTICIPANTSUMMARYLABEL,
        POSTCARD,
        THANKSGIVINGDELIVERY,
        INVALID,
    }

    public class FilterSetTypeFilters
    {
        private output_doc_types[] TypesWithDonor = new output_doc_types[]
        {
            output_doc_types.ALL, output_doc_types.DONORANDMASTERLIST, output_doc_types.BAG, output_doc_types.GIFT
        };
        private output_doc_types[] TypesWithoutDonor = new output_doc_types[]
        {
            output_doc_types.PARTICIPANTLIST, output_doc_types.PARTICIPANTSUMMARYLABEL, output_doc_types.POSTCARD,
            output_doc_types.THANKSGIVINGDELIVERY
        };
        private output_doc_types ty;
        public bool HasDonor()
        {
            return TypesWithDonor.Contains(this.ty);
        }

        public FilterSetTypeFilters(output_doc_types odt)
        {
            this.ty = odt;
        }
        /// <summary>
        /// Use this constructor to make a FilterSetTypeFilters object
        /// from a string, assumed to be part of a file name.
        /// </summary>
        /// <param name="s"></param>
        public FilterSetTypeFilters(string s)
        {
            s = Utils.TextUtils.CleanString(s);
            s = s.ToUpper();
            if (s.StartsWith("DONOR") || s.StartsWith("MASTER") )
            {
                this.ty = output_doc_types.DONORANDMASTERLIST;
                return;
            }
            if (s.StartsWith("PARTICIPANT_SUMMARY"))
            {
                this.ty = output_doc_types.PARTICIPANTSUMMARYLABEL;
                return;
            }
            if (s.StartsWith("PARTICIPANT_LIST"))
            {
                this.ty = output_doc_types.PARTICIPANTLIST;
                return;
            }
            if (s.StartsWith("THANKSGIVING_DELIVERY"))
            {
                this.ty = output_doc_types.THANKSGIVINGDELIVERY;
                return;
            }
            // For other output_doc_types, only the portion
            // before the first '_' is significant.
            s = s.Split('_')[0];
            // If parsing into one of our output_doc_types works,
            // use that. Otherwise set to INVALID
            if (!Enum.TryParse(s, out this.ty))
                this.ty = output_doc_types.INVALID;
        }

        public bool IsValid {  get { return this.ty != output_doc_types.INVALID; } }
        public bool IsParticipantList {  get { return this.ty == output_doc_types.PARTICIPANTLIST; } }
        public bool Matches(FilterSetTypeFilters other)
        {
            return (this.ty == output_doc_types.ALL) ||
            (this.ty == other.ty);
        }
    }


    public class FilterSet
    {
        private string _donorfilter;
        public FilterSetTypeFilters TypeFIlter { get; set; }
        public string YearFilter { get; set; }
        
        public string DonorFilter
        {
            get { return this._donorfilter; }
            set { this._donorfilter = value.ToUpper(); }
        }
        public bool IncludeBackupsFilter { get; set; }

        public bool DonorMatches(string donfltr)
        {
            return (donfltr.ToUpper() == "ALL") || (this.DonorFilter == donfltr);
        }

        public bool TypeMatches(FilterSetTypeFilters t)
        {
            return this.TypeFIlter.Matches(t);
        }

        public bool YearMatches(string y)
        {
            return this.YearFilter == y;
        }
    }
    /// <summary>
    /// Maintain lists of HllFileInfo objects and
    /// filters which apply to those lists.
    /// </summary>
    public class HllFileListManager
    {
        public FilterSet filterset = new FilterSet();
        public string YearFilter
        {
            get { return this.filterset.YearFilter; }
            set { this.filterset.YearFilter = value; }
        }
        public FilterSetTypeFilters TypeFilter
        {
            get { return this.filterset.TypeFIlter;  }
            set { this.filterset.TypeFIlter = value; }
        }
        public string DonorFilter
        {
            get { return this.filterset.DonorFilter; }
            set { this.filterset.DonorFilter = value; }
        }

        public bool IncludeBackupsFilter
        {
            get { return this.filterset.IncludeBackupsFilter; }
            set { this.filterset.IncludeBackupsFilter = value; }
        }

        private string[] AllFilenames;
        private List<HllFileInfo> RegularHllFiles = new List<HllFileInfo>();
        private List<HllFileInfo> BackupHllFiles = new List<HllFileInfo>();
        
        public int AllFilesCount {  get { return AllFilenames.Count(); } }
        public bool IsEmpty { get { return (RegularFilesCount == 0) && (BackupFilesCount == 0); } }
        public int RegularFilesCount {  get { return RegularHllFiles.Count(); } }
        public int BackupFilesCount { get { return BackupHllFiles.Count(); } }
        public bool HasBackupFiles {  get { return BackupFilesCount > 0; } }
        private Dictionary<string, HllFileInfo> FilteredFiles = new Dictionary<string, HllFileInfo>();
        private DBWrapper context;
        
        /// <summary>
        /// Return list of all donors whose codes are
        /// in our file list.
        /// </summary>
        /// <returns></returns>
        public List<Donor> DonorsInFileList()
        {
            List<Donor> retList = new List<Donor>();
            // For each file, if it's of the proper type,
            // (ie if it's not a type for which donor
            // is irrelevant), add its donor to our
            // return list.
            List<string> codes = new List<string>();
            foreach (HllFileInfo hfi in RegularHllFiles)
            {
                if ( hfi.HasDonor )
                    codes.Add(hfi.DonorCode);
            }
            // remove dupes:
            codes = codes.Distinct().ToList();
            // Now add donors to our retList, using the codes we found:
            foreach (string c in codes)
            {
                Donor d = context.FindDonorByCode(c);
                if (d == null) // Uh-oh! This is a new donor.
                {
                    // add it to datastore as well as to our return list
                    d = new Donor(c, c);
                    context.AddOrUpdateDonor(d);
                }
                retList.Add(d);
            }
            // Sort the list by donor name:
            return retList.OrderBy(d => d.name).Distinct().ToList();
        }


        public List<Donor> ActiveDonorsForYear(string year)
        {
            List<Donor> retList = new List<Donor>();
            foreach(HllFileInfo hfi in RegularHllFiles)
            {
                if (hfi.IsValidHLL && hfi.HasDonor && hfi.Year == year)
                {
                    string c = hfi.DonorCode;
                    Donor d = context.FindDonorByCode(c);
                    if (d == null) // new donor! How did this happen?
                    {
                        // Make a new donor using this code.
                        // Add it to data store as well as our return list
                        d = new Donor(c, c);
                        context.AddOrUpdateDonor(d);
                    }
                    retList.Add(d);
                }
            }
            return retList.Distinct(new DistinctDonorComparer()).OrderBy(d => d.name).ToList();
            //return retList.GroupBy(gd => gd.name).Select(sd => sd.FirstOrDefault()).ToList();
        }

        /// <summary>
        /// Return array of distinct values
        /// of Year value in our list of files.
        /// 
        /// Array will be sorted in descending order, so that
        /// the combo box in the UI will show the most recent
        /// year at the top.
        /// </summary>
        /// <returns></returns>
        public string[] YearsInFileList()
        {
            // HashSet does not allow duplicates, so there
            // is no need to use Distinct() in the Linq below.
            HashSet<string> allyears = new HashSet<string>();
            foreach (HllFileInfo fi in RegularHllFiles)
            {
                if (fi.IsValidHLL) { allyears.Add(fi.Year); }
            }
            foreach (HllFileInfo fi in BackupHllFiles)
            {
                if (fi.IsValidHLL) { allyears.Add(fi.Year); }
            }
            return allyears.OrderByDescending(y => y).ToArray();
        }

    
        private bool YearMatches(HllFileInfo hfi, string year)
        {
            return hfi.Year == year;
        }

        private bool DonorMatches(HllFileInfo hfi, string donor_filter)
        {
            return hfi.DonorCode == donor_filter;
        }

        private bool TypeMatches(HllFileInfo hfi, FilterSetTypeFilters tf)
        {
            return hfi.Type.Matches(tf);
        }
        /// <summary>
        /// The UI allows users to select which files to see in the list. Filtering
        /// is possible by year, donor, and file type, and users can also choose
        /// whether or not to display backup files.
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        private bool PassesFilter(HllFileInfo hfi, FilterSet filterset)
        {
            if (!hfi.IsValidHLL)
                return false;
            if (hfi.Year != this.filterset.YearFilter)
                return false;
            if (! this.filterset.TypeFIlter.Matches(hfi.Type))
                return false;
            if (hfi.HasNoDonor)
                return true;
            else
                return DonorMatches(hfi, filterset.DonorFilter);
        }

        /// <summary>
        /// Load full list of file specs. The donor, type, and
        /// year filters determine which files from
        /// this list will be displayed to the user.
        /// </summary>
        public HllFileListManager(DBWrapper ctx)
        {
            context = ctx;
            LoadSourceFileList();
        }


        /// <summary>
        /// Get names of all files in and under output folder that
        /// have the appropriate extension.
        /// 
        /// For each of those files, make an HllFileInfo object
        /// and add that object to either the RegularHllFiles or the
        /// BackupHllFiles list.
        /// 
        /// </summary>
        public void LoadSourceFileList()
        {
            string fld = FolderManager.OutputFolder;
            this.RegularHllFiles.Clear();
            this.BackupHllFiles.Clear();

            if (Directory.Exists(fld))
            {
                // do recursive search for all files with appropriate extensions
                this.AllFilenames = Directory.GetFiles(fld, "*", SearchOption.AllDirectories)
                 .Where(f => HllUtils.LIST_AND_LABEL_EXTENSIONS.Contains(Path.GetExtension(f))).ToArray();
            }
            else
                this.AllFilenames = new string[0];

            if (this.AllFilenames.Count() > 0)
            {
                foreach (string fs in this.AllFilenames)
                {
                    HllFileInfo fi = new HllFileInfo(fs);
                    if (fi.IsValidHLL) // Don't bother if it's not a valid file.
                    {
                        if (fi.IsBackupFile)
                            this.BackupHllFiles.Add(fi);
                        else
                            this.RegularHllFiles.Add(fi);
                    }
                }
            }
        }


       

        /// <summary>
        /// Delete files on the BackupFiles list,
        /// then clear the list.
        /// 
        /// Return number of files deleted.
        /// </summary>
        public int DeleteBackupFiles()
        {

            int retInt = 0;
            foreach (HllFileInfo hfi in BackupHllFiles)
            { 
                if (File.Exists(hfi.FullPath))
                {

                    File.Delete(hfi.FullPath);
                    retInt++;
                }
            }
            BackupHllFiles.Clear();
            return retInt;
        }


        /// <summary>
        /// Iterate over lists of regular and backup files
        /// and delete those which match the list of years
        /// passed in by the caller.
        /// 
        /// Iterate in descending order so that we can delete items from the
        /// lists without changing the indices.
        /// 
        /// Do not delete ParticipantList files.
        /// 
        /// Return the count of files deleted.
        /// </summary>
        /// <param name="years"></param>
        /// <returns></returns>
        public int DeleteOldFiles(List<String> years)
        {
            int retInt = 0;
            for(int idx = RegularFilesCount - 1; idx >= 0; idx--)
            {
                HllFileInfo fi = RegularHllFiles[idx];
                if ( (years.Contains(fi.Year)) &&
                    ( ! fi.Type.IsParticipantList ) )
                {
                    if (File.Exists(fi.FullPath))
                    {
                        File.Delete(fi.FullPath);
                    }
                    RegularHllFiles.RemoveAt(idx);
                    retInt++;
                }
            }
            for (int idx = BackupFilesCount - 1; idx >= 0; idx--)
            {
                HllFileInfo fi = BackupHllFiles[idx];
                if ( (years.Contains(fi.Year)) &&
                    ( ! fi.Type.IsParticipantList ) )
                {
                    if (File.Exists(fi.FullPath))
                    {
                        File.Delete(fi.FullPath);
                    }
                    BackupHllFiles.RemoveAt(idx);
                    retInt++;
                }
            }
            return retInt;
        }

        /// <summary>
        /// Get list of full filespecs of 
        /// files matching the given filterset.
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public List<string> FilePathsMatchingFilter(FilterSet fs)
        {
            List<string> retList = new List<string>();
            foreach(HllFileInfo item in this.MatchingHFis(fs))
            {
                if (File.Exists(item.FullPath))
                {
                    retList.Add(item.FullPath);
                }
            }
            return retList;
        }

        /// <summary>
        /// Set FilteredFiles to include only those files
        /// matching our FilterSet member.
        /// </summary>
        public void ApplyFilters()
        {
            this.FilteredFiles = FilesMatchingFilter(this.filterset);
        }

        /// <summary>
        /// Return list of HllFileInfo objects matching
        /// the given filterset.
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private List<HllFileInfo> MatchingHFis(FilterSet fs)
        {
            List<HllFileInfo> retList = new List<HllFileInfo>();
            foreach (HllFileInfo hfi in RegularHllFiles)
            {
                if (PassesFilter(hfi, fs))
                    retList.Add(hfi);
            }
            if (fs.IncludeBackupsFilter)
            {
                foreach (HllFileInfo hfi in BackupHllFiles)
                {
                    if (PassesFilter(hfi, fs))
                        retList.Add(hfi);
                }
            }
            return retList;
        }

        /// <summary>
        /// Get a collection of HllFileInfo objects matching the given filterset.
        /// The collection is a Dictionary. The key of each item is the file
        /// name (without the directory) and the value of each item is
        /// the corresponding HllFileInfo object.
        /// 
        /// This is so that UI lists can use bare filenames as keys
        /// to the associated HllFileInfo objects.
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        internal Dictionary<string, HllFileInfo> FilesMatchingFilter(FilterSet fs)
        {
            Dictionary<string, HllFileInfo> retDict = new Dictionary<string, HllFileInfo>();

            foreach(HllFileInfo hfi in this.MatchingHFis(fs))
            {
                retDict[hfi.BareName] = hfi;
            }
            return retDict;
        }

        /// <summary>
        /// Given a bare file name, return the full path
        /// of the corresponding HllFileInfo object, if
        /// it exists in our FilteredFiles collection.
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public string FullPathForFile(string fn)
        {
            if (this.FilteredFiles.ContainsKey(fn))
                return this.FilteredFiles[fn].FullPath;
            else
                return "";
        }

        /// <summary>
        /// Return an array of file names for use in the UI's
        /// available files listview. This array is file names
        /// only (no paths).
        /// 
        /// TODO: change this to return a List<string>, to
        /// match FilePathsMatchingFilter.
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] FileNameList()
        {
            return FilteredFiles.Keys.ToArray();
        }
    }

}
