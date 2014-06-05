using System;
using System.Linq;

namespace dbAccess
{
    /// <summary>
    /// Custom enumeration types related to genealogy research in general,
    /// and specific to Illinois vital records specifically
    /// </summary>
    public class CustomTypes
    {
        /// <summary>
        /// Gender specification (including possible uncertainty)
        /// </summary>
        public enum Gender
        {
            /// <summary>
            /// Specifically identified as female
            /// </summary>
            Female = 1,
            /// <summary>
            /// Specifically identified as male
            /// </summary>
            Male = 2,
            /// <summary>
            /// Either unspecified as male or female,
            /// or the specification itself is uncertain
            /// </summary>
            Unknown = 3
        };

        /// <summary>
        /// Race of an individual, as specified in an official record
        /// </summary>
        public enum Race
        {
            /// <summary>
            /// Specified in an official record as white (typically indicating Caucasion)
            /// </summary>
            White = 1,
            /// <summary>
            /// Specified in an official records as African-American
            /// </summary>
            AfricanAmerican = 2,
            /// <summary>
            /// No specification of race in the official record,
            /// or the specification itself is uncertain
            /// </summary>
            Unknown = 3
        };

        /// <summary>
        /// Convert a gender specification in a database to an enumerated gender value
        /// </summary>
        /// <param name="db">The database to consult for a gender translation table</param>
        /// <param name="bGender">The gender specification to convert</param>
        /// <returns>An enumerated gender value</returns>
        /// <exception cref="ArgumentException">if <paramref name="bGender"/> does not represent a valid gender value in <paramref name="db"/></exception>
        public static Gender GetGender(Linq2SqlDataContext db, byte bGender)
        {
            var sGender = new string(new[] { (char)bGender });
            var query = from e in db.GenderMatches
                    where e.Mnemonic == sGender
                    select e;
            if (query.Any())
                return (Gender)query.First().GenderIndex;
            throw new ArgumentException("Invalid gender specification \"" + sGender + "\"");
        }

        /// <summary>
        /// Convert a race specification in a database to an enumerated race value
        /// </summary>
        /// <param name="db">The database to consult for a race translation table</param>
        /// <param name="bRace">The race specification to convert</param>
        /// <returns>An enumerated race value</returns>
        /// <exception cref="ArgumentException">if <paramref name="bRace"/> does not represent a valid race value in <paramref name="db"/></exception>
        public static Race GetRace(Linq2SqlDataContext db, byte bRace)
        {
            var sRace = new string(new[] { (char)bRace });
            var query = from e in db.RaceMatches
                        where e.Mnemonic == sRace
                        select e;
            if (query.Any())
                return (Race)query.First().RaceIndex;
            throw new ArgumentException("Invalid race specification \"" + sRace + "\"");
        }
    }
}
