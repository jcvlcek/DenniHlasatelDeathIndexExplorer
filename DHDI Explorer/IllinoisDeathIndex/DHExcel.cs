using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    /// <summary>
    /// <para>Excel spreadsheet formatted for Denni Hlasatel Death Index records</para>
    /// <remarks>This class was primarily for output only, to transcribe DH records into a spreadsheet for processing in Excel</remarks>
    /// </summary>
    public class DHExcel : ExcelFile
    {
        #region Private members
        private int mCurrentRow = 1;
        #endregion

        /// <summary>
        /// Create an empty spreadsheet file, containing only the initialized column headers
        /// </summary>
        public DHExcel()
        {
            mCurrentRow = 1;
            SetValueAt(new System.Drawing.Point(1, mCurrentRow), "Surname");
            SetValueAt(new System.Drawing.Point(2, mCurrentRow), "GivenName");
            SetValueAt(new System.Drawing.Point(3, mCurrentRow), "ReportDate");
        }
        
        /// <summary>
        /// Write a Denni Hlasatel Death Index record at the current row,
        /// and increment the current row index
        /// </summary>
        /// <param name="sGivenName">the given (first) name of the decedent</param>
        /// <param name="sSurname">the surname (family name) of the decedent</param>
        /// <param name="sReportDate">the date the death notice appeared in the Denni Hlasatel</param>
        public void WriteRecord(string sGivenName, string sSurname, string sReportDate)
        {
            ++mCurrentRow;
            SetValueAt(new System.Drawing.Point(1, mCurrentRow), sSurname);
            SetValueAt(new System.Drawing.Point(2, mCurrentRow), sGivenName);
            SetValueAt(new System.Drawing.Point(3, mCurrentRow), "'" + sReportDate);
        }

        /// <summary>
        /// The number of death records currently in the spreadsheet
        /// </summary>
        public int Records
        {
            get { return mCurrentRow - 1; }
        }
    }
}
