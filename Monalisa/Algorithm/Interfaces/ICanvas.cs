//-----------------------------------------------------------------------------
// <copyright file="ICanvas.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Our algorithms canvas. This is not the GUI canvas but the one of which
    /// our evolutionary algorithm population consists.
    /// </summary>
    public interface ICanvas : ICloneable<ICanvas>
    {
        /// <summary>
        /// Gets or sets the shapes that are drawn onto the canvas.
        /// </summary>
        List<IShape> Elements { get; set; }

        /// <summary>
        /// Gets the environment this individual lives in.
        /// </summary>
        EvolutionaryAlgorithm Environment { get; }

        /// <summary>
        /// Gets the fitness of this canvas.
        /// Fitness values are between 0.0 and 1.0. Higher fitness is better.
        /// </summary>
        double Fitness { get; }
    }
}
