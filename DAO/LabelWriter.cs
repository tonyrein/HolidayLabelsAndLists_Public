﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using Xceed.Words.NET; //for DocX
using Utils;
using GlobRes = AppWideResources.Properties.Resources;
namespace DAO
{
    /// <summary>
    /// Enum used to symbolically refer to
    /// the number of each type of unit
    /// in one inch. (One inch is approx. 2.54 cm.)
    /// </summary>
    public enum UnitRatios
    {
        Inches = 1,
        Picas = 6,
        Points = 72,
        Twips = 1440,
        Dpi96 = 96,
    }

    /// <summary>
    /// The units used by the EPPlus library to specify
    /// different parameters of page and table cell layouts.
    /// </summary>
    public enum DocPartUnits
    {
        CellWidth = UnitRatios.Twips,
        CellHeight = UnitRatios.Points,
        Margins = UnitRatios.Points,
        PageWidth = UnitRatios.Points,
        PageHeight = UnitRatios.Points,
    }


    /// <summary>
    /// Handles the creation of documents suitable for printing on label stock.
    /// 
    /// Uses a NovaCode Table object as a target for the output.
    /// This is the abstract base class -- the subclasses are specific to particular
    /// label types.
    /// </summary>
    public abstract class LabelWriter
    {
        protected int LabelHeight { get; set; }
        protected int LabelWidth { get; set; }
        protected int PageWidth { get; set; }
        protected int PageHeight { get; set; }
        protected int HorizontalPadding { get; set; }
        protected int VerticalPadding { get; set; }
        protected int LeftMargin { get; set; }
        protected int RightMargin { get; set; }
        protected int TopMargin { get; set; }
        protected int BottomMargin { get; set; }
        protected int NumberOfColumns { get; set; }
        protected int LabelRowsPerPage { get; }
        protected int TotalRowsPerPage { get;  }
        protected Orientation Orientation { get; set; }
        protected Table table;
        protected List<object> ItemList { get; set; }
        protected const string FILE_EXTENSION = ".docx";
        protected int Year { get; set; }
        protected abstract string TargetFolder { get; }
        private BackgroundWorker Worker;
        protected DBWrapper context;
        protected int RowHeight { get { return this.LabelHeight + this.VerticalPadding; } }
        /// <summary>
        /// Create target directory if it does not already exist.
        /// 
        /// Get the output file spec. If this file already exists,
        /// rename it with a backup file name.
        /// 
        /// Create a DocX object on the output file spec.
        /// 
        /// </summary>
        /// <returns></returns>
        public DocX OpenDocument()
        {
            if (!Directory.Exists(this.TargetFolder))
                Directory.CreateDirectory(this.TargetFolder);
            string outfile_spec = this.GetOutputFileSpec();
            if (File.Exists(outfile_spec))
            {
                Utils.FileUtils.MoveToBackup(outfile_spec);
            }
            return DocX.Create(outfile_spec);
        }

        /// <summary>
        /// The main thing this constructor does is initialize the dimensions.
        /// For some reason, different units are used for different parameters:
        ///    Cell width: twips
        ///    Cell height: 96 dots per inch
        ///    Margins: points
        ///    
        /// The values in the abstract base class constructor are defaults
        /// used by most of the label types in this application.
        /// 
        public LabelWriter
            (
            BackgroundWorker wk,
            DBWrapper ctx,
            int year,
            int label_height = (int)(1.066 * (int)DocPartUnits.CellHeight), // Avery 5160
            int label_width = (int)(2.63 * (int)DocPartUnits.CellWidth),
            int horizontal_padding = (int)(0.12 * (int)DocPartUnits.CellWidth),
            int vertical_padding = (int)(0.00 * (int)DocPartUnits.CellHeight),
            int left_margin = (int)(0.19 * (int)DocPartUnits.Margins),
            int right_margin = (int)(0.19 * (int)DocPartUnits.Margins),
            int top_margin = (int)(0.5 * (int)DocPartUnits.Margins),
            int bottom_margin = 0,
            int num_cols = 5,
            int label_rows_per_page = 2,
            int page_width = (int)(8.5 * (int)DocPartUnits.PageWidth),
            int page_height = (int)(11.0 * (int)DocPartUnits.PageHeight),
            Orientation orientation = Orientation.Portrait
            )
        {
            this.Worker = wk;
            this.context = ctx;
            this.Year = year;
            this.LabelHeight = label_height; this.LabelWidth = label_width;
            this.HorizontalPadding = horizontal_padding;
            this.VerticalPadding = vertical_padding; this.LeftMargin = left_margin;
            this.PageWidth = page_width;
            this.PageHeight = page_height;
            this.RightMargin = right_margin; this.TopMargin = top_margin;
            this.BottomMargin = bottom_margin; this.NumberOfColumns = num_cols;
            this.Orientation = orientation;
            if (this.VerticalPadding > 0)
                this.TotalRowsPerPage = (2 * this.LabelRowsPerPage) - 1;
            else
                this.TotalRowsPerPage = this.LabelRowsPerPage;
        }

