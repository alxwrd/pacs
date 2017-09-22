// Copyright 2008-2009 Yuhu on Sage, MIT License
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Configuration;

namespace ppaocr
{
    public partial class Form1 : Form
    {

        Uploader m_uploader = new Uploader();
        // Keep track of original font smoothing state at startup so that we can
        // restore it upon exit.
        private bool fontSmoothingWasOnAtStartup = true;    
        private Options options;
        string appDataDir;
        private bool autoFontSmoothingWasOnAtStartup = true;

        public Form1()
        {
            InitializeComponent();
            appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PPAOCR";
            options = Options.Deserialize(appDataDir + "\\Options.xml");
            LoadServerInfo();
            if (options.ShowFontSmoothingWarning && options.IsAutoFontSmoothing)
            {
                // Show user warning about font smoothing
                MessageBox.Show("Font Smoothing will automatically be turned off when starting this application.  When the application exits it will then automatically restore the font smoothing state.  This automatic feature may be turned off in the Tools Options dialog of this application.  Font Smoothing is turned off because the Optical Character Recognition (OCR) algorithm in this application does not currently work when it is on.", Application.ProductName); 
                // Shown to user once, don't show again
                options.ShowFontSmoothingWarning = false;
                options.Serialize(appDataDir + "\\Options.xml");
            }

            UpdateUploadServer();
            UpdateDebugState();
            autoFontSmoothingWasOnAtStartup = options.IsAutoFontSmoothing;
            // Cache font smoothing state at startup
            fontSmoothingWasOnAtStartup = Utils.GetFontSmoothing();
            // Disable font smoothing if auto font smoothing option is on
            if (options.IsAutoFontSmoothing && Utils.GetFontSmoothing())
                Utils.DisableFontSmoothing();
            UpdateFontSmoothingButtons();
            UpdatePPWindowList();
        }
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        Dictionary<string, Commodity> allCommods = new Dictionary<string, Commodity>();
        List<BidData> bidData = new List<BidData>();

