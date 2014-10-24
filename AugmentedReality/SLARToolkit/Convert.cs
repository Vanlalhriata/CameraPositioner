#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       Convert methods.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2011-04-13 20:19:56 +0100 (qua, 13 Abr 2011) $
//   Changed in:        $Revision: 63951 $
//   Project:           $URL: https://slartoolkit.svn.codeplex.com/svn/trunk/SLARToolKit/Source/SLARToolKit/Convert.cs $
//   Id:                $Id: Convert.cs 63951 2011-04-13 19:19:56Z unknown $
//
//
//   Copyright (c) 2009-2010 Rene Schulte
//
//   This program is open source software. Please read the License.txt.
//
#endregion

using jp.nyatla.nyartoolkit.cs.core;
using System.Windows.Media.Media3D;

namespace SLARToolKit
{   
   /// <summary>
   /// Convert methods.
   /// </summary>
   public static partial class Convert
   {
      /// <summary>
      /// Returns a Matrix3D from the NyARDoubleMatrix34 that is transposed and swapped.
      /// </summary>
      /// <param name="nymatrix">The matrix that should be converted.</param>
      /// <returns>The converted matrix.</returns>
      internal static Matrix3D ToMatrix3D(this NyARDoubleMatrix34 nymatrix)
      {
         return new Matrix3D
         (
            nymatrix.m00, -nymatrix.m10, -nymatrix.m20, 0,
            nymatrix.m01, -nymatrix.m11, -nymatrix.m21, 0,
            nymatrix.m02, -nymatrix.m12, -nymatrix.m22, 0,
            nymatrix.m03, -nymatrix.m13, -nymatrix.m23, 1
         );
      }

      /// <summary>
      /// Returns a right-handed perspective transformation matrix built from the camera calibration data.
      /// </summary>
      /// <param name="arParameters">The camera calibration data.</param>
      /// <param name="nearPlane">The near view plane of the frustum.</param>
      /// <param name="farPlane">The far view plane of the frustum.</param>
      /// <returns>The projection matrix.</returns>
      internal static Matrix3D GetCameraFrustumRH(this NyARParam arParameters, double nearPlane, double farPlane)
      {
         NyARMat transformation = new NyARMat(3, 4);
         NyARMat icParameters = new NyARMat(3, 4);
         double[,] p = new double[3, 3];
         double[,] q = new double[4, 4];

         NyARIntSize size = arParameters.getScreenSize();
         int width = size.w;
         int height = size.h;

         arParameters.getPerspectiveProjectionMatrix().decompMat(icParameters, transformation);

         double[][] icpara = icParameters.getArray();
         double[][] trans = transformation.getArray();
         for (int i = 0; i < 4; i++)
         {
            icpara[1][i] = (height - 1) * (icpara[2][i]) - icpara[1][i];
         }

         for (int i = 0; i < 3; i++)
         {
            for (int j = 0; j < 3; j++)
            {
               p[i, j] = icpara[i][j] / icpara[2][2];
            }
         }

         q[0, 0] = (2.0 * p[0, 0] / (width - 1));
         q[0, 1] = (2.0 * p[0, 1] / (width - 1));
         q[0, 2] = ((2.0 * p[0, 2] / (width - 1)) - 1.0);
         q[0, 3] = 0.0;

         q[1, 0] = 0.0;
         q[1, 1] = -(2.0 * p[1, 1] / (height - 1));
         q[1, 2] = -((2.0 * p[1, 2] / (height - 1)) - 1.0);
         q[1, 3] = 0.0;

         q[2, 0] = 0.0;
         q[2, 1] = 0.0;
         q[2, 2] = (farPlane + nearPlane) / (nearPlane - farPlane);
         q[2, 3] = 2.0 * farPlane * nearPlane / (nearPlane - farPlane);

         q[3, 0] = 0.0;
         q[3, 1] = 0.0;
         q[3, 2] = -1.0;
         q[3, 3] = 0.0;

         return new Matrix3D( q[0, 0] * trans[0][0] + q[0, 1] * trans[1][0] + q[0, 2] * trans[2][0],
                              q[1, 0] * trans[0][0] + q[1, 1] * trans[1][0] + q[1, 2] * trans[2][0],
                              q[2, 0] * trans[0][0] + q[2, 1] * trans[1][0] + q[2, 2] * trans[2][0],
                              q[3, 0] * trans[0][0] + q[3, 1] * trans[1][0] + q[3, 2] * trans[2][0],
                              q[0, 0] * trans[0][1] + q[0, 1] * trans[1][1] + q[0, 2] * trans[2][1],
                              q[1, 0] * trans[0][1] + q[1, 1] * trans[1][1] + q[1, 2] * trans[2][1],
                              q[2, 0] * trans[0][1] + q[2, 1] * trans[1][1] + q[2, 2] * trans[2][1],
                              q[3, 0] * trans[0][1] + q[3, 1] * trans[1][1] + q[3, 2] * trans[2][1],
                              q[0, 0] * trans[0][2] + q[0, 1] * trans[1][2] + q[0, 2] * trans[2][2],
                              q[1, 0] * trans[0][2] + q[1, 1] * trans[1][2] + q[1, 2] * trans[2][2],
                              q[2, 0] * trans[0][2] + q[2, 1] * trans[1][2] + q[2, 2] * trans[2][2],
                              q[3, 0] * trans[0][2] + q[3, 1] * trans[1][2] + q[3, 2] * trans[2][2],
                              q[0, 0] * trans[0][3] + q[0, 1] * trans[1][3] + q[0, 2] * trans[2][3] + q[0, 3],
                              q[1, 0] * trans[0][3] + q[1, 1] * trans[1][3] + q[1, 2] * trans[2][3] + q[1, 3],
                              q[2, 0] * trans[0][3] + q[2, 1] * trans[1][3] + q[2, 2] * trans[2][3] + q[2, 3],
                              q[3, 0] * trans[0][3] + q[3, 1] * trans[1][3] + q[3, 2] * trans[2][3] + q[3, 3]);
      }
   }
}
