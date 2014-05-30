using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// Our algorithms canvas. This is not the gui canvas but the one of which
    /// our evolutionary algorihm population consists.
    /// </summary>
    public class EvolutionaryAlgorithm
    {
        public event EventHandler<AlgorithmEvent> EpochCompleted;
        public event EventHandler<AlgorithmEvent> FitterFound;
        public event EventHandler<AlgorithmEvent> AlgorithmStarted;
        public event EventHandler<AlgorithmEvent> AlgorithmCompleted;

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

        /// <summary>
        /// Get the fitness of the fittest individual
        /// </summary>
        public double Fitness { get { return population.CalculateFittest().Fitness; } }
        private double previousFitness = 0;

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

        public ReadOnlyCollection<ICanvas> Population
        {
            get
            {
                return population.AsReadOnly();
            }
        }

        /// <summary>
        /// Relative chance the position will be mutated
        /// </summary>
        public double WeightPositionChange { get; set; }

        /// <summary>
        /// Relative chance the color will be mutated
        /// </summary>
        public double WeightColorChange { get; set; }

        /// <summary>
        /// Relative chance order of drawing shapes will be changed
        /// </summary>
        public double WeightIndexChange { get; set; }

        /// <summary>
        /// Relative chance a random canvas will be generated
        /// </summary>
        public double WeightRandomChange { get; set; }

        public double WeightTotal { get { return WeightColorChange + WeightIndexChange + WeightPositionChange + WeightRandomChange; } }


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
        }

        public async Task RunAsync(Func<Boolean> stopCondition, CancellationToken token)
        {
            ctoken = token;
            await Task.Run(() => Run(stopCondition), token);
        }

        private CancellationToken ctoken = CancellationToken.None;


        /// <summary>
        /// Run the actual algorithm
        /// </summary>
        /// <param name="stopCondition">When to stop algorithm</param>
        public void Run(Func<bool> stopCondition)
        {
            // Call algorithm start event
            if (AlgorithmStarted != null)
                AlgorithmStarted(this, new AlgorithmEvent(this));

            // start timer
            TimeStarted = DateTime.Now;

            // Generate random intial population
            population = factory.RandomCanvases();

            // while stopconditions is not met
            while (!stopCondition())
            {
                // Apply mutation on new indviduals
                var offspring = Mutate();

                // add to general population
                population.AddRange(offspring);

                // Kill off bottom
                ApplySurvivalOffTheFitest();

                // update itteration count
                Epoch++;

                // update stagnation count
                if (this.previousFitness == this.Fitness) StagnationCount++;
                else StagnationCount = 0;

                // Call epoch done event
                if (EpochCompleted != null)
                    EpochCompleted(this, new AlgorithmEvent(this));

                // update previous
                previousFitness = Fitness;

                ctoken.ThrowIfCancellationRequested();
            }

            // stop timer
            TimeStopped = DateTime.Now;

            // Call algorithm done event
            if (AlgorithmCompleted != null)
                AlgorithmCompleted(this, new AlgorithmEvent(this));
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
            var newCanvases = candidates != null ? candidates.Select(c => c.Clone()).ToList() : population.Select(c => c.Clone()).ToList();
            foreach (var canvas in newCanvases)
            {
                var dice = randomGenerator.NextDouble();
                if (dice < WeightColorChange / WeightTotal)
                {
                    canvas.Elements = canvas.Elements.Select(MutateColor).ToList();
                    continue;
                }

                dice = randomGenerator.NextDouble();
                if (dice < WeightPositionChange / WeightTotal)
                {
                    canvas.Elements = canvas.Elements.Select(MutatePosition).ToList();
                    continue;
                }

                dice = randomGenerator.NextDouble();
                if (dice < WeightIndexChange / WeightTotal)
                {
                    ShuffleZOrder(canvas);
                    continue;
                }

                dice = randomGenerator.NextDouble();
                if (dice < WeightRandomChange / WeightTotal)
                {
                    canvas.Elements = factory.RandomPolygons();
                }
            }
            return newCanvases;
        }

        protected IShape MutateColor(IShape shape)
        {
            var polygon = shape.Clone() as Polygon;
            polygon.Alpha = (byte)Math.Max(0, Math.Min(255, ((int)polygon.Alpha + randomGenerator.Next(-10, 11))));
            polygon.Red = (byte)Math.Max(0, Math.Min(255, ((int)polygon.Red + randomGenerator.Next(-10, 11))));
            polygon.Green = (byte)Math.Max(0, Math.Min(255, ((int)polygon.Green + randomGenerator.Next(-10, 11))));
            polygon.Blue = (byte)Math.Max(0, Math.Min(255, ((int)polygon.Blue + randomGenerator.Next(-10, 11))));
            return polygon;
        }

        protected IShape MutatePosition(IShape shape)
        {
            var polygon = shape.Clone() as Polygon;
            for (int i = 0; i < polygon.Coordinates.Count; i++)
            {

                var int1 = Math.Max(0, Math.Min(CanvasWidth, polygon.Coordinates[i].Item1 + randomGenerator.Next(-10, 11)));
                var int2 = Math.Max(0, Math.Min(CanvasHeight, polygon.Coordinates[i].Item2 + randomGenerator.Next(-10, 11)));
                polygon.Coordinates[i] = new Tuple<int, int>(int1, int2);
            }
            return polygon;
        }

        protected void ShuffleZOrder(ICanvas canvas)
        {
            int n = canvas.Elements.Count;
            while (n > 1)
            {
                n--;
                int k = randomGenerator.Next(n + 1);
                var value = canvas.Elements[k];
                canvas.Elements[k] = canvas.Elements[n];
                canvas.Elements[n] = value;
            }
        }

        /// <summary>
        /// Take first 50 of old population as new population
        /// </summary>
        protected void ApplySurvivalOffTheFitest()
        {
            population = population.SortByFitness().Take(this.CanvasCount).ToList();
        }

        public class AlgorithmEvent : EventArgs
        {
            public EvolutionaryAlgorithm Current { get; protected set; }

            public AlgorithmEvent(EvolutionaryAlgorithm current)
            {
                Current = current;
            }
        }
    }
}
