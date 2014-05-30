using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// Our algorithms canvas. This is not the gui canvas but the one of which
    /// our evolutionary algorihm population consists.
    /// </summary>
    public interface ICanvas : ICloneable<ICanvas>
    {
        /// <summary>
        /// This are the shapes that are drawn onto the canvas.
        /// </summary>
        List<IShape> Elements { get; set; }

        /// <summary>
        /// The fitness of this canvas.Fitness values are between 0.0 and 1.0. Higher fitness is better.
        /// </summary>
        double Fitness { get; }
    }
}
