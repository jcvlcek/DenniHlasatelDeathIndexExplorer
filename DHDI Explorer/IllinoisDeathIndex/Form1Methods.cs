using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using dbAccess;

namespace Genealogy
{
    public partial class Form1 : Form
    {
        #region Event handlers

        /// <summary>
        /// Scans through a spreadsheet of given (first) names and looks for
        /// invalid instance counts and non-printable characters
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void givenNamesCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sGivenNames;

            if (DialogResult.OK != Utilities.CheckForDataFile("GivenNamesMaleTransliterated-2.xls", out sGivenNames))
                return;
            var fNames = new ExcelFile(sGivenNames);
            var lSpecialChars = new List<char>();
            var lBogusCounts = new List<int>();

            txtResponse.Clear();
            int iBlanks, iNames, iRow;
            for (iBlanks = 0, iNames = 0, iRow = 2; iBlanks < 5; ++iRow)
            {
                // string sId = fNames.ValueAt( new Point( 1, iRow ) ).Trim();
                string sNative = fNames.ValueAt(new Point(2, iRow)).Trim();
                string sHtml = fNames.ValueAt(new Point(3, iRow)).Trim();
                string sPlainText = fNames.ValueAt(new Point(4, iRow)).Trim();
                string sCount = fNames.ValueAt(new Point(5, iRow)).Trim();

                if ((iRow % 1000) == 0)
                {
                    txtResponse.AppendText("Row " + iRow + ": Name \"" + sNative + "\"" + Environment.NewLine);
                    Application.DoEvents();
                }

                if (string.IsNullOrEmpty(sPlainText))
                    ++iBlanks;
                else
                {
                    int iCount;
                    if (!int.TryParse(sCount, out iCount))
                    {
                        txtResponse.AppendText("Invalid count \"" + sCount + "\" found at row " + iRow + Environment.NewLine);
                        lBogusCounts.Add(iRow);
                    }

                    bool bFoundSpecialChar = false;
                    foreach (char cNext in sPlainText + sHtml)
                    {
                        if ((cNext < 0x20) || (cNext > 127))
                        {
                            bFoundSpecialChar = true;
                            if (!lSpecialChars.Contains(cNext))
                                lSpecialChars.Add(cNext);
                        }
                    }
                    if (bFoundSpecialChar)
                    {
                        txtResponse.AppendText("Special char at row " + iRow + ": Web=\"" + sHtml + "\", PlainText=\"" + sPlainText + "\"" + Environment.NewLine);
                        //txtResponse.AppendText("Web=\"" + sHtml + "\", PlainText=\"" + sPlainText + "\" -> ");
                        //CharEquivalents.Transliterate(sNative, out sHtml, out sPlainText);
                        //fNames.SetValueAt(new Point(3, iRow), sHtml);
                        //fNames.SetValueAt(new Point(4, iRow), sPlainText);
                        //txtResponse.AppendText("Web=\"" + sHtml + "\", PlainText=\"" + sPlainText + "\"" + Environment.NewLine);
                    }
                    //foreach (char cNext in sPlainText.ToCharArray())
                    //{
                    //    if ((cNext < 0x20) || (cNext > 127))
                    //    {
                    //        if (!lSpecialChars.Contains(cNext))
                    //            lSpecialChars.Add(cNext);
                    //        txtResponse.AppendText("Name with special character (" + sPlainText + ")" +
                    //            "found in plain text string at row " + iRow.ToString() + Environment.NewLine);
                    //    }
                    //}
                    //foreach (char cNext in sHtml.ToCharArray())
                    //{
                    //    if ((cNext < 0x20) || (cNext > 127))
                    //    {
                    //        if (!lSpecialChars.Contains(cNext))
                    //            lSpecialChars.Add(cNext);
                    //        txtResponse.AppendText("Name with special character (" + sHtml + ")" +
                    //            "found in HTML string at row " + iRow.ToString() + Environment.NewLine);
                    //    }
                    //}
                    ++iNames;
                    iBlanks = 0;
                }
            }

            txtResponse.AppendText(iNames + " names found in spreadsheet" + Environment.NewLine);
            foreach (char cNext in lSpecialChars)
            {
                txtResponse.AppendText("new TransChar( \"" + cNext + "\", \"&#" + ((int)cNext) + ";\", \"\" )," + Environment.NewLine);
            }
            foreach (int iNextRow in lBogusCounts)
            {
                txtResponse.AppendText("Bogus count at row " + iNextRow + Environment.NewLine);
            }
            // fNames.SaveAs(System.IO.Path.Combine(Utilities.DataFilesFolder, "GivenNamesFemaleTransliterated-2.xls"));
            fNames.Close();
        }

