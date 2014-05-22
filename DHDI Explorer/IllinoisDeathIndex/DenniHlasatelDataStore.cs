using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// <para>Represents the Denni Hlasatel death index stored in a single XML document</para>
    /// Also provides utility functions for examining Denni Hlasatel death records in the original file format
    /// </summary>
    public class DenniHlasatelDataStore
    {
        #region Private members

        /// <summary>
        /// The XML document containing the Denni Hlasatel death index
        /// </summary>
        private XDocument mXmlRecords = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a Denni Hlasatel death index from a specified XML document
        /// </summary>
        /// <param name="sDocumentPath">the XML document containing the Denni Hlasatel death index</param>
        public DenniHlasatelDataStore(string sDocumentPath)
        {
            mXmlRecords = XDocument.Load(sDocumentPath);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the XML document containing the Denni Hlasatel death index records
        /// </summary>
        public XDocument XmlDocument
        {
            get { return mXmlRecords; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create a death record from a single line in an original (non-XML) Denni Hlasatel death index file
        /// </summary>
        /// <param name="sRecord">a single line in a Denni Hlasatel death index file</param>
        /// <returns>a death record constructed from <paramref name="sRecord"/></returns>
        public static IDeathRecord CreateFromRecord(string sRecord)
        {
            IDeathRecord drNew = DeathRecord.Create(sRecord);
            string[] sElements = sRecord.Split(',');
            drNew.FirstName = sElements[0];
            drNew.LastName = sElements[1];
            DateTime dtTarg = DateTime.MinValue;
            if (sElements.Length >= 5)
                DateTime.TryParse(sElements[2] + " " + sElements[3] + " " + sElements[4], out dtTarg);

            drNew.FilingDate = dtTarg;
            return drNew;
        }

        /// <summary>
        /// Create a death record from a full (first + last) name and a death date
        /// </summary>
        /// <param name="sFirstName">the first (given) name of the decedent</param>
        /// <param name="sLastName">the last (family) name of the decedent</param>
        /// <param name="sDate">the filing date of the death certificate</param>
        /// <returns>a death record constructed from the supplied parameters</returns>
        public static IDeathRecord CreateFromNameAndDate(string sFirstName, string sLastName, string sDate)
        {
            IDeathRecord drNew = DeathRecord.Create(sFirstName + " " + sLastName);
            drNew.FirstName = sFirstName;
            drNew.LastName = sLastName;
            DateTime dtTarg = DateTime.MinValue;
            DateTime.TryParse( sDate, out dtTarg );
            drNew.FilingDate = dtTarg;
            return drNew;
        }

        private static System.IO.TextReader fDataFile = null;

        /// <summary>
        /// Read the next death record from a Denni Hlasatel death index data file (in the original format)
        /// </summary>
        /// <returns>a death record created from the next line in the file</returns>
        /// <remarks>DEPRECATED - this was used only for testing purposes and should be deleted</remarks>
        public static IDeathRecord Next()
        {
            if (fDataFile == null)
            {
                string sNextDhFile;

                if (System.Windows.Forms.DialogResult.OK != Utilities.CheckForDataFile(string.Empty, out sNextDhFile))
                    throw new System.IO.DirectoryNotFoundException("Data files folder \"" + Utilities.DataFilesFolder + "\" not found");
                if (System.Windows.Forms.DialogResult.OK != Utilities.CheckForDataFile(@"DH Files\DH-V.txt", out sNextDhFile))
                    throw new System.IO.FileNotFoundException("Could not find Denni Hlasatel data file \"DH-V.txt\"");

                fDataFile = new System.IO.StreamReader(sNextDhFile);
            }

            string sNextEntry = fDataFile.ReadLine();
            // TODO: Detect end of file
            return CreateFromRecord(sNextEntry);
        }

        #endregion
    }
}
