using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dbAccess;

namespace Genealogy
{
    public class KdeJsmeWebQuery : WebQuery
    {
        #region Private members
        int mCount = 0;
        #endregion

        #region Public members
        public enum eWhichName {
            GIVEN_NAME,
            SURNAME,
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
        public KdeJsmeWebQuery(String sName, eWhichName eGivenOrSurname, WebBrowser browser) : base( browser, eQueryMethod.GET )
        {
            mUrl = GetUrl(sName, eGivenOrSurname);
        }

        #endregion

        #region Public methods

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
        /// <param name="sender">The object originating the event</param>
        /// <param name="e">The arguments passed by the web browser upon document completion</param>
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
