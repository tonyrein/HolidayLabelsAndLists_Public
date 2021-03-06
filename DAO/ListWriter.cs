﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel;
//using GlobRes = AppWideResources.Properties.Resources;

namespace DAO
{
    /// <summary>
    ///  Uses OfficeOpenXml ExcelPackage object to write to an
    ///  Excel spreadsheet.
    /// </summary>
    public abstract class ListWriter
    {
        /// <summary>
        /// Encapsulate information needed to properly format
        /// a column in the output.
        /// </summary>
        public class ColumnInfo
        {
            public string Name { get; set; }
            public int Width { get; set; } // desired width in characters
            public ExcelHorizontalAlignment HorizAlignment { get; set; }
            public ExcelVerticalAlignment VerticalAlignment { get; set; }
            public bool Wraps { get; set; }
            public ColumnInfo(string name, int width, ExcelHorizontalAlignment horiz_alignment,
                ExcelVerticalAlignment vert_alignment, bool wraps)
            {
                this.Name = name; this.Width = width;
                this.HorizAlignment = horiz_alignment;
                this.VerticalAlignment = vert_alignment;
                this.Wraps = wraps;
            }
        }

        private BackgroundWorker Worker;
        protected DBWrapper context;
        protected const string FILE_EXTENSION = ".xlsx";
        protected ExcelWorksheet sh;
        private int CurrentRow;
        private List<string[]> _datarows = null;
        private string[] _column_names = null;
        private string[] _header_rows = null;
        //protected ColumnInfo[] ColumnInfoArray = null;
        protected List<ColumnInfo> ColumnInfoList = null;
        protected List<string[]> DataRows
        {
            get
            {
                if (this._datarows == null)
                    this._datarows = this.FetchData();
                return this._datarows;
            }
        }
        protected string[] HeaderRows
        {
            get
            {
                if (this._header_rows == null)
                    this._header_rows = this.FetchHeaders();
                return this._header_rows;
            }
        }
        protected string[] ColumnNames
        {
            get
            {
                if (this._column_names == null)
                    this._column_names = this.FetchColumnNames();
                return this._column_names;
            }
        }

        protected int ColumnNamesRow;
        protected int TopDataRow;

        protected abstract string TargetFolder();

        public ListWriter(BackgroundWorker wk, DBWrapper ctx)
        {
            this.Worker = wk;
            this.context = ctx;
        }

        protected void Init()
        {
            if (!Directory.Exists(this.TargetFolder()))
                Directory.CreateDirectory(this.TargetFolder());
            this.PopulateColumnInfo();
        }

        public void Reset()
        {
            this._column_names = null;
            this._datarows = null;
            this._header_rows = null;
        }


        /// <summary>
        /// Generate an ExcelPackage instance to be written to.
        /// 
        /// Create the destination directory if it doesn't already exist.
        /// 
        /// Use this.GetOutputFileSpec() to find the destination
        /// filespec. IF there's already a file with that spec,
        /// rename it.
        /// 
        /// TODO: Add exception handling in case output file could
        /// not be opened or any existing file could not be moved
        /// to backup.
        /// 
        /// </summary>
        /// <returns></returns>
        private ExcelPackage OpenSpreadsheet()
        {
            if (!Directory.Exists(this.TargetFolder()))
                Directory.CreateDirectory(this.TargetFolder());
            string outfile_spec = this.GetOutputFileSpec();
            if (File.Exists(outfile_spec))
            {
                // Move original to backup ...
                Utils.FileUtils.MoveToBackup(outfile_spec);
            }
            FileInfo fi = new FileInfo(outfile_spec);
            return new ExcelPackage(fi);
        }

        /// <summary>
        /// Copy contents of string array into
        /// current row of this ListWriter's ExcelWorksheet
        /// object.
        /// 
        /// Note that this.sh.Cell's column index is one-based!
        /// </summary>
        /// <param name="row"></param>
        private void WriteRow(string[] row)
        {
            for (int i = 0; i < row.Length; i++)
            {
                this.sh.Cells[this.CurrentRow, i + 1].Value = row[i];
            }
        }

        private void NextRow()
        {
            this.CurrentRow++;
        }

        protected virtual void DoFormatting()
        {
            this.FormatColumnNames();
            this.FormatDataCells();
            this.SetPageLayout();
        }


