#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Abstract base class of a marker detector.
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

using jp.nyatla.nyartoolkit.cs.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;

namespace SLARToolKit
{
    /// <summary>
    /// Abstract base class of a marker detector.
    /// </summary>
    public abstract class AbstractMarkerDetector
    {
        private DetectionResults previousResults;

        /// <summary>
        /// The filtered buffer.
        /// </summary>
        protected NyARBinRaster filteredBuffer;

        /// <summary>
        /// The width of the bitmap buffer that is used for detection in screen coordinates.
        /// </summary>
        protected int bufferWidth;

        /// <summary>
        /// The height of the bitmap buffer that is used for detection in screen coordinates.
        /// </summary>
        protected int bufferHeight;

        /// <summary>
        /// Use adaptive thresholding.
        /// </summary>
        protected bool isAdaptive;

        private INyARRasterFilter_Rgb2Bin bufferFilter;
        private NyARSquareContourDetector squareDetector;
        private SquareDetectionListener squareDetectionListener;

        /// <summary>
        /// A right-handed perspective transformation matrix built from the camera calibration data.
        /// </summary>
        public Matrix3D Projection { get; private set; }

        /// <summary>
        /// The marker detection threshold. Default is 150.
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// Each frame the results are compared with the previous results.
        /// If the distance is above this threshold, the new result is returned, otherwise the old. 
        /// This prevents jittering;
        /// Default is 0.
        /// </summary>
        public int JitteringThreshold { get; set; }

        /// <summary>
        /// Creates a new instance of the AbstractMarkerDetector.
        /// </summary>
        protected AbstractMarkerDetector()
        {
            Threshold = 150;
            JitteringThreshold = 0;
        }

        /// <summary>
        /// Initializes the detector for single marker detection.
        /// </summary>
        /// <param name="width">The width of the buffer that will be used for detection.</param>
        /// <param name="height">The height of the buffer that will be used for detection.</param>
        /// <param name="nearPlane">The near view plane of the frustum.</param>
        /// <param name="farPlane">The far view plane of the frustum.</param>
        /// <param name="markers">A list of markers that should be detected.</param>
        /// <param name="bufferType">The type of the buffer.</param>
        /// <param name="adaptive">Performs an adaptive bitmap thresholding if set to true. Default = false.</param>
        protected void Initialize(int width, int height, double nearPlane, double farPlane, IList<Marker> markers, int bufferType, bool adaptive = false)
        {
            // Check arguments
            if (markers == null || !markers.Any())
            {
                throw new ArgumentNullException("markers");
            }

            // Member init
            this.bufferWidth = width;
            this.bufferHeight = height;
            this.isAdaptive = adaptive;

            // Init pattern matchers with markers and check segment size, whcih has to be equal for all markers
            int segmentX = markers[0].SegmentsX;
            int segmentY = markers[0].SegmentsY;
            var patternMatchers = new List<PatternMatcher>(markers.Count);
            foreach (var marker in markers)
            {
                if (marker.SegmentsX != segmentX || marker.SegmentsY != segmentY)
                {
                    throw new ArgumentException("The Segment size has to be equal for all markers. Don't mix 16x16 and 32x32 markers for example.", "markers");
                }
                patternMatchers.Add(new PatternMatcher(marker));
            }

            // Load deafult camera calibration data
            string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            StreamReader reader = new StreamReader(location + "/Content/Data/Camera_Calibration_1280x720.dat");
            var cameraParameters = new NyARParam();
            using (var cameraCalibrationDataStream = reader.BaseStream)
            {
                cameraParameters.loadARParam(cameraCalibrationDataStream);
                cameraParameters.changeScreenSize(width, height);
            }

            //var asmName = new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName).Name;

            //var uri = new Uri(asmName + ";component/Assets/data/Camera_Calibration_1280x720.dat", UriKind.Relative);
            //var streamResInfoCam = Application.GetResourceStream(uri);
            //if (null == streamResInfoCam)
            //    throw new FileNotFoundException("Application.GetResourceStream returned null", uri.OriginalString);

            //var cameraParameters = new NyARParam();
            //using (var cameraCalibrationDataStream = streamResInfoCam.Stream)
            //{
            //    cameraParameters.loadARParam(cameraCalibrationDataStream);
            //    cameraParameters.changeScreenSize(width, height);
            //}

            // Get projection matrix from camera calibration data
            this.Projection = cameraParameters.GetCameraFrustumRH(nearPlane, farPlane);

            // Init detector and necessary data
            var colorPattern = new NyARColorPatt_Perspective_O2(segmentX, segmentY, 4, 25);
            var patternMatchDeviationData = new NyARMatchPattDeviationColorData(segmentX, segmentY);
            this.squareDetector = new NyARSquareContourDetector_Rle(cameraParameters.getScreenSize());
            this.squareDetectionListener = new SquareDetectionListener(patternMatchers, cameraParameters, colorPattern, patternMatchDeviationData);

            // Init buffer members 
            this.filteredBuffer = new NyARBinRaster(width, height);
            if (adaptive)
            {
                this.bufferFilter = new NyARRasterFilter_AdaptiveThreshold(bufferType);
            }
            else
            {
                this.bufferFilter = new NyARRasterFilter_ARToolkitThreshold(this.Threshold, bufferType);
            }
        }

        /// <summary>
        /// Detects all markers in the buffer.
        /// </summary>
        /// <param name="buffer">The buffer which should be searched for markers.</param>
        /// <returns>The results of the detection.</returns>
        protected DetectionResults DetectAllMarkers(INyARRgbRaster buffer)
        {
            // Filter buffer
            if (!isAdaptive)
            {
                ((NyARRasterFilter_ARToolkitThreshold)this.bufferFilter).setThreshold(this.Threshold);
            }

            this.bufferFilter.doFilter(buffer, this.filteredBuffer);

            // Detect and return results
            this.squareDetectionListener.Reset();
            this.squareDetectionListener.Buffer = buffer;
            this.squareDetector.detectMarkerCB(this.filteredBuffer, squareDetectionListener);
            var results = this.squareDetectionListener.Results;

            // Prevent jittering
            if (previousResults != null && JitteringThreshold != 0)
            {
                var jitThresholdSq = JitteringThreshold * JitteringThreshold;
                var newResults = new DetectionResults();
                foreach (var result in results)
                {
                    var previousResult = previousResults.FirstOrDefault(r => r.Marker == result.Marker);
                    if (previousResult == null)
                    {
                        newResults.Add(result);
                    }
                    else
                    {
                        var pcenter = previousResult.Square.Center;
                        var center = result.Square.Center;
                        var dx = pcenter.X - center.X;
                        var dy = pcenter.X - center.X;
                        var lenSq = dx * dx + dy * dy;

                        newResults.Add(lenSq > jitThresholdSq ? result : previousResult);
                    }
                }
                previousResults = newResults;
                return newResults;
            }
            previousResults = results;

            return results;
        }
    }
}
