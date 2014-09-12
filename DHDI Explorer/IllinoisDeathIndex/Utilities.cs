using System;
using System.Windows.Forms;
using dbAccess;
using Genealogy.Properties;

namespace Genealogy
{
    /// <summary>
    /// General-purpose utilities class
    /// </summary>
    /// <remarks>This class contains only static members, methods and properties</remarks>
    public sealed class Utilities
    {
        #region Public properties

        /// <summary>
        /// Database connection (if available, otherwise null)
        /// </summary>
        public static Linq2SqlDataContext DataContext
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
                string sFolder = Settings.Default.DataFilesFolder;
                if (string.IsNullOrEmpty(sFolder))
                {
                    PromptForDataFilesFolder();
                    sFolder = Settings.Default.DataFilesFolder;
                }
                return sFolder;
            }
            set
            {
                Settings.Default.DataFilesFolder = value;
                // TODO: Enable Genealogy.Properties.Settings.Default.Save() when ready to commit...
                // Genealogy.Properties.Settings.Default.Save();
            }
        }

        #endregion

        /// <summary>
        /// Prompt the user to identify the disk folder in which the flat data files can be found
        /// </summary>
        public static void PromptForDataFilesFolder()
        {
            var fDataFiles = new FolderBrowserDialog
            {
                Description = Resources.Utilities_PromptForDataFilesFolder
            };

            string sDataFilesPath = Settings.Default.DataFilesFolder;

            fDataFiles.SelectedPath = System.IO.Directory.Exists(sDataFilesPath) ? sDataFilesPath : System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            DialogResult bOk = fDataFiles.ShowDialog();
            if (bOk == DialogResult.OK)
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
            string sDataFilesFolder = Settings.Default.DataFilesFolder;
            sFullPath = string.Empty;

            if (string.IsNullOrEmpty(sDataFilesFolder))
            {
                MessageBox.Show(string.Format(Resources.Utilities_SetDataFilesFolder, Environment.NewLine), "No data files folder location set", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return DialogResult.Cancel;
            }

            if (!System.IO.Directory.Exists(sDataFilesFolder))
            {
                MessageBox.Show(string.Format(Resources.Utilities_SetValidDataFilesFolder,
                    Environment.NewLine, Environment.NewLine, Environment.NewLine, sDataFilesFolder, Environment.NewLine), "Invalid data files folder location", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// <summary>
        /// Days of the week, in Czech (nominative case?), using HTML entities to represent characters with diacriticals
        /// </summary>
        static private readonly string[] DaysOfWeekList =
        {
            "Ned&#283;le", "Pond&#283;l&iacute;", "&Uacute;ter&yacute", "St&#345;eda",
            "&#268;tvrtek", "P&aacute;tek", "Sobota"
        };

        /// <summary>
        /// Months of the year, in Czech (nominative case?), using HTML entities to represent characters with diacriticals
        /// </summary>
        static private readonly string[] MonthsList =
        {
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

        /// <summary>
        /// Convert a <see cref="DateTime"/> structure to a Czech-language long form
        /// </summary>
        /// <param name="dtWhen"></param>
        /// <returns></returns>
        public static string CzechDate( DateTime dtWhen )
        {
            return DaysOfWeekList[(int)dtWhen.DayOfWeek] + " " + dtWhen.Day + ". "
                + MonthsList[dtWhen.Month - 1] + " " + dtWhen.Year;
        }
    }
}