        private void AddPaddingRow()
        {
            Row r = this.table.InsertRow();
            r.Height = this.VerticalPadding;
            SetPropertiesOneRow(r);
        }

        private Row AddRow()
        {
            if (this.VerticalPadding > 0)
            {
                // If the last row added was NOT the last
                // row of a page, put in a padding row:
                int row_count = this.table.RowCount;
                if ((row_count % this.TotalRowsPerPage) != 0)
                    this.AddPaddingRow();
            }
            Row r = this.table.InsertRow();
            SetHeightOneRow(r);
            SetPropertiesOneRow(r);
            return r;
        }

        /// <summary>
        /// Assume odd-numbered columns are label cells and
        /// even-numbered ones are padding cells.
        /// </summary>
        private void SetColumnWidths()
        {
            for (int i = 0; i < this.NumberOfColumns; i++)
            {
                if ((i % 2) == 0)
                    this.table.SetColumnWidth(i, this.LabelWidth);
                else
                    this.table.SetColumnWidth(i, this.HorizontalPadding);
            }
        }
        
        private void SetHeightOneRow(Row r)
        {
            r.Height = this.LabelHeight;
        }

        private void SetPropertiesOneRow(Row r)
        {
            r.BreakAcrossPages = false;
        }

        private void SetAllRowProperties()
        {
            foreach (Row r in this.table.Rows)
            {
                SetPropertiesOneRow(r);
                SetHeightOneRow(r);
            }

        }

        private void SetMargins(DocX doc)
        {
            doc.MarginBottom = this.BottomMargin;
            doc.MarginLeft = this.LeftMargin;
            doc.MarginRight = this.RightMargin;
            doc.MarginTop = this.TopMargin;
        }

