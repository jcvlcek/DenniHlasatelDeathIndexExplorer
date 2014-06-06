using System;

namespace dbAccess
{
    /// <summary>
    /// Comprehensive interface for death records
    /// (implementations are not required to support all properties)
    /// </summary>
    public interface IDeathRecord
    {
        /// <summary>
        /// Recorded age at time of death
        /// </summary>
        short AgeInYears { get; set; }
        /// <summary>
        /// Unique identifying number of the death certificate
        /// </summary>
        int CertificateNumber { get; set; }
        /// <summary>
        /// City (or municipality) where the death was recorded
        /// </summary>
        string City { get; set; }
        /// <summary>
        /// Count where the death was recorded
        /// </summary>
        string County { get; set; }
        /// <summary>
        /// Recorded date and time of the moment of death
        /// </summary>
        DateTime DeathDate { get; set; }
        /// <summary>
        /// Recorded date and time of the filing of the death certificate
        /// </summary>
        DateTime FilingDate { get; set; }
        /// <summary>
        /// First (given) name of the decedent, as recorded
        /// </summary>
        string FirstName { get; set; }
        /// <summary>
        /// Last (family) name (surname) of the decedent, as recorded
        /// </summary>
        string LastName { get; set; }
        /// <summary>
        /// Middle name (if any) of the decedent, as recorded
        /// </summary>
        string MiddleName { get; set; }
        /// <summary>
        /// Gender (male, female, unknown/unclear) of the decedent, as recorded
        /// </summary>
        CustomTypes.Gender Gender { get; set; }
        /// <summary>
        /// Race of the decedent, as recorded
        /// </summary>
        CustomTypes.Race Race { get; set; }
        /// <summary>
        /// Page number, within a containing volume, of the death certificate
        /// </summary>
        short PageNumber { get; set; }
        /// <summary>
        /// Volume number of the certificate collection containing this individual death certificate
        /// </summary>
        string Volume { get; set; }
    }

    /// <summary>
    /// Utility class, providing filters for comparing death records or instances of other related classes
    /// </summary>
    public sealed class Filters
    {
        /// <summary>
        /// Match the full name (first and last) and death date of two death records
        /// </summary>
        /// <param name="dr1">the first of the two death records to compare</param>
        /// <param name="dr2">the second of the two death records to compare</param>
        /// <returns>true if the first name, last name and death date/time of the two records match exactly, otherwise false</returns>
        public static bool MatchNameAndDate(IDeathRecord dr1, IDeathRecord dr2)
        {
            return (dr1.FirstName == dr2.FirstName) &&
                (dr1.LastName == dr2.LastName) &&
                (dr1.DeathDate.Date == dr2.DeathDate.Date);
        }
    }
}
