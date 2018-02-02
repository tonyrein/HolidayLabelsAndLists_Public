using System;
using System.IO;
using System.Runtime.InteropServices;

using Excel = Microsoft.Office.Interop.Excel;

using NPOI.SS.UserModel;

namespace DAO
{
    public static class ExcelInteropUtils
    {

        /// <summary>
        ///  Given an NPOI IRow, return an array of
        ///  string, with each slot in the array holding
        ///  the contents of a cell.
        ///  
        ///  NPOI IRows expose LastCellNum and FirstCellNum
        ///  properties. The FirstCellNum is the zero-based
        ///  index of the first non-empty cell in the row. The
        ///  LastCellNum is the zero-based index of the cell
        ///  immediately following the last non-empty cell.
        ///  To put it another way, FirstCellNum is the column
        ///  number of the left-most cell which contains data,
        ///  and LastCellNum - 1 is the column number of the
        ///  right-most cell which contains data.
        /// </summary>
        /// <param name="row"></param>
        /// <returns>
        ///   If row is null (corresponding to an empty Excel row),
        ///   return string[1] containing only "".
        ///   Otherwise, fill all array slots with cell string
        ///   values, or "" for cells that are null (corresponding
        ///   to empty Excel cells).
        /// </returns>
        public static string[] IRowToStringArray(IRow row)
        {
            if (row == null)
            {
                return new string[] { "" };
            }
            else
            {
                string[] retArray = new string[row.LastCellNum];
                for (int i = 0; i < row.LastCellNum; i++)
                {
                    ICell c = row.GetCell(i);
                    if (c == null)
                    {
                        retArray[i] = "";
                    }
                    else
                    {
                        c.SetCellType(CellType.String);
                        retArray[i] = c.StringCellValue;
                    }
                }
                return retArray;
            }
        }

        /// <summary>
        /// If Excel is already running, return a reference
        /// to that instance. Otherwise, start a new
        /// instance and return that.
        /// </summary>
        /// <returns></returns>
        public static Excel.Application GetExcelApp()
        {
            Excel.Application retApp = null;
            try
            {
                retApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
            }
            catch (COMException e)
            {
                retApp = GetNewExcelApp();
            }
            return retApp;
        }

        /// <summary>
        /// Start a fresh instance of Excel
        /// </summary>
        /// <returns></returns>
        public static Excel.Application GetNewExcelApp()
        {
            return new Excel.Application();
        }

        /// <summary>
        /// Given an Excel worksheet, return the count
        /// of used rows, in other words, the row number
        /// of the last row with content. This should work
        /// properly even if there are intervening blank
        /// rows.
        /// </summary>
        /// <param name="sh">
        ///     Microsoft.Office.Interop.Excel.Worksheet
        /// </param>
        /// <returns></returns>
        public static long lastRowIndex(Excel.Worksheet sh)
        {
            Excel.Range foundRange = sh.UsedRange.Find("*", sh.Cells[1, 1], Excel.XlFindLookIn.xlValues,
                Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious);
            if (foundRange == null)
            {
                return -1L;

            }
            else
            {
                return foundRange.Row;
            }
        }


        public static long rowIndexOf(Excel.Worksheet sh, string target_text, string colToSearch="A")
        {
            string col_address = string.Format("${0}$1:${0}${1}", colToSearch, sh.UsedRange.Rows.Count);
            Excel.Range range_to_search = sh.Range[col_address];
            Excel.Range foundRange = range_to_search.Find(What: target_text,
                LookIn: Excel.XlFindLookIn.xlValues,
                LookAt: Excel.XlLookAt.xlPart,
                SearchOrder: Excel.XlSearchOrder.xlByRows,
                SearchDirection: Excel.XlSearchDirection.xlPrevious);
            if (foundRange == null)
            {
                return -1L;
            }
            else
            {
                return foundRange.Row;
            }
        }

        public static string contentAt(Excel.Worksheet sh, long rowNumber, long colNumber)
        {
            return ((Excel.Range)sh.Cells[rowNumber, colNumber]).Value2;
        }

        public static string contentAt(Excel.Worksheet sh, string cellAddress)
        {
            return (sh.Range[cellAddress]).Value2;
        }

        /// <summary>
        /// If the workbook contains a sheet of the given name,
        /// return that worksheet. Otherwise, return null.
        /// </summary>
        /// <param name="bk">
        ///     Excel.Workbook
        /// </param>
        /// <param name="sheet_name">
        ///     string
        /// </param>
        /// <returns></returns>
        public static Excel.Worksheet SheetByName(Excel.Workbook bk, string sheet_name)
        {
            Excel.Worksheet retSheet = null;
            foreach (Excel.Worksheet sh in bk.Worksheets)
            {
                if (sh.Name == sheet_name)
                {
                    retSheet = sh;
                    break;
                }
            }
            return retSheet;
        }

        /// <summary>
        /// Get a reference to an open Excel workbook with the given name.
        /// 
        /// If there's already one by that name open AND if it has the
        /// given filespec (that is, if it's not a file in another folder that
        /// happens to have the same filename), return that.
        /// 
        /// If there's already one by that name open BUT it's not the same filespec,
        /// throw an exception
        /// 
        /// If there's not already one open by that name, try opening it. If successful,
        /// return that; otherwise, throw an exception.
        /// 
        /// NOTE: Excel cannot have more than one workbook with a given name open
        /// at one time, even if the duplicate is in a different folder.
        /// </summary>
        /// <param name="filespec"></param>
        /// <returns>
        ///  Excel.Workbook object if successful; otherwise raises
        ///  an exception
        /// </returns>
        public static Excel.Workbook OpenWorkbook(string filespec, bool read_only=true)
        {
            Excel.Workbook retBk = null;
            Excel.Application app = ExcelInteropUtils.GetNewExcelApp();
            string filename = Path.GetFileName(filespec);
            try
            {
                retBk = app.Workbooks[filename];
                // There was one by that name already open -- is
                // it the right one?
                if (retBk.FullName != filespec) // wrong one!
                {
                    throw new Exception(string.Format(
                        "Excel file named {0} already open. Close it and re-try",
                        filename));
                }
            }
            catch(Exception e)
            {
                // Open attempt failed. Figure out why:
                if (e is COMException || e is IndexOutOfRangeException)
                {
                    // No file by that name open -- try opening a fresh copy:
                    retBk = app.Workbooks.Open(filespec, ReadOnly: read_only);
                }
                else
                {
                    // Some other problem -- kick it upstairs.
                    throw;
                }
            }
            return retBk;
        }

    }
}