        /// <summary>
        /// Create output file and write data.
        /// 
        /// Return 1 if a file is written, else 0
        /// </summary>
        /// <returns></returns>
        public int TypeReport()
        {
            // Don't bother if no data rows:
            if (this.DataRows.Count == 0)
                return 0;
            // Initialize variables used in this method:
            int retInt = 0;
            string fn = Path.GetFileName(this.GetOutputFileSpec());
            this.ColumnNamesRow = 1;
            this.TopDataRow = this.ColumnNamesRow + 1;
            ExcelPackage pkg = null;
            // Tell the world what we're doing:
            this.Worker.ReportProgress(0,
                String.Format(Properties.Resources.CountWritingMsg,
                    this.DataRows.Count, "data rows", fn)
                    );
            try
            {
                // Write records to an ExcelWorksheet object attached
                // to our ExcelPackage, then save:
                pkg = this.OpenSpreadsheet();
                this.sh = pkg.Workbook.Worksheets.Add(this.WorksheetName);
                this.TypeHeader();
                this.TypeColumnNames();
                this.TypeData();
                this.DoFormatting();
                pkg.Save();
                this.Worker.ReportProgress(0,
                    String.Format(Properties.Resources.FileCreationSuccessMsg,fn)
                    );
                retInt = 1;
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(
                    Properties.Resources.FileExceptionErrorMsg,
                    fn, e.Message);
                this.Worker.ReportProgress(0, sb.ToString());
                retInt = 0;
            }
            finally
            {
                pkg.Dispose();
            }
            return retInt;
        }

        protected void TypeHeader()
        {
            // construct a string to be put into the header. Start
            // by turning on Bold and setting font size:
            StringBuilder sb = new StringBuilder("&B&18");
            // Append each header row to sb. Add a new
            // line after each one, except for the last one.
            for (int i = 0; i < this.HeaderRows.Length; i++)
            {
                sb.Append(this.HeaderRows[i]);
                if (i < (this.HeaderRows.Length - 1))
                    sb.Append('\n');
            }
            string s = sb.ToString();
            // BUG (?) We should be able to set differentFirst
            // and differentOdEven to false and have all headers
            // be assigned the same value. However, this doesn't
            // appear to work, so we explicitly assign the same
            // value to each.
            this.sh.HeaderFooter.differentFirst = false;
            this.sh.HeaderFooter.differentOddEven = false;
            this.sh.HeaderFooter.FirstHeader.LeftAlignedText = s;
            this.sh.HeaderFooter.EvenHeader.LeftAlignedText = s;
            this.sh.HeaderFooter.OddHeader.LeftAlignedText = s;
            // Now do page numbers on right side of header.
            // The below string results in bold text with,
            // for example, "page 3 of 5."
            this.sh.HeaderFooter.FirstHeader.RightAlignedText = Properties.Resources.PageNumbers;
            this.sh.HeaderFooter.EvenHeader.RightAlignedText = Properties.Resources.PageNumbers;
            this.sh.HeaderFooter.OddHeader.RightAlignedText = Properties.Resources.PageNumbers;
        }


        /// <summary>
        /// Fill in column labels for the data section.
        /// 
        /// It is assumed that this will only write to one row
        ///   that is, there are no multi-row column labels.
        /// </summary>
        protected void TypeColumnNames()
        {
            this.CurrentRow = this.ColumnNamesRow;
            this.WriteRow(this.ColumnNames);
        }

        /// <summary>
        /// Enter the report's data.
        /// </summary>
        protected void TypeData()
        {
            this.CurrentRow = this.TopDataRow;
            foreach (string[] string_arr in this.DataRows)
            {
                this.WriteRow(string_arr);
                this.NextRow();
            }
        }

        protected void TypeNoDataAlert()
        {
            this.CurrentRow = this.TopDataRow;
            this.WriteRow(new string[] { Properties.Resources.NoDataMsg });
        }

        /// <summary>
        /// Get list of data rows
        /// </summary>
        /// <returns></returns>
        protected abstract List<string[]> FetchData();

        /// <summary>
        /// Populate list of column names
        /// </summary>
        /// <returns></returns>
        protected abstract string[] FetchColumnNames();

        /// <summary>
        /// Populate list of header rows. The header
        /// is the information to be displayed at the top
        /// of the report, before the data section.
        /// </summary>
        /// <returns></returns>
        protected abstract string[] FetchHeaders();

