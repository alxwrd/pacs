// Copyright 2008-2009 Yuhu on Sage, MIT License
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace ppaocr
{
    /// <summary>
    /// Summary description for Options.
    /// </summary>
    public class EditOptions : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.TabControl tabControl1;
        private TabPage tabPage1;
        private CheckBox checkBoxEnableDebugMode;
        private CheckBox checkBoxAutoFontSmoothing;
        private HelpProvider helpProvider1;
        private Options options;

        public EditOptions(ref Options myOptions)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            options = myOptions;
            RefreshDisplay();
        }

        public void RefreshDisplay()
        {
            checkBoxAutoFontSmoothing.Checked = options.IsAutoFontSmoothing;
            checkBoxEnableDebugMode.Checked = options.IsDebugMode;

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
    #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxEnableDebugMode = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoFontSmoothing = new System.Windows.Forms.CheckBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(123, 256);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(211, 256);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(8, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(280, 240);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxEnableDebugMode);
            this.tabPage1.Controls.Add(this.checkBoxAutoFontSmoothing);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(272, 214);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Options";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableDebugMode
            // 
            this.checkBoxEnableDebugMode.AutoSize = true;
            this.helpProvider1.SetHelpKeyword(this.checkBoxEnableDebugMode, "");
            this.helpProvider1.SetHelpString(this.checkBoxEnableDebugMode, "If checked then various debug information is logged to files in the AppData\\PPAOC" +
                    "R directory.  This can be useful for tracking down defects in the application.  " +
                    "Normally this should be left unchecked.");
            this.checkBoxEnableDebugMode.Location = new System.Drawing.Point(8, 62);
            this.checkBoxEnableDebugMode.Name = "checkBoxEnableDebugMode";
            this.helpProvider1.SetShowHelp(this.checkBoxEnableDebugMode, true);
            this.checkBoxEnableDebugMode.Size = new System.Drawing.Size(124, 17);
            this.checkBoxEnableDebugMode.TabIndex = 10;
            this.checkBoxEnableDebugMode.Text = "Enable Debug Mode";
            this.checkBoxEnableDebugMode.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoFontSmoothing
            // 
            this.helpProvider1.SetHelpString(this.checkBoxAutoFontSmoothing, "If checked then font smoothing will automatically be turned off when this applica" +
                    "tion is started and then restored to its previous state when the application is " +
                    "exited. ");
            this.checkBoxAutoFontSmoothing.Location = new System.Drawing.Point(8, 14);
            this.checkBoxAutoFontSmoothing.Name = "checkBoxAutoFontSmoothing";
            this.helpProvider1.SetShowHelp(this.checkBoxAutoFontSmoothing, true);
            this.checkBoxAutoFontSmoothing.Size = new System.Drawing.Size(260, 42);
            this.checkBoxAutoFontSmoothing.TabIndex = 2;
            this.checkBoxAutoFontSmoothing.Text = "Automatically Disable/Restore Font Smoothing";
            // 
            // EditOptions
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 285);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditOptions";
            this.Text = "Edit Options";
            this.Load += new System.EventHandler(this.EditOptions_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion


        private void buttonOk_Click(object sender, System.EventArgs e)
        {
            if (options != null)
            {
                options.IsAutoFontSmoothing = checkBoxAutoFontSmoothing.Checked;
                options.IsDebugMode = checkBoxEnableDebugMode.Checked;

            }
        }

        private void EditOptions_Load(object sender, EventArgs e)
        {

        }

    }
}
