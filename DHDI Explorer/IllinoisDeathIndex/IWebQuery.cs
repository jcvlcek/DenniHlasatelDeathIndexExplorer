using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    /// <summary>
    /// Interface for genealogical queries made via web- (HTML)-based methods
    /// </summary>
    interface IWebQuery
    {
        /// <summary>
        /// Submit the query to a remote service
        /// </summary>
        void Submit();

        /// <summary>
        /// The Uniform Resource Locator (URL) of the web services provider
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Event raised on the (asynchronous) completion of the web request
        /// </summary>
        event EventHandler<WebQueryEventArgs> QueryCompleted;
    }
}
