//-----------------------------------------------------------------------------
// <copyright file="Painter.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;

    /// <summary>
    /// This is the true artist. Give him an environment and a canvas with 
    /// his soon to be drawn elements and then let the Paint() commence!
    /// </summary>
    public static class Painter
    {
        /// <summary>
        /// Converts a canvas to a bitmap object. This object can be saved and
        /// viewed by Bitmap.Save() and is also used for direct displaying and
        /// for pixel-by-pixel image comparison.
        /// </summary>
        /// <param name="canvas">Canvas with elements to paint.</param>
        /// <returns>The painted bitmap</returns>
        public static Bitmap Paint(ICanvas canvas)
        {
            var image = new Bitmap(
                canvas.Environment.CanvasWidth, 
                canvas.Environment.CanvasHeight);

            using (var gfx = Graphics.FromImage(image))
            {
                gfx.CompositingMode = CompositingMode.SourceOver;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                foreach (var element in canvas.Elements)
                {
                    Paint(element, gfx);
                }
            }
            // Turn blurring on or off
            bool blurring = false;

            Bitmap newImage = image;
            if (blurring)
            {
                // Create a filter
                AForge.Imaging.Filters.Blur filter = new AForge.Imaging.Filters.Blur();
                // Apply the filter
                newImage = filter.Apply(image);
            }
            return newImage;
        }

        /// <summary>
        /// Paint a shape given the graphics handler for the element.
        /// </summary>
        /// <param name="element">Shape to draw.</param>
        /// <param name="gfx">Graphics handler.</param>
        private static void Paint(IShape element, Graphics gfx)
        {
            // Check if shape is a polygon
            var polygon = (IPolygon)element;
            if (polygon == null)
            {
                throw new NotImplementedException(
                    "Only polygon shapes are implemented at this time");
            }

            // Paint polygon shape
            Paint(polygon, gfx);
        }

        /// <summary>
        /// Paint a polygon given the graphics handler for the element.
        /// </summary>
        /// <param name="element">Polygon shape to draw.</param>
        /// <param name="gfx">The graphics handle</param>
        private static void Paint(IPolygon element, Graphics gfx)
        {
            // Create a brush resource
            using (var brush = new SolidBrush(Color.FromArgb(
                element.Alpha,
                element.Red,
                element.Green,
                element.Blue)))
            {
                // Convert coordinates (tuple list => point array)
                var coordinates = element.Coordinates.Select(
                    tuple => new Point(tuple.Item1, tuple.Item2)).ToArray();

                // Draw polygon using the just created brush.
                gfx.FillPolygon(brush, coordinates);
            }
        }
    }
}
