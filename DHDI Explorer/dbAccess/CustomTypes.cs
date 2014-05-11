using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            FEMALE = 1,
            /// <summary>
            /// Specifically identified as male
            /// </summary>
            MALE = 2,
            /// <summary>
            /// Either unspecified as male or female,
            /// or the specification itself is uncertain
            /// </summary>
            UNKNOWN = 3
        };

        /// <summary>
        /// Race of an individual, as specified in an official record
        /// </summary>
        public enum Race
        {
            /// <summary>
            /// Specified in an official record as white (typically indicating Caucasion)
            /// </summary>
            WHITE = 1,
            /// <summary>
            /// Specified in an official records as African-American
            /// </summary>
            AFRICAN_AMERICAN = 2,
            /// <summary>
            /// No specification of race in the official record,
            /// or the specification itself is uncertain
            /// </summary>
            UNKNOWN = 3
        };

        /// <summary>
        /// Convert a gender specification in a database to an enumerated gender value
        /// </summary>
        /// <param name="db">The database to consult for a gender translation table</param>
        /// <param name="bGender">The gender specification to convert</param>
        /// <returns>An enumerated gender value</returns>
        /// <exception cref="ArgumentException">if <paramref name="bGender"/> does not represent a valid gender value in <paramref name="dB"/></exception>
        public static Gender GetGender(Linq2SqlDataContext db, byte bGender)
        {
            string sGender = new string(new char[] { (char)bGender });
            var query = from e in db.GenderMatches
                    where e.Mnemonic == sGender
                    select e;
            if (query.Count() > 0)
                return (Gender)query.First().GenderIndex;
            else
                throw new ArgumentException("Invalid gender specification \"" + sGender + "\"");
        }

        /// <summary>
        /// Convert a race specification in a database to an enumerated race value
        /// </summary>
        /// <param name="db">The database to consult for a race translation table</param>
        /// <param name="bRace">The race specification to convert</param>
        /// <returns>An enumerated race value</returns>
        /// <exception cref="ArgumentException">if <paramref name="bRace"/> does not represent a valid race value in <paramref name="dB"/></exception>
        public static Race GetRace(Linq2SqlDataContext db, byte bRace)
        {
            string sRace = new string(new char[] { (char)bRace });
            var query = from e in db.RaceMatches
                        where e.Mnemonic == sRace
                        select e;
            if (query.Count() > 0)
                return (Race)query.First().RaceIndex;
            else
                throw new ArgumentException("Invalid race specification \"" + sRace + "\"");
        }
    }
}
