using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace ppaocr
{
    public enum BinPixelConvertType
    {
        ColorIsZero = 0,
        ColorIsOne = 1
    }
    // Class to access pixels of a bitmap in a way that is faster than the
    // normal GetPixel/SetPixel of the Bitmap class.  This class uses pointers
    // and avoids lock/unlock for each access.
    public unsafe class UnsafeBitmap : IDisposable
    {
        private Bitmap bitmap;

        int width;
        BitmapData bitmapData = null;
        Byte* pBase = null;
        bool bitmapLocked = false;
        int bytesPerPixel = 3;

        public UnsafeBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            LockBitmap();
        }

        public UnsafeBitmap(string file)
        {
            this.bitmap = new Bitmap(file);
            LockBitmap();
        }

        public void Save(string file)
        {
            this.bitmap.Save(file);
        }

        public Bitmap RawBitmap
        {
            get { return bitmap; }
        }

        private Point PixelSize
        {
            get
            {
                GraphicsUnit unit = GraphicsUnit.Pixel;
                RectangleF bounds = bitmap.GetBounds(ref unit);

                return new Point((int)bounds.Width, (int)bounds.Height);
            }
        }

        private void LockBitmap()
        {
            if (bitmapLocked)
                return;

            UpdateFormat();

            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
            (int)boundsF.Y,
            (int)boundsF.Width,
            (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            width = bitmapData.Stride;

            bitmapLocked = true;


            pBase = (Byte*)bitmapData.Scan0.ToPointer();
            Debug.Assert(pBase != null);
        }

        public Color GetPixel(int x, int y)
        {
            PixelData rgb = *PixelAt(x, y);
            return Color.FromArgb(rgb.red, rgb.green, rgb.blue);
        }

        public void SetPixel(int x, int y, PixelData colour)
        {
            PixelData* pixel = PixelAt(x, y);
            *pixel = colour;
        }

        public void SetPixel(int x, int y, Color color)
        {
            PixelData rgb;
            rgb.red = color.R;
            rgb.green = color.G;
            rgb.blue = color.B;
            SetPixel(x, y, rgb);
        }

        private void UnlockBitmap()
        {
            if (!bitmapLocked)
                return;
            if (bitmap == null)
                return;
            bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            bitmapLocked = false;
            pBase = null;
        }
        public PixelData* PixelAt(int x, int y)
        {
            Debug.Assert(pBase != null);
            return (PixelData*)(pBase + y * width + x * bytesPerPixel);
        }

        public int Width
        {
            get { return bitmap.Width; }
        }

        public int Height
        {
            get { return bitmap.Height; }
        }

        public int[] CloneAsBin(Rectangle rect, BinPixelConvertType convertType, Color? refColor)
        {
            int yHeight = rect.Height;
            if (yHeight > 32)
                return null;
            int xWidth = rect.Width;
            int xStart = rect.Left;
            int yStart = rect.Top;

            // If refColor==null then sets convertType to ColorIsOne and refColor to black
            // Otherwise if convertType is ColorIsOne then if color matches refColor it is a 1 and all others are 0
            // If convertType is ColorIsZero then if color matches refColor then it is a 0 and all others are 1
            int[] columns = new int[xWidth];
            if (refColor == null)
            {
                convertType = BinPixelConvertType.ColorIsOne;
                refColor = Color.FromArgb(0, 0, 0); // Black
            }

            PixelData refColorPD;
            refColorPD.red = refColor.Value.R;
            refColorPD.blue = refColor.Value.B;
            refColorPD.green = refColor.Value.G;

            byte* pBaseTemp;

            if (convertType == BinPixelConvertType.ColorIsOne)
            {
                for (int y = 0; y < yHeight; y++)
                {
                    pBaseTemp = pBase + (y + yStart) * width + xStart * bytesPerPixel;
                    for (int x = 0; x < xWidth; x++)
                    {
                        if (*pBaseTemp==refColorPD.blue && *(pBaseTemp+1) == refColorPD.green && *(pBaseTemp+2) == refColorPD.red)
                            columns[x] |= 1 << y;
                        pBaseTemp += bytesPerPixel;
                    }
                }
            }
            else
            {
                for (int y = 0; y < yHeight; y++)
                {
                    pBaseTemp = pBase + (y + yStart) * width + xStart * bytesPerPixel;
                    for (int x = 0; x < xWidth; x++)
                    {
                        if (*pBaseTemp != refColorPD.blue && *(pBaseTemp+1) != refColorPD.green && *(pBaseTemp+2) != refColorPD.red)
                            columns[x] |= 1 << y;
                        pBaseTemp += bytesPerPixel;
                    }
                }
            }
            return columns;
        }

        public UnsafeBitmap Clone(Rectangle rect, PixelFormat format)
        {
            Bitmap bm = bitmap.Clone(rect, format);
            return new UnsafeBitmap(bm);
        }

        private void UpdateFormat()
        {
            if (bitmap.PixelFormat == PixelFormat.Format32bppPArgb || bitmap.PixelFormat == PixelFormat.Format32bppArgb || bitmap.PixelFormat == PixelFormat.Format32bppRgb)
            {
                bytesPerPixel = 4;
            }
        }

        public PixelFormat PixelFormat
        {
            get { return bitmap.PixelFormat; }
        }

        public int FindNextVerticalLine(int startIndex, Color c, bool equal)
        {
            // Finds next vertical line of the specified color
            // equal=false:  Find any vertical line with pixels all the same color 
            //               but the color does NOT match color "c".
            // equal=true:   Find vertical line with all pixels equal to color "c".

            // Finds non-white vertical line.  Returns -1 if not found.
            for (int x = startIndex; x < Width; x++)
            {
                bool lineFound = true;
                Color prev = GetPixel(x, 0);
                if (prev == c && !equal)
                    lineFound = false;
                if (prev != c && equal)
                    lineFound = false;
                for (int y = 1; y < Height && lineFound; y++)
                {
                    if (GetPixel(x, y) != prev)
                        lineFound = false;
                }

                if (lineFound)
                    return x;
            }

            return -1;
        }


        #region IDisposable Members
        // Track whether Dispose has been called.
        private bool disposed = false;

        private void Dispose(bool disposing)
        { 
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed resources
                if (disposing)
                {
                    // Dispose managed resources.
                    UnlockBitmap();
                    if (bitmap != null)
                        bitmap.Dispose();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~UnsafeBitmap()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion
    }
    public struct PixelData
    {
        public byte blue;
        public byte green;
        public byte red;
    }
}
