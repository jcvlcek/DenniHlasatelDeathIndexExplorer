using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Linq;
using dbAccess;

namespace Genealogy
{
    /// <summary>
    /// Main form for the Denni Hlasatel / Illinois Death Index Explorer program
    /// </summary>
    public partial class Form1 : Form
    {
        #region Private members
        private List<string> mGivenNamesCache = new List<string>();
        private List<string> mSurnamesCache = new List<string>();
        private static string[] MonthAbbreviations = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        System.Data.SqlClient.SqlConnection mdbConnection;
        TreeViewWrapper mTreeView;
        Linq2SqlDataContext mdB;
        List<IllinoisDeathIndexWebQuery> mPendingWebQueries = new List<IllinoisDeathIndexWebQuery>();
        #endregion

        #region Constructors

        public Form1()
        {
            InitializeComponent();
            mdbConnection = new System.Data.SqlClient.SqlConnection();
            mTreeView = new TreeViewWrapper(tvDocument);
            webBrowser1.DocumentText = "<HTML></HTML>";
        }

        #endregion

        #region Private methods

        private string FormalDay(string sInformalDay)
        {
            if (sInformalDay.Length < 2)
                return "0" + sInformalDay;
            else
                return sInformalDay;
        }

        static List<string> lMonths = new List<string>(new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" });
        private string FormalMonth(string sInformalMonth)
        {
            int iMonth = lMonths.IndexOf(sInformalMonth.ToUpper()) + 1;
            if (iMonth < 10)
                return "0" + iMonth.ToString();
            else
                return iMonth.ToString();
        }

        private void ChangeBindingSource(BindingSource bsTarg)
        {
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.AutoGenerateColumns = true;
            this.dataGridView1.Enabled = false;
            this.dataGridView1.Invalidate();
            this.dataGridView1.Enabled = true;
            dataGridView1.DataSource = bsTarg;
            this.dataGridView1.Refresh();
        }

        private void OnWebQueryCompleted(object sender, WebQueryEventArgs e)
        {
            HtmlDocument docResponse = webBrowser1.Document;
            mTreeView.DisplayInTree(docResponse);
            txtResponse.Text = webBrowser1.DocumentText;
            DateTime dtTarg = DateTime.MinValue;
            bool bAnyDate = true;

            if ( DateTime.TryParse(txtDate.Text.Replace(',', ' '), out dtTarg) )
                bAnyDate = false;

            // Display the results list contents in both the tree and list views
            DisplayRecords(e.Results, false);
            foreach (IDeathRecord recNext in e.Results)
            {
                if ( recNext is IObject )
                    mTreeView.DisplayInTree((IObject)recNext);
            }

            foreach (ListViewItem itmNext in lvHits.Items)
            {
                IDeathRecord recNext = (IDeathRecord)itmNext.Tag;
                if (bAnyDate)
                    itmNext.ImageKey = "Match";
                else if ((dtTarg >= recNext.DeathDate) &&
                    (dtTarg <= recNext.DeathDate.AddDays(4.0)))
                    itmNext.ImageKey = "Match";
                else
                    itmNext.ImageKey = "NoMatch";
            }

            if (mPendingWebQueries.Count > 0)
            {
                IllinoisDeathIndexWebQuery qNext = mPendingWebQueries[0];
                mPendingWebQueries.RemoveAt(0);
                qNext.QueryCompleted += new EventHandler<WebQueryEventArgs>(OnWebQueryCompleted);
                qNext.Submit();
            }
        }

        #endregion

        #region Event handlers

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // TODO: This line of code loads data into the 'genealogyDataSet.KrestniJmena' table. You can move, or remove it, as needed.
                this.krestniJmenaTableAdapter.Fill(this.genealogyDataSet.KrestniJmena);
                // TODO: This line of code loads data into the 'genealogyDataSet.Prijmeni' table. You can move, or remove it, as needed.
                this.prijmeniTableAdapter.Fill(this.genealogyDataSet.Prijmeni);
                // TODO: This line of code loads data into the 'genealogyDataSet.GivenNameEquivalents' table. You can move, or remove it, as needed.
                this.givenNameEquivalentsTableAdapter.Fill(this.genealogyDataSet.GivenNameEquivalents);
                // TODO: This line of code loads data into the 'genealogyDataSet.DHDeathIndex' table. You can move, or remove it, as needed.
                this.dHDeathIndexTableAdapter.Fill(this.genealogyDataSet.DHDeathIndex);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show( "Unable to load/fill database table adapters:" + Environment.NewLine + ex.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            mdB = new Linq2SqlDataContext();
            Utilities.dB = mdB;

            try
            {
                var query = from c in mdB.DHDeathIndexes
                            select c.GivenName;

                foreach (string sNext in query)
                {
                    if (!txtFirstName.AutoCompleteCustomSource.Contains(sNext))
                        txtFirstName.AutoCompleteCustomSource.Add(sNext);
                }
            }
            catch (System.Data.SqlClient.SqlException)
            {
                // TODO: Figure out what (if anything) to do here...  We don't want to rethrow, that's for sure!
                // throw;
            }
            webBrowser1.Navigating += new WebBrowserNavigatingEventHandler(webBrowser1_Navigating);
        }

        void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Scheme == "dhdi")
            {
                string sFunction = e.Url.AbsolutePath;
                string sQuery = e.Url.IsAbsoluteUri ? e.Url.Query : string.Empty;

                System.Collections.Specialized.NameValueCollection lQuery =
                    System.Web.HttpUtility.ParseQueryString(sQuery);

                DhdiScheme.NavigateTo(webBrowser1.Document.Window.Frames["Display_Area"].Document.Body, sFunction, lQuery);
                e.Cancel = true;
            }
        }

