// Copyright 2008-2009 Yuhu on Sage, MIT License
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Xml.Serialization;
using System.Web;
using System.Configuration;

namespace ppaocr
{
    enum UploadServerType
    {
        PCTB,
    }

    struct UploadServer
    {
        public Uri HomeUrl;
        public Uri UploadUrl;
        public UploadServerType ServerType;

        public override string ToString()
        {
            return HomeUrl.ToString();
        }
    }

    public class DescendingBuyComparer : IComparer<Commodity>
    {

        #region IComparer<Commodity> Members

        public int Compare(Commodity x, Commodity y)
        {
            if (x.BuyPrice == null && y.BuyPrice != null)
                return 1;
            if (x.BuyPrice != null && y.BuyPrice == null)
                return -1;
            if (x.BuyPrice == y.BuyPrice)
                return 0;
            if (x.BuyPrice < y.BuyPrice)
                return 1;
            return -1;
//            return -x.BuyPrice.CompareTo(y.BuyPrice);
        }

        #endregion
    }
    public class AscendingSellComparer : IComparer<Commodity>
    {

        #region IComparer<Commodity> Members

        public int Compare(Commodity x, Commodity y)
        {
            if (x.SellPrice == null && y.SellPrice != null)
                return -1;
            if (x.SellPrice != null && y.SellPrice == null)
                return 1;
            if (x.SellPrice == y.SellPrice)
                return 0;
            if (x.SellPrice < y.SellPrice)
                return -1;
            return 1;

//            return x.SellPrice.CompareTo(y.SellPrice);
        }

        #endregion
    }


    [Serializable]
    [XmlRootAttribute("CommodInfo", Namespace = "", IsNullable = false)]
    public class CommodInfo
    {
        List<CommodMap> commodMaps = new List<CommodMap>();
        Dictionary<string, CommodMap> commodMapsDict = new Dictionary<string, CommodMap>();

        public List<CommodMap> CommodMaps
        {
            set { commodMaps = value; }
            get { return commodMaps; }
        }

        [XmlIgnore]
        public Dictionary<string, CommodMap> CommodMapsDict
        {
            get { return commodMapsDict; }
            
        }

        public void Serialize(string file)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(CommodInfo));

            TextWriter writer = new StreamWriter(file);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static CommodInfo Deserialize(TextReader reader)
        {
            if (reader == null)
                return null;

            XmlSerializer serializer = new XmlSerializer(typeof(CommodInfo));

            CommodInfo info = new CommodInfo();

           
            
            info = (CommodInfo)serializer.Deserialize(reader);

            info.commodMapsDict.Clear();
            foreach (CommodMap c in info.CommodMaps)
            {
                if (!info.commodMapsDict.ContainsKey(c.Name))
                    info.commodMapsDict.Add(c.Name, c);
            }

            reader.Close();
            return info;
        }

        public static CommodInfo Deserialize(string file)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            if (File.Exists(file) == false)
                return null;


            TextReader reader = new StreamReader(file);

            CommodInfo info = Deserialize(reader);

