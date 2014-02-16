using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using dbAccess;

namespace Genealogy
{
    public partial class Form1 : Form
    {
        #region Event handlers

        private void givenNamesCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sGivenNames;

            if (DialogResult.OK != Utilities.CheckForDataFile("GivenNamesMaleTransliterated-2.xls", out sGivenNames))
                return;
            ExcelFile fNames = new ExcelFile(sGivenNames);
            List<char> lSpecialChars = new List<char>();
            List<int> lBogusCounts = new List<int>();

            txtResponse.Clear();
            int iBlanks, iNames, iRow;
            for (iBlanks = 0, iNames = 0, iRow = 2; iBlanks < 5; ++iRow)
            {
                string sId = fNames.ValueAt( new Point( 1, iRow ) ).Trim();
                string sNative = fNames.ValueAt(new Point(2, iRow)).Trim();
                string sHtml = fNames.ValueAt(new Point(3, iRow)).Trim();
                string sPlainText = fNames.ValueAt(new Point(4, iRow)).Trim();
                string sCount = fNames.ValueAt(new Point(5, iRow)).Trim();

                if ((iRow % 1000) == 0)
                {
                    txtResponse.AppendText("Row " + iRow.ToString() + ": Name \"" + sNative + "\"" + Environment.NewLine);
                    Application.DoEvents();
                }

                if (string.IsNullOrEmpty(sPlainText))
                    ++iBlanks;
                else
                {
                    int iCount;
                    if (!int.TryParse(sCount, out iCount))
                    {
                        txtResponse.AppendText("Invalid count \"" + sCount + "\" found at row " + iRow.ToString() + Environment.NewLine);
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
                        txtResponse.AppendText("Special char at row " + iRow.ToString() + ": Web=\"" + sHtml + "\", PlainText=\"" + sPlainText + "\"" + Environment.NewLine);
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

            txtResponse.AppendText(iNames.ToString() + " names found in spreadsheet" + Environment.NewLine);
            foreach (char cNext in lSpecialChars)
            {
                txtResponse.AppendText("new TransChar( \"" + cNext + "\", \"&#" + ((int)cNext).ToString() + ";\", \"\" )," + Environment.NewLine);
            }
            foreach (int iNextRow in lBogusCounts)
            {
                txtResponse.AppendText("Bogus count at row " + iNextRow.ToString() + Environment.NewLine);
            }
            // fNames.SaveAs(System.IO.Path.Combine(Utilities.DataFilesFolder, "GivenNamesFemaleTransliterated-2.xls"));
            fNames.Close();
        }

        private void givenNamesMergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sGivenNames;
            string sGender = "Female"; // "Male"

            if (DialogResult.OK != Utilities.CheckForDataFile("GivenNames" + sGender + "Transliterated-2.xls", out sGivenNames))
                return;
            ExcelFile fNames = new ExcelFile(sGivenNames);

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
                    txtResponse.AppendText("Row " + iRow.ToString() + ": Name \"" + sNative + "\"" + Environment.NewLine);
                    Application.DoEvents();
                    // break;
                }

                if (string.IsNullOrEmpty(sPlainText))
                    ++iBlanks;
                else
                {
                    int iCount = -1, iId = -1;
                    if (!int.TryParse(sCount, out iCount))
                        txtResponse.AppendText("Invalid count \"" + sCount + "\" found at row " + iRow.ToString() + Environment.NewLine);
                    if (!int.TryParse(sId, out iId ))
                        txtResponse.AppendText( "Invalid ID \"" + sId + "\" found at row " + iRow.ToString() + Environment.NewLine);

                    KrestniJmena q = (from jmNext in mdB.KrestniJmenas
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
                            q = new KrestniJmena();
                            q.CodePage = sNative;
                            q.Web = sHtml;
                            q.PlainText = sPlainText;
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
                            default:
                                break;
                        }

                        if ( bIsNewRecord )
                            mdB.KrestniJmenas.InsertOnSubmit(q);

                        try
                        {
                            mdB.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            txtResponse.AppendText("Database error at row " + iRow.ToString() + ": " + ex.ToString() + Environment.NewLine);
                        }
                    }

                    ++iNames;
                    iBlanks = 0;
                }
            }

