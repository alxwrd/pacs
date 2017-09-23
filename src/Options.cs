// Copyright 2008-2009 Yuhu on Sage, MIT License
using System;
using System.Xml.Serialization;
using System.IO;

namespace ppaocr
{
    /// <summary>
    /// Summary description for Options.
    /// </summary>
    [Serializable]
    public class Options
    {

        private bool isDebugMode = false;
        private bool isAutoFontSmoothing = true;
        private bool showFontSmoothingWarning = true;
        private string selectedUploadServer = "";

        public Options()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [XmlElementAttribute("IsDebugMode")]
        public bool IsDebugMode
        {
            get { return isDebugMode; }
            set { isDebugMode = value; }
        }

        [XmlElementAttribute("IsAutoFontSmoothing")]
        public bool IsAutoFontSmoothing
        {
            get { return isAutoFontSmoothing; }
            set { isAutoFontSmoothing = value; }
        }

        [XmlElementAttribute("ShowFontSmoothingWarning")]
        public bool ShowFontSmoothingWarning
        {
            get { return showFontSmoothingWarning; }
            set { showFontSmoothingWarning = value; }
        }

        [XmlElementAttribute("SelectedUploadServer")]
        public string SelectedUploadServer
        {
            get { return selectedUploadServer; }
            set { selectedUploadServer = value; }
        }

        public void Serialize(string file)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(Options));

            TextWriter writer = new StreamWriter(file);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static Options Deserialize(string file)
        {
            if (File.Exists(file) == false)
                return new Options();

            XmlSerializer serializer = new XmlSerializer(typeof(Options));

            TextReader reader = new StreamReader(file);
            if (reader == null)
                return null;

            Options options = new Options();
            options = (Options)serializer.Deserialize(reader);

            reader.Close();
            return options;
        }

    }
}