        /// <summary>
        /// Use this instance's properties to derive
        /// a filespec for the output file.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetOutputFileSpec();

        /// <summary>
        /// Use this instance's properties to derive
        /// the name of the worksheet to be used.
        /// </summary>
        protected abstract string WorksheetName { get; }


        /// <summary>
        /// Apply formatting to the labels at the tops of the data columns.
        /// Subclasses can override this in order to apply non-default
        /// formatting.
        /// </summary>
        protected virtual void FormatColumnNames()
        {
            ExcelRange r = this.sh.Cells[this.ColumnNamesRow, 1, this.ColumnNamesRow, this.ColumnNames.Length];
            r.Style.Font.Bold = true;
            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            foreach (var c in r)
            {
                c.Style.Border.BorderAround(ExcelBorderStyle.Hair);
            }
        }

        /// <summary>
        /// Apply formatting to the data section of the report. Subclasses
        /// can override this in order to apply non-default formatting.
        /// However, this can also be done by means of the ColumnInfoArray -- if
        /// this array is provided, its contents will be used for per-column
        /// formatting.
        /// 
        /// Note that this.sh.Cell uses one-based indexing.
        /// 
        /// </summary>
        protected virtual void FormatDataCells()
        {
            int bottom_row = this.TopDataRow + this.DataRows.Count - 1;
            // Define range on the data section:
            ExcelRange r = this.sh.Cells[this.TopDataRow, 1, bottom_row, this.ColumnNames.Length];
            // Apply default formatting to the range:
            r.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            r.AutoFitColumns();
            // Apply grid around each cell in range:
            foreach (var c in r)
            {
                c.Style.Border.BorderAround(ExcelBorderStyle.Hair);
            }

            // Per-column formatting supplied?
            if (this.ColumnInfoList != null)
            {
                for (int i = 0; i < this.ColumnInfoList.Count; i++)
                {
                    ColumnInfo ci = this.ColumnInfoList[i];
                    // Define range on this column:
                    r = this.sh.Cells[this.TopDataRow, i + 1,
                        bottom_row, i + 1];
                    // Apply values from ColumnInfo object to the range:
                    r.Style.HorizontalAlignment = ci.HorizAlignment;
                    r.Style.VerticalAlignment = ci.VerticalAlignment;
                    r.Style.WrapText = ci.Wraps;
                    if (ci.Width != -1)
                    {
                        this.sh.Column(i + 1).Width = ci.Width;
                    }
                }
            }

        }

        /// <summary>
        /// Use OfficeOpenXml PrinterSettings properties
        /// to set orientation and top margin of the output
        /// document, and to set the column name row to
        /// be repeated on each output page.
        /// </summary>
        protected virtual void SetPageLayout()
        {
            this.sh.PrinterSettings.Orientation = eOrientation.Landscape;
            // Repeat column names on each page:
            this.sh.PrinterSettings.RepeatRows =
                new ExcelAddress($"'{this.sh.Name}'!${this.ColumnNamesRow}:${this.ColumnNamesRow}");
            // Set top margin to allow room for header. "M" suffix means decimal literal, and
            // units are inches.
            this.sh.PrinterSettings.TopMargin = 1.0M;
        }


        /// <summary>
        /// Create an array of ColumnInfo and fill
        /// in parameters for each column in this report.
        /// 
        /// A subclass may provide a "no-op" implementation
        /// of this method -- in that case, all columns will
        /// have default settings:
        ///   * autofit width
        ///   * left horizontal alignment
        ///   * top vertical alignment
        ///   * no wrapping
        /// However, such a subclass would not be able to
        /// use ColumnInfo.Name in FetchColumnNames()
        /// </summary>
        protected abstract void PopulateColumnInfo();

    }

    public class DonorListWriter : ListWriter
    {
        public Donor Dnr { get; set; }
        public string RequestType { get; set; }
        public int Year { get; set; }

        public DonorListWriter(BackgroundWorker wk, DBWrapper ctx,
            Donor d, string request_type, int year)
            : base(wk, ctx)
        {
            this.Dnr = d;
            this.RequestType = request_type;
            this.Year = year;
            this.Init();
        }

