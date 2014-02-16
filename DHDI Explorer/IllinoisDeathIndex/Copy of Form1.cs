using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Genealogy
{
    public partial class Form1 : Form
    {
        #region Private members
        System.Data.SqlClient.SqlConnection mdbConnection;
        #endregion

        public Form1()
        {
            InitializeComponent();
            mdbConnection = new System.Data.SqlClient.SqlConnection();
        }

        private void cmdGet_Click(object sender, EventArgs e)
        {
            txtResponse.Clear();

            HttpWebRequest reqNew = (HttpWebRequest)WebRequest.CreateDefault(new Uri("http://www.ilsos.gov/isavital/idphdeathsrch.jsp"));
            reqNew.Method = "GET";

            // execute the request
            HttpWebResponse response = (HttpWebResponse)
                reqNew.GetResponse();

            // we will read data via the response stream
            System.IO.Stream resStream = response.GetResponseStream();
            System.IO.TextReader sReader = new System.IO.StreamReader(resStream);

            string tempString = null;
            while ( ( tempString = sReader.ReadLine() ) != null )
            {
                txtResponse.AppendText(tempString + Environment.NewLine );
            }
        }

        private void cmdPost_Click(object sender, EventArgs e)
        {
            txtResponse.Clear();

            string[] sDate = txtDate.Text.Split(',');
            bool bBefore1916 = false;

            if (sDate.Length > 2)
            {
                string sYear = sDate[2];
                int iYear = 1916;

                if (int.TryParse(sYear, out iYear))
                {
                    bBefore1916 = iYear < 1916;
                }
            }

            string sUrl = bBefore1916 ? "http://www.ilsos.gov/isavital/deathSearch.do" : "http://www.ilsos.gov/isavital/idphDeathSearch.do";
            string sPostData = bBefore1916 ? "name=" + txtLastName.Text + ", " + txtFirstName.Text + "&county=COOK" : "firstName=" + txtFirstName.Text + "&lastName=" + txtLastName.Text + "&middleName=&county=COOK";
            HttpWebRequest reqNew = (HttpWebRequest)WebRequest.CreateDefault(new Uri(sUrl));
            reqNew.Method = "POST";

            reqNew.ContentType = "application/x-www-form-urlencoded";

            byte[] bytedata = Encoding.UTF8.GetBytes(sPostData);
            reqNew.ContentLength = bytedata.Length;

            System.IO.Stream requestStream = reqNew.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();

            // execute the request
            System.IO.Stream resStream;
            try
            {
                HttpWebResponse response = (HttpWebResponse)
                reqNew.GetResponse();

                // we will read data via the response stream
                resStream = response.GetResponseStream();
            }
            catch (WebException ex)
            {
                string sMessage = ex.Response != null ? ex.Response.ToString() : ex.ToString();
                MessageBox.Show(sMessage, "Web Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resStream = ex.Response.GetResponseStream();
            }
            catch (Exception)
            {
                throw;
            }

            System.IO.TextReader sReader = new System.IO.StreamReader(resStream);
            

            string tempString = null;
            while ((tempString = sReader.ReadLine()) != null)
            {
                txtResponse.AppendText(tempString + Environment.NewLine);
            }
            resStream.Close();

            webBrowser1.DocumentText = txtResponse.Text;
            int iLenText = txtResponse.TextLength;
            //while (webBrowser1.DocumentText.Length != iLenText)
            //    System.Threading.Thread.Sleep(100);
            //HtmlDocument docResponse = webBrowser1.Document;

            //DisplayInTree(docResponse);
        }

        /// <summary>
        /// Called by the thread.start to set up for filling the DOM tree
        /// </summary>
        /// <param name="elemColl">HTMLElement collection with the "HTML" tag as its root</param>
        /// <remarks>Has to be passed as an object for the ParameterizedThreadStart</remarks>
        private void DisplayInTree(HtmlDocument docResponse)
        {
            HtmlElementCollection elemColl = docResponse.GetElementsByTagName("html");

            tvDocument.Nodes.Clear();
            TreeNode rootNode = new TreeNode("Web response");
            rootNode.Tag = -1;
            tvDocument.Nodes.Add(rootNode);
            FillDomTree(elemColl, rootNode, 0);
        }

        /// <summary>
        /// Recursive function to add the DOM element to the treeview
        /// </summary>
        /// <param name="currentEle">collection of elements</param>
        /// <param name="tn">tree node. currently not used</param>
        /// <remarks>The treenode was used prior to placing this function on its own thread
        /// at that point, I was nesting the nodes. right now, i am having trouble doing this across threads</remarks>
        private static void FillDomTree(HtmlElementCollection currentEle, TreeNode tn, int nodeTracker)
        {

            foreach (HtmlElement element in currentEle)
            {
                TreeNode tempNode;

                var tag = element.TagName;

                if (tag.CompareTo("!") == 0)
                {
                    tempNode = tn.Nodes.Add("<Comment>");
                }
                else
                {
                    string sNodeText = "<" + element.TagName;
                    if ((element.Name != null ) && (element.Name.Length > 0))
                        sNodeText += " name=\"" + element.Name + "\"";
                    if ((element.Id != null) && (element.Id.Length > 0))
                        sNodeText += " id=\"" + element.Id + "\"";
                    sNodeText += ">";
                    tempNode = tn.Nodes.Add(sNodeText);
                    int iInnerTextLength = element.InnerText != null ? element.InnerText.Length : 0;
                    if (iInnerTextLength > 0)
                    {
                        string sInnerText;
                        if (iInnerTextLength > 20)
                            sInnerText = element.InnerText.Substring(0, 20) + "...";
                        else
                            sInnerText = element.InnerText;
                        tempNode.Nodes.Add("InnerText=\"" + sInnerText + "\"");
                    }
                }

                /** a counter to keep track of the absolute index of the nodes. Stuffing value into tag **/
                tempNode.Tag = nodeTracker++;

                /** over and over and over and over **/
                FillDomTree(element.Children, tempNode, nodeTracker);
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlDocument docResponse = webBrowser1.Document;
            DisplayInTree(docResponse);

            lvHits.Items.Clear();
            lvHits.Columns.Clear();

            HtmlElement eTable = docResponse.GetElementById("theTable");
            if (eTable == null)
                return;
            HtmlElementCollection cRows = eTable.GetElementsByTagName("TR");

            for (int i = 0 ; i < cRows.Count ; ++i)
            {
                HtmlElement eNextRow = cRows[i];
                HtmlElementCollection cFields = eNextRow.GetElementsByTagName("TD");
                ListViewItem itmNext = i != 0 ? lvHits.Items.Add(cFields[0].InnerText) : null;
                DeathRecord drNew = DeathRecord.Create("NewDeathRecord");
                List<string> lPropertyNames = new List<string>();

                foreach (HtmlElement eField in cFields)
	            {
                    if (i == 0)
                    {
                        lvHits.Columns.Add(eField.InnerText);
                        lPropertyNames.Add( eField.InnerHtml );
                    }
                    else
                    {
                        for (int j = 0; j < cFields.Count; ++j)
                        {
                            string sValue = cFields[j].InnerText;
                            drNew.SetDeathIndexProperty(lPropertyNames[j], sValue);
                            if ( j > 0 )
                                itmNext.SubItems.Add(sValue);
                        }
                    }
	            }
            }
        }

        private const string sDataFilesFolder = "C:/Documents and Settings/Dell User/My Documents/Dropbox/Family/Denni Hlasatel Death Index/Denni Hlasatel data files";
        // private const string sDataFilesFolder = "C:/Users/James Vlcek/Dropbox/Family/Denni Hlasatel Death Index/Denni Hlasatel data files";
        private System.IO.TextReader fDataFile = null;

        private void cmdNext_Click(object sender, EventArgs e)
        {
            if (fDataFile == null)
            {
                fDataFile = new System.IO.StreamReader(System.IO.Path.Combine(sDataFilesFolder, "DH-V.txt"));
            }

            string sNextEntry = fDataFile.ReadLine();
            // TODO: Detect end of file
            string[] sFields = sNextEntry.Split( ',' );
            txtFirstName.Text = sFields[0];
            txtLastName.Text = sFields[1];
            txtDate.Text = sFields[2] + "," + sFields[3] + "," + sFields[4];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'denniHlasatelDataSet.DenniHlasatelDeathIndices' table. You can move, or remove it, as needed.
            this.denniHlasatelDeathIndicesTableAdapter.Fill(this.denniHlasatelDataSet.DenniHlasatelDeathIndices);

        }

        private void cmdFamilySearch_Click(object sender, EventArgs e)
        {
            txtResponse.Clear();

            string[] sDate = txtDate.Text.Split(',');

            string sYear = sDate[2];

            //string sUrl = "https://www.familysearch.org/search/records/index#count=20&query=%2B";
            //sUrl += "givenname%3A" + txtFirstName.Text + "~%20%2Bsurname%3A" + txtLastName.Text + "~%20%2Bdeath_place%3AIllinois~%20%2Bdeath_year%3A" + sYear + "-" + sYear;
            string sUrl = "https://www.familysearch.org/search/index/record-search";
            string sPostData = "searchType=records&filtered=false&fed=true&collectionId=&collectionName=&givenname=" + txtFirstName.Text + "&surname=" + txtLastName.Text;
            sUrl += "?" + sPostData;
            HttpWebRequest reqNew = (HttpWebRequest)WebRequest.CreateDefault(new Uri(sUrl));
            reqNew.Method = "GET";

            //reqNew.ContentType = "application/x-www-form-urlencoded";

            //byte[] bytedata = Encoding.UTF8.GetBytes(sPostData);
            //reqNew.ContentLength = bytedata.Length;

            //System.IO.Stream requestStream = reqNew.GetRequestStream();
            //requestStream.Write(bytedata, 0, bytedata.Length);
            //requestStream.Close();

            // execute the request
            System.IO.Stream responseStream;
            try
            {
                HttpWebResponse response = (HttpWebResponse)
                reqNew.GetResponse();

                // we will read data via the response stream
                responseStream = response.GetResponseStream();
            }
            catch (WebException ex)
            {
                string sMessage = ex.Response != null ? ex.Response.ToString() : ex.ToString();
                MessageBox.Show(sMessage, "Web Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                responseStream = ex.Response.GetResponseStream();
            }
            catch (Exception)
            {
                throw;
            }

            System.IO.TextReader sReader = new System.IO.StreamReader(responseStream);


            string tempString = null;
            while ((tempString = sReader.ReadLine()) != null)
            {
                txtResponse.AppendText(tempString + Environment.NewLine);
            }
            responseStream.Close();

            webBrowser1.DocumentText = txtResponse.Text;
            int iLenText = txtResponse.TextLength;
        }
    }
}
