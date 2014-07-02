//-----------------------------------------------------------------------------
// <copyright file="MathHelper.cs" company="Delft University of Technology">
//  <a href="http://en.wikipedia.org/wiki/MIT_License">MIT License</a>
// </copyright>
//-----------------------------------------------------------------------------

namespace Org.Monalisa.Algorithm
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Helper for math related functions.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Calculates the long sum of a source using a selector function.
        /// </summary>
        /// <typeparam name="TSource">Type to convert</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="selector">Selector function</param>
        /// <returns>Sum of elements selected</returns>
        public static ulong Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, ulong> selector)
        {
            var sum = 0UL;
            foreach (var element in source)
            {
                sum += selector(element);
            }

            return sum;
        }

        /// <summary>
        /// Clips the lower and upper bound of a value.
        /// </summary>
        /// <param name="val">Value to clip</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <returns>Clipped value</returns>
        public static int Clip(this int val, int lower, int upper)
        {
            return Math.Max(lower, Math.Min(upper, val));
        }

        /// <summary>
        /// Clips the lower and upper bound of a value.
        /// </summary>
        /// <param name="val">Value to clip</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <returns>Clipped value</returns>
        public static byte Clip(this byte val, byte lower, byte upper)
        {
            return Math.Max(lower, Math.Min(upper, val));
        }

        /// <summary>
        /// Changes the value up or down by delta.
        /// </summary>
        /// <param name="val">Value to change</param>
        /// <param name="delta">Amount to change</param>
        /// <param name="r">Random number generator</param>
        /// <returns>Modified value</returns>
        public static int Change(this int val, int delta, Random r = null)
        {
            r = r ?? new Random();
            return (byte)((int)val + r.Next(-delta, delta + 1));
        }

        /// <summary>
        /// Changes the value up or down by delta.
        /// </summary>
        /// <param name="val">Value to change</param>
        /// <param name="delta">Amount to change</param>
        /// <param name="r">Random number generator</param>
        /// <returns>Modified value</returns>
        public static byte Change(this byte val, byte delta, Random r = null)
        {
            r = r ?? new Random();
            return (byte)(val + r.Next(-delta, delta + 1));
        }

        /// <summary>
        /// Generates a random unsigned long
        /// </summary>
        /// <param name="r">Random number generator</param>
        /// <returns>Randomly generated unsigned long</returns>
        public static ulong NextULong(this Random r)
        {
            var buffer = new byte[8];
            r.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Checks if polygon is concave, works for squares and pentagons
        /// </summary>
        /// <param name="shape">Polygon</param>
        /// <returns>True if concave, else False</returns>
        public static bool IsConvex(this Polygon shape)
        {
            var polygon = shape.Clone() as Polygon;
            int numVertices = polygon.Coordinates.Count;
            double curDetValue;

            for (int i = 0; i < numVertices; i++)
            {
                var coord = polygon.Coordinates[i];
                var int1 = coord.Item1;
                var int2 = coord.Item2;
                polygon.Coordinates[i] = new Tuple<int, int>(int1, int2);
                
            }

            // There are more than 4 vertices, so also check for seperate parts. This works only for pentagons
            if (numVertices > 4 && numVertices < 7)
            {
                Polygon polyFirst = new Polygon();
                Polygon polySec = new Polygon();

                // Split the polygon up in two parts and check individual concavity
                for (int i = 0; i < 4; i++)
                {
                    polyFirst.Coordinates.Add(polygon.Coordinates[i]);
                }

                for (int i = numVertices-4; i < (numVertices); i++)
                {
                    polySec.Coordinates.Add(polygon.Coordinates[i]);
                }
                
                // Both polygons should be concave
                if (!(polyFirst.IsConvex() && polySec.IsConvex())){
                    return false;
                }
            }

            if (numVertices < 3)
            {
                return true;
            }

            Tuple<int, int> v1 = CalcVector(polygon.Coordinates[0], polygon.Coordinates[numVertices - 1]);
            Tuple<int, int> v2 = CalcVector(polygon.Coordinates[1], polygon.Coordinates[0]);
            double detValue = CalcDet(v1, v2);

            for (int i = 1; i<numVertices-1; i++){
                v1 = v2;
                v2 = CalcVector(polygon.Coordinates[i+1],polygon.Coordinates[i]);
                curDetValue = CalcDet(v1,v2);

                if( (curDetValue * detValue) < 0.0)
                {
                    return false;
                }
            }

            v1 = v2;
            v2 = CalcVector(polygon.Coordinates[0], polygon.Coordinates[numVertices - 1]);
            curDetValue = CalcDet(v1, v2);

            if ( (curDetValue * detValue) < 0.0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Calculate the difference vector
        public static Tuple<int,int> CalcVector(Tuple<int,int> v1, Tuple<int,int> v2)
        {
            var vector = new Tuple<int,int>(v1.Item1 - v2.Item1, v1.Item2 - v2.Item2);
            return vector;
        }

        public static double CalcDet(Tuple<int,int> v1, Tuple<int,int> v2)
        {
            return (v1.Item1 * v2.Item2 - v1.Item2 * v2.Item1);

        }
    
    }
}
