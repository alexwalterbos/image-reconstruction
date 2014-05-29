using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// Our algorithms canvas. This is not the gui canvas but the one of which
    /// our evolutionary algorihm population consists.
    /// </summary>
    public class EvolutionaryAlgorithm
    {
        /// <summary>
        /// Width of canvas.
        /// </summary>
        public int CanvasWidth { get; set; }

        /// <summary>
        /// Height of canvas
        /// </summary>
        public int CanvasHeight { get; set; }

        /// <summary>
        /// Number of individuals in one population
        /// </summary>
        public int CanvasCount { get; set; }

        /// <summary>
        /// Number of polygons in one canvas
        /// </summary>
        public int PolygonCount { get; set; }

        /// <summary>
        /// Number of edges in one polygon.
        /// i.e. 3 => triangle.
        /// </summary>
        public int PolygonEdgeCount { get; set; }

        /// <summary>
        /// Set the amount to be selected for reproduction
        /// example: CanvasCount         = 100
        ///          CrossoverPercentage = 0.5
        ///          => 50 pairs (100 offspring)
        /// </summary>
        public double CrossoverFactor { get; set; }

        /// <summary>
        /// Chance that considered individuals mutate.
        /// </summary>
        public double MutationChance { get; set; }

        /// <summary>
        /// Image to recreate.
        /// </summary>
        public Bitmap Seed { get; set; }

        // backing field
        private byte[] seedByteArray;

        /// <summary>
        /// Image to recreate as byte array.
        /// </summary>
        public byte[] SeedByteArray { get { return seedByteArray ?? (seedByteArray = Seed.AsByteArray()); } }

        /// <summary>
        /// Number of runs currently done
        /// </summary>
        public int Epoch { get; protected set; }

        /// <summary>
        /// Number of epochs the fittest individual has not changed
        /// </summary>
        public int StagnationCount { get; protected set; }
        private double previousFittest = 0;


        /// <summary>
        /// Time the algorithm has currently ran
        /// </summary>
        public TimeSpan TimeRan
        {
            get
            {
                if (!TimeStarted.HasValue) 
                    return TimeSpan.Zero;
                else if (!TimeStopped.HasValue)
                    return DateTime.Now - TimeStarted.Value;
                else
                    return TimeStopped.Value - TimeStarted.Value;
            }
        }

        /// <summary>
        /// Time the algorithm started
        /// </summary>
        public DateTime? TimeStarted { get; protected set; }

        /// <summary>
        /// Time the algorithm was finished
        /// </summary>
        public DateTime? TimeStopped { get; protected set; }

        // actual population used by algorithm
        private List<ICanvas> population;

        // factory used for random initializations
        private PolygonFactory factory;

        // random number generator used for evolutionary strategies
        private Random randomGenerator;

        /// <summary>
        /// Create a new Evolutionary algorithm.
        /// It can be run by using it's Run() function.
        /// </summary>
        public EvolutionaryAlgorithm()
        {
            randomGenerator = new Random();
            factory = new PolygonFactory(this, randomGenerator);
            CanvasWidth = 100;
            CanvasHeight = 100;
            CanvasCount = 200;
            PolygonCount = 300;
            PolygonEdgeCount = 3; // triangles
            CrossoverFactor = 0.25; // creates 25 pairs, 50 offspring
            MutationChance = 0.05;

            Seed = new Bitmap(CanvasWidth, CanvasHeight);
            using (Graphics gfx = Graphics.FromImage(Seed))
            {
                var brushR = new SolidBrush(Color.Red);
                var brushB = new SolidBrush(Color.Blue);
                var triangle1 = new Point[] { new Point(0, 0), new Point(100, 0), new Point(0, 100) };
                var triangle2 = new Point[] { new Point(100, 100), new Point(100, 0), new Point(0, 100) };
                gfx.FillPolygon(brushR, triangle1, System.Drawing.Drawing2D.FillMode.Alternate);
                gfx.FillPolygon(brushB, triangle2, System.Drawing.Drawing2D.FillMode.Alternate);
                brushR.Dispose();
                brushB.Dispose();
            }

            // goto {project_root}/Console/bin/{debug|release}/Seed.bmp to see seed image
            Seed.Save("Seed.bmp");
        }

        /// <summary>
        /// Run the actual algorithm
        /// </summary>
        /// <param name="stopCondition">When to stop algorithm</param>
        public void Run(Func<bool> stopCondition) 
        {
            // start timer
            TimeStarted = DateTime.Now;

            // Generate random intial population
            population = factory.RandomCanvases();

            // while stopconditions is not met
            while (!stopCondition())
            {
                // Apply crossover
                var offspring = Crossover(population);

                // Apply mutation on new indviduals
                offspring = Mutate(offspring);

                // add to general population
                population.AddRange(offspring);
                
                // Kill off bottom
                ApplySurvivalOffTheFitest();

                // update itteration count
                Epoch++;

                // update stagnation count
                if (previousFittest == population.CalculateFittest().Fitness)
                    StagnationCount++;
                else StagnationCount = 0;
                previousFittest = population.CalculateFittest().Fitness;
                
                // print fittest
                Console.Write("Epoch {0, -4}: {1,5:N5}", Epoch, population.CalculateFittest().Fitness);
                if (StagnationCount > 0) for (int i = 0; i < StagnationCount; i++) Console.Write('*');
                Console.WriteLine();
            }

            // stop timer
            TimeStopped = DateTime.Now;

            // output best result
            Painter.Paint(this, population.CalculateFittest()).Save("Fittest.bmp");
        }

        /// <summary>
        /// Apply crossover on candidates or complete population if no candidates are given
        /// </summary>
        /// <param name="candidates">candidates for crossover</param>
        /// <returns>Offspring</returns>
        protected List<ICanvas> Crossover(List<ICanvas> candidates = null)
        {
            // Select couples by doing a roulette wheel selection
            int amount = (int)Math.Floor(CanvasCount * CrossoverFactor);
            var selected = RouletteWheelSelect(amount, candidates ?? population);

            // recombine shapes in parent canvases to create offspring
            var offspring = new List<ICanvas>();
            foreach (var couple in selected)
            {
                var crossoverPoint = randomGenerator.Next(PolygonCount);
                List<IShape> elements1 = new List<IShape>(this.PolygonCount);
                List<IShape> elements2 = new List<IShape>(this.PolygonCount);
                elements1.AddRange(couple.Item1.Elements.Take(crossoverPoint));
                elements1.AddRange(couple.Item2.Elements.Skip(crossoverPoint));
                elements2.AddRange(couple.Item2.Elements.Take(crossoverPoint));
                elements2.AddRange(couple.Item1.Elements.Skip(crossoverPoint));
                offspring.Add(new Canvas(this) { Elements = elements1 });
                offspring.Add(new Canvas(this) { Elements = elements2 });
            }

            // return list of offspring
            return offspring;
        }

        /// <summary>
        /// Select couples using roulettewheel selection
        /// </summary>
        /// <param name="amount">number of couples to select</param>
        /// <param name="candidates">candidates to form couples out of</param>
        /// <returns>list of couples</returns>
        public List<Tuple<ICanvas, ICanvas>> RouletteWheelSelect(int amount, List<ICanvas> candidates = null)
        {
            // if no candidates are given, use entire population
            candidates = candidates ?? population;
            
            // sort by fitness  TODO: remove because it's costly and not necessary
            var sortedPopulation = candidates.SortByFitness();

            // generate total sum of fitness (used for normalization)
            var fitnessSum = candidates.Sum(c => c.Fitness);
            
            // create a accumelated and normalized reproduction chance list (example: fitness = [1, 2, 2] => accum[0.2, 0.6, 1.0])
            var accumulatedReproductionChance = sortedPopulation.Select(p => (double)p.Fitness / fitnessSum).ToList();
            for (int i = accumulatedReproductionChance.Count - 2; i >= 0; i--)
                accumulatedReproductionChance[i] += accumulatedReproductionChance[i + 1];

            // untill enough couples: spin the roulettewheel twice and see who is up all night to get lucky
            var couples = new List<Tuple<ICanvas, ICanvas>>(amount);
            while (couples.Count < amount)
            {
                var rand1 = randomGenerator.NextDouble();
                var rand2 = randomGenerator.NextDouble();
                ICanvas firstSelected = null;
                ICanvas secondSelected = null;

                // search for lucky couple
                for (int i = sortedPopulation.Count - 1; i >= 0; i--)
                {
                    if (firstSelected == null && accumulatedReproductionChance[i] >= rand1)
                        firstSelected = sortedPopulation[i];
                    if (secondSelected == null && accumulatedReproductionChance[i] >= rand2)
                        secondSelected = sortedPopulation[i];

                    // both individuals fount, stop searching for couple
                    if (firstSelected != null && secondSelected != null) break;                        
                }

                // only add if not accidently added same individual on both sides of the couple
                if (firstSelected != secondSelected) couples.Add(new Tuple<ICanvas, ICanvas>(firstSelected, secondSelected));
            }

            // return the newly coupled
            return couples;
        }

        /// <summary>
        /// Mutates candidates with a certain chance, 
        /// or mutates complete population if no candidates are given
        /// </summary>
        /// <param name="candidates">individuals considered for mutation</param>
        /// <returns>List of possibly mutated individuals</returns>
        protected List<ICanvas> Mutate(List<ICanvas> candidates = null)
        {
            // roll dice for each individual if selected, mutate
            var newCanvases = new List<ICanvas>(candidates.Count);
            foreach (var canvas in candidates ?? population)
            {
                if (randomGenerator.NextDouble() < MutationChance)
                {
                    List<IShape> elements = new List<IShape>(canvas.Elements);
                    List<IShape> mutated = new List<IShape>(canvas.Elements.Count);

                    // generate crossover point
                    var crossoverPoint = randomGenerator.Next(PolygonCount);
                    // generate direction
                    var upto = randomGenerator.NextDouble() > 0.5;

                    // regenerate everything from crossover point in certain direction
                    if (upto)
                    {
                        mutated.AddRange(elements.Take(crossoverPoint));
                        mutated.AddRange(factory.RandomPolygons(this.PolygonCount - crossoverPoint));
                    }
                    else
                    {
                        mutated.AddRange(factory.RandomPolygons(crossoverPoint));
                        mutated.AddRange(elements.Skip(crossoverPoint));
                    }
                }
                // not mutated, add original
                else
                {
                    newCanvases.Add(canvas);
                }
            }
            return newCanvases;
        }

        /// <summary>
        /// Take first 50 of old population as new population
        /// </summary>
        protected void ApplySurvivalOffTheFitest()
        {
            population = population.SortByFitness().Take(this.CanvasCount).ToList();
        }
    }
}
