using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;

namespace Genealogy
{
    public class ExcelFile
    {
        #region Private members
        private Excel.Application mXlApp = null;
        private Excel.Workbook mXlWorkBook = null;
        private Excel.Worksheet mXlDefaultWorkSheet = null;
        #endregion

        #region Constructors
        public ExcelFile(string sPath)
        {
            if (mXlApp == null)
                mXlApp = new Excel.Application();
            mXlWorkBook = mXlApp.Workbooks.Open(sPath,
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

        public ExcelFile()
        {
            if (mXlApp == null)
                mXlApp = new Excel.Application();
            mXlWorkBook = mXlApp.Workbooks.Add();
            mXlDefaultWorkSheet = mXlWorkBook.Worksheets[1];
        }
        #endregion

        #region Public methods
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

        public void SetValueAt(Point where, string sValue)
        {
            mXlDefaultWorkSheet.Cells[where.Y, where.X].Value2 = sValue;
        }

        public void SaveAs(string sPath)
        {
            mXlWorkBook.SaveAs(sPath, Excel.XlFileFormat.xlExcel8);
        }

        public void Close()
        {
            mXlWorkBook.Close();
            mXlApp.Quit();
        }
        #endregion
    }
}
