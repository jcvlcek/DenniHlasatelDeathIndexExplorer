using System.Globalization;

namespace Genealogy
{
    public class IllinoisDeathRecord : DeathRecord
    {
        #region Public members

        /// <summary>
        /// Column headers from Illinois death index search results
        /// </summary>
        public sealed class DeathIndexSearchTags
        {
            public const string FullName = "Name of Deceased"; // ("LastName, FirstName")
            public const string Pre1916DeathDate = "Date"; // (YYYY-MM-DD)
            public const string City = "City";
            public const string AgeInYears = "Age"; // ("99 YR", "UNK")
            public const string Gender = "Sex"; // (Typically "U")
            public const string Volume = "Volume";
            public const string Page = "Page";
            public const string Pre1916CertificateNumber = "Cert No";
            public const string County = "County";
            public const string LastName = "Last Name";
            public const string FirstName = "First Name";
            public const string MiddleName = "Middle Name";
            public const string GenderAndRace = "Sex/Race"; // (Format "M/W")
            public const string Post1915CertificateNumber = "Cert No.";
            public const string Post1915DeathDate = "Death Date"; // (YYYY-MM-DD)
            public const string DateFiled = "Date Filed"; // (YY-MM-DD)
        };

        #endregion

        #region Constructors
        public IllinoisDeathRecord(IObject oTarg)
            : base(oTarg)
        {
        }

        public static new IllinoisDeathRecord Create(string sName)
        {
            return new IllinoisDeathRecord(new SimpleObject(sName, DeathRecordClass));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Interpret a property value read from one of the Illinois Statewide Death Index tables
        /// </summary>
        /// <param name="sName">The name of the property, as it exists in the Illinois Statewide Death Index</param>
        /// <param name="sValue">The value of the property, as it exists in the Illinois Statewide Death Index</param>
        public void SetDeathIndexProperty(string sName, string sValue)
        {
            switch (sName)
            {
                case DeathIndexSearchTags.AgeInYears:
                    {
                        // Age can be of the form
                        // Y-99
                        // 99 YR
                        // 99 MO
                        // 99 DA
                        // UNK
                        double dAge;
                        short iAge = -1;
                        if (sValue == "UNK")
                            iAge = -1;
                        else if (sValue.StartsWith("Y-"))
                            short.TryParse(sValue.Substring(2), out iAge);
                        else if (sValue.EndsWith(" YR"))
                            short.TryParse(sValue.Substring(0, sValue.Length - 3), out iAge);
                        else if (sValue.EndsWith(" MO"))
                        {
                            if (double.TryParse(sValue.Substring(0, sValue.Length - 3), out dAge))
                                iAge = (short)(dAge / 12.0 + 0.5);
                        }
                        else if (sValue.EndsWith(" DA"))
                        {
                            if (double.TryParse(sValue.Substring(0, sValue.Length - 3), out dAge))
                                iAge = (short)(dAge / 365.0 + 0.5);
                        }
                        else
                            iAge = -1;
                        ForcePropertyValue(PropertyTags.AgeInYears, iAge.ToString(CultureInfo.InvariantCulture));
                    }
                    break;
                case DeathIndexSearchTags.City:
                    ForcePropertyValue(PropertyTags.City, sValue);
                    break;
                case DeathIndexSearchTags.County:
                    ForcePropertyValue(PropertyTags.County, sValue);
                    break;
                case DeathIndexSearchTags.DateFiled:
                    // NOTE: Format of this date is typically YY-MM-DD, not YYYY-MM-DD,
                    // thus we prepend "19" if the year field is two characters in length
                    // (This property is unique to the post-1916 database, thus we always prepend "19")
                    {
                        string[] sElements = sValue.Split('-');
                        if (sElements[0].Length == 2)
                            sValue = "19" + sValue;
                    }
                    ForcePropertyValue(PropertyTags.FilingDate, sValue);
                    break;
                case DeathIndexSearchTags.FirstName:
                    // TODO: What to do about capitalization?
                    ForcePropertyValue(PropertyTags.FirstName, sValue);
                    break;
                case DeathIndexSearchTags.FullName:
                    // TODO: What to do about capitalization?
                    {
                        var sElements = sValue.Split(',');
                        ForcePropertyValue(PropertyTags.LastName, sElements[0].Trim());
                        if (sElements.Length > 1)
                        {
                            var sGivenNames = sElements[1].Trim().Split(' ');
                            ForcePropertyValue(PropertyTags.FirstName, sGivenNames[0].Trim());
                            if (sGivenNames.Length > 1)
                                ForcePropertyValue(PropertyTags.MiddleName, sGivenNames[1].Trim());
                        }
                    }
                    break;
                case DeathIndexSearchTags.Gender:
                    // TODO: Figure out a way to use CustomTypes.GetGender here instead
                    SetGender(sValue);
                    break;
                case DeathIndexSearchTags.GenderAndRace:
                    {
                        var sElements = sValue.Split('/');
                        SetGender(sElements[0]);
                        if (sElements.Length > 1)
                            SetRace(sElements[1]);
                    }
                    break;
                case DeathIndexSearchTags.LastName:
                    // TODO: What to do about capitalization?
                    ForcePropertyValue(PropertyTags.LastName, sValue);
                    break;
                case DeathIndexSearchTags.MiddleName:
                    // TODO: What to do about capitalization?
                    ForcePropertyValue(PropertyTags.MiddleName, sValue);
                    break;
                case DeathIndexSearchTags.Page:
                    ForcePropertyValue(PropertyTags.Page, sValue);
                    break;
                case DeathIndexSearchTags.Post1915CertificateNumber:
                case DeathIndexSearchTags.Pre1916CertificateNumber:
                    ForcePropertyValue(PropertyTags.CertificateNumber, sValue);
                    break;
                case DeathIndexSearchTags.Post1915DeathDate:
                case DeathIndexSearchTags.Pre1916DeathDate:
                    ForcePropertyValue(PropertyTags.DeathDate, sValue);
                    break;
                case DeathIndexSearchTags.Volume:
                    ForcePropertyValue(PropertyTags.Volume, sValue);
                    break;
            }
        }

        #endregion
    }
}
