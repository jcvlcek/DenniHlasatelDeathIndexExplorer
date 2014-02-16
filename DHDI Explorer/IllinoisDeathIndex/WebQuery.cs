using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using dbAccess;

namespace Genealogy
{
    public class WebQuery : IWebQuery
    {
        #region Private members
        protected string mUrl;
        protected string mPostData;
        protected WebBrowser mWebBrowser;
        protected eQueryMethod mQueryMethod;
        protected List<IDeathRecord> mRecords = new List<IDeathRecord>();
        #endregion

        #region Public members

        public enum eQueryMethod
        {
            GET,
            POST
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a generic web query for genealogical records
        /// </summary>
        /// <param name="browser"><see cref="WebBrowser"/> control to display results in</param>
        public WebQuery(WebBrowser browser, eQueryMethod queryMethod)
        {
            mWebBrowser = browser;
            mQueryMethod = queryMethod;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Submit the query to the appropriate web server
        /// </summary>
        public virtual void Submit()
        {
            if (mQueryMethod == eQueryMethod.GET)
                // mUrl += "?" + mPostData;
                mUrl += mPostData;

            Uri urlTarg = new Uri(mUrl);
            HttpWebRequest reqNew = (HttpWebRequest)WebRequest.CreateDefault(new Uri(Url));

            switch (mQueryMethod)
            {
                case eQueryMethod.GET:
                default:
                    reqNew.Method = "GET"; 
                    break;
                case eQueryMethod.POST:
                    reqNew.Method = "POST"; 
                    reqNew.ContentType = "application/x-www-form-urlencoded";
                    break;
            }

            if (mQueryMethod == eQueryMethod.POST)
            {
                byte[] bytedata = Encoding.UTF8.GetBytes(mPostData);
                reqNew.ContentLength = bytedata.Length;

                System.IO.Stream requestStream = reqNew.GetRequestStream();
                requestStream.Write(bytedata, 0, bytedata.Length);
                requestStream.Close();
            }

            // execute the request
            System.IO.Stream resStream;
            try
            {
                HttpWebResponse response = (HttpWebResponse)
                reqNew.GetResponse();

                // we will read data via the response stream
                resStream = response.GetResponseStream();
            }
            catch (WebException ex)
            {
                string sMessage;

                if (ex.Response != null)
                    sMessage = ex.Response.ToString();
                else
                    sMessage = ex.ToString();
                if (!string.IsNullOrEmpty(ex.Message))
                    sMessage += Environment.NewLine + ex.Message;
                if (ex.InnerException != null)
                    sMessage += Environment.NewLine + ex.InnerException.ToString();
                MessageBox.Show(sMessage, "Web Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resStream = ex.Response.GetResponseStream();
            }
            catch (Exception)
            {
                throw;
            }

            System.IO.TextReader sReader = new System.IO.StreamReader(resStream);

            string tempString = null; string sResponseText = string.Empty;
            while ((tempString = sReader.ReadLine()) != null)
            {
                sResponseText += tempString + Environment.NewLine;
            }
            resStream.Close();

            //mWebBrowser.Url = urlTarg;
            //string sDomain = urlTarg.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped);
            //HtmlDocument docNew = mWebBrowser.Document.OpenNew(true);
            //docNew.Domain = sDomain;
            //docNew.Write(sResponseText);
            // mWebBrowser.Document.Domain = sDomain;
            mWebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(mWebBrowser_DocumentCompleted);
            mWebBrowser.DocumentText = sResponseText;
        }

        public static string ConvertToProperCase(string sOriginal)
        {
            int iLen = sOriginal.Length;

            switch (iLen)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return sOriginal.ToUpper();
                default:
                    {
                        string sReturn = string.Empty, sPartial = string.Empty;
                        int i = 0, iIndex = 0;

                        while (i < iLen)
                        {
                            char cNext = sOriginal[i];
                            if ((cNext == '&') && ((iIndex = sOriginal.IndexOf(';', i )) > i ))
                            {
                                WebChar.Cases cWeb = WebChar.ConvertCase( sOriginal.Substring(i, iIndex + 1 - i) );
                                if (i == 0)
                                    sPartial = cWeb.Upper;
                                else
                                    sPartial = cWeb.Lower;
                                i = iIndex + 1;
                            }
                            else
                            {
                                if (i == 0)
                                    sPartial = new string(cNext, 1).ToUpper();
                                else
                                    sPartial = new string(cNext, 1).ToLower();
                                ++i;
                            }

                            sReturn += sPartial;
                        }

                        return sReturn;
                    }
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Universal Resource Locator (web page or link) for the web query
        /// </summary>
        virtual public string Url
        {
            get { return mUrl; }
            protected set { mUrl = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// The event raised when a web query has completed,
        /// and the response document fully parsed by the web browser.
        /// </summary>
        public event EventHandler<WebQueryEventArgs> QueryCompleted;

        #endregion

        #region Event handlers

        /// <summary>
        /// Event handler called when the web browser has completed parsing a response document
        /// </summary>
        /// <param name="sender">The object originating the event</param>
        /// <param name="e">The arguments passed by the web browser upon document completion</param>
        private void mWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Ensure that this object's event handler isn't called again
            mWebBrowser.DocumentCompleted -= this.mWebBrowser_DocumentCompleted;
            mRecords.Clear();
            OnDocumentCompleted();

            if ( QueryCompleted != null )
                QueryCompleted( this, new WebQueryEventArgs( mRecords ));
        }

        /// <summary>
        /// Method called when the web browser has completed the parsing of a response document
        /// </summary>
        /// <remarks>This method is to be overridden in a derived class.
        /// The derived class must inspect the parsed web document
        /// and construct a results list to return in the <see cref="QueryCompleted"/>
        /// event sent to any listeners.</remarks>
        protected virtual void OnDocumentCompleted()
        {
        }

        #endregion

    }
}
