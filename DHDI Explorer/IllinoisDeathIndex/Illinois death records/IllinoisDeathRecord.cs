using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbAccess;

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
            public const string FULL_NAME = "Name of Deceased"; // ("LastName, FirstName")
            public const string PRE1916_DEATH_DATE = "Date"; // (YYYY-MM-DD)
            public const string CITY = "City";
            public const string AGE_IN_YEARS = "Age"; // ("99 YR", "UNK")
            public const string GENDER = "Sex"; // (Typically "U")
            public const string VOLUME = "Volume";
            public const string PAGE = "Page";
            public const string PRE1916_CERTIFICATE_NUMBER = "Cert No";
            public const string COUNTY = "County";
            public const string LAST_NAME = "Last Name";
            public const string FIRST_NAME = "First Name";
            public const string MIDDLE_NAME = "Middle Name";
            public const string GENDER_AND_RACE = "Sex/Race"; // (Format "M/W")
            public const string POST1915_CERTIFICATE_NUMBER = "Cert No.";
            public const string POST1915_DEATH_DATE = "Death Date"; // (YYYY-MM-DD)
            public const string DATE_FILED = "Date Filed"; // (YY-MM-DD)
        };

        #endregion

        #region Constructors
        public IllinoisDeathRecord(IObject oTarg)
            : base(oTarg)
        {
        }

        public static new IllinoisDeathRecord Create(string sName)
        {
            return new IllinoisDeathRecord(new SimpleObject(sName, DEATH_RECORD_CLASS));
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
                case DeathIndexSearchTags.AGE_IN_YEARS:
                    {
                        // Age can be of the form
                        // Y-99
                        // 99 YR
                        // 99 MO
                        // 99 DA
                        // UNK
                        double dAge = -1.0;
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
                        ForcePropertyValue(PropertyTags.AGE_IN_YEARS, iAge.ToString());
                    }
                    break;
                case DeathIndexSearchTags.CITY:
                    ForcePropertyValue(PropertyTags.CITY, sValue);
                    break;
                case DeathIndexSearchTags.COUNTY:
                    ForcePropertyValue(PropertyTags.COUNTY, sValue);
                    break;
                case DeathIndexSearchTags.DATE_FILED:
                    // NOTE: Format of this date is typically YY-MM-DD, not YYYY-MM-DD,
                    // thus we prepend "19" if the year field is two characters in length
                    // (This property is unique to the post-1916 database, thus we always prepend "19")
                    {
                        string[] sElements = sValue.Split('-');
                        if (sElements[0].Length == 2)
                            sValue = "19" + sValue;
                    }
                    ForcePropertyValue(PropertyTags.FILING_DATE, sValue);
                    break;
                case DeathIndexSearchTags.FIRST_NAME:
                    // TODO: What to do about capitalization?
                    ForcePropertyValue(PropertyTags.FIRST_NAME, sValue);
                    break;
                case DeathIndexSearchTags.FULL_NAME:
                    // TODO: What to do about capitalization?
                    {
                        string[] sElements = sValue.Split(',');
                        ForcePropertyValue(PropertyTags.LAST_NAME, sElements[0].Trim());
                        if (sElements.Length > 1)
                        {
                            string[] sGivenNames = sElements[1].Trim().Split(' ');
                            ForcePropertyValue(PropertyTags.FIRST_NAME, sGivenNames[0].Trim());
                            if (sGivenNames.Length > 1)
                                ForcePropertyValue(PropertyTags.MIDDLE_NAME, sGivenNames[1].Trim());
                        }
                    }
                    break;
                case DeathIndexSearchTags.GENDER:
                    // TODO: Figure out a way to use CustomTypes.GetGender here instead
                    SetGender(sValue);
                    break;
                case DeathIndexSearchTags.GENDER_AND_RACE:
                    {
                        string[] sElements = sValue.Split('/');
                        SetGender(sElements[0]);
                        if (sElements.Length > 1)
                            SetRace(sElements[1]);
                    }
                    break;
                case DeathIndexSearchTags.LAST_NAME:
                    // TODO: What to do about capitalization?
                    ForcePropertyValue(PropertyTags.LAST_NAME, sValue);
                    break;
                case DeathIndexSearchTags.MIDDLE_NAME:
                    // TODO: What to do about capitalization?
                    ForcePropertyValue(PropertyTags.MIDDLE_NAME, sValue);
                    break;
                case DeathIndexSearchTags.PAGE:
                    ForcePropertyValue(PropertyTags.PAGE, sValue);
                    break;
                case DeathIndexSearchTags.POST1915_CERTIFICATE_NUMBER:
                case DeathIndexSearchTags.PRE1916_CERTIFICATE_NUMBER:
                    ForcePropertyValue(PropertyTags.CERTIFICATE_NUMBER, sValue);
                    break;
                case DeathIndexSearchTags.POST1915_DEATH_DATE:
                case DeathIndexSearchTags.PRE1916_DEATH_DATE:
                    ForcePropertyValue(PropertyTags.DEATH_DATE, sValue);
                    break;
                case DeathIndexSearchTags.VOLUME:
                    ForcePropertyValue(PropertyTags.VOLUME, sValue);
                    break;
            }
        }

        #endregion
    }
}
