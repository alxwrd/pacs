using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

namespace ppaocr
{
    public class MyFont
    {
        FontInfo info;
        SortedDictionary<char, BmpChar> refBmpChars1 = new SortedDictionary<char, BmpChar>(new AlphabetFrequencyComparer());
        SortedDictionary<char, BmpChar> refBmpNumChars1 = new SortedDictionary<char, BmpChar>(new AlphabetFrequencyComparer());
        SortedDictionary<char, BinaryPixelBitmapChar> refBinBmpChars = new SortedDictionary<char, BinaryPixelBitmapChar>(new AlphabetFrequencyComparer());
        SortedDictionary<char, BinaryPixelBitmapChar> refBinBmpNumChars = new SortedDictionary<char, BinaryPixelBitmapChar>(new AlphabetFrequencyComparer());

        public MyFont(string font)
        {
            // Load reference bit map of characters for Font #1
            UnsafeBitmap bmChars = new UnsafeBitmap(font+"Characters.bmp");

            info = FontInfo.Deserialize(font + "Info.xml");
            // Create list of reference bitmaps (one for each character) for Font #1
            // Used for commodities and island name (when on ship)
            // Order of characters in FontInfo XML file must match the order in the reference bitmap
            refBmpChars1.Clear();
            refBmpNumChars1.Clear();
            int temp;
            for (int i = 0; i < info.CharInfos.Count; i++)
            {
                refBmpChars1.Add(info.CharInfos[i].Character[0], new BmpChar(GetReferenceCharBmp(bmChars, i), info.CharInfos[i].Character[0]));
                refBinBmpChars.Add(info.CharInfos[i].Character[0], new BinaryPixelBitmapChar(GetReferenceCharBmp(bmChars, i), info.CharInfos[i]));
                if (int.TryParse(info.CharInfos[i].Character, out temp) || info.CharInfos[i].Character[0].Equals('>') || info.CharInfos[i].Character[0].Equals(',') || info.CharInfos[i].Character[0].Equals('.'))
                {
                    refBmpNumChars1.Add(info.CharInfos[i].Character[0], refBmpChars1[info.CharInfos[i].Character[0]]);
                    refBinBmpNumChars.Add(info.CharInfos[i].Character[0], refBinBmpChars[info.CharInfos[i].Character[0]]);
                }
            }
        }

        public SortedDictionary<char, BinaryPixelBitmapChar> AllBinChars
        {
            get { return refBinBmpChars; }
        }

        public SortedDictionary<char, BinaryPixelBitmapChar> NumericalBinChars
        {
            get { return refBinBmpNumChars; }
        }

        private UnsafeBitmap GetReferenceCharBmp(UnsafeBitmap bmChars, int index)
        {
            if (index * 16 >= bmChars.Width)
                return null;


            return bmChars.Clone(new Rectangle(index * 16 + 1, 0, 15, bmChars.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }

    }

    [Serializable]
    [XmlRootAttribute("FontInfo", Namespace = "", IsNullable = false)]
    public class FontInfo
    {
        private int tracking = 0;
        private List<CharInfo> charInfos = new List<CharInfo>();
        private Dictionary<string, CharInfo> charInfosDict = new Dictionary<string, CharInfo>();

        [XmlElementAttribute("Tracking")]
        public int Tracking
        {
            get { return tracking; }
            set { tracking = value; }
        }

        public List<CharInfo> CharInfos
        {
            set { charInfos = value; }
            get { return charInfos; }
        }

        [XmlIgnore]
        public Dictionary<string, CharInfo> CharInfosDict
        {
            get 
            {
                // Build dictionary
                if (charInfosDict.Count <= 0)
                {
                    foreach (CharInfo info in charInfos)
                        charInfosDict.Add(info.Character, info);
                }

                return charInfosDict; 
            }
        }

        public void Serialize(string file)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(FontInfo));

            TextWriter writer = new StreamWriter(file);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static FontInfo Deserialize(string file)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            if (File.Exists(file) == false)
                return null;

            XmlSerializer serializer = new XmlSerializer(typeof(FontInfo));

            TextReader reader = new StreamReader(file);
            if (reader == null)
                return null;

            FontInfo info = new FontInfo();
            info = (FontInfo)serializer.Deserialize(reader);
            foreach (CharInfo ci in info.CharInfos)
                ci.Tracking = info.Tracking;

            reader.Close();
            return info;
        }

    }

    public class CharInfo
    {
        int kerningMin = 0;
        int kerningMax = 0;
        string character;
        int tracking = 3;

        [XmlElementAttribute("Character")]
        public string Character
        {
            get { return character; }
            set { character = value; }
        }

        [XmlElementAttribute("KerningMin")]
        public int KerningMin
        {
            get { return kerningMin; }
            set { kerningMin = value; }
        }

        [XmlElementAttribute("KerningMax")]
        public int KerningMax
        {
            get { return kerningMax; }
            set { kerningMax = value; }
        }

        [XmlIgnore]
        public int Tracking
        {
            get { return tracking; }
            set { tracking = value; }
        }

    }
}
