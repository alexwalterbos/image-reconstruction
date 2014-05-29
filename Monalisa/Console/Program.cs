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
            Console.WriteLine("Hallo wereld!");
            var EA = new EvolutionaryAlgorithm();
            EA.Run(() => EA.StagnationCount > 50);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
