#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       NyAR Raster implementation for the WriteableBitmap
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2011-05-24 21:13:11 +0100 (ter, 24 Mai 2011) $
//   Changed in:        $Revision: 65834 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKit/Buffer/XrgbIRaster.cs $
//   Id:                $Id: XrgbIRaster.cs 65834 2011-05-24 20:13:11Z unknown $
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using jp.nyatla.nyartoolkit.cs.core;

namespace SLARToolKit
{
    /// <summary>
    /// NyAR Raster implementation for the IXrgReader
    /// </summary>
    internal class XrgbIRaster : NyARRgbRaster_BasicClass
    {
        /// <summary>
        /// The Buffer type (BYTE1D_X8R8G8B8_32)
        /// </summary>
        public const int BufferType = NyARBufferType.BYTE1D_X8R8G8B8_32;

        private ArgbPixelReader pixelReader;
        private IXrgbReader xrgbReader;
        private byte[] byteBuffer;

        /// <summary>
        /// The data buffer.
        /// </summary>
        public IXrgbReader XrgbReader
        {
            get { return xrgbReader; }
            set
            {
                xrgbReader = value;
                if (_size.w != xrgbReader.Width || _size.h != xrgbReader.Height)
                {
                    _size = new NyARIntSize(xrgbReader.Width, xrgbReader.Height);
                }
                SetUpReaders();
            }
        }

        /// <summary>
        /// Initializes a new WriteableBitmap buffer,
        /// </summary>
        /// <param name="width">The width of the buffer that will be used for detection.</param>
        /// <param name="height">The height of the buffer that will be used for detection.</param>
        public XrgbIRaster(int width, int height)
            : base(width, height, BufferType)
        {
        }

        /// <summary>
        /// Initializes a new WriteableBitmap buffer,
        /// </summary>
        /// <param name="xrgbReader">The buffer reader.</param>
        public XrgbIRaster(IXrgbReader xrgbReader)
            : this(xrgbReader.Width, xrgbReader.Height)
        {
            this.xrgbReader = xrgbReader;
        }

        /// <summary>
        /// Gets the RGB pixel reader implementation.
        /// </summary>
        /// <returns>The RGB pixel reader implementation.</returns>
        public override INyARRgbPixelReader getRgbPixelReader()
        {
            return pixelReader;
        }

        /// <summary>
        /// Convert to ARGB byte buffer and init the readers.
        /// </summary>
        private void SetUpReaders()
        {
            // The int[] is copied using the fast Buffer.BlockCopy
            // Format INT1D_X8R8G8B8_32 is not used directly due to some detection issues. Maybe a NyARToolkit bug.

            // Init ARGB byte buffer
            var w = _size.w;
            var h = _size.h;
            byteBuffer = xrgbReader.GetAllPixelsAsByte();

            // Init readers
            pixelReader = new ArgbPixelReader(byteBuffer, w, h);
        }

        /// <summary>
        /// Returns the internal ARGB byte buffer.
        /// </summary>
        /// <returns></returns>
        public override object getBuffer()
        {
            return byteBuffer;
        }

        /// <summary>
        /// Determines if this instance has an internal buffer.
        /// </summary>
        /// <returns></returns>
        public override bool hasBuffer()
        {
            return byteBuffer != null;
        }

        /// <summary>
        /// Changes the internal buffer to another ARGB byte buffer.
        /// Actually not used. Only to satisfy the interface.
        /// </summary>
        /// <param name="newBuffer">The new buffer that should be used.</param>
        public override void wrapBuffer(object newBuffer)
        {
            byteBuffer = (byte[])newBuffer;
        }
    }
}