using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using dbAccess;

namespace Genealogy
{
    public class FamilySearchWebQuery : WebQuery
    {
        #region Constructors

        public FamilySearchWebQuery(IDeathRecord drTarg, WebBrowser browser)
            : base(browser, eQueryMethod.GET)
        {
            string sYear = drTarg.DeathDate.Year.ToString();

            // https://familysearch.org/search/records/index#count=20&query=%2Bgivenname%3AFrank~%20%2Bsurname%3AUlcek~%20%2Bdeath_place%3AIllinois~%20%2Bdeath_year%3A1918-1918
            // mUrl = "https://www.familysearch.org/search/records/index#count=20&query=%2B";
            // mPostData = "givenname%3A" + drTarg.FirstName + "~%20%2Bsurname%3A" + drTarg.LastName + "~%20%2Bdeath_place%3AIllinois~%20%2Bdeath_year%3A" + sYear + "-" + sYear;
            mUrl = "https://www.familysearch.org/search/records/index#count=20&query=+";
            mPostData = "givenname:" + drTarg.FirstName + "~ +surname:" + drTarg.LastName + "~ +death_place:Illinois~ +death_year:" + sYear + "-" + sYear;
            // mUrl = "https://www.familysearch.org/search/index/record-search";
            // mPostData = "searchType=records&filtered=false&fed=true&collectionId=&collectionName=&givenname=" + drTarg.FirstName + "&surname=" + drTarg.LastName;
            System.Net.Security.RemoteCertificateValidationCallback ServerCertificateValidationCallback = delegate { return true; };
        }

        #endregion

        #region Public methods

        public override void Submit()
        {
            if (mQueryMethod == eQueryMethod.GET)
                // mUrl += "?" + mPostData;
                mUrl += mPostData;

            Uri urlTarg = new Uri(mUrl);
            mWebBrowser.Url = urlTarg;
        }

        #endregion

        #region Private methods

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

        protected override void OnDocumentCompleted()
        {
            WriteOutFrame( mWebBrowser );
        }

        #endregion
    }
}
