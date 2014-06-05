using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Genealogy
{
    /// <summary>
    /// Form for searching string lists for names matching a specified pattern
    /// (optionally using regular expressions for matching)
    /// </summary>
    /// <remarks>The search results are placed into a text box supplied to this form as an argument to the <see cref="Search"/> method</remarks>
    public class SearchForm : Form
    {
        #region User interface (UX) elements

        private Label _label1;

        /// <summary>
        /// Flags whether the contents of <see cref="_txtSearchTerm"/> are to be treated as a regular expression
        /// </summary>
        private CheckBox _chkUseRegex;

        /// <summary>
        /// Searches for names matching the pattern in <see cref="_txtSearchTerm"/>
        /// </summary>
        private Button _cmdSearch;

        /// <summary>
        /// Cancels the search and closes the form
        /// </summary>
        private Button _cmdCancel;

        /// <summary>
        /// The string (potentially using regular expressions) to search for in the name list
        /// </summary>
        private TextBox _txtSearchTerm;
    
        /// <summary>
        /// Initialize the user interface of the form
        /// </summary>
        private void InitializeComponent()
        {
            _label1 = new Label();
            _txtSearchTerm = new TextBox();
            _chkUseRegex = new CheckBox();
            _cmdSearch = new Button();
            _cmdCancel = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            _label1.AutoSize = true;
            _label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _label1.Location = new System.Drawing.Point(15, 24);
            _label1.Name = "_label1";
            _label1.Size = new System.Drawing.Size(83, 16);
            _label1.TabIndex = 0;
            _label1.Text = "Search term:";
            // 
            // txtSearchTerm
            // 
            _txtSearchTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _txtSearchTerm.Location = new System.Drawing.Point(104, 21);
            _txtSearchTerm.Name = "_txtSearchTerm";
            _txtSearchTerm.Size = new System.Drawing.Size(139, 22);
            _txtSearchTerm.TabIndex = 1;
            // 
            // chkUseRegex
            // 
            _chkUseRegex.AutoSize = true;
            _chkUseRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _chkUseRegex.Location = new System.Drawing.Point(23, 60);
            _chkUseRegex.Name = "_chkUseRegex";
            _chkUseRegex.Size = new System.Drawing.Size(173, 20);
            _chkUseRegex.TabIndex = 2;
            _chkUseRegex.Text = "Use regular expressions";
            _chkUseRegex.UseVisualStyleBackColor = true;
            // 
            // cmdSearch
            // 
            _cmdSearch.DialogResult = DialogResult.OK;
            _cmdSearch.Location = new System.Drawing.Point(182, 91);
            _cmdSearch.Name = "_cmdSearch";
            _cmdSearch.Size = new System.Drawing.Size(60, 26);
            _cmdSearch.TabIndex = 3;
            _cmdSearch.Text = "Search";
            _cmdSearch.UseVisualStyleBackColor = true;
            _cmdSearch.Click += cmdSearch_Click;
            // 
            // cmdCancel
            // 
            _cmdCancel.DialogResult = DialogResult.Cancel;
            _cmdCancel.Location = new System.Drawing.Point(116, 91);
            _cmdCancel.Name = "_cmdCancel";
            _cmdCancel.Size = new System.Drawing.Size(60, 26);
            _cmdCancel.TabIndex = 4;
            _cmdCancel.Text = "Cancel";
            _cmdCancel.UseVisualStyleBackColor = true;
            _cmdCancel.Click += cmdCancel_Click;
            // 
            // SearchForm
            // 
            ClientSize = new System.Drawing.Size(257, 131);
            Controls.Add(_cmdCancel);
            Controls.Add(_cmdSearch);
            Controls.Add(_chkUseRegex);
            Controls.Add(_txtSearchTerm);
            Controls.Add(_label1);
            Name = "SearchForm";
            Text = "Search";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of the search form
        /// </summary>
        public SearchForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Private members

        /// <summary>
        /// <see cref="TextBox"/> the search results are placed into
        /// (the previous contents are completely overwritten with new search results)
        /// </summary>
        private TextBox _mResults;

        /// <summary>
        /// The list of strings (names) to be searched
        /// </summary>
        private IEnumerable<string> _mEntries;
        #endregion

        #region Public methods

        /// <summary>
        /// <para>Specifies the list of strings to be searched, and the text box to display the results in</para>
        /// After initializing these internal members, the form displays itself
        /// </summary>
        /// <param name="lEntries">the list of strings to be searched</param>
        /// <param name="txtResults"></param>
        /// <remarks>Despite its name, this method does not perform an actual search.
        /// A search is performed only when the <see cref="_cmdSearch"/> button is pressed</remarks>
        public void Search(IEnumerable<string> lEntries, TextBox txtResults)
        {
            _mResults = txtResults;
            _mEntries = lEntries;
            Show();
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Action(s) taken on pressing the "Cancel" button: Hide the form
        /// </summary>
        /// <param name="sender">sender of the event (the <see cref="_cmdCancel"/> button</param>
        /// <param name="e">button press event arguments</param>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Action(s) taken on pressing the "Search" button:
        /// clear the results text box, perform the search, and write the results into the text box
        /// </summary>
        /// <param name="sender">sender of the event (the <see cref="_cmdCancel"/> button</param>
        /// <param name="e">button press event arguments</param>
        private void cmdSearch_Click(object sender, EventArgs e)
        {
            _mResults.Clear();

            foreach (string sNext in _mEntries)
            {
                bool bMatch;

                if (_chkUseRegex.Checked)
                {
                    try
                    {
                        bMatch = Regex.IsMatch(sNext, _txtSearchTerm.Text);
                    }
                    catch (Exception eX)
                    {
                        MessageBox.Show(eX.Message, "Invalid regular expression pattern", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    bMatch = sNext.Contains(_txtSearchTerm.Text);
                }

                if (!bMatch) continue;
                _mResults.AppendText(sNext + Environment.NewLine);
                Application.DoEvents();
            }

            Hide();
        }

        #endregion

    }
}
