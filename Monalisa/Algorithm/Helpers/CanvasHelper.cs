//-----------------------------------------------------------------------------
// <copyright file="CanvasHelper.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helper class for grouped canvas methods
    /// </summary>
    public static class CanvasHelper
    {
        /// <summary>
        /// Returns the fittest individual of the canvas.
        /// </summary>
        /// <param name="canvases">The collection of canvases</param>
        /// <returns>The fittest canvas according to canvas.Fitness</returns>
        public static ICanvas CalculateFittest(this ICollection<ICanvas> canvases)
        {
            var fittest = canvases.First();
            foreach (var canvas in canvases)
            {
                if (canvas.Fitness > fittest.Fitness)
                {
                    fittest = canvas;
                }
            }

            return fittest;
        }

        /// <summary>
        /// Returns an ordered list of canvases, sorted by Canvas.Fitness in 
        /// descending order.
        /// </summary>
        /// <param name="canvases">The collection of canvases</param>
        /// <returns>The canvases ordered by descending canvas.Fitness</returns>
        public static IList<ICanvas> SortByFitness(this ICollection<ICanvas> canvases)
        {
            return canvases.OrderByDescending(c => c.Fitness).ToList();
        }
    }
}
