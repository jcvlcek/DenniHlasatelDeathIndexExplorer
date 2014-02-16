using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dbAccess;

namespace Genealogy
{
    public sealed class Utilities
    {
        #region Public properties

        /// <summary>
        /// Database connection (if available, otherwise null)
        /// </summary>
        public static Linq2SqlDataContext dB
        {
            get;
            set;
        }

        /// <summary>
        /// The root folder of the data files used in this application
        /// </summary>
        public static string DataFilesFolder
        {
            get { 
                string sFolder = Genealogy.Properties.Settings.Default.DataFilesFolder;
                if (string.IsNullOrEmpty(sFolder))
                {
                    PromptForDataFilesFolder();
                    sFolder = Genealogy.Properties.Settings.Default.DataFilesFolder;
                }
                return sFolder;
            }
            set
            {
                Genealogy.Properties.Settings.Default.DataFilesFolder = value;
                // TODO: Enable Genealogy.Properties.Settings.Default.Save() when ready to commit...
                // Genealogy.Properties.Settings.Default.Save();
            }
        }

        #endregion

        public static void PromptForDataFilesFolder()
        {
            FolderBrowserDialog fDataFiles = new FolderBrowserDialog();
            fDataFiles.Description = "Select the folder containing the death records data files:";

            string sDataFilesPath = Genealogy.Properties.Settings.Default.DataFilesFolder;

            if (System.IO.Directory.Exists(sDataFilesPath))
                fDataFiles.SelectedPath = sDataFilesPath;
            else
                fDataFiles.SelectedPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            DialogResult bOk = fDataFiles.ShowDialog();
            if (bOk == System.Windows.Forms.DialogResult.OK)
                DataFilesFolder = fDataFiles.SelectedPath;
        }

        /// <summary>
        /// Check for the existence of a data file in the folder specified by the application's settings,
        /// returning a flag indicating whether the data file exists, and the full path
        /// </summary>
        /// <param name="sFile">The data file to search for.  This argument should be in the form of a relative path, which may contain leading subfolder names</param>
        /// <param name="sFullPath">Contains on return the full path of the data file, if found, otherwise the empty string</param>
        /// <returns>DialogResult.OK if the file is found, otherwise DialogResult.Cancel</returns>
        public static DialogResult CheckForDataFile(string sFile, out string sFullPath)
        {
            string sDataFilesFolder = Genealogy.Properties.Settings.Default.DataFilesFolder;
            sFullPath = string.Empty;

            if (string.IsNullOrEmpty(sDataFilesFolder))
            {
                MessageBox.Show("Please set a data files folder location" + Environment.NewLine + "from the File menu", "No data files folder location set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Cancel;
            }

            if (!System.IO.Directory.Exists(sDataFilesFolder))
            {
                MessageBox.Show("Please set a valid data files folder location" + Environment.NewLine + "from the File menu. The current folder:" + Environment.NewLine + sDataFilesFolder + Environment.NewLine + "does not exist", "Invalid data files folder location", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return DialogResult.Cancel;
            }

            sFullPath = System.IO.Path.Combine(sDataFilesFolder, sFile);

            if (string.IsNullOrEmpty(sFile))
                return DialogResult.OK;

            if (!System.IO.File.Exists(sFullPath) && !System.IO.Directory.Exists( sFullPath ) )
            {
                sFullPath = string.Empty;
                MessageBox.Show("Data files folder" + Environment.NewLine + "exists, but the required file or subfolder" + Environment.NewLine + sFile + Environment.NewLine + "cannot be found there, sorry.", "Cannot find required file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return DialogResult.Cancel;
            }

            return DialogResult.OK;
        }

        // Neděle, pondělí, úterý, středa, čtvrtek, pátek, sobota
        static private string[] mDays = new string[] {
            "Ned&#283;le", "Pond&#283;l&iacute;", "&Uacute;ter&yacute", "St&#345;eda",
            "&#268;tvrtek", "P&aacute;tek", "Sobota"
        };

        // 
        static private string[] mMonths = new string[] {
            "Ledna",
            "&Uacute;nora",
            "B&#345;ezna",
            "Dubna",
            "Kv&#283;tna",
            "&#268;ervna",
            "&#268;ervence",
            "Srpna",
            "Z&aacute;&#345;&iacute;",
            "&#344;&iacute;jna",
            "Listopadu",
            "Prosince"
        };

        public static string CzechDate( DateTime dtWhen )
        {
            return mDays[(int)dtWhen.DayOfWeek] + " " + dtWhen.Day.ToString() + ". "
                + mMonths[dtWhen.Month - 1] + " " + dtWhen.Year.ToString();
        }
    }
}