        /// <summary>
        /// Insert label cells into the output document.
        /// 
        /// Return 1 if file written, else 0
        /// 
        /// </summary>
        public int TypeAllRecords()
        {

            // Don't do anything unless there are records to write
            if (this.ItemList.Count == 0)
                return 0;
            string fn = Path.GetFileName(this.GetOutputFileSpec());
            int retInt;
            this.Worker.ReportProgress(0,
                string.Format(GlobRes.CountWritingMsg,
                    this.ItemList.Count, "labels", fn)
                    );
            int col_idx = 0;
            DocX doc = null;
            try
            {
                doc = this.OpenDocument();
                doc.PageLayout.Orientation = this.Orientation;
                doc.PageWidth = this.PageWidth;
                doc.PageHeight = this.PageHeight;
                doc.PageLayout.Orientation = this.Orientation;
                this.SetMargins(doc);
                // Add a table with one row and correct # of columns,
                // but do not insert it into the document yet.
                this.table = doc.AddTable(1, this.NumberOfColumns);
                this.table.Alignment = Alignment.center;
                // Get a reference to the table's first row:
                Row current_row = this.table.Rows[0];
                SetHeightOneRow(current_row);
                SetPropertiesOneRow(current_row);
                // Loop through records. "Type" each record's
                // contents into the next label cell, starting
                // with the first cell in the first (and so far
                // the only) row. Add new rows as needed.
                foreach (object item in this.ItemList)
                {
                    // End of this row? If so, add
                    // a new row and go to left-hand cell.
                    if (col_idx == this.NumberOfColumns)
                    {
                        // Keep current row reference set
                        // to table's last row:
                        current_row = this.AddRow();
                        col_idx = 0;
                    }
                    // Odd number? That means this cell is a padding
                    // cell -- don't write this record into it -- instead,
                    // do anything appropriate for padding cells (might
                    // well be nothing) and skip to next cell.
                    if ((col_idx % 2) != 0)
                    {
                        this.DoSpace(current_row.Cells[col_idx]);
                        col_idx++;
                    }
                    // Put this record's contents into this cell and
                    // go to next cell.
                    this.TypeOneRecord(current_row.Cells[col_idx], item);
                    col_idx++;
                }
                // Set table's properties BEFORE
                // inserting the table into the document:
                this.SetColumnWidths();
                //this.SetAllRowProperties();
                doc.InsertTable(this.table);
                //this.SetMargins(doc);
                doc.Save();
                this.Worker.ReportProgress(0,
                    string.Format(GlobRes.FileCreationSuccessMsg, fn)
                    );
                retInt = 1;
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(
                    GlobRes.FileExceptionErrorMsg,
                    fn, e.Message, e.StackTrace
                );
                this.Worker.ReportProgress(0, sb.ToString());
                retInt = 0;
            }
            finally
            {
                doc.Dispose();
            }
            return retInt;
        }

        /// <summary>
        /// Action to take for padding cells.
        /// 
        /// By default, this does nothing, but subclasses
        /// might override it.
        /// </summary>
        /// <param name="c"></param>
        protected void DoSpace(Cell c)
        {

        }

        /// <summary>
        /// Set cell parameters (margins, etc.) Then insert
        /// desired text fields from record into cell.
        /// </summary>
        /// <param name="c" - a NovaCode Cell object></param>
        /// <param name="rec"></param>
        protected abstract void TypeOneRecord(Cell c, object rec);

        /// <summary>
        /// Get list of items to be included in this label document
        /// </summary>
        protected abstract void SetItemList();

        /// <summary>
        /// Use this instance's properties to derive
        /// a filespec for the output file.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetOutputFileSpec();
    }

    public class GiftLabelWriter : LabelWriter
    {
        private Donor Dnr { get; set; }
        private string RequestType { get; set; }

        protected override string TargetFolder
        {
            get { return FolderManager.ChristmasProgramFolder(Year); }
        }

        /// <summary>
        /// Make a LabelWriter with default dimensions (Avery 5160)
        /// anda specified BackgroundWorker, DBWrapper, Donor,
        /// request type, and year
        /// </summary>
        public GiftLabelWriter(BackgroundWorker wk, DBWrapper ctx, Donor d, string request_type, int year)
            : base(wk, ctx, year, label_rows_per_page: 9)
        {
            this.Dnr = d;
            this.RequestType = request_type;
            this.SetItemList();
        }

        /// <summary>
        /// Initialize item list from context, a
        /// DBWrapper object. context has a list of
        /// GiftLabelInfo objects -- get those
        /// which match this GiftLabelWriter's
        /// year, donor code, donor name, and request type.
        /// </summary>
        protected override void SetItemList()
        {
            var query = context.GliList.Where(
                s => (s.year == this.Year) &&
                (s.donor_code == this.Dnr.code) &&
                (s.donor_name == this.Dnr.name) &&
                (s.request_type == this.RequestType)
                );
            this.ItemList = query.ToList<object>();
        }