        /// <summary>
        /// Header for DonorListWriter is, for example:
        ///  [ "Donor List", "Toys", "2017", "Amalgamated Consolidated Leasing LLC" ]
        /// </summary>
        /// <returns></returns>
        protected override string[] FetchHeaders()
        {
            return new string[]
            {
                string.Format(Properties.Resources.DonorListHeader,
                this.RequestType, this.Year, this.Dnr.name)
            };
        }

        /// <summary>
        /// Specify header text and widths for relevant columns.
        /// -1 for width means that the
        /// column's width will be set later based on the
        /// length of the contents.
        /// </summary>
        protected override void PopulateColumnInfo()
        {
            this.ColumnInfoList = new List<ColumnInfo>();
            //this.ColumnInfoArray = new ColumnInfo[num_columns];
            this.ColumnInfoList.Add(new ColumnInfo("Fam ID", -1,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Child", -1,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Gen", 4,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Age", 4,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Req. Type", -1,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Request Detail", 35,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add(new ColumnInfo("Donor Name", 30,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Donor Phone", 17,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, false));
        }
        protected override string[] FetchColumnNames()
        {
            return this.ColumnInfoList.Select(ci => ci.Name).ToArray();
        }

        /// <summary>
        /// Fetch the GiftLabelInfo objects that have the
        /// appropriate year, request type and donor code. Sort
        /// them by family id, then child's name.
        /// Then, make a list of string arrays, with each string
        /// array containing data for one GLI.
        /// </summary>
        /// <returns></returns>
        protected override List<string[]> FetchData()
        {
            List<string[]> retList = new List<string[]>();

            var query = context.GliList.Where(
                s => (s.year == this.Year) &&
                (s.request_type == this.RequestType) &&
                (s.donor_code == this.Dnr.code)
                )
               .OrderBy(s => s.family_id).ThenBy(s => s.child_name);
            foreach (GiftLabelInfo i in query)
            {
                retList.Add(
                                new string[] { i.family_id, i.child_name, i.child_gender,
                                        i.child_age.ToString(), i.request_type, i.request_detail }
                            );
            }
            return retList;
        }

        /// <summary>
        /// Construct a worksheet name from the donor code, year,
        /// and request type.
        /// </summary>
        protected override string WorksheetName
        {
            get
            {
                string dn = Utils.TextUtils.CleanString(this.Dnr.code);
                return string.Format(Properties.Resources.DonorListWorksheetName,
                    dn, this.Year, this.RequestType);
            }
        }

        /// <summary>
        /// Output filespec is constructed from string "Donor_List",
        /// year, request type, and donor code.
        /// </summary>
        protected override string GetOutputFileSpec()
        {
            string dc = Utils.TextUtils.CleanString(this.Dnr.code).ToUpper();
            string name = string.Format(Properties.Resources.DonorListBasefilename,
                this.Year, this.RequestType, dc);
            return Path.Combine(this.TargetFolder(), name);
        }

        protected override string TargetFolder()
        {
            return FolderManager.ChristmasProgramFolder(Year);
        }
    }


    public class MasterListWriter : ListWriter
    {
        public Donor Dnr { get; set; }
        public string RequestType { get; set; }
        public int Year { get; set; }

        public MasterListWriter(BackgroundWorker wk, DBWrapper ctx,
            Donor d, string request_type, int year)
            : base(wk, ctx)
        {
            this.Dnr = d;
            this.RequestType = request_type;
            this.Year = year;
            this.Init();
        }

        /// <summary>
        /// MasterListWriter headers are
        /// [ "Master List", <request type>, <year>, <donor name> ]
        /// </summary>
        /// <returns></returns>
        protected override string[] FetchHeaders()
        {
            return new string[]
            {
                string.Format(Properties.Resources.MasterListHeader,
                this.RequestType, this.Year, this.Dnr.name)
            };
        }

        protected override string[] FetchColumnNames()
        {
            return this.ColumnInfoList.Select(ci => ci.Name).ToArray();
        }

