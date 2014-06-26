namespace Genealogy
{
    /// <summary>
    /// <para>Excel spreadsheet formatted for Denni Hlasatel Death Index records</para>
    /// <remarks>This class was primarily for output only, to transcribe DH records into a spreadsheet for processing in Excel</remarks>
    /// </summary>
    public class DenniHlasatelExcelFile : ExcelFile
    {
        #region Private members
        private int _currentRow = 1;
        #endregion

        /// <summary>
        /// Create an empty spreadsheet file, containing only the initialized column headers
        /// </summary>
        public DenniHlasatelExcelFile()
        {
            _currentRow = 1;
            SetValueAt(new System.Drawing.Point(1, _currentRow), "Surname");
            SetValueAt(new System.Drawing.Point(2, _currentRow), "GivenName");
            SetValueAt(new System.Drawing.Point(3, _currentRow), "ReportDate");
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
            ++_currentRow;
            SetValueAt(new System.Drawing.Point(1, _currentRow), sSurname);
            SetValueAt(new System.Drawing.Point(2, _currentRow), sGivenName);
            SetValueAt(new System.Drawing.Point(3, _currentRow), "'" + sReportDate);
        }

        /// <summary>
        /// The number of death records currently in the spreadsheet
        /// </summary>
        public int Records
        {
            get { return _currentRow - 1; }
        }
    }
}
