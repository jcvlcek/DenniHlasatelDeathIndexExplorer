using System;
using System.Globalization;
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
        public const string DeathRecordClass = "DeathRecord";

        /// <summary>
        /// Names of properties used in this object class
        /// </summary>
        public sealed class PropertyTags
        {
            public const string DeathDate = "DeathDate";
            public const string City = "City";
            public const string AgeInYears = "Age";
            public const string Gender = "Sex";
            public const string Race = "Race";
            public const string Volume = "Volume";
            public const string Page = "Page";
            public const string CertificateNumber = "CertificateNumber";
            public const string County = "County";
            public const string LastName = "LastName";
            public const string FirstName = "FirstName";
            public const string MiddleName = "MiddleName";
            public const string FilingDate = "FilingDate";
        };
        #endregion

        #region Private members
        #endregion

        #region Constructors
        protected DeathRecord(IObject target)
            : base(target)
        {
            if (!PropertyExists(PropertyTags.City))
                AddProperty(PropertyTags.City, "Unspecified");
            if (!PropertyExists(PropertyTags.AgeInYears))
                AddProperty(PropertyTags.AgeInYears, "-1");
            if (!PropertyExists(PropertyTags.Gender))
                AddProperty(PropertyTags.Gender, "U");
            if (!PropertyExists(PropertyTags.Race))
                AddProperty(PropertyTags.Race, "U");
            if (!PropertyExists(PropertyTags.Volume))
                AddProperty(PropertyTags.Volume, "-1");
            if (!PropertyExists(PropertyTags.Page))
                AddProperty(PropertyTags.Page, "-1");
            if (!PropertyExists(PropertyTags.CertificateNumber))
                AddProperty(PropertyTags.CertificateNumber, "-1");
            if (!PropertyExists(PropertyTags.County))
                AddProperty(PropertyTags.County, "Unspecified");
            if (!PropertyExists(PropertyTags.LastName))
                AddProperty(PropertyTags.LastName, "Unknown");
            if (!PropertyExists(PropertyTags.FirstName))
                AddProperty(PropertyTags.FirstName, "Unknown");
            if (!PropertyExists(PropertyTags.MiddleName))
                AddProperty(PropertyTags.MiddleName, string.Empty);
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
            return new DeathRecord(new SimpleObject(sName, DeathRecordClass));
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
            var oNew = new SimpleObject(sName, oSource);
            return new DeathRecord(oNew);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Last (family) name of the decedent
        /// </summary>
        public string LastName
        {
            get { return GetPropertyValue(PropertyTags.LastName); }
            set { ForcePropertyValue(PropertyTags.LastName, value); }
        }

        /// <summary>
        /// First (given) name of the decedent
        /// </summary>
        public string FirstName
        {
            get { return GetPropertyValue(PropertyTags.FirstName); }
            set { ForcePropertyValue(PropertyTags.FirstName, value); }
        }

        /// <summary>
        /// Middle name (or middle initial) of the decedent
        /// </summary>
        public string MiddleName
        {
            get { return GetPropertyValue(PropertyTags.MiddleName); }
            set { ForcePropertyValue(PropertyTags.MiddleName, value); }
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
                    if (PropertyExists(PropertyTags.DeathDate))
                        return DateTime.Parse(GetPropertyValue(PropertyTags.DeathDate));
                    else if (PropertyExists(PropertyTags.FilingDate))
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
                ForcePropertyValue(PropertyTags.DeathDate, value.ToShortDateString());
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
                if ( PropertyExists( PropertyTags.FilingDate ) )
                    DateTime.TryParse(GetPropertyValue(PropertyTags.FilingDate), out dtFilingDate);
                return dtFilingDate;
            }
            set
            {
                ForcePropertyValue(PropertyTags.FilingDate, value.ToShortDateString());
            }
        }

        /// <summary>
        /// Age at death of the decedent if known)
        /// </summary>
        public short AgeInYears
        {
            get
            {
                short iAge;
                if ( short.TryParse(GetPropertyValue(PropertyTags.AgeInYears), out iAge) )
                    return iAge;
                return -1;
            }
            set
            {
                ForcePropertyValue(PropertyTags.AgeInYears, value.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Death certificate number
        /// </summary>
        public int CertificateNumber
        {
            get 
            {
                int iCertificate;
                if ( int.TryParse(GetPropertyValue(PropertyTags.CertificateNumber), out iCertificate) )
                    return iCertificate;
                return -1;
            }
            set { ForcePropertyValue(PropertyTags.CertificateNumber, value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// Gender (M=male, F=female or U=unknown) of the decedent
        /// </summary>
        public CustomTypes.Gender Gender
        {
            get
            {
                if (PropertyExists(PropertyTags.Gender))
                {
                    CustomTypes.Gender eGender;
                    if (Enum.TryParse<CustomTypes.Gender>(GetPropertyValue(PropertyTags.Gender), out eGender))
                        return eGender;
                }

                return CustomTypes.Gender.Unknown;
            }
            set { ForcePropertyValue(PropertyTags.Gender, value.ToString()); }
        }

        /// <summary>
        /// Race (W=White, ..., U=unknown) of the decedent
        /// </summary>
        public CustomTypes.Race Race
        {
            get
            {
                if (PropertyExists(PropertyTags.Race))
                {
                    CustomTypes.Race eRace;
                    if ( Enum.TryParse<CustomTypes.Race>( GetPropertyValue( PropertyTags.Race ), out eRace ) )
                        return eRace;
                }

                return CustomTypes.Race.Unknown;
            }
            set { ForcePropertyValue(PropertyTags.Race, value.ToString()); }
        }

        /// <summary>
        /// City where death occurred
        /// </summary>
        public string City
        {
            get { return GetPropertyValue(PropertyTags.City); }
            set { ForcePropertyValue(PropertyTags.City, value); }
        }

        /// <summary>
        /// County where death occurred
        /// </summary>
        public string County
        {
            get { return GetPropertyValue(PropertyTags.County); }
            set { ForcePropertyValue(PropertyTags.County, value); }
        }

        /// <summary>
        /// Unique identifier of the published volume containing this death record
        /// </summary>
        public string Volume
        {
            get { return GetPropertyValue(PropertyTags.Volume); }
            set { ForcePropertyValue(PropertyTags.Volume, value); }
        }

        /// <summary>
        /// Page number within <see cref="Volume"/> of this death record
        /// </summary>
        public short PageNumber
        {
            get 
            {
                short iPage = -1;
                short.TryParse(GetPropertyValue(PropertyTags.Page), out iPage);
                return iPage;
            }
            set { ForcePropertyValue(PropertyTags.Page, value.ToString()); }
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
                    Gender = CustomTypes.Gender.Male; break;
                case "F":
                case "FEMALE":
                case "W":
                case "WOMAN":
                    Gender = CustomTypes.Gender.Female; break;
                default:
                    Gender = CustomTypes.Gender.Unknown; break;
            }
        }

        protected void SetRace(string sValue)
        {
            switch (sValue.ToUpper())
            {
                case "W":
                    Race = CustomTypes.Race.White; break;
                case "N":
                    Race = CustomTypes.Race.AfricanAmerican; break;
                default:
                    Race = CustomTypes.Race.Unknown; break;
            }
        }
        #endregion
    }
}
