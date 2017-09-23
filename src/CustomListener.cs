using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ppaocr
{
    public class CustomListener : TextWriterTraceListener
    {
        public CustomListener() : base(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PPAOCR\\DebugOutput.log")
        {
            
        }
    }
}
