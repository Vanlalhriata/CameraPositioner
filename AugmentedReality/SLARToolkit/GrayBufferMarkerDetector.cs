#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Marker detector that searches markers in a WriteableBitmap.
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

using System;
using System.Collections.Generic;

namespace SLARToolKit
{
    /// <summary>
    /// Marker detector that searches markers in a byte gray buffer, usually the Y.
    /// </summary>
    public class GrayBufferMarkerDetector : AbstractMarkerDetector
    {
        private GrayBufferReader grayBufferReader;
        private XrgbIRaster raster;

        /// <summary>
        /// Creates a new instance of the GrayBufferMarkerDetector.
        /// </summary>
        public GrayBufferMarkerDetector()
        {
            grayBufferReader = new GrayBufferReader();
        }

        /// <summary>
        /// Creates a new instance of the GrayBufferMarkerDetector.
        /// </summary>
        /// <param name="width">The width of the buffer that will be used for detection.</param>
        /// <param name="height">The height of the buffer that will be used for detection.</param>
        /// <param name="nearPlane">The near view plane of the frustum.</param>
        /// <param name="farPlane">The far view plane of the frustum.</param>
        /// <param name="markers">A list of markers that should be detected.</param>
        public GrayBufferMarkerDetector(int width, int height, double nearPlane, double farPlane, IList<Marker> markers)
        {
            Initialize(width, height, nearPlane, farPlane, markers);
        }

        /// <summary>
        /// Initializes the detector for multiple marker detection.
        /// </summary>
        /// <param name="width">The width of the buffer that will be used for detection.</param>
        /// <param name="height">The height of the buffer that will be used for detection.</param>
        /// <param name="nearPlane">The near view plane of the frustum.</param>
        /// <param name="farPlane">The far view plane of the frustum.</param>
        /// <param name="markers">A list of markers that should be detected.</param>
        public void Initialize(int width, int height, double nearPlane, double farPlane, IList<Marker> markers)
        {
            raster = new XrgbIRaster(width, height);
            Initialize(width, height, nearPlane, farPlane, markers, XrgbIRaster.BufferType);
        }

        /// <summary>
        /// Initializes the detector for single marker detection.
        /// </summary>
        /// <param name="width">The width of the buffer that will be used for detection.</param>
        /// <param name="height">The height of the buffer that will be used for detection.</param>
        /// <param name="nearPlane">The near view plane of the frustum.</param>
        /// <param name="farPlane">The far view plane of the frustum.</param>
        /// <param name="markers">Marker(s) that should be detected.</param>
        public void Initialize(int width, int height, double nearPlane, double farPlane, params Marker[] markers)
        {
            Initialize(width, height, nearPlane, farPlane, new List<Marker>(markers));
        }

        /// <summary>
        /// Detects all markers in the buffer.
        /// </summary>
        /// <param name="buffer">The buffer containing byte brightness values which should be searched for markers.</param>
        /// <param name="width">The width of the buffer.</param>
        /// <param name="height">The height of the buffer.</param>
        /// <returns>The results of the detection.</returns>
        public DetectionResults DetectAllMarkers(byte[] buffer, int width, int height)
        {
            // Check argument
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            // Update buffer and check size
            grayBufferReader.Buffer = buffer;
            grayBufferReader.Width = width;
            grayBufferReader.Height = height;
            raster.XrgbReader = grayBufferReader;
            if (!filteredBuffer.getSize().isEqualSize(raster.getSize()))
            {
                throw new ArgumentException("The size of the GrayReader differs from the initialized size.", "buffer");
            }

            // Detect markers
            return DetectAllMarkers(raster);
        }
    }
}