#region Header
//
//   Project:           SLARToolKit - Silverlight Augmented Reality Toolkit
//   Description:       An AR marker.
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
using System.IO;

namespace SLARToolKit
{
    /// <summary>
    /// An AR marker.
    /// </summary>
    public class Marker
    {
        /// <summary>
        /// The number of marker segments in x direction.
        /// </summary>
        public int SegmentsX { get; private set; }

        /// <summary>
        /// The number of marker segments in y direction.
        /// </summary>
        public int SegmentsY { get; private set; }

        /// <summary>
        /// The physical width of the marker in millimeters.
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// The underlying Ny marker NyARCode
        /// </summary>
        internal NyARCode NyMarker { get; private set; }

        /// <summary>
        /// The offset of the marker used for detection.
        /// </summary>
        internal NyARRectOffset RectOffset { get; private set; }

        /// <summary>
        /// The optional name of the marker.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a marker instance form the stream data.
        /// </summary>
        /// <param name="markerStream">The stream for the marker data.</param>
        /// <param name="segmentsX">The number of marker segments in x direction.</param>
        /// <param name="segmentsY">The number of marker segments in y direction.</param>
        /// <param name="width">The physical width of the marker in millimeters.</param>
        /// <returns>A new marker instance.</returns>
        public static Marker Load(Stream markerStream, int segmentsX, int segmentsY, double width)
        {
            // Load marker data with segments x segments pattern
            var marker = new Marker
            {
                SegmentsX = segmentsX,
                SegmentsY = segmentsY,
                Width = width,
                NyMarker = new NyARCode(segmentsX, segmentsY),
                RectOffset = new NyARRectOffset(),
            };
            marker.NyMarker.loadARPatt(markerStream);
            marker.RectOffset.setSquare(width);
            return marker;
        }

        /// <summary>
        /// Creates a marker instance from the stream data.
        /// </summary>
        /// <param name="markerStream">The stream for the marker data.</param>
        /// <param name="segmentsX">The number of marker segments in x direction.</param>
        /// <param name="segmentsY">The number of marker segments in y direction.</param>
        /// <param name="width">The physical width of the marker in millimeters.</param>
        /// <param name="name">The name the marker should get.</param>
        /// <returns>A new marker instance.</returns>
        public static Marker Load(Stream markerStream, int segmentsX, int segmentsY, double width, string name)
        {
            var marker = Load(markerStream, segmentsX, segmentsY, width);
            marker.Name = name;
            return marker;
        }

        /// <summary>
        /// Creates a marker instance from the applications resource file.
        /// </summary>
        /// <param name="relativePath">Only the relative path to the resource file. The assembly name is retrieved automatically.</param>
        /// <param name="segmentsX">The number of marker segments in x direction.</param>
        /// <param name="segmentsY">The number of marker segments in y direction.</param>
        /// <param name="width">The physical width of the marker in millimeters.</param>
        /// <returns>A new marker instance.</returns>
        /// <returns>The WriteableBitmap that was passed as parameter.</returns>
        public static Marker LoadFromResource(string relativePath, int segmentsX, int segmentsY, double width)
        {
            return LoadFromResource(relativePath, segmentsX, segmentsY, width, System.Reflection.Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Creates a marker instance from the applications resource file.
        /// </summary>
        /// <param name="relativePath">Only the relative path to the resource file. The assembly name is retrieved automatically.</param>
        /// <param name="segmentsX">The number of marker segments in x direction.</param>
        /// <param name="segmentsY">The number of marker segments in y direction.</param>
        /// <param name="width">The physical width of the marker in millimeters.</param>
        /// <param name="name">The name the marker should get.</param>
        /// <returns>A new marker instance.</returns>
        /// <returns>The WriteableBitmap that was passed as parameter.</returns>
        public static Marker LoadFromResource(string relativePath, int segmentsX, int segmentsY, double width, string name)
        {
            var marker = LoadFromResource(relativePath, segmentsX, segmentsY, width, System.Reflection.Assembly.GetCallingAssembly());
            marker.Name = name;
            return marker;
        }

        /// <summary>
        /// Creates a marker instance from the applications resource file.
        /// </summary>
        /// <param name="relativePath">Only the relative path to the resource file. The assembly name is retrieved automatically.</param>
        /// <param name="segmentsX">The number of marker segments in x direction.</param>
        /// <param name="segmentsY">The number of marker segments in y direction.</param>
        /// <param name="width">The physical width of the marker in millimeters.</param>
        /// <param name="assembly">The assembly that will be used to generate the right URI.</param>
        /// <returns>A new marker instance.</returns>
        /// <returns>The WriteableBitmap that was passed as parameter.</returns>
        private static Marker LoadFromResource(string relativePath, int segmentsX, int segmentsY, double width, System.Reflection.Assembly assembly)
        {
            string location = System.IO.Path.GetDirectoryName(assembly.Location);
            StreamReader reader = new StreamReader(Path.Combine(location, relativePath));
            using (var markerStream = reader.BaseStream)
            {
                return Load(markerStream, segmentsX, segmentsY, width);
            }

            //var asmName = new System.Reflection.AssemblyName(assembly.FullName).Name;
            //var uri = new Uri(asmName + ";component/" + relativePath, UriKind.Relative);
            //var resourceStream = Application.GetResourceStream(uri);

            //if (null == resourceStream)
            //    throw new FileNotFoundException("Application.GetResourceStream returned null", uri.OriginalString);

            //using (var markerStream = resourceStream.Stream)
            //{
            //    return Load(markerStream, segmentsX, segmentsY, width);
            //}
        }

        /// <summary>
        /// Returns the name of the marker.
        /// </summary>
        /// <returns>The name of the marker.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}