            txtResponse.AppendText(iNames.ToString() + " names found in spreadsheet" + Environment.NewLine);
            txtResponse.AppendText(iNew.ToString() + " records added" + Environment.NewLine);
            txtResponse.AppendText(iModified.ToString() + " records modified" + Environment.NewLine);
            fNames.Close();
        }

        #endregion

        #region Archived code

        private void CombineDhFilesIntoSingleCSV()
        {
            char[] cSeparators = new char[] { ',' };
            string sDhFolder, sDhOriginalsFolder;

            if ((DialogResult.OK != Utilities.CheckForDataFile("DH files", out sDhFolder)) ||
                (DialogResult.OK != Utilities.CheckForDataFile(@"DH files\Originals", out sDhOriginalsFolder)))
                return;
            int iRecords = 0;
            System.IO.StreamWriter fOut = new System.IO.StreamWriter(System.IO.Path.Combine(sDhFolder, "DH-ZZ-All.txt"));

            fOut.WriteLine("GivenName,Surname,ReportDate");

            foreach (string sNextFile in System.IO.Directory.EnumerateFiles(sDhOriginalsFolder, "DH-*.TXT"))
            {
                System.IO.StreamReader fIn = new System.IO.StreamReader(sNextFile);
                string sPathOut = System.IO.Path.Combine(sDhFolder, System.IO.Path.GetFileName(sNextFile));

                while (!fIn.EndOfStream)
                {
                    string sNextLine = fIn.ReadLine();
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
                            DateTime dtWhen = new DateTime(int.Parse(sYear), int.Parse(sMonth), int.Parse(sDay));
                            txtDate.Text = dtWhen.ToString();
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
        }

        private void CombineDhFilesIntoXml()
        {
            char[] cSeparators = new char[] { ',' };
            string sDhFolder;

            if (DialogResult.OK != Utilities.CheckForDataFile("DH files", out sDhFolder))
                return;

            int iRecords = 0;
            XDocument xmlOut = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Root", "DenniHlasatelDeathIndices"));
            XElement xRoot = xmlOut.Element("Root");
            xRoot.Add(new XComment("Attribute meanings:"));
            xRoot.Add(new XComment("\"gn\" == Given name"));
            xRoot.Add(new XComment("\"fn\" == Family name"));
            xRoot.Add(new XComment("\"fd\" == Filing date"));

            foreach (string sNextFile in System.IO.Directory.EnumerateFiles(sDhFolder, "DH-*.TXT"))
            {
                System.IO.StreamReader fIn = new System.IO.StreamReader(sNextFile);

                while (!fIn.EndOfStream)
                {
                    string sNextLine = fIn.ReadLine();
                    string[] sParsedLine = sNextLine.Split(cSeparators);
                    if (sParsedLine.Length >= 5)
                    {
                        ++iRecords;
                        string sDay = FormalDay(sParsedLine[2]);
                        string sMonth = FormalMonth(sParsedLine[3]);
                        string sYear = sParsedLine[4];
                        XElement xNext = new XElement( "dr" );
                        xNext.Add(new XAttribute("gn", sParsedLine[0] ?? string.Empty));
                        xNext.Add(new XAttribute("fn", sParsedLine[1] ?? string.Empty));
                        xNext.Add(new XAttribute("fd", sDay + " " + sMonth + " " + sYear));
                        xNext.Add(new XAttribute("id", iRecords.ToString()));
                        xRoot.Add(xNext);
                        txtFirstName.Text = sParsedLine[0];
                        txtLastName.Text = sParsedLine[1];
                        txtDate.Text = sDay + " " + sMonth + " " + sYear;
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

        private DenniHlasatelDataStore dsDenniHlasatel = null;

        private void denniHlasatelXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dsDenniHlasatel == null)
            {
                string sPath;
                
                if ( DialogResult.OK != Utilities.CheckForDataFile( @"DH files\DH-ZZ-All.xml", out sPath ) )
                    return;

                dsDenniHlasatel = new DenniHlasatelDataStore( sPath );
            }

            var query = from dr in dsDenniHlasatel.XmlDocument.Descendants("dr")
                        where dr.Attribute("gn").Value.Contains( txtFirstName.Text ) && 
                        dr.Attribute("fn").Value.Contains( txtLastName.Text )
                        select dr;

            if (mDhMatches == null)
                mDhMatches = new List<IDeathRecord>();
            else
                mDhMatches = new List<IDeathRecord>();
            txtResponse.Clear();
            txtResponse.AppendText(query.Count().ToString() + " matches found:" + Environment.NewLine);
            foreach (var drNext in query)
            {
                string sId = drNext.Attribute("id").Value, sGivenName = drNext.Attribute("gn").Value, sFamilyName = drNext.Attribute("fn").Value;
                string sFilingDate = drNext.Attribute("fd").Value;
                string[] sDateParts = sFilingDate.Split(new char[] { ' ' });
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

        private void dataFilesFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.PromptForDataFilesFolder();
        }

        #endregion

    }
}