        /// <summary>
        /// "Type" the contents of this record into a NovaCode Cell.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="rec"></param>
        protected override void TypeOneRecord(Cell c, object rec)
        {
            c.MarginTop = (int)(0.1 * (int)DocPartUnits.Margins);
            GiftLabelInfo gli = (GiftLabelInfo)rec;
            // Xceed.Words.NET should initialize Cells with one
            // paragraph. But, in the unlikeley case
            // that that paragraph isn't there, add one.
            Paragraph p = c.Paragraphs.First();
            if (p == null)
                p = c.InsertParagraph();
            string gen = gli.child_gender == "NotSpecified" ? "" : ", " + gli.child_gender;
            string top_line = $"{Dnr.code} {gli.family_id} {gli.child_name} {gli.child_age} Yr{gen}";
            p.Append(top_line).Bold()
                .AppendLine(gli.request_detail);
            if (this.RequestType == "Clothing")
                p.AppendLine(GlobRes.GiftReceiptRequest);
        }

        /// <summary>
        /// Construct output filespec from request type, donor code,
        /// and year. Return full path for this filespec.
        /// </summary>
        /// <returns></returns>
        protected override string GetOutputFileSpec()
        {
            string cln_rt = TextUtils.CleanString(this.RequestType);
            string cln_dcd = TextUtils.CleanString(this.Dnr.code).ToUpper();
            string name = string.Format(
                GlobRes.GiftLabelBaseFilename,
                this.Year, cln_rt, cln_dcd
                );
            return Path.Combine(this.TargetFolder, name);
        }
    }

    public class BagLabelWriter : LabelWriter
    {
        private Donor Dnr { get; set; }
        private string RequestType { get; set; }

        protected override string TargetFolder
        {
            get { return FolderManager.ChristmasProgramFolder(Year); }
        }

        /// <summary>
        /// Make a LabelWriter with Avery 5168 dimensions:
        /// =============
        /// Cell Height: 5.276"
        /// Cell Width: 3.5"
        /// Padding Width: 0.5"
        /// Left and Right Margins: 0.5"
        /// 
        /// Also specify a BackgroundWorker, DBWrapper, Donor,
        /// request type, and year.
        /// </summary>
        public BagLabelWriter(BackgroundWorker wk, DBWrapper ctx, Donor d, string request_type, int year)
            : base(
                wk,
                ctx,
                year,
                label_height: (int)(5.0 * (int)DocPartUnits.CellHeight), // Avery 5168
                label_width: (int)(3.5 * (int)DocPartUnits.CellWidth),
                horizontal_padding: (int)(0.5 * (int)DocPartUnits.CellWidth),
                left_margin: (int)(0.5 * (int)DocPartUnits.Margins),
                right_margin: (int)(0.5 * (int)DocPartUnits.Margins),
                num_cols: 3,
                label_rows_per_page: 2
              )
        {
            this.Dnr = d;
            this.RequestType = request_type;
            this.Year = year;
            this.SetItemList();
        }

        /// <summary>
        /// Add the BagLabelInfo objects with the desired
        /// year, donor code, donor name, and request type.
        /// </summary>
        protected override void SetItemList()
        {
            var query = context.BliList.Where(
                s => (s.year == this.Year) &&
                (s.donor_code == this.Dnr.code) &&
                (s.donor_name == this.Dnr.name) &&
                (s.request_type == this.RequestType)
                );
            this.ItemList = query.ToList<object>();
        }

        /// <summary>
        /// "Type" the contents of this record into a NovaCode Cell.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="rec"></param>
        protected override void TypeOneRecord(Cell c, object rec)
        {
            string request_type_code = this.RequestType.Substring(0, 1).ToUpper();
            System.Drawing.Color request_type_color;
            System.Drawing.Color default_color = System.Drawing.Color.Black;
            switch (request_type_code)
            {
                case "C": request_type_color = System.Drawing.Color.Green; break;
                case "T": request_type_color = System.Drawing.Color.Red; break;
                default: request_type_color = default_color; break;
            }

            c.MarginTop = 0;
            BagLabelInfo bli = (BagLabelInfo)rec;
            Paragraph p = c.Paragraphs.First();
            // Xceed.Words.NET should initialize Cells with one
            // paragraph. But, in the unlikeley case
            // that that paragraph isn't there, add one.
            if (p == null)
                p = c.InsertParagraph();
            p.SpacingBefore(0);
            p.SpacingAfter(0);
            p.Append(request_type_code).Color(request_type_color).FontSize(36).Bold()
                .Append(" " + this.Dnr.code + " " + bli.family_id).Color(default_color).FontSize(36)
                .AppendLine(bli.family_name).Color(default_color).FontSize(36).Bold()
                .AppendLine(bli.family_members).Color(default_color).FontSize(28);
        }

