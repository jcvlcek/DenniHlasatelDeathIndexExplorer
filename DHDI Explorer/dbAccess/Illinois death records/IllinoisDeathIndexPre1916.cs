using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dbAccess
{
    /// <summary>
    /// Represents a database record of a pre-1916 death occurring in the state of Illinois
    /// </summary>
    public partial class IllinoisDeathIndexPre1916 : IDeathRecord
    {
        #region Public properties

        public string Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public short PageNumber
        {
            get { return page; }
            set { page = value; }
        }

        public CustomTypes.Race Race
        {
            get { return CustomTypes.Race.Unknown; }
            set { }
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
            // TODO: What do we return for FilingDate if none is given in the record?
            get { return DateTime.MinValue; }
            set { }
        }

        public DateTime DeathDate
        {
            get { return deathDate; }
            set { deathDate = value; }
        }

        public string County
        {
            get { return county; }
            set { county = value; }
        }

        public string City
        {
            get { return city; }
            set { city = value; }
        }

        public int CertificateNumber
        {
            get { return certificateNumber; }
            set { certificateNumber = value; }
        }

        public short AgeInYears
        {
            get { return ageInYears; }
            set { ageInYears = value; }
        }

        #endregion
    }
}
