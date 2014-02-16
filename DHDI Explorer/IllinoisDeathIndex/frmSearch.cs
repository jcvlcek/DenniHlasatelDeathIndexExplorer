using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Genealogy
{
    public class frmSearch : Form
    {
        private Label label1;
        private CheckBox chkUseRegex;
        private Button cmdSearch;
        private Button cmdCancel;
        private TextBox txtSearchTerm;
    
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearchTerm = new System.Windows.Forms.TextBox();
            this.chkUseRegex = new System.Windows.Forms.CheckBox();
            this.cmdSearch = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search term:";
            // 
            // txtSearchTerm
            // 
            this.txtSearchTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchTerm.Location = new System.Drawing.Point(104, 21);
            this.txtSearchTerm.Name = "txtSearchTerm";
            this.txtSearchTerm.Size = new System.Drawing.Size(139, 22);
            this.txtSearchTerm.TabIndex = 1;
            // 
            // chkUseRegex
            // 
            this.chkUseRegex.AutoSize = true;
            this.chkUseRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseRegex.Location = new System.Drawing.Point(23, 60);
            this.chkUseRegex.Name = "chkUseRegex";
            this.chkUseRegex.Size = new System.Drawing.Size(173, 20);
            this.chkUseRegex.TabIndex = 2;
            this.chkUseRegex.Text = "Use regular expressions";
            this.chkUseRegex.UseVisualStyleBackColor = true;
            // 
            // cmdSearch
            // 
            this.cmdSearch.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdSearch.Location = new System.Drawing.Point(182, 91);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(60, 26);
            this.cmdSearch.TabIndex = 3;
            this.cmdSearch.Text = "Search";
            this.cmdSearch.UseVisualStyleBackColor = true;
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(116, 91);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(60, 26);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // frmSearch
            // 
            this.ClientSize = new System.Drawing.Size(257, 131);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdSearch);
            this.Controls.Add(this.chkUseRegex);
            this.Controls.Add(this.txtSearchTerm);
            this.Controls.Add(this.label1);
            this.Name = "frmSearch";
            this.Text = "Search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public frmSearch()
        {
            InitializeComponent();
        }

        #region Private members
        private TextBox mResults;
        private IEnumerable<string> mEntries;
        #endregion

        #region Public methods

        public void Search(IEnumerable<string> lEntries, TextBox txtResults)
        {
            mResults = txtResults;
            mEntries = lEntries;
            Show();
        }

        #endregion

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            mResults.Clear();

            if (chkUseRegex.Checked)
            {
                try
                {
                    Regex.IsMatch("Name", txtSearchTerm.Text);
                }
                catch (ArgumentException eX)
                {
                    MessageBox.Show(eX.Message, "Invalid regular expression patter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            foreach (string sNext in mEntries)
            {
                bool bMatch = false;

                if (chkUseRegex.Checked)
                {
                    bMatch = Regex.IsMatch(sNext, txtSearchTerm.Text);
                }
                else
                {
                    bMatch = sNext.Contains(txtSearchTerm.Text);
                }

                if (bMatch)
                {
                    mResults.AppendText(sNext + Environment.NewLine);
                    Application.DoEvents();
                }
            }

            Hide();
        }
    }
}
