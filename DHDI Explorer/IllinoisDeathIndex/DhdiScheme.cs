using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace Genealogy
{
    public class DhdiScheme
    {
        #region Public methods

        public static void NavigateTo(HtmlElement elemTarg, string sPath, NameValueCollection lQuery)
        {
            if (sPath.StartsWith("/"))
                sPath = sPath.Substring(1);

            switch ( sPath ) {
                case "SurnameSearch":
                    SurnameSearch(elemTarg, sPath, lQuery);
                    break;
                case "GivenNameSearch":
                    break;
                default:
                    Default(elemTarg, sPath, lQuery);
                    break;
            }
        }

        #endregion

        #region Private methods

        private static void ListQueryParameters(HtmlElement elemTarg, string sPath, NameValueCollection lQuery, string sBaseMessage)
        {
            string sMessage = sBaseMessage;
            var items = lQuery.AllKeys.SelectMany(lQuery.GetValues, (k, v) => new { key = k, value = v });
            foreach (var item in items)
                sMessage += item.key + " = " + item.value + "<br>" + Environment.NewLine;
            // MessageBox.Show(sMessage, "DH Death Index URL", MessageBoxButtons.OK, MessageBoxIcon.Information);
            elemTarg.InnerHtml = sMessage;
        }

        public static void SurnameSearch(HtmlElement elemTarg, string sPath, NameValueCollection lQuery)
        {
            if (lQuery.AllKeys.Contains("Surname"))
            {
                string sSurname = lQuery["Surname"];
                string sHtml = "<h2>Matches for surname \"" + sSurname + "\":</h2>" + Environment.NewLine;
                var q = from c in Utilities.dB.Prijmenis
                        where c.PlainText == sSurname
                        select c;

                foreach (var surNext in q)
                {
                    sHtml +=
                        surNext.Web + "<br>" + Environment.NewLine +
                        "Count: " + surNext.Count.ToString() + "<br>" + Environment.NewLine +
                        "Rank: " + surNext.Rank.ToString() + "<br>" + Environment.NewLine;
                }

                elemTarg.InnerHtml = sHtml;
            }
            else
            {
                ListQueryParameters(elemTarg, sPath, lQuery, "<h2>Error: Web link did not contain a \"Surname\" to search for</h2>" + Environment.NewLine);
            }
        }

        public static void Default(HtmlElement elemTarg, string sPath, NameValueCollection lQuery)
        {
            ListQueryParameters(elemTarg, sPath, lQuery, "<h2>Error: Unrecognized Denni Hlasatel Web Function</h2>" + Environment.NewLine + "Function: " + sPath + "<br>" + Environment.NewLine);
        }

        #endregion

    }
}
