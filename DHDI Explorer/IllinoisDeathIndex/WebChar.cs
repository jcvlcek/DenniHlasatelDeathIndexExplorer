using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace Genealogy
{
    /// <summary>
    /// Utility class for representing international characters
    /// in differing character sets (CP125x, ISO-8859-*, Unicode, UTF-8, etc.)
    /// </summary>
    public sealed class WebChar
    {
        #region Private members
        /// <summary>
        /// Dictionary of Web characters, indexed by characters in a native code page
        /// </summary>
        private static Dictionary<Char, WebChar> mCharList = new Dictionary<Char, WebChar>();

        /// <summary>
        /// Associates HTML entities corresponding to upper and lower case forms of the same character
        /// </summary>
        public sealed class Cases : Tuple<String, String>
        {
            public Cases(String Upper, String Lower) : base(Upper, Lower) { }

            public string Upper { get { return Item1; } }

            public string Lower { get { return Item2; } }
        }

        private static Dictionary<String, Cases> mCaseList = new Dictionary<string, Cases>();
        private static XDocument mDoc = null;

        /// <summary>
        /// <para>String representations of a single character as an:</para>
        /// <para>HTML entry (mWeb)</para>
        /// <para>Unicode code point (mUniCodePoint)</para>
        /// <para>UTF-8 character (mUTF8)</para>
        /// </summary>
        private string mWeb, mUniCodePoint, mUTF8;
        /// <summary>
        /// The character in its original representation (likely a code page e.g. CP125x)
        /// </summary>
        Char mNative;
        /// <summary>
        /// The character in its closest 7-bit ASCII representation
        /// </summary>
        char mPlainText;
        #endregion

        #region Constructors

        private WebChar( Char cNative, string sWeb, char cPlainText, string sUniCodePoint, string sUTF8 )
        {
            mNative = cNative; mPlainText = cPlainText;
            mWeb = sWeb; mUniCodePoint = sUniCodePoint; mUTF8 = sUTF8;
        }

        #endregion

        #region Public properties

        public Char Native { get { return mNative; } }

        public char PlainText { get { return mPlainText; } }

        public string Web { get { return mWeb; } }

        public string UrlUTF8 { get { return "%" + mUTF8.Replace( ' ', '%' ); } }

        #endregion

        #region Public methods

        public static Cases ConvertCase(String sKey)
        {
            if (sKey[1] == '#')
            {
                if (mCaseList.Count < 1)
                    PopulateCaseList();

                return mCaseList[sKey];
            }
            else
            {
                string sFirstChar = sKey.Substring(0, 1);
                string sSecondChar = sKey.Substring(1, 1);
                string sRemainder = sKey.Substring(2);
                string sUpper = sFirstChar + sSecondChar.ToUpper() + sRemainder;
                string sLower = sFirstChar + sSecondChar.ToLower() + sRemainder;
                return new Cases(sUpper, sLower);
            }
        }

        public static WebChar Resolve(Char cTarg)
        {
            if (mCharList.Count < 1)
                PopulateList();

            if (mCharList.ContainsKey(cTarg))
                return mCharList[cTarg];
            else
                return null;
        }

        public static string ToUrlEncoding(String sTarget)
        {
            String sUrlEncoded = String.Empty;

            foreach (Char cNext in sTarget.ToCharArray())
            {
                WebChar wChar = Resolve(cNext);
                if (wChar != null)
                    sUrlEncoded += wChar.UrlUTF8;
                else
                    sUrlEncoded += cNext;
            }

            return sUrlEncoded;
        }

        #endregion

        #region Private members

        private static void PopulateCaseList()
        {
            foreach (Cases cNext in new Cases[] {
                new Cases( "&#260;", "&#261;" ),
                new Cases( "&#262;", "&#263;" ),
                new Cases( "&#268;", "&#269;" ),
                new Cases( "&#270;", "&#271;" ),
                new Cases( "&#282;", "&#283;" ),
                new Cases( "&#280;", "&#281;" ),
                new Cases( "&#313;", "&#314;" ),
                new Cases( "&#317;", "&#318;" ),
                new Cases( "&#321;", "&#322;" ),
                new Cases( "&#323;", "&#324;" ),
                new Cases( "&#327;", "&#328;" ),
                new Cases( "&#344;", "&#345;" ),
                new Cases( "&#346;", "&#347;" ),
                new Cases( "&#352;", "&#353;" ),
                new Cases( "&#356;", "&#357;" ),
                new Cases( "&#366;", "&#367;" ),
                new Cases( "&#360;", "&#361;" ),
                new Cases( "&#377;", "&#378;" ),
                new Cases( "&#381;", "&#382;" )
            })
            {
                mCaseList.Add(cNext.Upper, cNext);
                mCaseList.Add(cNext.Lower, cNext);
            }
        }

        private static void PopulateList()
        {
            mDoc = System.Xml.Linq.XDocument.Load(System.IO.Path.Combine(Utilities.DataFilesFolder, "WebCharConversions.xml"));

            var query = from c in mDoc.Descendants("WebChar")
                        select c;

            foreach (var cNext in query)
            {
                String sNative = cNext.Attribute( "Char" ).Value;
                string sWeb = cNext.Attribute( "Web" ).Value;
                string sPlainText = cNext.Attribute( "PlainText" ).Value;
                string sUniCodePoint = cNext.Attribute( "UniCodePoint" ).Value;
                string sUTF8 = cNext.Attribute( "UTF8" ).Value;
                mCharList.Add(sNative[0], new WebChar(sNative[0], sWeb, sPlainText[0], sUniCodePoint, sUTF8));
            }
        }

        #endregion
    }
}
