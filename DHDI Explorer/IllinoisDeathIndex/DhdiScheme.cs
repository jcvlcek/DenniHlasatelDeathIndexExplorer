using System;
using System.Globalization;
using System.Linq;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace Genealogy
{
    /// <summary>
    /// Custom URL scheme for redirecting hyperlinks into Denni Hlasatel Death Index program code
    /// </summary>
    public class DhdiScheme
    {
        #region Public methods

        /// <summary>
        /// Execute a Denni Hlasatel Death Index query URI,
        /// and place its output into a specified location
        /// </summary>
        /// <param name="elemTarg">the HTML element into which the query outut will be directed</param>
        /// <param name="sPath">the Denni Hlasatel Death Index query to be executed</param>
        /// <param name="lQuery">the arguments to the query, as name/value pairs</param>
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

        /// <summary>
        /// List, for debugging purposes, the parameters passed to a Denni Hlasatel Death Index query
        /// </summary>
        /// <param name="elemTarg">the HTML element into which the list outut will be directed</param>
        /// <param name="sPath">the Denni Hlasatel Death Index query to be executed</param>
        /// <param name="lQuery">the arguments to the query, as name/value pairs</param>
        /// <param name="sBaseMessage">a header string that will be prepended to the list output</param>
        private static void ListQueryParameters(HtmlElement elemTarg, string sPath, NameValueCollection lQuery, string sBaseMessage)
        {
            var sMessage = sBaseMessage + "Path: " + sPath + "<br>" + Environment.NewLine;
            var items = lQuery.AllKeys.SelectMany(lQuery.GetValues, (k, v) => new { key = k, value = v });
            sMessage = items.Aggregate(sMessage, (current, item) => current + (item.key + " = " + item.value + "<br>" + Environment.NewLine));
            // MessageBox.Show(sMessage, "DH Death Index URL", MessageBoxButtons.OK, MessageBoxIcon.Information);
            elemTarg.InnerHtml = sMessage;
        }

        /// <summary>
        /// Search the Denni Hlasatel Death Index for a specific surname
        /// (specified by the parameter in <paramref name="lQuery"/> named <value>Surname</value>)
        /// </summary>
        /// <param name="elemTarg">the HTML element into which the query outut will be directed</param>
        /// <param name="sPath">the Denni Hlasatel Death Index query to be executed</param>
        /// <param name="lQuery">the arguments to the query, as name/value pairs</param>
        public static void SurnameSearch(HtmlElement elemTarg, string sPath, NameValueCollection lQuery)
        {
            if (lQuery.AllKeys.Contains("Surname"))
            {
                string sSurname = lQuery["Surname"];
                string sHtml = "<h2>Matches for surname \"" + sSurname + "\":</h2>" + Environment.NewLine;
                var q = from c in Utilities.DataContext.Prijmenis
                        where c.PlainText == sSurname
                        select c;

                sHtml = Enumerable.Aggregate(q, sHtml, (current, surNext) => current + (surNext.Web + "<br>" + Environment.NewLine + "Count: " + surNext.Count.ToString(CultureInfo.InvariantCulture) + "<br>" + Environment.NewLine + "Rank: " + surNext.Rank.ToString(CultureInfo.InvariantCulture) + "<br>" + Environment.NewLine));

                elemTarg.InnerHtml = sHtml;
            }
            else
            {
                ListQueryParameters(elemTarg, sPath, lQuery, "<h2>Error: Web link did not contain a \"Surname\" to search for</h2>" + Environment.NewLine);
            }
        }

        /// <summary>
        /// Default Denni Hlasatel Death Index URI handler
        /// (invoked if no handler is registered for the specified value of <paramref name="sPath"/>)
        /// </summary>
        /// <param name="elemTarg">the HTML element into which the query outut will be directed</param>
        /// <param name="sPath">the (unrecognized) Denni Hlasatel Death Index query</param>
        /// <param name="lQuery">the arguments to the query, as name/value pairs</param>
        public static void Default(HtmlElement elemTarg, string sPath, NameValueCollection lQuery)
        {
            ListQueryParameters(elemTarg, sPath, lQuery, "<h2>Error: Unrecognized Denni Hlasatel Web Function</h2>" + Environment.NewLine + "Function: " + sPath + "<br>" + Environment.NewLine);
        }

        #endregion

    }
}
