using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using dbAccess;

namespace Genealogy
{
    public class DenniHlasatelDataStore
    {
        #region Private members

        private XDocument mXmlRecords = null;

        #endregion

        #region Constructors

        public DenniHlasatelDataStore(string sDocumentPath)
        {
            mXmlRecords = XDocument.Load(sDocumentPath);
        }

        #endregion

        #region Public properties

        public XDocument XmlDocument
        {
            get { return mXmlRecords; }
        }

        #endregion

        #region Public methods

        public static IDeathRecord CreateFromRecord(string sRecord)
        {
            string sName = sRecord;

            IDeathRecord drNew = DeathRecord.Create(sName);
            string[] sElements = sRecord.Split(',');
            drNew.FirstName = sElements[0];
            drNew.LastName = sElements[1];
            DateTime dtTarg = DateTime.MinValue;
            if (sElements.Length >= 5)
                DateTime.TryParse(sElements[2] + " " + sElements[3] + " " + sElements[4], out dtTarg);

            drNew.FilingDate = dtTarg;
            return drNew;
        }

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

        private static string sDataFilesFolder = "";
        private static System.IO.TextReader fDataFile = null;

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
