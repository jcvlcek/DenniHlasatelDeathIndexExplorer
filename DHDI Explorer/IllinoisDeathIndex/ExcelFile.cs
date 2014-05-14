using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;

namespace Genealogy
{
    /// <summary>
    /// Basic wrapper class for working with Excel files in .NET
    /// </summary>
    public class ExcelFile
    {
        #region Private members
        private Excel.Application mXlApp = null;
        private Excel.Workbook mXlWorkBook = null;
        private Excel.Worksheet mXlDefaultWorkSheet = null;
        #endregion

        #region Constructors

        /// <summary>
        /// Open an Excel spreadsheet at a specified path
        /// </summary>
        /// <param name="sPath">the file path name of the Excel spreadsheet</param>
        public ExcelFile(string sPath)
        {
            if (mXlApp == null)
                mXlApp = new Excel.Application();
            Excel.Workbooks workBooks = mXlApp.Workbooks;
            mXlWorkBook = workBooks.Open(sPath,
                Excel.XlUpdateLinks.xlUpdateLinksAlways,
                true, 
                5, // Excel.XlFileFormat.xlWorkbookDefault,
                "", "", 
                false,
                Excel.XlPlatform.xlWindows,
                "\t", false, false, 0, true, false,
                Excel.XlCorruptLoad.xlNormalLoad);
                //        oWB = oXL.Workbooks.Open(path, 0, false, 5, "", "",
                //false, Excel.XlPlatform.xlWindows, "", true, false,
                //0, true, false, false);
            mXlDefaultWorkSheet = mXlWorkBook.Worksheets[1];
        }

        /// <summary>
        /// Create an empty Excel spreadsheet, with no associated file
        /// </summary>
        public ExcelFile()
        {
            if (mXlApp == null)
                mXlApp = new Excel.Application();
            Excel.Workbooks workBooks = mXlApp.Workbooks;
            mXlWorkBook = workBooks.Add();
            mXlDefaultWorkSheet = mXlWorkBook.Worksheets[1];
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Get the string value of a spreadsheet cell's contents
        /// </summary>
        /// <param name="where">the row (<paramref name="where"/>.Y) and column (<paramref name="where"/>.X) of the cell to inspect</param>
        /// <returns>the string value of the contents of the cell at <paramref name="where"/>, or the empty string on error</returns>
        public string ValueAt(Point where)
        {
            try
            {
                return mXlDefaultWorkSheet.Cells[where.Y, where.X].Value2.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Set the value of a spreadsheet cell's contents
        /// </summary>
        /// <param name="where">the row (<paramref name="where"/>.Y) and column (<paramref name="where"/>.X) of the cell to inspect</param>
        /// <param name="sValue">the value to be written to the cell at <paramref name="where"/></param>
        public void SetValueAt(Point where, string sValue)
        {
            mXlDefaultWorkSheet.Cells[where.Y, where.X].Value2 = sValue;
        }

        /// <summary>
        /// Save the spreadsheet to a specified file path
        /// </summary>
        /// <param name="sPath">the file path to which the spreadsheet is to be saved</param>
        public void SaveAs(string sPath)
        {
            mXlWorkBook.SaveAs(sPath, Excel.XlFileFormat.xlExcel8);
        }

        /// <summary>
        /// Close the spreadsheet
        /// </summary>
        /// <remarks><para>No testing for unsaved changes is performed by this method</para>
        /// This method also attempts to quit the Excel application object</remarks>
        public void Close()
        {
            mXlWorkBook.Close();
            mXlApp.Quit();
        }
        #endregion
    }
}
