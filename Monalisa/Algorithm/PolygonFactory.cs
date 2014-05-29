using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    /// <summary>
    /// A factory for the generation of random polygons and canvases.
    /// The factory is created within a specific environment which is used
    /// to figure out the configuration settings.
    /// </summary>
    public class PolygonFactory
    {
        // The random number generator used by this factory
        private Random r;

        // the environment for this factory
        private EvolutionaryAlgorithm environment;

        /// <summary>
        /// Create a new factory for the given environment.
        /// </summary>
        /// <param name="environment">Used for finding out configuration.</param>
        /// <param name="r">Custom random number generator.</param>
        public PolygonFactory(EvolutionaryAlgorithm environment, Random r = null)
        {
            this.environment = environment;

            // assign given generator to r or default if null 
            this.r = r ?? new Random();            
        }

        /// <summary>
        /// Create <paramref name="amount"/> of canvases.
        /// if no amount is geven, it generates according to the number
        /// of canvases in the environment.
        /// </summary>
        /// <param name="amount">Number of canvases to generate.</param>
        /// <returns>A list of generated canvases.</returns>
        public List<ICanvas> RandomCanvases(int? amount = null)
        {
            var canvasList = new List<ICanvas>();
            for (int i = 0; i < (amount??environment.CanvasCount); i++) canvasList.Add(RandomCanvas());
            return canvasList;
        }

        /// <summary>
        /// Generate a single random canvas.
        /// </summary>
        /// <returns>The generated canvas.</returns>
        public ICanvas RandomCanvas()
        {
            return new Canvas(environment) { Elements = RandomPolygons() };
        }

        /// <summary>
        /// Generate <paramref name="amount"/> of polygons.
        /// if no amount is given, generates the default amount for canvas
        /// given by the environment.
        /// </summary>
        /// <param name="amount">Number of polygons to generate.</param>
        /// <returns>A list of generated polygons.</returns>
        public List<IShape> RandomPolygons(int? amount = null)
        {
            var polygonList = new List<IShape>(amount??environment.PolygonCount);
            for (int i = 0; i < (amount??environment.PolygonCount); i++) polygonList.Add(RandomPolygon());
            return polygonList;
        }

        /// <summary>
        /// Generates a single random polygon.
        /// </summary>
        /// <returns>The generated polygon.</returns>
        public IShape RandomPolygon()
        {
            return new Polygon()
            {
                Coordinates = RandomTuples(),
                Alpha = (byte)r.Next(255),
                Red = (byte)r.Next(255),
                Green = (byte)r.Next(255),
                Blue = (byte)r.Next(255)
            };
        }

        /// <summary>
        /// Generate <paramref name="amount"/> of integer pairs.
        /// if no amount is given, generates the default amount for polygonEdgeCount
        /// given by the environment.
        /// </summary>
        /// <param name="amount">Number of pairs to generate.</param>
        /// <returns>A list of generated pairs.</returns>
        public List<Tuple<int, int>> RandomTuples(int? amount = null)
        {
            var tupleList = new List<Tuple<int, int>>(amount??environment.PolygonEdgeCount);
            for (int i = 0; i < (amount??environment.PolygonEdgeCount); i++) tupleList.Add(RandomTuple());
            return tupleList;
        }        

        /// <summary>
        /// Generate a single random pair of integers.
        /// </summary>
        /// <returns>The generated pair of integers.</returns>
        public Tuple<int, int> RandomTuple()
        {
            return new Tuple<int, int>(r.Next(environment.CanvasWidth), r.Next(environment.CanvasHeight));
        }
    }
}
