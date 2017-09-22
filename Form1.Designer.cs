namespace ppaocr
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonCaptureMarketData = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelText = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBoxPPWindows = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonCaptureBidData = new System.Windows.Forms.Button();
            this.buttonEnableFontSmoothing = new System.Windows.Forms.Button();
            this.buttonDisableFontSmoothing = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonWebHome = new System.Windows.Forms.ToolStripButton();
            this.comboBoxUploadServer = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsRunChecks = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOCRBitmapFile = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonUpload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCaptureMarketData
            // 
            this.buttonCaptureMarketData.Location = new System.Drawing.Point(12, 14);
            this.buttonCaptureMarketData.Name = "buttonCaptureMarketData";
            this.buttonCaptureMarketData.Size = new System.Drawing.Size(145, 37);
            this.buttonCaptureMarketData.TabIndex = 1;
            this.buttonCaptureMarketData.Text = "Capture Market Data";
            this.buttonCaptureMarketData.UseVisualStyleBackColor = true;
            this.buttonCaptureMarketData.Click += new System.EventHandler(this.buttonCaptureMarketData_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(706, 271);
            this.dataGridView1.TabIndex = 2;
            // 
            // labelText
            // 
            this.labelText.AutoSize = true;
            this.labelText.Location = new System.Drawing.Point(489, 40);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(37, 13);
            this.labelText.TabIndex = 4;
            this.labelText.Text = "Rows:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.comboBoxPPWindows);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.labelTitle);
            this.panel1.Controls.Add(this.labelText);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(720, 74);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(587, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxPPWindows
            // 
            this.comboBoxPPWindows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPPWindows.FormattingEnabled = true;
            this.comboBoxPPWindows.Location = new System.Drawing.Point(133, 7);
            this.comboBoxPPWindows.Name = "comboBoxPPWindows";
            this.comboBoxPPWindows.Size = new System.Drawing.Size(448, 21);
            this.comboBoxPPWindows.TabIndex = 6;
            this.comboBoxPPWindows.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Puzzle Pirates Window:";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(8, 35);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(142, 20);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Island: Unknown";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.buttonCaptureBidData);
            this.panel2.Controls.Add(this.buttonEnableFontSmoothing);
            this.panel2.Controls.Add(this.buttonDisableFontSmoothing);
            this.panel2.Controls.Add(this.buttonCaptureMarketData);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 401);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(720, 63);
            this.panel2.TabIndex = 3;
            // 
            // buttonCaptureBidData
            // 
            this.buttonCaptureBidData.Location = new System.Drawing.Point(178, 14);
            this.buttonCaptureBidData.Name = "buttonCaptureBidData";
            this.buttonCaptureBidData.Size = new System.Drawing.Size(145, 37);
            this.buttonCaptureBidData.TabIndex = 9;
            this.buttonCaptureBidData.Text = "Capture Bid Data";
            this.buttonCaptureBidData.UseVisualStyleBackColor = true;
            this.buttonCaptureBidData.Click += new System.EventHandler(this.buttonCaptureBidData_Click);
            // 
            // buttonEnableFontSmoothing
            // 
            this.buttonEnableFontSmoothing.Location = new System.Drawing.Point(556, 37);
            this.buttonEnableFontSmoothing.Name = "buttonEnableFontSmoothing";
            this.buttonEnableFontSmoothing.Size = new System.Drawing.Size(152, 23);
            this.buttonEnableFontSmoothing.TabIndex = 7;
            this.buttonEnableFontSmoothing.Text = "Enable Font Smoothing";
            this.buttonEnableFontSmoothing.UseVisualStyleBackColor = true;
            this.buttonEnableFontSmoothing.Click += new System.EventHandler(this.buttonEnableFontSmoothing_Click);
            // 
            // buttonDisableFontSmoothing
            // 
            this.buttonDisableFontSmoothing.Location = new System.Drawing.Point(556, 7);
            this.buttonDisableFontSmoothing.Name = "buttonDisableFontSmoothing";
            this.buttonDisableFontSmoothing.Size = new System.Drawing.Size(152, 23);
            this.buttonDisableFontSmoothing.TabIndex = 6;
            this.buttonDisableFontSmoothing.Text = "Disable Font Smoothing";
            this.buttonDisableFontSmoothing.UseVisualStyleBackColor = true;
            this.buttonDisableFontSmoothing.Click += new System.EventHandler(this.buttonDisableFontSmoothing_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 98);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(720, 303);
            this.tabControl1.TabIndex = 4;
            this.tabControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabControl1_KeyDown);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(712, 277);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Data";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.buttonUpload);
            this.tabPage2.Controls.Add(this.webBrowser);
            this.tabPage2.Controls.Add(this.toolStrip1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(712, 277);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Upload";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(3, 28);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.AllowNavigation = true;
            this.webBrowser.Size = new System.Drawing.Size(706, 246);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.Url = new System.Uri("", System.UriKind.Relative);
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            this.webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser_Navigated);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonWebHome,
            this.comboBoxUploadServer});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(706, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonWebHome
            // 
            this.toolStripButtonWebHome.Image = global::ppaocr.Properties.Resources.Home2;
            this.toolStripButtonWebHome.Name = "toolStripButtonWebHome";
            this.toolStripButtonWebHome.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonWebHome.Text = "Home";
            this.toolStripButtonWebHome.ToolTipText = "Home";
            this.toolStripButtonWebHome.Click += new System.EventHandler(this.toolStripButtonWebHome_Click);
            // 
            // comboBoxUploadServer
            // 
            this.comboBoxUploadServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxUploadServer.Name = "comboBoxUploadServer";
            this.comboBoxUploadServer.Size = new System.Drawing.Size(300, 25);
            this.comboBoxUploadServer.SelectedIndexChanged += new System.EventHandler(this.comboBoxUploadServer_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(52, 22);
            this.toolStripLabel1.Text = "Address:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem1,
            this.helpToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(720, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem1
            // 
            this.toolsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsOptions,
            this.mnuToolsRunChecks,
            this.mnuToolsOCRBitmapFile});
            this.toolsToolStripMenuItem1.Name = "toolsToolStripMenuItem1";
            this.toolsToolStripMenuItem1.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem1.Text = "Tools";
            // 
            // mnuToolsOptions
            // 
            this.mnuToolsOptions.Name = "mnuToolsOptions";
            this.mnuToolsOptions.Size = new System.Drawing.Size(169, 22);
            this.mnuToolsOptions.Text = "Options...";
            this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
            // 
            // mnuToolsRunChecks
            // 
            this.mnuToolsRunChecks.Name = "mnuToolsRunChecks";
            this.mnuToolsRunChecks.Size = new System.Drawing.Size(169, 22);
            this.mnuToolsRunChecks.Text = "Run Checks";
            this.mnuToolsRunChecks.Click += new System.EventHandler(this.mnuToolsRunChecks_Click);
            // 
            // mnuToolsOCRBitmapFile
            // 
            this.mnuToolsOCRBitmapFile.Name = "mnuToolsOCRBitmapFile";
            this.mnuToolsOCRBitmapFile.Size = new System.Drawing.Size(169, 22);
            this.mnuToolsOCRBitmapFile.Text = "OCR Bitmap File...";
            this.mnuToolsOCRBitmapFile.Click += new System.EventHandler(this.mnuToolsOCRBitmapFile_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem1.Text = "Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(116, 22);
            this.mnuHelpAbout.Text = "About...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(86, 22);
            this.toolStripLabel2.Text = "toolStripLabel2";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "IslandCapture.tif";
            this.openFileDialog.Filter = "Bitmap|*.tif";
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(3, 28);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(75, 23);
            this.buttonUpload.TabIndex = 2;
            this.buttonUpload.Text = "Upload";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 464);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Puzzle Pirates Automated OCR for use with PCTB";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCaptureMarketData;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button buttonEnableFontSmoothing;
        private System.Windows.Forms.Button buttonDisableFontSmoothing;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Button buttonCaptureBidData;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonWebHome;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOptions;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ComboBox comboBoxPPWindows;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsRunChecks;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOCRBitmapFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripComboBox comboBoxUploadServer;
        private System.Windows.Forms.Button buttonUpload;
    }
}

