using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Linq;
using dbAccess;
using Genealogy.Properties;

namespace Genealogy
{
    /// <summary>
    /// Main form for the Denni Hlasatel / Illinois Death Index Explorer program
    /// </summary>
    public partial class Form1
    {
        #region Private members
        private readonly List<string> _mGivenNamesCache = new List<string>();
        private readonly List<string> _mSurnamesCache = new List<string>();
        private static readonly string[] MonthAbbreviations = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        readonly TreeViewWrapper _mTreeView;
        Linq2SqlDataContext _mdB;
        readonly List<IllinoisDeathIndexWebQuery> _mPendingWebQueries = new List<IllinoisDeathIndexWebQuery>();
        #endregion

        #region Constructors

        /// <summary>
        /// <para>Construct a default version of the form</para>
        /// Initializes the user interface, instantiates an SQL connection,
        /// instantiates the tree view, and initializes the web browser with an empty HTML document
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            _mTreeView = new TreeViewWrapper(tvDocument);
            webBrowser1.DocumentText = "<HTML></HTML>";
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Ensures that a day-of-the-week/month string is two characters in length
        /// (padding with a leading "0" as necessary)
        /// </summary>
        /// <param name="sInformalDay">the existing day-of-the-week/month string</param>
        /// <returns>a two-character day-of-the-week/month string, padded with a leading "0" as necessary</returns>
        private string FormalDay(string sInformalDay)
        {
            return sInformalDay.Length < 2 ? "0" + sInformalDay : sInformalDay;
        }