        /// <summary>
        /// Fetch the GiftLabelInfo objects that have the
        /// appropriate year, request type and donor code. Sort
        /// them by family name, then child's name.
        /// Then, make a list of string arrays, with each string
        /// array containing data for one GLI.
        /// </summary>
        /// <returns></returns>
        protected override List<string[]> FetchData()
        {
            List<string[]> retList = new List<string[]>();
                var query = context.GliList.Where(
                    s => (s.year == this.Year) &&
                    (s.request_type == this.RequestType) &&
                    (s.donor_code == this.Dnr.code)
                    )
                   .OrderBy(s => s.family_name).ThenBy(s => s.child_name);
                foreach (GiftLabelInfo i in query)
                {
                    retList.Add(
                                    new string[] { i.family_id, i.family_name, i.child_name, i.child_gender,
                                        i.child_age.ToString(), i.request_type, i.request_detail }
                                );
                }
            return retList;
        }

        /// <summary>
        /// Worksheet name is constructed from donor code,
        /// year, the string "ML", and the request type.
        /// </summary>
        protected override string WorksheetName
        {
            get
            {
                string dc = Utils.TextUtils.CleanString(this.Dnr.code);
                return string.Format(Properties.Resources.MasterListWorksheetName,
                    dc, this.Year, this.RequestType);
            }
        }

        /// <summary>
        /// Output filespec is constructed from year,
        /// request type, and donor code.
        /// </summary>
        /// <returns></returns>
        protected override string GetOutputFileSpec()
        {
            string dc = Utils.TextUtils.CleanString(this.Dnr.code).ToUpper();
            string name = string.Format(Properties.Resources.MasterListBasefilename,
                this.Year, this.RequestType, dc);
            return Path.Combine(this.TargetFolder(), name);
        }

