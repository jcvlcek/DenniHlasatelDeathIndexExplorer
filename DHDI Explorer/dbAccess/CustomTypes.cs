using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    public class CustomTypes
    {
        public enum Gender
        {
            FEMALE = 1,
            MALE = 2,
            UNKNOWN = 3
        };

        public enum Race
        {
            WHITE = 1,
            AFRICAN_AMERICAN = 2,
            UNKNOWN = 3
        };

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
