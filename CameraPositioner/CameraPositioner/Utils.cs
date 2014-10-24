using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace CameraPositioner
{
    public class Utils
    {

        public static double GetPointsDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }


        public static Matrix ToXnaMatrix(System.Windows.Media.Media3D.Matrix3D matrix)
        {
            var m = new Matrix(
               (float)matrix.M11, (float)matrix.M12, (float)matrix.M13, (float)matrix.M14,
               (float)matrix.M21, (float)matrix.M22, (float)matrix.M23, (float)matrix.M24,
               (float)matrix.M31, (float)matrix.M32, (float)matrix.M33, (float)matrix.M34,
               (float)matrix.OffsetX, (float)matrix.OffsetY, (float)matrix.OffsetZ, (float)matrix.M44);

            return m;
        }

        public static float AnimateTo(float value, float target, float elapsedSeconds, float durationSeconds)
        {
            if (value == target)
                return value;

            float del = elapsedSeconds / durationSeconds;
            int direction = value < target ? 1 : -1;

            value += del * direction;
            return MathHelper.Clamp(value, 0, 1);
        }

        public static void LogException(Exception e, bool isInnerException = false)
        {
            string n = Environment.NewLine;

            using (StreamWriter writer = new StreamWriter("ErrorLog.txt", isInnerException))
            {
                writer.WriteLine("{0:yyyy-MMM-dd HH:mm:ss}", DateTime.Now);
                writer.WriteLine(e.GetType());
                writer.WriteLine(e.Message);
                writer.WriteLine(n + "Stack trace:" + n);
                writer.WriteLine(e.StackTrace);
                writer.WriteLine(n + "---------------------" + n);

                writer.Close();
            }

            if (null != e.InnerException)
                LogException(e.InnerException, true);
        }

    }
}
