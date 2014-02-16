using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    public partial class IllinoisDeathIndexPost1915 : IDeathRecord
    {
        public string Volume
        {
            get { return string.Empty; }
            set { }
        }

        public short PageNumber
        {
            get { return -1; }
            set { }
        }

        public CustomTypes.Race Race
        {
            get { return (CustomTypes.Race)RaceIndex; }
            set { RaceIndex = (short)value; }
        }

        public CustomTypes.Gender Gender
        {
            get { return (CustomTypes.Gender)GenderIndex; }
            set { GenderIndex = (short)value; }
        }

        public string LastName
        {
            get { return Surname; }
            set { Surname = value; }
        }

        public string FirstName
        {
            get { return GivenName; }
            set { GivenName = value; }
        }

        public DateTime FilingDate
        {
            get { return ReportDate; }
            set { ReportDate = value; }
        }
    }
}