            return info;
        }

    }

    public class CommodMap
    {
        private string name = "";
        private int index = 0;

        [XmlElementAttribute("Name")]
        public string Name
        {
            get { return name; }
            set { name = value.Trim(); }
        }

        [XmlElementAttribute("Index")]
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

    }

    class Uploader
    {
        string dataFileName;
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PPAOCR";

        private Dictionary<string, Commodity> commods;
        private Dictionary<string, int> stallNames = new Dictionary<string,int>();

        // Sorted List of Lists.  Sorted list of commodity indexes.  There is in turn
        // a list of stalls/shops for each commodity index.
        private SortedList<int, List<Commodity>> buys = new SortedList<int, List<Commodity>>();
        private SortedList<int, List<Commodity>> sells = new SortedList<int, List<Commodity>>();
        int buyCount = 0;
        int sellCount = 0;
        private Dictionary<string, string> stallTypeToAbbrevMap = new Dictionary<string, string>();
        private CommodInfo commodInfo = null;
        private bool isInitialized = false;

        string errorMsg;

        public Uploader()
        {
            dataFileName = appDataDir + "\\capture.dat";

            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);
        }

        public string Error
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }


        public void Initialize(UploadServer server)
        {
            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Initialize Enter");
            errorMsg = "";
            UpdateCommodNameToIndexMap(server);
            UpdateStallTypeToAbbrevMap();
            isInitialized = true;
            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Initialize Return");
        }

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public CommodInfo CommodityMap
        {
            get { return commodInfo; }
        }

        private void CreateUploadStream(Stream fs)
        {
            BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);
           
            bw.Write("005\n".ToCharArray());  // Version
            bw.Write((stallNames.Count.ToString()+"\n").ToCharArray());   // # of Stalls

            // Write out stall/shop names with abbreviations
            string stallName;
            string stallAbbrev;
            foreach(string name in stallNames.Keys)
            {
                int index = name.IndexOf("'s");
                if (index > 0)
                {
                    // Stall, so remove the possessive part and also find out
                    // what type of stall it is.
                    stallName = name.Substring(0, index);
                    if ((index + 2) < name.Length)
                    {
                        stallTypeToAbbrevMap.TryGetValue(name.Substring(index + 2, name.Length - (index + 2)).Trim(), out stallAbbrev);
                        if (stallAbbrev == null)
                        {
                            // Could not determine stall type, so try again by just matching
                            // the start of the stall type since it may be shortened due to
                            // truncation (i.e. "...").
                            string endOfName = name.Substring(index + 2, name.Length - (index + 2)).Trim();
                            endOfName = endOfName.Replace("...","");
                            foreach (string s in stallTypeToAbbrevMap.Keys)
                            {
                                if (s.StartsWith(endOfName))
                                {
                                    stallAbbrev = stallTypeToAbbrevMap[s];
                                    break;
                                }
                            }
                        }
                    }
                    else
                        stallAbbrev = "";

                    if (stallAbbrev == null)
                    {
                        stallName = name;
                        stallAbbrev = "";
                    }
                }
                else
                {
                    stallName = name;
                    stallAbbrev = "";
                }
                bw.Write(stallName.ToCharArray());
                if (stallAbbrev.Length > 0)
                    bw.Write(("^" + stallAbbrev).ToCharArray());
                bw.Write('\n');
            }

            bw.Write(Convert.ToInt16(buyCount)); // # of buy orders
            foreach (int commodIndex in buys.Keys)
            {
                bw.Write(Convert.ToInt16(commodIndex)); // Index of commodity
                bw.Write(Convert.ToInt16(buys[commodIndex].Count));  // # Offer count
//                foreach (int stallIndex in buys[commodIndex].Keys)
                foreach(Commodity c in buys[commodIndex])
                {
//                    Commodity c = buys[commodIndex][stallIndex];
                    int stallIndex = stallNames[c.Shop];
                    bw.Write(Convert.ToInt16(stallIndex));
                    bw.Write(Convert.ToInt16(c.BuyPrice));
                    bw.Write(Convert.ToInt16(c.BuyQty));
                }
            }

            bw.Write(Convert.ToInt16(sellCount)); // # of sell orders
            foreach (int commodIndex in sells.Keys)
            {
                bw.Write(Convert.ToInt16(commodIndex)); // Index of commodity
                bw.Write(Convert.ToInt16(sells[commodIndex].Count));  // # Offer count
//                foreach (int stallIndex in sells[commodIndex].Keys)
                foreach (Commodity c in sells[commodIndex])
                {
//                    Commodity c = sells[commodIndex][stallIndex];
                    int stallIndex = stallNames[c.Shop];
                    bw.Write(Convert.ToInt16(stallIndex));
                    bw.Write(Convert.ToInt16(c.SellPrice));
                    bw.Write(Convert.ToInt16(c.SellQty));
                }
            }

            bw.Close();
        }

        private void UploadFile(UploadServer server)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                using (Stream fileStream = File.OpenRead(appDataDir+"\\Commodities.json"))
                using (Stream requestStream = client.OpenWrite(server.UploadUrl, "POST"))
                {
                    fileStream.CopyTo(requestStream);
                }
            }
        }

        private void UploadWithBrowser(UploadServer server, System.Windows.Forms.WebBrowser webBrowser)
        {

            //string url = "http://pctb.ilk.org/upload.php";
            //string url = ConfigurationSettings.AppSettings["HostUrl"].ToString() + "/upload.php";
            string url = server.UploadUrl.ToString();
            string[] files = { "marketdata.gz" };

            long length = 0;
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            boundary = "SEP9242";

            string navHeader = "Content-Type: multipart/form-data; boundary=" + boundary + "\r\n";

            Stream memStream = new System.IO.MemoryStream();

            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] finalBoundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");


            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            length += boundarybytes.Length;

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\nContent-Type: application/gzip\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {

                string header = string.Format(headerTemplate, "marketdata", files[i]);

                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);
                length += headerbytes.Length;

                FileStream fileStream = new FileStream(appDataDir+"\\"+ files[i], FileMode.Open, FileAccess.Read);
                
                byte[] buffer = new byte[1024];

                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                    length += bytesRead;
                }

                if (i == (files.Length-1))
                    memStream.Write(finalBoundarybytes, 0, finalBoundarybytes.Length);
                else
                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                length += boundarybytes.Length;

                fileStream.Close();
            }

            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            webBrowser.Navigate(url, "", tempBuffer, navHeader);
            System.Threading.Thread.Sleep(1000);
        }

        public void Upload(List<BidData> bidData, UploadServer server, System.Windows.Forms.WebBrowser webBrowser)
        {
            errorMsg = "";

            if (bidData == null)
            {
                errorMsg = "No bid data to upload";
                return;
            }
            if (bidData.Count <= 0)
            {
                errorMsg = "No bid data to upload";
                return;
            }

            if (!IsInitialized)
                Initialize(server);

            if (errorMsg.Length > 0)
                return;

            CommodMap commodMap = null;

            string url = server.UploadUrl.ToString() + "?action=upload&biddata=";
            //string url = ConfigurationSettings.AppSettings["HostUrl"].ToString() + "/upload.php?action=upload&biddata=";
            foreach (BidData b in bidData)
            {
                if (commodInfo.CommodMapsDict.TryGetValue(b.CommodityName, out commodMap))
                {
                    url += commodMap.Index.ToString();
                    url += "," + b.HighBid.ToString();
                    url += "," + b.Qty.ToString() + ",";
                }
                else
                {
                    errorMsg = "Could not find commodity in commodity map: " + b.CommodityName;
                    return;
                }

            }
            webBrowser.Url = new Uri(url);  // Upload data

        }

        public void Upload(Dictionary<string, Commodity> commods, UploadServer server, System.Windows.Forms.WebBrowser webBrowser)
        {
            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Upload Enter");
            if (commods == null)
            {
                errorMsg = "No commodities to upload";
                return;
            }
            if (commods.Count <= 0)
            {
                errorMsg = "No commodities to upload";
                return;
            }

            errorMsg = "";

            if (!IsInitialized)
                Initialize(server);

            if (errorMsg.Length > 0)
                return;

            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Upload UpdateLists");
            this.commods = commods;
            List<string> commoditiesWithoutMap;
            UpdateLists(out commoditiesWithoutMap);

            if (errorMsg.Length > 0)
                return;

            // Create file equivalent of upload data (useful for debugging)
            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Upload Create Capture.dat");
            FileStream fs;
            if (Utils.traceSwitch.Level == TraceLevel.Verbose)
            {
                if (File.Exists(dataFileName))
                    File.Delete(dataFileName);

                fs = new FileStream(dataFileName, FileMode.CreateNew);
                CreateUploadStream(fs); // For debugging
                fs.Close();
            }

            // Create GZip stream for upload data
//            MemoryStream ms = new MemoryStream();
            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Upload Create marketdata.gz");
            string uploadFile = appDataDir+"\\marketdata.gz";

            if (File.Exists(uploadFile))
                File.Delete(uploadFile);
            fs = new FileStream(uploadFile, FileMode.CreateNew);
            GZipStream zipStream = new GZipStream(fs, CompressionMode.Compress);
            CreateUploadStream(zipStream);
            zipStream.Close();
            fs.Close();


            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Upload Create Upload Stream");
            MemoryStream ms = new MemoryStream();
            GZipStream zipMemoryStreem = new GZipStream(ms, CompressionMode.Compress);
            CreateUploadStream(zipMemoryStreem);
            zipMemoryStreem.Close();
            ms.Close();

//            UploadFile();   // Works
            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Upload Upload To Server");
            UploadFile(server);  // Works

            // if (commoditiesWithoutMap != null && commoditiesWithoutMap.Count > 0)
            // {
            //     foreach (string s in commoditiesWithoutMap)
            //         errorMsg = errorMsg + "\n" + "The server does not support the commodity: " + s;
            // }

            Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, DateTime.Now.ToUniversalTime().ToString() + ": Uploader.Upload Return");

            
        }

        private void UpdateLists(out List<string> missingCommodityMaps)
        {
            errorMsg = "";

            missingCommodityMaps = new List<string>();

            // Create mapping of unique stall names to index
            stallNames.Clear();

            int stallCount = 0;
            foreach (Commodity c in commods.Values)
            {
                // Check if name is too short (not wide enough window)
                string shortShopName = c.Shop;
                int index = shortShopName.IndexOf("'s");
                if (index > 0)
                    shortShopName = c.Shop.Substring(0, index);
                if (shortShopName.Contains("..."))
                {
                    errorMsg = "Could not upload data due to incomplete shop/stall names (names contain '...').  Try using a wider window size or avoid capturing from a ship.";
                    return;
                }

                // Add name
                if (!stallNames.ContainsKey(c.Shop))
                {
                    stallCount++;
                    stallNames.Add(c.Shop, stallCount);
                }
            }

            // Create sorted list of commodity names where the key is the commodity name index and the value
            // is a Dictionary List of Commodity (stalls with buy/sell info) with the key being the stall index.

            // Sorted list of commodity (stalls with buy offers... sorted in descending order of buy price)
            buys.Clear();
            sells.Clear();
            buyCount = 0;
            sellCount = 0;
            //int commodIndex = 0;
            CommodMap commodMap = null;
            foreach (Commodity c in commods.Values)
            {
                if (c.BuyQty <= 0 && c.SellQty <= 0)
                    continue;

                // Ignore some commodities that we won't be supporting
                if (c.Name.Equals("Navy Dye"))
                    continue;

                if (commodInfo.CommodMapsDict.TryGetValue(c.Name, out commodMap))
                {
                    List<Commodity> stalls;
                    if (c.BuyQty > 0)
                    {
                        if (!buys.TryGetValue(commodMap.Index, out stalls))
                        {
                            stalls = new List<Commodity>();
                            buys.Add(commodMap.Index, stalls);
                        }
                        stalls.Add(c);
                        buyCount++;
                    }

                    if (c.SellQty > 0)
                    {
                        if (!sells.TryGetValue(commodMap.Index, out stalls))
                        {
                            stalls = new List<Commodity>();
                            sells.Add(commodMap.Index, stalls);
                        }

//                        stalls.Add(stallNames[c.Stall], c);
                        stalls.Add(c);
                        sellCount++;
                    }
                }
                else
                {
                    // Could not find this commodity in the commodity map
                    // Only add it once per commodity name.
                    if (!missingCommodityMaps.Contains(c.Name))
                        missingCommodityMaps.Add(c.Name);
//                    errorMsg = errorMsg + "\n" + "Could not find commodity in commodity map: " + c.Name;
                }
            }

            // Sort buy list in descending order of price
            foreach (List<Commodity> list in buys.Values)
                list.Sort(new DescendingBuyComparer());
            
            // Sort sell list in ascending order of price
            foreach (List<Commodity> list in sells.Values)
                list.Sort(new AscendingSellComparer());

            
        }

        private void UpdateStallTypeToAbbrevMap()
        {
            stallTypeToAbbrevMap.Clear();
            stallTypeToAbbrevMap.Add("Apothecary Stall", "A");
            stallTypeToAbbrevMap.Add("Distilling Stall", "D");
            stallTypeToAbbrevMap.Add("Furnishing Stall", "F");
            stallTypeToAbbrevMap.Add("Ironworking Stall", "I");
            stallTypeToAbbrevMap.Add("Shipbuilding Stall", "S");
            stallTypeToAbbrevMap.Add("Tailoring Stall", "T");
            stallTypeToAbbrevMap.Add("Weaving Stall", "W");
        }

        private void UpdateCommodNameToIndexMap(UploadServer server)
        {
            bool useLocalCommodMapFile = Convert.ToBoolean(ConfigurationSettings.AppSettings["UseLocalCommodMapFile"]);

            if (useLocalCommodMapFile)
            {
                commodInfo = CommodInfo.Deserialize("CommodInfo.xml");
                return;
            }

            string url = server.HomeUrl + "/commodmap.php";
            HttpWebRequest web = (HttpWebRequest)WebRequest.Create(url);
            web.KeepAlive = true;

            web.Credentials = System.Net.CredentialCache.DefaultCredentials;

            WebResponse webResponse = web.GetResponse();
            
            Stream stream2 = webResponse.GetResponseStream();

            StreamReader reader = new StreamReader(stream2);

            string findStart;
            while (!reader.EndOfStream)
            {
                findStart = reader.ReadLine();
                if (findStart.Contains("<pre>"))
                    break;
            }

            //string text = HttpUtility.HtmlDecode(reader.ReadToEnd());
            string text = reader.ReadToEnd();
//            text = text.Replace("&lt", "&lt;");
//            text = text.Replace("&gt", "&gt;");
//            text = text.Replace("&lt; ", "&lt;");
//            text = text.Replace("&gt; ", "&gt;");
            
            text = HttpUtility.HtmlDecode(text).Trim();

            int index = text.IndexOf("</body>");
            if (index >= 0)
                text = text.Remove(index);

            index = text.IndexOf("</pre>");
            if (index >= 0)
                text = text.Remove(index);
           
            commodInfo = CommodInfo.Deserialize(new StringReader(text));

            reader.Close();


        }


    }
}