        /// <summary>
        /// Specify header text and widths for relevant columns.
        /// -1 for width means that the
        /// column's width will be set later based on the
        /// length of the contents.
        /// </summary>
        protected override void PopulateColumnInfo()
        {
            this.ColumnInfoList = new List<ColumnInfo>();
            this.ColumnInfoList.Add(new ColumnInfo("Fam ID", -1,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Family Name", -1,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Child", -1,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Gen", 4,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Age", 4,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Req. Type", -1,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add(new ColumnInfo("Request Detail", 35,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
        }

        protected override string TargetFolder()
        {
            return FolderManager.ChristmasProgramFolder(Year);
        }
    }


    public class ParticipantListWriter : ListWriter
    {
        public string ServiceType { get; set; }
        public int Year { get; set; }

        public ParticipantListWriter(BackgroundWorker wk, DBWrapper ctx,
            string srv_type, int year)
            : base(wk, ctx)
        {
            this.ServiceType = srv_type;
            this.Year = year;
            this.Init();
        }

        /// <summary>
        /// Specify header text and widths for relevant columns.
        /// -1 for width means that the
        /// column's width will be set later based on the
        /// length of the contents.
        /// </summary>
        protected override void PopulateColumnInfo()
        {
            this.ColumnInfoList = new List<ColumnInfo>();
            this.ColumnInfoList.Add( new ColumnInfo("Head of Household", 30,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add( new ColumnInfo("Primary Phone", 15,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add( new ColumnInfo("Address", 25,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add( new ColumnInfo("City", 15,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add( new ColumnInfo("ST", 5,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add( new ColumnInfo("Zip", 7,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
        }

        protected override string[] FetchColumnNames()
        {
            return this.ColumnInfoList.Select(ci => ci.Name).ToArray();
        }

        /// <summary>
        /// Fetch ServicesHouseholdEnrollment objects with
        /// the appropriate year and service type. Sort the
        /// list by head of household. Then add each one's
        /// data to our return list of string arrays.
        /// </summary>
        /// <returns></returns>
        protected override List<string[]> FetchData()
        {
            List<string[]> retList = new List<string[]>();
                var query = context.HoEnrList.Where(
                    s => (s.year == this.Year) &&
                    (s.service_type == this.ServiceType)
                    )
                   .OrderBy(s => s.head_of_household);
                foreach (ServicesHouseholdEnrollment e in query)
                {
                    retList.Add(
                                    new string[] { e.head_of_household, e.phone, e.address,
                                        e.city, e.state_or_province,
                                        Utils.TextUtils.CanonicalPostalCode(e.postal_code) }
                                );
                }
            return retList;
        }

        /// <summary>
        /// ParticipantListWriter header is
        /// [ <service type>, <year> ]
        /// </summary>
        /// <returns></returns>
        protected override string[] FetchHeaders()
        {
            return new string[]
            {
                string.Format(Properties.Resources.ParticipantListHeader,
                this.ServiceType, this.Year)
            };
        }

        /// <summary>
        /// ParticipantListWriter worksheet names
        /// are the service type, canonicalized.
        /// </summary>
        protected override string WorksheetName
        {
            get { return Utils.TextUtils.CleanString(this.ServiceType); }
        }

        /// <summary>
        /// Output filespec is constructed from
        /// year and service type.
        /// </summary>
        /// <returns></returns>
        protected override string GetOutputFileSpec()
        {
            string name = string.Format(Properties.Resources.ParticipantListBasefilename,
                this.Year,
                Utils.TextUtils.CleanString(this.ServiceType));
            return Path.Combine(this.TargetFolder(), name);
        }

        protected override string TargetFolder()
        {
            return FolderManager.PostcardsAndParticipantsFolder(Year, ServiceType);
        }
    }

    public class ThanksgivingDeliveryList : ListWriter
    {
        public string ServiceType { get; set; }
        public int Year { get; set; }

        public ThanksgivingDeliveryList(BackgroundWorker wk, DBWrapper ctx,
            int year)
            : base(wk, ctx)
        {
            this.ServiceType = "Thanksgiving basket";
            this.Year = year;
            this.Init();
        }

        /// <summary>
        /// Specify header text and widths for relevant columns.
        /// -1 for width means that the
        /// column's width will be set later based on the
        /// length of the contents.
        /// </summary>
        protected override void PopulateColumnInfo()
        {
            this.ColumnInfoList = new List<ColumnInfo>();
            this.ColumnInfoList.Add( new ColumnInfo("Head of Household", 28,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add( new ColumnInfo("Primary Phone", 13,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add( new ColumnInfo("Address", 22,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add( new ColumnInfo("City", 12,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
            this.ColumnInfoList.Add( new ColumnInfo("ST", 4,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add( new ColumnInfo("Zip", 7,
                ExcelHorizontalAlignment.Center,
                ExcelVerticalAlignment.Top, false));
            this.ColumnInfoList.Add( new ColumnInfo("Directions/Notes", 35,
                ExcelHorizontalAlignment.Left,
                ExcelVerticalAlignment.Top, true));
        }

        protected override string[] FetchColumnNames()
        {
            return this.ColumnInfoList.Select(ci => ci.Name).ToArray();
        }

        /// <summary>
        /// Fetch ServicesHouseholdEnrollment objects with
        /// the appropriate year and service type. Sort the
        /// list by head of household. Then add each one's
        /// data to our return list of string arrays.
        /// </summary>
        /// <returns></returns>
        /// 
        protected override List<string[]> FetchData()
        {
            List<string[]> retList = new List<string[]>();
            var query = context.HoEnrList.Where(
                s => (s.year == this.Year) &&
                (s.service_type == this.ServiceType)
                )
               .OrderBy(s => s.head_of_household);
            foreach (ServicesHouseholdEnrollment e in query)
            {
                retList.Add(
                                new string[] { e.head_of_household, e.phone, e.address,
                                        e.city, e.state_or_province,
                                        Utils.TextUtils.CanonicalPostalCode(e.postal_code),
                                ""}
                            );
            }
            return retList;
        }

        /// <summary>
        /// ParticipantListWriter header is
        /// [ <service type>, <year> ]
        /// </summary>
        /// <returns></returns>
        /// 
        protected override string[] FetchHeaders()
        {
            return new string[]
            {
                string.Format(Properties.Resources.ThanksgivingDeliveryListHeader, this.Year)
            };
        }

        /// <summary>
        /// ParticipantListWriter worksheet names
        /// are the service type, canonicalized.
        /// </summary>
        protected override string WorksheetName
        {
            get { return Properties.Resources.ThanksgivingDeliveryListSheetName; }
        }

        /// <summary>
        /// Output filespec is constructed from
        /// year and service type.
        /// </summary>
        /// <returns></returns>
        /// 
        protected override string GetOutputFileSpec()
        {
            string name = string.Format(Properties.Resources.ThanksgivingDeliveryBaseFilename,
                this.Year);
            return Path.Combine(this.TargetFolder(), name);
        }

        protected override string TargetFolder()
        {
            return FolderManager.PostcardsAndParticipantsFolder(Year, ServiceType);
        }
    }


}
