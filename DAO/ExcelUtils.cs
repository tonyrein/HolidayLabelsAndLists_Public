using System.Collections.Generic;

using OfficeOpenXml;

using System.IO;
namespace DAO
{

    /// <summary>
    /// Class for handling a collection of strings,
    /// interpreted as the contents of a spreadsheet row.
    /// </summary>
    public class ExcelRow
    {
        private List<string> Cells;
        public int Length { get { return Cells.Count; } }

        /// <summary>
        /// Allows retrieval of cells using array notation ( MyRow[10], for example )
        /// 
        /// Returns contents of this row's Cells data member at the given index.
        /// 
        /// Returns null if index is less than zero or >= length of Cells
        /// 
        /// NOTE: index is zero-based.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                if (index >= 0 && index < this.Length)
                    return this.Cells[index];
                else
                    return null;
            }
            set
            {
                if (index >= 0 && index < this.Length)
                    this.Cells[index] = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ExcelRow()
        {
            this.Cells = new List<string>();
        }

        /// <summary>
        /// Construct a DAO.ExcelRow from a List of string.
        /// </summary>
        /// <param name="stringlist"></param>
        public ExcelRow(List<string> stringlist)
        {
            this.Cells = stringlist;
        }
       
        /// <summary>
        /// Return the contents of this row as an
        /// array of strings.
        /// </summary>
        /// <returns></returns>
        public string[] ToStringArray()
        {
            return this.Cells.ToArray();
        }

        /// <summary>
        /// If the first element of this row's Cells member
        /// is null or empty, interpret that as this row being empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return this.Cells[0] == null;
        }
    }

    /// <summary>
    /// Class for handling a collection of ExcelRows
    /// </summary>
    public class ExcelSheet
    {
        public string Name { get; set; }
        public List<ExcelRow> Rows { get; }
       
        ///
        /// default constructor
        /// 
        public ExcelSheet()
        {
            this.Rows = new List<ExcelRow>();
        }

        public ExcelSheet(string name, List<List<string>> rows)
            : this()
        {
            this.Name = name;
            foreach (List<string> r in rows)
            {
                this.Rows.Add(new ExcelRow(r));
            }

        }

        /// <summary>
        /// Allow addressing with array syntax ( MySheet[20], for example )
        /// 
        /// Returns the ExcelRow at the given index, or null if the
        /// index is less than zero or >= to this sheet's row count.
        /// 
        /// NOTE: index is zero-based.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ExcelRow this[int index]
        {
            get
            {
                if (index >= 0 && index < this.Rows.Count)
                    return this.Rows[index];
                else
                    return null;
            }
            set
            {
                if (index >= 0 && index < this.Rows.Count)
                    this.Rows[index] = value;
            }
        }

        /// <summary>
        /// 
        /// Search through the rows to find the one which
        /// contains the indicated text in the indicated slot
        /// (the column number).
        /// If no column number is given, default to 0 (that is, column "A").
        /// 
        /// Return -1 if text isn't found.
        /// 
        /// NOTE: Row and column indices are ZERO-BASED!
        /// NOTE: Not all rows need to have the same number of columns.
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="col_idx"></param>
        /// <returns></returns>
        public int RowIndexOf(string val, int col_idx = 0)
        {
            for (int row_idx = 0; row_idx < this.Rows.Count; row_idx++)
            {
                if (this.ValueAt(row_idx, col_idx) == val)
                    return row_idx;
            }
            return -1;
        }

        /// <summary>
        /// Return the value of the cell at (row_idx, col_idx)
        /// 
        /// Return null for invalid row or column
        /// 
        /// NOTE: indices are ZERO-BASED, unlike in Interop.Excel
        /// </summary>
        /// <param name="row_idx"></param>
        /// <param name="col_idx"></param>
        /// <returns></returns>
        public string ValueAt(int row_idx, int col_idx)
        {
            return this[row_idx][col_idx];
        }
    }

    /// <summary>
    /// In-Memory representation of one or more Excel
    /// worksheets.
    /// </summary>
    public class WorkbookWrapper
    {
        public List<ExcelSheet> Sheets { get; }
        private string FullName { get; set; }
        private string Name { get; set; }

        /// <summary>
        /// Construct a WorkbookWrapper from an Excel file.
        /// </summary>
        /// <param name="filespec"></param>
        public WorkbookWrapper(string filespec)
        {
            this.Sheets = new List<ExcelSheet>();
            this.LoadWorkbook(filespec);
        }

        /// <summary>
        /// 
        /// Set this WorkbookWrapper's Name property
        /// from the name of an Excel file, and
        /// load the contents of that file.
        /// 
        /// </summary>
        /// <param name="filespec"></param>
        public void LoadWorkbook(string filespec)
        {
            this.FullName = filespec;
            this.Name = Path.GetFileNameWithoutExtension(filespec);
            FileInfo fi = new FileInfo(filespec);
            using (ExcelPackage pkg = new ExcelPackage(fi))
            {
                this.LoadSheets(pkg.Workbook);
            }



        }

        /// <summary>
        /// Load the contents of this workbook's
        /// worksheets into DAO.ExcelSheet objects.
        /// 
        /// Note that the Worksheets collection
        /// indexing is 1-based.
        /// </summary>
        /// <param name="bk"></param>
        public void LoadSheets(ExcelWorkbook bk)
        {
            this.Sheets.Clear();
            if (bk != null)
            {
                for (int i = 1; i <= bk.Worksheets.Count; i++)
                {
                    OfficeOpenXml.ExcelWorksheet sh = bk.Worksheets[i];
                    List<List<string>> sheet_contents = ExcelUtils.OoXmlGetSheetContents(sh);
                    string name = sh.Name;
                    this.Sheets.Add(new ExcelSheet(name, sheet_contents));
                }
            }
        }

        /// <summary>
        /// Does this WorkbookWrapper have a sheet with the
        /// given name? If so, return it; otherwise return null.
        /// 
        /// NOTE: Match is case-sensetive -- "SHEET33" will not match "sheet33"
        /// </summary>
        /// <param name="sheet_name"></param>
        /// <returns></returns>
        public ExcelSheet SheetByName(string sheet_name)
        {
            foreach (ExcelSheet sh in this.Sheets)
            {
                if (sh.Name == sheet_name)
                    return sh;
            }
            return null;
        }
    }

    public static class ExcelUtils
    {
       
        /// <summary>
        /// Given a range, assumed to be a single cell,
        /// return the contents as a string.
        /// 
        /// Return empty string if range is null or if
        /// range's value is null.
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string OoXmlGetCellContents(OfficeOpenXml.ExcelRange c)
        {
            string retString;
            if (c == null || c.Value == null)
                retString = "";
            else
            {
                retString = c.Value.ToString();
            }
            return retString;
        }

        /// <summary>
        /// Fill in a List of Lists of string from the contents of
        /// an OoXml ExcelWorksheet
        /// </summary>
        /// <param name="sh"></param>
        /// <returns></returns>
        public static List<List<string>> OoXmlGetSheetContents(OfficeOpenXml.ExcelWorksheet sh)
        {
            // Make a List of Lists to hold the return values:
            List<List<string>> retList = new List<List<string>>();
            // Find the parameters of this sheet:
            int start_row = sh.Dimension.Start.Row;
            int end_row = sh.Dimension.End.Row;
            int first_col = sh.Dimension.Start.Column;
            int last_col = sh.Dimension.End.Column;
            // Iterate over the rows; for each one, fill
            // in a List of strings from its contents and
            // add that List to our return List of Lists:
            for (int i = start_row; i <= end_row; i++)
            {
                List<string> celllist = new List<string>();
                // OoXml indexing here is 1-based!
                for (int col = first_col; col <= last_col; col++)
                {
                    string contents = OoXmlGetCellContents(sh.Cells[i, col]);
                    celllist.Add(contents);
                }
                // If all cells are null or blank, mark this row as empty:
                if (celllist.TrueForAll(c=>string.IsNullOrEmpty(c)))
                {
                    celllist.Clear();
                    celllist.Add(null);
                }
                retList.Add(celllist);
            }
            return retList;
        }

    }
}
