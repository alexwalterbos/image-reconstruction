//-----------------------------------------------------------------------------
// <copyright file="BitmapHelper.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Helper class for converting bitmap from and to byte array form
    /// </summary>
    public static class BitmapHelper
    {
        /// <summary>
        /// Convert a bitmap to byte array
        /// </summary>
        /// <param name="bmp">Bitmap to convert</param>
        /// <returns>Byte array representing the bitmap</returns>
        public static byte[] AsByteArray(this Bitmap bmp)
        {
            if (bmp == null)
            {
                throw new NullReferenceException("Bitmap is null");
            }

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = data.Scan0;

            // declare an array to hold the bytes of the bitmap
            int numBytes = data.Stride * bmp.Height;
            byte[] bytes = new byte[numBytes];

            // copy the RGB values into the array
            System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, numBytes);

            bmp.UnlockBits(data);

            return bytes;
        }

        /// <summary>
        /// Convert a byte array to a grey-valued bitmap (8 bits per pixel)
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <param name="width">Canvas width</param>
        /// <param name="height">Canvas height</param>
        /// <returns>A 8 bits per pixel grey-valued bitmap</returns>
        public static Bitmap AsBitmap(this byte[] bytes, int width, int height)
        {
            Bitmap grayBmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            Rectangle grayRect = new Rectangle(0, 0, grayBmp.Width, grayBmp.Height);
            BitmapData grayData = grayBmp.LockBits(grayRect, ImageLockMode.ReadWrite, grayBmp.PixelFormat);
            IntPtr grayPtr = grayData.Scan0;

            int grayBytes = grayData.Stride * grayBmp.Height;
            ColorPalette pal = grayBmp.Palette;

            for (int g = 0; g < 256; g++)
            {
                pal.Entries[g] = Color.FromArgb(g, g, g);
            }

            grayBmp.Palette = pal;

            System.Runtime.InteropServices.Marshal.Copy(bytes, 0, grayPtr, grayBytes);

            grayBmp.UnlockBits(grayData);
            return grayBmp;
        }
    }
}
