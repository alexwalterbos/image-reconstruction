using org.monalisa.algorithm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// This is the true artist. Give him an environment and a canvas
    /// with his soon to be drawn elements and then let the Paint()ing commence!
    /// </summary>
    public static class Painter
    {
        /// <summary>
        /// Paint a shape given the graphics handler for the element.
        /// </summary>
        /// <param name="element">Shape to draw.</param>
        /// <param name="gfx">Graphics handler.</param>
        private static void Paint(IShape element, Graphics gfx)
        {
            // Check if shape is a polygon
            var polygon = (IPolygon)element;
            if (polygon == null) throw new NotImplementedException("Only polygon shapes are implemented at this time");

            // Paint polygon shape
            Paint(polygon, gfx);
        }

        /// <summary>
        /// Paint a polygon given the graphics handler for the element.
        /// </summary>
        /// <param name="element">Polygon shape to draw.</param>
        private static void Paint(IPolygon element, Graphics gfx)
        {
            // Create a brush resource
            using (var brush = new SolidBrush(Color.FromArgb(element.Alpha, element.Red, element.Green, element.Blue)))
            {
                // Convert coordinates (tuple list => point array)
                var coordinates = element.Coordinates.Select(tuple => new Point(tuple.Item1, tuple.Item2)).ToArray();

                // Draw polygon using the just created brush.
                gfx.FillPolygon(brush, coordinates);
            }
        }

        /// <summary>
        /// Converts a canvas to a bitmap object. This object can be saved and
        /// viewed by Bitmap.Save() and is also used for direct displaying and
        /// for pixel-by-pixel image comparison.
        /// </summary>
        /// <param name="environment">The setup (used for canvas size).</param>
        /// <param name="canvas">Canvas with elements to paint.</param>
        public static Bitmap Paint(EvolutionaryAlgorithm environment, ICanvas canvas)
        {
            var image = new Bitmap(environment.CanvasWidth, environment.CanvasHeight);
            using (var gfx = Graphics.FromImage(image))
            {
                gfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver; // enable alpha blending
                //gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // enable AA
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality; // don't know
                foreach (var element in canvas.Elements) Paint(element, gfx);
            }
            return image;
        }
    }
}
