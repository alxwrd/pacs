using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;

namespace ppaocr
{
    [RunInstaller(true)]
    public partial class InstallerCleanup : Installer
    {
        public InstallerCleanup()
        {
            InitializeComponent();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            try
            {
                // Cleanup User's Application Data Folder since the normall installer
                // doesn't if there are files present in it.

                // Decided not to do this.  Rational was that the uninstaller should only
                // uninstall files that it installed.
/*                string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PPAOCR";
                if (Directory.Exists(appDataDir))
                {
                    foreach (string file in Directory.GetFiles(appDataDir))
                    {
                        if (File.Exists(file))
                            File.Delete(file);
                    }
                    Directory.Delete(appDataDir);
                }
                string fileName = (string)savedState["AddinPath"];
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
 */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
