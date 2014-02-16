using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    public class DHExcel : ExcelFile
    {
        #region Private members
        private int mCurrentRow = 1;
        #endregion

        public DHExcel()
        {
            mCurrentRow = 1;
            SetValueAt(new System.Drawing.Point(1, mCurrentRow), "Surname");
            SetValueAt(new System.Drawing.Point(2, mCurrentRow), "GivenName");
            SetValueAt(new System.Drawing.Point(3, mCurrentRow), "ReportDate");
        }

        public void WriteRecord(string sGivenName, string sSurname, string sReportDate)
        {
            ++mCurrentRow;
            SetValueAt(new System.Drawing.Point(1, mCurrentRow), sSurname);
            SetValueAt(new System.Drawing.Point(2, mCurrentRow), sGivenName);
            SetValueAt(new System.Drawing.Point(3, mCurrentRow), "'" + sReportDate);
        }

        public int Records
        {
            get { return mCurrentRow - 1; }
        }
    }
}