        static readonly List<string> MonthList = new List<string>(new[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" });

        /// <summary>
        /// Converts a three-character month abbreviation to a two-character month index (1-referenced).
        /// The month index is zero-padded on the left for the months "Jan" through "Sep".
        /// </summary>
        /// <param name="sInformalMonth">a three-character month abbreviation</param>
        /// <returns>a two-character month index, in the range "01" to "12"</returns>
        /// <remarks>Accepts only the US English month abbreviations</remarks>
        private string FormalMonth(string sInformalMonth)
        {
            var iMonth = MonthList.IndexOf(sInformalMonth.ToUpper()) + 1;
            return iMonth < 10 ? "0" + iMonth : iMonth.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Changes the binding source propert of the data grid view.
        /// This has the effect of displaying a different database table in the grid view.
        /// </summary>
        /// <param name="bsTarg">the new binding source (database table) to display in the grid view</param>
        private void ChangeBindingSource(BindingSource bsTarg)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.Enabled = false;
            dataGridView1.Invalidate();
            dataGridView1.Enabled = true;
            dataGridView1.DataSource = bsTarg;
            dataGridView1.Refresh();
        }

        /// <summary>
        /// <para>Event handler called when a web query (of any flavor) has completed</para>
        /// Displays the <code>Results</code> list in the tree and list views,
        /// and submits for processing any queries in the "pending" queue
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">the results of the web query, including the <code>Results</code> list of death records</param>
        private void OnWebQueryCompleted(object sender, WebQueryEventArgs e)
        {
            HtmlDocument docResponse = webBrowser1.Document;
            _mTreeView.DisplayInTree(docResponse);
            txtResponse.Text = webBrowser1.DocumentText;
            DateTime dtTarg;
            var bAnyDate = !DateTime.TryParse(txtDate.Text.Replace(',', ' '), out dtTarg);

            // Display the results list contents in both the tree and list views
            DisplayRecords(e.Results, false);
            foreach (IDeathRecord recNext in e.Results)
            {
                if ( recNext is IObject )
                    _mTreeView.DisplayInTree((IObject)recNext);
            }

            foreach (ListViewItem itmNext in lvHits.Items)
            {
                var recNext = (IDeathRecord)itmNext.Tag;
                if (bAnyDate)
                    itmNext.ImageKey = Resources.SearchMatchString;
                else if ((dtTarg >= recNext.DeathDate) &&
                    (dtTarg <= recNext.DeathDate.AddDays(4.0)))
                    itmNext.ImageKey = Resources.SearchMatchString;
                else
                    itmNext.ImageKey = Resources.SearchNoMatchString;
            }

            if (_mPendingWebQueries.Count > 0)
            {
                IllinoisDeathIndexWebQuery qNext = _mPendingWebQueries[0];
                _mPendingWebQueries.RemoveAt(0);
                qNext.QueryCompleted += OnWebQueryCompleted;
                qNext.Submit();
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Load event handler for the main form <see cref="Form1"/>
        /// </summary>
        /// <param name="sender">the originator of the event (an instance of <see cref="Form1"/>)</param>
        /// <param name="e">arguments to the event handler</param>
        /// <remarks>This event handler currently loads the database table adapters.
        /// These calls to the <code>TableAdapter.Fill</code> method will ultimately be moved
        /// to another location, to enable selective connection to the database.</remarks>
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // TODO: This line of code loads data into the 'genealogyDataSet.KrestniJmena' table. You can move, or remove it, as needed.
                krestniJmenaTableAdapter.Fill(genealogyDataSet.KrestniJmena);
                // TODO: This line of code loads data into the 'genealogyDataSet.Prijmeni' table. You can move, or remove it, as needed.
                prijmeniTableAdapter.Fill(genealogyDataSet.Prijmeni);
                // TODO: This line of code loads data into the 'genealogyDataSet.GivenNameEquivalents' table. You can move, or remove it, as needed.
                givenNameEquivalentsTableAdapter.Fill(genealogyDataSet.GivenNameEquivalents);
                // TODO: This line of code loads data into the 'genealogyDataSet.DHDeathIndex' table. You can move, or remove it, as needed.
                dHDeathIndexTableAdapter.Fill(genealogyDataSet.DHDeathIndex);

                _mdB = new Linq2SqlDataContext();
                Utilities.DataContext = _mdB;

                var query = from c in _mdB.DHDeathIndexes
                            select c.GivenName;

                foreach (string sNext in query)
                {
                    if (!txtFirstName.AutoCompleteCustomSource.Contains(sNext))
                        txtFirstName.AutoCompleteCustomSource.Add(sNext);
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show( "Unable to load/fill database table adapters:" + Environment.NewLine + ex.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }

            webBrowser1.Navigating += webBrowser1_Navigating;
        }

        /// <summary>
        /// Event raised by the web browser when (just prior to) navigating to a new URL
        /// </summary>
        /// <param name="sender">the originator of te event</param>
        /// <param name="e">additional details of the navigation event</param>
        /// <remarks>This event is hooked, to enable the program to detect URLs with custom schemes.
        /// For example, the <code>dhdi</code> scheme represents a query
        /// into the Denni Hlasatel death indes</remarks>
        void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme == "dhdi")
            {
                string sFunction = e.Url.AbsolutePath;
                string sQuery = e.Url.IsAbsoluteUri ? e.Url.Query : string.Empty;
                string unableToExecute = "Unable to execute function \"" + sFunction + @"\";

                System.Collections.Specialized.NameValueCollection lQuery =
                    System.Web.HttpUtility.ParseQueryString(sQuery);

                if (webBrowser1.Document == null)
                {
                    MessageBox.Show("No browser document loaded", unableToExecute, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (webBrowser1.Document.Window == null)
                {
                    MessageBox.Show("Browser document window is not set", unableToExecute, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (webBrowser1.Document.Window.Frames == null)
                {
                    MessageBox.Show("Browser document window contains no frames", unableToExecute, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (webBrowser1.Document.Window.Frames["Display_Area"] == null)
                {
                    MessageBox.Show("Frame \"Display_Area\" not found in the browser document window", unableToExecute, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (webBrowser1.Document.Window.Frames["Display_Area"].Document == null)
                {
                    MessageBox.Show("Frame \"Display_Area\" has no document associated with it", unableToExecute, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (webBrowser1.Document.Window.Frames["Display_Area"].Document.Body == null)
                {
                    MessageBox.Show("Document associated with frame \"Display_Area\" has no body", unableToExecute, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else 
                {
                    DhdiScheme.NavigateTo(webBrowser1.Document.Window.Frames["Display_Area"].Document.Body, sFunction, lQuery);
                } 
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Event handler for the list view containing "hits" in the Illinois death index database.
        /// Adds (if possible) the death record clicked on into the appropriate local Illinois Death index database.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details associated with the event</param>
        /// <remarks>This implementation of the double-click event handler is experimental,
        /// to enable testing the code for adding records to the local Illinois death index databases.
        /// It is unlikely that the final version of the software will include this method of adding database records</remarks>
        private void lvHits_DoubleClick(object sender, EventArgs e)
        {
            DateTime dtWhen = DateTime.Parse( txtDate.Text.Replace( ',', ' ' ));

            var query = from c in _mdB.DHDeathIndexes
                        where ( c.GivenName == txtFirstName.Text ) &&
                            ( c.Surname == txtLastName.Text ) &&
                            ( c.ReportDate.Year == dtWhen.Year ) &&
                            ( c.ReportDate.Month == dtWhen.Month ) &&
                            ( c.ReportDate.Day == dtWhen.Day )
                        select c;

            foreach ( var q in query )
            {
                foreach (ListViewItem itmNext in lvHits.SelectedItems)
                {
                    IDeathRecord drTarg = (IDeathRecord)itmNext.Tag;

                    if (drTarg.DeathDate.Year > 1915)
                    {
                        IllinoisDeathIndexPost1915 indxNew = IllinoisDeathIndex.CreatePost1915(drTarg);
                        _mdB.IllinoisDeathIndexPost1915s.InsertOnSubmit(indxNew);
                        try
                        {
                            _mdB.SubmitChanges();
                            MessageBox.Show("Added to Illinois post-1915 db as serial #" + indxNew.Serial, "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Failed to add to post-1915 db", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        IllinoisDeathIndexPre1916 indxNew = IllinoisDeathIndex.CreatePre1916(drTarg);
                        _mdB.IllinoisDeathIndexPre1916s.InsertOnSubmit(indxNew);
                        try
                        {
                            _mdB.SubmitChanges();
                            MessageBox.Show("Added to Illinois pre-1916 db as serial #" + indxNew.serial.ToString(), "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Failed to add to pre-1916 db", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <para>Event handler for the Denni Hlasatel menu item</para> 
        /// Display the Denni Hlasatel death index table in the program's grid view
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void denniHlasatelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBindingSource(dHDeathIndexBindingSource);
        }

        /// <summary>
        /// <para>Event handler for the given name equivalents menu item</para> 
        /// Display the given name equivalents table in the program's grid view
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void givenNameEquivalentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBindingSource(givenNameEquivalentsBindingSource);
        }

        /// <summary>
        /// <para>Event handler for the Prijmeni menu item</para> 
        /// Display the Prijmeni (family or surname) table in the program's grid view
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void prijmeniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBindingSource(prijmeniBindingSource);
        }

        /// <summary>
        /// <para>Event handler for the Krestni Jmena menu item</para> 
        /// Display the Krestni Jmena (given or Christian name) table in the program's grid view
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void krestniJmenaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBindingSource(krestniJmenaBindingSource);
        }

        /// <summary>
        /// <para>Event handler for the "frames dump" menu item</para>
        /// Dumps the contents of the frame(s) contained in a Family Search web query reply
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>This event handler is for diagnostic purposes only,
        /// and will be removed from the final version of the software.
        /// Specifically, it has been used in a (to date unsuccessful) attempt
        /// to determine a way to get the actual results out of a Family Search reply</remarks>
        private void framesDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FamilySearchWebQuery.WriteOutFrame(webBrowser1);
        }

        /// <summary>
        /// Queries the Illinois Statewide Death Index databases
        /// for a match to the data currently entered into the user interface
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void illinoisDeathIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mPendingWebQueries.Clear();
            txtResponse.Clear();
            ClearRecords();

            IDeathRecord drTarg = DenniHlasatelDataStore.CreateFromRecord(txtFirstName.Text + "," + txtLastName.Text + "," + txtDate.Text);
            GivenName gnTarg = GivenName.Get(txtFirstName.Text);
            String lAlternateGivenNameSpelling = gnTarg.EquivalentNativeForm;
            List<string> lEquivalentForms = gnTarg.EquivalentForms;
            List<string> lAlternateGivenNames = lEquivalentForms.GetRange(1, lEquivalentForms.Count - 1);

            if (lAlternateGivenNames.Count > 0)
            {
                string sAlternateForms = txtFirstName.Text + " ";
                if (lAlternateGivenNameSpelling.Length > 0)
                {
                    sAlternateForms += "(" + lAlternateGivenNameSpelling + ") ";
                }
                sAlternateForms += txtLastName.Text + Environment.NewLine;

                sAlternateForms = lAlternateGivenNames.Aggregate(sAlternateForms, (current, sNext) => current + (sNext + " " + txtLastName.Text + Environment.NewLine));

                MessageBox.Show("All possible forms of this name:" + Environment.NewLine + sAlternateForms, "Alternate names possible", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (lAlternateGivenNames.Count > 0)
            {
                foreach (string sNextName in lAlternateGivenNames)
	            {
                    IDeathRecord drNext = DenniHlasatelDataStore.CreateFromRecord(sNextName + "," + txtLastName.Text + "," + txtDate.Text);
                    var qNext = new IllinoisDeathIndexWebQuery( drNext, webBrowser1 );
                    _mPendingWebQueries.Add(qNext);
                    if (drTarg.FilingDate.Year < 1700)
                    {
                        drNext = DenniHlasatelDataStore.CreateFromRecord(sNextName + "," + txtLastName.Text + "," + "1,Jan,1916");
                        _mPendingWebQueries.Add(new IllinoisDeathIndexWebQuery(drNext, webBrowser1));
                    }
	            }
            }

            var qNew = new IllinoisDeathIndexWebQuery(drTarg, webBrowser1);
            if (drTarg.FilingDate.Year < 1700)
            {
                drTarg = DenniHlasatelDataStore.CreateFromRecord(txtFirstName.Text + "," + txtLastName.Text + "," + "1,Jan,1916");
                _mPendingWebQueries.Add(new IllinoisDeathIndexWebQuery(drTarg, webBrowser1));
            }
            qNew.QueryCompleted += OnWebQueryCompleted;
            qNew.Submit();
        }

        /// <summary>
        /// Queries the Family Search databases
        /// for a match to the data currently entered into the user interface
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void familySearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtResponse.Clear();

            // IDeathRecord drTarg = DenniHlasatelDataStore.CreateFromRecord(txtFirstName.Text + "," + txtLastName.Text + "," + txtDate.Text);
            IDeathRecord drTarg = DenniHlasatelDataStore.CreateFromNameAndDate(txtFirstName.Text, txtLastName.Text, txtDate.Text);
            var qNew = new FamilySearchWebQuery(drTarg, webBrowser1);
            qNew.QueryCompleted += OnWebQueryCompleted;
            qNew.Submit();
        }

        /// <summary>
        /// Copies the original Denni Hlasatel data files into a single XML-formatted output file
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void convertDHFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CombineDhFilesIntoXml();
        }

        /// <summary>
        /// Copies the names in the "prijmeni.xls" spreadsheet, converting them to proper casing
        /// (only the first character upper-cased) and writes the output into a "prijmeni lowercase.xls" file
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void prijmeniConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sPrijmeniPath;

            if (DialogResult.OK != Utilities.CheckForDataFile("prijmeni.xls", out sPrijmeniPath))
                return;
            var fNames = new ExcelFile(sPrijmeniPath);
            var fNamesOut = new ExcelFile();

            var pTarg = new Point(5, 1);
            var pHtml = new Point(4, 1);
            int iBlanks, iNames, iRow;
            for (iBlanks = 0, iNames = 0, iRow = 2; iBlanks < 5; ++iRow)
            {
                string sValue = string.Empty;
                string sHtml = string.Empty;

                foreach (var iNext in new[] { 1, 2, 3, 4, 5, 6 })
                {
                    var pNext = new Point(iNext, iRow);
                    string sNextValue = fNames.ValueAt(pNext);
                    if (iNext == 4)
                        sHtml = sNextValue;
                    if (iNext == 5)
                        sValue = sNextValue;
                    string sValueOut = sNextValue;
                    if ( ( iNext >= 2 ) && ( iNext <= 5 ) )
                        sValueOut = WebQuery.ConvertToProperCase( sNextValue );
                    fNamesOut.SetValueAt(pNext, sValueOut);
                }

                if ((iRow % 100) == 0)
                {
                    txtLastName.Text = sValue;
                    txtFirstName.Text = iRow.ToString(CultureInfo.InvariantCulture);
                    Application.DoEvents();
                }

                pTarg.Y = iRow; pHtml.Y = iRow;
                if (string.IsNullOrEmpty(sValue))
                    ++iBlanks;
                else
                {
                    /*
                    foreach (char cNext in sValue.ToCharArray())
                    {
                        if ((cNext < 0x20) || (cNext > 127))
                            MessageBox.Show("Name with special character (" + sValue + ")" + Environment.NewLine +
                                "found in row " + iRow.ToString(), "Special character detected", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    foreach (char cNext in sHtml.ToCharArray())
                    {
                        if ((cNext < 0x20) || (cNext > 127))
                            MessageBox.Show("Name with special character (" + sHtml + ")" + Environment.NewLine +
                                "found in HTML string at row " + iRow.ToString(), "Special character detected", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    */
                    ++iNames;
                    iBlanks = 0;
                }
            }

            MessageBox.Show(iNames + " names found in spreadsheet", "Prijmeni scan", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fNames.Close();
            fNamesOut.SaveAs(System.IO.Path.Combine(Utilities.DataFilesFolder, "prijmeni lowercased.xls"));
            fNamesOut.Close();
        }

        /// <summary>
        /// Advances to the next record in the Denni Hlasatel data store, and displays it in the user interface
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>This method was used for diagnostic purposes only, and will be removed from the final release of the software</remarks>
        private void nextDHRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IDeathRecord drNew = DenniHlasatelDataStore.Next();
                txtFirstName.Text = drNew.FirstName;
                txtLastName.Text = drNew.LastName;
                txtDate.Text = drNew.FilingDate.Day + "," + MonthAbbreviations[drNew.FilingDate.Month - 1] + "," + drNew.FilingDate.Year;
                DisplayRecord(drNew);
                MessageBox.Show(drNew.FirstName + " " + drNew.LastName + "'s death notice was published" + Environment.NewLine
                    + drNew.FilingDate.ToLongDateString(), "Denni Hlasatel record", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message + Environment.NewLine + ex, "Unable to find next Denni Hlasatel death record", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        /// <summary>
        /// Reads through the rank-sorted Czech family names list "prijmeni rank-sorted.xls",
        /// displaying every 100-th name in the user interface, and flagging any duplicate entries
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void prijmeniRankCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sPrijmeniPath;

            if (DialogResult.OK != Utilities.CheckForDataFile("prijmeni rank-sorted.xls", out sPrijmeniPath))
                return;
            var fNames = new ExcelFile(sPrijmeniPath);

            int iBlanks, iNames, iRow, iPreviousRank = -1;
            var sPreviousName = string.Empty;
            var lRanks = new List<int>();

            for (iBlanks = 0, iNames = 0, iRow = 2; iBlanks < 5; ++iRow)
            {
                var pTarg = new Point(1, iRow);
                var pName = new Point(5, iRow);
                string sRank = fNames.ValueAt(pTarg);
                string sName = fNames.ValueAt(pName);

                if ((iRow % 100) == 0)
                {
                    txtLastName.Text = sName;
                    txtFirstName.Text = iRow.ToString();
                    Application.DoEvents();
                }

                if (string.IsNullOrEmpty(sRank))
                    ++iBlanks;
                else
                {
                    int iRank = int.Parse(sRank);
                    if (lRanks.Contains(iRank))
                        MessageBox.Show("Duplicate rank (" + sRank + ") at name \"" + sName + "\"", "Duplicate key", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    else
                        lRanks.Add(iRank);
                    iPreviousRank = iRank;
                    sPreviousName = sName;
                    ++iNames;
                    iBlanks = 0;
                }
            }

            fNames.Close();
        }

        /// <summary>
        /// <para>Event hander called when text in the last name text box is changed</para>
        /// Queries the Denni Hlasatel death index for possible results against the text currently in the box,
        /// scheduling a call to the <see cref="OnLastNameChanged"/> web document complete event handler
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            if (_mdB != null)
                DisplayDenniHlasatelResultsInBrowser(OnLastNameChanged);
        }

        /// <summary>
        /// Displays possible name matches, within the Denni Hlasatel death index,
        /// with the text in a first or last name text box
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>Possible matches are displayed only if three or more characters have been entered into the text box</remarks>
        private void OnLastNameChanged(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string sInnerHtml = string.Empty;

            string sName = txtLastName.Text;
            int iLen = sName.Length;

            if (iLen >= 3)
            {
                var query = (from c in _mdB.DHDeathIndexes
                            where c.Surname.StartsWith(sName)
                            select c.Surname).ToList().Distinct();

                foreach (string sNext in query)
                {
                    Surname snNext = Surname.Get(sNext);
                    sInnerHtml += sNext;
                    if (snNext.AlternateForms.Count > 0)
                    {
                        sInnerHtml += " (";
                        int iCount = 0;
                        foreach (Surname.NativeForm sNextAlternate in snNext.AlternateForms)
                        {
                            if ( iCount > 0 )
                                sInnerHtml += ", ";
                            ++iCount;
                            if ( String.IsNullOrEmpty( sNextAlternate.Value ) )
                                sInnerHtml += sNextAlternate.Web;
                            else
                                sInnerHtml += "<a href=\"http://www.kdejsme.cz/prijmeni/" +
                                    WebChar.ToUrlEncoding(sNextAlternate.Value) + "/hustota/\" target=\"_blank\" >" + sNextAlternate.Web + "</a>";
                        }
                        sInnerHtml += ")";
                    }
                    sInnerHtml += "<br>";
                }
            }
            else if (iLen < 3)
            {
                sInnerHtml = "<h2>Not enough characters to get a last name</h2>";
            }

            // It's lousy to use try/catch in this way, but the alternative is really stupidly painful
            // (particularly given that there's no way to test if Frame "Display_Area" exists or not...
            try
            {
                HtmlWindow frame = webBrowser1.Document.Window.Frames["Display_Area"];
                if ((frame.Document != null) && (frame.Document.Body != null))
                    frame.Document.Body.InnerHtml = sInnerHtml;
            }
            catch
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Display a collection of strings in the response text box.
        /// A count of the number of strings is displayed at the end of the list.
        /// </summary>
        /// <param name="lStrings">the collection of strings to display</param>
        /// <remarks>This method is for diagnostic purposes only, and will be removed from the final version of the software</remarks>
        private void ListStrings(IEnumerable<string> lStrings)
        {
            List<string> lDistinctNames = new List<string>(lStrings);
            lDistinctNames.Sort();
            txtResponse.Clear();
            foreach (string sNext in lDistinctNames)
                txtResponse.AppendText(sNext + Environment.NewLine);
            txtResponse.AppendText(Environment.NewLine +
                lDistinctNames.Count.ToString() + " names total" + Environment.NewLine);
        }

        /// <summary>
        /// Generate a list of all the known Czech given (first) names,
        /// taking input from any / all of the database tables.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>This method has been used only for diagnostic purposes,
        /// and it uses comments to select one of a variety of output display options
        /// (e.g. only displaying names in the Denni Hlasatel index that don't match
        /// any name in the other tables, thus generating a list of possible errors.)</remarks>
        private void listGivenNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var query = (from c in mdB.DHDeathIndexes
            //             select c.GivenName).ToList<string>().Distinct<string>();
            List<string> lAllGivenNames = new List<string>();

            var q1 = from tK in _mdB.KrestniJmenas
                     select tK.PlainText;

            foreach (string sNext in q1 )
                if ( !lAllGivenNames.Contains( sNext ) )
                    lAllGivenNames.Add( sNext );

            var q2 = from tG in _mdB.GivenNameEquivalents
                     select tG.English;

            foreach (string sNext in q2 )
                if ( !lAllGivenNames.Contains( sNext ) )
                    lAllGivenNames.Add( sNext );

            // Example of join that finds records in B not occurring in A
            //var rows = (from t1 in mdB.DHDeathIndexes
            //            join t2 in lAllGivenNames
            //            on t1.GivenName equals t2 into tg
            //            from tcheck in tg.DefaultIfEmpty()
            //            where tcheck == null
            //            select t1.GivenName).ToList<string>().Distinct<string>();

            // TODO: The following uploads too many parameters in the SQL query - fix
            //var rows = (from t1 in mdB.DHDeathIndexes
            //            where !lAllGivenNames.Contains(t1.GivenName)
            //            select t1.GivenName).ToList().Distinct();

            // The following query gets a list of given names in DH but not in our "standard names" list
            var dhNames = (from t1 in _mdB.DHDeathIndexes
                          select t1.GivenName).ToList().Distinct();

            var rows = from s1 in dhNames
                       join s2 in lAllGivenNames
                       on s1 equals s2 into tg
                       from tcheck in tg.DefaultIfEmpty()
                       where tcheck == null
                       select s1;

            List<string> lOddNames = new List<string>();
            List<string> lUnmatchedNames = new List<string>();
            foreach (string sNext in rows)
            {
                int iCount = 0;

                var oddNames = from d1 in _mdB.DHDeathIndexes
                               where d1.GivenName == sNext
                               select d1;
                foreach (DHDeathIndex dNext in oddNames)
                {
                    // lOddNames.Add(dNext.GivenName + " " + dNext.LastName + " (" + dNext.FilingDate.ToShortDateString() + ")");
                    ++iCount;
                }

                lUnmatchedNames.Add("\"" + sNext + "\"," + iCount.ToString());
            }
            // ListStrings(rows);
            // ListStrings(lOddNames);
            ListStrings(lUnmatchedNames);
        }

        /// <summary>
        /// Generate a list of all the known Czech family (last) names,
        /// taking input from any / all of the database tables.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>This method has been used only for diagnostic purposes,
        /// and it uses comments to select one of a variety of output display options
        /// (e.g. only displaying names in the Denni Hlasatel index that don't match
        /// any name in the other tables, thus generating a list of possible errors.)</remarks>
        private void listSurnamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This query generate a list of all surnames in Denni Hlasatel
            //var query = (from c in mdB.DHDeathIndexes
            //             select c.Surname).ToList<string>().Distinct<string>();

            // Example of join that finds records in B not occurring in A
            var query = (from t1 in _mdB.DHDeathIndexes
                        join t2 in _mdB.Prijmenis
                        on t1.Surname equals t2.PlainText into tg
                        from tcheck in tg.DefaultIfEmpty()
                        where tcheck.PlainText == null
                        select t1.Surname).ToList<string>().Distinct<string>();
            
            ListStrings(query);
        }

        /// <summary>
        /// Event handler for the given names transliteration menu item.
        /// No longer sure just what this did when it was last used, to be frank.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>Looks like this was used in a combination of two operations:
        /// Transliterating Czech names (with diacriticals) to English (without),
        /// and obtaining counts of the frequency of each name.</remarks>
        private void givenNamesTranslitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sGivenNames;

            if (DialogResult.OK != Utilities.CheckForDataFile("GivenNamesMaleTransliterated.xls", out sGivenNames))
                return;
            var fNames = new ExcelFile(sGivenNames);

            int iRow = 2, iBlankRows = 0, iModified = 0, iNew = 0;

            txtResponse.Clear();
            while (iBlankRows < 1)
            {
                string sId = fNames.ValueAt(new Point(1, iRow)).Trim();
                String sNative = fNames.ValueAt(new Point(2, iRow)).Trim();
                string sWeb = fNames.ValueAt(new Point(3, iRow)).Trim();
                string sPlainText = fNames.ValueAt(new Point(4, iRow)).Trim();
                string sCount = fNames.ValueAt(new Point(5, iRow)).Trim();

                //if ((iRow % 100) == 0)
                //{
                //    txtResponse.AppendText("Row " + iRow.ToString() +
                //        " (Id: " + sId + ") - " + iNew.ToString() + " records added, " +
                //        iModified.ToString() + " records modified (Current name \"" +
                //        sNative + "\", " + sCount + " individuals with this name)" + Environment.NewLine);
                //    Application.DoEvents();
                //}

                if (string.IsNullOrEmpty(sId))
                    ++iBlankRows;
                else if ( !string.IsNullOrEmpty(sNative) )
                {
                    int iCount, iIndex;

                    int.TryParse(sCount, out iCount);
                    int.TryParse(sId, out iIndex);
                    //if (!int.TryParse(sCount, out iCount))
                    //    MessageBox.Show("Invalid count at row " + iRow.ToString() + ": " + sCount, "Not a valid integer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    var q = (from jmNext in _mdB.KrestniJmenas
                             where jmNext.CodePage == sNative
                             select jmNext).FirstOrDefault();

                    if (q != null)
                    {
                        ++iModified;
                        txtResponse.AppendText("Modifying existing entry \"" + sNative + "\" with count of " + sCount + Environment.NewLine);
                        Application.DoEvents();
                    }
                    else
                    {
                        if ((iCount > 1) && !sNative.Contains(' '))
                        {
                            ++iNew;
                            txtResponse.AppendText("Adding new entry \"" + sNative + "\"" + Environment.NewLine);
                            //var jmNew = new KrestniJmena();
                            //jmNew.CodePage = sNative;
                            //jmNew.Web = sWeb;
                            //jmNew.PlainText = sPlainText;
                            //mdB.KrestniJmenas.InsertOnSubmit(jmNew);
                            Application.DoEvents();
                        }
                    }
                }

                ++iRow;
            }

            txtResponse.AppendText(string.Format(">>> Totals: {0} rows, {1} records added, {2} records modified <<<{3}", iRow, iNew, iModified, Environment.NewLine));

            // fNames.SaveAs(System.IO.Path.Combine(Utilities.DataFilesFolder, "GivenNamesFemaleTransliterated.xls"));
            fNames.Close();
        }

        private List<IDeathRecord> _denniHlasatelMatches;

        /// <summary>
        /// Search the Denni Hlasatel death index database table
        /// for a match to the person identified in the user interface
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>This method takes its input solely from the user interface,
        /// via the text boxes <see cref="txtFirstName"/>, <see cref="txtLastName"/> and <see cref="txtDate"/></remarks>
        private void mnuDenniHlasatelSearch_Click(object sender, EventArgs e)
        {
            IDeathRecord drMatch = DeathRecord.Create(txtLastName.Text + ", " + txtFirstName.Text);
            drMatch.FirstName = txtFirstName.Text;
            drMatch.LastName = txtLastName.Text;
            drMatch.MiddleName = string.Empty;
            string sDate = txtDate.Text.Replace(',', ' ');
            DateTime filingDate, dtStart, dtEnd;
            int iYear;
            if (int.TryParse(sDate, out iYear))
            {
                drMatch.FilingDate = new DateTime(iYear, 6, 30, 0, 0, 0);
                dtStart = new DateTime(iYear, 1, 1, 0, 0, 0);
                dtEnd = new DateTime(iYear, 12, 31, 23, 59, 59);
            }
            else if (DateTime.TryParse("1 " + sDate, out filingDate))
            {
                drMatch.FilingDate = filingDate;
                dtStart = filingDate;
                iYear = filingDate.Year;
                int iMonth = filingDate.Month;
                switch (iMonth)
                {
                    case 1: case 3: case 5: case 7: case 8: case 10: case 12:
                        dtEnd = new DateTime(iYear, iMonth, 31, 23, 59, 59);
                        break;
                    default: // Applies to 4 (April), 6 (June), 9 (September) and 11 (November)
                        dtEnd = new DateTime(iYear, iMonth, 30, 23, 59, 59);
                        break;
                    case 2:
                        iYear = filingDate.Year;
                        if ((iYear % 4) != 0)
                            dtEnd = new DateTime(iYear, iMonth, 28, 23, 59, 59);
                        else if (((iYear % 100) != 0) || ((iYear % 400) == 0))
                            dtEnd = new DateTime(iYear, iMonth, 29, 23, 59, 59);
                        else
                            dtEnd = new DateTime(iYear, iMonth, 28, 23, 59, 59);
                        break;
                }
            }
            else if (DateTime.TryParse(sDate, out filingDate))
            {
                drMatch.FilingDate = filingDate;
                dtStart = filingDate;
                dtEnd = dtStart.AddSeconds( 86399.0 );
            }
            else 
            {
                drMatch.FilingDate = DateTime.Now;
                dtStart = DateTime.MinValue;
                dtEnd = DateTime.MaxValue;
            }
            _denniHlasatelMatches = DHDeathIndex.Matches(_mdB.DHDeathIndexes, drMatch, dtStart, dtEnd );
            DisplayRecords(_denniHlasatelMatches);
            if (!string.IsNullOrEmpty(Utilities.DataFilesFolder))
            {
                //System.IO.StreamWriter fOut = new System.IO.StreamWriter(System.IO.Path.Combine(Utilities.DataFilesFolder, @"DH files\results.txt"));
                //foreach (IDeathRecord drNext in _denniHlasatelMatches)
                //{
                //    fOut.WriteLine(drNext.FirstName + " " + drNext.LastName + ": " + drNext.FilingDate.ToLongDateString());
                //}
                //fOut.Close();

                DisplayDenniHlasatelResultsInBrowser( OnDhDocumentLoaded );
            }
        }

        #endregion

        private Uri _baseUri;

        /// <summary>
        /// Loads the empty Denni Hlasatal death index HTML file, and populates it with the results of a web query
        /// </summary>
        /// <param name="target">the action to be taken when the empty HTML file is completely loaded</param>
        /// <remarks>Denni Hlasatel queries can be processed by simply calling this method,
        /// passing it a web query event handler to be executed when the basic (empty) death index HTML file has been loaded.
        /// The web query event handler is then executed, and its results are inserted into the body of the basic HTML document.</remarks>
        private void DisplayDenniHlasatelResultsInBrowser( Action< object, WebBrowserDocumentCompletedEventArgs> target )
        {
            if (_baseUri == null)
            {
                string sUri = "file://" + System.IO.Path.Combine(Utilities.DataFilesFolder, @"DH files\Denni-Hlasatel-Obituary-Index.htm");
                _baseUri = new Uri(sUri);
            }
            if (webBrowser1.Url != _baseUri)
            {
                _onDocumentCompleted = target;
                webBrowser1.Url = _baseUri;
            }
            else
            {
                _onDocumentCompleted = null;
                target( webBrowser1, new WebBrowserDocumentCompletedEventArgs( _baseUri ) );
            }
        }

        /// <summary>
        /// Represents an alternate form (with or without diacriticals or -ova or similar endings)
        /// of a person's full name.  Usage rankings (expressed as <code>Count</code> properties)
        /// are included to provide a measure of relative frequency of usage.
        /// </summary>
        private class AlternateName
        {
            /// <summary>
            /// Construct an alternate form of a person's full name
            /// </summary>
            /// <param name="sGivenName">The person's first name</param>
            /// <param name="sSurname">The person's last (family) name</param>
            public AlternateName(string sGivenName, string sSurname) 
            {
                GivenName = sGivenName;
                Surname = sSurname;
                GivenNameCount = 0;
                SurnameCount = 0;
            }

            /// <summary>
            /// The person's first name
            /// </summary>
            public string GivenName { get; private set; }

            /// <summary>
            /// The person's last (family) name
            /// </summary>
            public string Surname { get; private set; }

            /// <summary>
            /// Number of occurrences of this person's first name in the given names database
            /// </summary>
            public int GivenNameCount { get; set; }

            /// <summary>
            /// Number of occurrences of this person's last (family) name in the surname database
            /// </summary>
            public int SurnameCount { get; set; }
        }

        /// <summary>
        /// Displays the results of a Denni Hlasatel death index search on a person's full name and death reporting date
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>j
        /// <remarks>This method is used to display the results of the <see cref="mnuDenniHlasatelSearch_Click"/> event handler</remarks>
        void OnDhDocumentLoaded(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string sInnerHtml = string.Empty, sLastFullName = string.Empty;

            foreach (IDeathRecord drNext in _denniHlasatelMatches)
            {
                string sCurrentFullName = drNext.FirstName + " " + drNext.LastName;
                if (sCurrentFullName != sLastFullName)
                {
                    sInnerHtml += "<h1>" + sCurrentFullName + "</h1>";
                    if (_mdB != null)
                    {
                        var lAlternateNames = new List<AlternateName>();
                        List<GivenName> lGivenNames = GivenName.MatchToPlainTextName(drNext.FirstName, _mdB);
                        Surname snPrimary = Surname.Get(drNext.LastName);
                        List<Surname.NativeForm> lSurnames = snPrimary.AlternateForms;
                        var qSurname = (from sn in _mdB.Prijmenis
                                        where sn.PlainText == drNext.LastName
                                        select sn.Web).ToList<string>();
                        if ((lGivenNames.Count > 0) || (qSurname.Count > 0))
                        {
                            if (lGivenNames.Count < 1)
                                lGivenNames.Add(GivenName.Get(drNext.FirstName));
                            if (qSurname.Count < 1)
                                qSurname.Add(drNext.LastName);

                            foreach (var gnNext in lGivenNames)
                            {
                                foreach (string sSurname in qSurname)
                                {
                                    var altNext = new AlternateName(gnNext.Value, sSurname);
                                    lAlternateNames.Add(altNext);
                                    altNext.GivenNameCount = gnNext.MaleCount + gnNext.FemaleCount;
                                }
                            }
                        }

                        if (lAlternateNames.Count > 0)
                        {
                            sInnerHtml += "<h2>Alternate forms of the name:</h2>";
                            foreach (AlternateName altNext in lAlternateNames)
                            {
                                string sNext = altNext.GivenName + " " + altNext.Surname;
                                if (altNext.GivenNameCount > 0)
                                    sNext += " (" + altNext.GivenNameCount + " individuals with this given name)";
                                sInnerHtml += sNext + "<br>";
                            }
                        }

                        if (lSurnames.Count > 1)
                        {
                            sInnerHtml = lSurnames.Aggregate("<h2>Alternate forms of the surname:</h2>", (current, nfNext) => current + (nfNext.Web + ", "));
                            sInnerHtml += "<br>";
                        }
                    }
                    sInnerHtml += "<h2>Records:</h2>";
                }

                sLastFullName = sCurrentFullName;
                sInnerHtml += "<a target=\"_blank\" href=\"" + IllinoisDeathIndexWebQuery.GetUrl(drNext) + "\">" + "#" + drNext.CertificateNumber + "</a>: " 
                // sInnerHtml += "<a href=\"" + IllinoisDeathIndexWebQuery.GetUrl(drNext) + "\">" + "#" + drNext.CertificateNumber.ToString() + "</a>: " 
                    + Utilities.CzechDate( drNext.FilingDate ) + " (" + drNext.FilingDate.ToLongDateString() + ")<br>";
            }

            // HtmlElement frame = webBrowser1.Document.GetElementsByTagName("iframe")[0];
            try
            {
                var frame = webBrowser1.Document.Window.Frames["Display_Area"];
                // frame.InnerHtml = sInnerHtml;
                // frame.InnerText = sInnerHtml;
                frame.Document.Body.InnerHtml = sInnerHtml;
            }
            catch (Exception ex )
            {
                txtResponse.AppendText(
                    "Exception when attempting to write to document frame.  The HTML to be displayed is:" + Environment.NewLine +
                    sInnerHtml + Environment.NewLine +
                    "The exception text is:" +
                    ex);
            }
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// <para>Event handler invoked when text is changed in <see cref="txtFirstName"/> text box.</para>
        /// Attempts to display possible matches to the (partial) first name in the text box
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            if ( _mdB != null )
                DisplayDenniHlasatelResultsInBrowser(OnFirstNameChanged);
        }

        /// <summary>
        /// Displays, in the basic Denni Hlasatel HTML document, the possible matches to the (partial)
        /// first name entered in the <see cref="txtFirstName"/> text box.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>This method requires a connection to the Denni Hlasatel database.
        /// Matches are only sought if three or more characters have been entered into the text box.</remarks>
        private void OnFirstNameChanged( object sender, WebBrowserDocumentCompletedEventArgs e )
        {
            string sInnerHtml = string.Empty;

            string sName = txtFirstName.Text;
            int iLen = sName.Length;

            if (iLen >= 3)
            {
                var query = (from c in _mdB.DHDeathIndexes
                             where c.GivenName.StartsWith(sName)
                             select c.GivenName).ToList().Distinct();

                foreach (string sNext in query)
                {
                    GivenName snNext = GivenName.Get(sNext);
                    sInnerHtml += sNext;
                    if (snNext.AlternateForms.Count > 0)
                    {
                        sInnerHtml += " (";
                        int iCount = 0;
                        foreach (GivenName.NativeForm sNextAlternate in snNext.AlternateForms)
                        {
                            if (iCount > 0)
                                sInnerHtml += ", ";
                            ++iCount;
                            if ( String.IsNullOrEmpty( sNextAlternate.Value ) )
                                sInnerHtml += sNextAlternate.Web;
                            else
                                sInnerHtml += "<a href=\"http://www.kdejsme.cz/jmeno/" +
                                    WebChar.ToUrlEncoding(sNextAlternate.Value) + "/hustota/\" target=\"_blank\" >" + sNextAlternate.Web + "</a>";
                        }
                        sInnerHtml += ")";
                    }
                    sInnerHtml += "<br>";
                }
            }
            else if (iLen < 3)
            {
                sInnerHtml = "<h2>Not enough <a href=\"dhdi://localhost/SurnameSearch?Surname=Vlcek&Gender=Male\">characters</a> to get a given name</h2>";
            }

            // It's lousy to use try/catch in this way, but the alternative is really stupidly painful
            // (particularly given that there's no way to test if Frame "Display_Area" exists or not...
            try
            {
                HtmlWindow frame = webBrowser1.Document.Window.Frames["Display_Area"];
                if ((frame.Document != null) && (frame.Document.Body != null))
                    frame.Document.Body.InnerHtml = sInnerHtml;
            }
            catch (Exception ex)
            {
                txtResponse.AppendText(
                    "Exception when attempting to write to document frame.  The HTML to be displayed is:" + Environment.NewLine +
                    sInnerHtml + Environment.NewLine +
                    "The exception text is:" +
                    ex);
            }
        }

        /// <summary>
        /// Queries the KdeJsme online database for a possible match to the surname entered into the user interface
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>The contents of the <see cref="txtLastName"/> text box are submitted to the web query.</remarks>
        private void kdeJsmeSurnameSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var kjQ = new KdeJsmeWebQuery(txtLastName.Text, KdeJsmeWebQuery.NameComponent.Surname, webBrowser1);
            MessageBox.Show(kjQ.Url, "URL of Kde Jsme search", MessageBoxButtons.OK, MessageBoxIcon.Information);
            kjQ.Submit();
        }

        /// <summary>
        /// Cached value of the "next document completed event handler".
        /// This member is used when sequentially processing web queries that "stack"
        /// (one query triggering another, etc.)
        /// </summary>
        private Action<object, WebBrowserDocumentCompletedEventArgs> _onDocumentCompleted = null;

        /// <summary>
        /// <para>Directly handles the <see cref="WebBrowser.DocumentCompleted"/> event of the web browser.</para>
        /// This event handler forwards the event to the "next document completed event handler"
        /// (stored in the member variable <see cref="_onDocumentCompleted"/>, if that member is not null.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if ( _onDocumentCompleted != null )
                _onDocumentCompleted(sender, e);
            _onDocumentCompleted = null;
        }

        private SearchForm _fGivenNameSearchForm = new SearchForm();

        /// <summary>
        /// Initiates a search into the Denni Hlasatel death index
        /// for given names matching a pattern specified in a search dialog.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>This method makes the search dialog visible.
        /// User interaction with the search dialog causes the actual results to be displayed.</remarks>
        private void searchGivenNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_mGivenNamesCache.Count < 1)
            {
                var query = (from c in _mdB.DHDeathIndexes
                             select c.GivenName).ToList<string>().Distinct();
                _mGivenNamesCache.AddRange(query);
            }

            _fGivenNameSearchForm.Search(_mGivenNamesCache, txtResponse);
        }

        private SearchForm _fSurnameSearchForm = new SearchForm();

        /// <summary>
        /// Initiates a search into the Denni Hlasatel death index
        /// for surnames matching a pattern specified in a search dialog.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        /// <remarks>This method makes the search dialog visible.
        /// User interaction with the search dialog causes the actual results to be displayed.</remarks>
        private void searchSurnamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_mSurnamesCache.Count < 1)
            {
                var query = (from c in _mdB.DHDeathIndexes
                             select c.Surname).ToList<string>().Distinct();
                _mSurnamesCache.AddRange(query);
            }

            _fSurnameSearchForm.Search(_mSurnamesCache, txtResponse);
        }

        private void convertToJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var drSample = DeathRecord.Create("GrandfathersDeath");
            drSample.AgeInYears = 30;
            drSample.CertificateNumber = 12345;
            drSample.City = "Chicago";
            drSample.County = "Cook";
            drSample.FilingDate = new DateTime(1918, 4, 28, 12, 0, 0);
            drSample.FirstName = "Frantisek";
            drSample.LastName = "Vlcek";
            drSample.PageNumber = 123;
            drSample.Volume = "A";
            drSample.Gender = CustomTypes.Gender.Male;
            drSample.DeathDate = new DateTime(1918, 4, 26, 12, 0, 0);

            var oChild = new SimpleObject("Cross-Reference", "Reference");
            oChild.AddProperty("SampleProperty", "SampleValue");
            drSample.AddChild(oChild);

            var txtOut = new System.IO.StreamWriter(@"C:\users\james\FrantisekVlcek.json");
            var jsFmt = new JsonSaveFormat();
            jsFmt.Stream(txtOut, drSample);
            txtOut.Close();
        }

        private void unrecognizedGivenNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Move code here (from "list given names" menu event handler) to generate list of unmatched given names
        }
    }
}
