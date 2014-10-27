using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AugmentedReality
{
    internal class Utils
    {
        internal static byte[] FlipHorizontalGrayscale(byte[] buffer, int width, int height)
        {
            byte[] result = new byte[buffer.Length];
            Rectangle imageBounds = new Rectangle(0, 0, width, height);

            // Assume our buffer contains 8bpp data
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

            // Copy from buffer to bitmap
            BitmapData bitmapData = bitmap.LockBits(imageBounds, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            IntPtr ptr = bitmapData.Scan0;
            Marshal.Copy(buffer, 0, ptr, buffer.Length);
            bitmap.UnlockBits(bitmapData);

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);

            // Copy bitmap to result after flipping/rotating
            bitmapData = bitmap.LockBits(imageBounds, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            ptr = bitmapData.Scan0;
            Marshal.Copy(ptr, result, 0, result.Length);
            bitmap.UnlockBits(bitmapData);

            return result;
        }
    }
}