        /// <summary>
        /// Reads in a spreadsheet containing given (first) names
        /// and adds the names to the given names database table if they don't already exist there.
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void givenNamesMergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sGivenNames;
            string sGender = "Female"; // "Male"

            if (DialogResult.OK != Utilities.CheckForDataFile("GivenNames" + sGender + "Transliterated-2.xls", out sGivenNames))
                return;
            var fNames = new ExcelFile(sGivenNames);

            txtResponse.Clear();
            int iBlanks, iNames, iNew, iModified, iRow;
            for (iBlanks = 0, iNames = 0, iNew = 0, iModified = 0, iRow = 2; iBlanks < 5; ++iRow)
            {
                string sId = fNames.ValueAt(new Point(1, iRow)).Trim();
                string sNative = fNames.ValueAt(new Point(2, iRow)).Trim();
                string sHtml = fNames.ValueAt(new Point(3, iRow)).Trim();
                string sPlainText = fNames.ValueAt(new Point(4, iRow)).Trim();
                string sCount = fNames.ValueAt(new Point(5, iRow)).Trim();

                if ((iRow % 1000) == 0)
                {
                    txtResponse.AppendText("Row " + iRow + ": Name \"" + sNative + "\"" + Environment.NewLine);
                    Application.DoEvents();
                    // break;
                }

                if (string.IsNullOrEmpty(sPlainText))
                    ++iBlanks;
                else
                {
                    int iCount, iId;
                    if (!int.TryParse(sCount, out iCount))
                        txtResponse.AppendText(string.Format("Invalid count \"{0}\" found at row {1}{2}", sCount, iRow, Environment.NewLine));
                    if (!int.TryParse(sId, out iId ))
                        txtResponse.AppendText( string.Format("Invalid ID \"{0}\" found at row {1}{2}", sId, iRow, Environment.NewLine));

                    KrestniJmena q = (from jmNext in _mdB.KrestniJmenas
                                      where jmNext.CodePage == sNative 
                                      select jmNext).FirstOrDefault();

                    bool bIsNewRecord = false;

                    if (q != null)
                    {
                        ++iModified;
                        //q.CodePage = sNative;
                        //q.Web = sHtml;
                        //q.PlainText = sPlainText;
                        // txtResponse.AppendText("Modifying existing entry \"" + sNative + "\" with ID=" + sId + " and Count=" + sCount + Environment.NewLine);
                        // Application.DoEvents();
                    }
                    else
                    {
                        bIsNewRecord = true;

                        if ((iCount > 1) || !sNative.Contains(' '))
                        {
                            ++iNew;
                            // txtResponse.AppendText("Adding new entry \"" + sNative + "\" with ID=" + sId + " and Count=" + sCount + Environment.NewLine);
                            q = new KrestniJmena {CodePage = sNative, Web = sHtml, PlainText = sPlainText};
                            // Application.DoEvents();
                        }
                    }

                    if (q != null)
                    {
                        switch (sGender)
                        {
                            case "Male":
                                q.MaleIndex = iId;
                                q.MaleCount = iCount;
                                break;
                            case "Female":
                                q.FemaleIndex = iId;
                                q.FemaleCount = iCount;
                                break;
                        }

                        if ( bIsNewRecord )
                            _mdB.KrestniJmenas.InsertOnSubmit(q);

                        try
                        {
                            _mdB.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            txtResponse.AppendText(string.Format("Database error at row {0}: {1}{2}", iRow, ex, Environment.NewLine));
                        }
                    }

                    ++iNames;
                    iBlanks = 0;
                }
            }

