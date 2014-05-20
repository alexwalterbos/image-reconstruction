using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// Example of how a polygon interface could be implemented. This class 
    /// exists purely for demonstration purposes and should be removed once
    /// actual implementations exists
    /// </summary>
    public class TriangleTest : IPolygon
    {
        /// <summary>
        /// Three hardcoded coordinate points.
        /// </summary>
        public ICollection<Tuple<int, int>> Coordinates
        {
            get
            {
                var coordinates = new List<Tuple<int, int>>(3);
                coordinates.Add(new Tuple<int, int>(0, 0));
                coordinates.Add(new Tuple<int, int>(200, 0));
                coordinates.Add(new Tuple<int, int>(0, 200));
                return coordinates;
            }
        }

        /// <summary>
        /// Harcoded alpha channel (100% opaque)
        /// </summary>
        public byte Alpha { get { return 0xFF; } }

        /// <summary>
        /// Hardcoded red channel
        /// </summary>
        public byte Red { get { return 0xFF; } }

        /// <summary>
        /// Hardcoded green channel
        /// </summary>
        public byte Green { get { return 0x00; } }

        /// <summary>
        /// Hardcoded blue channel
        /// </summary>
        public byte Blue { get { return 0xFF; } }
    }
}
