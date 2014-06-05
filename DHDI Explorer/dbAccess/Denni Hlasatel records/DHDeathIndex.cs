using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    public partial class DHDeathIndex : IDeathRecord
    {
        #region Constructors

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

        public static DHDeathIndex Create(IDeathRecord drSource)
        {
            DHDeathIndex drNew = new DHDeathIndex();
            Initialize(drSource, drNew);
            return drNew;
        }

        #endregion

        #region Public properties

        public short AgeInYears { get {return -1; } set { } }
        public int CertificateNumber { get { return this.serial; } set { } }
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

        public static List<IDeathRecord> Matches( IEnumerable<DHDeathIndex> lIndices, IDeathRecord drMatch, Func<IDeathRecord, IDeathRecord, bool> filter)
        {
            List<IDeathRecord> lMatches = new List<IDeathRecord>();

            var query = from c in lIndices
                        where filter( c, drMatch )
                        select c;

            foreach (DHDeathIndex indxNext in query)
                lMatches.Add(indxNext);

            return lMatches;
        }

        public static List<IDeathRecord> Matches( IQueryable<DHDeathIndex> lIndices, IDeathRecord drMatch, DateTime dtStart, DateTime dtEnd )
        {
            List<IDeathRecord> lMatches = new List<IDeathRecord>();

            var query = from c in lIndices 
                        where ( string.IsNullOrEmpty(drMatch.FirstName ) || ( c.GivenName.Contains( drMatch.FirstName ) ) ) &&
                        ( string.IsNullOrEmpty(drMatch.LastName) || ( c.Surname.Contains( drMatch.LastName ) ) ) &&
                        ( ( c.ReportDate >= dtStart ) && ( c.ReportDate <= dtEnd ) )
                        select c;

            foreach (DHDeathIndex indxNext in query)
                lMatches.Add(indxNext);

            return lMatches;
        }

        #endregion
    }
}