            txtResponse.AppendText(iNames + " names found in spreadsheet" + Environment.NewLine);
            txtResponse.AppendText(iNew + " records added" + Environment.NewLine);
            txtResponse.AppendText(iModified + " records modified" + Environment.NewLine);
            fNames.Close();
        }

        #endregion

        #region Archived code

        /// <summary>
        /// Reads in all the original Denni Hlasatel death index data files
        /// and writes the data into a single comma-separated values file
        /// </summary>
        /// <returns>the number of records read</returns>
        private int CombineDhFilesIntoSingleCsv()
        {
            char[] cSeparators = { ',' };
            string sDhFolder, sDhOriginalsFolder;

            if ((DialogResult.OK != Utilities.CheckForDataFile("DH files", out sDhFolder)) ||
                (DialogResult.OK != Utilities.CheckForDataFile(@"DH files\Originals", out sDhOriginalsFolder)))
                return 0;
            int iRecords = 0;
            var fOut = new System.IO.StreamWriter(System.IO.Path.Combine(sDhFolder, "DH-ZZ-All.txt"));

            fOut.WriteLine("GivenName,Surname,ReportDate");

            foreach (string sNextFile in System.IO.Directory.EnumerateFiles(sDhOriginalsFolder, "DH-*.TXT"))
            {
                var fIn = new System.IO.StreamReader(sNextFile);
                // string sPathOut = System.IO.Path.Combine(sDhFolder, System.IO.Path.GetFileName(sNextFile));

                while (!fIn.EndOfStream)
                {
                    string sNextLine = fIn.ReadLine();
                    if (sNextLine == null)
                        break;
                    ++iRecords;
                    string[] sParsedLine = sNextLine.Split(cSeparators);
                    if (sParsedLine.Length >= 5)
                    {
                        string sDay = FormalDay(sParsedLine[2]);
                        string sMonth = FormalMonth(sParsedLine[3]);
                        string sYear = sParsedLine[4];
                        fOut.WriteLine(sParsedLine[0] + "," + sParsedLine[1] + "," +
                            sYear + "-" + sMonth + "-" + sDay);
                        txtFirstName.Text = sParsedLine[0];
                        txtLastName.Text = sParsedLine[1];
                        try
                        {
                            var dtWhen = new DateTime(int.Parse(sYear), int.Parse(sMonth), int.Parse(sDay));
                            txtDate.Text = dtWhen.ToString(CultureInfo.CurrentCulture);
                        }
                        catch
                        {
                            txtResponse.AppendText(sNextLine + Environment.NewLine);
                        }
                        // txtDate.Text = sParsedLine[2] + " " + sParsedLine[3] + " " + sParsedLine[4];

                        //xlOut.WriteRecord(sParsedLine[0], sParsedLine[1], sParsedLine[2] + " " + sParsedLine[3] + " " + sParsedLine[4]);
                        //if (iRecords >= 50000)
                        //{
                        //    fOut.Close();
                        //    ++iFilePart;
                        //    fOut = new System.IO.StreamWriter(System.IO.Path.Combine(sDhFolder, "DH-ZZ-All-Part" + iFilePart.ToString() + ".txt"));
                        //    fOut.WriteLine("GivenName,Surname,ReportDate");
                        //    iRecords = 0;
                        //}
                        Application.DoEvents();
                    }
                }

                fIn.Close();
                //fOut.Close();
            }

            fOut.Close();
            //xlOut.SaveAs(System.IO.Path.Combine(sDhFolder, "DH-ZZ-Part" + iFilePart.ToString() + ".xls"));
            //xlOut.Close();
            return iRecords;
        }

        /// <summary>
        /// Reads in all the original Denni Hlasatel death index data files
        /// and writes the data into a single XML file
        /// </summary>
        private void CombineDhFilesIntoXml()
        {
            char[] cSeparators = { ',' };
            string sDhFolder;

            if (DialogResult.OK != Utilities.CheckForDataFile("DH files", out sDhFolder))
                return;

            int iRecords = 0;
            var xRoot = new XElement("Root", "DenniHlasatelDeathIndices");
            var xmlOut = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xRoot);
            xRoot.Add(new XComment("Attribute meanings:"));
            xRoot.Add(new XComment("\"gn\" == Given name"));
            xRoot.Add(new XComment("\"fn\" == Family name"));
            xRoot.Add(new XComment("\"fd\" == Filing date"));

            foreach (string sNextFile in System.IO.Directory.EnumerateFiles(sDhFolder, "DH-*.TXT"))
            {
                var fIn = new System.IO.StreamReader(sNextFile);

                while (!fIn.EndOfStream)
                {
                    string sNextLine = fIn.ReadLine();
                    if (sNextLine == null)
                        break;
                    string[] sParsedLine = sNextLine.Split(cSeparators);
                    if (sParsedLine.Length >= 5)
                    {
                        ++iRecords;
                        string sDay = FormalDay(sParsedLine[2]);
                        string sMonth = FormalMonth(sParsedLine[3]);
                        string sYear = sParsedLine[4];
                        var xNext = new XElement( "dr" );
                        xNext.Add(new XAttribute("gn", sParsedLine[0] ?? string.Empty));
                        xNext.Add(new XAttribute("fn", sParsedLine[1] ?? string.Empty));
                        xNext.Add(new XAttribute("fd", sDay + " " + sMonth + " " + sYear));
                        xNext.Add(new XAttribute("id", iRecords.ToString(CultureInfo.InvariantCulture)));
                        xRoot.Add(xNext);
                        txtFirstName.Text = sParsedLine[0];
                        txtLastName.Text = sParsedLine[1];
                        txtDate.Text = string.Format("{0} {1} {2}", sDay, sMonth, sYear);
                        // txtDate.Text = sParsedLine[2] + " " + sParsedLine[3] + " " + sParsedLine[4];

                        //xlOut.WriteRecord(sParsedLine[0], sParsedLine[1], sParsedLine[2] + " " + sParsedLine[3] + " " + sParsedLine[4]);
                        //if (iRecords >= 50000)
                        //{
                        //    fOut.Close();
                        //    ++iFilePart;
                        //    fOut = new System.IO.StreamWriter(System.IO.Path.Combine(sDhFolder, "DH-ZZ-All-Part" + iFilePart.ToString() + ".txt"));
                        //    fOut.WriteLine("GivenName,Surname,ReportDate");
                        //    iRecords = 0;
                        //}
                        Application.DoEvents();
                    }
                }

                fIn.Close();
            }

            xmlOut.Save(System.IO.Path.Combine(sDhFolder, "DH-ZZ-All.xml"));
        }

        private DenniHlasatelDataStore _dsDenniHlasatel;

        /// <summary>
        /// Searches the XML Denni Hlasatel death index data store
        /// for death records matching entries in the user interface
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void denniHlasatelXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dsDenniHlasatel == null)
            {
                string sPath;
                
                if ( DialogResult.OK != Utilities.CheckForDataFile( @"DH files\DH-ZZ-All.xml", out sPath ) )
                    return;

                _dsDenniHlasatel = new DenniHlasatelDataStore( sPath );
            }

            var query = from dr in _dsDenniHlasatel.XmlDocument.Descendants("dr")
                        where dr.Attribute("gn").Value.Contains( txtFirstName.Text ) && 
                        dr.Attribute("fn").Value.Contains( txtLastName.Text )
                        select dr;

            mDhMatches = new List<IDeathRecord>();
            txtResponse.Clear();
            var xElements = query as IList<XElement> ?? query.ToList();
            txtResponse.AppendText(xElements.Count() + " matches found:" + Environment.NewLine);
            foreach (var drNext in xElements)
            {
                string sId = drNext.Attribute("id").Value, sGivenName = drNext.Attribute("gn").Value, sFamilyName = drNext.Attribute("fn").Value;
                string sFilingDate = drNext.Attribute("fd").Value;
                string[] sDateParts = sFilingDate.Split(new[] { ' ' });
                sFilingDate = sDateParts[1] + "/" + sDateParts[0] + "/" + sDateParts[2];
                txtResponse.AppendText("[" + sId + "] ");
                txtResponse.AppendText(sGivenName + " ");
                txtResponse.AppendText(sFamilyName + ", ");
                txtResponse.AppendText(sFilingDate + Environment.NewLine);
                IDeathRecord drNew = DeathRecord.Create(sFamilyName + ", " + sGivenName);
                drNew.FirstName = sGivenName; drNew.LastName = sFamilyName; drNew.CertificateNumber = int.Parse(sId); drNew.FilingDate = DateTime.Parse(sFilingDate); drNew.DeathDate = DateTime.MinValue;
                mDhMatches.Add(drNew);
            }

            DisplayRecords(mDhMatches);
            DisplayDenniHlasatelResultsInBrowser( OnDhDocumentLoaded );
        }

        /// <summary>
        /// Displays a <see cref="FolderBrowserDialog"/>, enabling the user
        /// to identify the disk folder containing the program's data files
        /// </summary>
        /// <param name="sender">originator of the event</param>
        /// <param name="e">additional details on the event</param>
        private void dataFilesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.PromptForDataFilesFolder();
        }

        #endregion

    }
}
