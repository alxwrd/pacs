using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

namespace ppaocr
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label labelTextArea;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            this.labelTextArea = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelTextArea
            // 
            this.labelTextArea.Location = new System.Drawing.Point(24, 24);
            this.labelTextArea.Name = "labelTextArea";
            this.labelTextArea.Size = new System.Drawing.Size(248, 241);
            this.labelTextArea.TabIndex = 0;
            this.labelTextArea.Text = "label1";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(98, 268);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // About
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(288, 303);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.labelTextArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "About";
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void About_Load(object sender, System.EventArgs e)
		{
			Assembly ThisAssembly = Assembly.GetExecutingAssembly();

			AssemblyName ThisAssemblyName = ThisAssembly.GetName();

			string FriendlyVersion = ThisAssemblyName.Version.Major.ToString() + "." + ThisAssemblyName.Version.Minor.ToString();
			Array Attributes = ThisAssembly.GetCustomAttributes( false );

			string Title = "Unknown Application";
			string Copyright = "Unknown Copyright";
			foreach ( object o in Attributes )
			{
				if ( o is AssemblyTitleAttribute )
				{
					Title = ((AssemblyTitleAttribute)o).Title;
				}
				else if ( o is AssemblyCopyrightAttribute )
				{
					Copyright = ((AssemblyCopyrightAttribute)o).Copyright;
				}
			}

			this.Text = "About " + Title;

			StringBuilder sb = new StringBuilder("");

			sb.Append( Title );
			sb.Append(" Version ");
			sb.Append( FriendlyVersion );
			sb.Append( " (" );
			sb.Append( ThisAssemblyName.Version.ToString()	);
			sb.Append( ")\n\n" );
            sb.Append("Pirates Automatic Commodity Scraper (PACS)\n\n");
            sb.Append(Copyright);
            sb.Append("\n\n");
            sb.Append("Credits:\n\n");
            sb.Append("   Original PPAOCR: Yuhu, Brandishwar Copyright ©  2008 - 2009\n");

			labelTextArea.Text = sb.ToString();

		}

		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		internal static void ShowAboutForm( IWin32Window Owner )
		{
			About form = new About();
			form.ShowDialog( Owner );
		}

	}
}
