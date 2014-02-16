using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    public sealed class IllinoisDeathIndex
    {
        #region Constructors

        static private void Initialize( IDeathRecord drSource, IDeathRecord drTarg )
        {
            drTarg.AgeInYears = drSource.AgeInYears;
            drTarg.CertificateNumber = drSource.CertificateNumber;
            drTarg.City = drSource.City;
            drTarg.County = drSource.County;
            drTarg.DeathDate = drSource.DeathDate;
            drTarg.FilingDate = drSource.FilingDate;
            drTarg.FirstName = drSource.FirstName;
            drTarg.Gender = drSource.Gender;
            drTarg.LastName = drSource.LastName;
            drTarg.MiddleName = drSource.MiddleName;
            drTarg.PageNumber = drSource.PageNumber;
            drTarg.Race = drSource.Race;
            drTarg.Volume = drSource.Volume;
        }

        public static IDeathRecord Create(IDeathRecord drSource)
        {
            IDeathRecord drNew;
            if (drSource.DeathDate.Year < 1916)
                drNew = new IllinoisDeathIndexPre1916();
            else
                drNew = new IllinoisDeathIndexPost1915();
            Initialize(drSource, drNew);
            return drNew;
        }

        public static IllinoisDeathIndexPre1916 CreatePre1916(IDeathRecord drSource)
        {
            if (drSource.DeathDate.Year > 1915)
                throw new ArgumentException("Death date \"" + drSource.DeathDate.ToShortDateString() + "\" is not pre-1916");

            IllinoisDeathIndexPre1916 drNew = new IllinoisDeathIndexPre1916();
            Initialize(drSource, drNew);
            return drNew;
        }

        public static IllinoisDeathIndexPost1915 CreatePost1915(IDeathRecord drSource)
        {
            if (drSource.DeathDate.Year < 1916)
                throw new ArgumentException("Death date \"" + drSource.DeathDate.ToShortDateString() + "\" is not pre-1916");

            IllinoisDeathIndexPost1915 drNew = new IllinoisDeathIndexPost1915();
            Initialize(drSource, drNew);
            return drNew;
        }

        #endregion

        #region Public methods

        public static List<IDeathRecord> Matches(Linq2SqlDataContext dB, IDeathRecord drMatch, Func<IDeathRecord, IDeathRecord, bool> filter)
        {
            List<IDeathRecord> lMatches = new List<IDeathRecord>();

            if (drMatch.DeathDate.Year < 1916)
            {
                var query = from c in dB.IllinoisDeathIndexPre1916s
                            where filter( c, drMatch )
                            select c;

                foreach (IDeathRecord indxNext in query)
                    lMatches.Add(indxNext);
            }
            else
            {
                var query = from c in dB.IllinoisDeathIndexPost1915s
                            where filter( c, drMatch )
                            select c;

                foreach (IDeathRecord indxNext in query)
                    lMatches.Add(indxNext);
            }

            return lMatches;
        }

        #endregion
    }
}
