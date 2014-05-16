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

        /// <summary>
        /// Construct a <see cref="WebChar"/> from its constituent parts
        /// </summary>
        /// <param name="cNative">the character in its "native" code page</param>
        /// <param name="sWeb">the character expressed as an HTML entity</param>
        /// <param name="cPlainText">the character expressed in 7-bit ASCII (stripped of any diacritical marks)</param>
        /// <param name="sUniCodePoint">the character expressed as a Unicode code point</param>
        /// <param name="sUTF8">the character expressed in URL- ("percent")-encoded UTF-8</param>
        private WebChar( Char cNative, string sWeb, char cPlainText, string sUniCodePoint, string sUTF8 )
        {
            mNative = cNative; mPlainText = cPlainText;
            mWeb = sWeb; mUniCodePoint = sUniCodePoint; mUTF8 = sUTF8;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The character in its "native" code page
        /// </summary>
        public Char Native { get { return mNative; } }

        /// <summary>
        /// The character expressed in 7-bit ASCII (stripped of any diacritical marks)
        /// </summary>
        public char PlainText { get { return mPlainText; } }

        /// <summary>
        /// The character expressed as an HTML entity
        /// </summary>
        public string Web { get { return mWeb; } }

        /// <summary>
        /// The character expressed in URL- ("percent")-encoded UTF-8
        /// </summary>
        public string UrlUTF8 { get { return "%" + mUTF8.Replace( ' ', '%' ); } }

        #endregion

        #region Public methods

        /// <summary>
        /// Constructs a <see cref="Cases"/> string pair of the upper and lower case forms of
        /// an HTML entity representing a commonly-used European character with diacritical marks
        /// </summary>
        /// <param name="sKey">the HTML entity to construct the upper/lower case forms</param>
        /// <returns>a <see cref="Cases"/> string pair of the upper and lower case forms of <paramref name="sKey"/></returns>
        /// <remarks><para>This method operates on HTML entities similar to the following examples:</para>
        /// <para>&amp;Iacute&#59; (whose lower case form is &amp;iacute&#59;)</para>
        /// <para>&amp;&#35;260&#59; (whose lower case form is &amp;&#35;261&#59;)</para></remarks>
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

        /// <summary>
        /// Find a character record matching a code page character
        /// </summary>
        /// <param name="cTarg">the code page character to match</param>
        /// <returns>a matching <see cref="WebChar"/>, if one can be found, otherwise <value>null</value></returns>
        public static WebChar Resolve(Char cTarg)
        {
            if (mCharList.Count < 1)
                PopulateList();

            if (mCharList.ContainsKey(cTarg))
                return mCharList[cTarg];
            else
                return null;
        }

        /// <summary>
        /// Convert characters in a <see cref="String"/>, as necessary, to URL ("percent") encoding,
        /// for inclusion within a Uniform Resource Locator
        /// </summary>
        /// <param name="sTarget">the <see cref="String"/> to convert</param>
        /// <returns>the URL-encoded <see cref="String"/></returns>
        /// <remarks>It might be possible to replace this method with a call to an existing .NET utility method</remarks>
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

        /// <summary>
        /// Populate the list of matching upper/lower case HTML entities for commonly-used European characters with diacritical marks
        /// </summary>
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

        /// <summary>
        /// <para>Read in the list of web characters from the <value>WebCharConversions.xml</value> file</para>
        /// This initializes the list of available character conversions
        /// </summary>
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
