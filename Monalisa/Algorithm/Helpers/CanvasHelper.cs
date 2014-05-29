using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    public static class CanvasHelper
    {
        // returns fittest individual
        public static ICanvas CalculateFittest(this ICollection<ICanvas> canvases)
        {
            var fittest = canvases.First();
            foreach (var canvas in canvases)
                if (canvas.Fitness > fittest.Fitness)
                    fittest = canvas;
            return fittest;
        }

        // returns ordered list of fitnesses
        public static IList<ICanvas> SortByFitness(this ICollection<ICanvas> canvases)
        {
            return canvases.OrderByDescending(c => c.Fitness).ToList();
        }

        public static Tuple<ICanvas, ICanvas> Reproduce(this Tuple<ICanvas, ICanvas> couple)
        {
            throw new NotImplementedException();
        }
    }
}
