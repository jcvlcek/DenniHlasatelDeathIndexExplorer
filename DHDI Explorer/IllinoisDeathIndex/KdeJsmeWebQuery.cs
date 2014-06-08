using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// Web query for searching the Czech online names database http://www.kdejsme.cz/
    /// </summary>
    public class KdeJsmeWebQuery : WebQuery
    {
        #region Private members

        /// <summary>
        /// Count of individuals bearing a specified name
        /// </summary>
        int mCount = 0;

        #endregion

        #region Public members
        /// <summary>
        /// Identifies a specific component of a person's full name
        /// </summary>
        public enum eWhichName {
            /// <summary>
            /// First (or "given") name
            /// </summary>
            GIVEN_NAME,

            /// <summary>
            /// Last (or family) name
            /// </summary>
            SURNAME,

            /// <summary>
            /// Unspecified whether first or last name
            /// </summary>
            UNKNOWN
        };
        #endregion

        #region Constructors

        /// <summary>
        /// Search the Kde Jsme database for a surname or given name
        /// </summary>
        /// <param name="sName">Name to search for (in UTF8)</param>
        /// <param name="eGivenOrSurname">Whether <see cref="sName"/> is a given name or a surname</param>
        /// <param name="browser"><see cref="WebBrowser"/> control to display results in</param>
        public KdeJsmeWebQuery(String sName, eWhichName eGivenOrSurname, WebBrowser browser) : base( browser, QueryMethod.Get )
        {
            mUrl = GetUrl(sName, eGivenOrSurname);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get the Uniform Resource Locator (URL) for querying the KdeJsme web site
        /// in regards to a specified name
        /// </summary>
        /// <param name="sName">the name to query</param>
        /// <param name="eGivenOrSurname">which component of a full name <paramref name="sName"/> corresponds to</param>
        /// <returns></returns>
        public static string GetUrl(String sName, eWhichName eGivenOrSurname)
        {
            string sDatabase;

            switch ( eGivenOrSurname )
            {
                case eWhichName.GIVEN_NAME:
                    sDatabase = "jmeno"; break ;
                case eWhichName.SURNAME:
                    sDatabase = "prijmeni"; break;
                default:
                    throw new ArgumentException( "Cannot construct a Kde Jsme web query for a name unless we know if it's a given name or a surname" );
            }

            return "http://www.kdejsme.cz/" + sDatabase + "/" + WebChar.ToUrlEncoding(sName) + "/hustota/";
        }

        #endregion

        #region Public properties

        #endregion

        #region Event handlers

        /// <summary>
        /// Event handler called when the web browser has completed parsing a response document
        /// </summary>
        override protected void OnDocumentCompleted()
        {
            HtmlDocument docResponse = mWebBrowser.Document;

            HtmlElementCollection lTitles = docResponse.GetElementsByTagName("title");
            if (lTitles.Count < 1)
                return;
            HtmlElement eTitle = lTitles[0];
            String sValue = eTitle.InnerText;
            String[] sItems = sValue.Split(new char[] { '|' });
            if (sItems.Count() < 2)
                return;
            String sDatum = sItems[1].Trim();
            String[] sNameValuePair = sDatum.Split(new char[] { ':' });
            if (sNameValuePair.Count() < 2)
                return;
            String sName = sNameValuePair[0];
            String sCount = sNameValuePair[1];
            int iCount = 0;
            if (int.TryParse(sCount, out iCount))
                mCount = iCount;
        }

        #endregion
    }
}
