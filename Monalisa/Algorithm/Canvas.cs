//-----------------------------------------------------------------------------
// <copyright file="Canvas.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Drawing;
    
    /// <summary>
    /// Canvases is what the population is made out of. Each canvas consists of 
    /// a collection of shapes.
    /// </summary>
    public class Canvas : ICanvas
    {
        /// <summary>
        /// Private backing field
        /// </summary>
        private double? fitness;

        /// <summary>
        /// Initializes a new instance of the <see cref="Canvas"/> class. It 
        /// used the given algorithm as environment to run in. The created 
        /// canvas is still empty (i.e. has no shapes in it yet).
        /// </summary>
        /// <param name="environment">The context for this individual.</param>
        public Canvas(EvolutionaryAlgorithm environment)
        {
            this.Environment = environment;
        }

        /// <summary>
        /// Gets or sets the environment in which this individual lives.
        /// </summary>
        public EvolutionaryAlgorithm Environment { get; protected set; }

        /// <summary>
        /// Gets or sets the shapes that are drawn onto the canvas.
        /// </summary>
        public List<IShape> Elements { get; set; }

        /// <summary>
        /// Gets the fitness of this canvas in the given environment 
        /// (given upon construction). Fitness values are between 0.0 and 1.0. 
        /// Higher fitness is better.
        /// </summary>
        public double Fitness
        {
            get
            {
                if (this.fitness == null)
                {
                    this.fitness = this.CalculateMeanSquaredSimilarity();
                }

                return this.fitness.Value;
            }
        }

        /// <summary>
        /// Returns a string representation of this canvas.
        /// </summary>
        /// <returns>The string representation</returns>
        public override string ToString()
        {
            var elementsAsString = this.Elements.Select(p => p.ToString());
            return string.Join("\n", elementsAsString.ToArray());
        }

        /// <summary>
        /// Creates a copy of this canvas.
        /// </summary>
        /// <returns>The copied canvas</returns>
        public ICanvas Clone()
        {
            return new Canvas(this.Environment)
            {
                Elements = this.Elements.Select(e => e.Clone()).ToList()
            };
        }

        /// <summary>
        /// Fitness method 1. 
        /// Calculates the MSE pixel-by-pixel.
        /// </summary>
        /// <returns>Fitness in interval [0.0, 1.0]. higher is better</returns>
        private double CalculateMeanSquaredSimilarity()
        {
            ulong meanSquaredError = 0;
            ulong maxMeanSquaredError = ulong.MaxValue;

            using (var image = Painter.Paint(this))
            {
                byte[] test = image.AsByteArray();
                byte[] seed = this.Environment.Seed.AsByteArray();
                maxMeanSquaredError = (255UL * 255UL) * (ulong)seed.Length;

                // use reversed loop for increased performance
                for (int i = seed.Length - 1; i >= 0; i--)
                {
                    ulong p1 = (ulong)seed[i];
                    ulong p2 = (ulong)test[i];
                    meanSquaredError += (p1 - p2) * (p1 - p2);
                }
            }

            return 1D - ((double)meanSquaredError / (double)maxMeanSquaredError);
        }

        /// <summary>
        /// Fitness method 2.
        /// Calculates how many pixel have exactly the same color.
        /// </summary>
        /// <returns>Fitness in interval [0.0, 1.0]. higher is better</returns>
        private double CalculateSimilarity()
        {
            double similar;
            using (var image = Painter.Paint(this))
            {
                long similarityCount = 0L;
                byte[] test = image.AsByteArray();
                byte[] seed = this.Environment.Seed.AsByteArray();
                for (int i = 0; i < test.Length; i++)
                {
                    if (test[i] == seed[i])
                    {
                        similarityCount++;
                    }
                }

                similar = similarityCount / (double)test.Length;
            }

            return similar;
        }
    }
}
