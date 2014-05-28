using org.monalisa.algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace org.monalisa.gui
{
    /// <summary>
    /// This is the true artist. Give him a canvas and some preperation time
    /// with his soon to be drawn elements and then let the Paint()ing commence!
    /// </summary>
    public class Painter
    {
        /// <summary>
        /// Canvas used to paint on
        /// </summary>
        protected Panel Canvas { get; set; }

        /// <summary>
        /// Assign a new painter. he needs a canvas to draw his paintings
        /// </summary>
        /// <param name="Canvas">The canvas the painter should draw on</param>
        public Painter(Panel Canvas)
        {
            this.Canvas = Canvas;
        }

        /// <summary>
        /// Prepare element for drawing. Adds the element to canvas but does
        /// not yet display it.
        /// </summary>
        /// <param name="element">Shape to draw</param>
        public void Prepare(IShape element)
        {
            // check if sape is a polygon
            var polygon = (IPolygon)element;
            if (polygon == null) throw new NotImplementedException("Only polygon shapes are implemented at this time");

            // Prepare polygon shape
            this.Prepare(polygon);
        }

        /// <summary>
        /// Prepare a polygon for drawing.
        /// </summary>
        /// <param name="element">Polygon shape to draw</param>
        protected void Prepare(IPolygon element)
        {
            var polygon = new System.Windows.Shapes.Polygon();
            polygon.Fill = new SolidColorBrush(Color.FromArgb(element.Alpha, element.Red, element.Green, element.Blue));
            foreach (var point in element.Coordinates) polygon.Points.Add(new Point(point.Item1, point.Item2));
            Canvas.Children.Add(polygon);
        }

        /// <summary>
        /// Start the painting and show the prepared elements on the canvas
        /// </summary>
        public void Paint()
        {
            Canvas.UpdateLayout();
        }

        /// <summary>
        /// Paint a complete canvas in one go
        /// </summary>
        /// <param name="canvas">canvas with elements to paint</param>
        public void Paint(ICanvas canvas)
        {
            foreach (var element in canvas.Elements) 
            {
                this.Prepare(element);
            }
            this.Paint();
        }
    }
}
