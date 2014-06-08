using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Windows.Forms;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// Abstract base class foundation for genealogical queries performed via Web (HTML) methods
    /// </summary>
    public abstract class WebQuery : IWebQuery
    {
        #region Private members
        /// <summary>
        /// The Uniform Resource Locater of the web query
        /// </summary>
        protected string mUrl;
        /// <summary>
        /// Data to be supplied to a POST request
        /// </summary>
        protected string mPostData;
        /// <summary>
        /// A Web Browser to process the query
        /// </summary>
        protected WebBrowser mWebBrowser;
        /// <summary>
        /// GET or POST
        /// </summary>
        protected QueryMethod mQueryMethod;
        /// <summary>
        /// The results of the query
        /// </summary>
        protected List<IDeathRecord> mRecords = new List<IDeathRecord>();
        #endregion

        #region Public members

        /// <summary>
        /// The HTML method to be executed by the query
        /// </summary>
        public enum QueryMethod
        {
            Get,
            Post
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a generic web query for genealogical records
        /// </summary>
        /// <param name="browser"><see cref="WebBrowser"/> control to display results in</param>
        /// <param name="queryMethod">HTML method (e.g. GET, POST) to invoke</param>
        protected WebQuery(WebBrowser browser, QueryMethod queryMethod)
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
            if (mQueryMethod == QueryMethod.Get)
                // mUrl += "?" + mPostData;
                mUrl += mPostData;

            var reqNew = (HttpWebRequest)WebRequest.CreateDefault(new Uri(Url));

            switch (mQueryMethod)
            {
                case QueryMethod.Get:
                default:
                    reqNew.Method = "GET"; 
                    break;
                case QueryMethod.Post:
                    reqNew.Method = "POST"; 
                    reqNew.ContentType = "application/x-www-form-urlencoded";
                    break;
            }

            if (mQueryMethod == QueryMethod.Post)
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
                var response = (HttpWebResponse)reqNew.GetResponse();

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
                    sMessage += Environment.NewLine + ex.InnerException;
                MessageBox.Show(sMessage, "Web Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resStream = ex.Response.GetResponseStream();
            }

            System.IO.TextReader sReader = new System.IO.StreamReader(resStream);

            string tempString; var sResponseText = string.Empty;
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
            mWebBrowser.DocumentCompleted += mWebBrowser_DocumentCompleted;
            mWebBrowser.DocumentText = sResponseText;
        }

        /// <summary>
        /// <para>Change to lower case, as necessary, all but the first character in a string.</para>
        /// HTML entities are (supposed to be) properly converted by this method.
        /// </summary>
        /// <param name="sOriginal">the string to be converted to "proper case"</param>
        /// <returns>the original string, with all but the first character in lower case</returns>
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
                        string sReturn = string.Empty;
                        int i = 0;

                        while (i < iLen)
                        {
                            var cNext = sOriginal[i];
                            string sPartial;
                            int iIndex = 0;
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
            mWebBrowser.DocumentCompleted -= mWebBrowser_DocumentCompleted;
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
