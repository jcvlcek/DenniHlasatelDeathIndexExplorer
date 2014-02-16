using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// Arguments provided by web query events,
    /// passed to event handlers
    /// </summary>
    public class WebQueryEventArgs : EventArgs
    {
        #region Private members

        /// <summary>
        /// The list of results returned by the Web query
        /// </summary>
        private List<IDeathRecord> mResults;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a web query event arguments instance,
        /// using the supplied results list
        /// </summary>
        /// <param name="lResults">The list of "hits" returned by the web query</param>
        public WebQueryEventArgs(List<IDeathRecord> lResults)
        {
            if (lResults != null)
                mResults = lResults;
            else
                mResults = new List<IDeathRecord>();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The list of "hits" (matches) in response to a web query
        /// </summary>
        public List<IDeathRecord> Results { get { return mResults; } }

        #endregion
    }
}