        private void LoadServerInfo()
        {
            comboBoxUploadServer.Items.Clear();

            int i = 1;
            while (ConfigurationSettings.AppSettings["ServerHomeUrl" + i.ToString()] != null)
            {
                UploadServer temp = new UploadServer();
                temp.HomeUrl = new Uri(ConfigurationSettings.AppSettings["ServerHomeUrl" + i.ToString()]);
                temp.UploadUrl = new Uri(ConfigurationSettings.AppSettings["ServerUploadUrl" + i.ToString()]);
                temp.ServerType = (UploadServerType)Convert.ToInt32(ConfigurationSettings.AppSettings["ServerType" + i.ToString()]);
                comboBoxUploadServer.Items.Add(temp);
                i++;
            }

            if (options.SelectedUploadServer.Length <= 0)   // Not initialized yet
            {
                UploadServer server = (UploadServer)comboBoxUploadServer.Items[0];
                options.SelectedUploadServer = server.HomeUrl.ToString();
                comboBoxUploadServer.SelectedIndex = 0;
            }
            else
            {
                // Find the selected upload server
                bool found = false;
                for (int j = 0; j < comboBoxUploadServer.Items.Count; j++)
                {
                    if (((UploadServer)comboBoxUploadServer.Items[j]).HomeUrl.ToString().Equals(options.SelectedUploadServer))
                    {
                        comboBoxUploadServer.SelectedIndex = j;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    comboBoxUploadServer.SelectedIndex = 0;
            }

        }

        private void UpdateUploadServer()
        {
            // Turn everything off, then selectively turn on applicable items on upload tab
            toolStripButtonWebHome.Visible = false;
            webBrowser.Url = null;
            webBrowser.Visible = false;
            buttonUpload.Visible = false;

            UploadServer server = (UploadServer)comboBoxUploadServer.SelectedItem;
            if (server.ServerType == UploadServerType.PCTB)
            {
                toolStripButtonWebHome.Visible = true;
                webBrowser.Visible = true;
                webBrowser.Url = server.UploadUrl;
            }
            else
                buttonUpload.Visible = true;

        }

        private void CaptureBidData()
        {
            try
            {
                CleanupDebugFiles();

                ProcessWrapper procWrap = comboBoxPPWindows.SelectedItem as ProcessWrapper;
                if (procWrap == null || procWrap.Process == null || procWrap.Process.HasExited)
                {
                    MessageBox.Show(this,"Can't find Puzzle Pirates window.",Application.ProductName);
                    return;
                }

                PPOcr myOcr = new PPOcr(procWrap.Process);

                string island;

                DateTime dtStart = DateTime.Now;

                if (allCommods != null) allCommods.Clear();
                bidData = myOcr.ExtractAllBidData(out island);

                this.BringToFront();
                this.Activate();

                if (myOcr.Error.Length > 0)
                    MessageBox.Show(this, myOcr.Error,Application.ProductName);

                if (island == null || island.Length <= 0)
                    labelTitle.Text = "Island: Unknown";
                else
                    labelTitle.Text = "Island: " + island;

                labelTitle.Text = labelTitle.Text + " on " + myOcr.Ocean;

                if (bidData == null)
                {
                    dataGridView1.DataSource = null;
                    labelText.Text = "Rows: 0";
                }
                else
                {
                    DateTime dtStop = DateTime.Now;

                    List<BidData> list = new List<BidData>(bidData);
                    dataGridView1.DataSource = list;
                    labelText.Text = "Rows: " + bidData.Count.ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this,ex.Message.ToString() + "\n" + "\n" + ex.StackTrace.ToString(),Application.ProductName);
            }

        }

        private void CaptureMarketData()
        {
            try
            {

                CleanupDebugFiles();

                ProcessWrapper procWrap = comboBoxPPWindows.SelectedItem as ProcessWrapper;
                if (procWrap == null || procWrap.Process == null || procWrap.Process.HasExited)
                {
                    MessageBox.Show(this,"Can't find Puzzle Pirates window.",Application.ProductName);
                    return;
                }

                PPOcr myOcr = new PPOcr(procWrap.Process);
                string island;

                DateTime dtStart = DateTime.Now;

                if (bidData != null) bidData.Clear();
                allCommods = myOcr.ExtractAllCommodities(out island);

                this.BringToFront();
                this.Activate();

                if (myOcr.Error.Length > 0)
                    MessageBox.Show(this, myOcr.Error,Application.ProductName);

                if (island == null || island.Length <= 0)
                    labelTitle.Text = "Island: Unknown";
                else
                    labelTitle.Text = "Island: " + island;

                labelTitle.Text = labelTitle.Text + " on " + myOcr.Ocean;

                if (allCommods == null)
                {
                    dataGridView1.DataSource = null;
                    labelText.Text = "Rows: 0";
                }
                else
                {
                    DateTime dtStop = DateTime.Now;
                    List<Commodity> list = new List<Commodity>(allCommods.Values);
                    dataGridView1.DataSource = list;
                    
                    labelText.Text = "Rows: " + allCommods.Count.ToString() + "  Rows/Sec: " + (Convert.ToDouble(allCommods.Count) / dtStop.Subtract(dtStart).TotalSeconds).ToString("N0");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this,ex.Message.ToString() + "\n" + "\n" + ex.StackTrace.ToString(), Application.ProductName);
            }

        }

        private void buttonCaptureMarketData_Click(object sender, EventArgs e)
        {
            CaptureMarketData();
        }

        private void buttonDisableFontSmoothing_Click(object sender, EventArgs e)
        {
            Utils.DisableFontSmoothing();
            UpdateFontSmoothingButtons();
        }

        private void buttonEnableFontSmoothing_Click(object sender, EventArgs e)
        {
            Utils.EnableFontSmoothing();
            UpdateFontSmoothingButtons();
        }

        private void UpdateFontSmoothingButtons()
        {
            if (Utils.GetFontSmoothing())
            {
                buttonEnableFontSmoothing.Enabled = false;
                buttonDisableFontSmoothing.Enabled = true;
            }
            else
            {
                buttonEnableFontSmoothing.Enabled = true;
                buttonDisableFontSmoothing.Enabled = false;
            }
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.Equals("about:upload"))
            {
                if ((allCommods == null || allCommods.Count <= 0) && (bidData == null || bidData.Count <= 0) )
                {
                    e.Cancel = true;
                    MessageBox.Show(this,"No Commodities to upload.  Try capturing market or bid data first.", Application.ProductName);
                    return;
                }

                if (bidData != null && bidData.Count > 0)
                {
                    if (!m_uploader.IsInitialized)
                        m_uploader.Initialize((UploadServer)comboBoxUploadServer.SelectedItem);

                    m_uploader.Upload(bidData, (UploadServer)comboBoxUploadServer.SelectedItem, webBrowser);

                    if (m_uploader.Error.Length > 0)
                    {
                        e.Cancel = true;
                        MessageBox.Show(this,"Error: " + m_uploader.Error, Application.ProductName);
                        return;
                    }

                }

                if (allCommods != null && allCommods.Count > 0)
                {
                    if (!m_uploader.IsInitialized)
                        m_uploader.Initialize((UploadServer)comboBoxUploadServer.SelectedItem);

                    m_uploader.Upload(allCommods, (UploadServer)comboBoxUploadServer.SelectedItem, webBrowser);
                 // Avoid sending MessageBox here because it will stop the upload
                 // page from proceeding properly if we are just warning about
                 // missing commodities.  Display the error message in Document Completed
                 // event instead.
                 //   if (m_uploader.Error.Length > 0)
                 //       MessageBox.Show(this,"Error: " + m_uploader.Error);
                }
            }
            else if (e.Url.Equals("about:quit"))
                this.Close();
            else if (e.Url.Equals("about:redo"))
            {
                CaptureMarketData();
                e.Cancel = true;
            }
            else if (e.Url.ToString().Contains("action=setisland") && !e.Url.ToString().Contains("island="))
            {
                // Error must contain island selection (i.e. island=<#>)
                MessageBox.Show(this,"Error: No island selected.  \n\nPlease report this error and any possible steps to reproduce it by submitting a defect to http://www.sourceforge.net/projects/pctb2.\n\nDetails:\n\n" + e.Url.ToString(),Application.ProductName);
                e.Cancel = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateDebugState();
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
        }


        private void UpdateDebugState()
        {
            if (options.IsDebugMode)
            {
    
                Utils.traceSwitch.Level = TraceLevel.Verbose;
                if (Directory.Exists("Test"))
                {
                    mnuToolsRunChecks.Visible = true;
                    mnuToolsOCRBitmapFile.Visible = true;
                }
                else
                {
                    mnuToolsRunChecks.Visible = false;
                    mnuToolsOCRBitmapFile.Visible = false;
                }
                
            }
            else
            {
                Utils.traceSwitch.Level = TraceLevel.Off;
                mnuToolsRunChecks.Visible = false;
                mnuToolsOCRBitmapFile.Visible = false;
            }
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            About.ShowAboutForm(this);

        }

        
        private void buttonTest_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If auto font smoothing is on then ensure it gets restored
            // to the state it was in when the app started.
            if (options.IsAutoFontSmoothing)
            {
                bool fontSmoothingEnabled = Utils.GetFontSmoothing();
                if (fontSmoothingWasOnAtStartup != fontSmoothingEnabled)
                {
                    if (fontSmoothingWasOnAtStartup)
                        Utils.EnableFontSmoothing();
                    else
                        Utils.DisableFontSmoothing();
                }
            }

        }

        private void mnuToolsOptions_Click(object sender, EventArgs e)
        {
            EditOptions optionsEditor = new EditOptions(ref options);
            if (optionsEditor.ShowDialog() == DialogResult.OK)
            {
                // Persist settings
                options.Serialize(appDataDir + "\\Options.xml");
                UpdateDebugState();
                // Turn off font smoothing if auto font smoothing is enabled
                if (options.IsAutoFontSmoothing && Utils.GetFontSmoothing())
                {
                    Utils.DisableFontSmoothing();
                    UpdateFontSmoothingButtons();
                }
            }

        }

        private void CleanupDebugFiles()
        {
            try
            {
                string file = appDataDir + "\\CaptureBidError.tif";
                if (File.Exists(file))
                    File.Delete(file);

                file = appDataDir + "\\CaptureBid.tif";
                if (File.Exists(file))
                    File.Delete(file);

                file = appDataDir + "\\CaptureError.tif";
                if (File.Exists(file))
                    File.Delete(file);

                file = appDataDir + "\\Capture.tif";
                if (File.Exists(file))
                    File.Delete(file);

                file = appDataDir + "\\ShopNames.txt";
                if (File.Exists(file))
                    File.Delete(file);

                file = appDataDir + "\\Commodities.txt";
                if (File.Exists(file))
                    File.Delete(file);

                file = appDataDir + "\\IslandCapture.tif";
                if (File.Exists(file))
                    File.Delete(file);

                file = appDataDir + "\\IslandCell.tif";
                if (File.Exists(file))
                    File.Delete(file);

                string listenerFile = appDataDir + "\\DebugOutput.log";
                if (File.Exists(listenerFile))
                    File.Delete(listenerFile);
            }
            catch { }
        }

        private void buttonCaptureBidData_Click(object sender, EventArgs e)
        {
            CaptureBidData();
        }


        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (m_uploader.Error.Length > 0)
            {
                MessageBox.Show(this, "Error: " + m_uploader.Error, Application.ProductName);
                m_uploader.Error = "";
            }

        }

