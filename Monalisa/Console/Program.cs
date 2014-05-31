//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.ConsoleUI
{
    using System;
    using Org.Monalisa.Algorithm;

    /// <summary>
    /// Run console program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point method
        /// </summary>
        /// <param name="args">Not used</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting algorithm...");
            var algorithm = new EvolutionaryAlgorithm();

            // subscribe to algorithm started (save seed image)
            algorithm.AlgorithmStarted += (s, e) => algorithm.Seed.Save("Seed.bmp");

            // subscribe to epoch done (print results each epoch)
            algorithm.EpochCompleted += (s, e) => PrintEpochResults(algorithm);

            // subscribe to algorithm done even (save result to bitmap)
            algorithm.AlgorithmCompleted += (s, e) => Painter.Paint(algorithm.Population.CalculateFittest()).Save("Fittest.bmp");

            algorithm.Run(() => algorithm.StagnationCount > 10);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Print results for a single epoch.
        /// </summary>
        /// <param name="algorithm">algorithm to print for</param>
        private static void PrintEpochResults(EvolutionaryAlgorithm algorithm)
        {
            // print fittest
            Console.Write("Epoch {0, -4}: {1,5:N5}", algorithm.Epoch, algorithm.Fitness);
            if (algorithm.StagnationCount > 0)
            {
                for (int i = 0; i < algorithm.StagnationCount; i++)
                {
                    Console.Write('*');
                }
            }

            Console.WriteLine();
        }
    }
}
