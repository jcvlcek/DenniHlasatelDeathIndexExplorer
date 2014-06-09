using System;

namespace dbAccess
{
    /// <summary>
    /// Represents a Czech family (last, sur-) name in the SQL Server database
    /// </summary>
    public partial class Prijmeni : IJmeno
    {
        #region IJmeno interface

        /// <summary>
        /// Native form of the last / family / sur- name (with diacriticals), as stored in the SQL Server database
        /// </summary>
        /// <remarks>The native form may be in code page, but will be made into UTF-8 ultimately</remarks>
        public String Native { get { return this.Windows; } }

        #endregion
    }
}
