namespace Genealogy
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.givenNameEquivalentsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.genealogyDataSet = new Genealogy.GenealogyDataSet();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabBrowse = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tabHits = new System.Windows.Forms.TabPage();
            this.lvHits = new System.Windows.Forms.ListView();
            this.imgList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabText = new System.Windows.Forms.TabPage();
            this.txtResponse = new System.Windows.Forms.TextBox();
            this.tabTree = new System.Windows.Forms.TabPage();
            this.tvDocument = new System.Windows.Forms.TreeView();
            this.tabdB = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.englishDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nativeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dHDeathIndexBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.txtDate = new System.Windows.Forms.TextBox();
            this.dHDeathIndexTableAdapter = new Genealogy.GenealogyDataSetTableAdapters.DHDeathIndexTableAdapter();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataFilesFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.framesDumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.illinoisDeathIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.familySearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDenniHlasatelSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.denniHlasatelXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchGivenNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchSurnamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listGivenNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listSurnamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.unrecognizedGivenNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.givenNamesCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.givenNamesMergeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertDHFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prijmeniConversionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prijmeniRankCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.givenNamesTranslitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.nextDHRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kdeJsmeSurnameSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.denniHlasatelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.givenNameEquivalentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prijmeniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.krestniJmenaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.givenNameEquivalentsTableAdapter = new Genealogy.GenealogyDataSetTableAdapters.GivenNameEquivalentsTableAdapter();
            this.prijmeniBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.prijmeniTableAdapter = new Genealogy.GenealogyDataSetTableAdapters.PrijmeniTableAdapter();
            this.tmrAutoCompleteSurname = new System.Windows.Forms.Timer(this.components);
            this.krestniJmenaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.krestniJmenaTableAdapter = new Genealogy.GenealogyDataSetTableAdapters.KrestniJmenaTableAdapter();
            this.txtLastName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.givenNameEquivalentsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genealogyDataSet)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabBrowse.SuspendLayout();
            this.tabHits.SuspendLayout();
            this.tabText.SuspendLayout();
            this.tabTree.SuspendLayout();
            this.tabdB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dHDeathIndexBindingSource)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.prijmeniBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krestniJmenaBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // givenNameEquivalentsBindingSource
            // 
            this.givenNameEquivalentsBindingSource.DataMember = "GivenNameEquivalents";
            this.givenNameEquivalentsBindingSource.DataSource = this.genealogyDataSet;
            // 
            // genealogyDataSet
            // 
            this.genealogyDataSet.DataSetName = "GenealogyDataSet";
            this.genealogyDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // txtFirstName
            // 
            resources.ApplyResources(this.txtFirstName, "txtFirstName");
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.TextChanged += new System.EventHandler(this.txtFirstName_TextChanged);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabBrowse);
            this.tabControl1.Controls.Add(this.tabHits);
            this.tabControl1.Controls.Add(this.tabText);
            this.tabControl1.Controls.Add(this.tabTree);
            this.tabControl1.Controls.Add(this.tabdB);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabBrowse
            // 
            resources.ApplyResources(this.tabBrowse, "tabBrowse");
            this.tabBrowse.Controls.Add(this.webBrowser1);
            this.tabBrowse.Name = "tabBrowse";
            this.tabBrowse.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            resources.ApplyResources(this.webBrowser1, "webBrowser1");
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // tabHits
            // 
            resources.ApplyResources(this.tabHits, "tabHits");
            this.tabHits.Controls.Add(this.lvHits);
            this.tabHits.Name = "tabHits";
            this.tabHits.UseVisualStyleBackColor = true;
            // 
            // lvHits
            // 
            resources.ApplyResources(this.lvHits, "lvHits");
            this.lvHits.LargeImageList = this.imgList1;
            this.lvHits.Name = "lvHits";
            this.lvHits.SmallImageList = this.imgList1;
            this.lvHits.UseCompatibleStateImageBehavior = false;
            this.lvHits.View = System.Windows.Forms.View.Details;
            this.lvHits.DoubleClick += new System.EventHandler(this.lvHits_DoubleClick);
            // 
            // imgList1
            // 
            this.imgList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList1.ImageStream")));
            this.imgList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList1.Images.SetKeyName(0, "Match");
            this.imgList1.Images.SetKeyName(1, "NoMatch");
            // 
            // tabText
            // 
            resources.ApplyResources(this.tabText, "tabText");
            this.tabText.Controls.Add(this.txtResponse);
            this.tabText.Name = "tabText";
            this.tabText.UseVisualStyleBackColor = true;
            // 
            // txtResponse
            // 
            resources.ApplyResources(this.txtResponse, "txtResponse");
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.ReadOnly = true;
            // 
            // tabTree
            // 
            resources.ApplyResources(this.tabTree, "tabTree");
            this.tabTree.Controls.Add(this.tvDocument);
            this.tabTree.Name = "tabTree";
            this.tabTree.UseVisualStyleBackColor = true;
            // 
            // tvDocument
            // 
            resources.ApplyResources(this.tvDocument, "tvDocument");
            this.tvDocument.Name = "tvDocument";
            // 
            // tabdB
            // 
            resources.ApplyResources(this.tabdB, "tabdB");
            this.tabdB.Controls.Add(this.dataGridView1);
            this.tabdB.Name = "tabdB";
            this.tabdB.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.englishDataGridViewTextBoxColumn,
            this.nativeDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.givenNameEquivalentsBindingSource;
            this.dataGridView1.Name = "dataGridView1";
            // 
            // englishDataGridViewTextBoxColumn
            // 
            this.englishDataGridViewTextBoxColumn.DataPropertyName = "English";
            resources.ApplyResources(this.englishDataGridViewTextBoxColumn, "englishDataGridViewTextBoxColumn");
            this.englishDataGridViewTextBoxColumn.Name = "englishDataGridViewTextBoxColumn";
            // 
            // nativeDataGridViewTextBoxColumn
            // 
            this.nativeDataGridViewTextBoxColumn.DataPropertyName = "Native";
            resources.ApplyResources(this.nativeDataGridViewTextBoxColumn, "nativeDataGridViewTextBoxColumn");
            this.nativeDataGridViewTextBoxColumn.Name = "nativeDataGridViewTextBoxColumn";
            // 
            // dHDeathIndexBindingSource
            // 
            this.dHDeathIndexBindingSource.DataMember = "DHDeathIndex";
            this.dHDeathIndexBindingSource.DataSource = this.genealogyDataSet;
            // 
            // txtDate
            // 
            resources.ApplyResources(this.txtDate, "txtDate");
            this.txtDate.Name = "txtDate";
            // 
            // dHDeathIndexTableAdapter
            // 
            this.dHDeathIndexTableAdapter.ClearBeforeFill = true;
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.webToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.tableToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.dataFilesFolderToolStripMenuItem,
            this.framesDumpToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            // 
            // dataFilesFolderToolStripMenuItem
            // 
            resources.ApplyResources(this.dataFilesFolderToolStripMenuItem, "dataFilesFolderToolStripMenuItem");
            this.dataFilesFolderToolStripMenuItem.Name = "dataFilesFolderToolStripMenuItem";
            this.dataFilesFolderToolStripMenuItem.Click += new System.EventHandler(this.dataFilesFolderToolStripMenuItem_Click);
            // 
            // framesDumpToolStripMenuItem
            // 
            resources.ApplyResources(this.framesDumpToolStripMenuItem, "framesDumpToolStripMenuItem");
            this.framesDumpToolStripMenuItem.Name = "framesDumpToolStripMenuItem";
            this.framesDumpToolStripMenuItem.Click += new System.EventHandler(this.framesDumpToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // webToolStripMenuItem
            // 
            resources.ApplyResources(this.webToolStripMenuItem, "webToolStripMenuItem");
            this.webToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.illinoisDeathIndexToolStripMenuItem,
            this.familySearchToolStripMenuItem,
            this.mnuDenniHlasatelSearch,
            this.denniHlasatelXMLToolStripMenuItem});
            this.webToolStripMenuItem.Name = "webToolStripMenuItem";
            // 
            // illinoisDeathIndexToolStripMenuItem
            // 
            resources.ApplyResources(this.illinoisDeathIndexToolStripMenuItem, "illinoisDeathIndexToolStripMenuItem");
            this.illinoisDeathIndexToolStripMenuItem.Name = "illinoisDeathIndexToolStripMenuItem";
            this.illinoisDeathIndexToolStripMenuItem.Click += new System.EventHandler(this.illinoisDeathIndexToolStripMenuItem_Click);
            // 
            // familySearchToolStripMenuItem
            // 
            resources.ApplyResources(this.familySearchToolStripMenuItem, "familySearchToolStripMenuItem");
            this.familySearchToolStripMenuItem.Name = "familySearchToolStripMenuItem";
            this.familySearchToolStripMenuItem.Click += new System.EventHandler(this.familySearchToolStripMenuItem_Click);
            // 
            // mnuDenniHlasatelSearch
            // 
            resources.ApplyResources(this.mnuDenniHlasatelSearch, "mnuDenniHlasatelSearch");
            this.mnuDenniHlasatelSearch.Name = "mnuDenniHlasatelSearch";
            this.mnuDenniHlasatelSearch.Click += new System.EventHandler(this.mnuDenniHlasatelSearch_Click);
            // 
            // denniHlasatelXMLToolStripMenuItem
            // 
            resources.ApplyResources(this.denniHlasatelXMLToolStripMenuItem, "denniHlasatelXMLToolStripMenuItem");
            this.denniHlasatelXMLToolStripMenuItem.Name = "denniHlasatelXMLToolStripMenuItem";
            this.denniHlasatelXMLToolStripMenuItem.Click += new System.EventHandler(this.denniHlasatelXMLToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchGivenNamesToolStripMenuItem,
            this.searchSurnamesToolStripMenuItem,
            this.listGivenNamesToolStripMenuItem,
            this.listSurnamesToolStripMenuItem,
            this.toolStripMenuItem4,
            this.unrecognizedGivenNamesToolStripMenuItem,
            this.givenNamesCheckToolStripMenuItem,
            this.givenNamesMergeToolStripMenuItem,
            this.convertDHFilesToolStripMenuItem,
            this.convertToJSONToolStripMenuItem,
            this.prijmeniConversionToolStripMenuItem,
            this.prijmeniRankCheckToolStripMenuItem,
            this.givenNamesTranslitToolStripMenuItem,
            this.toolStripMenuItem3,
            this.nextDHRecordToolStripMenuItem,
            this.kdeJsmeSurnameSearchToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            // 
            // searchGivenNamesToolStripMenuItem
            // 
            resources.ApplyResources(this.searchGivenNamesToolStripMenuItem, "searchGivenNamesToolStripMenuItem");
            this.searchGivenNamesToolStripMenuItem.Name = "searchGivenNamesToolStripMenuItem";
            this.searchGivenNamesToolStripMenuItem.Click += new System.EventHandler(this.searchGivenNamesToolStripMenuItem_Click);
            // 
            // searchSurnamesToolStripMenuItem
            // 
            resources.ApplyResources(this.searchSurnamesToolStripMenuItem, "searchSurnamesToolStripMenuItem");
            this.searchSurnamesToolStripMenuItem.Name = "searchSurnamesToolStripMenuItem";
            this.searchSurnamesToolStripMenuItem.Click += new System.EventHandler(this.searchSurnamesToolStripMenuItem_Click);
            // 
            // listGivenNamesToolStripMenuItem
            // 
            resources.ApplyResources(this.listGivenNamesToolStripMenuItem, "listGivenNamesToolStripMenuItem");
            this.listGivenNamesToolStripMenuItem.Name = "listGivenNamesToolStripMenuItem";
            this.listGivenNamesToolStripMenuItem.Click += new System.EventHandler(this.listGivenNamesToolStripMenuItem_Click);
            // 
            // listSurnamesToolStripMenuItem
            // 
            resources.ApplyResources(this.listSurnamesToolStripMenuItem, "listSurnamesToolStripMenuItem");
            this.listSurnamesToolStripMenuItem.Name = "listSurnamesToolStripMenuItem";
            this.listSurnamesToolStripMenuItem.Click += new System.EventHandler(this.listSurnamesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            // 
            // unrecognizedGivenNamesToolStripMenuItem
            // 
            resources.ApplyResources(this.unrecognizedGivenNamesToolStripMenuItem, "unrecognizedGivenNamesToolStripMenuItem");
            this.unrecognizedGivenNamesToolStripMenuItem.Name = "unrecognizedGivenNamesToolStripMenuItem";
            this.unrecognizedGivenNamesToolStripMenuItem.Click += new System.EventHandler(this.unrecognizedGivenNamesToolStripMenuItem_Click);
            // 
            // givenNamesCheckToolStripMenuItem
            // 
            resources.ApplyResources(this.givenNamesCheckToolStripMenuItem, "givenNamesCheckToolStripMenuItem");
            this.givenNamesCheckToolStripMenuItem.Name = "givenNamesCheckToolStripMenuItem";
            this.givenNamesCheckToolStripMenuItem.Click += new System.EventHandler(this.givenNamesCheckToolStripMenuItem_Click);
            // 
            // givenNamesMergeToolStripMenuItem
            // 
            resources.ApplyResources(this.givenNamesMergeToolStripMenuItem, "givenNamesMergeToolStripMenuItem");
            this.givenNamesMergeToolStripMenuItem.Name = "givenNamesMergeToolStripMenuItem";
            this.givenNamesMergeToolStripMenuItem.Click += new System.EventHandler(this.givenNamesMergeToolStripMenuItem_Click);
            // 
            // convertDHFilesToolStripMenuItem
            // 
            resources.ApplyResources(this.convertDHFilesToolStripMenuItem, "convertDHFilesToolStripMenuItem");
            this.convertDHFilesToolStripMenuItem.Name = "convertDHFilesToolStripMenuItem";
            this.convertDHFilesToolStripMenuItem.Click += new System.EventHandler(this.convertDHFilesToolStripMenuItem_Click);
            // 
            // convertToJSONToolStripMenuItem
            // 
            resources.ApplyResources(this.convertToJSONToolStripMenuItem, "convertToJSONToolStripMenuItem");
            this.convertToJSONToolStripMenuItem.Name = "convertToJSONToolStripMenuItem";
            this.convertToJSONToolStripMenuItem.Click += new System.EventHandler(this.convertToJSONToolStripMenuItem_Click);
            // 
            // prijmeniConversionToolStripMenuItem
            // 
            resources.ApplyResources(this.prijmeniConversionToolStripMenuItem, "prijmeniConversionToolStripMenuItem");
            this.prijmeniConversionToolStripMenuItem.Name = "prijmeniConversionToolStripMenuItem";
            this.prijmeniConversionToolStripMenuItem.Click += new System.EventHandler(this.prijmeniConversionToolStripMenuItem_Click);
            // 
            // prijmeniRankCheckToolStripMenuItem
            // 
            resources.ApplyResources(this.prijmeniRankCheckToolStripMenuItem, "prijmeniRankCheckToolStripMenuItem");
            this.prijmeniRankCheckToolStripMenuItem.Name = "prijmeniRankCheckToolStripMenuItem";
            this.prijmeniRankCheckToolStripMenuItem.Click += new System.EventHandler(this.prijmeniRankCheckToolStripMenuItem_Click);
            // 
            // givenNamesTranslitToolStripMenuItem
            // 
            resources.ApplyResources(this.givenNamesTranslitToolStripMenuItem, "givenNamesTranslitToolStripMenuItem");
            this.givenNamesTranslitToolStripMenuItem.Name = "givenNamesTranslitToolStripMenuItem";
            this.givenNamesTranslitToolStripMenuItem.Click += new System.EventHandler(this.givenNamesTranslitToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            // 
            // nextDHRecordToolStripMenuItem
            // 
            resources.ApplyResources(this.nextDHRecordToolStripMenuItem, "nextDHRecordToolStripMenuItem");
            this.nextDHRecordToolStripMenuItem.Name = "nextDHRecordToolStripMenuItem";
            this.nextDHRecordToolStripMenuItem.Click += new System.EventHandler(this.nextDHRecordToolStripMenuItem_Click);
            // 
            // kdeJsmeSurnameSearchToolStripMenuItem
            // 
            resources.ApplyResources(this.kdeJsmeSurnameSearchToolStripMenuItem, "kdeJsmeSurnameSearchToolStripMenuItem");
            this.kdeJsmeSurnameSearchToolStripMenuItem.Name = "kdeJsmeSurnameSearchToolStripMenuItem";
            this.kdeJsmeSurnameSearchToolStripMenuItem.Click += new System.EventHandler(this.kdeJsmeSurnameSearchToolStripMenuItem_Click);
            // 
            // tableToolStripMenuItem
            // 
            resources.ApplyResources(this.tableToolStripMenuItem, "tableToolStripMenuItem");
            this.tableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.denniHlasatelToolStripMenuItem,
            this.givenNameEquivalentsToolStripMenuItem,
            this.prijmeniToolStripMenuItem,
            this.krestniJmenaToolStripMenuItem});
            this.tableToolStripMenuItem.Name = "tableToolStripMenuItem";
            // 
            // denniHlasatelToolStripMenuItem
            // 
            resources.ApplyResources(this.denniHlasatelToolStripMenuItem, "denniHlasatelToolStripMenuItem");
            this.denniHlasatelToolStripMenuItem.Name = "denniHlasatelToolStripMenuItem";
            this.denniHlasatelToolStripMenuItem.Click += new System.EventHandler(this.denniHlasatelToolStripMenuItem_Click);
            // 
            // givenNameEquivalentsToolStripMenuItem
            // 
            resources.ApplyResources(this.givenNameEquivalentsToolStripMenuItem, "givenNameEquivalentsToolStripMenuItem");
            this.givenNameEquivalentsToolStripMenuItem.Name = "givenNameEquivalentsToolStripMenuItem";
            this.givenNameEquivalentsToolStripMenuItem.Click += new System.EventHandler(this.givenNameEquivalentsToolStripMenuItem_Click);
            // 
            // prijmeniToolStripMenuItem
            // 
            resources.ApplyResources(this.prijmeniToolStripMenuItem, "prijmeniToolStripMenuItem");
            this.prijmeniToolStripMenuItem.Name = "prijmeniToolStripMenuItem";
            this.prijmeniToolStripMenuItem.Click += new System.EventHandler(this.prijmeniToolStripMenuItem_Click);
            // 
            // krestniJmenaToolStripMenuItem
            // 
            resources.ApplyResources(this.krestniJmenaToolStripMenuItem, "krestniJmenaToolStripMenuItem");
            this.krestniJmenaToolStripMenuItem.Name = "krestniJmenaToolStripMenuItem";
            this.krestniJmenaToolStripMenuItem.Click += new System.EventHandler(this.krestniJmenaToolStripMenuItem_Click);
            // 
            // givenNameEquivalentsTableAdapter
            // 
            this.givenNameEquivalentsTableAdapter.ClearBeforeFill = true;
            // 
            // prijmeniBindingSource
            // 
            this.prijmeniBindingSource.DataMember = "Prijmeni";
            this.prijmeniBindingSource.DataSource = this.genealogyDataSet;
            // 
            // prijmeniTableAdapter
            // 
            this.prijmeniTableAdapter.ClearBeforeFill = true;
            // 
            // tmrAutoCompleteSurname
            // 
            this.tmrAutoCompleteSurname.Interval = 25;
            // 
            // krestniJmenaBindingSource
            // 
            this.krestniJmenaBindingSource.DataMember = "KrestniJmena";
            this.krestniJmenaBindingSource.DataSource = this.genealogyDataSet;
            // 
            // krestniJmenaTableAdapter
            // 
            this.krestniJmenaTableAdapter.ClearBeforeFill = true;
            // 
            // txtLastName
            // 
            resources.ApplyResources(this.txtLastName, "txtLastName");
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.TextChanged += new System.EventHandler(this.txtLastName_TextChanged);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.txtDate);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.givenNameEquivalentsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genealogyDataSet)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabBrowse.ResumeLayout(false);
            this.tabHits.ResumeLayout(false);
            this.tabText.ResumeLayout(false);
            this.tabText.PerformLayout();
            this.tabTree.ResumeLayout(false);
            this.tabdB.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dHDeathIndexBindingSource)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.prijmeniBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krestniJmenaBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabText;
        private System.Windows.Forms.TextBox txtResponse;
        private System.Windows.Forms.TabPage tabTree;
        private System.Windows.Forms.TreeView tvDocument;
        private System.Windows.Forms.TabPage tabBrowse;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.TabPage tabHits;
        private System.Windows.Forms.ListView lvHits;
        private System.Windows.Forms.TabPage tabdB;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn firstNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn middleNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn noticeDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn serialDataGridViewTextBoxColumn;
        private System.Windows.Forms.ImageList imgList1;
        private GenealogyDataSet genealogyDataSet;
        private System.Windows.Forms.BindingSource dHDeathIndexBindingSource;
        private GenealogyDataSetTableAdapters.DHDeathIndexTableAdapter dHDeathIndexTableAdapter;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tableToolStripMenuItem;
        private GenealogyDataSetTableAdapters.GivenNameEquivalentsTableAdapter givenNameEquivalentsTableAdapter;
        private System.Windows.Forms.ToolStripMenuItem denniHlasatelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem givenNameEquivalentsToolStripMenuItem;
        private System.Windows.Forms.BindingSource givenNameEquivalentsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn englishDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nativeDataGridViewTextBoxColumn;
        private System.Windows.Forms.ToolStripMenuItem webToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem illinoisDeathIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem familySearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem framesDumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertDHFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prijmeniConversionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem nextDHRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prijmeniRankCheckToolStripMenuItem;
        private System.Windows.Forms.BindingSource prijmeniBindingSource;
        private GenealogyDataSetTableAdapters.PrijmeniTableAdapter prijmeniTableAdapter;
        private System.Windows.Forms.ToolStripMenuItem prijmeniToolStripMenuItem;
        private System.Windows.Forms.Timer tmrAutoCompleteSurname;
        private System.Windows.Forms.ToolStripMenuItem listGivenNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listSurnamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem givenNamesTranslitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem givenNamesCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem givenNamesMergeToolStripMenuItem;
        private System.Windows.Forms.BindingSource krestniJmenaBindingSource;
        private GenealogyDataSetTableAdapters.KrestniJmenaTableAdapter krestniJmenaTableAdapter;
        private System.Windows.Forms.ToolStripMenuItem krestniJmenaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuDenniHlasatelSearch;
        private System.Windows.Forms.ToolStripMenuItem denniHlasatelXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataFilesFolderToolStripMenuItem;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.ToolStripMenuItem kdeJsmeSurnameSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchGivenNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchSurnamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unrecognizedGivenNamesToolStripMenuItem;
    }
}

