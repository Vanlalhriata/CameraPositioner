#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Result from an AR marker detection.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2010-02-23 23:35:56 +0000 (ter, 23 Fev 2010) $
//   Changed in:        $Revision: 48548 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKit/Detector/Results/DetectionResult.cs $
//   Id:                $Id: DetectionResult.cs 48548 2010-02-23 23:35:56Z unknown $
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using System.Windows.Media.Media3D;

namespace SLARToolKit
{
   /// <summary>
   /// Result from an AR marker detection.
   /// </summary>
   public class DetectionResult
   {
      /// <summary>
      /// A reference to the found marker.
      /// </summary>
      public Marker Marker { get; private set; }

      /// <summary>
      /// The confidence / quality  of the result (Is this really the marker?). The maximum value is 1.
      /// </summary>
      public double Confidence { get; private set; }

      /// <summary>
      /// The transformation matrix for the marker.
      /// </summary>
      public Matrix3D Transformation { get; private set; }

      /// <summary>
      /// The pixel coordinates where the square marker was found. 
      /// </summary>
      public Square Square { get; private set; }

      /// <summary>
      /// Creates a new detection result
      /// </summary>
      /// <param name="marker">A reference to the found marker.</param>
      /// <param name="confidence">The confidence / quality  of the result.</param>
      /// <param name="transformation">The transformation matrix for the marker.</param>
      /// <param name="square">The pixel coordinates where the square marker was found. </param>
      public DetectionResult(Marker marker, double confidence, Matrix3D transformation, Square square)
      {
         this.Marker          = marker;
         this.Confidence      = confidence;
         this.Transformation  = transformation;
         this.Square          = square;
      }
   }
}
