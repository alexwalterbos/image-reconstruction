using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// Canvases is what the population is made out of.
    /// Each canvas consists of a collection of shapes.
    /// </summary>
    public class Canvas : ICanvas
    {
        /// <summary>
        /// The enviroment in which this individual lives.
        /// </summary>
        protected EvolutionaryAlgorithm environment;

        /// <summary>
        /// Create a new canvas using the given environment.
        /// The created canvas is still empty (i.e. has no shapes in it yet).
        /// </summary>
        /// <param name="environment">The context for this individual.</param>
        public Canvas(EvolutionaryAlgorithm environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// This are the shapes that are drawn onto the canvas.
        /// </summary>
        public List<IShape> Elements { get; set; }

        // backing field
        public double? fitness;
        
        /// <summary>
        /// The fitness of this canvas in the given environment (given upon construction).
        /// Fitness values are between 0.0 and 1.0. Higher fitness is better.
        /// </summary>
        public double Fitness { get { return (fitness ?? (fitness = CalculateMeanSquaredSimilarity())).Value; } }

        /// <summary>
        /// Fitness method 1. Calculates the MSE pixel-by-pixel.
        /// </summary>
        private double CalculateMeanSquaredSimilarity()
        {
            ulong meanSquaredError = 0;
            ulong maxMeanSquaredError = ulong.MaxValue;
            using (var image = Painter.Paint(environment, this))
            {
                byte[] test = image.AsByteArray();
                byte[] seed = environment.Seed.AsByteArray();
                maxMeanSquaredError = (255UL*255UL)*(ulong)seed.Length;

                // use reversed loop for increased performance
                for (int i = seed.Length - 1; i >= 0; i--)
                {
                    ulong p1 = (ulong)seed[i];
                    ulong p2 = (ulong)test[i];
                    meanSquaredError += ((p1 - p2) * (p1 - p2));
                }
            }
            return 1D-(double)meanSquaredError/(double)maxMeanSquaredError;
        }

        /// <summary>
        /// Fitness method 2. Caculates how many pixel have exactly the same color.
        /// </summary>
        private double CalculateSimilarity()
        {
            double similar;
            using (var image = Painter.Paint(environment, this))
            {
                long similarityCount = 0L;
                byte[] test = image.AsByteArray();
                byte[] seed = environment.Seed.AsByteArray();
                for (int i = 0; i < test.Length; i++)
                    if (test[i] == seed[i])
                        similarityCount++;
                similar = similarityCount / (double)test.Length;
            }
            return similar;
        }

        public override string ToString()
        {
            return string.Join("\n", Elements.Select(p=>p.ToString()).ToArray());
        }
    }
}
