using System;
using System.Collections.Generic;
using System.Linq;

namespace dbAccess
{
    /// <summary>
    /// Represents an individual death notice record in the Denni Hlasatel death index database.
    /// </summary>
    public partial class DHDeathIndex : IDeathRecord
    {
        #region Constructors

        /// <summary>
        /// Copies pertinent fields from a generic death record to a Denni Hlasatel death index record
        /// </summary>
        /// <param name="drSource">the (generic) death record to copy</param>
        /// <param name="drTarg">the Denni Hlasatel death index record <paramref name="drSource"/> is to be copied into</param>
        /// <remarks>This method copies only the <see cref="FilingDate"/>, <see cref="FirstName"/>, <see cref="MiddleName"/> and <see cref="LastName"/> properties.
        /// These are all that are supported by records in the Denni Hlasatel death index database.</remarks>
        private static void Initialize(IDeathRecord drSource, DHDeathIndex drTarg)
        {
            // The commented-out properties simply don't exist in the Denni Hlasatel index
            //drTarg.AgeInYears = drSource.AgeInYears;
            //drTarg.CertificateNumber = drSource.CertificateNumber;
            //drTarg.City = drSource.City;
            //drTarg.County = drSource.County;
            //drTarg.DeathDate = drSource.DeathDate;
            drTarg.FilingDate = drSource.FilingDate;
            drTarg.FirstName = drSource.FirstName;
            //drTarg.Gender = drSource.Gender;
            drTarg.LastName = drSource.LastName;
            drTarg.MiddleName = drSource.MiddleName;
            //drTarg.PageNumber = drSource.PageNumber;
            //drTarg.Race = drSource.Race;
            //drTarg.Volume = drSource.Volume;
        }

        /// <summary>
        /// Create a new Denni Hlasatel death index record by copying an existing, generic death record
        /// </summary>
        /// <param name="drSource">the existing death record to copy</param>
        /// <returns>A Denni Hlasatel death index record containing the supported vital statistics from <paramref name="drSource"/></returns>
        public static DHDeathIndex Create(IDeathRecord drSource)
        {
            var drNew = new DHDeathIndex();
            Initialize(drSource, drNew);
            return drNew;
        }

        #endregion

        #region Public properties

        public short AgeInYears { get {return -1; } set { } }
        public int CertificateNumber { get { return serial; } set { } }
        public string City { get { return string.Empty; } set { } }
        public string County { get { return string.Empty; } set { } }
        public DateTime DeathDate { get { return DateTime.MinValue; } set { } }
        public DateTime FilingDate { get { return ReportDate; } set { } }
        public string FirstName { get { return GivenName; } set { } }
        public string LastName { get { return Surname; } set { } }
        public string MiddleName { get { return string.Empty; } set { } }
        public CustomTypes.Gender Gender { get { return CustomTypes.Gender.Unknown; } set { } }
        public CustomTypes.Race Race { get { return CustomTypes.Race.Unknown; } set { } }
        public short PageNumber { get { return -1; } set { } }
        public string Volume { get { return string.Empty; } set { } }

        #endregion

        #region Public methods

        /// <summary>
        /// Tests a death record for a match to a specified name / filing date pattern.
        /// </summary>
        /// <param name="drQuery">the death record to test for a possible match</param>
        /// <param name="drMatch">a death record containing the fields to be matched (null or empty fields are interpreted as "matches any")</param>
        /// <returns><code>true</code> if <paramref name="drQuery"/> matches the pattern <paramref name="drMatch"/>, otherwise <code>false</code></returns>
        /// <remarks>This method tests only the <see cref="FilingDate"/>, <see cref="FirstName"/> and <see cref="LastName"/> properties for equality.
        /// Name properties that are empty or <code>null</code> in <paramref name="drMatch"/> are treated as "matches any" wildcards.
        /// A reporting date of <see cref="DateTime.MinValue"/> is also treated as a "matches any" wildcard.</remarks>
        public static bool DefaultFilter( IDeathRecord drQuery, IDeathRecord drMatch )
        {
            bool bMatchSurname = true, bMatchGivenName = true, bMatchDate = true;

            if (!string.IsNullOrEmpty(drMatch.LastName))
                bMatchSurname = drQuery.LastName == drMatch.LastName;
            if (!string.IsNullOrEmpty(drMatch.FirstName))
                bMatchGivenName = drQuery.FirstName == drMatch.FirstName;
            if (drMatch.FilingDate > DateTime.MinValue)
                bMatchDate = drQuery.FilingDate == drMatch.FilingDate;

            return bMatchDate && bMatchGivenName && bMatchSurname;
        }

        /// <summary>
        /// Constructs a list of all Denni Hlasatel death index records,
        /// in an enumerable list, that match a specified pattern
        /// </summary>
        /// <param name="lIndices">the database table to search</param>
        /// <param name="drMatch">a death record containing properties to be matches</param>
        /// <param name="filter">the filter function that tests each record in the table against the matching record <paramref name="drMatch"/></param>
        /// <returns>a list of matching death records</returns>
        public static List<IDeathRecord> Matches( IEnumerable<DHDeathIndex> lIndices, IDeathRecord drMatch, Func<IDeathRecord, IDeathRecord, bool> filter)
        {
            return (from c in lIndices
                where filter(c, drMatch)
                select (c as IDeathRecord)).ToList();
        }

        /// <summary>
        /// Constructs a list of all Denni Hlasatel death index records
        /// in a queryable table, that match a specified pattern
        /// </summary>
        /// <param name="lIndices">the table whose records are to be searched</param>
        /// <param name="drMatch">a death record containing the <see cref="FirstName"/> and <see cref="LastName"/> to be matched</param>
        /// <param name="dtStart">the earliest date of any matching death record</param>
        /// <param name="dtEnd">the latest date of any matching death record</param>
        /// <returns>a list of matching death records</returns>
        public static List<IDeathRecord> Matches( IQueryable<DHDeathIndex> lIndices, IDeathRecord drMatch, DateTime dtStart, DateTime dtEnd )
        {
            return (from c in lIndices
                where (string.IsNullOrEmpty(drMatch.FirstName) || (c.GivenName.Contains(drMatch.FirstName))) &&
                      (string.IsNullOrEmpty(drMatch.LastName) || (c.Surname.Contains(drMatch.LastName))) &&
                      ((c.ReportDate >= dtStart) && (c.ReportDate <= dtEnd))
                select (c as IDeathRecord)).ToList();
        }

        #endregion
    }
}