        /// <summary>
        /// Construct output filespec from request type, donor code,
        /// and year. Return full path for this filespec.
        /// </summary>
        /// <returns></returns>
        protected override string GetOutputFileSpec()
        {
            string cln_rt = TextUtils.CleanString(this.RequestType);
            string cln_dcd = TextUtils.CleanString(this.Dnr.code).ToUpper();
            string name = string.Format(
                GlobRes.BagLabelBaseFilename,
                this.Year, cln_rt, cln_dcd
                );
            return Path.Combine(this.TargetFolder, name);
        }
    }

    /// <summary>
    /// Make a LabelWriter with default dimensions (Avery 5160)
    /// and a specified BackgroundWorker, DBWrapper,
    /// service type, and year.
    /// </summary>
    public class PostcardLabelWriter : LabelWriter
    {
        private string ServiceType { get; set; }
        public PostcardLabelWriter(BackgroundWorker wk, DBWrapper ctx,
            string service_type, int year)
            : base(wk, ctx, year, label_rows_per_page: 9) // use defaults for all dimensions
        {
            this.ServiceType = service_type;
            this.SetItemList();
        }

        /// <summary>
        /// Get ienumerable yielding records with correct year.then
        /// group items by head of household and select first item
        ///     in each group, so we only get each head of household once.
        ///     then convert to list of objects.
        /// </summary>
        protected override void SetItemList()
        {
            var query = context.HoEnrList.Where(
                        s => (s.year == this.Year) &&
                        (s.service_type == this.ServiceType)
                        );
            this.ItemList = query.ToList()
                .GroupBy(s => s.head_of_household)
                .Select(g => g.First()).OrderBy(s => s.head_of_household)
                .ToList<object>();
        }

        /// <summary>
        /// "Type" the contents of this record into a NovaCode Cell.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="rec"></param>
        protected override void TypeOneRecord(Cell c, object rec)
        {
            c.MarginTop = (int)(0.1 * (int)DocPartUnits.Margins);
            ServicesHouseholdEnrollment e = (ServicesHouseholdEnrollment)rec;
            Paragraph p = c.Paragraphs.First();
            // Xceed.Words.NET should initialize Cells with one
            // paragraph. But, in the unlikeley case
            // that that paragraph isn't there, add one.
            if (p == null)
                p = c.InsertParagraph();
            string zip = Utils.TextUtils.CanonicalPostalCode(e.postal_code);
            p.Append(e.head_of_household).Bold()
                .AppendLine(e.address)
                .AppendLine(e.city + ", " + e.state_or_province + "  " + zip);
        }

        /// <summary>
        /// Construct output filespec from year.
        /// Return full path for this filespec.
        /// </summary>
        /// <returns></returns>
        protected override string GetOutputFileSpec()
        {
            string name = string.Format(
                GlobRes.PostcardLabelBaseFilename,
                this.Year,
                Utils.TextUtils.CleanString(this.ServiceType)
                );
            return Path.Combine(this.TargetFolder, name);
        }

        protected override string TargetFolder
        {
            get { return FolderManager.PostcardsAndParticipantsFolder(Year, ServiceType); }
        }
    }

    /// <summary>
    /// Dimensions:
    /// ======================
    /// Cell Height: 4.0"
    /// Cell Width: 3.33"
    /// Padding Width: 0.03"
    /// Top and Bottom Margins: 0.125"
    /// Left and Right Margins: 0.5"
    /// Landscape orientation
    /// </summary>
    public class ParticipantSummaryLabelWriter : LabelWriter
    {

