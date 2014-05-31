//-----------------------------------------------------------------------------
// <copyright file="IPolygon.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A contract interface for all polygons. By implementing this interface
    /// the polygon is also contracted for drawing by the 
    /// <see cref="Org.Monalisa.gui.Painter"/>. 
    /// </summary>
    public interface IPolygon : IShape
    {
        /// <summary>
        /// Gets the endpoints of this polygon. 
        /// Consists of a collection of integer pairs. Order matters.
        /// </summary>
        List<Tuple<int, int>> Coordinates { get; }

        /// <summary>
        /// Gets the transparency channel of this polygon's color.
        /// </summary>
        byte Alpha { get; }

        /// <summary>
        /// Gets the red color channel of this polygon.
        /// </summary>
        byte Red { get; }

        /// <summary>
        /// Gets the green color channel of this polygon.
        /// </summary>
        byte Green { get; }

        /// <summary>
        /// Gets the blue color channel of this polygon.
        /// </summary>
        byte Blue { get; }
    }
}
