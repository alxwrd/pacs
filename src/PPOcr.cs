// Copyright 2008-2009 Yuhu on Sage, MIT License
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Configuration;

namespace ppaocr
{

    public class Utils
    {
        public static TraceSwitch traceSwitch = new TraceSwitch("DebugSwitch", "Debug trace switch");


        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        /* Constants used for User32 calls. */
        const uint SPI_GETFONTSMOOTHING = 74;
        const uint SPI_SETFONTSMOOTHING = 75;
        const uint SPIF_UPDATEINI = 0x1;
        const uint SPIF_SENDCHANGE = 0x2;
        const uint SPI_GETFONTSMOOTHINGTYPE = 0x200A;
        const uint SPI_SETFONTSMOOTHINGTYPE = 0x200B;
        const int ClearType = 0x2;
        const int StandardType = 0x1;
        const uint NoType = 0x0;

        public static Boolean GetFontSmoothing()
        {
            bool iResult;
            int pv = 0;
            /* Call to systemparametersinfo to get the font smoothing value. */
            iResult = SystemParametersInfo(SPI_GETFONTSMOOTHING, 0, ref pv, 0);
            if (pv > 0)
            {
                //pv > 0 means font smoothing is on.
                return true;
            }
            else
            {
                //pv == 0 means font smoothing is off.
                return false;
            }

        }

        public static void DisableFontSmoothing()
        {
            bool iResult;
            int pv = 0;
            /* Call to systemparametersinfo to set the font smoothing value. */
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 0, ref pv, SPIF_UPDATEINI | SPIF_SENDCHANGE);
            pv = ClearType;
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHINGTYPE, 0x0, ref pv, SPIF_UPDATEINI | SPIF_SENDCHANGE);
        }

        public static void EnableFontSmoothing()
        {
            bool iResult;
            int pv = 0;
            /* Call to systemparametersinfo to set the font smoothing value. */
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 1, ref pv, SPIF_UPDATEINI | SPIF_SENDCHANGE);
            pv = StandardType;
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHINGTYPE, 0x0, ref pv, SPIF_UPDATEINI | SPIF_SENDCHANGE);

        }