        private void lvHits_DoubleClick(object sender, EventArgs e)
        {
            DateTime dtWhen = DateTime.Parse( txtDate.Text.Replace( ',', ' ' ));

            var query = from c in mdB.DHDeathIndexes
                        where ( c.GivenName == txtFirstName.Text ) &&
                            ( c.Surname == txtLastName.Text ) &&
                            ( c.ReportDate.Year == dtWhen.Year ) &&
                            ( c.ReportDate.Month == dtWhen.Month ) &&
                            ( c.ReportDate.Day == dtWhen.Day )
                        select c;

            foreach ( var q in query )
            {
                int iDhSerial = q.serial;

                foreach (ListViewItem itmNext in lvHits.SelectedItems)
                {
                    IDeathRecord drTarg = (IDeathRecord)itmNext.Tag;

                    if (drTarg.DeathDate.Year > 1915)
                    {
                        IllinoisDeathIndexPost1915 indxNew = IllinoisDeathIndex.CreatePost1915(drTarg);
                        mdB.IllinoisDeathIndexPost1915s.InsertOnSubmit(indxNew);
                        try
                        {
                            mdB.SubmitChanges();
                            MessageBox.Show("Added to Illinois post-1915 db as serial #" + indxNew.Serial.ToString(), "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Failed to add to post-1915 db", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        IllinoisDeathIndexPre1916 indxNew = IllinoisDeathIndex.CreatePre1916(drTarg);
                        mdB.IllinoisDeathIndexPre1916s.InsertOnSubmit(indxNew);
                        try
                        {
                            mdB.SubmitChanges();
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

        private void denniHlasatelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBindingSource(dHDeathIndexBindingSource);
        }

        private void givenNameEquivalentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBindingSource(givenNameEquivalentsBindingSource);
        }

        private void prijmeniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBindingSource(prijmeniBindingSource);
        }

        private void krestniJmenaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeBindingSource(krestniJmenaBindingSource);
        }

        private void framesDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FamilySearchWebQuery.WriteOutFrame(webBrowser1);
        }

        #endregion

        private void illinoisDeathIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mPendingWebQueries.Clear();
            txtResponse.Clear();
            ClearRecords();

            IDeathRecord drTarg = DenniHlasatelDataStore.CreateFromRecord(txtFirstName.Text + "," + txtLastName.Text + "," + txtDate.Text);
            GivenName gnTarg = GivenName.Get(txtFirstName.Text);
            String lAlternateGivenNameSpelling = gnTarg.EquivalentNativeForm;
            List<string> lEquivalentForms = gnTarg.EquivalentForms;
            List<string> lAlternateGivenNames = lEquivalentForms.GetRange(1, lEquivalentForms.Count - 1);

            if (lAlternateGivenNames.Count > 0)
            {
                string sAlternateForms = txtFirstName.Text + " "; ;
                if (lAlternateGivenNameSpelling.Length > 0)
                {
                    sAlternateForms += "(" + lAlternateGivenNameSpelling + ") ";
                }
                sAlternateForms += txtLastName.Text + Environment.NewLine;

                foreach (string sNext in lAlternateGivenNames)
                {
                    sAlternateForms += sNext + " " + txtLastName.Text + Environment.NewLine;
                }

                MessageBox.Show("All possible forms of this name:" + Environment.NewLine + sAlternateForms, "Alternate names possible", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (lAlternateGivenNames.Count > 0)
            {
                foreach (string sNextName in lAlternateGivenNames)
	            {
                    IDeathRecord drNext = DenniHlasatelDataStore.CreateFromRecord(sNextName + "," + txtLastName.Text + "," + txtDate.Text);
                    IllinoisDeathIndexWebQuery qNext = new IllinoisDeathIndexWebQuery( drNext, webBrowser1 );
                    mPendingWebQueries.Add(qNext);
                    if (drTarg.FilingDate.Year < 1700)
                    {
                        drNext = DenniHlasatelDataStore.CreateFromRecord(sNextName + "," + txtLastName.Text + "," + "1,Jan,1916");
                        mPendingWebQueries.Add(new IllinoisDeathIndexWebQuery(drNext, webBrowser1));
                    }
	            }
            }

            IllinoisDeathIndexWebQuery qNew = new IllinoisDeathIndexWebQuery(drTarg, webBrowser1);
            if (drTarg.FilingDate.Year < 1700)
            {
                drTarg = DenniHlasatelDataStore.CreateFromRecord(txtFirstName.Text + "," + txtLastName.Text + "," + "1,Jan,1916");
                mPendingWebQueries.Add(new IllinoisDeathIndexWebQuery(drTarg, webBrowser1));
            }
            qNew.QueryCompleted += new EventHandler<WebQueryEventArgs>(OnWebQueryCompleted);
            qNew.Submit();
        }

        private void familySearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtResponse.Clear();

            // IDeathRecord drTarg = DenniHlasatelDataStore.CreateFromRecord(txtFirstName.Text + "," + txtLastName.Text + "," + txtDate.Text);
            IDeathRecord drTarg = DenniHlasatelDataStore.CreateFromNameAndDate(txtFirstName.Text, txtLastName.Text, txtDate.Text);
            FamilySearchWebQuery qNew = new FamilySearchWebQuery(drTarg, webBrowser1);
            qNew.QueryCompleted += new EventHandler<WebQueryEventArgs>(OnWebQueryCompleted);
            qNew.Submit();
        }

        private void convertDHFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CombineDhFilesIntoXml();
        }

        private void prijmeniConversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sPrijmeniPath;

            if (DialogResult.OK != Utilities.CheckForDataFile("prijmeni.xls", out sPrijmeniPath))
                return;
            ExcelFile fNames = new ExcelFile(sPrijmeniPath);
            ExcelFile fNamesOut = new ExcelFile();

            System.Drawing.Point pTarg = new System.Drawing.Point(5, 1);
            System.Drawing.Point pHtml = new System.Drawing.Point(4, 1);
            int iBlanks, iNames, iRow;
            for (iBlanks = 0, iNames = 0, iRow = 2; iBlanks < 5; ++iRow)
            {
                string sValue = string.Empty;
                string sHtml = string.Empty;

                foreach (int iNext in new int[] { 1, 2, 3, 4, 5, 6 })
                {
                    System.Drawing.Point pNext = new System.Drawing.Point(iNext, iRow);
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
                    txtFirstName.Text = iRow.ToString();
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

            MessageBox.Show(iNames.ToString() + " names found in spreadsheet", "Prijmeni scan", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fNames.Close();
            fNamesOut.SaveAs(System.IO.Path.Combine(Utilities.DataFilesFolder, "prijmeni lowercased.xls"));
            fNamesOut.Close();
        }

        private void nextDHRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IDeathRecord drNew = DenniHlasatelDataStore.Next();
                txtFirstName.Text = drNew.FirstName;
                txtLastName.Text = drNew.LastName;
                txtDate.Text = drNew.FilingDate.Day.ToString() + "," + MonthAbbreviations[drNew.FilingDate.Month - 1] + "," + drNew.FilingDate.Year.ToString();
                DisplayRecord(drNew);
                MessageBox.Show(drNew.FirstName + " " + drNew.LastName + "'s death notice was published" + Environment.NewLine
                    + drNew.FilingDate.ToLongDateString(), "Denni Hlasatel record", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message + Environment.NewLine + ex.ToString(), "Unable to find next Denni Hlasatel death record", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void prijmeniRankCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sPrijmeniPath;

            if (DialogResult.OK != Utilities.CheckForDataFile("prijmeni rank-sorted.xls", out sPrijmeniPath))
                return;
            ExcelFile fNames = new ExcelFile(sPrijmeniPath);

            int iBlanks, iNames, iRow, iPreviousRank = -1;
            string sPreviousName = string.Empty;
            List<int> lRanks = new List<int>();

            for (iBlanks = 0, iNames = 0, iRow = 2; iBlanks < 5; ++iRow)
            {
                System.Drawing.Point pTarg = new System.Drawing.Point(1, iRow);
                System.Drawing.Point pName = new System.Drawing.Point(5, iRow);
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

        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            if (mdB != null)
                DisplayDenniHlasatelResultsInBrowser(OnLastNameChanged);
        }

        private void OnLastNameChanged(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string sInnerHtml = string.Empty;

            string sName = txtLastName.Text;
            int iLen = sName.Length;

            if (iLen >= 3)
            {
                var query = (from c in mdB.DHDeathIndexes
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

            // webBrowser1.Document.Body.InnerHtml = sInnerHtml;
            HtmlWindow frame = webBrowser1.Document.Window.Frames["Display_Area"];
            frame.Document.Body.InnerHtml = sInnerHtml;
        }

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

        private void listGivenNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var query = (from c in mdB.DHDeathIndexes
            //             select c.GivenName).ToList<string>().Distinct<string>();
            List<string> lAllGivenNames = new List<string>();

            var q1 = from tK in mdB.KrestniJmenas
                     select tK.PlainText;

            foreach (string sNext in q1 )
                if ( !lAllGivenNames.Contains( sNext ) )
                    lAllGivenNames.Add( sNext );

            var q2 = from tG in mdB.GivenNameEquivalents
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
            var dhNames = (from t1 in mdB.DHDeathIndexes
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

                var oddNames = from d1 in mdB.DHDeathIndexes
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

        private void listSurnamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This query generate a list of all surnames in Denni Hlasatel
            //var query = (from c in mdB.DHDeathIndexes
            //             select c.Surname).ToList<string>().Distinct<string>();

            // Example of join that finds records in B not occurring in A
            var query = (from t1 in mdB.DHDeathIndexes
                        join t2 in mdB.Prijmenis
                        on t1.Surname equals t2.PlainText into tg
                        from tcheck in tg.DefaultIfEmpty()
                        where tcheck.PlainText == null
                        select t1.Surname).ToList<string>().Distinct<string>();
            
            ListStrings(query);
        }

        private void givenNamesTranslitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sGivenNames;

            if (DialogResult.OK != Utilities.CheckForDataFile("GivenNamesMaleTransliterated.xls", out sGivenNames))
                return;
            ExcelFile fNames = new ExcelFile(sGivenNames);

            int iRow = 2, iBlankRows = 0, iModified = 0, iNew = 0;

            txtResponse.Clear();
            while (iBlankRows < 1)
            {
                string sId = fNames.ValueAt(new System.Drawing.Point(1, iRow)).Trim();
                String sNative = fNames.ValueAt(new System.Drawing.Point(2, iRow)).Trim();
                string sWeb = fNames.ValueAt(new System.Drawing.Point(3, iRow)).Trim();
                string sPlainText = fNames.ValueAt(new System.Drawing.Point(4, iRow)).Trim();
                string sCount = fNames.ValueAt(new System.Drawing.Point(5, iRow)).Trim();

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
                    int iCount = 0, iIndex = -1;

                    int.TryParse(sCount, out iCount);
                    int.TryParse(sId, out iIndex);
                    //if (!int.TryParse(sCount, out iCount))
                    //    MessageBox.Show("Invalid count at row " + iRow.ToString() + ": " + sCount, "Not a valid integer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    var q = (from jmNext in mdB.KrestniJmenas
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
                            KrestniJmena jmNew = new KrestniJmena();
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

            txtResponse.AppendText(">>> Totals: " + iRow.ToString() +
                " rows, " + iNew.ToString() + " records added, " +
                iModified.ToString() + " records modified <<<" + Environment.NewLine);

            // fNames.SaveAs(System.IO.Path.Combine(Utilities.DataFilesFolder, "GivenNamesFemaleTransliterated.xls"));
            fNames.Close();
        }

        private List<IDeathRecord> mDhMatches;

        private void mnuDenniHlasatelSearch_Click(object sender, EventArgs e)
        {
            IDeathRecord drMatch = DeathRecord.Create(txtLastName.Text + ", " + txtFirstName.Text);
            drMatch.FirstName = txtFirstName.Text;
            drMatch.LastName = txtLastName.Text;
            drMatch.MiddleName = string.Empty;
            string sDate = txtDate.Text.Replace(',', ' ');
            DateTime FilingDate, dtStart, dtEnd;
            int iYear;
            if (int.TryParse(sDate, out iYear))
            {
                drMatch.FilingDate = new DateTime(iYear, 6, 30, 0, 0, 0);
                dtStart = new DateTime(iYear, 1, 1, 0, 0, 0);
                dtEnd = new DateTime(iYear, 12, 31, 23, 59, 59);
            }
            else if (DateTime.TryParse("1 " + sDate, out FilingDate))
            {
                drMatch.FilingDate = FilingDate;
                dtStart = FilingDate;
                iYear = FilingDate.Year;
                int iMonth = FilingDate.Month;
                switch (iMonth)
                {
                    case 1: case 3: case 5: case 7: case 8: case 10: case 12:
                        dtEnd = new DateTime(iYear, iMonth, 31, 23, 59, 59);
                        break;
                    case 4: case 6: case 9: case 11: default:
                        dtEnd = new DateTime(iYear, iMonth, 30, 23, 59, 59);
                        break;
                    case 2:
                        iYear = FilingDate.Year;
                        if ((iYear % 4) != 0)
                            dtEnd = new DateTime(iYear, iMonth, 28, 23, 59, 59);
                        else if (((iYear % 100) != 0) || ((iYear % 400) == 0))
                            dtEnd = new DateTime(iYear, iMonth, 29, 23, 59, 59);
                        else
                            dtEnd = new DateTime(iYear, iMonth, 28, 23, 59, 59);
                        break;
                }
            }
            else if (DateTime.TryParse(sDate, out FilingDate))
            {
                drMatch.FilingDate = FilingDate;
                dtStart = FilingDate;
                dtEnd = dtStart.AddSeconds( 86399.0 );
            }
            else 
            {
                drMatch.FilingDate = DateTime.Now;
                dtStart = DateTime.MinValue;
                dtEnd = DateTime.MaxValue;
            }
            mDhMatches = DHDeathIndex.Matches(mdB.DHDeathIndexes, drMatch, dtStart, dtEnd );
            DisplayRecords(mDhMatches);
            if (!string.IsNullOrEmpty(Utilities.DataFilesFolder))
            {
                //System.IO.StreamWriter fOut = new System.IO.StreamWriter(System.IO.Path.Combine(Utilities.DataFilesFolder, @"DH files\results.txt"));
                //foreach (IDeathRecord drNext in mDhMatches)
                //{
                //    fOut.WriteLine(drNext.FirstName + " " + drNext.LastName + ": " + drNext.FilingDate.ToLongDateString());
                //}
                //fOut.Close();

                DisplayDenniHlasatelResultsInBrowser( OnDhDocumentLoaded );
            }
        }

        private Uri mBaseUri = null;

        private void DisplayDenniHlasatelResultsInBrowser( Action< object, WebBrowserDocumentCompletedEventArgs> target )
        {
            if (mBaseUri == null)
            {
                string sUri = "file://" + System.IO.Path.Combine(Utilities.DataFilesFolder, @"DH files\Denni-Hlasatel-Obituary-Index.htm");
                mBaseUri = new Uri(sUri);
            }
            if (webBrowser1.Url != mBaseUri)
            {
                mDocumentCompleted = target;
                webBrowser1.Url = mBaseUri;
            }
            else
            {
                mDocumentCompleted = null;
                target( webBrowser1, new WebBrowserDocumentCompletedEventArgs( mBaseUri ) );
            }
        }

        private class AlternateName
        {
            public AlternateName(string sGivenName, string sSurname) 
            {
                GivenName = sGivenName;
                Surname = sSurname;
                GivenNameCount = 0;
                SurnameCount = 0;
            }
            public string GivenName { get; private set; }
            public string Surname { get; private set; }
            public int GivenNameCount { get; set; }
            public int SurnameCount { get; set; }
        }

        void OnDhDocumentLoaded(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string sInnerHtml = string.Empty, sLastFullName = string.Empty, sCurrentFullName;

            foreach (IDeathRecord drNext in mDhMatches)
            {
                sCurrentFullName = drNext.FirstName + " " + drNext.LastName;
                if (sCurrentFullName != sLastFullName)
                {
                    sInnerHtml += "<h1>" + sCurrentFullName + "</h1>";
                    if (mdB != null)
                    {
                        List<AlternateName> lAlternateNames = new List<AlternateName>();
                        List<GivenName> lGivenNames = GivenName.MatchToPlainTextName(drNext.FirstName, mdB);
                        Surname snPrimary = Surname.Get(drNext.LastName);
                        List<Surname.NativeForm> lSurnames = snPrimary.AlternateForms;
                        var qSurname = (from sn in mdB.Prijmenis
                                        where sn.PlainText == drNext.LastName
                                        select sn.Web).ToList<string>();
                        if ((lGivenNames.Count > 0) || (qSurname.Count > 0))
                        {
                            if (lGivenNames.Count < 1)
                                lGivenNames.Add(GivenName.Get(drNext.FirstName));
                            if (qSurname.Count < 1)
                                qSurname.Add(drNext.LastName);

                            foreach (GivenName gnNext in lGivenNames)
                            {
                                foreach (string sSurname in qSurname)
                                {
                                    AlternateName altNext = new AlternateName(gnNext.Value, sSurname);
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
                                    sNext += " (" + altNext.GivenNameCount.ToString() + " individuals with this given name)";
                                sInnerHtml += sNext + "<br>";
                            }
                        }

                        if (lSurnames.Count > 1)
                        {
                            sInnerHtml += "<h2>Alternate forms of the surname:</h2>";
                            foreach (Surname.NativeForm nfNext in lSurnames)
                            {
                                sInnerHtml += nfNext.Web + ", ";
                            }
                            sInnerHtml += "<br>";
                        }
                    }
                    sInnerHtml += "<h2>Records:</h2>";
                }

                sLastFullName = sCurrentFullName;
                sInnerHtml += "<a target=\"_blank\" href=\"" + IllinoisDeathIndexWebQuery.GetUrl(drNext) + "\">" + "#" + drNext.CertificateNumber.ToString() + "</a>: " 
                // sInnerHtml += "<a href=\"" + IllinoisDeathIndexWebQuery.GetUrl(drNext) + "\">" + "#" + drNext.CertificateNumber.ToString() + "</a>: " 
                    + Utilities.CzechDate( drNext.FilingDate ) + " (" + drNext.FilingDate.ToLongDateString() + ")<br>";
            }

            // HtmlElement frame = webBrowser1.Document.GetElementsByTagName("iframe")[0];
            HtmlWindow frame = webBrowser1.Document.Window.Frames[ "Display_Area" ];
            // frame.InnerHtml = sInnerHtml;
            // frame.InnerText = sInnerHtml;
            frame.Document.Body.InnerHtml = sInnerHtml;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            if ( mdB != null )
                DisplayDenniHlasatelResultsInBrowser(OnFirstNameChanged);
        }

        private void OnFirstNameChanged( object sender, WebBrowserDocumentCompletedEventArgs e )
        {
            string sInnerHtml = string.Empty;

            string sName = txtFirstName.Text;
            int iLen = sName.Length;

            if (iLen >= 3)
            {
                var query = (from c in mdB.DHDeathIndexes
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

            // webBrowser1.Document.Body.InnerHtml = sInnerHtml;
            HtmlWindow frame = webBrowser1.Document.Window.Frames["Display_Area"];
            frame.Document.Body.InnerHtml = sInnerHtml;
        }

        private void kdeJsmeSurnameSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KdeJsmeWebQuery kjQ = new KdeJsmeWebQuery(txtLastName.Text, KdeJsmeWebQuery.eWhichName.SURNAME, webBrowser1);
            MessageBox.Show(kjQ.Url, "URL of Kde Jsme search", MessageBoxButtons.OK, MessageBoxIcon.Information);
            kjQ.Submit();
        }

        private Action<object, WebBrowserDocumentCompletedEventArgs> mDocumentCompleted = null;

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if ( mDocumentCompleted != null )
                mDocumentCompleted(sender, e);
            mDocumentCompleted = null;
        }

        private frmSearch fGivenNameSearch = new frmSearch();

        private void searchGivenNamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mGivenNamesCache.Count < 1)
            {
                var query = (from c in mdB.DHDeathIndexes
                             select c.GivenName).ToList<string>().Distinct();
                mGivenNamesCache.AddRange(query);
            }

            fGivenNameSearch.Search(mGivenNamesCache, txtResponse);
        }

        private frmSearch fSurnameSearch = new frmSearch();

        private void searchSurnamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSurnamesCache.Count < 1)
            {
                var query = (from c in mdB.DHDeathIndexes
                             select c.Surname).ToList<string>().Distinct();
                mSurnamesCache.AddRange(query);
            }

            fSurnameSearch.Search(mSurnamesCache, txtResponse);
        }
    }
}