        private void toolStripButtonWebHome_Click(object sender, EventArgs e)
        {
            webBrowser.Url = ((UploadServer)comboBoxUploadServer.SelectedItem).UploadUrl;
        }

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
                CaptureBidData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void UpdatePPWindowList()
        {
            List<Process> ppProcesses = new List<Process>();

            // Find puzzle pirates windows

            // Make a guess for the process name
            Process[] processes = Process.GetProcessesByName("javaw");
            foreach (Process p in processes)
                if (p.MainWindowTitle.Contains("Puzzle Pirates -"))
                    ppProcesses.Add(p);

            if (ppProcesses.Count <= 0)
            {
                // Guess was wrong so go looking by main window title only
                foreach (Process p in Process.GetProcesses())
                {
                    if (p != null && p.MainWindowTitle.Contains("Puzzle Pirates -"))
                        ppProcesses.Add(p);
                }

            }

            ProcessWrapper lastSelected = null;
            if (comboBoxPPWindows.Items.Count > 0)
                lastSelected = comboBoxPPWindows.SelectedItem as ProcessWrapper;

            comboBoxPPWindows.Items.Clear();
            foreach (Process p in ppProcesses)
            {
                comboBoxPPWindows.Items.Add(new ProcessWrapper(p));
                
            }
            if (comboBoxPPWindows.Items.Count > 0)
            {
                // Retain previous selection if possible, otherwise set to first item
                    comboBoxPPWindows.SelectedIndex = 0;
                if (lastSelected != null)
                {
                    ProcessWrapper p;
                    for(int i=0; i < comboBoxPPWindows.Items.Count; i++)
                    {
                        p = comboBoxPPWindows.Items[i] as ProcessWrapper;
                        if (p != null && p.ToString().Equals(lastSelected.ToString()))
                        {
                            comboBoxPPWindows.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdatePPWindowList();
        }

        private void mnuToolsRunChecks_Click(object sender, EventArgs e)
        {
            Cursor c = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            Test test = new Test();
            List<TestResult> results = new List<TestResult>();
            results.AddRange(test.CheckIslandNames());
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Island_Full_800x600.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Island_Part_800x600.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Ship_Full_800x600.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Island_Full_1024x768.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Island_Full_800x600_Tigerleaf.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Island_Full_800x600_Kasidim.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Island_Full_800x600_Kasidim2.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Ship_NoScroll.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractor("Market_Ship_Scroll.xml"));
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;
            results.AddRange(test.CheckCommodityExtractorMemory());
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = results;

            Cursor.Current = c;

        }

        private void mnuToolsOCRBitmapFile_Click(object sender, EventArgs e)
        {

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CleanupDebugFiles();

                PPOcr ppocr = new PPOcr(null);

                UnsafeBitmap bm = new UnsafeBitmap(openFileDialog.FileName);

                ppocr.ClearErrors();

                Rectangle commodArea = ppocr.FindCommodityArea(bm);
                string island = ppocr.ExtractIslandName(bm);

                if (island == null || island.Length <= 0)
                    MessageBox.Show(this,"Can't find island name", Application.ProductName);

                labelTitle.Text = "Island: " + island;

                List<Commodity> commodities = ppocr.ExtractCommodities(bm, commodArea);
                if (ppocr.Error.Length > 0)
                    MessageBox.Show(this,ppocr.Error,Application.ProductName);

                if (commodities == null || commodities.Count <= 0)
                    dataGridView1.DataSource = null;
                else
                    dataGridView1.DataSource = new List<Commodity>(commodities);

            }
        }

        private void comboBoxUploadServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUploadServer();
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            // Should only be called when upload server type is YARRG
            
            if ((allCommods == null || allCommods.Count <= 0) && (bidData == null || bidData.Count <= 0))
            {
                MessageBox.Show(this, "No Commodities to upload.  Try capturing market or bid data first.", Application.ProductName);
                return;
            }

            if (bidData != null && bidData.Count > 0)
            {
                MessageBox.Show(this, "Bid data is not supported by the currently selected server.", Application.ProductName);
                return;

                //m_uploader.Upload(bidData, webBrowser);

                //if (m_uploader.Error.Length > 0)
                //{
                //    MessageBox.Show(this, "Error: " + m_uploader.Error, Application.ProductName);
                //    return;
                //}

            }

            if (allCommods != null && allCommods.Count > 0)
            {
                m_uploader.Upload(allCommods, (UploadServer)comboBoxUploadServer.SelectedItem, webBrowser);

                if (m_uploader.Error.Length > 0)
                {
                    MessageBox.Show(this, "Error: " + m_uploader.Error, Application.ProductName);
                    return;
                }
            }

        }




    }
}
