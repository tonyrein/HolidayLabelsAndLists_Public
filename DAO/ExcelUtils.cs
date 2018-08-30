using System.Collections.Generic;

//using NPOI.SS.UserModel;
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
        private string[] Cells;
        public int Length { get { return Cells.Length; } }

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
                if (index >= 0 && index < this.Cells.Length)
                    return this.Cells[index];
                else
                    return null;
            }
            set
            {
                if (index >= 0 && index < this.Cells.Length)
                    this.Cells[index] = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ExcelRow()
        {
            this.Cells = new string[] { "" };
        }

        /// <summary>
        /// Construct a DAO.ExcelRow from a List of string.
        /// </summary>
        /// <param name="stringlist"></param>
        public ExcelRow(List<string> stringlist)
        {
            this.Cells = stringlist.ToArray();
        }
       
        /// <summary>
        /// Return the contents of this row as an
        /// array of strings.
        /// </summary>
        /// <returns></returns>
        public string[] ToStringArray()
        {
            return this.Cells;
        }

        /// <summary>
        /// If the first element of this row's Cells member
        /// is null or empty, interpret that as this row being empty.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this[0]);
        }
    }

    /// <summary>
    /// Class for handling a collection of ExcelRows
    /// </summary>
    public class ExcelSheet
    {
        public string Name { get; set; }
        public List<ExcelRow> Rows { get; }

       
        /// <summary>
        /// Construct a DAO.ExcelSheet from an OfficeOpenXML worksheet
        /// </summary>
        /// <param name="sh"></param>
        public ExcelSheet(OfficeOpenXml.ExcelWorksheet sh)
        {
            this.Name = sh.Name;
            this.Rows = new List<ExcelRow>();
            int start_row = sh.Dimension.Start.Row;
            int end_row = sh.Dimension.End.Row;
            int first_col = sh.Dimension.Start.Column;
            int last_col = sh.Dimension.End.Column;
            for(int i = start_row; i <= end_row; i++)
            {
                List<string> celllist = new List<string>();
                for(int col = first_col; col <= last_col; col++)
                {
                    var c = sh.Cells[i, col];
                    var v = c.
                    //var ctnt = sh.Cells[i, col].Value;
                    //if (ctnt.)
                    celllist.Add(sh.Cells[i, col].Text);
                }
                ExcelRow r = new ExcelRow(celllist);
                this.Rows.Add(r);
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
                    this.Sheets.Add(new ExcelSheet(bk.Worksheets[i]));
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
        ///// <summary>
        /////  Given an NPOI IRow, return an array of
        /////  string, with each slot in the array holding
        /////  the contents of a cell.
        /////  
        /////  NPOI IRows expose LastCellNum and FirstCellNum
        /////  properties. The FirstCellNum is the zero-based
        /////  index of the first non-empty cell in the row. The
        /////  LastCellNum is the zero-based index of the cell
        /////  immediately following the last non-empty cell.
        /////  To put it another way, FirstCellNum is the column
        /////  number of the left-most cell which contains data,
        /////  and LastCellNum - 1 is the column number of the
        /////  right-most cell which contains data.
        ///// </summary>
        ///// <param name="row"></param>
        ///// <returns>
        /////   If row is null (corresponding to an empty Excel row),
        /////   return string[1] containing only null.
        /////   Otherwise, fill all array slots with cell string
        /////   values, or "" for cells that are null (corresponding
        /////   to empty Excel cells).
        /////   
        /////   Cell contents which are not strings (eg date/times or formulas)
        /////   are converted to strings.
        /////   
        ///// </returns>
        //public static string[] IRowToStringArray(IRow row)
        //{
        //    // Initialize array to be returned:
        //    string[] retArray = null;
        //    // If IRow is null, the underlying Excel row
        //    // is empty. Return an array of string
        //    // with only one slot, containing null, to
        //    // encode an empty row.
        //    if (row == null)
        //    {
        //        retArray = new string[] { null };
        //    }
        //    else
        //    {
        //        // If IRow is not null, then loop over
        //        // each of its cells.
        //        // If a cell is null, add an empty string
        //        // to our return array. Otherwise, change
        //        // the cell type into a string and add that
        //        // string to our return array.
        //        retArray = new string[row.LastCellNum];
        //        for (int i = 0; i < row.LastCellNum; i++)
        //        {
        //            ICell c = row.GetCell(i);
        //            if (c == null)
        //            {
        //                retArray[i] = "";
        //            }
        //            else
        //            {
        //                c.SetCellType(CellType.String);
        //                retArray[i] = c.StringCellValue;
        //            }
        //        }
        //    }
        //    return retArray;
        //}

        public static string[] OOXMLRowToStringArray(OfficeOpenXml.ExcelRow row)
        {
            // Initialize array to be returned:
            string[] retArray = null;
            // If OOXML Row is null, the underlying Excel row
            // is empty. Return an array of string
            // with only one slot, containing null, to
            // encode an empty row.
            if (row == null)
            {
                retArray = new string[] { null };
            }
            return retArray;
        }
    }
}
