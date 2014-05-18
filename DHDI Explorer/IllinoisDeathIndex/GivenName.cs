using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// Represents the first (given) name of a person
    /// </summary>
    public class GivenName : PersonName<GivenName>
    {
        #region Public members
        #endregion

        #region Private members

        /// <summary>
        /// Count of males in the reference census with this given name
        /// </summary>
        private int mMaleCount = 0;

        /// <summary>
        /// Count of females in the reference census with this given name
        /// </summary>
        private int mFemaleCount = 0;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <see cref="GivenName"/> object corresponding to the argument string,
        /// and binds that object into the given name cache
        /// </summary>
        /// <param name="sName">the native (code page) form of the name</param>
        private GivenName(string sName) : base( sName )
        {
            var queryNative = (from cNative in Utilities.dB.KrestniJmenas
                               where cNative.CodePage == sName
                               select cNative).FirstOrDefault();
            if (queryNative != null)
            {
                MaleCount = queryNative.MaleCount;
                FemaleCount = queryNative.FemaleCount;
            }
            else
            {
                var queryPlainText = from cPlainText in Utilities.dB.KrestniJmenas
                                     where cPlainText.PlainText == sName
                                     select cPlainText;
                foreach (var gn in queryPlainText)
                {
                    MaleCount += gn.MaleCount;
                    FemaleCount += gn.FemaleCount;
                }
            }
            mCache.Add(sName, this);
        }

        /// <summary>
        /// Gets a <see cref="GivenName"/> corresponding to argument string
        /// </summary>
        /// <param name="sName">The name to search for</param>
        /// <returns>A cached <see cref="GivenName"/>, if one has already been created for <see cref="sName"/>,
        /// otherwise a new <see cref="GivenName"/> created from the argument string</returns>
        public new static GivenName Get( string sName )
        {
            PersonName<GivenName> gnTarg;

            if (!mCache.TryGetValue(sName, out gnTarg))
                gnTarg = new GivenName(sName);

            return (GivenName)gnTarg;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Total number of males in the census with this given name
        /// </summary>
        public int MaleCount
        {
            get { return mMaleCount; }
            private set { mMaleCount = value; }
        }

        /// <summary>
        /// Total number of females in the census with this given name
        /// </summary>
        public int FemaleCount
        {
            get { return mFemaleCount; }
            private set { mFemaleCount = value; }
        }

        /// <summary>
        /// Total number of individuals in the census with this given name
        /// </summary>
        public int TotalCount
        {
            get { return MaleCount + FemaleCount; }
        }

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

                    // First, assemble a query to get all direct matches
                    var query1 = from c in Utilities.dB.KrestniJmenas
                                 where (c.PlainText == Value) || (c.CodePage == Value)
                                 select c;

                    // Add all of the "hits" on the direct match query
                    foreach (KrestniJmena qNext in query1)
                    {
                        mAlternateForms.Add(new NativeForm(qNext.CodePage, qNext.Web, qNext.PlainText));
                    }

                    // If no direct matches found, add a record for the original value
                    // with the native form set to an empty string to flag 
                    // that there is no direct equivalent
                    if (mAlternateForms.Count < 1)
                        mAlternateForms.Add(new NativeForm(string.Empty, Value, Value));

                    List<String> lNatives = new List<String>();
                    List<string> lPlainTexts = new List<string>();

                    // Organize lists of matches for native and plain text forms of the name
                    // based on the original query
                    foreach (NativeForm nfNext in mAlternateForms)
                    {
                        if (!lNatives.Contains(nfNext.Value))
                            lNatives.Add(nfNext.Value);
                        if (!lPlainTexts.Contains(nfNext.PlainText))
                            lPlainTexts.Add(nfNext.PlainText);
                    }

                    // Construct the query to get the "first level" matches
                    var query2 = from c in Utilities.dB.GivenNameEquivalents
                                 where lNatives.Contains(c.Native) || lPlainTexts.Contains(c.English)
                                 select c;

                    // Add all the "hits" on the indirect query
                    foreach (GivenNameEquivalent geNext in query2)
                    {
                        var query3 = from c in Utilities.dB.KrestniJmenas
                                     where c.CodePage == geNext.Native
                                     select c;

                        string sWeb = query3.Count() > 0 ? query3.First().Web : geNext.English;
                        NativeForm nfNew = new NativeForm(geNext.Native, geNext.English, sWeb);
                        if (!mAlternateForms.Contains(nfNew))
                        {
                            mAlternateForms.Add(nfNew);

                            foreach (KrestniJmena kjNext in query3)
                            {
                                NativeForm nfRecursive = new NativeForm(kjNext.CodePage, kjNext.PlainText, kjNext.Web);
                                if (!mAlternateForms.Contains(nfRecursive))
                                    mAlternateForms.Add(nfRecursive);
                            }
                        }
                    }
                }

                return mAlternateForms;
            }
        }

        #endregion

        #region Private properties

        #endregion

        #region Public methods

        /// <summary>
        /// Constructs a list of given names matching a plain text (7-bit ASCII) representation
        /// </summary>
        /// <param name="sPlainTextName">the plain text representation of the given name(s)</param>
        /// <param name="mdB">the database connection to query for the matching given names</param>
        /// <returns>a list of given names matching <paramref name="sPlainTextName"/>.  This list may be empty, if no given names match</returns>
        /// <remarks>Multiple given names may match the same plain-text representation, as a result of differing applications of diacriticals</remarks>
        public static List<GivenName> MatchToPlainTextName(string sPlainTextName, Linq2SqlDataContext mdB)
        {
            List<GivenName> lMatches = new List<GivenName>();

            var qGivenName = from gn in mdB.KrestniJmenas
                             where gn.PlainText == sPlainTextName
                             select gn;
            foreach (var gnNext in qGivenName)
            {
                lMatches.Add(GivenName.Get(gnNext.CodePage));
            }

            return lMatches;
        }

        #endregion

        #region Private methods
        #endregion
    }
}
