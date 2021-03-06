﻿//-----------------------------------------------------------------------------
// <copyright file="PolygonFactory.cs" 
//            company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A factory for the generation of random polygons and canvases.
    /// The factory is created within a specific environment which is used
    /// to figure out the configuration settings.
    /// </summary>
    public class PolygonFactory
    {
        /// <summary>
        /// The random number generator used by this factory
        /// </summary>
        private Random r;

        /// <summary>
        /// the environment for this factory
        /// </summary>
        private EvolutionaryAlgorithm environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonFactory"/> 
        /// class. The environment provided is used for default values. 
        /// Optionally a custom random generator can be provided.
        /// </summary>
        /// <param name="env">The configuration.</param>
        /// <param name="r">Custom random number generator.</param>
        public PolygonFactory(EvolutionaryAlgorithm env, Random r = null)
        {
            this.environment = env;

            // Assign given generator to r or default if null 
            this.r = r ?? new Random();
        }

        /// <summary>
        /// Create <paramref name="amount"/> of canvases.
        /// if no amount is given, it generates according to the number
        /// of canvases in the environment.
        /// </summary>
        /// <param name="amount">Number of canvases to generate.</param>
        /// <returns>A list of generated canvases.</returns>
        public List<ICanvas> RandomCanvases(int? amount = null)
        {
            var canvasList = new List<ICanvas>();
            for (int i = 0; i < (amount ?? this.environment.CanvasCount); i++)
            {
                canvasList.Add(this.RandomCanvas());
            }

            return canvasList;
        }

        /// <summary>
        /// Generate a single random canvas.
        /// </summary>
        /// <returns>The generated canvas.</returns>
        public ICanvas RandomCanvas()
        {
            return new Canvas(this.environment) 
            { 
                Elements = this.RandomPolygons() 
            };
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
            int count = amount ?? this.environment.PolygonCount;
            var polygonList = new List<IShape>(count);
            for (int i = 0; i < count; i++)
            {
                polygonList.Add(this.RandomPolygon());
            }

            return polygonList;
        }

        /// <summary>
        /// Generates a single random polygon.
        /// </summary>
        /// <returns>The generated polygon.</returns>
        public IShape RandomPolygon()
        {
            var polygon = new Polygon();
            do
            {
                polygon = new Polygon()
                {
                    Coordinates = this.RandomTuples(),
                    Alpha = (byte)this.r.Next(255),
                    Red = (byte)this.r.Next(255),
                    Green = (byte)this.r.Next(255),
                    Blue = (byte)this.r.Next(255)
                };
            } while (!polygon.IsConvex());

            return polygon;
        }

        /// <summary>
        /// Generate <paramref name="amount"/> of integer pairs. If no amount 
        /// is given, generates the default amount for polygonEdgeCount given 
        /// by the environment.
        /// </summary>
        /// <param name="amount">Number of pairs to generate.</param>
        /// <returns>A list of generated pairs.</returns>
        public List<Tuple<int, int>> RandomTuples(int? amount = null)
        {
            int count = amount ?? this.environment.PolygonEdgeCount;
            var tuple1 = new Tuple<int,int> (0,0);
            var tuple2 = new Tuple<int,int> (0,0);
            var tuple3 = new Tuple<int,int> (0,0);
            var tupleList = new List<Tuple<int, int>>(count);

            for (int i = 0; i < count; i++)
            {
                tupleList.Add(RandomTuple(0, 0));
            }
                return tupleList;
        }

        /// <summary>
        /// Generate a single random pair of integers.
        /// </summary>
        /// <returns>The generated pair of integers.</returns>
        public Tuple<int, int> RandomTuple()
        {
            return new Tuple<int, int>(
                this.r.Next(this.environment.CanvasWidth), 
                this.r.Next(this.environment.CanvasHeight));
        }
        
        public Tuple<int, int> RandomTuple(int lowX, int lowY)
        {
            return new Tuple<int, int>(
                this.r.Next(lowX, this.environment.CanvasWidth),
                this.r.Next(lowY, this.environment.CanvasHeight));
        }
    }
}
