//-----------------------------------------------------------------------------
// <copyright file="Polygon.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A polygon is a shape with RGBA values. The shape is described by it's 
    /// corners coordinates.
    /// </summary>
    public class Polygon : IPolygon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        public Polygon()
        {
            Coordinates = new List<Tuple<int, int>>();
        }

        /// <summary>
        /// Gets or sets the endpoints of this polygon. Consists of a 
        /// collection of integer pairs. Order matters and should be in a 
        /// convex order.
        /// </summary>
        public List<Tuple<int, int>> Coordinates { get; set; }

        /// <summary>
        /// Gets or sets the transparency channel of this polygon's color.
        /// </summary>
        public byte Alpha { get; set; }

        /// <summary>
        /// Gets or sets the red color channel of this polygon.
        /// </summary>
        public byte Red { get; set; }

        /// <summary>
        /// Gets or sets the green color channel of this polygon.
        /// </summary>
        public byte Green { get; set; }

        /// <summary>
        /// Gets or sets the blue color channel of this polygon.
        /// </summary>
        public byte Blue { get; set; }

        /// <summary>
        /// Generates a string representation for this class
        /// </summary>
        /// <returns>The string representation</returns>
        public override string ToString()
        {
            return string.Format(
                "R: {0}, G: {1}, B: {2}, A: {3}, pts: {4}",
                Red,
                Green,
                Blue,
                Alpha,
                string.Join(", ", Coordinates.Select(p => p.ToString())));
        }

        /// <summary>
        /// Create a copy of the object. This object should not have any 
        /// reference to the original and should be save to alter.
        /// </summary>
        /// <returns>The copy</returns>
        public IShape Clone()
        {
            var coordinates = new Tuple<int, int>[this.Coordinates.Count];
            this.Coordinates.CopyTo(coordinates);

            return new Polygon()
            {
                Red = this.Red,
                Green = this.Green,
                Blue = this.Blue,
                Alpha = this.Alpha,
                Coordinates = coordinates.ToList()
            };
        }
    }
}
