using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// Represents the last (family) name of a person
    /// </summary>
    public class Surname : PersonName<Surname>
    {
        #region Private members
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <see cref="Surame"/> object corresponding to the argument string,
        /// and binds that object into the surname cache
        /// </summary>
        /// <param name="sName">the native (code page) form of the name</param>
        private Surname(string sName) : base( sName )
        {
            var queryNative = (from cNative in Utilities.dB.Prijmenis
                               where cNative.Windows == sName
                               select cNative).FirstOrDefault();
            if (queryNative != null)
            {
                TotalCount = queryNative.Count;
            }
            else
            {
                var queryPlainText = from cPlainText in Utilities.dB.Prijmenis
                                     where cPlainText.PlainText == sName
                                     select cPlainText;
                foreach (var gn in queryPlainText)
                {
                    TotalCount += gn.Count;
                }
            }
            mCache.Add(sName, this);
        }

        /// <summary>
        /// Gets a <see cref="Surname"/> corresponding to argument string
        /// </summary>
        /// <param name="sName">The name to search for</param>
        /// <returns>A cached <see cref="Surname"/>, if one has already been created for <see cref="sName"/>,
        /// otherwise a new <see cref="Surname"/> created for the argument string</returns>
        public static new Surname Get( string sName )
        {
           PersonName<Surname> gnTarg;

            if (!mCache.TryGetValue(sName, out gnTarg))
                gnTarg = new Surname(sName);

            return (Surname)gnTarg;
        }

        #endregion

        #region Private properties

        /// <summary>
        /// A comprehensive list of all available alternate forms of this name.
        /// This includes forms that match either a plain text or a "native" spelling of the name.
        /// The returned list will always have at least one entry, and the first entry in the list
        /// will always have a <see cref="NativeForm.PlainText"/> property equal to <see cref="Value"/>
        /// </summary>
        public override List<NativeForm> AlternateForms
        {
            get
            {
                if (mAlternateForms == null)
                {
                    mAlternateForms = new List<NativeForm>();

                    AddAlternateForms(Value);
                    // If no direct matches found, add a record for the original value
                    // with the native form set to an empty string to flag 
                    // that there is no direct equivalent
                    if ( true ) // mAlternateForms.Count < 1)
                    {
                        int iAdditions;

                        if (Value.EndsWith("ova"))
                        {
                            // Try the simplest case first: Feminine name
                            // is simply the masculine form with "ova" suffix appended
                            string sBase = Value.Substring(0, Value.Length - 3);
                            if ((iAdditions = AddAlternateForms(sBase)) > 0)
                            {
                                for (int iCount = mAlternateForms.Count, i = iCount - iAdditions; i < iCount; ++i)
                                {
                                    NativeForm nfNext = mAlternateForms[i];
                                    if (nfNext.Value != nfNext.PlainText)
                                        mAlternateForms.Add(new NativeForm(nfNext.Value + "ová", nfNext.Web + "ov&aacute;", nfNext.PlainText + "ova"));
                                }
                            }

                            // Next simplest case: "ova" suffix replaces
                            // the "a" (no diacritical) suffix of the masculine form
                            if ((iAdditions = AddAlternateForms(sBase + "a")) > 0)
                            {
                                for (int iCount = mAlternateForms.Count, i = iCount - iAdditions; i < iCount; ++i)
                                {
                                    NativeForm nfNext = mAlternateForms[i];
                                    if (nfNext.Value != nfNext.PlainText)
                                        mAlternateForms.Add(new NativeForm(
                                            nfNext.Value.Substring( 0, nfNext.Value.Length - 1 ) + "ová", 
                                            nfNext.Web.Substring(0, nfNext.Web.Length - 1) + "ov&aacute;", 
                                            nfNext.PlainText.Substring(0, nfNext.PlainText.Length - 1) + "ova"));
                                }
                            }

                            // Most complicated case: "ova" suffix added to
                            // a masculine form ending with consonant + "e" + consonant
                            // (Commonly "cek", "rek" or "sek")
                            if (sBase.Length >= 2)
                            {
                                if ((iAdditions = AddAlternateForms(sBase.Substring(0, sBase.Length - 1)
                                    + "e" + sBase.Substring(sBase.Length - 1, 1))) > 0)
                                {
                                    for (int iCount = mAlternateForms.Count, i = iCount - iAdditions; i < iCount; ++i)
                                    {
                                        NativeForm nfNext = mAlternateForms[i];
                                        if (nfNext.Value != nfNext.PlainText)
                                            mAlternateForms.Add(new NativeForm(
                                                nfNext.Value.Substring(0, nfNext.Value.Length - 2) + nfNext.Value.Substring( nfNext.Value.Length - 1, 1 ) + "ová",
                                                nfNext.Web.Substring(0, nfNext.Web.Length - 2) + nfNext.Web.Substring( nfNext.Web.Length - 1, 1 ) + "ov&aacute;",
                                                nfNext.PlainText.Substring(0, nfNext.PlainText.Length - 2) + nfNext.PlainText.Substring( nfNext.PlainText.Length - 1, 1 ) + "ova"));
                                    }
                                }
                            }
                        }
                        else if (Value.EndsWith("a"))
                        {
                            if ((iAdditions = AddAlternateForms(Value.Substring(0, Value.Length - 1) + "y")) > 0)
                            {
                                for (int iCount = mAlternateForms.Count, i = iCount - iAdditions; i < iCount; ++i)
                                {
                                    NativeForm nfNext = mAlternateForms[i];
                                    if ((nfNext.Value != nfNext.PlainText) && nfNext.Web.EndsWith( "&yacute;" ) )
                                        mAlternateForms.Add(new NativeForm(
                                            nfNext.Value.Substring(0, nfNext.Value.Length - 1) + "á",
                                            nfNext.Web.Substring(0, nfNext.Web.Length - 1) + "&aacute;",
                                            nfNext.PlainText.Substring(0, nfNext.PlainText.Length - 1) + "a"));
                                }
                            }
                        }
                    }

                    // If _still_ no matches found, simply add the original value
                    if (mAlternateForms.Count < 1)
                        mAlternateForms.Add(new NativeForm(Value, Value, Value));
                }

                return mAlternateForms;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Constructs a list of last names (surnames) matching a plain text (7-bit ASCII) representation
        /// </summary>
        /// <param name="sPlainTextName">the plain text representation of the surname(s)</param>
        /// <returns>a list of surnames matching <paramref name="sPlainTextName"/>.  This list may be empty, if no surnames match</returns>
        /// <remarks>Multiple surnames may match the same plain-text representation, as a result of differing applications of diacriticals</remarks>
        public static new List<Surname> MatchToPlainTextName(string sPlainTextName)
        {
            List<Surname> lMatches = new List<Surname>();

            var qSurnames = from sn in Utilities.dB.Prijmenis
                             where sn.PlainText == sPlainTextName
                             select sn;
            foreach (var snNext in qSurnames)
            {
                lMatches.Add(Surname.Get(snNext.Windows));
            }

            return lMatches;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Search the database and add any names matching the plain text search string
        /// </summary>
        /// <param name="sPlainText">The (plain text) name to search for</param>
        /// <returns>The number of names added to the alternate name forms list</returns>
        private int AddAlternateForms(string sPlainText)
        {
            // First, assemble a query to get all direct matches
            var query1 = from c in Utilities.dB.Prijmenis
                         where (c.PlainText == sPlainText) || (c.Windows == sPlainText)
                         select c;

            // Add all of the "hits" on the query
            int iHits = 0;
            foreach (Prijmeni qNext in query1)
            {
                NativeForm nfNew = new NativeForm(qNext.Windows, qNext.Web, qNext.PlainText);
                if (!mAlternateForms.Contains(nfNew))
                {
                    mAlternateForms.Add(nfNew);
                    ++iHits;
                }
            }

            return iHits;
        }

        #endregion

    }
}
