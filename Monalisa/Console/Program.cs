using org.monalisa.algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting algorithm...");
            var EA = new EvolutionaryAlgorithm();

            // subscribe to algorithm started (save seed image)
            EA.AlgorithmStarted += (s, e) => EA.Seed.Save("Seed.bmp");

            // subscribe to epoch done (print results each epoch)
            EA.EpochCompleted += (s,e) => PrintEpochResults(EA);

            // subscribe to algorithm done even (save result to bitmap)
            EA.AlgorithmCompleted += (s, e) => Painter.Paint(EA, EA.Population.CalculateFittest()).Save("Fittest.bmp");

            EA.Run(() => EA.StagnationCount > 10);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void PrintEpochResults(EvolutionaryAlgorithm EA)
        {
            // print fittest
            Console.Write("Epoch {0, -4}: {1,5:N5}", EA.Epoch, EA.Fitness);
            if (EA.StagnationCount > 0) for (int i = 0; i < EA.StagnationCount; i++) Console.Write('*');
            Console.WriteLine();
        }
    }
}
