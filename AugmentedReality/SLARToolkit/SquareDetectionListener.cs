#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Square detection listener used for marker detetcion..
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2010-05-20 19:20:42 +0100 (qui, 20 Mai 2010) $
//   Changed in:        $Revision: 48548 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKit/Detector/SquareDetectionListener.cs $
//   Id:                $Id: SquareDetectionListener.cs 48548 2010-05-20 18:20:42Z unknown $
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion


using jp.nyatla.nyartoolkit.cs.core;
using System.Collections.Generic;

namespace SLARToolKit
{
   /// <summary>
   /// Square detection listener used for marker detetcion.
   /// </summary>
   internal class SquareDetectionListener : NyARSquareContourDetector.IDetectMarkerCallback
   {
      private List<PatternMatcher> patternMatchers;
      private INyARColorPatt colorPattern;
      private NyARMatchPattDeviationColorData patternMatchDeviationData;
      private Coord2Linear coordinationMapper;
      private NyARTransMat matrixCalculator;
      private NyARIntPoint2d[] points;
      private NyARMatchPattResult evaluationResult;

      /// <summary>
      /// The bitmap buffer that is searched for the markers.
      /// </summary>
      public INyARRgbRaster Buffer { get; set; }

      /// <summary>
      /// The detection results
      /// </summary>
      public DetectionResults Results { get; private set; }

      /// <summary>
      /// Initialize a new SquareDetectionListener.
      /// </summary>
      /// <param name="patternMatchers">The pattern matchers with the marker data.</param>
      /// <param name="cameraParameters">The camera calibration data.</param>
      /// <param name="colorPattern">The used color pattern.</param>
      /// <param name="patternMatchDeviationData">The pattern match deviation data.</param>
      public SquareDetectionListener(List<PatternMatcher> patternMatchers, NyARParam cameraParameters, INyARColorPatt colorPattern, NyARMatchPattDeviationColorData patternMatchDeviationData)
      {
         this.patternMatchers = patternMatchers;
         this.colorPattern = colorPattern;
         this.patternMatchDeviationData = patternMatchDeviationData;
         this.coordinationMapper = new Coord2Linear(cameraParameters.getScreenSize(), cameraParameters.getDistortionFactor());
         this.matrixCalculator = new NyARTransMat(cameraParameters);
         this.points = NyARIntPoint2d.createArray(4);
         this.evaluationResult = new NyARMatchPattResult();

         Reset();
      }

      /// <summary>
      /// Resets the Results
      /// </summary>
      public void Reset()
      {
         this.Results = new DetectionResults();
      }

      /// <summary>
      /// Listener method called when something was detected.
      /// </summary>
      /// <param name="callingDetector">The detector that called the method.</param>
      /// <param name="coordsX">The four x coordinates of the detected marker square.</param>
      /// <param name="coordsY">The four y coordinates of the detected marker square.</param>
      /// <param name="coordCount">The number of coordinates.</param>
      /// <param name="coordIndices">The indices of the coordiantes in the coords array.</param>
      public void onSquareDetect(NyARSquareContourDetector callingDetector, int[] coordsX, int[] coordsY, int coordCount, int[] coordIndices)
      {
         // Init variables            
         points[0].x = coordsX[coordIndices[0]];
         points[0].y = coordsY[coordIndices[0]];
         points[1].x = coordsX[coordIndices[1]];
         points[1].y = coordsY[coordIndices[1]];
         points[2].x = coordsX[coordIndices[2]];
         points[2].y = coordsY[coordIndices[2]];
         points[3].x = coordsX[coordIndices[3]];
         points[3].y = coordsY[coordIndices[3]];

         // Evaluate and find best match
         if (this.colorPattern.pickFromRaster(this.Buffer, points))
         {
            // Find best matching marker
            this.patternMatchDeviationData.setRaster(this.colorPattern);
            Marker foundMarker = null;
            int foundDirection = NyARMatchPattResult.DIRECTION_UNKNOWN;
            double bestConfidence = 0;

            foreach (var patMat in patternMatchers)
            {
               // Evaluate
               patMat.evaluate(this.patternMatchDeviationData, evaluationResult);

               // Best match?
               if (evaluationResult.confidence > bestConfidence)
               {
                  foundMarker = patMat.Marker;
                  foundDirection = evaluationResult.direction;
                  bestConfidence = evaluationResult.confidence;
               }
            }

            // Calculate found marker square
            var square = new NyARSquare();
            var len = coordIndices.Length;
            for (int i = 0; i < len; i++)
            {
               int idx = (i + len - foundDirection) % len;
               this.coordinationMapper.coord2Line(coordIndices[idx], coordIndices[(idx + 1) % len], coordsX, coordsY, coordCount, square.line[i]);
            }
            // Calculate normal
            for (int i = 0; i < len; i++)
            {
               NyARLinear.crossPos(square.line[i], square.line[(i + 3) % len], square.sqvertex[i]);
            }

            // Calculate matrix using continued mode
            if (foundMarker != null)
            {
               var nymat = new NyARTransMatResult();
               matrixCalculator.transMatContinue(square, foundMarker.RectOffset, nymat);

               // Create and add result to collection
               var v = square.sqvertex;
               var resultSquare = new Square(v[0].x, v[0].y, v[1].x, v[1].y, v[2].x, v[2].y, v[3].x, v[3].y);
               Results.Add(new DetectionResult(foundMarker, bestConfidence, nymat.ToMatrix3D(), resultSquare));
            }
         }
      }
   }
}