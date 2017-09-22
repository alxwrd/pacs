// Copyright 2008-2009 Yuhu on Sage, MIT License
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace ppaocr
{
    class TestResult
    {
        string val;
        string test;
        string duration;

        public TestResult(string test, string result)
        {
            this.val = result;
            this.test = test;
        }

        public string Test
        {
            get { return test; }
        }

        public string Result
        {
            get { return val; }
        }

        public string Duration
        {
            get { return duration; }
            set { duration = value; }
        }
    }

    [Serializable]
    [XmlRootAttribute("CaptureInfo", Namespace = "", IsNullable = false)]
    public class CaptureInfo
    {
        List<Commodity> commodities = new List<Commodity>();
        private string island = "";
        private string bitmapFile = "";

        [XmlElementAttribute("BitmapFile")]
        public string BitmapFile
        {
            set { bitmapFile = value; }
            get { return bitmapFile; }
        }

        [XmlElementAttribute("Island")]
        public string Island
        {
            set { island = value; }
            get { return island; }
        }
        
        private Rectangle commodityRect = new Rectangle();
        [XmlElementAttribute("CommodityRect")]
        public Rectangle CommodityRect
        {
            set { commodityRect = value; }
            get { return commodityRect; }
        }

        public List<Commodity> Commodities
        {
            set { commodities = value; }
            get { return commodities; }
        }

        public void Serialize(string file)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(CaptureInfo));

            TextWriter writer = new StreamWriter(file);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static CaptureInfo Deserialize(string file)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            if (File.Exists(file) == false)
                return null;

            XmlSerializer serializer = new XmlSerializer(typeof(CaptureInfo));

            TextReader reader = new StreamReader(file);
            if (reader == null)
                return null;

            CaptureInfo info = new CaptureInfo();
            info = (CaptureInfo)serializer.Deserialize(reader);

            reader.Close();
            return info;
        }


    }

    class Test
    {
        Ocr ocr = new Ocr();
        PPOcr ppocr = new PPOcr(null);

        public List<TestResult> CheckIslandNames()
        {
            UnsafeBitmap bm = new UnsafeBitmap("Test\\IslandNames.tif");

            string[] islandNames = {"Wensleydale", "Ventress", "Terjit", "Squibnocket", "Spaniel", "Rowes",
                                   "Penobscot", "Morannon", "Mirage", "Lincoln", "Isle of Kent", "Jack's Last Gift",
                                   "Halley", "Greenwich", "Fluke", "Descartes Isle", "Caravanserai", "Blackthorpe",
                                   "Barbary", "Frond","Islay of Luthien","Epsilon","Eta","Alpha", "Namath",
                                   "Oyster", "Vernal Equinox", "Xi", "Zeta", "Uxmal", "Quetzal","Yax Mutal", 
                                   "Swampfen", "Spectre","Harmattan","Kirin","Typhoon"};

            List<TestResult> results = new List<TestResult>();

            int rowHeight = 15;
            int i = 0;
            int islandIndex = 0;
            while (i < bm.Height && islandIndex < islandNames.Length)
            {
                BinaryPixelBitmap binRow = new BinaryPixelBitmap(bm.CloneAsBin(new Rectangle(0, i, bm.Width, rowHeight), BinPixelConvertType.ColorIsZero, bm.GetPixel(0,i)), rowHeight);
                string island = ocr.ExtractText(binRow, FontType.Font2All);

                // Remove the word "Island" or ":" and anything after it from the name and trim
                // white space from left and right
                int index = island.IndexOf("Island");
                index = index > 0 ? index : island.Length;
                island = island.Substring(0, index).Trim();
                index = island.IndexOf(":");
                index = index > 0 ? index : island.Length;
                island = island.Substring(0, index).Trim();
                if (!island.Equals(islandNames[islandIndex]))
                    results.Add(new TestResult("IslandNames", "Expected '" + islandNames[islandIndex] + "', got '" + island + "'"));
                i += rowHeight;
                islandIndex = i / rowHeight;
            }

            if (results.Count <= 0)
                results.Add(new TestResult("IslandNames", "PASS"));
            return results;
        }

        public List<TestResult> CheckCommodityExtractorMemory()
        {
            List<TestResult> results = new List<TestResult>();
            Process process = Process.GetCurrentProcess();

            long memStart = process.WorkingSet64;

            for (int i = 0; i < 100; i++)
            {
                UnsafeBitmap bm = new UnsafeBitmap("Test\\Market_Island_Full_800x600.tif");
                ppocr.ClearErrors();

                Rectangle commodArea = ppocr.FindCommodityArea(bm);

                List<Commodity> commodities = ppocr.ExtractCommodities(bm, commodArea);

                GC.Collect();   // Force garbage collection
                GC.WaitForPendingFinalizers();

            }

            process = Process.GetCurrentProcess();
            long memStop = process.WorkingSet64;

            if ((memStop - memStart) > 2000000)
                results.Add(new TestResult("CheckCommodityExtractorMemory", "MemoryStart: " + memStart.ToString() + "  MemoryStop: " + memStop.ToString()));

            if (results.Count <= 0)
                results.Add(new TestResult("CheckCommodityExtractorMemory", "PASS"));

            return results;

        }

        public List<TestResult> CheckCommodityExtractor(string infoFile)
        {

            List<TestResult> results = new List<TestResult>();

            CaptureInfo info = CaptureInfo.Deserialize("Test\\"+infoFile);
            UnsafeBitmap bm = new UnsafeBitmap("Test\\"+info.BitmapFile);
            ppocr.ClearErrors();

            DateTime dtStart = DateTime.Now;

            Rectangle commodArea = ppocr.FindCommodityArea(bm);

            if (commodArea != info.CommodityRect)
            {
                results.Add(new TestResult(infoFile, "Commodity Rect Expected " + info.CommodityRect.ToString() + ", got " + commodArea.ToString()));
                return results;
            }

            string island = ppocr.ExtractIslandName(bm);
            if (!island.Equals(info.Island))
                results.Add(new TestResult(infoFile, "Expected island name: " + info.Island + ", Got: " + island));

            List<Commodity> commodities = ppocr.ExtractCommodities(bm, commodArea);
            if (ppocr.Error.Length > 0)
                results.Add(new TestResult(infoFile, ppocr.Error));

            List<Commodity> refCommods = info.Commodities;

            if (refCommods.Count != commodities.Count)
            {
                results.Add(new TestResult(infoFile, "Commodities Count is " + commodities.Count.ToString() + ", expected " + refCommods.Count.ToString()));
            }

            for (int i = 0; i < refCommods.Count && i < commodities.Count; i++)
            {
                if (!commodities[i].ToString().Equals(refCommods[i].ToString()))
                    results.Add(new TestResult(infoFile, "Expected: " + refCommods[i].ToString() + " Got: " + commodities[i].ToString()));
            }

            DateTime dtStop = DateTime.Now;

            if (results.Count <= 0)
            {
                TestResult result = new TestResult(infoFile, "PASS");
                result.Duration = dtStop.Subtract(dtStart).TotalSeconds.ToString();
                results.Add(result);

            }

            return results;
        }
    }
}
