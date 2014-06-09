using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// A web query, aimed at finding records matching a death record from another source, submitted to the Family Search Web site
    /// </summary>
    public class FamilySearchWebQuery : WebQuery
    {
        #region Constructors

        /// <summary>
        /// Construct a Family Search Web query against a specified death record
        /// </summary>
        /// <param name="drTarg">the death record to query for matching records on the Family Search Web site</param>
        /// <param name="browser">the web browser to use in processing the query</param>
        public FamilySearchWebQuery(IDeathRecord drTarg, WebBrowser browser)
            : base(browser, QueryMethod.Get)
        {
            string sYear = drTarg.DeathDate.Year.ToString();

            // https://familysearch.org/search/records/index#count=20&query=%2Bgivenname%3AFrank~%20%2Bsurname%3AUlcek~%20%2Bdeath_place%3AIllinois~%20%2Bdeath_year%3A1918-1918
            // _Url = "https://www.familysearch.org/search/records/index#count=20&query=%2B";
            // _PostData = "givenname%3A" + drTarg.FirstName + "~%20%2Bsurname%3A" + drTarg.LastName + "~%20%2Bdeath_place%3AIllinois~%20%2Bdeath_year%3A" + sYear + "-" + sYear;
            _Url = "https://www.familysearch.org/search/records/index#count=20&query=+";
            _PostData = "givenname:" + drTarg.FirstName + "~ +surname:" + drTarg.LastName + "~ +death_place:Illinois~ +death_year:" + sYear + "-" + sYear;
            // _Url = "https://www.familysearch.org/search/index/record-search";
            // _PostData = "searchType=records&filtered=false&fed=true&collectionId=&collectionName=&givenname=" + drTarg.FirstName + "&surname=" + drTarg.LastName;
            System.Net.Security.RemoteCertificateValidationCallback ServerCertificateValidationCallback = delegate { return true; };
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Submit the query to the Family Search web site
        /// </summary>
        public override void Submit()
        {
            if (_QueryMethod == QueryMethod.Get)
                // _Url += "?" + _PostData;
                _Url += _PostData;

            Uri urlTarg = new Uri(_Url);
            _WebBrowser.Url = urlTarg;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Write to a text file diagnostic information on the contents of a browser window
        /// </summary>
        /// <param name="browser">the web browser to dump diagnostic information from</param>
        public static void WriteOutFrame(WebBrowser browser)
        {
            System.IO.TextWriter fOut = new System.IO.StreamWriter(@"C:\temp\FamilySearch.txt");
            fOut.Write(browser.DocumentText);
            fOut.WriteLine();
            fOut.WriteLine();
            for (int i = 0; i < browser.Document.Window.Frames.Count; i++)
            {
                fOut.WriteLine("Frame[ " + i.ToString() + " ]:");
                HtmlWindow frame = browser.Document.Window.Frames[0];
                string str = frame.Document.Body.OuterHtml;
                fOut.WriteLine(str);
                
            } fOut.Close();
        }

        /// <summary>
        /// Dump diagnostic information to a temporary file upon document completion
        /// </summary>
        protected override void OnDocumentCompleted()
        {
            WriteOutFrame( _WebBrowser );
        }

        #endregion
    }
}
