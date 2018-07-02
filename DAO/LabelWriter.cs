using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using Novacode; //for DocX
using Utils;
using GlobRes = AppWideResources.Properties.Resources;
namespace DAO
{
    /// <summary>
    /// Enum used to symbolically refer to
    /// the number of each type of unit
    /// in one inch.
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
    /// For some reason, Word uses different units
    /// to measure different parameters of the
    /// page and label layouts.
    /// </summary>
    public enum DocPartUnits
    {
        CellWidth = UnitRatios.Twips,
        CellHeight = UnitRatios.Dpi96,
        Margins = UnitRatios.Points,
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
        protected int PaddingWidth { get; set; }
        protected int LeftMargin { get; set; }
        protected int RightMargin { get; set; }
        protected int TopMargin { get; set; }
        protected int BottomMargin { get; set; }
        protected int NumberOfColumns { get; set; }
        protected Table table;
        protected List<object> ItemList { get; set; }
        protected const string FILE_EXTENSION = ".docx";
        protected int Year { get; set; }
        protected abstract string TargetFolder { get; }
        private BackgroundWorker Worker;
        protected DBWrapper context;

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
                string bu_name = Utils.FileUtils.NextAvailableBackupName(outfile_spec);
                File.Move(outfile_spec, bu_name);
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
        /// </summary>
        /// <param name="label_height"></param>
        /// <param name="label_width"></param>
        /// <param name="padding_width"></param>
        /// <param name="left_margin"></param>
        /// <param name="right_margin"></param>
        /// <param name="top_margin"></param>
        /// <param name="bottom_margin"></param>
        /// <param name="num_cols"></param>
        public LabelWriter(
            BackgroundWorker wk,
            DBWrapper ctx,
            int year,
            int label_height = (int)(1.066 * (int)DocPartUnits.CellHeight), // Avery 5160
            int label_width = (int)(2.63 * (int)DocPartUnits.CellWidth),
            int padding_width = (int)(0.12 * (int)DocPartUnits.CellWidth),
            int left_margin = (int)(0.19 * (int)DocPartUnits.Margins),
            int right_margin = (int)(0.19 * (int)DocPartUnits.Margins),
            int top_margin = (int)(0.5 * (int)DocPartUnits.Margins),
            int bottom_margin = 0,
            int num_cols = 5
            )
        {
            this.Worker = wk;
            this.context = ctx;
            this.Year = year;
            this.LabelHeight = label_height; this.LabelWidth = label_width;
            this.PaddingWidth = padding_width; this.LeftMargin = left_margin;
            this.RightMargin = right_margin; this.TopMargin = top_margin;
            this.BottomMargin = bottom_margin; this.NumberOfColumns = num_cols;
        }

        private Row addRow()
        {
            Row r = this.table.InsertRow();
            r.Height = this.LabelHeight;
            return r;
        }

        /// <summary>
        /// Assume odd-numbered columns are label cells and
        /// even-numbered ones are padding cells.
        /// </summary>
        private void setColumnWidths()
        {
            for (int i = 0; i < this.NumberOfColumns; i++)
            {
                if ((i % 2) == 0)
                    this.table.SetColumnWidth(i, this.LabelWidth);
                else
                    this.table.SetColumnWidth(i, this.PaddingWidth);
            }
        }

        private void setMargins(DocX doc)
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
                this.setMargins(doc);
                // add a table with one row and correct # of columns:
                this.table = doc.AddTable(1, this.NumberOfColumns);
                this.table.Alignment = Alignment.center;
                Row current_row = table.Rows[0];
                current_row.Height = this.LabelHeight;
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
                        current_row = this.addRow();
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
                // When all records are written into the table's cells,
                // set the table's column widths, insert the table into
                // the NovaCode document, save the document, and report
                // our progress.
                this.setColumnWidths();
                doc.InsertTable(this.table);
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
                    fn, e.Message
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

        public GiftLabelWriter(BackgroundWorker wk, DBWrapper ctx, Donor d, string request_type, int year)
            : base(wk, ctx, year)
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
            // NovaCode should initialize Cells with one
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
        /// A BagLabelWriter uses cell layout parameters different from the other label
        /// writers, so we override the values inherited from the abstract base class.
        /// </summary>
        /// <param name="wk"></param>
        /// <param name="ctx"></param>
        /// <param name="d"></param>
        /// <param name="request_type"></param>
        /// <param name="year"></param>
        public BagLabelWriter(BackgroundWorker wk, DBWrapper ctx, Donor d, string request_type, int year)
            : base(
                wk,
                ctx,
                year,
                label_height: (int)(5.276 * (int)DocPartUnits.CellHeight), // Avery 5168
                label_width: (int)(3.5 * (int)DocPartUnits.CellWidth),
                padding_width: (int)(0.5 * (int)DocPartUnits.CellWidth),
                left_margin: (int)(0.5 * (int)DocPartUnits.Margins),
                right_margin: (int)(0.5 * (int)DocPartUnits.Margins),
                num_cols: 3
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
            // NovaCode should initialize Cells with one
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

    public class PostcardLabelWriter : LabelWriter
    {
        private string ServiceType { get; set; }
        public PostcardLabelWriter(BackgroundWorker wk, DBWrapper ctx,
            string service_type, int year)
            : base(wk, ctx, year) // use defaults for all dimensions
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
            // NovaCode should initialize Cells with one
            // paragraph. But, in the unlikeley case
            // that that paragraph isn't there, add one.
            if (p == null)
                p = c.InsertParagraph();
            p.Append(e.head_of_household).Bold()
                .AppendLine(e.address)
                .AppendLine(e.city + ", " + e.state_or_province + "  " + 
                Utils.TextUtils.CanonicalPostalCode(e.postal_code));
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
}
