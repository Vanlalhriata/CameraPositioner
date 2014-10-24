#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       NyAR PixelReader implementaion for ARGB byte buffer
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2010-02-23 23:35:56 +0000 (ter, 23 Fev 2010) $
//   Changed in:        $Revision: 48548 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKit/Buffer/ArgbPixelReader.cs $
//   Id:                $Id: ArgbPixelReader.cs 48548 2010-02-23 23:35:56Z unknown $
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System;

using jp.nyatla.nyartoolkit.cs.core;

namespace SLARToolKit
{
   /// <summary>
   /// NyAR PixelReader implementaion for ARGB byte buffer
   /// </summary>
   internal class ArgbPixelReader : INyARRgbPixelReader
   {
      private byte[] buffer;
      private int height;
      private int width;

      /// <summary>
      /// The ARGB byte Buffer.
      /// </summary>
      public byte[] Buffer 
      {
         get { return buffer; }
         set { buffer = value; }
      }

      /// <summary>
      /// Initializes a new ArgbPixelReader.
      /// </summary>
      /// <param name="width">The width of the buffer in pixels.</param>
      /// <param name="height">The height of the buffer in pixels.</param>
      public ArgbPixelReader(int width, int height)
      {
         this.width = width;
         this.height = height;
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="buffer">The ARGB byte buffer.</param>
      /// <param name="width">The width of the buffer in pixels.</param>
      /// <param name="height">The height of the buffer in pixels.</param>
      public ArgbPixelReader(byte[] buffer, int width, int height)
         : this(width, height)
      {
         this.buffer = buffer;
      }

      #region INyARRgbPixelReader Member

      /// <summary>
      /// Gets the RGB color of a single pixel.
      /// </summary>
      /// <param name="x">The x coordinate.</param>
      /// <param name="y">The y coordinate.</param>
      /// <param name="rgb">The out paramter for the color as RGB.</param>
      public void getPixel(int x, int y, int[] rgb)
      {
         int offset = (x + y * width) << 2;
         rgb[0] = buffer[offset + 1];   // R
         rgb[1] = buffer[offset + 2];   // G
         rgb[2] = buffer[offset + 3];   // B
      }

      /// <summary>
      /// Gets the RGB color of a pixel set.
      /// </summary>
      /// <param name="x">The x coordinates.</param>
      /// <param name="y">The y coordinates.</param>
      /// <param name="len">The length of the pixels to get.</param>
      /// <param name="rgb">The out paramter for the colors as RGB.</param>
      public void getPixelSet(int[] x, int[] y, int len, int[] rgb)
      {
         for (int i = 0; i < len; i++)
         {
            int offset = (x[i] + y[i] * width) << 2;
            int baseIdx = i * 3;
            rgb[baseIdx + 0] = buffer[offset + 1]; // R
            rgb[baseIdx + 1] = buffer[offset + 2]; // G
            rgb[baseIdx + 2] = buffer[offset + 3]; // B
         }
      }

      /// <summary>
      /// Sets the RGB color of a single pixel.
      /// </summary>
      /// <param name="x">The x coordinate.</param>
      /// <param name="y">The y coordinate.</param>
      /// <param name="rgb">The color as RGB in the range 0, 255.</param>
      public void setPixel(int x, int y, int[] rgb)
      {
         int offset = (x + y * width) << 2;
         buffer[offset + 1] = (byte)rgb[0];   // R
         buffer[offset + 2] = (byte)rgb[1];   // G
         buffer[offset + 3] = (byte)rgb[2];   // B
      }

      /// <summary>
      /// Sets the RGB color of a pixel set.
      /// </summary>
      /// <param name="x">The x coordinates.</param>
      /// <param name="y">The y coordinates.</param>
      /// <param name="len">The length of the pixels to set.</param>
      /// <param name="rgb">The color as RGB in the range 0, 255.</param>
      public void setPixels(int[] x, int[] y, int len, int[] rgb)
      {
         for (int i = 0; i < len; i++)
         {
            int offset = (x[i] + y[i] * width) << 2;
            int baseIdx = i * 3;
            buffer[offset + 1] = (byte)rgb[baseIdx + 0]; // R
            buffer[offset + 2] = (byte)rgb[baseIdx + 1]; // G
            buffer[offset + 3] = (byte)rgb[baseIdx + 2]; // B
         }
      }

      /// <summary>
      /// Changes the internal ARGB byte buffer to another buffer.
      /// Actually not used. Only to satisfy the interface.
      /// </summary>
      /// <param name="newBuffer">The new buffer that should be used.</param>
      public void switchBuffer(object newBuffer)
      {
         this.Buffer = (byte[])newBuffer;
      }

      #endregion
   }
}
