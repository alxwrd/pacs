// Copyright 2008-2009 Yuhu on Sage, MIT License
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace ppaocr
{
    public class AlphabetFrequencyComparer : IComparer<char>
    {
        // Comparer class to help sort characters by normal frequency of occurance
        Dictionary<char, double> freq = new Dictionary<char, double>();
        public AlphabetFrequencyComparer()
        {
            freq.Add('A', .08167);
            freq.Add('B', .01492);
            freq.Add('C', .02782);
            freq.Add('D', .04253);
            freq.Add('E', .12702);
            freq.Add('F', .02228);
            freq.Add('G', .02015);
            freq.Add('H', .06094);
            freq.Add('I', .06966);
            freq.Add('J', .00153);
            freq.Add('K', .00772);
            freq.Add('L', .04025);
            freq.Add('M', .02406);
            freq.Add('N', .06749);
            freq.Add('O', .07507);
            freq.Add('P', .01929);
            freq.Add('Q', .00095);
            freq.Add('R', .05987);
            freq.Add('S', .06327);
            freq.Add('T', .09056);
            freq.Add('U', .02758);
            freq.Add('V', .00978);
            freq.Add('W', .02360);
            freq.Add('X', .00150);
            freq.Add('Y', .01974);
            freq.Add('Z', .00074);
        }

        private double GetCharFrequency(char x)
        {
            double val = 0;
            if (freq.ContainsKey(char.ToUpper(x)))
                val = freq[Char.ToUpper(x)];

            return val;
        }

        #region IComparer<char> Members

        public int Compare(char x, char y)
        {
            if (x == y)
                return 0;
            if (Char.IsLower(x) && Char.IsUpper(y))
                return -1;
            if (Char.IsUpper(x) && Char.IsLower(y))
                return 1;
            if (Char.IsDigit(x) && !Char.IsDigit(y))
                return 1;
            if (!Char.IsDigit(x) && Char.IsDigit(y))
                return -1;
            if (Char.IsDigit(x) && Char.IsDigit(y))
                return x.CompareTo(y);
            if (Char.IsLetterOrDigit(x) && !Char.IsLetterOrDigit(y))
                return -1;
            if (!Char.IsLetterOrDigit(x) && Char.IsLetterOrDigit(y))
                return 1;

            double xFreq = GetCharFrequency(x);
            double yFreq = GetCharFrequency(y);
            if (xFreq == 0 || yFreq == 0)
                return x.CompareTo(y);
            

            if (xFreq == yFreq) return 0;
            return xFreq > yFreq ? -1 : 1;
        }

        #endregion
    }

    public class BmpChar
    {
        // Class to hold bitmaps of individual characters

        private char character;
        private UnsafeBitmap bm;
        private int charWidth = 0;
        private int charHeight = 0;
        private int charTop = 0;
        private int charBottom = 0;

        public BmpChar(UnsafeBitmap bmChar, char character)
        {
           
            this.character = character;
            bm = bmChar;

            if (bm == null)
                return;

            // Trim empty area from right side of reference bitmap
            int x = 0;
//            x = Utils.FindNextVerticalLine(bm, 0, Color.FromArgb(255, 255, 255, 255), true);
            x = bm.FindNextVerticalLine(0, Color.FromArgb(255, 255, 255), true);
            if (x > 0)
                bm = bm.Clone(new Rectangle(0, 0, x, bm.Height), bm.PixelFormat);
            charWidth = bm.Width;

            // Find character height
            int y=0;
            for (y = 0; y < bm.Height && charTop <= 0; y++)
                for (x = 0; x < bm.Width; x++)
                    if (bm.GetPixel(x, y) != Color.FromArgb(255, 255, 255))
                    {
                        charTop = y;
                        break;
                    }

            for (y=bm.Height-1; y >= 0 && charBottom <= 0; y--)
                for (x = 0; x < bm.Width; x++)
                    if (bm.GetPixel(x, y) != Color.FromArgb(255, 255, 255))
                    {
                        charBottom = y;
                        break;
                    }
            charHeight = charBottom - charTop + 1;

        }

        public Color GetPixel(int x, int y)
        {
            return bm.GetPixel(x, y);
        }

        public int CharTop
        {
            get { return charTop; }
        }

        public int CharBottom
        {
            get { return charBottom; }
        }

        public int CharWidth
        {
            get { return charWidth; }
        }

        public char Character
        {
            get { return character; }
            set { character = value; }
        }

    }

    public class Commodity
    {
        private string name="";
        private string shop="";
        private int? buyPrice=null;
        private int? buyQty=null;
        private int? sellPrice=null;
        private int? sellQty=null;

        public Commodity()
        {

        }

        public Commodity(string[] s)
        {
            bool isgt = false;

            if (s == null)
                return;
            if (s.Length > 0)
                name = s[0];
            if (s.Length > 1)
                shop = s[1];
            if (s.Length > 2)
            {
                int tmp;
                if (!Int32.TryParse(s[2], out tmp))
                    buyPrice = null;
                else
                    buyPrice = tmp;
            }

            if (s.Length > 3 && s[3] != null)
            {
                isgt = false;
                if (s[3].StartsWith(">"))
                {
                    isgt = true;
                    s[3] = s[3].Substring(1);
                }
                int tmp;
                if (!Int32.TryParse(s[3], out tmp))
                    buyQty = null;    // None
                else
                    buyQty = tmp;
                if (isgt)
                    buyQty++;
            }
            if (s.Length > 4)
            {
                int tmp;
                if (!Int32.TryParse(s[4], out tmp))
                    sellPrice = null; // None
                else
                    sellPrice = tmp;
            }
            if (s.Length > 5 && s[5] != null)
            {
                isgt = false;
                if (s[5].StartsWith(">"))
                {
                    isgt = true;
                    s[5] = s[5].Substring(1);
                }
                int tmp;
                if (!Int32.TryParse(s[5], out tmp))
                    sellQty = null;   // None
                else
                    sellQty = tmp;
                if (isgt)
                    sellQty++;
            }

        }

        public override string ToString()
        {
            string buyP = BuyPrice >= 0 ? BuyPrice.ToString() : "";
            string buyQ = BuyQty >= 0 ? BuyQty.ToString() : "";
            string sellP = SellPrice >= 0 ? SellPrice.ToString() : "";
            string sellQ = SellQty >= 0 ? SellQty.ToString() : "";

            return Name + ", " + Shop + ", " + buyP + ", " + buyQ+ ", " + sellP + ", " + sellQ;
        }

        [XmlElementAttribute("Name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlElementAttribute("Shop")]
        public string Shop
        {
            get { return shop; }
            set { shop = value; }
        }

        [XmlElementAttribute("BuyPrice")]
        public int? BuyPrice
        {
            get { return buyPrice; }
            set { buyPrice = Convert.ToInt32(value); }
        }

        [XmlElementAttribute("BuyQty")]
        public int? BuyQty
        {
            get { return buyQty; }
            set { buyQty = Convert.ToInt32(value); }
        }

        [XmlElementAttribute("SellPrice")]
        public int? SellPrice
        {
            get { return sellPrice; }
            set { sellPrice = Convert.ToInt32(value); }
        }

        [XmlElementAttribute("SellQty")]
        public int? SellQty
        {
            get { return sellQty; }
            set { sellQty = Convert.ToInt32(value); }
        }


    }

    public class BidData
    {
        private string commodityName = "";
        private int highBid = 0;
        private int bidQty = 0;

        public BidData(string commodityName, int highBid, int bidQuantity)
        {
            this.commodityName = commodityName;
            this.highBid = highBid;
            this.bidQty = bidQuantity;
        }

        public string CommodityName
        {
            get { return commodityName; }
            set { commodityName = value; }
        }

        public int HighBid
        {
            get { return highBid; }
            set { highBid = value; }
        }

        public int Qty
        {
            get { return bidQty; }
            set { bidQty = value; }
        }
    }


    public enum FontType
    {
        Font1All = 0,
        Font1Numbers = 1,
        Font2All = 2
    }

    public class BinaryPixelBitmap
    {
        // Can represent up to 32 pixel high bitmap... converts incoming
        // bit map to binary representation of pixels (where black is represented
        // as 1 and all other colors as 0).
        protected int[] columns = null;
        protected int height = 0;

        public BinaryPixelBitmap(int[] columns, int height)
        {
            this.columns = columns;
            this.height = height;
        }

        public BinaryPixelBitmap(UnsafeBitmap bm, int xStart, int yStart, int xWidth, int yHeight, BinPixelConvertType convertType, Color? refColor)
        {
            // If refColor==null then sets convertType to ColorIsOne and refColor to black
            // Otherwise if convertType is ColorIsOne then if color matches refColor it is a 1 and all others are 0
            // If convertType is ColorIsZero then if color matches refColor then it is a 0 and all others are 1
            height = yHeight;
            columns = new int[xWidth];
            int temp;
            if (refColor == null)
            {
                convertType = BinPixelConvertType.ColorIsOne;
                refColor = Color.FromArgb(0, 0, 0); // Black
            }

            if (convertType == BinPixelConvertType.ColorIsOne)
            {
                for (int x = xStart; x < xStart + xWidth; x++)
                {
                    temp = 0;
                    for (int y = yStart; y < (yStart + yHeight) && y < (yStart + 16); y++)
                    {
                        if (bm.GetPixel(x, y) == refColor)
                            temp |= 1 << (y-yStart);
                    }
                    columns[x-xStart] = temp;
                }
            }
            else
            {
                for (int x = xStart; x < xStart + xWidth; x++)
                {
                    temp = 0;
                    for (int y = yStart; y < (yStart + yHeight) && y < (yStart + 16); y++)
                    {
                        if (bm.GetPixel(x, y) != refColor)
                            temp |= 1 << (y-yStart);
                    }
                    columns[x-xStart] = temp;
                }
            }
        }

        public int FindNextVerticalLine(int startIndex)
        {
            for (int x = startIndex; x < Width; x++)
            {
                if ((GetColumn(x) & 0xFFFFFFFF) >= 0x00007FFF)
                    return x;
            }

            return -1;
        }

        public BinaryPixelBitmap Clone(int xStart, int width)
        {
            if (!((xStart + width) <= columns.Length && xStart >= 0))
                return null;
            int[] clone = new int[width];
            int j=0;
            for(int i=xStart; i < xStart + width; i++, j++)
            {
                clone[j] = columns[i];
            }
            return new BinaryPixelBitmap(clone, height);
        }

        public int Width
        {
            get { return columns.Length; }
        }

        public int Height
        {
            get { return height; }
        }

        public int GetColumn(int x)
        {
            return columns[x];
        }

    }

    public class BinaryPixelBitmapChar : BinaryPixelBitmap
    {
        // Can other characters be adjacent (without at least 1 pixel column of space between) 
        // to this one on the right side of it?  Assumption is yes, only set to false if it
        // cannot in all situations.
        bool canAdjacentRight = true;

        // Can this character potentially overlap with others (on either side)
        // Assumption is yes, only set to false if it cannot in all situations
        bool canOverlap = true;

        CharInfo charInfo = null;

        // Use white as a zero and any other color as a one.
        public BinaryPixelBitmapChar(UnsafeBitmap bm, CharInfo info) : base(bm, 0, 0, bm.Width, bm.Height, BinPixelConvertType.ColorIsZero, Color.FromArgb(255,255,255))
        {
            this.charInfo = info;
//            this.character = character;

            // Hack to avoid "q" being recognized as "c," or "d" being recognized as "cl", etc.
//            if (info.Character[0].Equals('c') || info.Character[0].Equals(':'))
            if (info.KerningMin > 0)
                canAdjacentRight = false;


            // Trim any blank columns from right side of character bitmap representation
            int blankColumns = 0;
            for (int i = columns.Length - 1; i >= 0; i--)
            {
                if (columns[i] == 0)
                    blankColumns++;
                else
                    break;
            }
            if (blankColumns > 0)
                Array.Resize(ref columns, columns.Length - blankColumns);

            // Hack to avoid "i" being recognized instead of "l"
            //            if (info.Character[0].Equals('i') || info.Character[0].Equals('l') || info.Character[0].Equals(',') || info.Character[0].Equals('I') || info.Character[0].Equals(':'))
            if (this.Width <= 1)
                canOverlap = false;

        }

        public bool CanAdjacentRight
        {
            get { return canAdjacentRight; }
        }

        public bool CanOverlap
        {
            get { return canOverlap; }
        }

        public Char Character
        {
            get { return charInfo.Character[0]; }
        }

        public int KerningMax
        {
            get { return charInfo.KerningMax; }
        }

        public int KerningMin
        {
            get { return charInfo.KerningMin; }
        }

        public int Tracking
        {
            get { return charInfo.Tracking; }   
        }
    }

    public class Ocr
    {

        public enum MatchType : int
        {
            Exact = 0,
            OverlapBothSides = 1,
            OverlapLeftSideOnly = 2,
            OverlapRightSideOnly = 3
        }

        SortedDictionary<char, BmpChar> refBmpChars2 = new SortedDictionary<char, BmpChar>(new AlphabetFrequencyComparer());
        SortedDictionary<char, BinaryPixelBitmapChar> refBinBmpChars2 = new SortedDictionary<char, BinaryPixelBitmapChar>(new AlphabetFrequencyComparer());

        MyFont font1;
        MyFont font2;

        public Ocr()
        {
            font1 = new MyFont("Font1");

            font2 = new MyFont("Font2");

        }
        private bool IsMatchBinBitmapChar(BinaryPixelBitmapChar charBmp, BinaryPixelBitmap unknownBmp, int xStart, MatchType matchType, BinaryPixelBitmapChar prevCharBmp, int xStartPrevChar)
        {
            // Now check character matches, allow left most column and right most column
            // to overlap

            // Check left column
//            if (charBmp.CanOverlap && (matchType == MatchType.OverlapBothSides || matchType == MatchType.OverlapLeftSideOnly))
            if (charBmp.CanOverlap && prevCharBmp != null && prevCharBmp.KerningMin < 0)
            {
                // Check for overlapped match on left column
                if ((charBmp.GetColumn(0) & ~unknownBmp.GetColumn(xStart)) > 0)
                    return false;

                // If overlapped character matches then check that whatever is left (after subtracting out
                // the bits for the current character) is a perfect match for the right side of the
                // previous character
                if (prevCharBmp != null && xStartPrevChar >= 0 && ((xStartPrevChar + prevCharBmp.Width-1) == xStart))
                {
                    if ((unknownBmp.GetColumn(xStart) & ~charBmp.GetColumn(0)) != prevCharBmp.GetColumn(prevCharBmp.Width - 1))
                        return false;
                }
            }
            else
            {
                if (charBmp.GetColumn(0) != unknownBmp.GetColumn(xStart))
                    return false;

                // If current character matches then check that whatever is left (after subtracting out
                // the bits for the current character) is a perfect match for the right side of the
                // previous character
                if (prevCharBmp != null && xStartPrevChar >= 0 && ((xStartPrevChar + prevCharBmp.Width - 1) == xStart))
                {
                    if ((unknownBmp.GetColumn(xStart) & ~charBmp.GetColumn(0)) != prevCharBmp.GetColumn(prevCharBmp.Width - 1))
                        return false;
                }

                // If single pixel wide character then check to ensure that there is nothing to
                // the right of it if it can't support adjacent characters
                if (charBmp.Width == 1 && !charBmp.CanAdjacentRight)
                {
                    if (unknownBmp.GetColumn(xStart + 1) != 0)
                        return false;   // Something adjacent when it is not allowed so not a match
                }

            }


            // Check middle part of character
            for (int x = 1; (x < charBmp.Width - 1) && ((xStart + x) < unknownBmp.Width); x++)
            {
                if (charBmp.GetColumn(x) != unknownBmp.GetColumn(xStart + x))
                    return false;
            }

            // Check right part of character
            if (charBmp.Width > 1)
            {
                if (charBmp.CanOverlap && charBmp.CanAdjacentRight && (matchType == MatchType.OverlapBothSides || matchType == MatchType.OverlapRightSideOnly))
                {
                    // Check for overlapped match on right side
                    if ((charBmp.GetColumn(charBmp.Width-1) & ~unknownBmp.GetColumn(xStart + charBmp.Width-1))> 0)
                        return false;
                }
                else
                {
                    if (charBmp.GetColumn(charBmp.Width-1) != unknownBmp.GetColumn(xStart + charBmp.Width-1))
                        return false;

                    if (!charBmp.CanAdjacentRight && !(unknownBmp.GetColumn(xStart + charBmp.Width) == 0))
                        return false;   // Must have blank column to right of this character, so not a match

                }
            }

            // Found match
            return true;    

        }

        int lastXStart = -1;
        int yTop = -1, yBottom = -1;
        private bool IsMatchCharBmp(BmpChar charBmp, UnsafeBitmap unknownBmp, int xStart, bool doOverlap)
        {
            Color cBlack = Color.FromArgb(0,0,0);

            // Check if charBmp matches specified locatoin in unknown bitmap
            if (xStart != lastXStart)
            {
                yTop = -1; yBottom = -1;
                // First time trying to match for this location so find top and bottom for
                // max width character (15 pixels) to speed up comparisons
                for (int y = 0; y < unknownBmp.Height && yTop < 0; y++)
                    for (int x = xStart; x < unknownBmp.Width && x < (xStart + 15); x++)
                        if (unknownBmp.GetPixel(x, y) == cBlack)
                        {
                            yTop = y;
                            break;
                        }

                if (yTop >= 0)
                {
                    for (int y = unknownBmp.Height - 1; y >= yTop && yBottom < 0; y--)
                        for (int x = xStart; x < unknownBmp.Width && x < (xStart + 15); x++)
                            if (unknownBmp.GetPixel(x, y) == cBlack)
                            {
                                yBottom = y;
                                break;
                            }
                }
            }

            lastXStart = xStart;

            // Check if height is within what will match
            if (yTop < 0 || yBottom < 0)
                return false;
            if (charBmp.CharTop < yTop)
                return false;
            if (charBmp.CharBottom > yBottom)
                return false;

            // Now check character matches, allow left most column and right most column
            // to overlap
            Color cUnknown, cKnown;

            // Check left column
            for (int y = yTop; y <= yBottom; y++)
            {
                if (doOverlap)
                {
                    if (charBmp.GetPixel(0, y) == cBlack && !(unknownBmp.GetPixel(xStart, y) == cBlack))
                        return false;
                }
                else
                {
                    cKnown = charBmp.GetPixel(0, y);
                    cUnknown = unknownBmp.GetPixel(xStart, y);
                    if (cKnown == cBlack && cUnknown != cBlack)
                        return false;
                    if (cUnknown == cBlack && cKnown != cBlack)
                        return false;
                }
                    
            }

            // Check middle part of character
            for (int x = 1; (x < charBmp.CharWidth - 1) && ((xStart + x) < unknownBmp.Width); x++)
                for (int y = yTop; y <= yBottom; y++)
                {
                    cKnown = charBmp.GetPixel(x, y);
                    cUnknown = unknownBmp.GetPixel(xStart + x, y);
                    if (cKnown == cBlack && cUnknown != cBlack)
                        return false;
                    if (cUnknown == cBlack && cKnown != cBlack)
                        return false;
                }

            // Check right part of character
            if (charBmp.CharWidth > 1)
            {
                for (int y = yTop; y <= yBottom; y++)
                {
                    if (doOverlap)
                    {
                        if (charBmp.GetPixel(charBmp.CharWidth - 1, y) == cBlack && !(unknownBmp.GetPixel(xStart + charBmp.CharWidth - 1, y) == cBlack))
                            return false;
                    }
                    else
                    {
                        cKnown = charBmp.GetPixel(charBmp.CharWidth-1, y);
                        cUnknown = unknownBmp.GetPixel(xStart + charBmp.CharWidth-1, y);
                        if (cKnown == cBlack && cUnknown != cBlack)
                            return false;
                        if (cUnknown == cBlack && cKnown != cBlack)
                            return false;
                    }
                }
            }

            // Found match
            return true;    

        }

        private bool IsRecursiveMatch(BinaryPixelBitmap binCell, ref int xPos,
            SortedDictionary<char, BinaryPixelBitmapChar> refChars, ref Stack<BinaryPixelBitmapChar> charsFound, int xPosPrevChar)
        {
            // Recursive function to match up multiple characters until a space is found...
            // to help find characters when they might be adjacent pixels or overlapped by one pixel.
            // Must have a match for all characters up to the space in order to help avoid false
            // positives.

            if (binCell.GetColumn(xPos) == 0 || xPos >= binCell.Width)
                return true;    // Found a space or end of bitmap, so assume matched up to this point

            bool bAllMatchesFound = false;
            int xInc = 0;
            MatchType matchType = MatchType.OverlapBothSides;
            // Ensure that first character must always have an exact match on the left side
            if (charsFound.Count <= 0)
                matchType = MatchType.OverlapRightSideOnly;

            foreach (BinaryPixelBitmapChar guessChar in refChars.Values)
            {
                // Check using overlapped matching
                BinaryPixelBitmapChar last = charsFound.Count <= 0 ? null : charsFound.Peek();
                if (IsMatchBinBitmapChar(guessChar, binCell, xPos, matchType, last, xPosPrevChar))
                {
                    // Found a potential match... so now check if it exactly matches on the right side
                    // and if so then next character test will be for adjacent, otherwise next character
                    // test will be for overlap
                    if (IsMatchBinBitmapChar(guessChar, binCell, xPos, MatchType.OverlapLeftSideOnly, last, xPosPrevChar))
                        xInc = guessChar.Width;
                    else
                        xInc = guessChar.Width - 1;
                    xPos += xInc;
                    charsFound.Push(guessChar);
                    if (IsRecursiveMatch(binCell, ref xPos, refChars, ref charsFound, xPos - xInc))
                    {
                        bAllMatchesFound = true;
                        break;  // Exit foreach
                    }
                    else
                    {
                        // subsequent pixels don't match any characters so back out
                        // previous one and keep trying
                        charsFound.Pop();
                        xPos -= xInc;
                    }
                }
            }

            return bAllMatchesFound;
        }

        public string ExtractText(BinaryPixelBitmap binCell, FontType selectFont)
        {
            // Assumes cell is composed of only white and black pixels and that black pixels
            // form characters.  Also assumes that characters can be overlapped by one column
            // as long as no black pixels of each character overlap (black pixel form one
            // character can overlap white space of character next to it).
            string words = "";

            SortedDictionary<char, BinaryPixelBitmapChar> refChars;
            if (selectFont == FontType.Font1Numbers)
                refChars = font1.NumericalBinChars;
            else if (selectFont == FontType.Font2All)
                refChars = font2.AllBinChars;
            else
                refChars = font1.AllBinChars;

            int xPos = 0;
            int blankLines = 0;
            while (xPos >= 0 && xPos < binCell.Width)
            {
                if (binCell.GetColumn(xPos) == 0)
                {
                    // Blank column
                    blankLines++;
                    xPos++;

                    if (words.Length > 0 && blankLines > 10)
                        break;  // Rest of cell is likely blank so don't bother searching the rest
                    continue;
                }

                Stack<BinaryPixelBitmapChar> charsFound = new Stack<BinaryPixelBitmapChar>();
                charsFound.Clear();

                // Find character(s) that match.  Will recursively look for matching
                // characters until it finds a blank column of pixels (this allows matching
                // of overlapped or adjacent characters).
                if(!IsRecursiveMatch(binCell, ref xPos, refChars, ref charsFound, -1))
                    xPos++; // If none found then increment xPos and try again

                if (charsFound.Count > 0)
                {
                    // Found a space before the character?
                    if (blankLines > 0)
                    {
                        // Use Kerning and Tracking to figure out if there are spaces
                        // and if so how many spaces.

                        // Only check for spaces after we have found some characters
                        // (i.e. strip leading spaces)
                        if (words.Length > 0)
                        {
                            BinaryPixelBitmapChar prevChar = refChars[words[words.Length-1]];
                            blankLines -= prevChar.KerningMin;  // Assumes Tracking is greater than KerningMax-KerningMin
                            while (blankLines >= prevChar.Tracking)
                            {
                                words += " ";   // space
                                blankLines -= prevChar.Tracking;
                            }
                        }
                    }

/*                    if (blankLines >= 2 && (selectFont != FontType.Font1Numbers))
                    {
                        char prevChar = ' ';
                        if (words.Length > 0)
                            prevChar = words[words.Length - 1];

                        if (prevChar.Equals('f'))
                            words = words + " ";    // space
                        else if (prevChar.Equals('1'))
                        {
                            if (blankLines > 4)
                                words = words + " ";    // space
                        }
                        else if (blankLines >= 3)
                            words = words + " ";    // space
                    }
 */
                    blankLines = 0;
                    string s = "";
                    // Pop characters off stack and then add to word(s)
                    foreach (BinaryPixelBitmapChar c in charsFound)
                        s = c.Character + s;
                    words += s;
                }

            }

            string wordsFixed = "";

            // Hack: Upper-case 'i' and lower-case 'L' look the same.
            // If previous character was a space then this character is probably an
            // upper-case 'i', otherwise set it to lower-case 'L'
            for (int i = 0; i < words.Length; i++)
            {
                if (i == 0)
                {
                    if (words[0].Equals('l'))
                        wordsFixed += "I";
                    else
                        wordsFixed += words[i];
                }
                else
                {
                    if (words[i].Equals('I') && !words[i - 1].Equals(' '))
                        wordsFixed += "l";
                    else if (words[i].Equals('l') && words[i - 1].Equals(' '))
                    {
                        if ((i + 1) < words.Length && words[i + 1].Equals('i'))
                            wordsFixed += "l";  // Not likely to have "Ii"  so it is probably "li" instead
                        else
                            wordsFixed += "I";
                    }
                    else
                        wordsFixed += words[i];
                }
            }

            return wordsFixed;
        }

        private bool IsBlankLine(UnsafeBitmap bmp, int x)
        {
            // Called blank if it is not all black.
            Color cBlack = Color.FromArgb(0, 0, 0);
            for (int y = 0; y < bmp.Height; y++)
            {
                if (bmp.GetPixel(x, y) == cBlack)
                    return false;                            
            }
            return true;
        }














    }
}
