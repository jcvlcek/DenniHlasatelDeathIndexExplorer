using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    public interface IDeathRecord
    {
        short AgeInYears { get; set; }
        int CertificateNumber { get; set; }
        string City { get; set; }
        string County { get; set; }
        DateTime DeathDate { get; set; }
        DateTime FilingDate { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string MiddleName { get; set; }
        CustomTypes.Gender Gender { get; set; }
        CustomTypes.Race Race { get; set; }
        short PageNumber { get; set; }
        string Volume { get; set; }
    }

    public sealed class Filters
    {
        public static bool MatchNameAndDate(IDeathRecord dr1, IDeathRecord dr2)
        {
            return (dr1.FirstName == dr2.FirstName) &&
                (dr1.LastName == dr2.LastName) &&
                (dr1.DeathDate.Date == dr2.DeathDate.Date);
        }
    }
}