        /// <summary>
        /// Make a LabelWriter with Avery 5614 dimensions
        /// and a specified BackgroundWorker, DBWrapper, and year.
        /// </summary>
        public ParticipantSummaryLabelWriter(BackgroundWorker wk, DBWrapper ctx, int year)
          : base(
            wk,
            ctx,
            year,
            label_width: (int)(3.33333 * (int)DocPartUnits.CellWidth),
            label_height: (int)(3.0 * (int)DocPartUnits.CellHeight),
            horizontal_padding: (int)(0 * (int)DocPartUnits.CellWidth),
            vertical_padding: (int)(0.375 * (int)DocPartUnits.CellHeight),
            top_margin: (int)(1.0 * (int)DocPartUnits.Margins),
            bottom_margin: (int)(1.0 * (int)DocPartUnits.Margins),
            left_margin: (int)(0.5 * (int)DocPartUnits.Margins),
            right_margin: (int)(0.5 * (int)DocPartUnits.Margins),
            num_cols: 5,
            label_rows_per_page: 2,
            orientation: Orientation.Landscape,
            page_width: (int)(11.0 * (int)DocPartUnits.PageWidth),
            page_height: (int)(8.5 * (int)DocPartUnits.PageHeight)
            )
        {
            this.SetItemList();
        }

        protected override string TargetFolder
        {
            // no ServiceType associated with this label type -- pass
            // empty string so we'll get back "Other" (the default).
            get { return FolderManager.PostcardsAndParticipantsFolder(Year, ""); }
        }

        protected override string GetOutputFileSpec()
        {
            string name = string.Format(
                  GlobRes.ParticipantSummaryLabelsBaseFilename,
                  this.Year
                  );
            return Path.Combine(this.TargetFolder, name);
        }

        /// <summary>
        /// For each ServicesHouseholdEnrollment object, if there
        /// are corresponding GiftLabelInfo objects for this writer's
        /// year:
        ///     For each such GiftLabelInfo object, make a FamiliesAndKids
        ///     object, fill its fields, and add it to our ItemList.
        /// </summary>
        protected override void SetItemList()
        {
            List<object> fkl = new List<object>();
            ServicesHouseholdEnrollment_DAO[] participant_array =
                this.context.HoEnrList.Select(h => h.dao).Where(h => h.year == this.Year).ToArray();
            foreach (ServicesHouseholdEnrollment_DAO participant in participant_array)
            {
                var gli_array = this.context.GliList.Where(g => (g.year == this.Year && g.family_id == participant.family_id)).ToArray();
                if (gli_array.Count() > 0)
                {
                    FamiliesAndKids fak = new FamiliesAndKids();
                    fak.dao = participant;
                    string[] sa = gli_array.Select(g => g.child_name).Distinct().ToArray();
                    fak.kids = string.Join(", ", sa);
                    fak.gift_card_count = gli_array.Where(g => g.donor_name == "Gift Cards").Count();
                    fkl.Add(fak);
                }
            }
            this.ItemList = fkl;
        }

        protected override void TypeOneRecord(Cell c, object rec)
        {
            // cast generic object to specific type
            // appropriate to this label type:
            FamiliesAndKids fk = (FamiliesAndKids)rec;
            c.MarginTop = 0;
            Paragraph p = c.Paragraphs.First();
            // NovaCode should initialize Cells with one
            // paragraph. But, in the unlikeley case
            // that that paragraph isn't there, add one.
            if (p == null)
                p = c.InsertParagraph();
            string zip = Utils.TextUtils.CanonicalPostalCode(fk.dao.postal_code);
            p.SpacingBefore(0);
            p.SpacingAfter(0);
            p.Append(fk.dao.head_of_household).FontSize(24).Bold()
                .AppendLine(fk.dao.phone).FontSize(18).Bold()
                .AppendLine(fk.dao.address).FontSize(16)
                .AppendLine(fk.dao.city + ", " + fk.dao.state_or_province + " " + zip).FontSize(16)
                .AppendLine("Gift Cards: " + fk.gift_card_count.ToString()).FontSize(16)
                .AppendLine("Number of Bags: ___").FontSize(16)
                .AppendLine("") // Another blank line before children's names
                .AppendLine("Children: " + fk.kids).FontSize(16);
        }
    }
}
