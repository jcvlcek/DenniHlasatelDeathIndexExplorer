﻿using System.Collections.Generic;
using System.Windows.Forms;
using dbAccess;

namespace Genealogy
{
    class IllinoisDeathIndexWebQuery : WebQuery
    {
        #region Private members
        #endregion

        #region Constructors

        /// <summary>
        /// Search the Illinois death indices for a specified individual
        /// </summary>
        /// <param name="drTarg">Death record of the desired individual</param>
        /// <param name="browser"><see cref="WebBrowser"/> control to display results in</param>
        public IllinoisDeathIndexWebQuery(IDeathRecord drTarg, WebBrowser browser) : base( browser, eQueryMethod.POST )
        {
            bool bBefore1916 = drTarg.FilingDate.Year < 1916;

            mUrl = bBefore1916 ? "http://www.ilsos.gov/isavital/deathSearch.do" : "http://www.ilsos.gov/isavital/idphDeathSearch.do";
            // mPostData = bBefore1916 ? "name=" + drTarg.LastName + ", " + drTarg.FirstName + "&county=COOK" : "firstName=" + drTarg.FirstName + "&lastName=" + drTarg.LastName + "&middleName=&county=COOK";
            mPostData = bBefore1916 ? "name=" + drTarg.LastName + ", " + drTarg.FirstName + "&county=STATEWIDE" : "firstName=" + drTarg.FirstName + "&lastName=" + drTarg.LastName + "&middleName=&county=STATEWIDE";
        }

        #endregion

        #region Public methods

        public static string GetUrl(IDeathRecord drTarg)
        {
            bool bBefore1916 = drTarg.FilingDate.Year < 1916;

            string sUrl = bBefore1916 ? "http://www.ilsos.gov/isavital/deathSearch.do" : "http://www.ilsos.gov/isavital/idphDeathSearch.do";
            sUrl += "?";
            sUrl += bBefore1916 ? "name=" + drTarg.LastName + ", " + drTarg.FirstName + "&county=STATEWIDE" : "firstName=" + drTarg.FirstName + "&lastName=" + drTarg.LastName + "&middleName=&county=STATEWIDE";
            return sUrl;
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

            HtmlElement eTable = docResponse.GetElementById("theTable");
            if (eTable == null)
                return;
            HtmlElementCollection cRows = eTable.GetElementsByTagName("TR");
            List<string> lPropertyNames = new List<string>();

            for (int i = 0; i < cRows.Count; ++i)
            {
                HtmlElement eNextRow = cRows[i];
                HtmlElementCollection cFields = eNextRow.GetElementsByTagName("TD");
                int j = 0;
                IllinoisDeathRecord drNew = IllinoisDeathRecord.Create("NewDeathRecord");

                foreach (HtmlElement eField in cFields)
                {
                    string sValue = eField.InnerText;
                    if (string.IsNullOrEmpty(sValue))
                        sValue = string.Empty;
                    else
                        sValue = sValue.Trim();

                    if (i == 0)
                    {
                        lPropertyNames.Add(sValue);
                    }
                    else
                    {
                        drNew.SetDeathIndexProperty(lPropertyNames[j], sValue);
                    }
                    ++j;
                }

                // The first row in the table is the header row, not a death record
                if (i > 0)
                {
                    string sName = drNew.LastName + ", " + drNew.FirstName + " (" + drNew.CertificateNumber + ")";
                    mRecords.Add(DeathRecord.Create(sName, drNew));
                }
            }
        }

        #endregion

    }
}