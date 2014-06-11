//-----------------------------------------------------------------------------
// <copyright file="EvolutionaryAlgorithm.cs" 
//            company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This is the algorithm with all it's settings
    /// </summary>
    public class EvolutionaryAlgorithm
    {
        /// <summary>
        /// Backing field for <see cref="SeedByteArray"/>.
        /// </summary>
        private byte[] seedByteArray;

        /// <summary>
        /// Stores the previous fitness, this is used to detect stagnation.
        /// </summary>
        private double previousFitness = 0;

        /// <summary>
        /// Population used by the algorithm.
        /// </summary>
        private List<ICanvas> population;

        /// <summary>
        /// Factory used for random initializations.
        /// </summary>
        private PolygonFactory factory;

        /// <summary>
        /// Random number generator used for evolutionary strategies.
        /// </summary>
        private Random rand;

        /// <summary>
        /// Token used for cancelation when running asynchronously
        /// </summary>
        private CancellationToken cancelationToken = CancellationToken.None;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="EvolutionaryAlgorithm"/> class. After creation, it 
        /// should be run by using it's Run() or RunAsync() function.
        /// </summary>
        public EvolutionaryAlgorithm()
        {
            rand = new Random();
            factory = new PolygonFactory(this, rand);
        }

        /// <summary>
        /// Occurs when an epoch is completed.
        /// </summary>
        public event EventHandler<AlgorithmEvent> EpochCompleted;

        /// <summary>
        /// Occurs when the algorithm starts. When run is called.
        /// </summary>
        public event EventHandler<AlgorithmEvent> AlgorithmStarted;

        /// <summary>
        /// Occurs when the algorithm is completed. When run is finished.
        /// </summary>
        public event EventHandler<AlgorithmEvent> AlgorithmCompleted;

        /// <summary>
        /// Gets or sets the width of canvas.
        /// </summary>
        public int CanvasWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of canvas.
        /// </summary>
        public int CanvasHeight { get; set; }

        /// <summary>
        /// Gets or sets the number of individuals in one population.
        /// </summary>
        public int CanvasCount { get; set; }

        /// <summary>
        /// Gets or sets the number of polygons in one canvas.
        /// </summary>
        public int PolygonCount { get; set; }

        /// <summary>
        /// Gets or sets the number of edges in one polygon.
        /// i.e. 3 => triangle.
        /// </summary>
        public int PolygonEdgeCount { get; set; }

        /// <summary>
        /// Gets or sets the image to recreate.
        /// </summary>
        public Bitmap Seed { get; set; }

        /// <summary>
        /// Gets the image to recreate as byte array.
        /// </summary>
        public byte[] SeedByteArray
        {
            get
            {
                return seedByteArray ?? (seedByteArray = Seed.AsByteArray());
            }
        }

        /// <summary>
        /// Gets or sets the number of runs currently done.
        /// </summary>
        public int Epoch { get; protected set; }

        /// <summary>
        /// Gets or sets the number of epochs the fittest individual has not 
        /// changed.
        /// </summary>
        public int StagnationCount { get; protected set; }

        /// <summary>
        /// Gets the fitness of the fittest individual.
        /// </summary>
        public double Fitness
        {
            get { return population.CalculateFittest().Fitness; }
        }

        /// <summary>
        /// Gets the time the algorithm has currently ran.
        /// </summary>
        public TimeSpan TimeRan
        {
            get
            {
                if (!TimeStarted.HasValue)
                {
                    return TimeSpan.Zero;
                }
                else if (!TimeStopped.HasValue)
                {
                    return DateTime.Now - TimeStarted.Value;
                }
                else
                {
                    return TimeStopped.Value - TimeStarted.Value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time the algorithm started.
        /// </summary>
        public DateTime? TimeStarted { get; protected set; }

        /// <summary>
        /// Gets or sets the time the algorithm was finished.
        /// </summary>
        public DateTime? TimeStopped { get; protected set; }

        /// <summary>
        /// Gets the current population of this algorithm.
        /// </summary>
        public List<ICanvas> Population
        {
            get { return population; }
            set { population = value; }
        }

        /// <summary>
        /// Gets or sets the relative chance the position will be mutated.
        /// </summary>
        public double WeightPositionChange { get; set; }

        /// <summary>
        /// Gets or sets the relative chance the color will be mutated.
        /// </summary>
        public double WeightColorChange { get; set; }

        /// <summary>
        /// Gets or sets the relative chance order of drawing shapes will be 
        /// changed.
        /// </summary>
        public double WeightIndexChange { get; set; }

        /// <summary>
        /// Gets or sets the relative chance a random canvas will be generated.
        /// </summary>
        public double WeightRandomChange { get; set; }

        /// <summary>
        /// Gets the total weight.
        /// </summary>
        public double WeightTotal
        {
            get
            {
                return WeightColorChange
                     + WeightIndexChange
                     + WeightPositionChange
                     + WeightRandomChange;
            }
        }

        /// <summary>
        /// Runs the algorithm asynchronous
        /// </summary>
        /// <param name="stopCondition">
        ///     When this function signals true, the algorithm is completed.
        /// </param>
        /// <param name="token">Cancellation token for aborting.</param>
        /// <returns>Task for awaiting</returns>
        public async Task RunAsync(
            Func<bool> stopCondition, CancellationToken token)
        {
            cancelationToken = token;
            await Task.Run(() => Run(stopCondition), token);
        }

        /// <summary>
        /// Run the actual algorithm
        /// </summary>
        /// <param name="stopCondition">When to stop algorithm</param>
        public void Run(Func<bool> stopCondition)
        {
            // Call algorithm start event
            if (AlgorithmStarted != null)
            {
                AlgorithmStarted(this, new AlgorithmEvent(this));
            }

            // start timer
            this.TimeStarted = DateTime.Now;

            // Generate random intial population (if not set beforehand)
            if (this.population == null)
            {
                this.population = factory.RandomCanvases();
            }

            // while stopconditions is not met
            while (!stopCondition())
            {
                // Apply mutation on new indviduals
                var offspring = Mutate();

                // add to general population
                population.AddRange(offspring);

                // Kill off bottom
                population = CalculateFittest();

                // update itteration count
                Epoch++;

                // update stagnation count
                if (previousFitness == Fitness)
                {
                    StagnationCount++;
                }
                else
                {
                    StagnationCount = 0;
                }

                // Call epoch done event
                if (EpochCompleted != null)
                {
                    EpochCompleted(this, new AlgorithmEvent(this));
                }

                // update previous
                previousFitness = Fitness;

                ProcessStatistics(previousFitness);

                cancelationToken.ThrowIfCancellationRequested();
            }

            // stop timer
            TimeStopped = DateTime.Now;

            // Call algorithm done event
            if (AlgorithmCompleted != null)
            {
                AlgorithmCompleted(this, new AlgorithmEvent(this));
            }
        }

        private void ProcessStatistics(double previousFitness)
        {
            string filePath = "saves/statistics.csv";
            string delimiter = ",";

            string[] output = new string[] { Epoch.ToString(), TimeRan.ToString(), previousFitness.ToString() };

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join(delimiter, output));

            File.AppendAllText(filePath, sb.ToString());
        }

        /// <summary>
        /// Mutates candidates with a certain chance, 
        /// or mutates complete population if no candidates are given
        /// </summary>
        /// <param name="candidates">individuals considered</param>
        /// <returns>List of possibly mutated individuals</returns>
        protected List<ICanvas> Mutate(List<ICanvas> candidates = null)
        {
            // roll dice for each individual if selected, mutate
            List<ICanvas> newCanvases;
            if (candidates != null)
            {
                newCanvases = candidates.Select(c => c.Clone()).ToList();
            }
            else
            {
                newCanvases = this.population.Select(c => c.Clone()).ToList();
            }

            foreach (var canvas in newCanvases)
            {
                var dice = this.rand.NextDouble();
                if (dice < this.WeightColorChange / this.WeightTotal)
                {
                    var i = this.rand.Next(this.PolygonCount);
                    canvas.Elements[i] = this.MutateColor(canvas.Elements[i]);
                    continue;
                }

                dice = this.rand.NextDouble();
                if (dice < this.WeightPositionChange / this.WeightTotal)
                {
                    var i = this.rand.Next(this.PolygonCount);
                    canvas.Elements[i] = this.MutatePosition(canvas.Elements[i]);
                    continue;
                }

                dice = this.rand.NextDouble();
                if (dice < this.WeightIndexChange / this.WeightTotal)
                {
                    var i = this.rand.Next(this.PolygonCount);
                    var elem = canvas.Elements[i];
                    canvas.Elements.Remove(elem);
                    canvas.Elements.Insert(0, elem);
                    continue;
                }

                dice = this.rand.NextDouble();
                if (dice < this.WeightRandomChange / this.WeightTotal)
                {
                    var i = this.rand.Next(this.PolygonCount);
                    canvas.Elements.RemoveAt(i);
                    canvas.Elements.Insert(0, this.factory.RandomPolygon());
                }
            }

            return newCanvases;
        }

        /// <summary>
        /// Changes the color by some amount (default: ± 10).
        /// </summary>
        /// <param name="shape">The shape to change color of</param>
        /// <param name="delta">The maximum amount to change the value</param>
        /// <returns>The changed shape</returns>
        protected IShape MutateColor(IShape shape, byte delta = 10)
        {
            var p = shape.Clone() as Polygon;
            p.Alpha = p.Alpha.Change(delta, this.rand).Clip(0, 255);
            p.Red = p.Red.Change(delta, this.rand).Clip(0, 255);
            p.Green = p.Green.Change(delta, this.rand).Clip(0, 255);
            p.Blue = p.Blue.Change(delta, this.rand).Clip(0, 255);
            return p;
        }

        /// <summary>
        /// Mutate the coordinates position (all at the same time).
        /// default: ± 10 pixels.
        /// </summary>
        /// <param name="shape">The shape to change color of</param>
        /// <param name="delta">The maximum amount to change the value</param>
        /// <returns>The changed shape</returns>
        protected IShape MutatePosition(IShape shape, int delta = 10)
        {
            var polygon = shape.Clone() as Polygon;
            for (int i = 0; i < polygon.Coordinates.Count; i++)
            {
                var coord = polygon.Coordinates[i];
                var int1 = coord.Item1.Change(delta, this.rand).Clip(0, CanvasWidth);
                var int2 = coord.Item2.Change(delta, this.rand).Clip(0, CanvasHeight);
                polygon.Coordinates[i] = new Tuple<int, int>(int1, int2);
            }

            return polygon;
        }

        /// <summary>
        /// Get the fittest CanvasCount part of the population
        /// </summary>
        /// <returns>The fittest individuals</returns>
        protected List<ICanvas> CalculateFittest()
        {
            return population.SortByFitness().Take(this.CanvasCount).ToList();
        }

        /// <summary>
        /// Class containing event data for when
        /// <see cref="EvolutionaryAlgorithm"/> class is triggered.
        /// </summary>
        public class AlgorithmEvent : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AlgorithmEvent"/>
            /// class. Contains the algorithm itself by default.
            /// </summary>
            /// <param name="current">The algorithm for this event</param>
            public AlgorithmEvent(EvolutionaryAlgorithm current)
            {
                this.Current = current;
            }

            /// <summary>
            /// Gets or sets the current algorithm
            /// </summary>
            public EvolutionaryAlgorithm Current { get; protected set; }
        }
    }
}