/*
        public static int FindNextVerticalLine(BinaryPixelBitmap row, int startIndex)
        {
            for (int x = startIndex; x < row.Width; x++)
            {
                if ((row.GetColumn(x) & 0xFFFFFFFF) >= 0x00007FFF)
                    return x;
            }

            return -1;
        }

        public static int FindNextVerticalLine(UnsafeBitmap row, int startIndex, Color c, bool equal)
        {
            // Finds next vertical line of the specified color
            // equal=false:  Find any vertical line with pixels all the same color 
            //               but the color does NOT match color "c".
            // equal=true:   Find vertical line with all pixels equal to color "c".

            // Finds non-white vertical line.  Returns -1 if not found.
            for (int x = startIndex; x < row.Width; x++)
            {
                bool lineFound = true;
                Color prev = row.GetPixel(x, 0);
                if (prev == c && !equal)
                    lineFound = false;
                if (prev != c && equal)
                    lineFound = false;
                for (int y = 1; y < row.Height && lineFound; y++)
                {
                    if (row.GetPixel(x, y) != prev)
                        lineFound = false;
                }

                if (lineFound)
                    return x;
            }

            return -1;
        }
*/

    }

    class ProcessWrapper
    {
        Process process = null;

        public ProcessWrapper(Process p)
        {
            process = p;
        }

        public Process Process
        {
            get { return process; }
        }

        public override string ToString()
        {
            if (process != null)
                return process.MainWindowTitle;
            else
                return "";
        }
    }

    class PPOcr
    {
        IntPtr hPPWindow = IntPtr.Zero;
        Process procPP = null;
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PPAOCR";
        Ocr ocr = new Ocr();
        string ocean = "";

        public PPOcr(Process procPP)
        {
            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);

            this.procPP = procPP;

            if (procPP != null)
            {
                hPPWindow = procPP.MainWindowHandle;

                // Extract ocean name
                string temp = procPP.MainWindowTitle;
                int index = temp.IndexOf(" on the ");
                if (index >= 0)
                    temp = temp.Remove(0, index + 8);
                index = temp.IndexOf(" ocean");
                if (index >= 0)
                    temp = temp.Remove(index);
                if (temp.Length > 0)
                    ocean = temp;
            }
        }
        string reason = "";
        public string Error
        {
            get { return reason; }
        }

        public void ClearErrors()
        {
            reason = "";
        }

        public string Ocean
        {
            get { return ocean; }
        }

        public Bitmap CaptureWindow()
        {
            if (hPPWindow == IntPtr.Zero)
                return null;

            int nWidth;
            int nHeight;
            RECT clientRect;
            RECT rect;
            GetWindowRect(hPPWindow, out rect);
            GetClientRect(hPPWindow, out clientRect);
            nWidth = clientRect.Right - clientRect.Left;
            nHeight = clientRect.Bottom - clientRect.Top;
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hdcTo = IntPtr.Zero;
            IntPtr hdcFrom = IntPtr.Zero;
            /*
                        // NOTE:  .NET routine here is about 25% slower than the native windows way below it
                        //        of capturing the window.
                        // Set bitmap object to size of screen area to be captured
                        Bitmap bmpScreenshot = new Bitmap(nWidth, nHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        // Create a graphics object from the bitmap
                        Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                        // Get absolute coordinates for top,left corner of client area for window
                        // Take the screen shot
                        int xLeft = rect.Left + ((rect.Right - rect.Left) - clientRect.Right)/2;
                        int yTop = rect.Top + ((rect.Bottom - rect.Top) - clientRect.Bottom)-1;
                        gfxScreenshot.CopyFromScreen(xLeft, yTop, 0, 0, new Size(nWidth, nHeight), CopyPixelOperation.SourceCopy);
                        return bmpScreenshot;
            */
            try
            {
                hdcFrom = GetDC(hPPWindow);
                Bitmap bm = null;
                //            Bitmap bm = new Bitmap(nWidth, nHeight);
                //            Graphics g = Graphics.FromImage(bm);
                //            BitBlt(g.GetHdc(), 0, 0, nWidth, nHeight, dc, 0, 0, SRCCOPY);
                //            ReleaseDC(IWindowhWnd, dc);

                // create dc that we can draw to...
                hdcTo = CreateCompatibleDC(hdcFrom);
                hBitmap = CreateCompatibleBitmap(hdcFrom, nWidth, nHeight);

                //  validate
                if (hBitmap != IntPtr.Zero)
                {
                    // adjust and copy
                    //                int x = appRect.Left < 0 ? -appRect.Left : 0;
                    //                int y = appRect.Top < 0 ? -appRect.Top : 0;
                    IntPtr hLocalBitmap = SelectObject(hdcTo, hBitmap);
                    BitBlt(hdcTo, 0, 0, nWidth, nHeight, hdcFrom, 0, 0, SRCCOPY);
                    SelectObject(hdcTo, hLocalBitmap);
                    //  create bitmap for window image...
                    bm = System.Drawing.Image.FromHbitmap(hBitmap);
                }

                if (!IsBitmapFormatOk(bm))
                    bm = bm.Clone(new Rectangle(0, 0, bm.Width, bm.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                return bm;
            }
            finally
            {
                if (hdcFrom != IntPtr.Zero)
                    ReleaseDC(hPPWindow, hdcFrom);
                if (hdcTo != IntPtr.Zero)
                    DeleteDC(hdcTo);
                if (hBitmap != IntPtr.Zero)
                    DeleteObject(hBitmap);

            }
        }
        private bool FindBidDataScrollClickCoorinates(UnsafeBitmap bm, out int scrollDownClickX, out int scrollDownClickY, out int scrollUpClickX, out int scrollUpClickY)
        {
            // Location of where to click for the very top of the scroll down button.
            // Returns true if found, otherwise false
            scrollDownClickX = -65535;   // Assume no scroll bar
            scrollDownClickY = -65535;
            scrollUpClickY = -65535;
            scrollUpClickX = -65535;

            //Rectangle bidArea = FindBidArea(bm);
            Rectangle bidArea = FindCommodityArea(bm);

            // Now check if there is a white line just to the right of the bid area
            // If there is no white line then there is no scroll bar
            Color cWhite = Color.FromArgb(255, 255, 255);
            if (bm.GetPixel(bidArea.Right, bidArea.Top + 1) != cWhite)
                return false;

            // Scroll bar present so now find bottom edge of the top scroll bar button
            int yTopButton = bidArea.Top + 1;
            for (int iRow = bidArea.Top + 2; iRow < bidArea.Top + 50; iRow++)
            {
                if (bm.GetPixel(bidArea.Right, iRow) != cWhite)
                {
                    scrollUpClickY = iRow-1;
                    break;
                }
            }
            scrollUpClickX = bidArea.Right + 5;

            // Now find top edge of the bottom scroll bar button
            for (int iRow = bidArea.Top + 100; iRow < bidArea.Bottom; iRow++)
            {
                if (bm.GetPixel(bidArea.Right, iRow) == cWhite)
                {
                    scrollDownClickY = iRow - 1;
                    break;
                }
            }

            scrollDownClickX = bidArea.Right + 5;

            if (scrollDownClickX > -65535 && scrollDownClickY > -65535 && scrollUpClickX > -65535 && scrollUpClickY > -65535)
                return true;
            else
                return false;
        }

        private bool FindScrollClickCoorinates(UnsafeBitmap bm, out int scrollClickX, out int scrollClickY)
        {
            // Location of where to click for the very top of the scroll down button.
            //            scrollClickX = 618;
            //            scrollClickY = 471;
            // Returns true if found, otherwise false
            scrollClickX = -65535;   // Assume no scroll bar
            scrollClickY = -65535;

            // Find vertical black line at right side of the commodity area
            int xBlackLine = 0;
            Color cBlack = Color.FromArgb(0, 0, 0);
            bool bMatchFound = false;
            for (int iCol = bm.Width / 2; iCol < bm.Width; iCol++)
            {
                for (int iRow = 150; iRow < 170; iRow++)
                {
                    if (bm.GetPixel(iCol, iRow) != cBlack)
                    {
                        bMatchFound = false;
                        break;
                    }
                    else
                        bMatchFound = true;
                }
                if (bMatchFound)
                {
                    xBlackLine = iCol;
                    break;
                }
            }

            if (xBlackLine == 0)
                return false;

            // Now look for white line that is at the top of the lower scrollbar button
            // If we encounter a black line first then there is no scroll bar
            Color cWhite = Color.FromArgb(255, 255, 255);
            bool bBlackLineFound = false;
            bool bWhiteLineFound = false;
            for (int iRow = 150; iRow < bm.Height; iRow++)
            {
                bBlackLineFound = true;
                bWhiteLineFound = true;
                for (int iCol = xBlackLine - 16; iCol <= xBlackLine - 11; iCol++)
                {
                    if (bm.GetPixel(iCol, iRow) != cWhite)
                        bWhiteLineFound = false;

                    if (bm.GetPixel(iCol, iRow) != cBlack)
                        bBlackLineFound = false;

                    if (bWhiteLineFound == false && bBlackLineFound == false)
                        break;  // No sense looking further on this row
                }

                if (bWhiteLineFound)
                {
                    // Found where to click for scrolling scroll bar down.  Set to first pixel of the top of
                    // the scroll down button.
                    scrollClickX = xBlackLine - 12;
                    scrollClickY = iRow - 1;
                    break;
                }

                if (bBlackLineFound)
                    break;
            }

            if (scrollClickX > -65535 && scrollClickY > -65535)
                return true;
            else
                return false;
        }

        private int FindHorizontalLine(UnsafeBitmap bm, int xStart, int yStart, int xStop, int yStop, Color cColor)
        {
            // Routine to search for a horizontal line that matches the specified color.

            bool bFound = false;
            for (int iRow = yStart; iRow <= yStop; iRow++)
            {
                bFound = true;
                for (int iCol = xStart; iCol <= xStop; iCol++)
                {
                    if (bm.GetPixel(iCol, iRow) != cColor)
                    {
                        bFound = false;
                        break;
                    }
                }

                if (bFound == true)
                    return iRow;
            }

            return -1;  // Could not find matching horizontal line
        }

        private int FindVerticalLine(UnsafeBitmap bm, int xStart, int yStart, int xStop, int yStop, Color cColor)
        {
            // Routine to search for a vertical line that matches the specified color.

            //if (Utils.traceSwitch.TraceVerbose && bm != null)
            //    bm.Save(appDataDir + "\\Test.tif");

            bool bFound = false;
            for (int iCol = xStart; iCol <= xStop; iCol++)
            {
                bFound = true;
                for (int iRow = yStart; iRow <= yStop; iRow++)
                {
                    if (bm.GetPixel(iCol, iRow) != cColor)
                    {
                        bFound = false;
                        break;
                    }
                }

                if (bFound == true)
                    return iCol;
            }

            return -1;  // Could not find matching line
        }

        /*
        public Rectangle FindBidArea(UnsafeBitmap bm)
        {
            // Note:  Bid Area size can change depending upon what commodity row
            //        is selected in it.  So, clients of this function should
            //        not assume it is constant.

            // Top left corner of bid commodity area (first pixel in border)
            int xStart = 20;
            int yStart = 85;

            // Color of vertical line at left and right of commodity grid area
            // Color varies slightly on different systems so sample it from a known
            // position.
            Color cBorder = bm.GetPixel(xStart, yStart);

            // Find Width.
            int xPos = FindVerticalLine(bm, xStart + 1, yStart + 3, bm.Width, yStart + 50, cBorder);
//            xPos--; // 1 Pixel to left of line (so it doesn't include it)

            // Find Height.  How far down the left border goes.
            int iRow = 0;
            for (iRow = yStart; iRow <= bm.Height; iRow++)
            {
                if (bm.GetPixel(xStart, iRow) != cBorder)
                    break;
            }

            Rectangle rect = new Rectangle(xStart, yStart, xPos - xStart + 1, iRow - yStart + 1);
            
            return rect;

        }
*/
        private Rectangle FindDisplayDropDownListTriangle(UnsafeBitmap bm)
        {
            // Find the tringle looking graphic for the drop down list that selects
            // what kind of commmodities to show.

            // It must be above the commodity area
            Rectangle area = FindCommodityArea(bm);
            if (area.Height < 15 && area.Width < 100)
                return new Rectangle();

            int xPos = 1, yPos = 1;

            Color cColor = bm.GetPixel(xPos, yPos);
            while (yPos < area.Top)
            {
                // Should be within left half above commodity area
                if (xPos >= area.Width / 2)
                {
                    xPos = 1;
                    yPos++;
                }
                else
                    xPos++;
                cColor = bm.GetPixel(xPos, yPos);

                if (bm.GetPixel(xPos - 1, yPos) != cColor && bm.GetPixel(xPos, yPos - 1) != cColor &&
                    bm.GetPixel(xPos, yPos + 1) != cColor && bm.GetPixel(xPos + 1, yPos) == cColor
                    && bm.GetPixel(xPos+10, yPos) == cColor && bm.GetPixel(xPos+11, yPos) != cColor)
                {
                    // Potentially the start of triangle area... check further

                    // Check top line of triangle
                    if (FindHorizontalLine(bm, xPos, yPos, xPos + 10, yPos, cColor) < 0)
                        continue;

                    // Check that pixels above triangle are different color
                    if (FindHorizontalLine(bm, xPos, yPos - 1, xPos + 11, yPos - 1, bm.GetPixel(xPos, yPos - 1)) < 0)
                        continue;

                    // Check lines 2-5 of triangle
                    for (int i = 1; i <= 5; i++)
                    {
                        if (FindHorizontalLine(bm, xPos + i, yPos + i, xPos + 10-i, yPos+i, cColor) < 0)
                            continue;
                        if (bm.GetPixel(xPos+i-1, yPos + i) == cColor || bm.GetPixel(xPos + 11-i, yPos + i) == cColor)
                            continue;
                    }

                    // Found triangle
                    return new Rectangle(xPos, yPos, 11, 6);
                }

            }

            return new Rectangle();
        }

        public Rectangle FindCommodityArea(UnsafeBitmap bm)
        {
            // Find top left corner of commodity area (dark border above left of column headers)
            // Look for something that looks like F in shape.  We have to search for the border
            // and we don't know the exact color of it since it can vary on different systems.

            int xPos=1;
            int yPos=1;
            int temp;

            Color cBorder = bm.GetPixel(1, 1);  // Initialize to something

            // Top left corner should be within top left 1/4 of window
            while (yPos < bm.Height / 2)
            {
                if (xPos >= bm.Width / 2)
                {
                    xPos = 1;
                    yPos++;
                }
                else
                    xPos++;

                cBorder = bm.GetPixel(xPos, yPos);
                if (bm.GetPixel(xPos - 1, yPos) != cBorder && bm.GetPixel(xPos, yPos - 1) != cBorder &&
                    bm.GetPixel(xPos + 1, yPos) == cBorder && bm.GetPixel(xPos, yPos + 1) == cBorder &&
                    bm.GetPixel(xPos+1, yPos+1) != cBorder)
                {
                    // Found potential corner... check it further

                    // Check if top horizontal line present
                    temp = FindHorizontalLine(bm, xPos, yPos, xPos + 20, yPos, cBorder);
                    if (temp < 0)  
                        continue;   // doesn't match
                    // Check if left vertical line present
                    temp = FindVerticalLine(bm, xPos, yPos, xPos, yPos + 20, cBorder);
                    if (temp < 0)
                        continue;   // doesn't match
                    // Check for horizontal line just below "Commodity" header
                    temp = FindHorizontalLine(bm, xPos+5, yPos+16, xPos+Int32.Parse(ConfigurationSettings.AppSettings["TitleBarHeight"]), yPos + 20, cBorder);
                    if (temp < 0)
                        continue;
                    // Check that there is solid different color above top line
                    temp = FindHorizontalLine(bm, xPos, yPos - 1, xPos + 20, yPos - 1, bm.GetPixel(xPos, yPos - 1));
                    if (temp < 0)
                        continue;
                    // Check that there is a solid different color just below top line
                    temp = FindHorizontalLine(bm, xPos+1, yPos + 1, xPos + 20, yPos + 1, bm.GetPixel(xPos+1, yPos+1));
                    if (temp < 0)
                        continue;


                    // Found corner
                    break;
                }
            }

            yPos = yPos + Int32.Parse(ConfigurationSettings.AppSettings["TitleBarHeight"]);   // Commodity area top left

            // Find Width (it can vary depending upon windows size and whether a scrollbar
            // is present or not).  On 800x600 size when there is no scroll bar there is a 
            // single pixel at 623,85 (on the right border of the commodity area) that is a 
            // different pixel color than the rest of the border, so search for the border 
            // starting just below that bad pixel value (start at yStart+3). 
            int xPos2 = FindVerticalLine(bm, xPos + 1, yPos + 3, bm.Width, yPos + 50, cBorder);
            xPos2--; // 1 Pixel to left of line (so it doesn't include it)

            // Find Height.  No predictable border at the bottom so instead look for how far
            // down the left border goes
            int iRow = 0;
            for (iRow = yPos; iRow <= bm.Height; iRow++)
            {
                if (bm.GetPixel(xPos, iRow) != cBorder)
                    break;
            }

            Rectangle rect = new Rectangle(xPos, yPos, xPos2 - xPos + 1, iRow - yPos + 1);

            return rect;

        }

        private List<BidData> ExtractBidData(UnsafeBitmap bm)
        {
            List<BidData> bidData = new List<BidData>();

            //Rectangle bidArea = FindBidArea(bm);
            Rectangle bidArea = FindCommodityArea(bm);

            // Light grey color just to left of right dark border is
            // also used for color of separater between rows.
            Color cRowSeparator = bm.GetPixel(bidArea.Right - 1, bidArea.Top + 10);

            // Do OCR instead of Clipboard copy because sending CTRL+C to the PP
            // window to get the bid data turned out to be buggy (sometimes it 
            // wouldn't copy to clibpoard and at other times exceptions were thrown 
            // by clipboard api when doing Clipboard.GetText).

            int yPos = bidArea.Top;  // Bottom of first potential row
            int yPosPrev = yPos;

            // height of each row is 35 pixels including the outer border, if a row
            // is highlighted then there is a 1 pixel grey inner boarder all around it


            while (yPos > 0 && yPos < bidArea.Bottom)
            {
                yPos = FindHorizontalLine(bm, bidArea.Left + 10, yPos+1, bidArea.Left + 100, bidArea.Bottom, cRowSeparator);
                if (yPos > 0 && (yPos - yPosPrev >= 24))
                {
                    // Check if this commodity row is high lighted (i.e. has 1 pixel inner border)
                    yPosPrev = yPos;
                    yPos = FindHorizontalLine(bm, bidArea.Left + 10, yPos+1, bidArea.Left + 100, yPos+1, cRowSeparator);
                    if ((yPos - yPosPrev) != 1)
                        yPos = yPosPrev;
                    else
                        yPosPrev = yPos;
                        
                    // Found a row, now find each cell

                    // Find first cell which contains the commodity name
                    Color cCellBackground = bm.GetPixel(bidArea.Left + 2, yPos - 2);
                    int xPos1 = FindVerticalLine(bm, bidArea.Left + 20, yPos - 35 + 2, bidArea.Right, yPos - 2, cCellBackground);
                    if (xPos1 <= 0)
                        break;
                    int xPos2 = FindVerticalLine(bm, xPos1 + 1, yPos - 35 + 2, bidArea.Right, yPos - 2, cRowSeparator);
                    if (xPos2 <= 0)
                        break;
                    BinaryPixelBitmap binCell = new BinaryPixelBitmap(bm.CloneAsBin(new Rectangle(xPos1, yPos - 24, xPos2 - xPos1 + 1, 15), BinPixelConvertType.ColorIsZero, cCellBackground), 15);
                    string commodityName = ocr.ExtractText(binCell, FontType.Font1All);
                    commodityName = FixTextWithHeuristics(commodityName, true, false);

                    // Find 2nd cell which contains the high bid price
                    xPos1 = FindVerticalLine(bm, xPos2 + 1, yPos - 35 + 2, bidArea.Right, yPos - 2, cCellBackground);
                    if (xPos1 <= 0)
                        break;
                    xPos2 = FindVerticalLine(bm, xPos1 + 1, yPos - 35 + 2, bidArea.Right, yPos - 2, cRowSeparator);
                    if (xPos2 <= 0)
                        break;
                    binCell = new BinaryPixelBitmap(bm.CloneAsBin(new Rectangle(xPos1, yPos - 24, xPos2 - xPos1 + 1, 15), BinPixelConvertType.ColorIsZero, cCellBackground), 15);
                    string highBid = ocr.ExtractText(binCell, FontType.Font1Numbers);

                    // Find 3rd cell which contains the quantity
                    xPos1 = FindVerticalLine(bm, xPos2 + 1, yPos - 35 + 2, bidArea.Right, yPos - 2, cCellBackground);
                    if (xPos1 <= 0)
                        break;
                    xPos2 = FindVerticalLine(bm, xPos1 + 1, yPos - 35 + 2, bidArea.Right, yPos - 2, cRowSeparator);
                    if (xPos2 <= 0)
                        break;
                    binCell = new BinaryPixelBitmap(bm.CloneAsBin(new Rectangle(xPos1, yPos - 24, xPos2 - xPos1 + 1, 15), BinPixelConvertType.ColorIsZero, cCellBackground), 15);
                    string bidQuantity = ocr.ExtractText(binCell, FontType.Font1Numbers);
                    // Use Decimal.Parse since it takes into account localization settings for thousands place separator
                    BidData bd = new BidData(commodityName, Convert.ToInt32(Decimal.Parse(highBid)), Convert.ToInt32(Decimal.Parse(bidQuantity)));
                    bidData.Add(bd);
                    yPos += 30;
                }
            }

            return bidData;
        }

        private bool IsBitmapFormatOk(Bitmap bm)
        {
            if (bm.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb &&
                bm.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb &&
                bm.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppPArgb &&
                bm.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppRgb)
            {
                return false;
            }

            return true;
        }

        public List<BidData> ExtractAllBidData(out string island)
        {
            Point lastCursorPos = Cursor.Position;
            List<BidData> bidData = new List<BidData>();
            island = "";
            UnsafeBitmap bm = null;

            try
            {
                ClearErrors();

                if (hPPWindow == IntPtr.Zero)
                {
                    reason = "Can't find Puzzle Pirates window";
                    return null;
                }

                // Font smoothing must be off for Java 6 or later
                if (Utils.GetFontSmoothing() && procPP.MainModule.FileVersionInfo.FileMajorPart >= 6)
                {
                    reason = "Can't perform OCR with font smoothing (cleartype font) enabled.  Turn off font smoothing first.";
                    return null;
                }

                // Anti-alias option in Puzzle Pirates must be off for versions of
                // Java prior to version 6
                if (IsPPAntialiasFontsOn() && procPP.MainModule.FileVersionInfo.FileMajorPart < 6)
                {
                    reason += "\nCan't perform OCR because the Puzzle Pirates font antialiasing feature is on.  Font antialiasing may be disabled in the Puzzle Pirates General Options tab.";
                    return null;
                }


                SetForegroundWindow(hPPWindow);

                // Give plenty of time for window to come to foreground to ensure we
                // get a good screen shot during the island capture part
                if (Utils.traceSwitch.TraceVerbose)
                    System.Threading.Thread.Sleep(1000);

                MySendInput sendKey = new MySendInput();

                RECT rect;
                RECT clientRect;

                GetWindowRect(hPPWindow, out rect);

                if (!IsWindowWithinScreen(rect))
                {
                    reason = "The Puzzle Pirates window is not entirely within a screen.";
                    return null;
                }

                GetClientRect(hPPWindow, out clientRect);


                // Minimum size of 800 x 600
                if ((rect.Right-rect.Left) < 800 || (rect.Bottom-rect.Top) < 600)
                {
                    reason = "Puzzle pirates window is wrong size (expected at least 800 x 600).";
                    return null;
                }

                // Get absolute coordinates for top,left corner of client area for window
                Point point = new Point();
                ClientToScreen(hPPWindow, ref point);
                int xLeft = point.X;
                int yTop = point.Y;
                island = ExtractIslandName(xLeft, yTop);

                if (island == null || island.Length <= 0)
                    return null;

                // Find initial bid area
                bm = new UnsafeBitmap(CaptureWindow());

                int scrollUpClickX, scrollUpClickY, scrollDownClickX, scrollDownClickY;
                bool isScroll = FindBidDataScrollClickCoorinates(bm, out scrollDownClickX, out scrollDownClickY, out scrollUpClickX, out scrollUpClickY);
                if (isScroll)
                {
                    scrollUpClickX += xLeft;
                    scrollUpClickY += yTop;
                    scrollDownClickX += xLeft;
                    scrollDownClickY += yTop;
                }

                // Ensure scroll bar is scrolled to the top
                if (isScroll)
                {
                    // Click just below top scroll button so that it pages up 
                    // then scroll up a few times to make sure it is at the top
                    sendKey.SendMouseClick(scrollUpClickX, scrollUpClickY+1);
                    sendKey.SendMouseClick(scrollUpClickX, scrollUpClickY);
                    sendKey.SendMouseClick(scrollUpClickX, scrollUpClickY);
                    sendKey.SendMouseClick(scrollUpClickX, scrollUpClickY);
                    System.Threading.Thread.Sleep(100);
                    bm = new UnsafeBitmap(CaptureWindow());
                }


                int numListings = -1;
                int j = 0;
                int retryCount = 0;
                bool bSingleScroll = false;
                bool bInvalidDataDetected = false;

                while (retryCount < 30)
                {
                    j++;
                    if (numListings == bidData.Count)
                        retryCount++;
                    else
                        retryCount = 0;

                    numListings = bidData.Count;

                    GC.Collect();   // Force garbage collection
                    GC.WaitForPendingFinalizers();

                    bm = new UnsafeBitmap(CaptureWindow());

                    int bidDataAdded = 0;

                    List<BidData> bidDataOneScreen = ExtractBidData(bm);
                    if (bidDataOneScreen != null)
                    {
                        foreach (BidData bd in bidDataOneScreen)
                        {
                            if (!bidData.Exists(delegate(BidData d) { return d.CommodityName.Equals(bd.CommodityName); }))
                            {
                                if (bd.Qty < 0 || bd.HighBid < 0 || bd.CommodityName.Length <= 0)
                                {
                                    if (!bInvalidDataDetected)
                                    {
                                        // Only show error once per capture
                                        reason += "\nIncorrect bid data detected.  Please report this issue to http://www.sourceforge.net/projects/pctb2";
                                        if (Utils.traceSwitch.TraceVerbose && bm != null)
                                            bm.Save(appDataDir + "\\CaptureBidError.tif");
                                        bInvalidDataDetected = true;
                                    }
                                }
                                bidDataAdded++;
                                bidData.Add(bd);
                            }
                        }
                    }

                    SetForegroundWindow(hPPWindow);

                    // If scroll bar is present then try scrolling down
                    if (isScroll)
                    {
                        // If nothing new after several retries then assume we are near the bottom
                        // and page down won't work anymore so switch to single scrolling
                        if (retryCount > 20)
                            bSingleScroll = true;

                        if (bSingleScroll)
                            sendKey.SendMouseClick(scrollDownClickX, scrollDownClickY + 6);    // Scroll by line since page down scrolling doesn't always get quite to the end
                        else if (bidDataAdded > 0)
                        {
                            sendKey.SendMouseClick(scrollDownClickX, scrollDownClickY - 1); // Page down
                        }
                    }

                }
            }
            finally
            {

                Cursor.Position = lastCursorPos;

                if (Utils.traceSwitch.TraceVerbose && bm != null)
                    bm.Save(appDataDir + "\\CaptureBid.tif");
                bm = null;
                GC.Collect();   // Force garbage collection
                GC.WaitForPendingFinalizers();
            }

            return bidData;
        }

        private bool IsWindowWithinScreen(RECT win)
        {
            // Check all screens in case multi-monitor configurations are being used
            // Must be entirely within one of the screens (no overlapping between screens
            // allowed so that we don't have to deal with various positions that the
            // commodity display dropdown might be shown in... it will always drop DOWN
            // that way instead of sometimes dropping UP).
            bool winInScreen = false;
            foreach (Screen s in Screen.AllScreens)
            {
                if (win.Left >= s.Bounds.Left && win.Right <= s.Bounds.Right && win.Top >= s.Bounds.Top && win.Bottom <= s.Bounds.Bottom)
                    winInScreen = true;
            }

            return winInScreen;
        }

        public Dictionary<string, Commodity> ExtractAllCommodities(out string island)
        {
            Point lastCursorPos = Cursor.Position;
            Dictionary<string, Commodity> allCommods = new Dictionary<string, Commodity>();
            island = "";
            UnsafeBitmap bm = null;

            try
            {
                ClearErrors();

                if (hPPWindow == IntPtr.Zero)
                {
                    reason = "Can't find Puzzle Pirates window";
                    return null;
                }

                // Font smoothing must be off for Java 6 or later
                if (Utils.GetFontSmoothing() && procPP.MainModule.FileVersionInfo.FileMajorPart >= 6)
                {
                    reason = "Can't perform OCR with font smoothing (cleartype font) enabled.  Turn off font smoothing first.";
                    return null;
                }

                // Anti-alias option in Puzzle Pirates must be off for versions of
                // Java prior to version 6
                if (IsPPAntialiasFontsOn() && procPP.MainModule.FileVersionInfo.FileMajorPart < 6)
                {
                    reason += "\nCan't perform OCR because the Puzzle Pirates font antialiasing feature is on.  Font antialiasing may be disabled in the Puzzle Pirates General Options tab.";
                    return null;
                }


                SetForegroundWindow(hPPWindow);

                // Give plenty of time for window to come to foreground to ensure we
                // get a good screen shot during the island capture part
                if (Utils.traceSwitch.TraceVerbose)
                    System.Threading.Thread.Sleep(1000);  

                MySendInput sendKey = new MySendInput();

                RECT rect;
                RECT clientRect;

                GetWindowRect(hPPWindow, out rect);

                if (!IsWindowWithinScreen(rect))
                {
                    reason = "The Puzzle Pirates window is not entirely within a screen.";
                    return null;
                }

                GetClientRect(hPPWindow, out clientRect);


                // Minimum size of 800 x 600
                if ((rect.Right-rect.Left) < 800 || (rect.Bottom-rect.Top) < 600)
                {
                    reason = "Puzzle pirates window is wrong size (expected at least 800 x 600).";
                    return null;
                }
                
                // Get absolute coordinates for top,left corner of client area for window
                Point point = new Point();
                ClientToScreen(hPPWindow, ref point);
                int xLeft = point.X;
                int yTop = point.Y;

                island = ExtractIslandName(xLeft, yTop);

                if (island == null || island.Length <= 0)
                    return null;


                Bitmap bmWindow = CaptureWindow();
                if (!IsBitmapFormatOk(bmWindow))
                {
                    reason = "Pixel Format " + bmWindow.PixelFormat.ToString() + " is not supported.";
                    return null;
                }

                bm = new UnsafeBitmap(bmWindow);

                // Ensure that "Display" is set to "All" so that all commodities are selected 
                // Click relative to top left corner of commodity area.
                Rectangle triangleArea = FindDisplayDropDownListTriangle(bm);
                if (triangleArea.Top > 0 && triangleArea.Left > 0)
                {
                    // Create a black/white representation of area to left of drop down list triangle
                    // so we can OCR what is currently selected in the drop down list.  The color
                    // of the text should be the same color as the triangle.  Only need to go about
                    // 50 pixels to the left since we are only looking for the word "All"
                    BinaryPixelBitmap binRow = new BinaryPixelBitmap(bm.CloneAsBin(new Rectangle(triangleArea.Left - 51, triangleArea.Top - 4, 50, 15),
                            BinPixelConvertType.ColorIsOne, bm.GetPixel(triangleArea.Left, triangleArea.Top)), 15);

                    string text = ocr.ExtractText(binRow, FontType.Font1All);
                    if (text == null || !text.Contains("All"))
                    {
                        // "All" is not selected so we need to select it                        
                        if (triangleArea.Top > 5 && triangleArea.Left > 51)
                        {
                            int triXClick = triangleArea.Left + triangleArea.Width / 2;
                            int triYClick = triangleArea.Top + triangleArea.Height / 2;
                            sendKey.SendMouseClick(xLeft + triXClick, yTop + triYClick);
                            sendKey.SendMouseClick(xLeft + triXClick, yTop + triYClick + 20);  // "All" is first in drop down list and about 20 pixels down
                            //sendKey.SendMouseClick(xLeft + commodArea.Left + 80, yTop + commodArea.Top - 30);
                            //sendKey.SendMouseClick(xLeft + commodArea.Left + 80, yTop + commodArea.Top - 10);

                            // Check if "All" is selected yet.  It can take a bit on some systems for it
                            // to take effect and we don't want to proceed until it does, otherwise we'll
                            // end up OCRing the wrong list of commodities.
                            // Try for 1 second.
                            for (int i = 0; i < 10; i++)
                            {
                                GC.Collect();   // Force garbage collection
                                GC.WaitForPendingFinalizers();

                                bmWindow = CaptureWindow();
                                bm = new UnsafeBitmap(bmWindow);
                                triangleArea = FindDisplayDropDownListTriangle(bm);
                                if (triangleArea.Top > 5 && triangleArea.Left > 51)
                                {
                                    binRow = new BinaryPixelBitmap(bm.CloneAsBin(new Rectangle(triangleArea.Left - 51, triangleArea.Top - 4, 50, 15),
                                            BinPixelConvertType.ColorIsOne, bm.GetPixel(triangleArea.Left, triangleArea.Top)), 15);
                                    text = ocr.ExtractText(binRow, FontType.Font1All);
                                    if (text == null || !text.Contains("All"))
                                        System.Threading.Thread.Sleep(100);
                                    else
                                        break;
                                }
                            }
                        }
                    }
                }


                int scrollClickX;
                int scrollClickY;
                bool isScroll = FindScrollClickCoorinates(bm, out scrollClickX, out scrollClickY);
                if (isScroll)
                {
                    scrollClickX += xLeft;
                    scrollClickY += yTop;
                }

                Rectangle commodArea = FindCommodityArea(bm);

                // Click in commodity area to give it focus
                //sendKey.SendMouseClick(xLeft + 65, yTop + 91);
                sendKey.SendMouseClick(xLeft + commodArea.Left + 10, yTop + commodArea.Top + 10);
                System.Threading.Thread.Sleep(100);

                // Ensure we start at the top of the screen
                // Note:  This has a side effect of forcing all scroll bars (including chat
                //        window) to the top (even though we clicked in the commodity area
                //        to give it focus).
                sendKey.DoKeyboard(MySendInput.VK.CONTROL, MySendInput.KEYEVENT.KEYDOWN);
                sendKey.DoKeyboard(MySendInput.VK.HOME, MySendInput.KEYEVENT.KEYDOWN);
                sendKey.DoKeyboard(MySendInput.VK.HOME, MySendInput.KEYEVENT.KEYUP);
                sendKey.DoKeyboard(MySendInput.VK.CONTROL, MySendInput.KEYEVENT.KEYUP);
                System.Threading.Thread.Sleep(500);

                List<Commodity> oneScreenCommods = new List<Commodity>();

                DateTime dtStart = DateTime.Now;

                int numListings = -1;
                int j = 0;
                int retryCount = 0;
                bool bSingleScroll = false;
                bool bInvalidCommodityDetected = false;

                while (retryCount < 30)
                {
                    j++;
                    Console.WriteLine("Rows: " + allCommods.Count);

                    if (numListings == allCommods.Count)
                        retryCount++;
                    else
                        retryCount = 0;

                    numListings = allCommods.Count;

                   GC.Collect();   // Force garbage collection
                   GC.WaitForPendingFinalizers();

                   bmWindow = CaptureWindow();
                   bm = new UnsafeBitmap(bmWindow);

                    int commodsAdded = 0;

                    oneScreenCommods = ExtractCommodities(bm, commodArea);
                    if (oneScreenCommods != null)
                    {
                        foreach (Commodity c in oneScreenCommods)
                        {
                            if (!allCommods.ContainsKey(c.Name + "," + c.Shop))
                            {
                                if (c.Name.Length <= 0 || c.Shop.Length <= 0 || 
                                    (c.BuyPrice == null && c.BuyQty ==null && c.SellPrice ==null && c.SellQty ==null) ||
                                    (c.BuyQty ==null && c.BuyPrice !=null) || (c.BuyQty !=null && c.BuyPrice ==null) ||
                                    (c.SellQty ==null && c.SellPrice !=null) || (c.SellQty !=null && c.SellPrice ==null))
                                {
                                    if (!bInvalidCommodityDetected)
                                    {
                                        // Only show error once per capture
                                        reason += "\nIncorrect commodity data detected.  Please report this issue to http://www.sourceforge.net/projects/pctb2";
                                        if (Utils.traceSwitch.TraceVerbose && bm != null)
                                            bm.Save(appDataDir + "\\CaptureError.tif");
                                        bInvalidCommodityDetected = true;
                                    }
                                }
                                commodsAdded++;
                                allCommods.Add(c.Name + "," + c.Shop, c);
                            }
                        }
                    }
                    Console.WriteLine("j: " + j.ToString() + "  Count: " + allCommods.Count);

                    Console.WriteLine("Page Down");
                    SetForegroundWindow(hPPWindow);

                    // If scroll bar is present then try scrolling down
                    if (isScroll)
                    {
                        // If nothing new after several retries then assume we are near the bottom
                        // and page down won't work anymore so switch to single scrolling
                        if (retryCount > 20)
                            bSingleScroll = true;  

                        if (bSingleScroll)
                            sendKey.SendMouseClick(scrollClickX, scrollClickY + 6);    // Scroll by line since page down scrolling doesn't always get quite to the end
                        else if (commodsAdded > 0)
                        {
                            sendKey.SendMouseClick(scrollClickX, scrollClickY - 1); // Page down
                        }

                    }

                }

                // Force scroll bars to the end (so chat window will be back at the end)
                sendKey.DoKeyboard(MySendInput.VK.CONTROL, MySendInput.KEYEVENT.KEYDOWN);
                sendKey.DoKeyboard(MySendInput.VK.END, MySendInput.KEYEVENT.KEYDOWN);
                sendKey.DoKeyboard(MySendInput.VK.END, MySendInput.KEYEVENT.KEYUP);
                sendKey.DoKeyboard(MySendInput.VK.CONTROL, MySendInput.KEYEVENT.KEYUP);

                DateTime dtStop = DateTime.Now;
            }
            finally
            {

                Cursor.Position = lastCursorPos;

                if (Utils.traceSwitch.TraceVerbose && bm != null)
                    bm.Save(appDataDir+"\\Capture.tif");
                bm = null;
                GC.Collect();   // Force garbage collection
                GC.WaitForPendingFinalizers();
            }

            if (Utils.traceSwitch.TraceVerbose)
            {
                // Create unique list of shop names and write it to text file
                Dictionary<string, string> shopNames = new Dictionary<string, string>();
                
                TextWriter writer = new StreamWriter(appDataDir+ "\\ShopNames.txt",false);
                writer.WriteLine(island);
                writer.WriteLine();
                foreach (Commodity c in allCommods.Values)
                {
                    if (!shopNames.ContainsKey(c.Shop) && !c.Shop.Contains("Stall") && !c.Shop.Contains("'s"))
                    {
                        shopNames.Add(c.Shop, c.Shop);
                        writer.WriteLine(c.Shop);
                    }
                }

                writer.Close();

                // Write all commodity information to text file
                writer = new StreamWriter(appDataDir+"\\Commodities.txt",false);
                foreach (Commodity c in allCommods.Values)
                    writer.WriteLine(c.ToString());
                writer.Close();

            }

            return allCommods;

        }
        public bool IsShip(UnsafeBitmap bmWindow)
        {
            // Look for white border area just below anticipated scroll bar location on ship

            // Look for double black horizontal lines (with one pixel of non black in between)
            // In the island view area of upper right section of PP window.
            int yPos = FindHorizontalLine(bmWindow, bmWindow.Width - 80, 165, bmWindow.Width - 30, 220, Color.FromArgb(0, 0, 0));
            if (yPos < 0)
                return false;
            yPos = FindHorizontalLine(bmWindow, bmWindow.Width - 80, yPos + 2, bmWindow.Width - 30, yPos + 2, Color.FromArgb(0, 0, 0));
            if (yPos < 0)
                return false;

            return true;
        }

        private List<Rectangle> FindCellAreas(BinaryPixelBitmap bpbm)
        {
            int x1 = 0;
            int x2 = 0;
            List<Rectangle> areas = new List<Rectangle>();

            while (x1 >= 0 && x1 < bpbm.Width)
            {
                x1 = bpbm.FindNextVerticalLine(x1 + 1);
//                x1 = Utils.FindNextVerticalLine(bpbm, x1 + 1);
                if (x1 >= 0)
                {
                    areas.Add(new Rectangle(x2, 0, x1 - x2, bpbm.Height));
                    x2 = x1 + 1;    // Don't include vertical separator in cell area
                }
            }

            return areas;
        }


        public List<Commodity> ExtractCommodities(UnsafeBitmap bmWindow, Rectangle area)
        {
            // Color of horizontal separator lines between rows of commodities
            // (gray color, lighter color than the dark border around the commodity area)
            // Color can vary slightly on different systems so sample it form a known
            // position (just to left of right border around the commodity area, which
            // should be the very right of the area defined for the commodity area)
            Color cLineColor1 = bmWindow.GetPixel(area.X + area.Width - 1, area.Y + 5);

            // Color of horizontal separator line at top and bottom of commodity grid area
            // (darker colored line)... same as dark line on left side of area
            // Note:  Color varies slightly on different PCs (XP vs. Vista, Graphics ??)
            //        therefore get color from a known position.
            //Color cLineColor2 = bmWindow.GetPixel(15, 80);

            Color cLineColor2 = bmWindow.GetPixel(area.X, area.Y + 5);

            List<Commodity> commods = new List<Commodity>();
            commods.Clear();


            // Commodity Area: x=15, y=83, width=598, height=401

            //            UnsafeBitmap bmFound2 = bmWindow.Clone(new Rectangle(15, 83, 598, 401), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //            bmFound2.Save("tmpCommods.tif", System.Drawing.Imaging.ImageFormat.Tiff);

            // Area of bitmap to search 
            //   From Market: 15,83 - 613,483  (15,83 - 622,483 without vertical scrollbar)
            //   From Ship: 15,83 - 613,341
            //
            //   Note:  When vertical scroll bar is not present then width of the area is different.
            //          So, search for right side when first row is found.
            int xStart = area.X; //= 15;
            int yStart = area.Y; // 83;
            int xWidth = area.Width; // 598;

            int yHeight = area.Height; // 401;

            // Find bitmap area for a row of commods
            int lineFound1 = 0, lineFound2 = 0;
            List<Rectangle> cellAreas = new List<Rectangle>();

            for (int iRow = yStart; iRow < yStart + yHeight; iRow++)
            {
                // Find line of same color as one of the grid lines
                bool bLineFound = true;
                Color cTemp;
                for (int iCol = xStart + 30; iCol < xStart + 50; iCol++)
                {
                    cTemp = bmWindow.GetPixel(iCol, iRow);
                    if (cTemp != cLineColor1 && cTemp != cLineColor2)
                    {
                        bLineFound = false;
                        break;
                    }
                }

                if (bLineFound)
                {
                    lineFound1 = lineFound2;
                    lineFound2 = iRow;

                    if ((lineFound2 - lineFound1) == 16)
                    {
                        // Found row area of bitmap, create a black/white representation of it
                        BinaryPixelBitmap binRow = new BinaryPixelBitmap(bmWindow.CloneAsBin(new Rectangle(xStart, lineFound1 + 1, xWidth, lineFound2 - lineFound1 - 1), BinPixelConvertType.ColorIsZero, bmWindow.GetPixel(xStart + 1, lineFound1 + 1)), lineFound2-lineFound1-1);

                        // Find cell areas (commod name, stall name, buy price, buy qty, sell price, sell qty)
                        // the first time through... the areas will be the same for subsequent rows.
                        if (cellAreas.Count <= 0)
                        {
                            cellAreas = FindCellAreas(binRow);
                            if (cellAreas.Count < 6)
                                return commods; // Should never happen but just in case
                        }

                        string[] s = ExtractCommodFromBinRow(binRow, cellAreas);

                        if (s != null && string.Join("",s).Length > 0)
                            commods.Add(new Commodity(s));

                    }
                }

            }

            return commods;
        }


        private string[] ExtractCommodFromBinRow(BinaryPixelBitmap row, List<Rectangle> cells)
        {
            // Index:
            //   0: Commodity
            //   1: Stall
            //   2: Buy Price
            //   3: Will Buy (quantity)
            //   4: Sell Price
            //   5: Will Sell (quantity)
            string[] commod = new string[6];
            BinaryPixelBitmap cell;

            if (cells.Count < commod.Length)
                return commod;

            // Extract text from each cell
            for (int i = 0; i < commod.Length; i++)
            {
                // If Price does not exist then quantity should not either so
                // in the interest of speed don't bother trying to OCR the quantity
                if ((i == 3 || i == 5) && commod[i - 1].Length <= 0)
                {
                    commod[i] = "";
                    continue;
                }
                cell = row.Clone(cells[i].X, cells[i].Width);
                if (i < 2)
                {
                    commod[i] = ocr.ExtractText(cell, FontType.Font1All);
                    commod[i] = FixTextWithHeuristics(commod[i], true, i > 0 ? true : false);
                }
                else
                    commod[i] = ocr.ExtractText(cell, FontType.Font1Numbers);
            }

            return commod;
        }

        private string FixTextWithHeuristics(string text, bool capFirstWord, bool capOtherWords)
        {
            // There are some characters that are ambiguous and that cannot be recognized
            // properly by the OCR routines.  This occurs for the letters "I" and "l" (lower case
            // "L").  They have the same bitmaps.  So, try to fix it here.
            char[] textArray = text.ToCharArray();

            // Replace "I" with lower case "L" where needed
            for (int i = 0; i < textArray.Length; i++)
            {
                if (textArray[i].Equals('I'))
                {
                    if (i == 0)
                    {
                        if (!capFirstWord)
                            textArray[i] = 'l';
                    }
                    else if ((textArray[i - 1].Equals(' ') || textArray[i-1].Equals('-')) && !capOtherWords)
                        textArray[i] = 'l';
                }
            }

            // Replace lower case "L" with "I" where needed
            for (int i = 0; i < textArray.Length; i++)
            {
                if (textArray[i].Equals('l'))
                {
                    if (i == 0)
                    {
                        if (capFirstWord)
                            textArray[i] = 'I';
                    }
                    else if ((textArray[i - 1].Equals(' ') || textArray[i-1].Equals('-')) && capOtherWords)
                        textArray[i] = 'I';
                }
               
            }

            return new string(textArray);
        }

        

        public string ExtractIslandName(UnsafeBitmap bm)
        {
            int rowLeft = 0;
            int rowTop = 0;
            int rowWidth = 150;
            int rowHeight = 15;

            string island = "";

            // Check if this is on a ship
            if (IsShip(bm))
            {
                Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose,"ExtractIslandName--> IsShip");
                if (IsPPAntialiasFontsOn())
                {
                    island = "Unknown from Ship";   // Can't OCR if antialias font is on
                    reason += "\nThe island name cannot be scanned because the Puzzle Pirates font antialiasing feature is on.  Font antialiasing may be disabled in the Puzzle Pirates General Options tab.";
                }
                else
                {
                    // Find island name by doing OCR of island name in the island area at the top right
                    // of the PP window.
                    // rowLeft = 646; 
                    rowTop = 138;
                    rowLeft = bm.Width - 5 - rowWidth;
                    BinaryPixelBitmap islandRow = new BinaryPixelBitmap(bm.CloneAsBin(new Rectangle(rowLeft, rowTop, rowWidth, rowHeight), BinPixelConvertType.ColorIsOne, Color.FromArgb(255, 255, 255)), rowHeight);

                    island = ocr.ExtractText(islandRow, FontType.Font1All);
                }
            }
            else
            {
                Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, "ExtractIslandName--> IsIsland");
                // Sample color of island text and the horizontal line used to find where island
                // text is located.  The color is the same color as the "Players Online" title.  
                // But, the position of the "Players Online" text can shift left/right by at least 
                // one pixel depending upon the border that is sometimes different for the right pane
                // area.  So, sample the background first, then look to the right for first
                // pixel that is different and that should be the color of the "Players Online"
                // text.  Use that color when searching for the line.

                //                    Color cOne = Color.FromArgb(2, 33, 88);
                Color cBackgroundPlayersOnline = bm.GetPixel(bm.Width - 145, 265);
                Color cOne = bm.GetPixel(bm.Width - 62, 265);

                Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, "ExtractIslandName--> cBackgroundPlayersOnline=" + cBackgroundPlayersOnline.ToString() + " at " + (bm.Width-145).ToString() + ", 265");

                int i;
                for (i = bm.Width - 144; i < bm.Width - 1; i++)
                {
                    cOne = bm.GetPixel(i, 265);
                    if (cOne != cBackgroundPlayersOnline)
                        break;
                }
                Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, "ExtractIslandName--> cOne=" + cOne.ToString() + " at " + (i).ToString() + ", 265");

                // Find area for island name (it is just below a horizontal line that is almost
                // the width of the right info area).  
                int yPos = FindHorizontalLine(bm, bm.Width - 40, 295, bm.Width - 20, 395, cOne);

                Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, "ExtractIslandName--> yPos="+yPos);
                if (yPos >= 0)
                {
                    //                        rowLeft = 650; rowTop = iRow + 6; rowWidth = 130;
                    rowLeft = bm.Width - 150; rowTop = yPos + 6; rowWidth = 130;
                    Trace.WriteLineIf(Utils.traceSwitch.TraceVerbose, "ExtractIslandName--> rowLeft=" + rowLeft);
                    BinaryPixelBitmap islandRow = new BinaryPixelBitmap(bm.CloneAsBin(new Rectangle(rowLeft, rowTop, rowWidth, rowHeight), BinPixelConvertType.ColorIsOne, cOne), rowHeight);
                    island = ocr.ExtractText(islandRow, FontType.Font2All);
                }
            }

            if (Utils.traceSwitch.TraceVerbose)
            {
                string name = appDataDir + "\\IslandCapture.tif";
                if (File.Exists(name))
                    File.Delete(name);
                bm.Save(name);
                name = appDataDir + "\\IslandCell.tif";
                if (File.Exists(name))
                    File.Delete(name);
                bm.Clone(new Rectangle(rowLeft, rowTop, rowWidth, rowHeight), System.Drawing.Imaging.PixelFormat.Format32bppRgb).Save(name);
            }

            // Remove the word "Island" or ":" and anything after it from the name and trim
            // white space from left and right
            int index = island.IndexOf("Island");
            index = index > 0 ? index : island.Length;
            island = island.Substring(0, index).Trim();
            index = island.IndexOf(":");
            index = index > 0 ? index : island.Length;
            island = island.Substring(0, index).Trim();

            return island;

        }

        public bool IsPPAntialiasFontsOn()
        {
            bool bypassPPAntialiasFontCheck = Convert.ToBoolean(ConfigurationSettings.AppSettings["BypassPPAntialiasFontCheck"]);
            if (bypassPPAntialiasFontCheck)
                return false;
            bool isAliasOn = true;
            RegistryKey OurKey = Registry.CurrentUser;
            OurKey = OurKey.OpenSubKey(@"Software\JavaSoft\Prefs\rsrc\config\yohoho\client", false);
            if (OurKey != null)
                isAliasOn = Convert.ToBoolean(OurKey.GetValue("antialias_fonts", true));
            return isAliasOn;
        }

        private string ExtractIslandName(int xLeft, int yTop)
        {
            string island = "";
            Bitmap bmWindow = CaptureWindow();
            if (!IsBitmapFormatOk(bmWindow))
            {
                reason = "Pixel Format " + bmWindow.PixelFormat.ToString() + " is not supported.";
                return null;
            }
            UnsafeBitmap bm = new UnsafeBitmap(bmWindow);

            // Check if this is on a ship
            if (IsShip(bm))
            {
                island = ExtractIslandName(bm);
            }
            else
            {
                // Find island name by doing a /w and ocr of island name in right hand pane

                MySendInput sendKey = new MySendInput();

                // Find command area of window

                // Put focus to command area of window
                //                sendKey.SendMouseClick(xLeft + 114, yTop+bm.Height - 10);
                // Note:  Click far enough to the right so that if user has "tell" on it
                //        still clicks within the command area.
                sendKey.SendMouseClick(xLeft + 300, yTop + bm.Height - 10);

                System.Threading.Thread.Sleep(100); // Wait for screen to update

                // Send /w to list island info in right panel
                sendKey.DoKeyboard(MySendInput.VK.DIVIDE, MySendInput.KEYEVENT.KEYDOWN);
                sendKey.DoKeyboard(MySendInput.VK.DIVIDE, MySendInput.KEYEVENT.KEYUP);
                sendKey.DoKeyboard(MySendInput.VK.VK_W, MySendInput.KEYEVENT.KEYDOWN);
                sendKey.DoKeyboard(MySendInput.VK.VK_W, MySendInput.KEYEVENT.KEYUP);
                sendKey.DoKeyboard(MySendInput.VK.RETURN, MySendInput.KEYEVENT.KEYDOWN);
                sendKey.DoKeyboard(MySendInput.VK.RETURN, MySendInput.KEYEVENT.KEYUP);

                int retryCount = 0;
                while (retryCount < 10)
                {
                    bmWindow = CaptureWindow();
                    if (!IsBitmapFormatOk(bmWindow))
                    {
                        reason = "Pixel Format " + bmWindow.PixelFormat.ToString() + " is not supported.";
                        return null;
                    }
                    bm = new UnsafeBitmap(bmWindow);

                    island = ExtractIslandName(bm);
                    if (island != null && island.Length > 0)
                        break;

                    // Keep trying since "/w" command sometimes takes a while to take effect
                    System.Threading.Thread.Sleep(100);
                    retryCount++;
                }
            }

            // Log errors if island name not found
            if (island == null || island.Length <= 0)
            {
                reason = "Can't find name of island";
                return null;
            }

            return island;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        private static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
        wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
        private const Int32 SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        private static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern IntPtr DeleteObject(IntPtr hDc);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32")]
        private static extern int ClientToScreen(IntPtr hWnd, ref Point lpPoint);  

    }
}
