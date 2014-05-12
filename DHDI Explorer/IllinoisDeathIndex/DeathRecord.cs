using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// Baseline implementation of <see cref="IDeathRecord"/> interface
    /// via a decorator (wrapper) pattern approach
    /// </summary>
    public class DeathRecord : AliasObject, IDeathRecord
    {
        #region Public members
        /// <summary>
        /// Class tag identifying a death record
        /// </summary>
        public const string DEATH_RECORD_CLASS = "DeathRecord";

        /// <summary>
        /// Names of properties used in this object class
        /// </summary>
        public sealed class PropertyTags
        {
            public const string DEATH_DATE = "DeathDate";
            public const string CITY = "City";
            public const string AGE_IN_YEARS = "Age";
            public const string GENDER = "Sex";
            public const string RACE = "Race";
            public const string VOLUME = "Volume";
            public const string PAGE = "Page";
            public const string CERTIFICATE_NUMBER = "CertificateNumber";
            public const string COUNTY = "County";
            public const string LAST_NAME = "LastName";
            public const string FIRST_NAME = "FirstName";
            public const string MIDDLE_NAME = "MiddleName";
            public const string FILING_DATE = "FilingDate";
        };
        #endregion

        #region Private members
        #endregion

        #region Constructors
        protected DeathRecord(IObject target)
            : base(target)
        {
            if (!PropertyExists(PropertyTags.CITY))
                AddProperty(PropertyTags.CITY, "Unspecified");
            if (!PropertyExists(PropertyTags.AGE_IN_YEARS))
                AddProperty(PropertyTags.AGE_IN_YEARS, "-1");
            if (!PropertyExists(PropertyTags.GENDER))
                AddProperty(PropertyTags.GENDER, "U");
            if (!PropertyExists(PropertyTags.RACE))
                AddProperty(PropertyTags.RACE, "U");
            if (!PropertyExists(PropertyTags.VOLUME))
                AddProperty(PropertyTags.VOLUME, "-1");
            if (!PropertyExists(PropertyTags.PAGE))
                AddProperty(PropertyTags.PAGE, "-1");
            if (!PropertyExists(PropertyTags.CERTIFICATE_NUMBER))
                AddProperty(PropertyTags.CERTIFICATE_NUMBER, "-1");
            if (!PropertyExists(PropertyTags.COUNTY))
                AddProperty(PropertyTags.COUNTY, "Unspecified");
            if (!PropertyExists(PropertyTags.LAST_NAME))
                AddProperty(PropertyTags.LAST_NAME, "Unknown");
            if (!PropertyExists(PropertyTags.FIRST_NAME))
                AddProperty(PropertyTags.FIRST_NAME, "Unknown");
            if (!PropertyExists(PropertyTags.MIDDLE_NAME))
                AddProperty(PropertyTags.MIDDLE_NAME, string.Empty);
        }

        /// <summary>
        /// Create a new (blank) death record with the specified name
        /// <paramref name="sName"/> need not be the name of the decedent;
        /// it is used instead as a unique identifier for the death record
        /// </summary>
        /// <param name="sName">The unique ID to be applied to this death record</param>
        /// <returns>A new (and blank) death record with the specified name</returns>
        public static DeathRecord Create(string sName)
        {
            return new DeathRecord(new SimpleObject(sName, DEATH_RECORD_CLASS));
        }

        /// <summary>
        /// Deep copy an existing death record, into a new death record
        /// with a specified name
        /// </summary>
        /// <param name="sName">The uniqe ID to be applied to the copy</param>
        /// <param name="oSource">the death record to copy</param>
        /// <returns></returns>
        public static DeathRecord Create( string sName, DeathRecord oSource )
        {
            SimpleObject oNew = new SimpleObject(sName, oSource);
            return new DeathRecord(oNew);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Last (family) name of the decedent
        /// </summary>
        public string LastName
        {
            get { return GetPropertyValue(PropertyTags.LAST_NAME); }
            set { ForcePropertyValue(PropertyTags.LAST_NAME, value); }
        }

        /// <summary>
        /// First (given) name of the decedent
        /// </summary>
        public string FirstName
        {
            get { return GetPropertyValue(PropertyTags.FIRST_NAME); }
            set { ForcePropertyValue(PropertyTags.FIRST_NAME, value); }
        }

        /// <summary>
        /// Middle name (or middle initial) of the decedent
        /// </summary>
        public string MiddleName
        {
            get { return GetPropertyValue(PropertyTags.MIDDLE_NAME); }
            set { ForcePropertyValue(PropertyTags.MIDDLE_NAME, value); }
        }

        /// <summary>
        /// Date of death
        /// </summary>
        /// <remarks>This date may be approximate.
        /// For example, if only a filing date is known,
        /// the death date is assumed to be the day
        /// immediately before the filing date.</remarks>
        public DateTime DeathDate
        {
            // TODO: Need to align these values explicity to US Central time zone
            get
            {
                try
                {
                    if (PropertyExists(PropertyTags.DEATH_DATE))
                        return DateTime.Parse(GetPropertyValue(PropertyTags.DEATH_DATE));
                    else if (PropertyExists(PropertyTags.FILING_DATE))
                        return FilingDate.AddDays(-1.0);
                    else
                        return DateTime.MinValue;
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                ForcePropertyValue(PropertyTags.DEATH_DATE, value.ToShortDateString());
            }
        }

        /// <summary>
        /// Date a death notice was filed or published
        /// </summary>
        /// <remarks>The FilingDate is the date a death certificate was filed,
        /// in the case of Illinois death index records, or
        /// the data a death record was published,
        /// in the case of Denni Hlasatel death records</remarks>
        public DateTime FilingDate
        {
            // TODO: Need to align these values explicity to US Central time zone
            get {
                DateTime dtFilingDate = DateTime.MinValue;
                if ( PropertyExists( PropertyTags.FILING_DATE ) )
                    DateTime.TryParse(GetPropertyValue(PropertyTags.FILING_DATE), out dtFilingDate);
                return dtFilingDate;
            }
            set
            {
                ForcePropertyValue(PropertyTags.FILING_DATE, value.ToShortDateString());
            }
        }

        /// <summary>
        /// Age at death of the decedent if known)
        /// </summary>
        public short AgeInYears
        {
            get
            {
                short iAge = -1;
                short.TryParse(GetPropertyValue(PropertyTags.AGE_IN_YEARS), out iAge);
                return iAge;
            }
            set
            {
                ForcePropertyValue(PropertyTags.AGE_IN_YEARS, value.ToString());
            }
        }

        /// <summary>
        /// Death certificate number
        /// </summary>
        public int CertificateNumber
        {
            get 
            {
                int iCertificate = -1;
                int.TryParse(GetPropertyValue(PropertyTags.CERTIFICATE_NUMBER), out iCertificate);
                return iCertificate;
            }
            set { ForcePropertyValue(PropertyTags.CERTIFICATE_NUMBER, value.ToString()); }
        }

        /// <summary>
        /// Gender (M=male, F=female or U=unknown) of the decedent
        /// </summary>
        public CustomTypes.Gender Gender
        {
            get
            {
                if (PropertyExists(PropertyTags.GENDER))
                {
                    CustomTypes.Gender eGender;
                    if (Enum.TryParse<CustomTypes.Gender>(GetPropertyValue(PropertyTags.GENDER), out eGender))
                        return eGender;
                }

                return CustomTypes.Gender.UNKNOWN;
            }
            set { ForcePropertyValue(PropertyTags.GENDER, value.ToString()); }
        }

        /// <summary>
        /// Race (W=White, ..., U=unknown) of the decedent
        /// </summary>
        public CustomTypes.Race Race
        {
            get
            {
                if (PropertyExists(PropertyTags.RACE))
                {
                    CustomTypes.Race eRace;
                    if ( Enum.TryParse<CustomTypes.Race>( GetPropertyValue( PropertyTags.RACE ), out eRace ) )
                        return eRace;
                }

                return CustomTypes.Race.UNKNOWN;
            }
            set { ForcePropertyValue(PropertyTags.RACE, value.ToString()); }
        }

        /// <summary>
        /// City where death occurred
        /// </summary>
        public string City
        {
            get { return GetPropertyValue(PropertyTags.CITY); }
            set { ForcePropertyValue(PropertyTags.CITY, value); }
        }

        /// <summary>
        /// County where death occurred
        /// </summary>
        public string County
        {
            get { return GetPropertyValue(PropertyTags.COUNTY); }
            set { ForcePropertyValue(PropertyTags.COUNTY, value); }
        }

        /// <summary>
        /// Unique identifier of the published volume containing this death record
        /// </summary>
        public string Volume
        {
            get { return GetPropertyValue(PropertyTags.VOLUME); }
            set { ForcePropertyValue(PropertyTags.VOLUME, value); }
        }

        /// <summary>
        /// Page number within <see cref="Volume"/> of this death record
        /// </summary>
        public short PageNumber
        {
            get 
            {
                short iPage = -1;
                short.TryParse(GetPropertyValue(PropertyTags.PAGE), out iPage);
                return iPage;
            }
            set { ForcePropertyValue(PropertyTags.PAGE, value.ToString()); }
        }

        #endregion

        #region Public methods

        #endregion

        #region Private methods

        protected void ForcePropertyValue(string sName, string sValue)
        {
            if (PropertyExists(sName))
                SetPropertyValue(sName, sValue);
            else
                AddProperty(sName, sValue);
        }

        protected void SetGender(string sValue)
        {
            switch (sValue.ToUpper())
            {
                case "M":
                case "MALE":
                case "MAN":
                    Gender = CustomTypes.Gender.MALE; break;
                case "F":
                case "FEMALE":
                case "W":
                case "WOMAN":
                    Gender = CustomTypes.Gender.FEMALE; break;
                default:
                    Gender = CustomTypes.Gender.UNKNOWN; break;
            }
        }

        protected void SetRace(string sValue)
        {
            switch (sValue.ToUpper())
            {
                case "W":
                    Race = CustomTypes.Race.WHITE; break;
                case "N":
                    Race = CustomTypes.Race.AFRICAN_AMERICAN; break;
                default:
                    Race = CustomTypes.Race.UNKNOWN; break;
            }
        }
        #endregion
    }
}
