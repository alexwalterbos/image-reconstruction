using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// A polygon is a shape with RGBA values.
    /// The shape is described by it's corners coordinates.
    /// </summary>
    public class Polygon : IPolygon
    {
        /// <summary>
        /// Create a new polygon.
        /// </summary>
        public Polygon()
        {
            Coordinates = new List<Tuple<int, int>>();
        }

        /// <summary>
        /// Endpoints of this polygon. Consists of a collection of integer 
        /// pairs. Order matters and should be in a convex order.
        /// </summary>
        public List<Tuple<int, int>> Coordinates { get; set; }

        /// <summary>
        /// Transparency channel of this polygon's color.
        /// </summary>
        public byte Alpha { get; set; }


        /// <summary>
        /// Red color channel of this polygon.
        /// </summary>
        public byte Red { get; set; }

        /// <summary>
        /// Green color channel of this polygon.
        /// </summary>
        public byte Green { get; set; }

        /// <summary>
        /// Blue color channel of this polygon.
        /// </summary>
        public byte Blue { get; set; }
    }
}
