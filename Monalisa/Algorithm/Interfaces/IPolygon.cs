using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// A contract interface for all polygons. By implementing this interface
    /// the polygon is also contracted for drawing by the 
    /// <see cref="org.monalisa.gui.Painter"/>. 
    /// </summary>
    public interface IPolygon : IShape
    {
        /// <summary>
        /// Endpoints of this polygon. Consists of a collection of integer 
        /// pairs.
        /// </summary>
        ICollection<Tuple<int,int>> Coordinates { get; }
        
        /// <summary>
        /// Transparency channel of this polygon's color
        /// </summary>
        byte Alpha { get; }

        /// <summary>
        /// Red color channel of this polygon
        /// </summary>
        byte Red { get; }

        /// <summary>
        /// Green color channel of this polygon
        /// </summary>
        byte Green { get; }

        /// <summary>
        /// Blue color channel of this polygon
        /// </summary>
        byte Blue { get; }
    }
}
