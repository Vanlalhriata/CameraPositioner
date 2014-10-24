#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Provides data in XRGB format from the WriteableBitmap.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

namespace SLARToolKit
{
    /// <summary>
    /// Provides data in XRGB format from the gray byte buffer.
    /// </summary>
    internal class GrayBufferReader : IXrgbReader
    {
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Thw width of the buffer.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the buffer.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets the color as XRGB byte components for the given x and y coordinate. 
        /// Only the RGB part will actually be used.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The color at the x and y coordinate.</returns>
        public int GetPixel(int x, int y)
        {
            var g = Buffer[y * Width + x];
            return (g << 24) | (g << 16) | (g << 8) | (g);
        }

        /// <summary>
        /// Gets the color as XRGB byte components for a set of x and y coordinates. 
        /// Only the RGB part will actually be used.
        /// </summary>
        /// <param name="x">The set of x coordinates.</param>
        /// <param name="y">The set of y coordinates.</param>
        /// <returns>The color at the x and y coordinates.</returns>
        public int[] GetPixels(int[] x, int[] y)
        {
            if (x.Length != y.Length)
            {
                throw new System.ArgumentException("The length oy the x coordinate set is not equal to the y set.", "x, y");
            }

            var buffer = Buffer;
            var w = Width;
            var l = x.Length;
            var r = new int[l];

            for (var i = 0; i < l; i++)
            {
                var g = buffer[y[i] * w + x[i]];
                r[i] = (g << 24) | (g << 16) | (g << 8) | (g);
            }

            return r;
        }

        /// <summary>
        /// Gets the color as XRGB byte components for the whole bitmap. 
        /// Only the RGB part will actually be used.
        /// </summary>
        /// <returns>The color for the whole bitmap as int array.</returns>
        public int[] GetAllPixels()
        {
            var buffer = Buffer;
            var length = buffer.Length;
            var result = new int[length];

            for (var i = 0; i < length; i++)
            {
                var g = buffer[i];
                result[i] = (g << 24) | (g << 16) | (g << 8) | (g);
            }

            return result;
        }

        /// <summary>
        /// Gets the color as XRGB bytes for the whole bitmap. 
        /// Only the RGB part will actually be used.
        /// </summary>
        /// <returns>The color for the whole bitmap as int array.</returns>
        public byte[] GetAllPixelsAsByte()
        {
            var buffer = Buffer;
            var length = buffer.Length;
            var result = new byte[length << 2];
            var i2 = 0;

            for (var i = 0; i < length; i++, i2 += 4)
            {
                var g = buffer[i];
                result[i2] = g;
                result[i2 + 1] = g;
                result[i2 + 2] = g;
                result[i2 + 3] = g;
            }

            return result;
        }
    }
}